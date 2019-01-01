using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using DBManager.OnlineResults.Tasks;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.OnlineResults
{
	public class CQueueItem
	{
        #region PCWbkFullPath
        private string m_PCWbkFullPath = null;

        public string PCWbkFullPath
        {
            get { return m_PCWbkFullPath; }
            set
            {
                if (m_PCWbkFullPath != value)
                {
                    m_PCWbkFullPath = value;
                }
            }
        }
        #endregion

        #region CompId
        private long m_CompId = -1;

        public long CompId
        {
            get { return m_CompId; }
            set
            {
                if (m_CompId != value)
                {
                    m_CompId = value;
                }
            }
        }
        #endregion

        #region GroupId
        private long m_GroupId = -1;

        public long GroupId
        {
            get { return m_GroupId; }
            set
            {
                if (m_GroupId != value)
                {
                    m_GroupId = value;
                }
            }
        }
        #endregion

        #region CompSettings
        private CCompSettings m_CompSettings = null;

        public CCompSettings CompSettings
        {
            get { return m_CompSettings; }
            set
            {
                if (m_CompSettings != value)
                {
                    m_CompSettings = value;
                }
            }
        }
        #endregion

        #region Round
        private enRounds m_Round = enRounds.None;

        public enRounds Round
        {
            get { return m_Round; }
            set
            {
                if (m_Round != value)
                {
                    m_Round = value;
                }
            }
        }
        #endregion

        public OnlineResultsTaskBase Task { get; set; }
    }
}
