using DBManager.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public class WorkbookFlagsWrapper
    {
        #region Флаги
        const int ONE_DAY_COMPETITION_BIT = 1;
        const int SOME_DAYS_COMPETITION_BIT = 2;
        const int CMB_YYYY_SELECTED_INDEX_BIT = 3;
        /// <summary>
        /// 0 - м, 1 - ж
        /// </summary>
        const int SEX_BIT = 4;
        const int GROUP_YEARS_NEEDED_BIT = 5;
        const int SHOW_ROW_6_BIT = 8;
        /// <summary>
        /// Вводилось ли примерное количество участников (= открыли форму frmStartForm первый раз)
        /// </summary>
        const int WBK_IS_OPENED_FIRST_TIME_BIT = 9;
        /// <summary>
        /// 0 - команда, 1 - тренер
        /// </summary>
        const int TEAM_COACH_BIT = 11;

        const int GROUP_INDEX_1ST_BIT = 6;
        const int GROUP_INDEX_2ND_BIT = 7;
        const int GROUP_INDEX_3RD_BIT = 14;
        #endregion

        BitArray m_flags = new BitArray(16);

        public ushort Value
        {
            get
            {
                int res = 0;
                for (int i = 0; i < m_flags.Count; i++)
                {
                    res |= (m_flags[i] ? 1 : 0) << i;
                }

                return (ushort)res;
            }
            set
            {
                for (int i = 0; i < m_flags.Count; i++)
                {
                    m_flags[i] = (value >> i) == 1;
                }
            }
        }

        #region Задание флагов и получение их состояния

        public bool CompetitionHoldsOneDay
        {
            get { return m_flags[ONE_DAY_COMPETITION_BIT]; }
            private set { m_flags[ONE_DAY_COMPETITION_BIT] = value; }
        }

        public bool CompetitionHoldsSomeDays
        {
            get { return m_flags[SOME_DAYS_COMPETITION_BIT]; }
            private set { m_flags[SOME_DAYS_COMPETITION_BIT] = value; }
        }

        public bool Select1stRowInCmbYYYY
        {
            get { return !m_flags[CMB_YYYY_SELECTED_INDEX_BIT]; }
            private set { m_flags[CMB_YYYY_SELECTED_INDEX_BIT] = !value; }
        }

        public enSex Sex
        {
            get { return (enSex)m_flags[SEX_BIT].ToInt32(); }
            set { m_flags[SOME_DAYS_COMPETITION_BIT] = ((int)value).ToBool(); }
        }

        public bool GroupYearsAreNeeded
        {
            get { return m_flags[GROUP_YEARS_NEEDED_BIT]; }
            set { m_flags[GROUP_YEARS_NEEDED_BIT] = value; }
        }

        public bool ShowRow6
        {
            get { return m_flags[SHOW_ROW_6_BIT]; }
            set { m_flags[SHOW_ROW_6_BIT] = value; }
        }

        public bool WorkbookIsOpenedFirstTime
        {
            get { return !m_flags[WBK_IS_OPENED_FIRST_TIME_BIT]; }
            set { m_flags[WBK_IS_OPENED_FIRST_TIME_BIT] = !value; }
        }

        public enSecondColNameType SecondColNameType
        {
            get { return m_flags[TEAM_COACH_BIT] ? enSecondColNameType.Coach : enSecondColNameType.Team; }
            set { m_flags[TEAM_COACH_BIT] = value == enSecondColNameType.Coach; }
        }

        public int GroupIndexValue
        {
            get
            {
                return m_flags[GROUP_INDEX_1ST_BIT].ToInt32()
                        | m_flags[GROUP_INDEX_2ND_BIT].ToInt32() << 1
                        | m_flags[GROUP_INDEX_3RD_BIT].ToInt32() << 2;
            }
            set
            {
                m_flags[GROUP_INDEX_1ST_BIT] = (value & 1).ToBool();
                m_flags[GROUP_INDEX_2ND_BIT] = (value & (1 << 2)).ToBool();
                m_flags[GROUP_INDEX_3RD_BIT] = (value & (1 << 2)).ToBool();
            }
        }

        #endregion

        public void SetFlagsByCompDates(DateTime startDate, DateTime? endDate)
        {
            CompetitionHoldsOneDay = endDate == null || startDate == endDate;
            CompetitionHoldsSomeDays = !CompetitionHoldsOneDay;

            Select1stRowInCmbYYYY = DateTime.Today.Year == startDate.Year;
        }
    }
}
