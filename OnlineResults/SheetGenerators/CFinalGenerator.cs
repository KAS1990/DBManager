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
	public class CFinalGenerator : CFTPSheetGeneratorBase
	{
		#region Номера столбцов в листе Excel
		private int EXCEL_PERSONAL_COL_NUM = 2;
		private int EXCEL_TEAM_COL_NUM = 3;
		private int EXCEL_YEAR_OF_BIRTH_COL_NUM = 4;
		private int EXCEL_GRADE_COL_NUM = 5;
		private int EXCEL_ROUTE1_COL_NUM = 6;
		private int EXCEL_ROUTE2_COL_NUM = 7;
		private int EXCEL_SUM_COL_NUM = 8;
		private int EXCEL_PLACE_COL_NUM = 9;
		#endregion

		protected string RN_FIRST_DATA_ROW_12 = "FirstDataRow12";
		protected string RN_FIRST_DATA_ROW_34 = "FirstDataRow34";


		public class CFinalTask : CFTPSheetGeneratorBase.CTask
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
			CFinalTask CurTask = Task as CFinalTask;
			if (CurTask == null)
				throw new InvalidCastException("Invalid of type m_Task in DBManager.FTP.SheetGenerators.CFinalGenerator. Requires CFinalGenerator.CFinalTask");

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

				MSExcel.Range rng = null;

				if (MemberAndResults.StartNumber.Value < 3)
					rng = m_wshTarget.Range[RN_FIRST_DATA_ROW_34];
				else
					rng = m_wshTarget.Range[RN_FIRST_DATA_ROW_12];
				if (MemberAndResults.StartNumber.Value % 2 == 0)
					rng = rng.Offset[1];
				
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

				if (MemberAndResults.Place.HasValue)
				{
					rng.Cells[1, EXCEL_PLACE_COL_NUM].Value = GlobalDefines.EncodePlace((byte)MemberAndResults.Place.Value);
					if (MemberAndResults.Place.Value < 4)
					{	// Места с первого по третье подсвечиваем красным
						rng.Cells[1, EXCEL_PLACE_COL_NUM].Font.Color = Color.Red.ToExcelColor();
					}
				}
				else
					rng.Cells[1, EXCEL_PLACE_COL_NUM].Value =  "";

				if ((i + 1) % 2 == 0)
				{	// Чётная строка => определяем победителя в забеге
					for (int col = 0; col < rng.Columns.Count; col++)
					{
						if (FirstMemberSum.HasValue && MemberAndResults.Results.Sum.Time.HasValue)
						{	// Оба участника в забеге имеют результаты
							if (FirstMemberSum < MemberAndResults.Results.Sum.Time)
							{	// Победил первый спортсмен
								rng.Offset[-1, 0].Cells[1, col + 1].Interior.Pattern = MSExcel.Constants.xlSolid;
								rng.Offset[-1, 0].Cells[1, col + 1].Interior.PatternColorIndex = MSExcel.Constants.xlAutomatic;
								rng.Offset[-1, 0].Cells[1, col + 1].Interior.Color = Color.Yellow.ToExcelColor();

								rng.Cells[1, col + 1].Interior.Pattern = MSExcel.Constants.xlNone;
							}
							else
							{
								rng.Offset[-1, 0].Cells[1, col + 1].Interior.Pattern = MSExcel.Constants.xlNone;

								rng.Cells[1, col + 1].Interior.Pattern = MSExcel.Constants.xlSolid;
								rng.Cells[1, col + 1].Interior.PatternColorIndex = MSExcel.Constants.xlAutomatic;
								rng.Cells[1, col + 1].Interior.Color = Color.Yellow.ToExcelColor();
							}
						}
						else
						{
							rng.Offset[-1, 0].Cells[1, col + 1].Interior.Pattern =
								rng.Cells[1, col + 1].Interior.Pattern = MSExcel.Constants.xlNone;
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
