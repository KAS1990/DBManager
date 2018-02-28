using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Excel.Exporting.Tabs;
using DBManager.Scanning.XMLDataClasses;
using System.Windows;

namespace DBManager.Excel.Exporting.ExportingClasses
{
	public class CReportExporterBase
	{
		#region Название диапазонов
		protected string RN_COMP_NAME = "CompName";
		protected string RN_ROUND_DATE = "Date";
		protected string RN_FIRST_DATA_ROW = "FirstDataRow";
		protected string RN_MAIN_JUDGE = "MainJudge";
		protected string RN_MAIN_SECRETARY = "MainSecretary";
		protected string RN_REPORT_NAME = "ReportName";
		protected string RN_ROW_6 = "RowSix";
		protected string RN_SECOND_COL_NAME = "SecondColName";
		#endregion


		protected int EXCEL_MAX_LINES_IN_REPORTS = 150;
		

		public class CTask
		{
			public enReportTypes m_ReportType;
			public descriptions m_CompDesc;
		}

		protected readonly CTask m_Task = null;

		public CReportExporterBase()
		{
		
		}

		public CReportExporterBase(CTask Task)
		{
			m_Task = Task;
		}


		public static CReportExporterBase GetReportClass(CTask Task)
		{
			switch (Task.m_ReportType)
			{
				case enReportTypes.Qualif:
				case enReportTypes.Qualif2:
					return new CQualifExporter(Task);
				
				case enReportTypes.OneEighthFinal:
				case enReportTypes.QuaterFinal:
				case enReportTypes.SemiFinal:
					return new CMiddleSheetsExporter(Task);
				
				case enReportTypes.Final:
					return new CFinalExporter(Task);
				
				case enReportTypes.Total:
					return new CTotalExporter(Task);
				
				case enReportTypes.Team:
					return new CTeamExporter(Task);
				
				case enReportTypes.Personal:
					return new CPersonalExporter(Task);
				
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
		/// Книга, в которую добавляется отчёт. Добавление всегда происходит в конец
		/// </param>
		/// <param name="wbkTemplates">
		/// Книга с шаблонами отчётов
		/// </param>
		/// <returns></returns>
		public virtual bool CreateReport(MSExcel.Application excelApp,
											MSExcel.Workbook wbkTarget,
											MSExcel.Workbook wbkTemplates)
		{
			return false;
		}

								
		protected string CreateGroupName(CGroupItem GroupItem,
										CAgeGroup AgeGroup,
										out int SelectedStartYear,
										out int SelectedEndYear)
		{
			SelectedStartYear = GroupItem.YearsOfBirth[GroupItem.StartYearIndToExport];
			SelectedEndYear = GroupItem.YearsOfBirth[GroupItem.EndYearIndToExport];

			if (GroupItem.StartYearIndToExport == 0 &&
				GroupItem.EndYearIndToExport == GroupItem.YearsOfBirth.Count - 1)
			{	// Нужно вывести всех спорсменов группы => название равно CompSettings.AgeGroup.FullGroupName
				return AgeGroup.FullGroupName;
			}
			else if (SelectedStartYear != GlobalDefines.MIN_GROUP_YEAR &&
					 SelectedEndYear != GlobalDefines.MAX_GROUP_YEAR)
			{
				if (SelectedStartYear == SelectedEndYear)
				{
					return string.Format("{0} {1} г.р.",
										AgeGroup.Name,
										SelectedStartYear);
				}
				else
				{
					return string.Format("{0} {1}-{2} г.р.",
										AgeGroup.Name,
										SelectedStartYear,
										SelectedEndYear);
				}
			}
			else if (SelectedStartYear == GlobalDefines.MIN_GROUP_YEAR)
			{
				return string.Format("{0} {1} г.р. и старше",
									AgeGroup.Name,
									SelectedEndYear);
			}
			else
			{
				return string.Format("{0} {1} г.р. и моложе",
									AgeGroup.Name,
									SelectedStartYear);
			}
		}
	}
}
