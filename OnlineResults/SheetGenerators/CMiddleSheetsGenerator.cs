using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.RoundMembers.Converters;
using System.Globalization;
using System.Drawing;

namespace DBManager.FTP.SheetGenerators
{
	public class CMiddleSheetsGenerator : CFTPSheetGeneratorBase
	{
		#region Номера столбцов в листе Excel
		private int EXCEL_PERSONAL_COL_NUM = 4;
		private int EXCEL_TEAM_COL_NUM = 5;
		private int EXCEL_YEAR_OF_BIRTH_COL_NUM = 6;
		private int EXCEL_GRADE_COL_NUM = 7;
		private int EXCEL_ROUTE1_COL_NUM = 8;
		private int EXCEL_ROUTE2_COL_NUM = 9;
		private int EXCEL_SUM_COL_NUM = 10;
		#endregion


		public class CMiddleSheetsTask : CFTPSheetGeneratorBase.CTask
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
			CMiddleSheetsTask CurTask = Task as CMiddleSheetsTask;
			if (CurTask == null)
				throw new InvalidCastException("Invalid of type m_Task in DBManager.FTP.SheetGenerators.CMiddleSheetsGenerator. Requires CMiddleSheetsGenerator.CMiddleSheetsTask");

			if (m_wshTarget == null)
			{
				string SheetName = null;
				int SheetIndex = -1; // Начинается с 1!!!
				bool IsBefore = true;

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
				else
					m_wshTarget = AddSheetToWbk(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, SheetName, IsBefore, ref SheetIndex);
			}
			else
			{	// Нужно удалить всё содержимое листа, т.к. каждый раз заполняем лист заново
				ClearExistSheet(wbkTarget, wbkTemplates, CurTask.m_GeneratorType, m_wshTarget);
			}

			m_wshTarget.Range[RN_SECOND_COL_NAME].Value = CurTask.m_SecondColName;

			TimeSpan? FirstMemberSum = null;

			for (int i = 0; i < CurTask.m_lstMembers.Count; i++)
			{
				CMemberAndResults MemberAndResults = CurTask.m_lstMembers[i] as CMemberAndResults;

				MSExcel.Range rng = m_wshTarget.Range[RN_FIRST_DATA_ROW].Offset[i];

				rng.Cells[1, EXCEL_PERSONAL_COL_NUM].Value = MemberAndResults.MemberInfo.SurnameAndName;
				rng.Cells[1, EXCEL_TEAM_COL_NUM].Value = MemberAndResults.MemberInfo.SecondCol;
				rng.Cells[1, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = MemberAndResults.MemberInfo.YearOfBirth;

				GradeMarkupConverter conv = new GradeMarkupConverter();
				rng.Cells[1, EXCEL_GRADE_COL_NUM].Value = conv.Convert(MemberAndResults.MemberInfo.InitGrade,
																		MemberAndResults.MemberInfo.InitGrade.GetType(),
																		null,
																		CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));

				rng.Cells[1, EXCEL_ROUTE1_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route1.Time);
				rng.Cells[1, EXCEL_ROUTE2_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route2.Time);
				rng.Cells[1, EXCEL_SUM_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Sum.Time);

				if ((i + 1) % 2 == 0)
				{	// Чётная строка => определяем победителя в забеге
					for (int col = 1; col <= rng.Columns.Count; col++)
					{
						if (col == 2)
						{	// Пропускаем столбец "Забег"
							continue;
						}

						if (FirstMemberSum.HasValue && MemberAndResults.Results.Sum.Time.HasValue)
						{	// Оба участника в забеге имеют результаты
							if (FirstMemberSum < MemberAndResults.Results.Sum.Time)
							{	// Победил первый спортсмен
								rng.Offset[-1, 0].Cells[1, col].Interior.Pattern = MSExcel.Constants.xlSolid;
								rng.Offset[-1, 0].Cells[1, col].Interior.PatternColorIndex = MSExcel.Constants.xlAutomatic;
								rng.Offset[-1, 0].Cells[1, col].Interior.Color = Color.Yellow.ToExcelColor();

								rng.Cells[1, col].Interior.Pattern = MSExcel.Constants.xlNone;
							}
							else
							{
								rng.Offset[-1, 0].Cells[1, col].Interior.Pattern = MSExcel.Constants.xlNone;

								rng.Cells[1, col].Interior.Pattern = MSExcel.Constants.xlSolid;
								rng.Cells[1, col].Interior.PatternColorIndex = MSExcel.Constants.xlAutomatic;
								rng.Cells[1, col].Interior.Color = Color.Yellow.ToExcelColor();
							}
						}
						else
						{
							rng.Offset[-1, 0].Cells[1, col].Interior.Pattern =
								rng.Cells[1, col].Interior.Pattern = MSExcel.Constants.xlNone;
						}
					}
				}
				else
					FirstMemberSum = MemberAndResults.Results.Sum.Time;
			}

			return true;
		}
	}
}
