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
using System.Collections.ObjectModel;
using MSExcel = Microsoft.Office.Interop.Excel;
using System.IO;
using DBManager.Scanning.XMLDataClasses;
using DBManager.SettingsWriter;

namespace DBManager.Excel.Exporting.Tabs
{
    /// <summary>
    /// Interaction logic for CLeadReportInfoTab.xaml
    /// </summary>
    public partial class CLeadReportInfoTab : CExportingTabBase
    {
        #region FirstMemberRow
        private static readonly string FirstMemberRowPropertyName = GlobalDefines.GetPropertyName<CLeadReportInfoTab>(m => m.FirstMemberRow);

        private int m_FirstMemberRow = 8;
        /// <summary>
        /// Номер строки, с которой начинается список участников
        /// Начинается с 1!!!
        /// </summary>
        public int FirstMemberRow
        {
            get { return m_FirstMemberRow; }
            set
            {
                if (m_FirstMemberRow != value)
                {
                    m_FirstMemberRow = value;
                    OnPropertyChanged(FirstMemberRowPropertyName);
                }
            }
        }
        #endregion


        #region PlaceColumnIndex
        private static readonly string PlaceColumnIndexPropertyName = GlobalDefines.GetPropertyName<CLeadReportInfoTab>(m => m.PlaceColumnIndex);

        private int m_PlaceColumnIndex = 1;
        /// <summary>
        /// Номер столбца, в котором находится место участника
        /// Начинается с 1!!!
        /// </summary>
        public int PlaceColumnIndex
        {
            get { return m_PlaceColumnIndex; }
            set
            {
                if (m_PlaceColumnIndex != value)
                {
                    m_PlaceColumnIndex = value;
                    OnPropertyChanged(PlaceColumnIndexPropertyName);
                }
            }
        }
        #endregion


        #region PersonalDataColumnIndex
        private static readonly string PersonalDataColumnIndexPropertyName = GlobalDefines.GetPropertyName<CLeadReportInfoTab>(m => m.PersonalDataColumnIndex);

        private int m_PersonalDataColumnIndex = 2;
        /// <summary>
        /// Номер столбца, в котором содержится ФИ участника.
        /// Начинается с 1!!!
        /// </summary>
        public int PersonalDataColumnIndex
        {
            get { return m_PersonalDataColumnIndex; }
            set
            {
                if (m_PersonalDataColumnIndex != value)
                {
                    m_PersonalDataColumnIndex = value;
                    OnPropertyChanged(PersonalDataColumnIndexPropertyName);
                }
            }
        }
        #endregion


        #region YearOfBirthColumnIndex
        private static readonly string YearOfBirthColumnIndexPropertyName = GlobalDefines.GetPropertyName<CLeadReportInfoTab>(m => m.YearOfBirthColumnIndex);

        private int m_YearOfBirthColumnIndex = 4;
        /// <summary>
        /// Номер столбца, в котором содержится г.р. участника.
        /// Начинается с 1!!!
        /// </summary>
        public int YearOfBirthColumnIndex
        {
            get { return m_YearOfBirthColumnIndex; }
            set
            {
                if (m_YearOfBirthColumnIndex != value)
                {
                    m_YearOfBirthColumnIndex = value;
                    OnPropertyChanged(YearOfBirthColumnIndexPropertyName);
                }
            }
        }
        #endregion

        
        #region TeamColumnIndex
        private static readonly string TeamColumnIndexPropertyName = GlobalDefines.GetPropertyName<CLeadReportInfoTab>(m => m.TeamColumnIndex);

        private int m_TeamColumnIndex = 3;

        public int TeamColumnIndex
        {
            get { return m_TeamColumnIndex; }
            set
            {
                if (m_TeamColumnIndex != value)
                {
                    m_TeamColumnIndex = value;
                    OnPropertyChanged(TeamColumnIndexPropertyName);
                }
            }
        }
        #endregion
                
        
        #region Groups
        private ObservableCollection<CGroupItem> m_Groups = new ObservableCollection<CGroupItem>();

        public ObservableCollection<CGroupItem> Groups
        {
            get { return m_Groups; }
        }
        #endregion


        #region LeadSheets
        private ObservableCollection<string> m_LeadSheets = new ObservableCollection<string>();

        public ObservableCollection<string> LeadSheets
        {
            get { return m_LeadSheets; }
        }
        #endregion


        descriptions CompDesc
        {
            get { return m_ParentWnd == null ? null : m_ParentWnd.m_CompDesc; }
        }
        

        public CLeadReportInfoTab()
        {
            InitializeComponent();
        }

        public CLeadReportInfoTab(CExportToExcelWnd ParentWnd, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups) :
            base(ParentWnd, CompGroups)
        {
            InitializeComponent();

            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;
                                
                if (CompDesc != null)
                {
                    CCompSpecificSets CompSettings = null;
                    if (settings.dictCompSettings.TryGetValue(CompDesc.id_desc, out CompSettings))
                    {
                        if (CompSettings.FirstMemberRow != GlobalDefines.DEFAULT_XML_INT_VAL)
                            FirstMemberRow = CompSettings.FirstMemberRow;
                        else
                            FirstMemberRow = settings.DefaultCompSettings.FirstMemberRow;

                        if (CompSettings.PlaceColumnIndex != GlobalDefines.DEFAULT_XML_INT_VAL)
                            PlaceColumnIndex = CompSettings.PlaceColumnIndex;
                        else
                            PlaceColumnIndex = settings.DefaultCompSettings.PlaceColumnIndex;

                        if (CompSettings.PersonalDataColumnIndex != GlobalDefines.DEFAULT_XML_INT_VAL)
                            PersonalDataColumnIndex = CompSettings.PersonalDataColumnIndex;
                        else
                            PersonalDataColumnIndex = settings.DefaultCompSettings.PersonalDataColumnIndex;

                        if (CompSettings.YearOfBirthColumnIndex != GlobalDefines.DEFAULT_XML_INT_VAL)
                            YearOfBirthColumnIndex = CompSettings.YearOfBirthColumnIndex;
                        else
                            YearOfBirthColumnIndex = settings.DefaultCompSettings.YearOfBirthColumnIndex;

                        if (CompSettings.TeamColumnIndex != GlobalDefines.DEFAULT_XML_INT_VAL)
                            TeamColumnIndex = CompSettings.TeamColumnIndex;
                        else
                            TeamColumnIndex = settings.DefaultCompSettings.TeamColumnIndex;
                        
                        if (CompSettings.LeadReportXlsPath != GlobalDefines.DEFAULT_XML_STRING_VAL)
                            XlsPath = CompSettings.LeadReportXlsPath;
                        else
                            XlsPath = settings.DefaultCompSettings.LeadReportXlsPath;
                    }
                    else
                    {
                        FirstMemberRow = settings.DefaultCompSettings.FirstMemberRow;
                        PlaceColumnIndex = settings.DefaultCompSettings.PlaceColumnIndex;
                        PersonalDataColumnIndex = settings.DefaultCompSettings.PersonalDataColumnIndex;
                        YearOfBirthColumnIndex = settings.DefaultCompSettings.YearOfBirthColumnIndex;
                        TeamColumnIndex = settings.DefaultCompSettings.TeamColumnIndex;
                        XlsPath = settings.DefaultCompSettings.LeadReportXlsPath;
                    }
                    HasLeadReport = !string.IsNullOrWhiteSpace(XlsPath);
                    CheckPathSettings();

                    // Добавляем группы в колекцию
                    foreach (KeyValuePair<long, CKeyValuePairEx<long, CCompSettings>> Group in CompGroups)
                    {
                        CGroupItem GroupItem = new CGroupItem(Group.Key)
                        {
                            Name = Group.Value.Value.AgeGroup.FullGroupName,
                            Sex = Group.Value.Value.AgeGroup.SexInEnum
                        };
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
                                                                        
                        Groups.Add(GroupItem);
                    }

                    if (CompSettings != null && CompSettings.dictGroupsLeadSheetsInfos != null)
                    {
                        List<long> Keys = CompSettings.dictGroupsLeadSheetsInfos.Keys.ToList(); 
                        foreach (long Key in Keys)
                        {
                            if (Groups.FirstOrDefault(arg => arg.id == Key) == null)
                            {	// Такой группы больше нет
                                CompSettings.dictGroupsLeadSheetsInfos.Remove(Key);
                            }
                        }
                    }
                                        
                    OnLeadReportRefreshed();

                    // Присваивать LeadSheetIndex нужно только после OnLeadReportRefreshed
                    foreach (CGroupItem GroupItem in Groups)
                    {
                        if (CompSettings != null && CompSettings.dictGroupsLeadSheetsInfos != null)
                        {
                            CLeadSheetInfo LeadSheetInfo;
                            if (CompSettings.dictGroupsLeadSheetsInfos.TryGetValue(GroupItem.id, out LeadSheetInfo))
                            {
                                GroupItem.LeadSheetIndex = LeadSheetInfo.SheetIndex;
                                GroupItem.StartDate = LeadSheetInfo.StartDate;
                                GroupItem.EndDate = LeadSheetInfo.EndDate;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Изменили путь к книге с протоколами трудности
        /// </summary>
        private void OnLeadReportRefreshed()
        {
            LeadSheets.Clear();

            foreach (CGroupItem group in Groups)
            {
                group.LeadSheetIndex = -1;
                group.LeadSheetsCount = 0;
            }

            if (HasLeadReport && CheckPathSettings())
            {
                bool NewAppCreated;
                MSExcel.Application excelApp = GlobalDefines.StartExcel(out NewAppCreated);
                
                if (excelApp != null)
                {
                    MSExcel.Workbook wbk = null;
                    bool NewWbkCreated = false;

                    try
                    {
                        foreach (MSExcel.Workbook book in excelApp.Workbooks)
                        {
                            if (book.FullName == XlsPath)
                            {	// Книга уже открыта => используем её
                                wbk = book;
                                break;
                            }
                        }
                        if (wbk == null)
                        {
                            if (NewAppCreated)
                            {
                                excelApp.Visible = true;
                                excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                            }
                            wbk = excelApp.Workbooks.Open(XlsPath);
                            if (NewAppCreated)
                                excelApp.Visible = false;
                            
                            NewWbkCreated = true;
                        }

                        if (wbk != null)
                        {
                            foreach (MSExcel.Worksheet wsh in wbk.Worksheets)
                                LeadSheets.Add(wsh.Name);

                            LeadSheets.Add(Properties.Resources.resCmbiClear); // Последним пунктом будет "Очистить"
                        }

                        foreach (CGroupItem group in Groups)
                            group.LeadSheetsCount = LeadSheets.Count;

                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(m_ParentWnd,
                                        string.Format(Properties.Resources.resfmtErrorDurExcelOperation, ex.Message),
                                        Parent == null ? m_ParentWnd.Title : (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                    finally
                    {
                        if (wbk != null && NewWbkCreated)
                        {
                            wbk.Saved = true; // Чтобы не появлялось сообщение с вопросом на сохранение
                            wbk.Close();
                        }

                        if (NewAppCreated)
                            excelApp.Quit();
                    }
                }
                else
                {
                    MessageBox.Show(m_ParentWnd,
                                    string.Format(Properties.Resources.resfmtErrorDurExcelOperation, Properties.Resources.resCantCreateExcelApp),
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }


        bool CheckPathSettings()
        {
            if (HasLeadReport)
            {
                if (string.IsNullOrWhiteSpace(XlsPath) ||
                            !Directory.Exists(System.IO.Path.GetDirectoryName(XlsPath)) ||
                            (System.IO.Path.GetExtension(XlsPath) != GlobalDefines.XLSX_EXTENSION &&
                            System.IO.Path.GetExtension(XlsPath) != GlobalDefines.XLS_EXTENSION))
                {
                    HasLeadReport = false;
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resInvalidXlsxPath,
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }

                if (!File.Exists(XlsPath))
                {	// Файл должен быть
                    HasLeadReport = false;
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resLeadReportNotFound,
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }


        public override bool CheckSettings()
        {
            if (HasLeadReport && CheckPathSettings())
            {
                //if (Groups.FirstOrDefault(arg => arg.LeadSheetIndex < 0) != null)
                //{	// Все группы должны быть заполнены
                //    MessageBox.Show(m_ParentWnd,
                //                    Properties.Resources.resGroupWithoutLeadSheetIndex,
                //                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                //                    MessageBoxButton.OK,
                //                    MessageBoxImage.Error);
                //    return false;
                //}

                if ((from gr in Groups
                     where gr.LeadSheetIndex >= 0
                     group gr by gr.LeadSheetIndex into LeadSheets
                     where LeadSheets.Count() > 1
                     select LeadSheets.Count()).Count() > 0)
                {	// Для разных групп выбраны одинаковые листы
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resLeadSheetsDuplicated,
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }

                if ((from gr in Groups
                     where gr.LeadSheetIndex >= 0
                            && (gr.StartDate.HasValue && gr.EndDate.HasValue && gr.StartDate.Value > gr.EndDate.Value) ||
                                !gr.StartDate.HasValue ||
                                gr.EndDate.Value.Year < GlobalDefines.MIN_GROUP_YEAR
                     select gr).Count() > 0)
                {	// Есть группы с неверно заданными датами
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resInvalidDatesForLeadSheets,
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }

                if (FirstMemberRow < 1 ||
                    PlaceColumnIndex < 1 ||
                    PersonalDataColumnIndex < 1 ||
                    YearOfBirthColumnIndex < 1 ||
                    TeamColumnIndex < 1)
                {
                    MessageBox.Show(m_ParentWnd,
                                    Properties.Resources.resInvalidLeadReportInfoVals,
                                    Parent == null ? Properties.Resources.resError : (Parent as TabItem).Header.ToString(),
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }


        public override void BeforeExporting()
        {
            SaveSettings();
        }


        private void SaveSettings()
        {
            if (HasLeadReport)
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

                    CompSettings.FirstMemberRow = FirstMemberRow;
                    CompSettings.PlaceColumnIndex = PlaceColumnIndex;
                    CompSettings.PersonalDataColumnIndex = PersonalDataColumnIndex;
                    CompSettings.YearOfBirthColumnIndex = YearOfBirthColumnIndex;
                    CompSettings.TeamColumnIndex = TeamColumnIndex;
                    CompSettings.LeadReportXlsPath = XlsPath;
                    CompSettings.dictGroupsLeadSheetsInfos = new SerializableDictionary<long, CLeadSheetInfo>();

                    foreach (CGroupItem Group in Groups)
                    {
                        CLeadSheetInfo LeadSheetInfo = new CLeadSheetInfo()
                        {
                            SheetIndex = Group.LeadSheetIndex,
                            StartDate = new CCompDate(),
                            EndDate = new CCompDate()
                        };
                        LeadSheetInfo.StartDate.Day = Group.StartDate.Value.Day;
                        LeadSheetInfo.StartDate.Month = Group.StartDate.Value.Month - 1;
                        LeadSheetInfo.StartDate.Year = Group.StartDate.Value.Year;

                        LeadSheetInfo.EndDate.Day = Group.EndDate.Value.Day;
                        LeadSheetInfo.EndDate.Month = Group.EndDate.Value.Month - 1;
                        LeadSheetInfo.EndDate.Year = Group.EndDate.Value.Year;

                        CompSettings.dictGroupsLeadSheetsInfos.Add(Group.id, LeadSheetInfo);
                    }

                    settings.dictCompSettings.TryAddValue(CompSettings.CompId, CompSettings);
                }

                DBManagerApp.m_AppSettings.Write();
            }
        }


        private void btnBrowseLeadReport_Click(object sender, RoutedEventArgs e)
        {
            string SelectedPath;

            if (BaseBrowse_Click(out SelectedPath, Properties.Resources.resAllExcellFilesFilter, true))
            {
                XlsPath = SelectedPath;
                HasLeadReport = !string.IsNullOrWhiteSpace(XlsPath);
                OnLeadReportRefreshed();
            }
        }


        public override void OnPropertyChanged(string info)
        {
            if (IsLoaded && info == XlsPathPropertyName)
            {
                HasLeadReport = !string.IsNullOrWhiteSpace(XlsPath);
                OnLeadReportRefreshed();
            }
            base.OnPropertyChanged(info);
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (CheckSettings())
                SaveSettings();
        }
    }
}
