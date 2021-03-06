﻿using System;
using System.Globalization;
using System.Threading;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public class SetupWorksheetHelper
    {
        #region Consts
        private const string SETUP_SHEET_NAME = "Setup";
        private const string RN_FLAGS = "FLAGS";
        private const string RN_HIDE_FLAGS = "HideFlags";
        private const string RN_TRANS_FLAGS = "TransFlags";
        private const string RN_ON_SHEET_FLAGS = "OnSheetFlags";
        private const string RN_INIT_ON_SHEET_FLAGS_VALUE = "InitOnSheetFlagsValue";
        private const string RN_REQUEST = "Request";
        private const string RN_COMP_NAME_INDEX = "CompNameIndex";
        private const string RN_MAIN_JUDGE_INDEX = "MainJudgeIndex";
        private const string RN_MAIN_SECRETARY_INDEX = "MainSecretaryIndex";
        private const string RN_ROW_6_INDEX = "Row6Index";
        private const string RN_START_COMP_DATE = "StartCompDate";
        private const string RN_END_COMP_DATE = "EndCompDate";
        private const string RN_START_GROUP_YEAR = "StartGroupYear";
        private const string RN_END_GROUP_YEAR_INDEX = "EndGroupYearIndex";
        private const int REQUEST_LOAD_FLAGS = 1;
        private const int REQUEST_CLEAR_BOOK_SILENTLY = 2;
        private const int REQUEST_FILL_WBK_BASED_ON_SETUP_SHEET = 3;
        private const int CLEAR_WBK_FLAGS_VALUE = 64;
        private const int INIT_HIDE_FLAGS_VALUE = 0;
        private const int INIT_TRANS_FLAGS_VALUE = 0;

        #endregion

        private readonly WorkbookDataFileWrapper m_DataFileWrapper = null;
        private readonly MSExcel.Workbook m_wbk = null;
        private readonly MSExcel.Worksheet m_wshSetup = null;

        #region CompName

        private string m_CompName = null;
        public string CompName
        {
            get
            {
                if (m_CompName == null)
                {
                    m_CompName = GetStringByIndex(RN_COMP_NAME_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.CompetitionName);
                }

                return m_CompName;
            }
            set
            {
                SetStringByIndex(RN_COMP_NAME_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.CompetitionName,
                        value);

                m_CompName = value;
            }
        }

        #endregion

        #region MainJudge

        private string m_MainJudge = null;
        public string MainJudge
        {
            get
            {
                if (m_MainJudge == null)
                {
                    m_MainJudge = GetStringByIndex(RN_MAIN_JUDGE_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.MainJudge);
                }

                return m_MainJudge;
            }
            set
            {
                SetStringByIndex(RN_MAIN_JUDGE_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.MainJudge,
                        value);

                m_MainJudge = value;
            }
        }

        #endregion

        #region MainSecretary

        private string m_MainSecretary = null;
        public string MainSecretary
        {
            get
            {
                if (m_MainSecretary == null)
                {
                    m_MainSecretary = GetStringByIndex(RN_MAIN_SECRETARY_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.MainSecretary);
                }

                return m_MainSecretary;
            }
            set
            {
                SetStringByIndex(RN_MAIN_SECRETARY_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.MainSecretary,
                        value);

                m_MainSecretary = value;
            }
        }

        #endregion

        #region Row6

        private string m_Row6 = null;
        public string Row6
        {
            get
            {
                if (m_Row6 == null)
                {
                    m_Row6 = GetStringByIndex(RN_ROW_6_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.Row6);
                }

                return m_Row6;
            }
            set
            {
                SetStringByIndex(RN_ROW_6_INDEX,
                        WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType.Row6,
                        value);

                m_Row6 = value;
            }
        }

        #endregion

        #region StartCompDate

        private DateTime? m_StartCompDate = null;
        public DateTime? StartCompDate
        {
            get
            {
                if (m_StartCompDate == null)
                {
                    m_StartCompDate = GetDate(RN_START_COMP_DATE);
                }

                return m_StartCompDate;
            }
            set
            {
                SetDate(RN_START_COMP_DATE, value);

                m_StartCompDate = value;
            }
        }

        #endregion

        #region EndCompDate

        private DateTime? m_EndCompDate = null;
        public DateTime? EndCompDate
        {
            get
            {
                if (m_EndCompDate == null)
                {
                    m_EndCompDate = GetDate(RN_END_COMP_DATE);
                }

                return m_EndCompDate;
            }
            set
            {
                if (value != StartCompDate)
                    SetDate(RN_END_COMP_DATE, value);

                m_EndCompDate = value;
            }
        }

        #endregion

        #region StartGroupYear

        private int? m_StartGroupYear = null;
        public int? StartGroupYear
        {
            get
            {
                if (m_StartGroupYear == null)
                {
                    if (m_wshSetup == null)
                        throw new InvalidOperationException("m_wshSetup should not be null");

                    string startGroupYearRaw = m_wshSetup.Range[RN_START_GROUP_YEAR].Value.ToString();
                    int startGroupYear;
                    if (!string.IsNullOrEmpty(startGroupYearRaw)
                        && int.TryParse(startGroupYearRaw, out startGroupYear))
                    {
                        m_StartGroupYear = startGroupYear;
                    }
                }

                return m_StartGroupYear;
            }
            set
            {
                if (m_wshSetup == null)
                    throw new InvalidOperationException("m_wshSetup should not be null");

                m_wshSetup.Range[RN_START_GROUP_YEAR].Value = value.HasValue
                            ? value.Value.ToString()
                            : "";

                m_StartGroupYear = value;
            }
        }

        #endregion

        #region EndGroupYear

        private int? m_EndGroupYear = null;
        public int? EndGroupYear
        {
            get
            {
                if (m_EndGroupYear == null)
                {
                    if (m_wshSetup == null)
                        throw new InvalidOperationException("m_wshSetup should not be null");
                    if (StartGroupYear == null)
                        throw new InvalidOperationException("StartGroupYear should not be null");

                    string endGroupYearIndexRaw = m_wshSetup.Range[RN_END_GROUP_YEAR_INDEX].Value.ToString();
                    int endGroupYearIndex;
                    if (!string.IsNullOrEmpty(endGroupYearIndexRaw)
                        && int.TryParse(endGroupYearIndexRaw, out endGroupYearIndex))
                    {
                        switch (endGroupYearIndex)
                        {
                            case 0: // Год не введён
                                m_EndGroupYear = null;
                                break;

                            case 1: // и моложе
                                m_EndGroupYear = (int)enEndYearSpecVals.AndYounger;
                                break;

                            case 2: // и старше
                                m_EndGroupYear = (int)enEndYearSpecVals.AndElder;
                                break;

                            default:
                                m_EndGroupYear = StartGroupYear + 1 + endGroupYearIndex - 3;
                                break;
                        }
                    }
                }

                return m_EndGroupYear;
            }
            set
            {
                if (m_wshSetup == null)
                    throw new InvalidOperationException("m_wshSetup should not be null");
                if (StartGroupYear == null)
                    throw new InvalidOperationException("StartGroupYear should not be null");

                if (value == null)
                    m_wshSetup.Range[RN_END_GROUP_YEAR_INDEX].Value = 0;

                switch (value.Value)
                {
                    case (int)enEndYearSpecVals.AndYounger:
                        m_wshSetup.Range[RN_END_GROUP_YEAR_INDEX].Value = 1;
                        break;

                    case (int)enEndYearSpecVals.AndElder:
                        m_wshSetup.Range[RN_END_GROUP_YEAR_INDEX].Value = 2;
                        break;

                    default:
                        m_wshSetup.Range[RN_END_GROUP_YEAR_INDEX].Value = value.Value - StartGroupYear + 3 - 1;
                        break;
                }

                m_EndGroupYear = value;
            }
        }

        #endregion

        #region FLAGS

        private ushort? m_FLAGS = null;
        public ushort FLAGS
        {
            get
            {
                if (m_FLAGS == null)
                {
                    m_FLAGS = Convert.ToUInt16(m_wshSetup.Range[RN_FLAGS].Value);
                }

                return m_FLAGS ?? 0;
            }
            set
            {
                m_wshSetup.Range[RN_FLAGS].Value = value;

                m_FLAGS = value;
            }
        }

        #endregion

        #region Constructors

        public SetupWorksheetHelper(MSExcel.Workbook wbk)
        {
            m_wbk = wbk;
            m_wshSetup = m_wbk.Worksheets[SETUP_SHEET_NAME];
        }

        public SetupWorksheetHelper(WorkbookDataFileWrapper dataFileWrapper,
            MSExcel.Workbook wbk
            )
        {
            m_DataFileWrapper = dataFileWrapper;
            m_wbk = wbk;
            if (m_wbk != null)
                m_wshSetup = m_wbk.Worksheets[SETUP_SHEET_NAME];
        }

        #endregion

        public bool PrepareSheetToClearWorkbook(out string message)
        {
            message = null;

            if (m_wshSetup == null)
            {
                message = $"error in PrepareSheetToClearWorkbook: m_wshSetup == null";
                return false;
            }

            try
            {
                FLAGS = CLEAR_WBK_FLAGS_VALUE;
                m_wshSetup.Range[RN_HIDE_FLAGS].Value = INIT_HIDE_FLAGS_VALUE;
                m_wshSetup.Range[RN_TRANS_FLAGS].Value = INIT_TRANS_FLAGS_VALUE;
                m_wshSetup.Range[RN_ON_SHEET_FLAGS].Value = m_wshSetup.Range[RN_INIT_ON_SHEET_FLAGS_VALUE].Value;
                SaveAllFlags();

                m_wshSetup.Range[RN_REQUEST].Value = REQUEST_CLEAR_BOOK_SILENTLY; // Посылаем запрос на молчаливую очистку книги при следующем открытии
            }
            catch (Exception ex)
            {
                message = $"exception in PrepareSheetToClearWorkbook: {ex.Message}";
                return false;
            }

            return true;
        }

        public void SaveAllFlags()
        {
            m_wshSetup.Range[RN_REQUEST].Value = REQUEST_LOAD_FLAGS; // Посылаем запрос на сохранение флагов
            Thread.Sleep(100);
        }

        public void SendRequestToFillWbkBasedOnSetupSheet()
        {
            m_wshSetup.Range[RN_REQUEST].Value = REQUEST_FILL_WBK_BASED_ON_SETUP_SHEET;
            Thread.Sleep(3000);
        }

        private string GetStringByIndex(string excelRangeName,
            WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType itemType)
        {
            if (m_DataFileWrapper == null)
                throw new InvalidOperationException("m_DataFileWrapper should not be null");
            if (m_wshSetup == null)
                throw new InvalidOperationException("m_wshSetup should not be null");

            string res = null;
            string indexRaw = m_wshSetup.Range[excelRangeName].Value?.ToString();
            int index = -1;
            if (!string.IsNullOrEmpty(indexRaw)
                && int.TryParse(indexRaw, out index))
            {
                var stringsInFile = m_DataFileWrapper.GetStrings(itemType);
                if (index < stringsInFile.Count && index >= 0)
                    res = stringsInFile[index];
            }

            return res;
        }

        private void SetStringByIndex(string excelRangeName,
            WorkbookDataFileWrapper.enWorkbookDataFileHelperItemType itemType,
            string value)
        {
            if (m_DataFileWrapper == null)
                throw new InvalidOperationException("m_DataFileWrapper should not be null");
            if (m_wshSetup == null)
                throw new InvalidOperationException("m_wshSetup should not be null");

            if (value == null)
            {
                m_wshSetup.Range[excelRangeName].Value = null;
            }
            else
            {
                var stringsInFile = m_DataFileWrapper.GetStrings(itemType);
                int index = stringsInFile.IndexOf(value);
                if (index < 0)
                {
                    m_DataFileWrapper.AddItemIfNotExists(value, itemType);
                    index = stringsInFile.Count;
                }
                m_wshSetup.Range[excelRangeName].Value = index;
            }
        }

        private DateTime? GetDate(string excelRangeName)
        {
            if (m_wshSetup == null)
                throw new InvalidOperationException("m_wshSetup should not be null");

            string dateRaw = m_wshSetup.Range[excelRangeName].Value.ToString();
            DateTime date;
            if (!string.IsNullOrEmpty(dateRaw)
                && DateTime.TryParseExact(dateRaw,
                                        "dd.MM.yyyy",
                                        CultureInfo.CurrentCulture,
                                        DateTimeStyles.None,
                                        out date))
            {
                return date;
            }

            return null;
        }

        private void SetDate(string excelRangeName, DateTime? value)
        {
            if (m_wshSetup == null)
                throw new InvalidOperationException("m_wshSetup should not be null");

            m_wshSetup.Range[excelRangeName].Value = value.HasValue
                        ? value.Value.ToString("dd.MM.yyyy")
                        : "";
        }
    }
}
