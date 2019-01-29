using DBManager.Excel.GeneratingWorkbooks.Helpers;
using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public abstract class GroupItemBase : IGroupItem, INotifyPropertyChanged
    {
        #region CompDesc
        /// <summary>
        /// 
        /// </summary>
        public ICompDesc CompDesc { get; } = null;
        #endregion

        #region Name
        protected static readonly string NamePropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.Name);
        protected string m_Name = null;
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NamePropertyName);
                    FillWorkbookName();
                }
            }
        }
        #endregion

        #region Sex
        private static readonly string SexPropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.Sex);
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
        private static readonly string StartYearPropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.StartYear);
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
                    FillWorkbookName();
                }
            }
        }

        public string StartYearInString => m_StartYear.CreateYearInString();
        #endregion

        #region EndYear
        private static readonly string EndYearPropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.EndYear);
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
                    FillWorkbookName();
                }
            }
        }

        public string EndYearInString => m_EndYear.CreateYearInString();
        #endregion

        #region StartDate
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.StartDate);
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
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.EndDate);
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
        private static readonly string WorkbookNamePropertyName = GlobalDefines.GetPropertyName<GroupItemBase>(m => m.WorkbookName);
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

        public GroupItemBase(ICompDesc compDesc)
        {
            CompDesc = compDesc;
        }

        public GroupItemBase(GroupItemBase rhs)
        {
            CompDesc = rhs.CompDesc;
            Name = rhs.Name;
            WorkbookName = rhs.WorkbookName;
            Sex = rhs.Sex;
            StartYear = rhs.StartYear;
            EndYear = rhs.EndYear;
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
                if (EndYear == (int)enEndYearSpecVals.AndElder || EndYear == (int)enEndYearSpecVals.AndYounger)
                    result += $" {StartYearInString} {EndYearInString}{GlobalDefines.MAIN_WBK_EXTENSION}";
                else
                    result += $" {StartYearInString}-{EndYearInString}{GlobalDefines.MAIN_WBK_EXTENSION}";
            }
            WorkbookName = result;
        }
    }
}
