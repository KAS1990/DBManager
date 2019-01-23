using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using DBManager.OnlineDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в таблице групп
    /// </summary>
    public class GroupItemRemoteDB : IGroupItem, INotifyPropertyChanged
    {
        #region IsSelected
        private static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.IsSelected);
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
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.ID);
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
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.Name);
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
        private static readonly string SexPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.Sex);
        private enSex m_Sex = enSex.None;
        /// <summary>
        /// 
        /// </summary>
        public enSex Sex
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
        private static readonly string StartYearPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.StartYear);
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

        public string StartYearInString => CreateYearInString(m_StartYear);
        #endregion

        #region EndYear
        private static readonly string EndYearPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.EndYear);
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

        public string EndYearInString => CreateYearInString(m_StartYear);
        #endregion

        #region StartDate
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.StartDate);
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
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.EndDate);
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

        #region WorkbookName
        private static readonly string WorkbookNamePropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.WorkbookName);
        private string m_WorkbookName = null;
        /// <summary>
        /// Название книги, в которой будут храниться результаты участников группы
        /// </summary>
        public string WorkbookName
        {
            get { return m_WorkbookName; }
            set
            {
                if (m_WorkbookName != value)
                {
                    m_WorkbookName = Path.GetFileNameWithoutExtension(value) + GlobalDefines.MAIN_WBK_EXTENSION;
                    OnPropertyChanged(WorkbookNamePropertyName);
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

        public GroupItemRemoteDB()
        {

        }

        public GroupItemRemoteDB(GroupItemRemoteDB rhs)
        {
            Name = rhs.Name;
            WorkbookName = rhs.WorkbookName;
            Sex = rhs.Sex;
            StartYear = rhs.StartYear;
            EndYear = rhs.EndYear;
            ID = rhs.ID;
            StartDate = rhs.StartDate;
            EndDate = rhs.EndDate;
        }

        public void FillWorkbookName()
        {
            string result = Name;
            if (!(string.IsNullOrWhiteSpace(Name)
                    || string.IsNullOrWhiteSpace(StartYearInString)
                    || string.IsNullOrWhiteSpace(EndYearInString)))
            {
                result += $" {StartYearInString}-{EndYearInString}{GlobalDefines.MAIN_WBK_EXTENSION}";
            }
            WorkbookName = result;
        }

        public static string CreateYearInString(int? year)
        {
            if (year == null)
                return null;

            switch (year.Value)
            {
                case (int)enEndYearSpecVals.AndElder:
                    return Properties.Resources.resAndElder;

                case (int)enEndYearSpecVals.AndYounger:
                    return Properties.Resources.resAndYounger;

                default:
                    return year.Value.ToString();
            }
        }
    }
}
