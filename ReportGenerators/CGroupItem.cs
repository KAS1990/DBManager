using DBManager.Global;
using DBManager.ReportGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DBManager.Excel.Exporting.Tabs
{
    public class CGroupItem : INotifyPropertyChanged
    {
        #region id
        private readonly long m_id = 0;

        public long id
        {
            get { return m_id; }
        }
        #endregion


        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.Name);

        private string m_Name = null;

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


        #region NameForHeader
        private static readonly string NameForHeaderPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.NameForHeader);
        /// <summary>
        /// Если есть выделенный раунд, то свойство добавляет * к названию группы
        /// </summary>
        public string NameForHeader
        {
            get
            {
                return Rounds == null || Rounds.FirstOrDefault(arg => arg.IsSelected) == null ? Name : Name + "*";
            }
        }
        #endregion


        #region IsSelected
        public static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.IsSelected);

        private bool m_IsSelected = false;

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


        #region LeadSheetIndex
        private static readonly string LeadSheetIndexPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.LeadSheetIndex);

        private int m_LeadSheetIndex = -1;
        /// <summary>
        /// Номер листа, соотвествующей группе в протоколе трудности 
        /// </summary>
        public int LeadSheetIndex
        {
            get { return m_LeadSheetIndex; }
            set
            {
                if (m_LeadSheetIndex != value)
                {
                    m_LeadSheetIndex = value;

                    if (m_LeadSheetsCount > 0 && m_LeadSheetIndex == m_LeadSheetsCount - 1)
                        ThreadManager.Instance.InvokeUI(new Action(() => { LeadSheetIndex = -1; }));
                    else
                        OnPropertyChanged(LeadSheetIndexPropertyName);
                }
            }
        }
        #endregion



        #region LeadSheetsCount
        private static readonly string LeadSheetsCountPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.LeadSheetsCount);

        private int m_LeadSheetsCount = 0;

        public int LeadSheetsCount
        {
            get { return m_LeadSheetsCount; }
            set
            {
                if (m_LeadSheetsCount != value)
                {
                    m_LeadSheetsCount = value;
                    OnPropertyChanged(LeadSheetsCountPropertyName);
                }
            }
        }
        #endregion



        #region SheetName
        private static readonly string SheetNamePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.SheetName);

        private string m_SheetName = "";

        public string SheetName
        {
            get { return m_SheetName; }
            set
            {
                if (m_SheetName != value)
                {
                    m_SheetName = value;
                    OnPropertyChanged(SheetNamePropertyName);
                }
            }
        }
        #endregion


        #region Rounds
        private readonly ObservableCollection<CRoundItem> m_Rounds = new ObservableCollection<CRoundItem>();

        public ObservableCollection<CRoundItem> Rounds
        {
            get { return m_Rounds; }
        }
        #endregion


        #region StartYearIndToExport
        private static readonly string StartYearIndToExportPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.StartYearIndToExport);

        private int m_StartYearIndToExport = -1;

        public int StartYearIndToExport
        {
            get { return m_StartYearIndToExport; }
            set
            {
                if (m_StartYearIndToExport != value)
                {
                    m_StartYearIndToExport = value;
                    OnPropertyChanged(StartYearIndToExportPropertyName);
                }
            }
        }
        #endregion


        #region EndYearIndToExport
        private static readonly string EndYearIndToExportPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.EndYearIndToExport);

        private int m_EndYearIndToExport = -1;

        public int EndYearIndToExport
        {
            get { return m_EndYearIndToExport; }
            set
            {
                if (m_EndYearIndToExport != value)
                {
                    m_EndYearIndToExport = value;
                    OnPropertyChanged(EndYearIndToExportPropertyName);
                }
            }
        }
        #endregion


        #region YearsOfBirth
        private static readonly string YearsOfBirthPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.YearsOfBirth);

        private readonly List<int> m_YearsOfBirth = new List<int>();

        public List<int> YearsOfBirth
        {
            get { return m_YearsOfBirth; }
        }
        #endregion


        #region CanSelectYears
        private static readonly string CanSelectYearsPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.CanSelectYears);

        private bool m_CanSelectYears = true;

        public bool CanSelectYears
        {
            get { return m_CanSelectYears; }
            set
            {
                if (m_CanSelectYears != value)
                {
                    m_CanSelectYears = value;
                    OnPropertyChanged(CanSelectYearsPropertyName);
                }
            }
        }
        #endregion


        #region StartDate
        private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.StartDate);

        private DateTime? m_StartDate = DateTime.Today;

        public DateTime? StartDate
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
        private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.EndDate);

        private DateTime? m_EndDate = DateTime.Today;

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


        #region Sex
        private static readonly string SexPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.Sex);

        private enSex m_Sex = enSex.None;

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



        public CGroupItem()
        {
        }


        public CGroupItem(long id)
        {
            m_id = id;

            m_Rounds.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                    e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    foreach (CRoundItem item in e.NewItems)
                    {
                        item.PropertyChanged -= Round_PropertyChanged;
                        item.PropertyChanged += Round_PropertyChanged;
                    }
                }
            };
        }

        private void Round_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CRoundItem.IsSelectedPropertyName)
            {   // Какой-то раунд выделили или сняли с него выделение =>
                // нужно заменить заголовок вкладки
                OnPropertyChanged(NameForHeaderPropertyName);
            }
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public virtual void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
