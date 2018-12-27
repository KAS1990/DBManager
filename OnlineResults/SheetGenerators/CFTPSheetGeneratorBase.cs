using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.DBAdditionalDataClasses;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;


namespace DBManager.FTP.SheetGenerators
{
	public class CFTPSheetGeneratorBase
	{
		#region Название диапазонов
		protected string RN_FIRST_DATA_ROW = "FirstDataRow";
		protected string RN_SECOND_COL_NAME = "SecondColName";
		#endregion


		protected int EXCEL_MAX_LINES_IN_REPORTS = 150;


		public class CTask
		{
			public enFTPSheetGeneratorTypes m_GeneratorType;
			public List<CDBAdditionalClassBase> m_lstMembers;
			public List<enRounds> m_lstCompRounds;
			public string m_SecondColName;
			public long m_GroupId;
			public long m_CompId;
		}

		protected MSExcel.Worksheet m_wshTarget = null;


		public CFTPSheetGeneratorBase()
		{
		
		}


		public static CFTPSheetGeneratorBase GetGeneratorClass(enFTPSheetGeneratorTypes Type)
		{
			switch (Type)
			{
				case enFTPSheetGeneratorTypes.Qualif:
				case enFTPSheetGeneratorTypes.Qualif2:
				case enFTPSheetGeneratorTypes.Start:
					return new CQualifGenerator();

				case enFTPSheetGeneratorTypes.OneEighthFinal:
				case enFTPSheetGeneratorTypes.QuaterFinal:
				case enFTPSheetGeneratorTypes.SemiFinal:
					return new CMiddleSheetsGenerator();

				case enFTPSheetGeneratorTypes.Final:
					return new CFinalGenerator();

				case enFTPSheetGeneratorTypes.Total:
					return new CTotalGenerator();

				default:
					return null;
			}
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
		public virtual bool AddDataToWbk(MSExcel.Application excelApp,
											MSExcel.Workbook wbkTarget,
											MSExcel.Workbook wbkTemplates,
											CTask Task)
		{
			return false;
		}


		public void DetachFromSheet()
		{
			m_wshTarget = null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="wbkTarget"></param>
		/// <param name="lstCompRounds"></param>
		/// <param name="Round"></param>
		/// <param name="SheetIndex"></param>
		/// <param name="SheetName"></param>
		/// <param name="IsBefore"></param>
		/// <returns>
		/// true - лист найден в книге <paramref name="wbkTarget"/>
		/// false - его нужно туда вставить
		/// </returns>
		protected bool FindSheetPos(MSExcel.Workbook wbkTarget,
									List<enRounds> lstCompRounds,
									enRounds Round,
									out int SheetIndex,
									out string SheetName,
									out bool IsBefore)
		{
			List<string> RoundNamesAfterThis = (from round in lstCompRounds
												where round > Round
												select GlobalDefines.ROUND_NAMES[(byte)round].Replace('_', ' ')).ToList();

			IsBefore = true;
			SheetName = GlobalDefines.ROUND_NAMES[(byte)Round].Replace('_', ' ');
			SheetIndex = 1;
			for (; SheetIndex <= wbkTarget.Worksheets.Count; SheetIndex++)
			{
				if (wbkTarget.Worksheets[SheetIndex].Name == SheetName)
				{	// Нашли лист => используем его
					return true;
				}
				else if (RoundNamesAfterThis.Contains(wbkTarget.Worksheets[SheetIndex].Name))
				{	// Нашли лист, перед которым нужно вставить m_wshTarget
					break;
				}
			}
			if (SheetIndex == wbkTarget.Worksheets.Count + 1)
			{	// Добавляем в конец
				IsBefore = false;
				SheetIndex = wbkTarget.Worksheets.Count;
			}

			return false;
		}


		protected MSExcel.Worksheet AddSheetToWbk(MSExcel.Workbook wbkTarget,
													MSExcel.Workbook wbkTemplates,
													enFTPSheetGeneratorTypes Round,
													string SheetName,
													bool IsBefore,
													ref int SheetIndex)
		{
			MSExcel.Worksheet result = null;

			MSExcel.Worksheet wsh = null;
			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				wsh = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictFTPSheetTemplates[Round]];
			}

			if (IsBefore)
				wsh.Copy(Before: wbkTarget.Worksheets[SheetIndex]);
			else
			{
				wsh.Copy(After: wbkTarget.Worksheets[SheetIndex]);
				SheetIndex++;
			}

			// Лист, в который нужно будет вставлять данные
			result = wbkTarget.Worksheets[SheetIndex];
			result.Name = SheetName;

			try
			{
				MSExcel.Worksheet whs = wbkTarget.Worksheets[CFTPExporter.TEMP_SHEET_NAME];
				// Удаляем этот временный лист
				if (SheetIndex > whs.Index)
					SheetIndex--;
				whs.Delete();
			}
			catch
			{ }

			return result;
		}


		protected void ClearExistSheet(MSExcel.Workbook wbkTarget,
										MSExcel.Workbook wbkTemplates,
										enFTPSheetGeneratorTypes Round,
										MSExcel.Worksheet wsh)
		{
			MSExcel.Worksheet wshTemplate = null;
			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				wshTemplate = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictFTPSheetTemplates[Round]];
			}

			wshTemplate.Cells.Copy();
			wsh.Select();
			wsh.Range["A1"].Select();
			wsh.Paste();
			wbkTarget.Application.CutCopyMode = 0;
		}
	}
}
