using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;
using DBManager.RoundMembers.Converters;
using System.Globalization;
using DBManager.Scanning.DBAdditionalDataClasses;
using Microsoft.Office.Interop.Excel;

namespace DBManager.FTP.SheetGenerators
{
	public class CQualifGenerator : CFTPSheetGeneratorBase
	{
		#region Название диапазонов
		private string RN_PLACE_NUMBER = "PlaceNumber";
		#endregion

		#region Номера столбцов в листе Excel
		private int EXCEL_PLACE_COL_NUM = 1;
		private int EXCEL_PERSONAL_COL_NUM = 2;
		private int EXCEL_TEAM_COL_NUM = 3;
		private int EXCEL_YEAR_OF_BIRTH_COL_NUM = 4;
		private int EXCEL_GRADE_COL_NUM = 5;
		private int EXCEL_ROUTE1_COL_NUM = 6;
		private int EXCEL_ROUTE2_COL_NUM = 7;
		private int EXCEL_SUM_COL_NUM = 8;
		#endregion

		public class CQualifTask : CFTPSheetGeneratorBase.CTask
		{
			public int m_MembersAfterQualif = -1;
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
			CQualifTask CurTask = Task as CQualifTask;
			if (CurTask == null)
				throw new InvalidCastException("Invalid of type m_Task in DBManager.FTP.SheetGenerators.CQualifGenerator. Requires CQualifGenerator.CQualifTask");

			if (m_wshTarget == null)
			{
				string SheetName = "";
				int SheetIndex = -1; // Начинается с 1!!!
				bool IsBefore = true;
				switch (CurTask.m_GeneratorType)
				{
					case Global.enFTPSheetGeneratorTypes.Start:
						SheetName = Properties.Resources.resStartList;
						SheetIndex = 1;
						if (wbkTarget.Worksheets[SheetIndex].Name == SheetName)
						{	// Нашли лист
							m_wshTarget = wbkTarget.Worksheets[SheetIndex];
							ClearExistSheet(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, m_wshTarget);
						}
						break;

					case Global.enFTPSheetGeneratorTypes.Qualif:
					case Global.enFTPSheetGeneratorTypes.Qualif2:
						if (FindSheetPos(wbkTarget,
										CurTask.m_lstCompRounds,
										(enRounds)CurTask.m_GeneratorType,
										out SheetIndex,
										out SheetName,
										out IsBefore))
						{
							m_wshTarget = wbkTarget.Worksheets[SheetIndex];
							ClearExistSheet(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, m_wshTarget);
						}
						break;
				}

				if (m_wshTarget == null)
				{	// Такого листа нет
					m_wshTarget = AddSheetToWbk(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, SheetName, IsBefore, ref SheetIndex);
				}
			}
			else
			{	// Нужно удалить всё содержимое листа, т.к. каждый раз заполняем лист заново
				ClearExistSheet(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, m_wshTarget);
			}

			if (CurTask.m_GeneratorType == Global.enFTPSheetGeneratorTypes.Start)
			{	// В стартовом протоколе нужно сделать изменения в столбце "Место"
				m_wshTarget.Range[RN_PLACE_NUMBER].Value = Properties.Resources.resStartNumber;
				for (int i = 0; i < 3; i++)
					m_wshTarget.Range[RN_FIRST_DATA_ROW].Offset[i].Cells[1, EXCEL_PLACE_COL_NUM].Value = i + 1;
			}

			m_wshTarget.Range[RN_SECOND_COL_NAME].Value = CurTask.m_SecondColName;

			if (CurTask.m_GeneratorType == Global.enFTPSheetGeneratorTypes.Start)
			{	//	Убираем подсветку первой строки, т.к. в стартовом протоколе она не нужна
				m_wshTarget.Range[RN_FIRST_DATA_ROW].Interior.Pattern = Constants.xlNone;
				
				// Результаты в стартовом протоколе не нужны
				m_wshTarget.Columns[((char)(EXCEL_ROUTE1_COL_NUM + 'A' - 1)).ToString() + ":" + ((char)(EXCEL_SUM_COL_NUM + 'A' - 1)).ToString()].Delete();
			}

			for (int i = 0; i < CurTask.m_lstMembers.Count; i++)
			{
				CMemberAndResults MemberAndResults = CurTask.m_lstMembers[i] as CMemberAndResults;

				MSExcel.Range rng = m_wshTarget.Range[RN_FIRST_DATA_ROW].Offset[i];
								
				if (CurTask.m_MembersAfterQualif > 0)
				{
					if (i < CurTask.m_MembersAfterQualif && MemberAndResults.Place.HasValue)
					{	// Выделяем участника цветом, т.к. он вышел из квалификации. Образцом выступает первая строка
						rng.Interior.Color = m_wshTarget.Range[RN_FIRST_DATA_ROW].Cells[1, 1].Interior.Color;
						rng.Interior.Pattern = m_wshTarget.Range[RN_FIRST_DATA_ROW].Cells[1, 1].Interior.Pattern;
						rng.Interior.PatternTintAndShade = m_wshTarget.Range[RN_FIRST_DATA_ROW].Cells[1, 1].Interior.PatternTintAndShade;
						rng.Interior.TintAndShade = m_wshTarget.Range[RN_FIRST_DATA_ROW].Cells[1, 1].Interior.TintAndShade;
					}
					if (i == CurTask.m_MembersAfterQualif - 1)
					{	// Подчёркиваем участников, вышедших из квалификации
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThick;
					}
					else
						rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;
				}
				else
					rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThin;

				if (CurTask.m_GeneratorType != Global.enFTPSheetGeneratorTypes.Start)
					rng.Cells[1, EXCEL_PLACE_COL_NUM].Value = MemberAndResults.Place.HasValue ? GlobalDefines.EncodePlace((byte)MemberAndResults.Place.Value) : "";

				rng.Cells[1, EXCEL_PERSONAL_COL_NUM].Value = MemberAndResults.MemberInfo.SurnameAndName;
				rng.Cells[1, EXCEL_TEAM_COL_NUM].Value = MemberAndResults.MemberInfo.SecondCol;
				rng.Cells[1, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = MemberAndResults.MemberInfo.YearOfBirth;

				GradeMarkupConverter conv = new GradeMarkupConverter();
				rng.Cells[1, EXCEL_GRADE_COL_NUM].Value = conv.Convert(MemberAndResults.MemberInfo.InitGrade,
																		MemberAndResults.MemberInfo.InitGrade.GetType(),
																		null,
																		CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));

				if (CurTask.m_GeneratorType != Global.enFTPSheetGeneratorTypes.Start)
				{	// Результаты в стартовом протоколе не нужны
					rng.Cells[1, EXCEL_ROUTE1_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route1.Time);
					rng.Cells[1, EXCEL_ROUTE2_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route2.Time);
					rng.Cells[1, EXCEL_SUM_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Sum.Time);
				}
			}

			// Удаляем лишние строки
			m_wshTarget.Rows[(CurTask.m_lstMembers.Count + m_wshTarget.Range[RN_FIRST_DATA_ROW].Row).ToString() +
								":" +
								(EXCEL_MAX_LINES_IN_REPORTS + m_wshTarget.Range[RN_FIRST_DATA_ROW].Row - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);

			return true;
		}
	}
}
