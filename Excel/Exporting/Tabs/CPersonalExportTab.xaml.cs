using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBManager.Global;
using DBManager.SettingsWriter;
using System.IO;
using System.Collections.ObjectModel;
using DBManager.Scanning.XMLDataClasses;


namespace DBManager.Excel.Exporting.Tabs
{
    /// <summary>
    /// Личный зачёт
    /// </summary>
    public partial class CPersonalExportTab : CExportingTabBase
    {
        #region PlaceAggregationMethod
        private static readonly string PlaceAggregationMethodPropertyName = GlobalDefines.GetPropertyName<CPersonalExportTab>(m => m.PlaceAggregationMethod);

        private enPersRepPlacesAggregationMethod m_PlaceAggregationMethod = enPersRepPlacesAggregationMethod.Sum;

        public enPersRepPlacesAggregationMethod PlaceAggregationMethod
        {
            get { return m_PlaceAggregationMethod; }
            set
            {
                if (m_PlaceAggregationMethod != value)
                {
                    m_PlaceAggregationMethod = value;
                    OnPropertyChanged(PlaceAggregationMethodPropertyName);
                }
            }
        }
        #endregion
                
        #region WinnerDetection
        private static readonly string WinnerDetectionPropertyName = GlobalDefines.GetPropertyName<CPersonalExportTab>(m => m.WinnerDetection);

        private enPersRepWinnerDetection m_WinnerDetection = enPersRepWinnerDetection.LeadPriority;
        /// <summary>
        /// Метод определения победителя при равенстве мест
        /// </summary>
        public enPersRepWinnerDetection WinnerDetection
        {
            get { return m_WinnerDetection; }
            set
            {
                if (m_WinnerDetection != value)
                {
                    m_WinnerDetection = value;
                    OnPropertyChanged(WinnerDetectionPropertyName);
                }
            }
        }
        #endregion

        #region PriorityCompetitionKind
        private static readonly string PriorityCompetitionKindPropertyName = GlobalDefines.GetPropertyName<CPersonalExportTab>(m => m.PriorityCompetitionKind);

        private enPriorityCompetitionKind m_PriorityCompetitionKind = enPriorityCompetitionKind.Lead;
        /// <summary>
        /// Метод определения победителя при равенстве мест
        /// </summary>
        public enPriorityCompetitionKind PriorityCompetitionKind
        {
            get { return m_PriorityCompetitionKind; }
            set
            {
                if (m_PriorityCompetitionKind != value)
                {
                    m_PriorityCompetitionKind = value;
                    OnPropertyChanged(PriorityCompetitionKindPropertyName);
                }
            }
        }
        #endregion
        
        #region GroupsForReport
        private ObservableCollection<CGroupItem> m_GroupsForReport = new ObservableCollection<CGroupItem>();

        public ObservableCollection<CGroupItem> GroupsForReport
        {
            get { return m_GroupsForReport; }
        }
        #endregion
                        
        public List<CGroupItem> SelectedGroups
        {
            get
            {
                return new List<CGroupItem>(GroupsForReport.Where(arg => arg.IsSelected));
            }
        }


        descriptions CompDesc
        {
            get { return m_ParentWnd == null ? null : m_ParentWnd.m_CompDesc; }
        }


        public CPersonalExportTab()
        {
            InitializeComponent();
        }


        public CPersonalExportTab(CExportToExcelWnd ParentWnd, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups) :
            base(ParentWnd, CompGroups)
        {
            InitializeComponent();

            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

                if (CompDesc != null)
                {
                    // Добавляем группы в колекцию
                    foreach (KeyValuePair<long, CKeyValuePairEx<long, CCompSettings>> Group in CompGroups)
                    {
                        CGroupItem GroupItem = new CGroupItem(Group.Key)
                        {
                            Name = Group.Value.Value.AgeGroup.FullGroupName,
                            SheetName = Properties.Resources.resPersonalReport + " " + Group.Value.Value.AgeGroup.FullGroupName,
                            Sex = Group.Value.Value.AgeGroup.SexInEnum
                        };
                        GroupItem.SheetName = GroupItem.SheetName.Left(m_ParentWnd.MaxSheetNameLen);
                        if (Group.Value.Value.AgeGroup.GroupType == CAgeGroup.enGroupType.Children)
                        {	// Есть года рождения
                            if (Group.Value.Value.AgeGroup.StartYear == GlobalDefines.DEFAULT_XML_INT_VAL ||
                                Group.Value.Value.AgeGroup.wEndYear == GlobalDefines.DEFAULT_XML_INT_VAL)
                            {	// Но пока их почему-то не ввели
                                GroupItem.CanSelectYears = false;
                            }
                            else
                            {
                                int EndYear = 0, StartYear = 0;
                                switch (Group.Value.Value.AgeGroup.wEndYear)
                                {
                                    case (int)DBManager.Scanning.XMLDataClasses.CAgeGroup.enEndYearSpecVals.AndElder:
                                        StartYear = GlobalDefines.MIN_GROUP_YEAR;
                                        EndYear = Group.Value.Value.AgeGroup.StartYear;
                                        break;

                                    case (int)DBManager.Scanning.XMLDataClasses.CAgeGroup.enEndYearSpecVals.AndYounger:
                                        StartYear = Group.Value.Value.AgeGroup.StartYear;
                                        EndYear = GlobalDefines.MAX_GROUP_YEAR;
                                        break;

                                    default:
                                        StartYear = Group.Value.Value.AgeGroup.StartYear;
                                        EndYear = Group.Value.Value.AgeGroup.wEndYear;
                                        break;
                                }
                                for (int i = StartYear; i <= EndYear; i++)
                                    GroupItem.YearsOfBirth.Add(i);
                                GroupItem.StartYearIndToExport = 0;
                                GroupItem.EndYearIndToExport = GroupItem.YearsOfBirth.Count - 1;
                            }
                        }
                        else
                        {	// Годов рождения нет
                            for (int i = GlobalDefines.MIN_GROUP_YEAR; i <= GlobalDefines.MAX_GROUP_YEAR; i++)
                                GroupItem.YearsOfBirth.Add(i);
                            GroupItem.StartYearIndToExport = 0;
                            GroupItem.EndYearIndToExport = GroupItem.YearsOfBirth.Count - 1;
                        }
                        
                        GroupsForReport.Add(GroupItem);
                    }
                }

                CCompSpecificSets CompSettings;
                if (settings.dictCompSettings.TryGetValue(CompDesc.id_desc, out CompSettings))
                {
                    PlaceAggregationMethod = CompSettings.PersRepPlaceAggregationMethod;
                    WinnerDetection = CompSettings.PersRepWinnerDetection;
                    PriorityCompetitionKind = CompSettings.PriorityCompetitionKind;
                }
                else
                {
                    PlaceAggregationMethod = settings.DefaultCompSettings.PersRepPlaceAggregationMethod;
                    WinnerDetection = settings.DefaultCompSettings.PersRepWinnerDetection;
                    PriorityCompetitionKind = settings.DefaultCompSettings.PriorityCompetitionKind;
                }
            }

            GlobalDefines.TuneComboboxWidth2(cmbPlaceAggregationMethod);
            GlobalDefines.TuneComboboxWidth2(cmbWinnerDetection);

            cmbPlaceAggregationMethod.Width = cmbWinnerDetection.Width = Math.Max(cmbPlaceAggregationMethod.Width, cmbWinnerDetection.Width);
            foreach (ComboBoxItem cmbi in cmbPlaceAggregationMethod.Items)
                cmbi.Width = Math.Max(cmbi.Width, (cmbWinnerDetection.Items[0] as ComboBoxItem).Width);
            foreach (ComboBoxItem cmbi in cmbWinnerDetection.Items)
                cmbi.Width = (cmbPlaceAggregationMethod.Items[0] as ComboBoxItem).Width;
        }


        public override bool CheckSettings()
        {
            if (CreateReport)
            {
                if (ExportToAnotherWbk)
                {
                    if (string.IsNullOrWhiteSpace(XlsPath) ||
                        !Directory.Exists(System.IO.Path.GetDirectoryName(XlsPath)) ||
                        System.IO.Path.GetExtension(XlsPath) != GlobalDefines.XLSX_EXTENSION)
                    {
                        MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resInvalidXlsxPath,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return false;
                    }

                    if (File.Exists(XlsPath))
                    {	// Перезаписывать файлы нельзя
                        MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resXlsxFileAlreadyExists,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return false;
                    }
                }

                if (SelectedGroups.Count == 0)
                {	// Ничего не выделено
                    MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resNoGroupsToExport,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    return false;
                }

                if (SelectedGroups.Where(arg => arg.StartYearIndToExport > arg.EndYearIndToExport && arg.CanSelectYears).Count() > 0)
                {	// Есть ошибки в задании годов
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resInvalidYearsToExport,
                                    (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }


        public override void BeforeExporting()
        {
            if (CreateReport)
            {
                lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                {
                    AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

                    CCompSpecificSets CompSettings = null;
                    if (!settings.dictCompSettings.TryGetValue(CompDesc.id_desc, out CompSettings))
                    {
                        CompSettings = new CCompSpecificSets()
                        {
                            CompId = CompDesc.id_desc,
                        };
                    }
                                        
                    settings.dictCompSettings.TryAddValue(CompSettings.CompId, CompSettings);

                    CompSettings.PersRepPlaceAggregationMethod = PlaceAggregationMethod;
                    CompSettings.PersRepWinnerDetection = WinnerDetection;
                    CompSettings.PriorityCompetitionKind = PriorityCompetitionKind;
                }

                DBManagerApp.m_AppSettings.Write();
            }
        }
        

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            string SelectedPath;

            if (BaseBrowse_Click(out SelectedPath, Properties.Resources.resXlsxFilesFilter, false))
                XlsPath = SelectedPath;
        }
    }
}
