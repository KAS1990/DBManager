using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using System;
using System.ComponentModel;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public abstract class CompDescBase : ICompDesc, INotifyPropertyChanged
    {
        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.Name);
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
        public static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.StartDate);
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
        #endregion

        #region EndDate
        public static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.EndDate);
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
        #endregion

        #region MainJudge
        private static readonly string MainJudgePropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.MainJudge);
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
        private static readonly string MainSecretaryPropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.MainSecretary);
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
        private static readonly string ShowRow6PropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.ShowRow6);
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
        private static readonly string Row6PropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.Row6);
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
        private static readonly string SecondColNameTypePropertyName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.SecondColNameType);
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
        private static readonly string DestCompFolderName = GlobalDefines.GetPropertyName<CompDescBase>(m => m.DestCompFolder);
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

        public virtual void CopyCompSpecificFields(ICompDesc src)
        {
            MainJudge = src.MainJudge;
            MainSecretary = src.MainSecretary;
            ShowRow6 = src.ShowRow6;
            Row6 = src.Row6;
            SecondColNameType = src.SecondColNameType;
            DestCompFolder = src.DestCompFolder;
        }

        public virtual string GetDefaultDestCompFolderName()
        {
            if (EndDate.HasValue && StartDate != EndDate.Value)
            {
                if (StartDate.Month == EndDate.Value.Month)
                {
                    return string.Format("{0:D2}-{1:D2}.{2:D2}.{3:D2}",
                                        StartDate.Day,
                                        EndDate.Value.Day,
                                        StartDate.Month,
                                        StartDate.Year);
                }
                else
                {
                    return string.Format("{0:D2}.{1:D2}-{2:D2}.{3:D2}.{4:D2}",
                                        StartDate.Day,
                                        StartDate.Month,
                                        EndDate.Value.Day,
                                        EndDate.Value.Month,
                                        StartDate.Year);
                }
            }
            else
            {
                return string.Format("{0:D2}.{1:D2}.{2:D2}",
                                    StartDate.Day,
                                    StartDate.Month,
                                    StartDate.Year);
            }
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
