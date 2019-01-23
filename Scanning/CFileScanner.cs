using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DBManager.Stuff;
using DBManager.Global;
using System.IO;
using DBManager.SettingsWriter;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.Scanning
{
	/// <summary>
	/// Поток, который ищет изменения в xml-файле, в который книга Excel копирует результаты соревнований 
	/// </summary>
	public class CFileScanner : CScannerBase
	{
		public class CSyncParam : CScannerBase.CSyncParamBase
		{
			public long m_GroupId = GlobalDefines.NO_OUR_COMP_IN_DB;

			/// <summary>
			/// Полный путь к файлу
			/// </summary>
			public string m_FullFilePath = GlobalDefines.DEFAULT_XML_STRING_VAL;

			public CSyncParam(long GroupId, string FullFilePath) :
				base()
			{
				m_GroupId = GroupId;
				m_FullFilePath = FullFilePath;
			}
		}


		CGroupResultsManager m_ResultsManager = null;


		#region Group
		/// <summary>
		/// Группа в БД, результаты участников которой экземпляр класса заносит в БД
		/// </summary>
		private groups m_Group = null;
		public groups Group
		{
			get { return m_Group; }
			private set { m_Group = value; }
		}
		#endregion


		#region ScanningPath
		/// <summary>
		/// Полный путь к сканируемому файлу
		/// </summary>
		public override string ScanningPath
		{
			get { return m_ScanningPath; }
			set
			{
				LastException = null;
				if (Group != null)
				{
					List<CDataChangedInfo> Changes = new List<CDataChangedInfo>();

					try
					{
						string PrevPath = Group.xml_file_name;
						Group.xml_file_name = value;
						DBManagerApp.m_Entities.SaveChanges();

						m_ScanningPath = value;

						Changes.Add(new CDataChangedInfo(this)
						{
							ChangedObjects = enDataChangedObjects.Paths,
							ChangingType = enDataChangesTypes.Changing,
							PrevVal = PrevPath,
							CurVal = value,
							GroupID = Group.id_group
						});
					}
					catch (Exception ex)
					{
						OnException(ref Changes, ex, Group == null ? GlobalDefines.DEFAULT_XML_INT_VAL : Group.id_group);
					}

					RaiseDataChangedEvent(new DataChangedEventArgs(Changes));
				}
				else
					m_ScanningPath = value;
			}
		}
		#endregion


		#region CompId
		long CompId
		{
			get { return (Parent as CDirScanner).CompId; }
			set { (Parent as CDirScanner).CompId = value; }
		}
		#endregion


		/// <summary>
		/// Сериализатор, который будет работать с XML-файлом
		/// </summary>
		CXMLDataSerializer m_XMLDataSer = new CXMLDataSerializer();

		
		#region DataFromXml
		CAllExcelData m_DataFromXml = null;
		public CAllExcelData DataFromXml
		{
			get { return m_DataFromXml; }
			private set { m_DataFromXml = value; }
		}
		#endregion


		public object DataSyncObj
		{
			get { return m_XMLDataSer.DataSyncObj; }
		}
						

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Path">
		/// Название файла с расширением
		/// </param>
		/// <param name="ParentScanner">
		/// 
		/// </param>
		/// <param name="Sync">
		/// Нужно ли выполнить синхронизацию объекта с БД и файлами, уже имеющимися в каталоге.
		/// При этом вызывается функция SyncWithFilesAndDB
		/// </param>
		/// <param name="SyncParam">
		/// 
		/// </param>
		public CFileScanner(string ScanningXmlFile,
							CScannerBase ParentScanner,
							bool Sync,
							CSyncParam SyncParam = null) :
			base(ScanningXmlFile, ParentScanner, Sync, SyncParam)
		{
			
		}


		/// <summary>
		/// Запустить сканирование
		/// </summary>
		/// <param name="ScanningXmlFile"></param>
		/// <param name="MadeChanges"></param>
		/// <returns></returns>
		public override bool Start(string ScanningXmlFile)
		{
			List<CDataChangedInfo> MadeChanges = new List<CDataChangedInfo>();

			if (State == enScanningThreadState.Worked)
				// Поток в данный момент работает
				return false;

			Group = null;

			if (!File.Exists(ScanningXmlFile))
				return false;

			try
			{
				if (CopyDataFromXMLFile2DB(ScanningXmlFile, out MadeChanges, false))
				{
					if (Group != null)
					{
						lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
						{
							if (DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.TryAddValue(ScanningPath, new CFileScannerSettings()
																														{
																															FullFilePath = ScanningPath,
																															GroupId = Group.id_group
																														})
								)
							{	// Информации об этом файле в настройках ещё нет
								DBManagerApp.m_AppSettings.Write();
							}
						}
					}

					State = enScanningThreadState.Worked;

					RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));

					return true;
				}
			}
			catch (Exception ex)
			{
				OnException(ref MadeChanges, ex, Group == null ? GlobalDefines.DEFAULT_XML_INT_VAL : Group.id_group);
				RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));
			}

			return false;
		}


		/// <summary>
		/// Остановить сканирование
		/// </summary>
		public override void Stop(bool OnRestart)
		{
			LastException = null;

			lock (EventsCS)	// Ждём, когда завершаться все события
			{
				if (State == enScanningThreadState.Worked)
				{
					// Свойство ScanningPath здесь использовать нельзя
					m_XMLDataSer.FullFilePath = m_ScanningPath = "";
					m_XMLDataSer.ClearData();
					Group = null;

					State = enScanningThreadState.Stopped;
				}
			}
		}


		/// <summary>
		/// Переписываем данные из файла в БД
		/// </summary>
		/// <param name="Param"></param>
		public override bool SyncWithFilesAndDB(CScannerBase.CSyncParamBase Param)
		{
			LastException = null;

			List<CDataChangedInfo> MadeChanges = new List<CDataChangedInfo>();

			lock (EventsCS)
			{
				if (State == enScanningThreadState.Worked)
				{	// Синхронизацию можно проводить только при незапущенном сканировании
					return false;
				}

				CSyncParam SyncParam = Param as CSyncParam;

				if (SyncParam == null ||
					SyncParam.m_FullFilePath == GlobalDefines.DEFAULT_XML_STRING_VAL ||
					!File.Exists(SyncParam.m_FullFilePath))
				{
					return false;
				}

				try
				{
					if (CopyDataFromXMLFile2DB(SyncParam.m_FullFilePath, out MadeChanges, true))
					{
						RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));
						return true;
					}
					else
					{	// Не удалось прочитать данные из файла
						m_XMLDataSer.FullFilePath = ScanningPath = "";
						m_XMLDataSer.ClearData();
						Group = null;
						DataFromXml = null;

						MadeChanges.Add(new CDataChangedInfo(this)
						{
							ChangedObjects = enDataChangedObjects.None,
							ChangingType = enDataChangesTypes.SyncFailed,
							Argument = Param,
						});
						RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));
					}
				}
				catch (Exception ex)
				{
					OnException(ref MadeChanges, ex, Group == null ? GlobalDefines.DEFAULT_XML_INT_VAL : Group.id_group);
					RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));
				}
			}

			return false;
		}


		public override void RaiseDataChangedEvent(DataChangedEventArgs e)
		{
			if (Parent != null && Parent is CDirScanner)
				Parent.RaiseDataChangedEvent(e);
		}


		/// <summary>
		/// Содержимое xml-файла изменилось
		/// </summary>
		/// <returns>
		/// Сделанные изменения
		/// </returns>
		public List<CDataChangedInfo> XMLFileChanged()
		{
			LastException = null;

			List<CDataChangedInfo> result = new List<CDataChangedInfo>();

			lock (EventsCS)	// Ждём, когда завершаться все события
			{
				if (State != enScanningThreadState.Worked)
					return null;

				lock (m_XMLDataSer.DataSyncObj)
				{
					if (!m_XMLDataSer.Read())
					{	// Если файл не удалось прочитать, то дальнейшие действия невозможны
						return null;
					}

					if (string.IsNullOrWhiteSpace(m_XMLDataSer.Data.Settings.CompName) ||
						string.IsNullOrWhiteSpace(m_XMLDataSer.Data.Settings.SecondColName) ||
						m_XMLDataSer.Data.Settings.AgeGroup == null ||
						string.IsNullOrWhiteSpace(m_XMLDataSer.Data.Settings.AgeGroup.Name) ||
						m_XMLDataSer.Data.Settings.StartDate == null ||
						m_XMLDataSer.Data.Settings.StartDate.Date == GlobalDefines.DEFAULT_XML_DATE_TIME_VAL)
					{	// В xml-файле нет данных 
						return null;
					}

					try
					{
						if (!m_XMLDataSer.Data.Settings.Equals(DataFromXml.Settings))
						{   /* Настройки соревнований изменились */
							if (!m_XMLDataSer.Data.Settings.DescriptionPropsEquals(DataFromXml.Settings))
							{	// Меняем данные в таблице descriptions
								IEnumerable<descriptions> DescsWithCompId = DBManagerApp.m_Entities.descriptions.Where(arg => arg.id_desc == CompId);
								descriptions Desc = null;
								if (DescsWithCompId.Count() > 0)
								{
									Desc = DescsWithCompId.First();
									CopyXmlToDescEntity(Desc, m_XMLDataSer.Data, Parent.ScanningPath);
								}
								else
								{	// Нужно добавить соревнование
									Desc = DescFromXml2Entity(Parent.ScanningPath, m_XMLDataSer.Data);
									if (Desc != null)
									{
										DBManagerApp.m_Entities.descriptions.Add(Desc);
										DBManagerApp.m_Entities.SaveChanges(); // Чтобы получить id_desc

										CompId = Desc.id_desc;
									}
									else
										return null;
								}

								result.Add(new CDataChangedInfo(this)
								{
									ChangingType = DescsWithCompId.Count() > 0 ? enDataChangesTypes.Changing : enDataChangesTypes.Add,
									ChangedObjects = enDataChangedObjects.CompSettings,
									ID = CompId,
									Argument = Desc,
								});
							}

							if (DataFromXml.Settings.EndDate != m_XMLDataSer.Data.Settings.EndDate ||
								!DataFromXml.Settings.RoundDatesEquals(m_XMLDataSer.Data.Settings))
							{
								if (DataFromXml.Settings.EndDate == null)
								{
									if (m_XMLDataSer.Data.Settings.EndDate != null)
									{	// Добавляем даты проведения раундов соревнований
										AddRoundsDates(m_XMLDataSer.Data.Settings.RoundDates, Group.id_group, false);
									}
								}
								else
								{
									// Нужно удалить даты, т.к. они изменились
									foreach (round_dates RoundDate in (from round_date in DBManagerApp.m_Entities.round_dates
																	  where round_date.Group == Group.id_group
																	  select round_date).ToList())
									{
										DBManagerApp.m_Entities.round_dates.Remove(RoundDate);
									}

									DBManagerApp.m_Entities.SaveChanges(); // Чтобы удаление применилось

									if (m_XMLDataSer.Data.Settings.EndDate != null &&
										m_XMLDataSer.Data.Settings.EndDate.Date != GlobalDefines.DEFAULT_XML_DATE_TIME_VAL)
									{
										AddRoundsDates(m_XMLDataSer.Data.Settings.RoundDates, Group.id_group, false);
									}
								}
							}

							if (Group == null)
							{
								Group = GroupFromXml2Entity(CompId, ScanningPath, m_XMLDataSer.Data);
								if (Group == null)
									return null;

								DBManagerApp.m_Entities.groups.Add(Group);
								DBManagerApp.m_Entities.SaveChanges(); // Чтобы получить id_group

								result.Add(new CDataChangedInfo(this)
								{
									ChangingType = enDataChangesTypes.Add,
									ChangedObjects = enDataChangedObjects.Group,
									ID = Group.id_group,
									GroupID = Group.id_group,
								});
							}
							else
								CopyXmlToGroupEntity(Group, m_XMLDataSer.Data, CompId, ScanningPath);

							if (DataFromXml.Settings.SecondColNameType != m_XMLDataSer.Data.Settings.SecondColNameType)
							{	// Тип второй колонки изменился => нужно перезаполнить поля teams и coaches
								DBManagerApp.m_Entities.SaveChanges();

								List<participations> PartsToChange = (from part in DBManagerApp.m_Entities.participations
																	  where part.Group == Group.id_group
																	  select part).ToList();
								if (DataFromXml.Settings.SecondColNameType == enSecondColNameType.Coach)
								{	// Переносим индексы из teams в coaches
									PartsToChange.ForEach(part =>
									{
										if (part.team.HasValue)
										{
											string CurTeamName = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == part.team).name;
											part.team = null;
											part.coach = GlobalDefines.GetCoachId(CurTeamName, true);
										}
									});
								}
								else
								{
									PartsToChange.ForEach(part =>
									{
										if (part.coach.HasValue)
										{
											string CurCoachName = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == part.coach).name;
											part.coach = null;
											part.team = GlobalDefines.GetTeamId(CurCoachName, true);
										}
									});
								}

								DBManagerApp.m_Entities.SaveChanges();

								GlobalDefines.DeleteUnusedCoaches();
								GlobalDefines.DeleteUnusedTeams();
							}

							if (DataFromXml.Settings.AgeGroup != null &&
								DataFromXml.Settings.AgeGroup.Sex != m_XMLDataSer.Data.Settings.AgeGroup.Sex)
							{	// Сменился пол группы =>
								// нужно сменить пол у всех участников, которые не участвовали в полностью завершённых соревах.
								// Т.к. если соревы полностью завершены, то пол менять нельзя
								DBManagerApp.m_Entities.SaveChanges();
																
								List<members> MembersInFinishedGroups = (from member in DBManagerApp.m_Entities.members
																		 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																		 join gr in DBManagerApp.m_Entities.groups on part.Group equals gr.id_group
																		 where (gr.round_finished_flags.HasValue && (gr.round_finished_flags.Value & (1 << (int)enRounds.Total)) != 0)
																		 select member).ToList();
								List<members> MembersToChange = (from member in DBManagerApp.m_Entities.members
																 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																 where part.Group == Group.id_group
																 select member).ToList();
								
								MembersToChange.ForEach(member =>
								{
									if (!MembersInFinishedGroups.Contains(member))
										member.sex = m_XMLDataSer.Data.Settings.AgeGroup.Sex;
								});

								DBManagerApp.m_Entities.SaveChanges();
							}

							DBManagerApp.m_Entities.SaveChanges(); // Сохраняем все сделанные изменения

							result.Add(new CDataChangedInfo(this)
								{
									ChangingType = m_XMLDataSer.Data.Settings == null ? enDataChangesTypes.Add : enDataChangesTypes.Changing,
									ChangedObjects = enDataChangedObjects.CompSettings,
									ID = Group.id_group,
									GroupID = Group.id_group,
									Argument = Group,
								});
						}

						// Обработка изменений результатов соревнований
						List<CDataChangedInfo> ChangesOfResults = new List<CDataChangedInfo>();
						foreach (CSpeedResults results in m_XMLDataSer.Data.AllFilledResults)
						{
							List<CDataChangedInfo> MadeChanges;
							if (m_ResultsManager == null)
							{
								m_ResultsManager = new CGroupResultsManager(this,
																			results,
																			Group,
																			m_XMLDataSer.Data.Settings,
																			false,
																			out MadeChanges);
							}
							else
								m_ResultsManager.HandleResults(results, m_XMLDataSer.Data.Settings, false, out MadeChanges);

							if (MadeChanges != null && MadeChanges.Count > 0)
								ChangesOfResults.AddRange(MadeChanges);
						}

						DBManagerApp.m_Entities.SaveChanges(); // Сохраняем все сделанные изменения

						if (ChangesOfResults.Count > 0)
						{
							// Очищаем все причины изменения файла и обновляем xml-файл
							m_XMLDataSer.Data.ClearAllChangeReasons();
							m_XMLDataSer.Write();
						}

						DataFromXml = m_XMLDataSer.Data;

						// Удаляем все повторяющиеся записи из ChangesOfResults и добавляем их в result
						result.AddRange(ChangesOfResults.Distinct());

						lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
						{
							if (DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.TryAddValue(ScanningPath, new CFileScannerSettings()
																														{
																															FullFilePath = ScanningPath,
																															GroupId = Group.id_group
																														})
								)
							{	// Информации об этом файле в настройках ещё нет
								DBManagerApp.m_AppSettings.Write();
							}
						}
					}
					catch (Exception ex)
					{
						OnException(ref result, ex, Group == null ? GlobalDefines.DEFAULT_XML_INT_VAL : Group.id_group);
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Удаление группы из БД и словаря DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.
		/// </summary>
		public bool DeleteGroup()
		{
			LastException = null;

			lock (EventsCS)
			{
				try
				{
					// Нужно удалить сначала записи из participations
					List<participations> PartsToDelete = Group.participations.ToList();
					for (int i = 0; i < PartsToDelete.Count; i++)
					{
						List<results_speed> ResultsToDelete = PartsToDelete[i].results_speed.ToList();
						for (int n = 0; n < ResultsToDelete.Count; n++)
							DBManagerApp.m_Entities.results_speed.Remove(ResultsToDelete[n]);
						try
						{
							DBManagerApp.m_Entities.SaveChanges();
						}
						catch (Exception ex)
						{
							ex.ToString();
						}

						DBManagerApp.m_Entities.participations.Remove(PartsToDelete[i]);
					}
					DBManagerApp.m_Entities.SaveChanges();

					// Теперь из round_dates
					List<round_dates> DatesToDelete = Group.round_dates.ToList();
					for (int i = 0; i < DatesToDelete.Count; i++)
						DBManagerApp.m_Entities.round_dates.Remove(DatesToDelete[i]);
					DBManagerApp.m_Entities.SaveChanges();

					//Теперь из falsestarts_rules
					List<falsestarts_rules> RulesToDelete = DBManagerApp.m_Entities.falsestarts_rules.Where(arg => arg.Group == Group.id_group).ToList();
					for (int i = 0; i < RulesToDelete.Count; i++)
						DBManagerApp.m_Entities.falsestarts_rules.Remove(RulesToDelete[i]);
					DBManagerApp.m_Entities.SaveChanges();

					DBManagerApp.m_Entities.groups.Remove(Group);
					DBManagerApp.m_Entities.SaveChanges();

					// Удаляем соревнования, для которых нет групп
					List<descriptions> DescsToDelete = DBManagerApp.m_Entities.descriptions.Where(arg => arg.groups.Count == 0).ToList();
					for (int i = 0; i < DescsToDelete.Count; i++)
						DBManagerApp.m_Entities.descriptions.Remove(DescsToDelete[i]);

					// Удаляем спортсменов, которые нигде не участвовали
					List<members> MembersToDelete = DBManagerApp.m_Entities.members.Where(arg => arg.participations.Count == 0).ToList();
					for (int i = 0; i < MembersToDelete.Count; i++)
						DBManagerApp.m_Entities.members.Remove(MembersToDelete[i]);
					DBManagerApp.m_Entities.SaveChanges();
				}
				catch (Exception ex)
				{
					List<CDataChangedInfo> Changes = new List<CDataChangedInfo>();
					OnException(ref Changes, ex, Group == null ? GlobalDefines.DEFAULT_XML_INT_VAL : Group.id_group);
					RaiseDataChangedEvent(new DataChangedEventArgs(Changes));
					return false;
				}

				if (State == enScanningThreadState.Worked)
				{
					// Свойство ScanningPath здесь использовать нельзя
					m_XMLDataSer.FullFilePath = m_ScanningPath = "";
					m_XMLDataSer.ClearData();
					Group = null;

					State = enScanningThreadState.Stopped;
				}

				lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
				{
					if (DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(ScanningPath))
					{
						DBManagerApp.m_AppSettings.Write();
					}
				}
			}

			return true;
		}

				
		#region Заполнение сущностей данными из xml
		private descriptions DescFromXml2Entity(string DirPath, CAllExcelData XmlData)
		{
			descriptions result = new descriptions();

			if (CopyXmlToDescEntity(result, XmlData, DirPath))
				return result;
			else
				return null;
		}


		private bool CopyXmlToDescEntity(descriptions Dest, CAllExcelData Src, string DirPath)
		{
			if (Src.Settings != null &&
				!string.IsNullOrWhiteSpace(Src.Settings.CompName) &&
				!string.IsNullOrWhiteSpace(DirPath) &&
				Directory.Exists(DirPath))
			{
				Dest.dir = DirPath;
				Dest.name = Src.Settings.CompName;

				return true;
			}
			else
				return false;
		}


		private groups GroupFromXml2Entity(long DescId, string XmlFullFilePath, CAllExcelData XmlData)
		{
			groups result = new groups();
			
			if (CopyXmlToGroupEntity(result, XmlData, DescId, XmlFullFilePath))
				return result;
			else
				return null;
		}


		/// <summary>
		/// Копирует все поля, кроме индексов
		/// </summary>
		/// <param name="Dest"></param>
		/// <param name="Src"></param>
		/// <param name="DescId"></param>
		/// <param name="XmlFullFilePath"></param>
		/// <returns></returns>
		private bool CopyXmlToGroupEntity(groups Dest, CAllExcelData Src, long DescId, string XmlFullFilePath)
		{
			if (Src.Settings != null)
			{
				try
				{
					Dest.desc = DescId;
					Dest.xml_file_name = XmlFullFilePath;

					Dest.main_judge = Src.Settings.MainJudge;
					Dest.main_secretary = Src.Settings.MainSecretary;
					Dest.row6 = Src.Settings.Row6;
					Dest.second_col_name = Src.Settings.SecondColName;

					if (Src.Settings.StartDate != null)
						Dest.comp_start_date = Src.Settings.StartDate.Date;
					if (Src.Settings.EndDate != null)
						Dest.comp_end_date = Src.Settings.EndDate.Date;

					if (Src.Settings.MembersFrom1stQualif != GlobalDefines.DEFAULT_XML_BYTE_VAL)
						Dest.from_1_qualif = Src.Settings.MembersFrom1stQualif;
					if (Src.Settings.MembersFrom2ndQualif != GlobalDefines.DEFAULT_XML_BYTE_VAL)
						Dest.from_2_qualif = Src.Settings.MembersFrom2ndQualif;

					if (Src.Settings.AgeGroup != null)
					{
						Dest.name = Src.Settings.AgeGroup.Name;
						Dest.sex = Src.Settings.AgeGroup.Sex;
						if (Src.Settings.AgeGroup.StartYear != GlobalDefines.DEFAULT_XML_INT_VAL)
							Dest.start_year = Src.Settings.AgeGroup.StartYear;
						if (Src.Settings.AgeGroup.wEndYear != GlobalDefines.DEFAULT_XML_INT_VAL)
							Dest.end_year = Src.Settings.AgeGroup.wEndYear;
					}

					if (Src.RoundAfterQualif != null)
						Dest.round_after_qualif = GlobalDefines.ROUND_IDS[Src.RoundAfterQualif.NodeName];

					return true;
				}
				catch
				{
					return false;
				}
			}
			else
				return false;
		}
		#endregion


		private void AddRoundsDates(List<KeyValuePair<string, string>> RoundDates, long GroupId, bool SaveChangesAfterAdd)
		{
			foreach (KeyValuePair<string, string> RoundDate in RoundDates)
			{
				round_dates Date = new round_dates()
				{
					round = GlobalDefines.ROUND_IDS[RoundDate.Key],
					date = RoundDate.Value,
					Group = GroupId
				};
				DBManagerApp.m_Entities.round_dates.Add(Date);
			}

			if (SaveChangesAfterAdd)
				DBManagerApp.m_Entities.SaveChanges();
		}


		private bool CopyDataFromXMLFile2DB(string XMLFullFilePath, out List<CDataChangedInfo> MadeChanges, bool FromSyncMethod)
		{
			MadeChanges = new List<CDataChangedInfo>();
			lock (m_XMLDataSer.DataSyncObj)
			{
				ScanningPath = m_XMLDataSer.FullFilePath = XMLFullFilePath;
				string XMLFileDir = Path.GetDirectoryName(XMLFullFilePath);
				
				if (!m_XMLDataSer.Read())
				{	// Если файл не удалось прочитать, то дальнейшие действия невозможны
					return false;
				}

				bool GroupAdded = false;

				DataFromXml = m_XMLDataSer.Data;

				// Проверяем, есть ли в БД соревнования
				IEnumerable<descriptions> DescsInDB = from desc in DBManagerApp.m_Entities.descriptions
													  where (XMLFileDir == desc.dir)
													  select desc;
				bool HasCompsInDB = DescsInDB.Count() > 0;
				if (!HasCompsInDB)
				{	// соревнований по этому идентификатору нет => проверяем нет ли по названию
					if (DataFromXml.Settings != null)
					{
						DescsInDB = from desc in DBManagerApp.m_Entities.descriptions
								   where (DataFromXml.Settings.CompName == desc.name)
								   select desc;
						HasCompsInDB = DescsInDB.Count() > 0;
					}
				}

				if (HasCompsInDB)
				{	// Соревнования уже есть => проверяем, не изменилось ли в них что-то
					descriptions Desc = DescsInDB.First();
					CompId = Desc.id_desc;
					if (!DataFromXml.Settings.DescriptionPropsEquals(Desc))
					{
						if (string.IsNullOrEmpty(DataFromXml.Settings.CompName))
						{	// Соревнование уже есть, но к нему добавилась новая группа, для которой пока не введено название соревнований
							DataFromXml.Settings.CompName = Desc.name;
						}
						else
						{
							Desc.name = DataFromXml.Settings.CompName;
							DBManagerApp.m_Entities.SaveChanges();

							MadeChanges.Add(new CDataChangedInfo(this)
							{
								ChangingType = enDataChangesTypes.Changing,
								ChangedObjects = enDataChangedObjects.CompSettings,
								ID = CompId,
								Argument = Desc,
							});
						}
					}
				}
				else
				{	// Соревнований нет => добавляем их
					descriptions Desc = DescFromXml2Entity(XMLFileDir, DataFromXml);
					if (Desc != null)
					{
						DBManagerApp.m_Entities.descriptions.Add(Desc);
						DBManagerApp.m_Entities.SaveChanges(); // Чтобы получить id_desc

						CompId = Desc.id_desc;

						MadeChanges.Add(new CDataChangedInfo(this)
						{
							ChangingType = enDataChangesTypes.Add,
							ChangedObjects = enDataChangedObjects.CompSettings,
							ID = CompId,
							Argument = Desc,
						});
					}
					else
						return true;
				}

				if (CompId < 0 || DataFromXml.Settings.AgeGroup == null || string.IsNullOrWhiteSpace(DataFromXml.Settings.AgeGroup.Name))
				{	// Нет данных о соревнованиях
					return true;
				}

				IEnumerable<groups> GroupInDB = from gr in DBManagerApp.m_Entities.groups
												where (CompId == gr.desc &&
														(gr.xml_file_name == XMLFullFilePath ||
															(gr.name == DataFromXml.Settings.AgeGroup.Name &&
															gr.start_year == DataFromXml.Settings.AgeGroup.StartYear &&
															gr.end_year == DataFromXml.Settings.AgeGroup.wEndYear &&
															gr.sex == DataFromXml.Settings.AgeGroup.Sex)))
												select gr;
				switch (GroupInDB.Count())
				{
					case 0: // Такой группы ещё нет => добавляем её в БД
						Group = GroupFromXml2Entity(CompId, XMLFullFilePath, DataFromXml);

						if (Group != null)
						{
							GroupAdded = true;
							DBManagerApp.m_Entities.groups.Add(Group);
							DBManagerApp.m_Entities.SaveChanges(); // Чтобы получить id_group

							if (DataFromXml.Settings.EndDate != null)
							{	// Добавляем даты проведения раундов соревнований
								AddRoundsDates(DataFromXml.Settings.RoundDates, Group.id_group, false);
							}

							MadeChanges.Add(new CDataChangedInfo(this)
							{
								ChangingType = enDataChangesTypes.Add,
								ChangedObjects = enDataChangedObjects.Group,
								ID = Group.id_group,
								GroupID = Group.id_group,
							});
						}
						break;

					case 1:
						Group = GroupInDB.First();

						if (GlobalDefines.IsRoundFinished(Group.round_finished_flags, enRounds.Final))
						{	// Если соревы закончились, то изменять данные в БД уже нельзя!!!
							DataFromXml.Settings = new CCompSettings(Group);

							MadeChanges.Add(new CDataChangedInfo(this)
							{
								ChangingType = enDataChangesTypes.Changing,
								ChangedObjects = enDataChangedObjects.CompSettings,
								ID = Group.id_group,
								GroupID = Group.id_group,
								Argument = Group,
							});
							return true; // Больше ничего в таком случае делать не нужно
						}
						else
						{
							// Проверяем, не изменились ли сведения о группе
							if (!DataFromXml.Settings.GroupPropsEquals(Group, XMLFullFilePath))
							{	/* Настройки соревнований изменились */
								if (Group.comp_end_date == null)
								{
									if (DataFromXml.Settings.EndDate != null &&
										DataFromXml.Settings.EndDate.Date != GlobalDefines.DEFAULT_XML_DATE_TIME_VAL)
									{	// Добавляем даты проведения раундов соревнований
										AddRoundsDates(DataFromXml.Settings.RoundDates, Group.id_group, false);
									}
								}
								else
								{
									if (DataFromXml.Settings.EndDate == null ||
										DataFromXml.Settings.EndDate.Date == GlobalDefines.DEFAULT_XML_DATE_TIME_VAL ||
										!DataFromXml.Settings.RoundDatesEquals(Group))
									{
										// Нужно удалить даты, т.к. они изменились
										foreach (round_dates RoundDate in (from round_date in DBManagerApp.m_Entities.round_dates
																		   where round_date.Group == Group.id_group
																		   select round_date).ToList())
										{
											DBManagerApp.m_Entities.round_dates.Remove(RoundDate);
										}

										DBManagerApp.m_Entities.SaveChanges(); // Чтобы удаление применилось

										if (DataFromXml.Settings.EndDate != null &&
											DataFromXml.Settings.EndDate.Date != GlobalDefines.DEFAULT_XML_DATE_TIME_VAL)
										{
											AddRoundsDates(m_XMLDataSer.Data.Settings.RoundDates, Group.id_group, false);
										}
									}
								}

								string CurSecondColNameInDB = Group.second_col_name;
								byte CurSexInDB = Group.sex;
								CopyXmlToGroupEntity(Group, DataFromXml, CompId, ScanningPath);

								if (DataFromXml.Settings.SecondColName != CurSecondColNameInDB)
								{	// Тип второй колонки изменился => нужно перезаполнить поля teams и coaches
									DBManagerApp.m_Entities.SaveChanges();

									List<participations> PartsToChange = (from part in DBManagerApp.m_Entities.participations
																		  where part.Group == Group.id_group
																		  select part).ToList();
									if (DataFromXml.Settings.SecondColNameType == enSecondColNameType.Coach)
									{	// Переносим индексы из teams в coaches
										PartsToChange.ForEach(part =>
										{	
											if (part.team.HasValue)
											{
												string CurTeamName = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == part.team).name;
												part.team = null;
												part.coach = GlobalDefines.GetCoachId(CurTeamName, true);
											}
										});
									}
									else
									{
										PartsToChange.ForEach(part =>
										{
											if (part.coach.HasValue)
											{
												string CurCoachName = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == part.coach).name;
												part.coach = null;
												part.team = GlobalDefines.GetTeamId(CurCoachName, true);
											}
										});
									}
									
									DBManagerApp.m_Entities.SaveChanges();

									GlobalDefines.DeleteUnusedCoaches();
									GlobalDefines.DeleteUnusedTeams();
								}

								if (DataFromXml.Settings.AgeGroup.Sex != CurSexInDB)
								{	// Сменился пол группы =>
									// нужно сменить пол у всех участников, которые не участвовали в полностью завершённых соревах.
									// Т.к. если соревы полностью завершены, то пол менять нельзя
									DBManagerApp.m_Entities.SaveChanges();

									List<members> MembersInFinishedGroups = (from member in DBManagerApp.m_Entities.members
																			 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																			 join gr in DBManagerApp.m_Entities.groups on part.Group equals gr.id_group
																			 where (gr.round_finished_flags.HasValue && (gr.round_finished_flags.Value & (1 << (int)enRounds.Total)) != 0)
																			 select member).ToList();
									List<members> MembersToChange = (from member in DBManagerApp.m_Entities.members
																	 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																	 where part.Group == Group.id_group
																	 select member).ToList();

									MembersToChange.ForEach(member =>
									{
										if (!MembersInFinishedGroups.Contains(member))
											member.sex = m_XMLDataSer.Data.Settings.AgeGroup.Sex;
									});
									
									DBManagerApp.m_Entities.SaveChanges();
								}

								DBManagerApp.m_Entities.SaveChanges(); // Сохраняем все сделанные изменения

								MadeChanges.Add(new CDataChangedInfo(this)
								{
									ChangingType = DataFromXml.Settings == null ? enDataChangesTypes.Add : enDataChangesTypes.Changing,
									ChangedObjects = enDataChangedObjects.CompSettings,
									ID = Group.id_group,
									GroupID = Group.id_group,
									Argument = Group,
								});
							}
						}
						break;

					default:
						throw new InvalidOperationException(string.Format("More then 1 groups are binded to file \"{0}\" in competition {1}",
																			ScanningPath,
																			CompId));
				}

				if (Group != null)
				{	// Добавляем спортсменов в БД
					bool HasChangesInResults = false;
					List<CDataChangedInfo> ChangesOfResults = null;
					foreach (CSpeedResults results in DataFromXml.AllFilledResults)
					{
						if (FromSyncMethod)
						{	/* Если нам нужно синхронизироваться, то по-любому нужно выполнить переписать все данные из файла в БД.
								* Для этого устанавливаем соотвествующие ChangeReason */
							switch (results.RoundInEnum)
							{
								case enRounds.Qualif:
									results.ChangeReason = enChangeReason.crWholeContent;
									break;

								default:
									results.ChangeReason = enChangeReason.crResultsChanged;
									break;
							}
						}
						if (m_ResultsManager == null)
						{
							m_ResultsManager = new CGroupResultsManager(this,
																		results,
																		Group,
																		DataFromXml.Settings,
																		GroupAdded,
																		out ChangesOfResults);
						}
						else
							m_ResultsManager.HandleResults(results, DataFromXml.Settings, GroupAdded, out ChangesOfResults);

						if (ChangesOfResults != null && ChangesOfResults.Count > 0)
						{
							MadeChanges.AddRange(ChangesOfResults);
							HasChangesInResults = true;
						}
					}

					MadeChanges = new List<CDataChangedInfo>(MadeChanges.Distinct());

					DBManagerApp.m_Entities.SaveChanges(); // Сохраняем все сделанные изменения

					if (HasChangesInResults)
					{
						// Очищаем все причины изменения файла и обновляем xml-файл
						DataFromXml.ClearAllChangeReasons();
						m_XMLDataSer.Write();
					}
				}
			}

			return true;
		}
	}
}
