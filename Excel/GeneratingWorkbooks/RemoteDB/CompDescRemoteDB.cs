using DBManager.Global;
using System;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class CompDescRemoteDB : CompDescBase
    {
        #region RemoteStartDate
        private DateTime m_RemoteStartDate;
        /// <summary>
        /// Start date is read frome remote DB
        /// </summary>
        public DateTime RemoteStartDate
        {
            get { return m_RemoteStartDate; }
            set
            {
                if (m_RemoteStartDate != value)
                {
                    m_RemoteStartDate = value;
                }
            }
        }
        #endregion

        #region RemoteEndDate
        private DateTime? m_RemoteEndDate;
        /// <summary>
        /// End date is read frome remote DB
        /// </summary>
        public DateTime? RemoteEndDate
        {
            get { return m_RemoteEndDate; }
            set
            {
                if (m_RemoteEndDate != value)
                {
                    m_RemoteEndDate = value;
                }
            }
        }
        #endregion

        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<CompDescRemoteDB>(m => m.ID);
        private int m_ID = -1;

        public int ID
        {
            get { return m_ID; }
            set
            {
                if (m_ID != value)
                {
                    m_ID = value;
                    OnPropertyChanged(IDPropertyName);
                }
            }
        }
        #endregion

        public CompDescRemoteDB()
        {
        }


        public void UpdateDatesFromRemoteOnes()
        {
            StartDate = RemoteStartDate;
            EndDate = RemoteEndDate;
        }
    }
}
