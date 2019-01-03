using DBManager.Global;
using DBManager.OnlineDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в таблице групп
    /// </summary>
    public class GroupItem : INotifyPropertyChanged
    {
        #region IsSelected
        private static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.IsSelected);
        private bool m_IsSelected = false;
        /// <summary>
        ///
        /// </summary>
        public bool IsSelected
        {
            get { return m_IsSelected; }
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    OnPropertyChanged(IsSelectedPropertyName);
                }
            }
        }
        #endregion

        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.ID);
        private int m_ID = 0;
        /// <summary>
        /// ID in remote DB
        /// </summary>
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

        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.Name);
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

        #region Sex
        private static readonly string SexPropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.Sex);
        private enOnlineSex m_Sex = enOnlineSex.None;
        /// <summary>
        /// 
        /// </summary>
        public enOnlineSex Sex
        {
            get { return m_Sex; }
            set
            {
                if (m_Sex != value)
                {
                    m_Sex = value;
                    OnPropertyChanged(SexPropertyName);
                }
            }
        }
        #endregion

        #region StartYear
        private static readonly string StartYearPropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.StartYear);
        private int m_StartYear = 0;
        /// <summary>
        /// 
        /// </summary>
        public int StartYear
        {
            get { return m_StartYear; }
            set
            {
                if (m_StartYear != value)
                {
                    m_StartYear = value;
                    OnPropertyChanged(StartYearPropertyName);
                }
            }
        }
        #endregion

        #region EndYear
        private static readonly string EndYearPropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.EndYear);
        private int? m_EndYear = null;
        /// <summary>
        /// 
        /// </summary>
        public int? EndYear
        {
            get { return m_EndYear; }
            set
            {
                if (m_EndYear != value)
                {
                    m_EndYear = value;
                    OnPropertyChanged(EndYearPropertyName);
                }
            }
        }
        #endregion

        #region StartDate
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.StartDate);
        private DateTime m_StartDate;
        /// <summary>
        /// 
        /// </summary>
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
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<GroupItem>(m => m.EndDate);
        private DateTime? m_EndDate = null;
        /// <summary>
        /// 
        /// </summary>
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

        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public virtual void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        public GroupItem()
        {

        }

        public GroupItem(GroupItem rhs)
        {
            Name = rhs.Name;
            Sex = rhs.Sex;
            StartYear = rhs.StartYear;
            EndYear = rhs.EndYear;
            ID = rhs.ID;
            StartDate = rhs.StartDate;
            EndDate = rhs.EndDate;
        }
    }
}
