using DBManager.Excel.Exporting.Tabs;
using System.Collections.Generic;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.Exporting.ExportingClasses
{
    public class CLeadReportInfo
    {
        /// <summary>
        /// Книга с протоколом трудности
        /// </summary>
        public MSExcel.Workbook m_wbkLeadReport = null;

        /// <summary>
        /// Номер строки, с которой начинается список участников
        /// Начинается с 1!!!
        /// </summary>
        public int m_FirstMemberRow = -1;

        /// <summary>
        /// Номер столбца, в котором находится место участника
        /// Начинается с 1!!!
        /// </summary>
        public int m_PlaceColumnIndex = -1;

        /// <summary>
        /// Номер столбца, в котором содержится ФИ участника.
        /// Начинается с 1!!!
        /// </summary>
        public int m_PersonalDataColumnIndex = -1;

        /// <summary>
        /// Номер столбца, в котором содержится г.р. участника.
        /// Начинается с 1!!!
        /// </summary>
        public int m_YearOfBirthColumnIndex = -1;

        /// <summary>
        /// Номер столбца, в котором содержится команда участника.
        /// Начинается с 1!!!
        /// </summary>
        public int m_TeamColumnIndex = -1;

        /// <summary>
        /// Сведения о группах в протоколе трудности
        /// ключ - идентификатор группы
        /// Если какая-то группа не найдена в словаре, то её нет в протоколе трудности
        /// </summary>
        public Dictionary<long, CGroupItem> m_dictLeadGroupInfos = new Dictionary<long, CGroupItem>();
        public List<string> m_LeadSheets = null;
    }
}
