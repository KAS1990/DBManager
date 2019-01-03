using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в выпадающем списке соревнований
    /// </summary>
    public class CompItem : INotifyPropertyChanged
    {
        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.Name);
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
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.StartDate);
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
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.EndDate);
        private DateTime? m_EndDate;

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


        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.ID);
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


        #region Groups
        public ObservableCollection<GroupItem> Groups { get; private set; } = new ObservableCollection<GroupItem>();
        #endregion

        public CompItem()
        {
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
