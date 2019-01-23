using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Excel.Exporting.Tabs;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.RoundMembers.Converters;
using System.Globalization;

namespace DBManager.Excel.Exporting.ExportingClasses
{
	public class CTotalExporter : CReportExporterBase
	{
		#region Номера столбцов в листе Excel
		private int EXCEL_PLACE_COL_NUM = 1;
		private int EXCEL_PERSONAL_COL_NUM = 2;
		private int EXCEL_TEAM_COL_NUM = 3;
		private int EXCEL_YEAR_OF_BIRTH_COL_NUM = 4;
		private int EXCEL_GRADE_COL_NUM = 5;
		private int EXCEL_ROUTE1_COL_OFS = 1;
		private int EXCEL_ROUTE2_COL_OFS = 2;
		private int EXCEL_SUM_COL_OFS = 3;
		#endregion

		protected string RN_QUALIF_1 = "Qualif1";
		protected string RN_QUALIF_2 = "Qualif2";
		protected string RN_ONE_EIGHTH_FINAL = "OneEighthFinal";
		protected string RN_QUATERFINAL = "Quaterfinal";
		protected string RN_SEMIFINAL = "Semifinal";
		protected string RN_FINAL = "Final";
		protected string RN_RESULT_GRADE = "ResultGrade";
		protected string RN_BALLS_FOR_PLACES = "BallsForPlaces";
		protected string RN_FIRST_LOOSER_DATA_ROW = "FirstLooserDataRow";
		protected string RN_TABLE_HEADER = "TableHeader";
						
			
		public class CTotalTask : CReportExporterBase.CTask
		{
			public CGroupItem m_GroupToExport;
		}


		CTotalTask CurTask
		{
			get
			{
				if (m_Task is CTotalTask)
					return m_Task as CTotalTask;
				else
					throw new InvalidCastException("Invalid of type m_Task in DBManager.Excel.Exporting.ExportingClasses.CTotalExporter. Requires CTotalExporter.CTotalTask");
			}
		}


		public CTotalExporter(CTask Task) :
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
			// Копируем в конец новой книги лист-шаблон
			MSExcel.Worksheet wsh = null;
			MSExcel.Range rng = null;
			lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
			{
				wsh = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictReportTemplates[enReportTypes.Total]];
			}
			wsh.Copy(After: wbkTarget.Worksheets[wbkTarget.Worksheets.Count]);

			// Лист, в который нужно будет вставлять данные
			wsh = wbkTarget.Worksheets[wbkTarget.Worksheets.Count];
			wsh.Name = CurTask.m_GroupToExport.Rounds.First(arg => arg.id == enRounds.Total).SheetName;

			groups GroupInDB = CurTask.m_CompDesc.groups.First(arg =>
			{
				return arg.id_group == CurTask.m_GroupToExport.id;
			});
			CCompSettings CompSettings = new CCompSettings(GroupInDB);
						
			wsh.Range[RN_COMP_NAME].Value = CompSettings.CompName;

			wsh.Range[RN_ROUND_DATE].Value = GlobalDefines.CreateCompDate(CompSettings.StartDate, CompSettings.EndDate == null ? null : (DateTime?)CompSettings.EndDate.Date);

			
			wsh.Range[RN_MAIN_JUDGE].Value = CompSettings.MainJudge;
			wsh.Range[RN_MAIN_SECRETARY].Value = CompSettings.MainSecretary;
			wsh.Range[RN_SECOND_COL_NAME].Value = CompSettings.SecondColName;
			if (CompSettings.Row6 == GlobalDefines.DEFAULT_XML_STRING_VAL)
				wsh.Cells[wsh.Range[RN_ROW_6].Row, 1].Value = "";
			else
				wsh.Range[RN_ROW_6].Value = CompSettings.Row6;

			string ReportName = wsh.Range[RN_REPORT_NAME].Value;
			int SelectedStartYear, SelectedEndYear;
			wsh.Range[RN_REPORT_NAME].Value = ReportName.Replace(GlobalDefines.EXCEL_REPORT_NAME_TEXT_TO_REPLACE,
																CreateGroupName(CurTask.m_GroupToExport,
																				CompSettings.AgeGroup,
																				out SelectedStartYear,
																				out SelectedEndYear));

			// Выводим участников соревнования
			int FirstRow = wsh.Range[RN_FIRST_DATA_ROW].Row;

			List<enRounds> CompRounds = (from round in CurTask.m_GroupToExport.Rounds
										 orderby round.id
										 select round.id).ToList();
			int FirstMiddleSheetRoundMembers = 0;
			if (CompRounds.Contains(enRounds.Qualif2))
			{
				FirstMiddleSheetRoundMembers = GroupInDB.from_2_qualif.Value;
			}
			else
			{
				FirstMiddleSheetRoundMembers = GroupInDB.from_1_qualif.Value;
				// Удаляем столбцы со второй квалификацией
				int ColQ = wsh.Range[RN_QUALIF_2].Columns.Count;
				for (int col = 0; col < ColQ; col++)
					wsh.Columns[wsh.Range[RN_QUALIF_2].Column].Delete(); 
			}

			// Удаляем столбцы для тех раундов, которых нет
			if (!CompRounds.Contains(enRounds.OneEighthFinal))
				wsh.Columns[wsh.Range[RN_ONE_EIGHTH_FINAL].Column].Delete();
			if (!CompRounds.Contains(enRounds.QuaterFinal))
				wsh.Columns[wsh.Range[RN_QUATERFINAL].Column].Delete();
			if (!CompRounds.Contains(enRounds.SemiFinal))
				wsh.Columns[wsh.Range[RN_SEMIFINAL].Column].Delete(); 

			List<results_speed> AllGroupResultsInDB = (from part in DBManagerApp.m_Entities.participations
													   join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
													   where part.Group == CurTask.m_GroupToExport.id
													   select result).ToList();
			
			List<CMemberInTotal> lstResults = (from member in DBManagerApp.m_Entities.members
												join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
												where part.Group == GroupInDB.id_group &&
														part.result_place.HasValue
												orderby part.result_place
												select new CMemberInTotal
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

													TotalGrade = part.result_grade,
													Place = part.result_place,
													id_part = part.id_participation
												}).ToList();
						
			byte CurPlace = 1;
			byte ResultIndex = 0;
			int RowsQ = 0;
			int? PrevResult = null; // Такого результата не может быть
			foreach (CMemberInTotal MemberInTotal in lstResults)
			{
				RowsQ++;

				if (MemberInTotal.MemberInfo.YearOfBirth.HasValue &&
					(MemberInTotal.MemberInfo.YearOfBirth.Value < SelectedStartYear ||
						MemberInTotal.MemberInfo.YearOfBirth.Value > SelectedEndYear))
				{	// Фильтруем участников
					continue;
				}

				ResultIndex++;

				// Заносим результат всех раундов для участника
				IEnumerable<results_speed> CurMemberResults = AllGroupResultsInDB.Where(arg => arg.participation == MemberInTotal.id_part);
				foreach (results_speed MemberResult in CurMemberResults)
				{
					if (MemberResult.round == CurMemberResults.Max(arg => arg.round))
						wsh.Range[RN_BALLS_FOR_PLACES].Offset[RowsQ].Value = MemberResult.balls;

					switch ((enRounds)MemberResult.round)
					{
						case enRounds.Qualif:
							wsh.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_ROUTE1_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.route1);
							wsh.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_ROUTE2_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.route2);
							wsh.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;

						case enRounds.Qualif2:
							wsh.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_ROUTE1_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.route1);
							wsh.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_ROUTE2_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.route2);
							wsh.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;

						case enRounds.OneEighthFinal:
							wsh.Range[RN_ONE_EIGHTH_FINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;

						case enRounds.QuaterFinal:
							wsh.Range[RN_QUATERFINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;

						case enRounds.SemiFinal:
							wsh.Range[RN_SEMIFINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;

						case enRounds.Final:
							wsh.Range[RN_FINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberResult.sum);
							break;
					}
				}

				if (MemberInTotal.Place.HasValue && MemberInTotal.Place != PrevResult)
					CurPlace = ResultIndex;

				wsh.Cells[RowsQ + FirstRow - 1, EXCEL_PLACE_COL_NUM].Value = GlobalDefines.EncodePlace(CurPlace);

				wsh.Cells[RowsQ + FirstRow - 1, EXCEL_PERSONAL_COL_NUM].Value = MemberInTotal.MemberInfo.SurnameAndName;
				if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
					wsh.Cells[RowsQ + FirstRow - 1, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == MemberInTotal.MemberInfo.Coach).name;
				else
					wsh.Cells[RowsQ + FirstRow - 1, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == MemberInTotal.MemberInfo.Team).name;
				wsh.Cells[RowsQ + FirstRow - 1, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = MemberInTotal.MemberInfo.YearOfBirth;

				GradeMarkupConverter conv = new GradeMarkupConverter();
				wsh.Cells[RowsQ + FirstRow - 1, EXCEL_GRADE_COL_NUM].Value = conv.Convert(MemberInTotal.MemberInfo.InitGrade,
																						MemberInTotal.MemberInfo.InitGrade.GetType(),
																						null,
																						CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));
				if (MemberInTotal.TotalGrade.HasValue)
				{
					wsh.Range[RN_RESULT_GRADE].Offset[RowsQ].Value = conv.Convert(MemberInTotal.TotalGrade,
																					MemberInTotal.TotalGrade.GetType(),
																					null,
																					CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));
				}

				PrevResult = MemberInTotal.Place;
			}
									
			// Редактируем сетку
			// в квалификации
			rng = wsh.Range[wsh.Cells[FirstMiddleSheetRoundMembers + FirstRow, EXCEL_PLACE_COL_NUM],
							wsh.Cells[RowsQ + FirstRow - 1, EXCEL_GRADE_COL_NUM + EXCEL_SUM_COL_OFS]];
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

			// В выполненном разряде
			rng = wsh.Range[wsh.Range[RN_RESULT_GRADE].Offset[FirstMiddleSheetRoundMembers + 1],
								wsh.Range[RN_RESULT_GRADE].Offset[RowsQ + FirstRow]];
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

			// В баллах
			rng = wsh.Range[wsh.Range[RN_BALLS_FOR_PLACES].Offset[FirstMiddleSheetRoundMembers + 1],
								wsh.Range[RN_BALLS_FOR_PLACES].Offset[RowsQ + FirstRow]];
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

									
			if (CompRounds.Contains(enRounds.Qualif2))
			{	// Во второй квалификации
				// Удаляем подчёркивание, где оно не нужно и добавляем там, где нужно
				rng = wsh.Range[wsh.Cells[FirstMiddleSheetRoundMembers + FirstRow, EXCEL_PLACE_COL_NUM],
								wsh.Cells[GroupInDB.from_1_qualif.Value + FirstRow - 1, wsh.Range[RN_QUALIF_2].Column + EXCEL_SUM_COL_OFS - 1]];
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight = MSExcel.XlBorderWeight.xlThin;
								
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

				// Удаляем сетку, где она не нужна
				rng = wsh.Range[wsh.Range[RN_QUALIF_2].Offset[GroupInDB.from_1_qualif.Value + 1].Cells[1, 1],
								wsh.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS]];
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = 
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].LineStyle = 
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideVertical].LineStyle = MSExcel.Constants.xlNone;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

				// В выполненном разряде
				// Выполняем подчёркивание
				rng = wsh.Range[RN_RESULT_GRADE].Offset[GroupInDB.from_1_qualif.Value + 1];
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

				// В баллах
				rng = wsh.Range[RN_BALLS_FOR_PLACES].Offset[GroupInDB.from_1_qualif.Value + 1];
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;
			}

			bool NeedCorrectBorders = false;
			int MaxRows = EXCEL_MAX_LINES_IN_REPORTS;
			rng = wsh.Range[RN_TABLE_HEADER].Cells[1, EXCEL_PERSONAL_COL_NUM];
			for (int r = 0; r < RowsQ; )
			{
				if (string.IsNullOrEmpty(rng.Offset[r + 1].Value as string))
				{
					wsh.Rows[rng.Offset[r + 1].Row].Delete();
					MaxRows--;
					RowsQ--;
					if (r < GroupInDB.from_1_qualif.Value)
						NeedCorrectBorders = true;
				}
				else
					r++;
			}
			wsh.Rows[(RowsQ + FirstRow).ToString() + ":" + (MaxRows + FirstRow - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);

			if (NeedCorrectBorders)
			{	// Ещё раз корректируем подчёркивание
				if (CompRounds.Contains(enRounds.Qualif2))
				{
					GlobalDefines.CorrectBordersInTotal(wsh,
														wsh.Range[RN_QUALIF_2].Offset[1].Cells[1, EXCEL_SUM_COL_OFS],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														RN_BALLS_FOR_PLACES);
				}

				if (CompRounds.Contains(enRounds.OneEighthFinal))
				{
					GlobalDefines.CorrectBordersInTotal(wsh,
														wsh.Range[RN_ONE_EIGHTH_FINAL].Offset[1],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														RN_BALLS_FOR_PLACES);
				}

				if (CompRounds.Contains(enRounds.QuaterFinal))
				{
					GlobalDefines.CorrectBordersInTotal(wsh,
														wsh.Range[RN_QUATERFINAL].Offset[1],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														RN_BALLS_FOR_PLACES);
				}

				if (CompRounds.Contains(enRounds.SemiFinal))
				{
					GlobalDefines.CorrectBordersInTotal(wsh,
														wsh.Range[RN_FINAL].Offset[1],	// Здесь должен быть RN_FINAL
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														RN_BALLS_FOR_PLACES); 
				}
			}

			if (!CurTask.m_GroupToExport.Rounds.First(arg => arg.id == enRounds.Total).ExtraOption.Value)
			{	// Удаляем столбец с баллами
				wsh.Columns[wsh.Range[RN_BALLS_FOR_PLACES].Column].Delete(MSExcel.XlDirection.xlToLeft);
			}
			
			return true;
		}
	}
}
