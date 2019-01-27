using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public class StartListWorksheetHelper
    {
        #region Consts

        const int FIRST_DATA_ROW_INDEX = 8;

        const int NUMBER_COLUMN_INDEX = 1;
        const int NAME_AND_LASTNAME_COLUMN_INDEX = 2;
        const int TEAM_COLUMN_INDEX = 3;
        const int YoB_COLUMN_INDEX = 4;
        const int GRADE_COLUMN_INDEX = 5;

        #endregion

        readonly MSExcel.Workbook m_wbk = null;
        readonly MSExcel.Worksheet m_wshStartList = null;

        public StartListWorksheetHelper(MSExcel.Workbook wbk)
        {
            m_wbk = wbk;
            m_wshStartList = m_wbk.Worksheets[1];
        }

        public void PrepareTable(int requiredRowCount)
        {
            // Последняя строка с таблицы
            int existingRowsCount = 0;
            while (!string.IsNullOrEmpty(m_wshStartList.Cells[existingRowsCount + FIRST_DATA_ROW_INDEX, NUMBER_COLUMN_INDEX].Value?.ToString()))
                existingRowsCount++;

            if (existingRowsCount == requiredRowCount)
            {   // Таблица уже имеет верный размер
                return;
            }
            else if (existingRowsCount > requiredRowCount)
            {   // Удаляем лишние строки
                m_wshStartList
                    .Rows[$"{requiredRowCount + FIRST_DATA_ROW_INDEX}:{existingRowsCount + FIRST_DATA_ROW_INDEX - 1}"]
                    .Delete(MSExcel.XlDirection.xlUp);
            }
            else
            {   // Добавляем недостающие строки
                m_wshStartList
                    .Rows[$"{existingRowsCount + FIRST_DATA_ROW_INDEX}:{requiredRowCount + FIRST_DATA_ROW_INDEX - 1}"]
                    .Insert(MSExcel.XlDirection.xlDown);

                // Вставляем в новые строки форматы
                string address = string.Format("{0}{1}:{2}{1}",
                                                NUMBER_COLUMN_INDEX.ToLatinCapitalLetter(),
                                                FIRST_DATA_ROW_INDEX,
                                                GRADE_COLUMN_INDEX.ToLatinCapitalLetter());
                m_wshStartList.Range[address].Copy();

                address = string.Format("{0}{1}:{2}{3}",
                                            NUMBER_COLUMN_INDEX.ToLatinCapitalLetter(),
                                            existingRowsCount + FIRST_DATA_ROW_INDEX,
                                            GRADE_COLUMN_INDEX.ToLatinCapitalLetter(),
                                            requiredRowCount + FIRST_DATA_ROW_INDEX - 1);
                m_wshStartList.Range[address].PasteSpecial(MSExcel.XlPasteType.xlPasteFormats);
                m_wbk.Application.CutCopyMode = 0;
            }
        }

        public void SetMember(CFullMemberInfo memberInfo, int index)
        {
            int row = index + FIRST_DATA_ROW_INDEX;

            m_wshStartList.Cells[row, NUMBER_COLUMN_INDEX].Value = index + 1;
            m_wshStartList.Cells[row, NAME_AND_LASTNAME_COLUMN_INDEX].Value = memberInfo.SurnameAndName;
            m_wshStartList.Cells[row, TEAM_COLUMN_INDEX].Value = memberInfo.SecondCol;
            m_wshStartList.Cells[row, YoB_COLUMN_INDEX].Value = memberInfo.YearOfBirthForShow;
            m_wshStartList.Cells[row, GRADE_COLUMN_INDEX].Value = memberInfo.InitGradeForShow;
        }
    }
}
