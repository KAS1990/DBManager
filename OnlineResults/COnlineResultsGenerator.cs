using DBManager.Global;
using DBManager.OnlineDB;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.Scanning.XMLDataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace DBManager.OnlineResults
{
    public class COnlineResultsGenerator : IDisposable
    {
        private const int REQUEST_TIMEOUT_MS = 3000;

        private bool m_Disposed = false;
        private readonly object m_csTasksToExport = new object();
        private readonly Queue<CQueueItem> m_quTasksToExport = new Queue<CQueueItem>();
        private readonly OnlineDBManager m_DBManager = OnlineDBManager.Instance;
        private readonly Thread m_thExporter = null;
        private bool m_ThreadGo = false;
        private readonly ManualResetEvent m_evHasData = new ManualResetEvent(false);

        public int MaxQueueLength { get; set; }

        public bool IsStarted { get; private set; } = false;

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    StopThread();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                m_Disposed = true;
            }
        }


        public COnlineResultsGenerator()
        {
            m_thExporter = new Thread(m_thExporter_ThreadProc)
            {
                IsBackground = false,
            };
            MaxQueueLength = 1;

            m_ThreadGo = true;
            m_thExporter.Start();
        }


        ~COnlineResultsGenerator()
        {
            Dispose(false);
        }

        private void m_thExporter_ThreadProc()
        {
            while (m_ThreadGo)
            {
                m_evHasData.WaitOne();
                if (!m_ThreadGo)
                    break;

                CQueueItem Item = null;

                lock (m_csTasksToExport)
                {
                    if (m_quTasksToExport.Count == 0)
                    {
                        m_evHasData.Reset();
                        continue;
                    }

                    Item = m_quTasksToExport.Dequeue();
                }

                // Обработка полученного из очереди элемента
                HandleItem(Item);

                lock (m_csTasksToExport)
                {
                    if (m_quTasksToExport.Count == 0)
                        m_evHasData.Reset();
                }
            }

            m_evHasData.Reset();
        }


        /// <summary>
        /// Эту функцию нужно обязательно вызывать перед закрытием приложения.
        /// Без этого поток нормально не завершится
        /// </summary>
        private void StopThread()
        {
            Stop();
            m_ThreadGo = IsStarted = false;
            m_evHasData.Set();
            m_thExporter.Join();
        }


        public void Start()
        {
            ClearQueue();
            IsStarted = true;
        }


        public void Stop()
        {
            lock (m_csTasksToExport)
            {
                ClearQueue();
            }
            IsStarted = false;
        }


        /// <summary>
        /// Обработка 1 элемента
        /// </summary>
        /// <param name="Item"></param>
        public bool HandleItem(CQueueItem Item)
        {
            CLogItem LogItem = new CLogItem()
            {
                CreationDate = DateTime.Now,
                PCWbkName = Item.PCWbkFullPath
            };

            // Проверка соединения с удалённой БД
            if (!m_DBManager.IsConnectedToRemoteDB)
            {
                string msg = string.Format(DBManager.Properties.Resources.resrmtCantConnectToRemoteDB, m_DBManager.ConnectionString);
                MessageBox.Show(msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                LogItem.Type = enOnlineResultsLogItemType.Error;
                LogItem.Text = msg;
                AddItemToLog(LogItem, Item);
                return false;
            }

            if (Item.Round == enRounds.Total)
            {   // TO DO: пока сайт не поддерживает вывод итоговых протоколов
                return true;
            }

            string GroupFullNameToPublish = Item.CompSettings.AgeGroup.FullGroupName;
            string roundNameToPublish = GlobalDefines.ROUND_NAMES[(byte)Item.Round];

            DBManagerApp.MainWnd.PublishingNow = true;

            try
            {
                // Получаем список участников заданной группы и раунда в удалённой БД
                var RemoteDBResults = (from result in m_DBManager.Entities.results_speed
                                       where result.groups == GroupFullNameToPublish
                                       && result.round == roundNameToPublish
                                       select result)
                                      .ToList();

                // Получаем список участников в локальной БД
                var LocalDBResults = (from member in DBManagerApp.m_Entities.members
                                      join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
                                      join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
                                      where result.round == (byte)Item.Round && part.Group == Item.GroupId
                                      select new CMemberAndResults
                                      {
                                          MemberInfo = new CFullMemberInfo()
                                          {
                                              IDMember = member.id_member,
                                              Surname = member.surname,
                                              Name = member.name,
                                              YearOfBirth = member.year_of_birth,
                                              Coach = part.coach,
                                              Team = part.team,
                                              InitGrade = part.init_grade,
                                          },

                                          Results = new COneRoundResults()
                                          {
                                              m_Round = (enRounds)result.round,
                                              Route1 = new CResult()
                                              {
                                                  ResultInDB = result,
                                                  ResultColumnNumber = enResultColumnNumber.Route1,
                                                  Time = result.route1,
                                              },
                                              Route2 = new CResult()
                                              {
                                                  ResultInDB = result,
                                                  ResultColumnNumber = enResultColumnNumber.Route2,
                                                  Time = result.route2,
                                              },
                                              Sum = new CResult()
                                              {
                                                  ResultInDB = result,
                                                  ResultColumnNumber = enResultColumnNumber.Sum,
                                                  Time = result.sum,
                                              },
                                          },

                                          StartNumber = result.number,
                                          Place = result.place,
                                      })
                                     .ToList();
                var LocalDBResultsWithSum = LocalDBResults.Where(arg => arg.Results?.Sum?.Time != null);

                // В основном запросе заполнить эти поля почему-то не получилось
                foreach (CMemberAndResults item in LocalDBResults)
                {
                    if (Item.CompSettings.SecondColNameType == enSecondColNameType.Coach)
                        item.MemberInfo.SecondCol = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == item.MemberInfo.Coach).name;
                    else
                        item.MemberInfo.SecondCol = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == item.MemberInfo.Team).name;
                }

                // Сравниваем результаты и заносим их в удалённую БД
                // Обновляем результаты или добавляем новые
                foreach (var pair in (from localResult in LocalDBResults
                                      join remoteResult in RemoteDBResults on localResult.MemberInfo.IDMember equals remoteResult.local_member_id into remoteResults
                                      from remoteResult in remoteResults.DefaultIfEmpty()
                                      select new
                                      {
                                          remoteResult,
                                          localResult
                                      }))
                {
                    OnlineDB.Data.results_speed remoteResult = pair.remoteResult;

                    if (remoteResult == null)
                    {   // Нужно добавить новый результат в удалённую БД
                        remoteResult = new OnlineDB.Data.results_speed(pair.localResult, GroupFullNameToPublish, Item.Round);
                        m_DBManager.Entities.results_speed.Add(remoteResult);
                    }
                    else if (!remoteResult.IsEqualWithoutIdentificationProperties(pair.localResult))
                    {   // Нужно заменить результат в удалённой БД
                        remoteResult.UpdateFromLocalData(pair.localResult);
                    }

                    // Обновляем поле pass_to_next_round
                    switch (Item.Round)
                    {
                        case enRounds.Qualif:
                            remoteResult.pass_to_next_round = remoteResult.place.HasValue
                                                                && Item.CompSettings.MembersFrom1stQualif > 0
                                                                && remoteResult.place <= Item.CompSettings.MembersFrom1stQualif;
                            break;

                        case enRounds.Qualif2:
                            remoteResult.pass_to_next_round = remoteResult.place.HasValue
                                                                && Item.CompSettings.MembersFrom2ndQualif > 0
                                                                && remoteResult.place <= Item.CompSettings.MembersFrom2ndQualif;
                            break;

                        case enRounds.OneEighthFinal:
                        case enRounds.QuaterFinal:
                        case enRounds.SemiFinal:
                        case enRounds.Final:

                            if (remoteResult.sum.HasValue)
                            {
                                int pairNumber = (remoteResult.number - 1) / 2;
                                int firstMemberStartNumber = (pairNumber * 2) + 1;
                                int secondMemberStartNumber = firstMemberStartNumber + 1;

                                var firstMember = LocalDBResultsWithSum.FirstOrDefault(arg => arg.StartNumber == firstMemberStartNumber);
                                var secondMember = LocalDBResultsWithSum.FirstOrDefault(arg => arg.StartNumber == secondMemberStartNumber);

                                if (firstMember != null && secondMember != null)
                                {   // Оба участника из пары пробежали обе трассы
                                    if (firstMember.Results.Sum.Time < secondMember.Results.Sum.Time)
                                    {
                                        remoteResult.pass_to_next_round = remoteResult.number == firstMember.StartNumber;
                                    }
                                    else
                                    {
                                        remoteResult.pass_to_next_round = remoteResult.number == secondMember.StartNumber;
                                    }
                                }
                                else
                                    remoteResult.pass_to_next_round = false;
                            }
                            else
                                remoteResult.pass_to_next_round = false;
                            break;
                    }
                }
                m_DBManager.Entities.SaveChanges();

                // Удаляем из удалённой БД результаты, которых больше нет
                foreach (var remoteResult in (from remoteResult in RemoteDBResults
                                              join localResult in LocalDBResults on remoteResult.local_member_id equals localResult.MemberInfo.IDMember into localResults
                                              from localResult in localResults.DefaultIfEmpty()
                                              where localResult == null
                                              select remoteResult))
                {
                    m_DBManager.Entities.results_speed.Remove(remoteResult);
                }
                m_DBManager.Entities.SaveChanges();
            }
            catch (Exception ex)
            {
                LogItem.Type = enOnlineResultsLogItemType.Error;
                LogItem.Text = string.Format("Error in HandleItem:\n{0}", ex.Message);
                AddItemToLog(LogItem, Item);

                DBManagerApp.MainWnd.PublishingNow = false;
                return false;
            }

            // Запись прошла успешно => добавляем запись в лог
            LogItem.Type = enOnlineResultsLogItemType.OK;
            LogItem.Text = string.Format("Group \"{0}\" round \"{1}\" has been published", GroupFullNameToPublish, roundNameToPublish);
            AddItemToLog(LogItem, Item);

            DBManagerApp.MainWnd.PublishingNow = false;
            return true;
        }


        public bool AddItemToQueue(CQueueItem Item)
        {
            lock (m_csTasksToExport)
            {
                if (m_quTasksToExport.Count < MaxQueueLength)
                {
                    m_quTasksToExport.Enqueue(Item);
                    m_evHasData.Set();
                    return true;
                }
                else
                    return false;
            }
        }


        public void ClearQueue()
        {
            lock (m_csTasksToExport)
            {
                m_evHasData.Reset();
                m_quTasksToExport.Clear();
            }
        }

        private void AddItemToLog(CLogItem LogItem, CQueueItem Item)
        {
            GlobalDefines.CheckPublishingDirExists();

            string Dir = GlobalDefines.STD_PUBLISHING_LOG_DIR + Item.CompId.ToString() + "\\";
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);

            try
            {
                using (TextWriter tw = new StreamWriter(string.Format("{0}{1}\\{2}{3}",
                                                                    GlobalDefines.STD_PUBLISHING_LOG_DIR,
                                                                    Item.CompId,
                                                                    Item.GroupId,
                                                                    GlobalDefines.PUBLISHING_LOG_FILE_EXTENSION), true))
                {
                    tw.WriteLine(LogItem.ToLogFileString());
                }
            }
            catch (Exception ex)
            {
                ex.ToString(); // make compiler happy
            }
        }
    }
}
