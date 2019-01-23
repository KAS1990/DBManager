using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class CompDesc : INotifyPropertyChanged
    {
        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.Name);
        private string m_Name = null;
        /// <summary>
        /// Название сорев. Выбирается из БД
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NamePropertyName);
                }
            }
        }
        #endregion
        
        #region StartDate
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.StartDate);
        private DateTime m_StartDate;

        public DateTime StartDate
        {
            get { return m_StartDate; }
            set
            {
                if (m_StartDate != value)
                {
                    m_StartDate = value;
                    OnPropertyChanged(StartDatePropertyName);
                }
            }
        }

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

        #region EndDate
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.EndDate);
        private DateTime? m_EndDate = null;

        public DateTime? EndDate
        {
            get { return m_EndDate; }
            set
            {
                if (m_EndDate != value)
                {
                    m_EndDate = value;
                    OnPropertyChanged(EndDatePropertyName);
                }
            }
        }

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

        #region MainJudge
        private static readonly string MainJudgePropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.MainJudge);
        private string m_MainJudge = null;

        public string MainJudge
        {
            get { return m_MainJudge; }
            set
            {
                if (m_MainJudge != value)
                {
                    m_MainJudge = value;
                    OnPropertyChanged(MainJudgePropertyName);
                }
            }
        }
        #endregion

        #region MainSecretary
        private static readonly string MainSecretaryPropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.MainSecretary);
        private string m_MainSecretary = null;

        public string MainSecretary
        {
            get { return m_MainSecretary; }
            set
            {
                if (m_MainSecretary != value)
                {
                    m_MainSecretary = value;
                    OnPropertyChanged(MainSecretaryPropertyName);
                }
            }
        }
        #endregion

        #region ShowRow6
        private static readonly string ShowRow6PropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.ShowRow6);
        private bool m_ShowRow6 = false;

        public bool ShowRow6
        {
            get { return m_ShowRow6; }
            set
            {
                if (m_ShowRow6 != value)
                {
                    m_ShowRow6 = value;
                    OnPropertyChanged(ShowRow6PropertyName);
                }
            }
        }
        #endregion

        #region Row6
        private static readonly string Row6PropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.Row6);
        private string m_Row6 = null;

        public string Row6
        {
            get { return m_Row6; }
            set
            {
                if (m_Row6 != value)
                {
                    m_Row6 = value;
                    OnPropertyChanged(Row6PropertyName);
                }
            }
        }
        #endregion

        #region SecondColNameType
        private static readonly string SecondColNameTypePropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.SecondColNameType);
        private enSecondColNameType m_SecondColNameType = enSecondColNameType.None;

        public enSecondColNameType SecondColNameType
        {
            get { return m_SecondColNameType; }
            set
            {
                if (m_SecondColNameType != value)
                {
                    m_SecondColNameType = value;
                    OnPropertyChanged(SecondColNameTypePropertyName);
                }
            }
        }
        #endregion

        #region DestCompFolder
        private static readonly string DestCompFolderName = GlobalDefines.GetPropertyName<CompDesc>(m => m.DestCompFolder);
        private string m_DestCompFolder = null;
        /// <summary>
        /// 
        /// </summary>
        public string DestCompFolder
        {
            get { return m_DestCompFolder; }
            set
            {
                if (m_DestCompFolder != value)
                {
                    m_DestCompFolder = value;
                    OnPropertyChanged(DestCompFolderName);
                }
            }
        }
        #endregion

        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<CompDesc>(m => m.ID);
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

        public CompDesc()
        {
        }


        public void UpdateDatesFromRemoteOnes()
        {
            StartDate = RemoteStartDate;
            EndDate = RemoteEndDate;
        }

        public void CopyNonRemoteFields(CompDesc src)
        {
            MainJudge = src.MainJudge;
            MainSecretary = src.MainSecretary;
            ShowRow6 = src.ShowRow6;
            Row6 = src.Row6;
            SecondColNameType = src.SecondColNameType;
            DestCompFolder = src.DestCompFolder;
        }

        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
