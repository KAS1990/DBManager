using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.RoundMembers.Converters;
using System.Globalization;

namespace DBManager.FTP.SheetGenerators
{
	public class CTotalGenerator : CFTPSheetGeneratorBase
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
		protected string RN_FIRST_LOOSER_DATA_ROW = "FirstLooserDataRow";
		protected string RN_TABLE_HEADER = "TableHeader";


		public class CTotalTask : CFTPSheetGeneratorBase.CTask
		{
			public int m_FirstMiddleSheetRoundMembers = -1;
			public int m_MembersAfter1stQualif = -1;
		}
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="excelApp">
		/// Приложение Excel, в котором открыты книги
		/// <paramref name="wbkTarget"/> и <paramref name="wbkTemplates"/>
		/// </param>
		/// <param name="wbkTarget">
		/// Книга, в которой находится лист добавляется лист. Добавление происходит в то место, в которое нужно
		/// </param>
		/// <param name="wbkTemplates">
		/// Книга с шаблонами листов
		/// </param>
		/// <returns></returns>
		public override bool AddDataToWbk(MSExcel.Application excelApp,
											MSExcel.Workbook wbkTarget,
											MSExcel.Workbook wbkTemplates,
											CTask Task)
		{
			CTotalTask CurTask = Task as CTotalTask;
			if (CurTask == null)
				throw new InvalidCastException("Invalid of type m_Task in DBManager.FTP.SheetGenerators.CTotalGenerator. Requires CTotalGenerator.CTotalTask");

			string SheetName = Properties.Resources.resTotalReport;
			if (m_wshTarget != null)
			{	// Итоговый протокол всегда будем формировать заново
				m_wshTarget.Delete();
				m_wshTarget = null;
			}
			else
			{
				try
				{
					m_wshTarget = wbkTarget.Worksheets[SheetName];
					m_wshTarget.Delete();
					m_wshTarget = null;
				}
				catch
				{ }
			}

			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				m_wshTarget = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictFTPSheetTemplates[CurTask.m_GeneratorType]];
			}

			m_wshTarget.Copy(After: wbkTarget.Worksheets[wbkTarget.Worksheets.Count]);
			m_wshTarget = wbkTarget.Worksheets[wbkTarget.Worksheets.Count];
			
			m_wshTarget.Range[RN_SECOND_COL_NAME].Value = CurTask.m_SecondColName;

			if (!CurTask.m_lstCompRounds.Contains(enRounds.Qualif2))
			{
				// Удаляем столбцы со второй квалификацией
				int ColQ = m_wshTarget.Range[RN_QUALIF_2].Columns.Count;
				for (int col = 0; col < ColQ; col++)
					m_wshTarget.Columns[m_wshTarget.Range[RN_QUALIF_2].Column].Delete();
			}

			// Удаляем столбцы для тех раундов, которых нет
			if (!CurTask.m_lstCompRounds.Contains(enRounds.OneEighthFinal))
				m_wshTarget.Columns[m_wshTarget.Range[RN_ONE_EIGHTH_FINAL].Column].Delete();
			if (!CurTask.m_lstCompRounds.Contains(enRounds.QuaterFinal))
				m_wshTarget.Columns[m_wshTarget.Range[RN_QUATERFINAL].Column].Delete();
			if (!CurTask.m_lstCompRounds.Contains(enRounds.SemiFinal))
				m_wshTarget.Columns[m_wshTarget.Range[RN_SEMIFINAL].Column].Delete();

			int RowsQ = 0;
			int FirstRow = m_wshTarget.Range[RN_FIRST_DATA_ROW].Row;
			foreach (CMemberInTotal MemberInTotal in CurTask.m_lstMembers)
			{
				RowsQ++;

				// Заносим результат всех раундов для участника
				if (MemberInTotal.QualifResults != null)
				{
					m_wshTarget.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_ROUTE1_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.QualifResults.Route1.Time);
					m_wshTarget.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_ROUTE2_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.QualifResults.Route2.Time);
					m_wshTarget.Range[RN_QUALIF_1].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.QualifResults.Sum.Time);
				}
				if (MemberInTotal.Qualif2Results != null)
				{
					m_wshTarget.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_ROUTE1_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.Qualif2Results.Route1.Time);
					m_wshTarget.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_ROUTE2_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.Qualif2Results.Route2.Time);
					m_wshTarget.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.Qualif2Results.Sum.Time);
				}
				if (MemberInTotal.OneEighthFinalResults != null)
				{
					m_wshTarget.Range[RN_ONE_EIGHTH_FINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.OneEighthFinalResults.Sum.Time);
				}
				if (MemberInTotal.QuaterFinalResults != null)
				{
					m_wshTarget.Range[RN_QUATERFINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.QuaterFinalResults.Sum.Time);
				}
				if (MemberInTotal.SemiFinalResults != null)
				{
					m_wshTarget.Range[RN_SEMIFINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.SemiFinalResults.Sum.Time);
				}
				if (MemberInTotal.FinalResults != null)
				{
					m_wshTarget.Range[RN_FINAL].Offset[RowsQ].Value = GlobalDefines.EncodeSpeedResult(MemberInTotal.FinalResults.Sum.Time);
				}
				
				m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_PLACE_COL_NUM].Value = MemberInTotal.Place.HasValue ? GlobalDefines.EncodePlace((byte)MemberInTotal.Place.Value) : "";

				m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_PERSONAL_COL_NUM].Value = MemberInTotal.MemberInfo.SurnameAndName;
				m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_TEAM_COL_NUM].Value = MemberInTotal.MemberInfo.SecondCol;
				m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = MemberInTotal.MemberInfo.YearOfBirth;

				GradeMarkupConverter conv = new GradeMarkupConverter();
				m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_GRADE_COL_NUM].Value = conv.Convert(MemberInTotal.MemberInfo.InitGrade,
																								MemberInTotal.MemberInfo.InitGrade.GetType(),
																								null,
																								CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));
				if (MemberInTotal.TotalGrade.HasValue)
				{
					m_wshTarget.Range[RN_RESULT_GRADE].Offset[RowsQ].Value = conv.Convert(MemberInTotal.TotalGrade,
																						MemberInTotal.TotalGrade.GetType(),
																						null,
																						CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));
				}
			}

			// Редактируем сетку
			// в квалификации
			MSExcel.Range rng = m_wshTarget.Range[m_wshTarget.Cells[CurTask.m_FirstMiddleSheetRoundMembers + FirstRow, EXCEL_PLACE_COL_NUM],
													m_wshTarget.Cells[RowsQ + FirstRow - 1, EXCEL_GRADE_COL_NUM + EXCEL_SUM_COL_OFS]];
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

			// В выполненном разряде
			rng = m_wshTarget.Range[m_wshTarget.Range[RN_RESULT_GRADE].Offset[CurTask.m_FirstMiddleSheetRoundMembers + 1],
									m_wshTarget.Range[RN_RESULT_GRADE].Offset[RowsQ + FirstRow]];
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
			rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

			if (CurTask.m_lstCompRounds.Contains(enRounds.Qualif2))
			{	// Во второй квалификации
				// Удаляем подчёркивание, где оно не нужно и добавляем там, где нужно
				rng = m_wshTarget.Range[m_wshTarget.Cells[CurTask.m_FirstMiddleSheetRoundMembers + FirstRow, EXCEL_PLACE_COL_NUM],
										m_wshTarget.Cells[CurTask.m_MembersAfter1stQualif + FirstRow - 1, m_wshTarget.Range[RN_QUALIF_2].Column + EXCEL_SUM_COL_OFS - 1]];
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight = MSExcel.XlBorderWeight.xlThin;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

				// Удаляем сетку, где она не нужна
				rng = m_wshTarget.Range[m_wshTarget.Range[RN_QUALIF_2].Offset[CurTask.m_MembersAfter1stQualif + 1].Cells[1, 1],
										m_wshTarget.Range[RN_QUALIF_2].Offset[RowsQ].Cells[1, EXCEL_SUM_COL_OFS]];
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeRight].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
					rng.Borders[MSExcel.XlBordersIndex.xlInsideVertical].LineStyle = MSExcel.Constants.xlNone;
				rng.Interior.Pattern = MSExcel.Constants.xlNone;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;

				// В выполненном разряде
				// Выполняем подчёркивание
				rng = m_wshTarget.Range[RN_RESULT_GRADE].Offset[CurTask.m_MembersAfter1stQualif + 1];
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].LineStyle =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlInsideHorizontal].Weight =
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].LineStyle = MSExcel.XlLineStyle.xlContinuous;
				rng.Borders[MSExcel.XlBordersIndex.xlEdgeTop].Weight = MSExcel.XlBorderWeight.xlMedium;
			}

			bool NeedCorrectBorders = false;
			int MaxRows = EXCEL_MAX_LINES_IN_REPORTS;
			rng = m_wshTarget.Range[RN_TABLE_HEADER].Cells[1, EXCEL_PERSONAL_COL_NUM];
			for (int r = 0; r < RowsQ; )
			{
				if (string.IsNullOrEmpty(rng.Offset[r + 1].Value as string))
				{
					m_wshTarget.Rows[rng.Offset[r + 1].Row].Delete();
					MaxRows--;
					RowsQ--;
					if (r < CurTask.m_MembersAfter1stQualif)
						NeedCorrectBorders = true;
				}
				else
					r++;
			}
			m_wshTarget.Rows[(RowsQ + FirstRow).ToString() + ":" + (MaxRows + FirstRow - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);

			if (NeedCorrectBorders)
			{	// Ещё раз корректируем подчёркивание
				if (CurTask.m_lstCompRounds.Contains(enRounds.Qualif2))
				{
					GlobalDefines.CorrectBordersInTotal(m_wshTarget,
														m_wshTarget.Range[RN_QUALIF_2].Offset[1].Cells[1, EXCEL_SUM_COL_OFS],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														null);
				}

				if (CurTask.m_lstCompRounds.Contains(enRounds.OneEighthFinal))
				{
					GlobalDefines.CorrectBordersInTotal(m_wshTarget,
														m_wshTarget.Range[RN_ONE_EIGHTH_FINAL].Offset[1],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														null);
				}

				if (CurTask.m_lstCompRounds.Contains(enRounds.QuaterFinal))
				{
					GlobalDefines.CorrectBordersInTotal(m_wshTarget,
														m_wshTarget.Range[RN_QUATERFINAL].Offset[1],
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														null);
				}

				if (CurTask.m_lstCompRounds.Contains(enRounds.SemiFinal))
				{
					GlobalDefines.CorrectBordersInTotal(m_wshTarget,
														m_wshTarget.Range[RN_FINAL].Offset[1],	// Здесь должен быть RN_FINAL
														RowsQ,
														EXCEL_PLACE_COL_NUM,
														RN_RESULT_GRADE,
														null);
				}
			}

			return true;
		}
	}
}
