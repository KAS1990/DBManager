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
using System.Windows.Shapes;
using DBManager.Global;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DBManager.Scanning.XMLDataClasses;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using DBManager.Stuff;
using DBManager.Excel.Exporting.Tabs;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Excel.Exporting.ExportingClasses;
using DBManager.ReportGenerators;

namespace DBManager.Excel.Exporting
{
    /// <summary>
    /// Interaction logic for CExportToExcelWnd.xaml
    /// </summary>
    public partial class CExportToExcelWnd : CNotifyPropertyChangedWnd
    {
        public readonly descriptions m_CompDesc = null;

        
        #region ShowWbkAfterExport
        private static readonly string ShowWbkAfterExportPropertyName = GlobalDefines.GetPropertyName<CExportToExcelWnd>(m => m.ShowWbkAfterExport);

        private bool m_ShowWbkAfterExport = false;

        public bool ShowWbkAfterExport
        {
            get { return m_ShowWbkAfterExport; }
            set
            {
                if (m_ShowWbkAfterExport != value)
                {
                    m_ShowWbkAfterExport = value;
                    OnPropertyChanged(ShowWbkAfterExportPropertyName);
                }
            }
        }
        #endregion
                

        private static readonly string MaxSheetNameLenPropertyName = GlobalDefines.GetPropertyName<CExportToExcelWnd>(m => m.MaxSheetNameLen);
        public int MaxSheetNameLen
        {
            get { return DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.MaxSheetNameLen; }
        }


        CMainExportTab MainExportTab
        {
            get { return (tbctrlTabs.Items[0] as TabItem).Content as CMainExportTab; }
        }


        CTeamExportTab TeamExportTab
        {
            get { return (tbctrlTabs.Items[1] as TabItem).Content as CTeamExportTab; }
        }


        CPersonalExportTab PersonalExportTab
        {
            get { return (tbctrlTabs.Items[2] as TabItem).Content as CPersonalExportTab; }
        }


        CLeadReportInfoTab LeadReportInfoTab
        {
            get { return (tbctrlTabs.Items[3] as TabItem).Content as CLeadReportInfoTab; }
        }
        
                
        public CExportToExcelWnd()
        {
            InitializeComponent();
        }


        public CExportToExcelWnd(long CompId, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups)
        {
            InitializeComponent();

            OnPropertyChanged(MaxSheetNameLenPropertyName);
                        
            m_CompDesc = DBManagerApp.m_Entities.descriptions.Where(arg => arg.id_desc == CompId).FirstOrDefault();

            tbctrlTabs.Items.Add(new TabItem()
                {
                    Header = Properties.Resources.resMainExportTab,
                    Content = new CMainExportTab(this, CompGroups)
                });
            tbctrlTabs.Items.Add(new TabItem()
            {
                Header = Properties.Resources.resTeamExportTab,
                Content = new CTeamExportTab(this, CompGroups)
            });
            tbctrlTabs.Items.Add(new TabItem()
            {
                Header = Properties.Resources.resPersonalExportTab,
                Content = new CPersonalExportTab(this, CompGroups)
            });
            tbctrlTabs.Items.Add(new TabItem()
            {
                Header = Properties.Resources.resLeadReportInfoTab,
                Content = new CLeadReportInfoTab(this, CompGroups)
            });
        }


        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            dpMain.IsEnabled = false;

            bool HasSelectedTab = false;
            foreach (TabItem ti in tbctrlTabs.Items)
            {
                if (!(ti.Content as CExportingTabBase).CheckSettings())
                {
                    ti.IsSelected = true;
                    dpMain.IsEnabled = true;
                    return;
                }
                HasSelectedTab |= (ti.Content as CExportingTabBase).CreateReport;
            }

            if (!HasSelectedTab)
            {	// Ничего не выбрано
                MessageBox.Show(this,
                                Properties.Resources.resNoTaskToExport,
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                dpMain.IsEnabled = true;
                return;
            }

            if (MainExportTab.CreateReport)
            {
                if (string.IsNullOrWhiteSpace(txtXlsPath.Text) ||
                    !Directory.Exists(System.IO.Path.GetDirectoryName(txtXlsPath.Text)) ||
                    System.IO.Path.GetExtension(txtXlsPath.Text) != GlobalDefines.XLSX_EXTENSION)
                {
                    MessageBox.Show(this,
                                    Properties.Resources.resInvalidXlsxPath,
                                    Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    dpMain.IsEnabled = true;
                    return;
                }

                if (File.Exists(txtXlsPath.Text))
                {	// Перезаписывать файлы нельзя
                    MessageBox.Show(this,
                                    Properties.Resources.resXlsxFileAlreadyExists,
                                    Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    dpMain.IsEnabled = true;
                    return;
                }
            }

            if (TeamExportTab.HasLeadReport && LeadReportInfoTab.Groups.FirstOrDefault(arg => arg.LeadSheetIndex < 0) != null)
            {	// Для командного отчёта нужно присвоить листы всем группам в трудности
                MessageBox.Show(this,
                                Properties.Resources.resGroupWithoutLeadSheetIndex,
                                Properties.Resources.resError,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                (TeamExportTab.Parent as TabItem).IsSelected = true;
                dpMain.IsEnabled = true;
                return;
            }

            foreach (TabItem ti in tbctrlTabs.Items)
                (ti.Content as CExportingTabBase).BeforeExporting();
            
            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;
            if ((DBManagerApp.MainWnd as DispatcherObject).CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        Title,
                                        string.Format(Properties.Resources.resfmtExportingToExcel, txtXlsPath.Text),
                                        this);
            }

            bool NewAppCreated;
            MSExcel.Application excelApp = GlobalDefines.StartExcel(out NewAppCreated);

            if (excelApp != null)
            {
                excelApp.DisplayAlerts = false; // Отключаем различные сообщения

                MSExcel.Workbook wbkTarget = null;
                MSExcel.Workbook wbkTemplates = null;
                bool WbkTemplatesOpened = false;

                string WbkTargetPath = txtXlsPath.Text;
                
                try
                {
                    if (MainExportTab.CreateReport)
                    {
                        // Создаём книгу, в которую будем добавлять листы
                        wbkTarget = excelApp.Workbooks.Add(MSExcel.XlWBATemplate.xlWBATWorksheet);
                        if (wbkTarget == null)
                        {
                            MessageBox.Show(this,
                                            string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                            Properties.Resources.resCantCreateTargetWbk),
                                            Title,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                            throw new NotImplementedException();
                        }

                        // Открываем книгу с шаблонами
                        wbkTemplates = OpenWbkTemplates(excelApp, NewAppCreated, out WbkTemplatesOpened);
                        if (wbkTemplates == null)
                            throw new NotImplementedException();

                        CReportExporterBase.CTask Task = null;
                        foreach (CGroupItem GroupItem in MainExportTab.SelectedGroups)
                        {
                            foreach (CRoundItem RoundItem in GroupItem.Rounds.Where(arg => arg.IsSelected))
                            {
                                switch ((enReportTypes)RoundItem.id)
                                {
                                    case enReportTypes.Qualif:
                                    case enReportTypes.Qualif2:
                                        Task = new CQualifExporter.CQualifTask()
                                        {
                                            m_ReportType = (enReportTypes)RoundItem.id,
                                            m_CompDesc = this.m_CompDesc,
                                            m_GroupToExport = GroupItem
                                        };
                                        break;
                                    
                                    case enReportTypes.OneEighthFinal:
                                    case enReportTypes.QuaterFinal:
                                    case enReportTypes.SemiFinal:
                                        Task = new CMiddleSheetsExporter.CMiddleSheetsTask()
                                        {
                                            m_ReportType = (enReportTypes)RoundItem.id,
                                            m_CompDesc = this.m_CompDesc,
                                            m_GroupToExport = GroupItem
                                        };
                                        break;
                                    
                                    case enReportTypes.Final:
                                        Task = new CFinalExporter.CFinalTask()
                                        {
                                            m_ReportType = (enReportTypes)RoundItem.id,
                                            m_CompDesc = this.m_CompDesc,
                                            m_GroupToExport = GroupItem
                                        };
                                        break;
                                    
                                    case enReportTypes.Total:
                                        Task = new CTotalExporter.CTotalTask()
                                        {
                                            m_ReportType = (enReportTypes)RoundItem.id,
                                            m_CompDesc = this.m_CompDesc,
                                            m_GroupToExport = GroupItem
                                        };
                                        break;
                                }

                                CReportExporterBase ReportExporter = CReportExporterBase.GetReportClass(Task);
                                if (ReportExporter == null ||
                                    !ReportExporter.CreateReport(excelApp, wbkTarget, wbkTemplates))
                                {
                                    MessageBox.Show(this,
                                        string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                        string.Format(Properties.Resources.resfmtErrDurQualifExporting,
                                                                        GroupItem.Name,
                                                                        RoundItem.RoundName)),
                                        Title,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                }
                            }
                        }
                    }

                    if (TeamExportTab.CreateReport)
                    {
                        if (!TeamExportTab.HasLeadReport || LeadReportInfoTab.HasLeadReport)
                        {
                            bool WbkLeadReportOpened = false;
                            bool CreateNewWbk = false;
                            if (wbkTarget == null)
                            {	// Книга не была ранее создана
                                CreateNewWbk = true;
                                if (TeamExportTab.ExportToAnotherWbk)
                                    WbkTargetPath = TeamExportTab.XlsPath;
                            }
                            else if (TeamExportTab.ExportToAnotherWbk)
                            {	// Сохраняем текущую книгу, т.к. двоеборье нужно добавлять в новую
                                CreateNewWbk = true;
                                if (wbkTarget.Worksheets.Count > 1)
                                {	// Удалось добавить хотя бы 1 лист
                                    // Удаляем первый лист, который был создан автоматически при создании книги
                                    wbkTarget.Worksheets[1].Delete();
                                    wbkTarget.SaveAs(WbkTargetPath);
                                    if (!ShowWbkAfterExport)
                                        wbkTarget.Close();
                                }
                                else
                                {
                                    wbkTarget.Close();
                                    File.Delete(WbkTargetPath);
                                }
                                WbkTargetPath = TeamExportTab.XlsPath;

                                wbkTarget = null;
                            }

                            // Создаём книгу, в которую будем добавлять листы
                            if (CreateNewWbk)
                            {
                                wbkTarget = excelApp.Workbooks.Add(MSExcel.XlWBATemplate.xlWBATWorksheet);
                                if (wbkTarget == null)
                                {
                                    MessageBox.Show(this,
                                                    string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                                    Properties.Resources.resCantCreateTargetWbk),
                                                    Title,
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                                    throw new NotImplementedException();
                                }
                            }

                            if (wbkTemplates == null)
                            {	// Открываем книгу с шаблонами
                                wbkTemplates = OpenWbkTemplates(excelApp, NewAppCreated, out WbkTemplatesOpened);
                                if (wbkTemplates == null)
                                    throw new NotImplementedException();
                            }

                            CTeamExporter.CTeamTask Task = new CTeamExporter.CTeamTask()
                            {
                                m_ReportType = enReportTypes.Team,
                                m_CompDesc = this.m_CompDesc,

                                m_Teams = TeamExportTab.TeamsForReport.ToList(),
                                m_MenInGroup = TeamExportTab.MenInGroup,
                                m_WomenInGroup = TeamExportTab.WomenInGroup,
                            };
                            if (TeamExportTab.HasLeadReport)
                            {
                                Task.m_LeadReportInfo.m_wbkLeadReport = OpenWbkLeadReport(excelApp, NewAppCreated, LeadReportInfoTab.XlsPath, out WbkLeadReportOpened);
                                Task.m_LeadReportInfo.m_dictLeadGroupInfos = LeadReportInfoTab.Groups.ToDictionary(arg => arg.id);
                                Task.m_LeadReportInfo.m_LeadSheets = LeadReportInfoTab.LeadSheets.ToList();
                                Task.m_LeadReportInfo.m_FirstMemberRow = LeadReportInfoTab.FirstMemberRow;
                                Task.m_LeadReportInfo.m_PersonalDataColumnIndex = LeadReportInfoTab.PersonalDataColumnIndex;
                                Task.m_LeadReportInfo.m_PlaceColumnIndex = LeadReportInfoTab.PlaceColumnIndex;
                                Task.m_LeadReportInfo.m_YearOfBirthColumnIndex = LeadReportInfoTab.YearOfBirthColumnIndex;
                                Task.m_LeadReportInfo.m_TeamColumnIndex = LeadReportInfoTab.TeamColumnIndex;
                            }

                            CReportExporterBase ReportExporter = CReportExporterBase.GetReportClass(Task);
                            if (ReportExporter == null ||
                                !ReportExporter.CreateReport(excelApp, wbkTarget, wbkTemplates))
                            {
                                MessageBox.Show(this,
                                    string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                    Properties.Resources.resErrDurTeamExporting),
                                    Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }

                            if (Task.m_LeadReportInfo.m_wbkLeadReport != null && WbkLeadReportOpened)
                            {	// Закрываем открытую ранее книгу с отчётом трудности
                                Task.m_LeadReportInfo.m_wbkLeadReport.Close();
                            }
                        }
                        else
                        {	// Не выбран протокол трудности
                            MessageBox.Show(this,
                                            string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                            Properties.Resources.resNoLeadReport),
                                            Title,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }

                    if (PersonalExportTab.CreateReport)
                    {
                        if (!PersonalExportTab.HasLeadReport || LeadReportInfoTab.HasLeadReport)
                        {
                            bool WbkLeadReportOpened = false;
                            bool CreateNewWbk = false;
                            if (wbkTarget == null)
                            {	// Книга не была ранее создана
                                CreateNewWbk = true;
                                if (PersonalExportTab.ExportToAnotherWbk)
                                    WbkTargetPath = PersonalExportTab.XlsPath;
                            }
                            else if (PersonalExportTab.ExportToAnotherWbk)
                            {	// Сохраняем текущую книгу, т.к. двоеборье нужно добавлять в новую
                                CreateNewWbk = true;
                                if (wbkTarget.Worksheets.Count > 1)
                                {	// Удалось добавить хотя бы 1 лист
                                    // Удаляем первый лист, который был создан автоматически при создании книги
                                    wbkTarget.Worksheets[1].Delete();
                                    wbkTarget.SaveAs(WbkTargetPath);
                                    if (!ShowWbkAfterExport)
                                        wbkTarget.Close();
                                }
                                else
                                {
                                    wbkTarget.Close();
                                    File.Delete(WbkTargetPath);
                                }
                                WbkTargetPath = PersonalExportTab.XlsPath;

                                wbkTarget = null;
                            }

                            // Создаём книгу, в которую будем добавлять листы
                            if (CreateNewWbk)
                            {
                                wbkTarget = excelApp.Workbooks.Add(MSExcel.XlWBATemplate.xlWBATWorksheet);
                                if (wbkTarget == null)
                                {
                                    MessageBox.Show(this,
                                                    string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                                    Properties.Resources.resCantCreateTargetWbk),
                                                    Title,
                                                    MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                                    throw new NotImplementedException();
                                }
                            }

                            if (wbkTemplates == null)
                            {	// Открываем книгу с шаблонами
                                wbkTemplates = OpenWbkTemplates(excelApp, NewAppCreated, out WbkTemplatesOpened);
                                if (wbkTemplates == null)
                                    throw new NotImplementedException();
                            }

                            CPersonalExporter.CPersonalTask Task = new CPersonalExporter.CPersonalTask()
                            {
                                m_ReportType = enReportTypes.Personal,
                                m_CompDesc = this.m_CompDesc,

                                m_Groups = PersonalExportTab.SelectedGroups,
                                m_PlaceAggregationMethod = PersonalExportTab.PlaceAggregationMethod,
                                m_WinnerDetection = PersonalExportTab.WinnerDetection,
                                m_PriorityCompetitionKind = PersonalExportTab.PriorityCompetitionKind
                            };
                            if (PersonalExportTab.HasLeadReport)
                            {
                                Task.m_LeadReportInfo.m_wbkLeadReport = OpenWbkLeadReport(excelApp, NewAppCreated, LeadReportInfoTab.XlsPath, out WbkLeadReportOpened);
                                Task.m_LeadReportInfo.m_dictLeadGroupInfos = LeadReportInfoTab.Groups.ToDictionary(arg => arg.id);
                                Task.m_LeadReportInfo.m_LeadSheets = LeadReportInfoTab.LeadSheets.ToList();
                                Task.m_LeadReportInfo.m_FirstMemberRow = LeadReportInfoTab.FirstMemberRow;
                                Task.m_LeadReportInfo.m_PersonalDataColumnIndex = LeadReportInfoTab.PersonalDataColumnIndex;
                                Task.m_LeadReportInfo.m_PlaceColumnIndex = LeadReportInfoTab.PlaceColumnIndex;
                                Task.m_LeadReportInfo.m_YearOfBirthColumnIndex = LeadReportInfoTab.YearOfBirthColumnIndex;
                                Task.m_LeadReportInfo.m_TeamColumnIndex = LeadReportInfoTab.TeamColumnIndex;
                            }

                            CReportExporterBase ReportExporter = CReportExporterBase.GetReportClass(Task);
                            if (ReportExporter == null ||
                                !ReportExporter.CreateReport(excelApp, wbkTarget, wbkTemplates))
                            {
                                MessageBox.Show(this,
                                    string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                    Properties.Resources.resErrDurPersonalExporting),
                                    Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }

                            if (Task.m_LeadReportInfo.m_wbkLeadReport != null && WbkLeadReportOpened)
                            {	// Закрываем открытую ранее книгу с отчётом трудности
                                Task.m_LeadReportInfo.m_wbkLeadReport.Close();
                            }
                        }
                        else
                        {	// Не выбран протокол трудности
                            MessageBox.Show(this,
                                            string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                            Properties.Resources.resNoLeadReport),
                                            Title,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is NotImplementedException))
                    {
                        MessageBox.Show(this,
                                        string.Format(Properties.Resources.resfmtErrorDurExcelOperation, ex.Message),
                                        Title,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }

                if (wbkTarget != null)
                {
                    if (wbkTarget.Worksheets.Count > 1)
                    {   // Удалось добавить хотя бы 1 лист
                        // Удаляем первый лист, который был создан автоматически при создании книги
                        // Перед этим обязательно нужно показать книгу, т.к. без этого метод Delete падает
                        excelApp.Visible = true;
                        excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                        excelApp.WindowState = MSExcel.XlWindowState.xlNormal;
                        excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                        wbkTarget.Worksheets[1].Delete();
                        wbkTarget.SaveAs(WbkTargetPath);

                        if (ShowWbkAfterExport)
                        {
                            NewAppCreated = false;
                            wbkTarget.Activate();
                        }
                        else
                        {
                            excelApp.Visible = false;
                            wbkTarget.Close();
                        }
                    }
                    else
                    {
                        wbkTarget.Close();
                        File.Delete(WbkTargetPath);
                    }
                }

                if (wbkTemplates != null && WbkTemplatesOpened)
                    wbkTemplates.Close();
                wbkTemplates = null;

                excelApp.DisplayAlerts = true;

                if (NewAppCreated)
                    excelApp.Quit();

                excelApp = null;
            }
            else
            {
                MessageBox.Show(this,
                                string.Format(Properties.Resources.resfmtErrorDurExcelOperation, Properties.Resources.resCantCreateExcelApp),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
                        
            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();

            MessageBox.Show(this,
                            Properties.Resources.resExportToExcelFinished,
                            Title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            dpMain.IsEnabled = true;
        }


        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog()
                {
                    CheckFileExists = false,
                    CreatePrompt = false,
                    AddExtension = true,
                    InitialDirectory = DBManagerApp.m_AppSettings.m_Settings.CompDir,
                    OverwritePrompt = true,
                    ValidateNames = true,
                    Filter = Properties.Resources.resXlsxFilesFilter,
                    DefaultExt = GlobalDefines.XLSX_EXTENSION
                };

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtXlsPath.Text = dlg.FileName;
                }
            }
        }


        private void CExportToExcelWnd_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalDefines.AutoscaleTabs(tbctrlTabs, null, null);
        }


        private MSExcel.Workbook OpenWbkTemplates(MSExcel.Application excelApp, bool NewAppCreated, out bool WbkTemplatesOpened)
        {
            MSExcel.Workbook result = null;

            WbkTemplatesOpened = false;

            // Открываем книгу с шаблонами
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                if (NewAppCreated)
                {	// Без этого книга не открывается
                    excelApp.Visible = true;
                    excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                }
                foreach (MSExcel.Workbook book in excelApp.Workbooks)
                {
                    if (book.FullName == GlobalDefines.STD_APP_CONFIGS_DIR + "\\" +
                                            DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.ReportTemplatesWbkName)
                    {	// Книга уже открыта => используем её
                        result = book;
                        break;
                    }
                }
                if (result == null)
                {
                    result = excelApp.Workbooks.Open(GlobalDefines.STD_APP_CONFIGS_DIR + "\\" +
                                                                DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.ReportTemplatesWbkName,
                                                            ReadOnly: true);
                    WbkTemplatesOpened = true;
                }
                if (NewAppCreated)
                {
                    excelApp.Visible = false;
                    excelApp.WindowState = MSExcel.XlWindowState.xlNormal;
                }
            }
            if (result == null)
            {
                MessageBox.Show(this,
                                string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                Properties.Resources.resCantCreateTemplateWbk),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            return result;
        }


        /// <summary>
        /// Открываем книгу с отчётом трудности
        /// </summary>
        /// <param name="excelApp"></param>
        /// <param name="NewAppCreated"></param>
        /// <param name="WbkFullPath"></param>
        /// <param name="WbkLeadReportOpened"></param>
        /// <returns></returns>
        protected MSExcel.Workbook OpenWbkLeadReport(MSExcel.Application excelApp,
                                                    bool NewAppCreated,
                                                    string WbkFullPath,
                                                    out bool WbkLeadReportOpened)
        {
            MSExcel.Workbook result = null;

            WbkLeadReportOpened = false;

            // Открываем книгу с шаблонами
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                if (NewAppCreated)
                {	// Без этого книга не открывается
                    excelApp.Visible = true;
                    excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                }
                foreach (MSExcel.Workbook book in excelApp.Workbooks)
                {
                    if (book.FullName == WbkFullPath)
                    {	// Книга уже открыта => используем её
                        result = book;
                        break;
                    }
                }
                if (result == null)
                {
                    result = excelApp.Workbooks.Open(WbkFullPath, ReadOnly: true);
                    WbkLeadReportOpened = true;
                }
                if (NewAppCreated)
                {
                    excelApp.Visible = false;
                    excelApp.WindowState = MSExcel.XlWindowState.xlNormal;
                }
            }
            if (result == null)
            {
                MessageBox.Show(this,
                                string.Format(Properties.Resources.resfmtErrorDurExcelOperation,
                                                string.Format(Properties.Resources.resfmtCantOpenWbkLead, WbkFullPath)),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            return result;
        }
    }
}
