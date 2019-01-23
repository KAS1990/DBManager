using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Excel.Exporting.Tabs;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using DBManager.ReportGenerators;
using System.Windows;

namespace DBManager.Excel.Exporting.ExportingClasses
{
	public class CTeamExporter : CReportExporterBase
	{
		#region Номера столбцов в листе Excel
		private int EXCEL_PLACE_COL_NUM = 1;
		private int EXCEL_TEAM_COL_NUM = 2;
		private int EXCEL_TEAM_MEMBERS_OFS = 0;
		private int EXCEL_TEAM_BALLS_OFS = 1;
		#endregion

		protected string RN_RESULT = "Result";
		protected string RN_LEAD = "Lead";
		protected string RN_SPEED = "Speed";
		protected string RN_MEN_IN_GROUP = "MenInGroup";
		protected string RN_WOMEN_IN_GROUP = "WomenInGroup";


		protected int EXCEL_MAX_LINES_IN_TEAM_REPORT = 50;


		public class CTeamTask : CReportExporterBase.CTask
		{
			/// <summary>
			/// Число мужчин в группе для командного зачёта
			/// </summary>
			public int m_MenInGroup = -1;

			/// <summary>
			/// Число женщин в группе для командного зачёта
			/// </summary>
			public int m_WomenInGroup = -1;

			/// <summary>
			/// Настройки протокола трудности
			/// </summary>
			public CLeadReportInfo m_LeadReportInfo = new CLeadReportInfo();

			/// <summary>
			/// Список команд, по которым будет составлен отчёт
			/// </summary>
			public List<CTeamForReportItem> m_Teams = new List<CTeamForReportItem>();
		}
		
		
		CTeamTask CurTask
		{
			get
			{
				if (m_Task is CTeamTask)
					return m_Task as CTeamTask;
				else
					throw new InvalidCastException("Invalid of type m_Task in DBManager.Excel.Exporting.ExportingClasses.CTeamExporter. Requires CTeamExporter.CTeamTask");
			}
		}


		public CTeamExporter(CTask Task) :
			base(Task)
		{
			
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="excelApp">
		/// Приложение Excel, в котором открыты книги
		/// <paramref name="wbkTarget"/> и <paramref name="wbkTemplates"/>
		/// </param>
		/// <param name="wbkTarget">
		/// Книга, в которую добавляется отчёт. Добавление всегда происходит в конец
		/// </param>
		/// <param name="wbkTemplates">
		/// Книга с шаблонами отчётов
		/// </param>
		/// <returns></returns>
		public override bool CreateReport(MSExcel.Application excelApp,
											MSExcel.Workbook wbkTarget,
											MSExcel.Workbook wbkTemplates)
		{
			List<CTeamWithMembersItem> TeamReportData = new List<CTeamWithMembersItem>();
			foreach (CTeamForReportItem item in CurTask.m_Teams)
			{
				TeamReportData.Add(new CTeamWithMembersItem()
					{
						m_TeamInfo = item
					});
			}

			// Копируем в конец новой книги лист-шаблон
			MSExcel.Worksheet wsh = null;
			lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
			{
				wsh = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictReportTemplates[enReportTypes.Team]];
			}
			wsh.Copy(After: wbkTarget.Worksheets[wbkTarget.Worksheets.Count]);

			// Лист, в который нужно будет вставлять данные
			wsh = wbkTarget.Worksheets[wbkTarget.Worksheets.Count];
			wsh.Name = Properties.Resources.resTeamReportSheetName;

			DateTime? MinDate = null, MaxDate = null;
												
			wsh.Range[RN_WOMEN_IN_GROUP].Value = CurTask.m_WomenInGroup;
			wsh.Range[RN_MEN_IN_GROUP].Value = CurTask.m_MenInGroup;

			bool HasLeadReport = CurTask.m_LeadReportInfo.m_wbkLeadReport != null;
			string UnknownLeadTeams = "";

			#region Подсчёт статистики в трудности
			if (HasLeadReport)
			{
				// Проходим все листы трудности - все группы
				foreach (KeyValuePair<long, CGroupItem> LeadGroupInfo in CurTask.m_LeadReportInfo.m_dictLeadGroupInfos)
				{
					if (MinDate == null || LeadGroupInfo.Value.StartDate < MinDate)
						MinDate = LeadGroupInfo.Value.StartDate;

					if (LeadGroupInfo.Value.EndDate != null)
					{
						if (MaxDate == null || LeadGroupInfo.Value.EndDate > MaxDate)
							MaxDate = LeadGroupInfo.Value.EndDate;
					}

					MSExcel.Worksheet wshLead = CurTask.m_LeadReportInfo.m_wbkLeadReport.Worksheets[LeadGroupInfo.Value.LeadSheetIndex + 1];

					List<CPersonalItem> LeadMembers = new List<CPersonalItem>();
					for (int Row = 0;
						wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value != null &&
						!string.IsNullOrWhiteSpace(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value.ToString());
						Row++)
					{
						int PlaceInLead;
						if (!int.TryParse(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value.ToString(), out PlaceInLead))
						{	// У участника какое-то неонятное место => пропускаем
							continue;
						}

						string[] arrNameAndSurname;
						string SurnameAndName = GlobalDefines.CorrectSurnameAndName(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PersonalDataColumnIndex].Value,
																					out arrNameAndSurname);

						// Ищем команду в списке
						string TeamNameInLead = wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_TeamColumnIndex].Value;
						TeamNameInLead = GlobalDefines.CorrectTeam(TeamNameInLead);
						CTeamWithMembersItem TeamInfo = TeamReportData.Find(arg => arg.m_TeamInfo.Subteams.FirstOrDefault(arg1 => arg1.Name == TeamNameInLead) != null);
						if (TeamInfo == null)
						{	// Такой команды нет => запоминаем её название, чтобы потом вывести его в сообщении
							// Но из рассмотрения этого участника не выкидываем, т.к. нужно посчитать заработанные им баллы
							UnknownLeadTeams += string.Format("\t {0} {1} {2};\n", GlobalDefines.BULLET_CHAR_IN_MSG_BOX, SurnameAndName, TeamNameInLead);
						}

						// Заносим участника в LeadMembers, чтобы потом посчитать его баллы
						CPersonalItem Member = new CPersonalItem()
						{
							m_Place = PlaceInLead,
							m_GroupId = LeadGroupInfo.Value.id,
							m_SurnameAndName = SurnameAndName,
							m_Sex = LeadGroupInfo.Value.Sex
						};
						LeadMembers.Add(Member);

						if (TeamInfo != null)
						{
							List<CPersonalItem> GroupLeadMembers;
							if (TeamInfo.m_dictLeadMembers.TryGetValue(Member.m_GroupId, out GroupLeadMembers))
							{
								if (GroupLeadMembers.Count < (LeadGroupInfo.Value.Sex == enSex.Male ? CurTask.m_MenInGroup : CurTask.m_WomenInGroup))
								{	// Группа ещё не заполнена
									GroupLeadMembers.Add(Member);
								}
							}
							else
							{
								GroupLeadMembers = new List<CPersonalItem>();
								GroupLeadMembers.Add(Member);
								TeamInfo.m_dictLeadMembers.Add(Member.m_GroupId, GroupLeadMembers);
							}
						}
					}

					MakeBallsForPlaces(LeadMembers);	// Считаем баллы за места в трудности
					LeadMembers.Clear();
				}

				// Удаляем команды, которых нет в трудности, т.к. для командного зачёта необходим результат команды и в трудности, и в скорости
				TeamReportData.RemoveAll(arg => arg.m_dictLeadMembers.Count == 0);

				if (!string.IsNullOrWhiteSpace(UnknownLeadTeams))
				{	// Есть команды, которых нет в TeamReportData => сообщаем об этом
					// Оставляем не более 10 записей, чтобы не раздувать сообщение
					List<string> Lines = UnknownLeadTeams.Split('\n').ToList();
					Lines.RemoveAt(Lines.Count - 1); // Последняя строка будет всегда пустой
					if (Lines.Count > 10)
					{
						UnknownLeadTeams = "";
						for (int i = 0; i < 10; i++)
							UnknownLeadTeams += Lines[i] + "\n";
						UnknownLeadTeams += "...\n";
					}
					MessageBox.Show(string.Format(Properties.Resources.resfmtUnknownTeamsInLead, UnknownLeadTeams),
											DBManagerApp.MainWnd.Title,
											MessageBoxButton.OK,
											MessageBoxImage.Error);
				}
			}
			#endregion

			#region Подсчитываем статистику в скорости
			bool IsFirstGroup = true;
			foreach (groups GroupInDB in CurTask.m_CompDesc.groups)
			{
				CCompSettings CompSettings = new CCompSettings(GroupInDB);

				if (IsFirstGroup)
				{
					IsFirstGroup = false;

					wsh.Range[RN_COMP_NAME].Value = CompSettings.CompName;
					wsh.Range[RN_MAIN_JUDGE].Value = CompSettings.MainJudge;
					wsh.Range[RN_MAIN_SECRETARY].Value = CompSettings.MainSecretary;
				}

				if (MinDate == null || CompSettings.StartDate.Date < MinDate)
					MinDate = CompSettings.StartDate.Date;

				if (CompSettings.EndDate != null)
				{
					if (MaxDate == null || CompSettings.EndDate.Date > MaxDate)
						MaxDate = CompSettings.EndDate.Date;
				}

				// Раскидываем участников по командам
				foreach (CPersonalItem Member in (from member in DBManagerApp.m_Entities.members
												  join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
												  where part.Group == GroupInDB.id_group &&
														part.result_place.HasValue &&
														part.results_speed.Count(arg => arg.balls.HasValue && arg.balls.Value > 0) == 1 // Выбираем только те группы, у которых расставлены баллы
												  orderby part.result_place
												  select new CPersonalItem()
												  {
													  m_Place = part.result_place.Value,
													  m_GroupId = part.Group,
													  m_SurnameAndName = member.surname + " " + member.name,
													  m_Sex = (enSex)member.sex,
													  m_Balls = (from result in part.results_speed
																where result.balls.HasValue
																select result.balls.Value).FirstOrDefault(),
													  m_Team = CompSettings.SecondColNameType == enSecondColNameType.Coach ? part.coach : part.team
												  }).ToList())
				{
					string TeamName = "";
					if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
						TeamName = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == Member.m_Team).name;
					else
						TeamName = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == Member.m_Team).name;

					CTeamWithMembersItem TeamInfo = TeamReportData.Find(arg => arg.m_TeamInfo.Subteams.FirstOrDefault(arg1 => arg1.Name == TeamName) != null);
					List<CPersonalItem> GroupSpeedMembers;
					if (TeamInfo.m_dictSpeedMembers.TryGetValue(Member.m_GroupId, out GroupSpeedMembers))
					{
						if (GroupSpeedMembers.Count < (Member.m_Sex == enSex.Male ? CurTask.m_MenInGroup : CurTask.m_WomenInGroup))
						{	// Группа ещё не заполнена
							GroupSpeedMembers.Add(Member);
						}
					}
					else
					{
						GroupSpeedMembers = new List<CPersonalItem>();
						GroupSpeedMembers.Add(Member);
						TeamInfo.m_dictSpeedMembers.Add(Member.m_GroupId, GroupSpeedMembers);
					}
				}
			}
			// Удаляем команды, которых нет в скорости, т.к. для командного зачёта необходим результат команды и в трудности, и в скорости
			TeamReportData.RemoveAll(arg => arg.m_dictSpeedMembers.Count == 0);
			#endregion

			// Выводим дату соревнований
			wsh.Range[RN_ROUND_DATE].Value = GlobalDefines.CreateCompDate(MinDate.Value, MaxDate);
						
			// Сортируем команды по убыванию заработанных баллов
			TeamReportData.Sort((lhs, rhs) =>
				{
					if (GlobalDefines.IsFloatsEqu(lhs.TotalBalls, rhs.TotalBalls, 1e-3))
						return 0;
					else if (lhs.TotalBalls < rhs.TotalBalls)
						return 1;
					else
						return -1;
				});

			TeamReportData.ForEach(team =>
				{
					// Сортируем группы по полу: сначала девочки, потом мальчики, а потом по годам рождения
					team.m_dictLeadMembers = (from GroupMembers in team.m_dictLeadMembers
											  orderby GroupMembers.Value[0].m_Sex descending,
														DBManagerApp.m_Entities.groups.FirstOrDefault(gr => gr.id_group == GroupMembers.Key).start_year descending
											  select GroupMembers).ToDictionary(key => key.Key, value => value.Value);
					team.m_dictSpeedMembers = (from GroupMembers in team.m_dictSpeedMembers
											  orderby GroupMembers.Value[0].m_Sex descending,
														DBManagerApp.m_Entities.groups.FirstOrDefault(gr => gr.id_group == GroupMembers.Key).start_year descending
											  select GroupMembers).ToDictionary(key => key.Key, value => value.Value);
				});

			#region Выводим отчёт
			int FirstRow = wsh.Range[RN_FIRST_DATA_ROW].Row;
			int CurPlace = 1;
			float? PrevResult = null; // Такого результата не может быть

			for (int i = 0; i < TeamReportData.Count; i++)
			{
				if (TeamReportData[i].TotalBalls != PrevResult)
					CurPlace = i + 1;

				wsh.Cells[i + FirstRow, EXCEL_PLACE_COL_NUM].Value = GlobalDefines.EncodePlace((byte)CurPlace);
				wsh.Cells[i + FirstRow, EXCEL_TEAM_COL_NUM].Value = TeamReportData[i].m_TeamInfo.Name;

				string MembersList = "";
				if (HasLeadReport)
				{
					foreach (KeyValuePair<long, List<CPersonalItem>> GroupMembers in TeamReportData[i].m_dictLeadMembers)
					{
						GroupMembers.Value.ForEach(member =>
							{
								MembersList += string.Format("{0} ({1}) ",
																member.m_SurnameAndName,
																member.m_Balls);
							});
						MembersList += "\n";
					}
					wsh.Range[RN_LEAD].Offset[i].Cells[1, EXCEL_TEAM_MEMBERS_OFS + 1].Value = MembersList.Left(MembersList.Length - 1);
					wsh.Range[RN_LEAD].Offset[i].Cells[1, EXCEL_TEAM_BALLS_OFS + 1].Value = TeamReportData[i].LeadBalls;
				}

				MembersList = "";
				foreach (KeyValuePair<long, List<CPersonalItem>> GroupMembers in TeamReportData[i].m_dictSpeedMembers)
				{
					GroupMembers.Value.ForEach(member =>
					{
						MembersList += string.Format("{0} ({1}) ",
														member.m_SurnameAndName,
														member.m_Balls);
					});
					MembersList += "\n";
				}
				wsh.Range[RN_SPEED].Offset[i].Cells[1, EXCEL_TEAM_MEMBERS_OFS + 1].Value = MembersList.Left(MembersList.Length - 1);
				wsh.Range[RN_SPEED].Offset[i].Cells[1, EXCEL_TEAM_BALLS_OFS + 1].Value = TeamReportData[i].SpeedBalls;

				if (HasLeadReport)
					wsh.Range[RN_RESULT].Offset[i].Value = TeamReportData[i].TotalBalls;
				else
					wsh.Range[RN_RESULT].Offset[i].FormulaR1C1 = "=RC[-3] + RC[-1]";

				PrevResult = TeamReportData[i].TotalBalls;
			}

			// Удаляем лишние строки
			wsh.Rows[(FirstRow + TeamReportData.Count).ToString() + ":" + (EXCEL_MAX_LINES_IN_TEAM_REPORT + FirstRow - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);
			#endregion

			return true;
		}


		private bool MakeBallsForPlaces(List<CPersonalItem> Members)
		{
			// Так как баллы начисляются НЕ БОЛЕЕ, чем 75% участников,
			// т.е. ВЕСЬ паровоз, выходящий за эту границу в расчет не принимается
			int MembersToCalcCount = (Members.Count * 3) / 4;

			// Последнее место участника в той части, где рассчитываем баллы
			int LastPlace = Members[MembersToCalcCount - 1].m_Place;
			int AfterLastBalls = 0; // Число баллов, даваемых за место, следующее после LastPlace
			int SpecialTrainEndInd = 0; // Номер строки, на которой заканчивается паровоз, _
			// выходящий за пределы 30-ого места
			int TrainBallsSumm = 0; // Сумма "баллов за места" участников паровоза
			int CurPlace = 0; // Текущее место
			List<CPersonalItem> Train = new List<CPersonalItem>();

			if (MembersToCalcCount < GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS)
			{
				// Проверяем факт того, что паровоз выходит за границу MembersToCalcCount
				if (Members[MembersToCalcCount - 1].m_Place == Members[MembersToCalcCount].m_Place)
				{
					// Исключаем весь паровоз из расчёта
					MembersToCalcCount--;
					while (Members[MembersToCalcCount - 1].m_Place == Members[MembersToCalcCount].m_Place &&
						MembersToCalcCount > 0)
					{
						MembersToCalcCount--;
					}
					if (MembersToCalcCount == 0)
					{	// Все заняли первое место, поэтому расчитать баллы нельзя
						return false;
					}
				}
			}
			else
			{	// После 30-ого места баллы все равно не начисляются
				MembersToCalcCount = GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS;
			}

			if (MembersToCalcCount == GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS)
			{	// В этом случае возможен паровоз, выходящий за границу 30-ого места
				AfterLastBalls = 0; // После 30-ого места баллы не начисляются
				CurPlace = GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS;
				SpecialTrainEndInd = MembersToCalcCount - 1;
				// Поиск конца паровоза
				while (Members[SpecialTrainEndInd].m_Place == Members[SpecialTrainEndInd + 1].m_Place)
					SpecialTrainEndInd++;
				if (SpecialTrainEndInd >= MembersToCalcCount)
				{	// Есть выходящий за 30-ое место паровоз
					TrainBallsSumm = 0;
					MembersToCalcCount--;
					
					// Ищем начало паровоза и вычисляем сумму баллов участников паровоза
					while (Members[MembersToCalcCount].m_Place == Members[MembersToCalcCount - 1].m_Place)
					{
						TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
						Train.Add(Members[MembersToCalcCount]); // Добавляем участника в паровоз
						MembersToCalcCount--;
						CurPlace--;
					}
					// Расставляем баллы участникам паровоза.
					foreach (CPersonalItem TrainItem in Train)
						TrainItem.m_Balls = GlobalDefines.MakeBalls(TrainBallsSumm, Train.Count);
					Train.Clear();
					MembersToCalcCount--; // Чтобы MembersToCalcCount содержал номер строки с участником перед паровозом 
				}
			}
			else
				AfterLastBalls = GlobalDefines.BALLS_FOR_PLACES[LastPlace + 1];

			// Просмотр всех остальных участников
			CurPlace = 1;
			TrainBallsSumm = 0;
			for (int MemberInd = 0; MemberInd < MembersToCalcCount; MemberInd++)
			{
				Train.Add(Members[MemberInd]); // Добавляем участника в паровоз

				if (Members[MemberInd].m_Place != Members[MemberInd + 1].m_Place)
				{	// Паровоз закончился
					// Добавляем баллы последнего участника паровоза
					TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
					// Вычитаем число баллов участника "после последнего"
					TrainBallsSumm -= AfterLastBalls * Train.Count;

					// Расставляем баллы участникам паровоза.
					foreach (CPersonalItem TrainItem in Train)
						TrainItem.m_Balls = GlobalDefines.MakeBalls(TrainBallsSumm, Train.Count);
					Train.Clear();
					
					TrainBallsSumm = 0;
				}
				else
				{	// Паровоз продолжается
					// Добавляем баллы текущего участника паровоза
					TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
				}

				CurPlace++; // Каждая новая строка - это новое место
			}

			return true;
		}
	}
}
