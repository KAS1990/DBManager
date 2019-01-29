using DBManager.Global;
using DBManager.Stuff;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using MSExcel = Microsoft.Office.Interop.Excel;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Interaction logic for GenerationFromLocalWorkbookWnd.xaml
    /// </summary>
    public partial class GenerationFromLocalWorkbookWnd : CNotifyPropertyChangedWnd
    {
        public ObservableCollectionEx<CompDescLocalWorkbook> LocalDBComps { get; set; } = new ObservableCollectionEx<CompDescLocalWorkbook>();

        #region SecectedCompGroups
        private static readonly string SecectedCompGroupsPropertyName = GlobalDefines.GetPropertyName<GenerationFromLocalWorkbookWnd>(m => m.CompGroups);
        private ObservableCollection<GroupItemLocalWorkbook> m_CompGroups = new ObservableCollection<GroupItemLocalWorkbook>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GroupItemLocalWorkbook> CompGroups
        {
            get { return m_CompGroups; }
        }
        #endregion

        #region SelectedCompName
        private static readonly string SelectedCompNamePropertyName = GlobalDefines.GetPropertyName<GenerationFromLocalWorkbookWnd>(m => m.SelectedCompName);
        private string m_SelectedCompName = null;
        /// <summary>
        /// 
        /// </summary>
        public string SelectedCompName
        {
            get { return m_SelectedCompName; }
            set
            {
                if (m_SelectedCompName != value)
                {
                    m_SelectedCompName = value;
                    if (m_PrevSelectedComp != null && SelectedComp == null)
                    {   
                        /* Создали название соревнования, которого ещё нет
                         * => нужно создать соревнование и добавить его в LocalDBComps */
                        var compDesc = new CompDescLocalWorkbook()
                        {
                            ID = -1,
                            Name = m_SelectedCompName,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(2)
                        };
                        compDesc.CopyCompSpecificFields(m_PrevSelectedComp);
                        compDesc.PropertyChanged += comp_PropertyChanged;
                        LocalDBComps.Add(compDesc);
                        SelectedComp = compDesc;
                    }
                    else if (SelectedComp != null && SelectedComp.ID == -1)
                    {
                        SelectedComp.Name = m_SelectedCompName;
                    }
                    OnPropertyChanged(SelectedCompNamePropertyName);
                }
            }
        }
        #endregion

        #region SelectedComp
        private static readonly string SecectedCompPropertyName = GlobalDefines.GetPropertyName<GenerationFromLocalWorkbookWnd>(m => m.SelectedComp);
        private CompDescLocalWorkbook m_SelectedComp = null;
        private CompDescLocalWorkbook m_PrevSelectedComp = null;
        /// <summary>
        /// 
        /// </summary>
        public CompDescLocalWorkbook SelectedComp
        {
            get { return m_SelectedComp; }
            set
            {
                if (((value?.ID ?? 0) == -1) || (m_SelectedComp?.ID != value?.ID))
                {
                    if (value != null)
                    {
                        if (m_SelectedComp != null)
                        {
                            value.CopyCompSpecificFields(m_SelectedComp);
                            if (m_SelectedComp.ID == -1)
                            {   // Текущее соревнование было добавлено ранее в LocalDBComps => удаляем его
                                LocalDBComps.Remove(m_SelectedComp);
                            }
                        }

                        value.DestCompFolder = GetDefaultDestCompFolderName(value);
                    }

                    m_PrevSelectedComp = m_SelectedComp;
                    m_SelectedComp = value;

                    OnPropertyChanged(SecectedCompPropertyName);
                }
            }
        }
        #endregion

        public List<int> StartYears { get; set; } = new List<int>();
        public List<int> EndYears { get; set; } = new List<int>();
        public List<string> GroupNames { get; set; } = new List<string>();
        public ObservableCollection<string> SheetNames { get; set; } = new ObservableCollection<string>();

        #region Команды

        /// <summary>
        /// Команда добавления строки.
        /// </summary>
        public static RoutedCommand cmdInsertRow = new RoutedCommand();

        /// <summary>
        /// cmdInsertRow.Execute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertRowCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CompGroups.Add(new GroupItemLocalWorkbook(SelectedComp)
            {
                StartDate = SelectedComp.StartDate,
                EndDate = SelectedComp.EndDate,
                SheetName = SheetNames.FirstOrDefault(),
                PersonalDataColumnIndex = 1,
                TeamColumnIndex = 2,
                YoBColumnIndex = 3,
                GradeColumnIndex = 4
            });
        }

        /// <summary>
        /// cmdInsertRow.CanExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertRowCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        /*----------------------------------------------------------*/

        /// <summary>
        /// Команда удаления строки.
        /// </summary>
        public static RoutedCommand cmdDelRow = new RoutedCommand();

        /// <summary>
        /// cmdDelRow.Execute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelRowCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CompGroups.RemoveAt(lstvGroups.SelectedIndex);
        }

        /// <summary>
        /// cmdDelRow.CanExecute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelRowCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (lstvGroups != null)
                e.CanExecute = lstvGroups.SelectedItem != null;
        }
        #endregion

        public GenerationFromLocalWorkbookWnd()
        {
            InitializeComponent();
        }
        
        public bool InitWndControls()
        {
            using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(Title,
                                                                                            Properties.Resources.resFillingGenerationFromOnlineBDWnd,
                                                                                            this,
                                                                                            CheckAccess()),
                                            asyncResult =>
                                            {
                                                if (asyncResult?.hFinishedSearchEvent != null)
                                                    asyncResult.hFinishedSearchEvent.Set();
                                            }))
            {
                EndYears.Clear();
                StartYears.Clear();
                EndYears.Add((int)enEndYearSpecVals.AndYounger);
                EndYears.Add((int)enEndYearSpecVals.AndElder);
                for (int i = DateTime.Now.Year - 7; i > DateTime.Now.Year - 100; i--)
                {
                    StartYears.Add(i);
                    EndYears.Add(i);
                }


                GroupNames.Clear();
                lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                {
                    foreach (var groupNameDesc in DBManagerApp.m_AppSettings.m_Settings.AvailableGroupNames)
                    {
                        GroupNames.Add(groupNameDesc.GroupName);
                    }
                }

                // Заполняем выпадающие списки текущими значениями
                LocalDBComps.Clear();
                
                try
                {
                    foreach (var comp in DBManagerApp.m_Entities.descriptions.ToList())
                    {
                        var item = new CompDescLocalWorkbook()
                        {
                            ID = comp.id_desc,
                            Name = comp.name,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(2)
                        };
                        item.DestCompFolder = GetDefaultDestCompFolderName(item);
                        item.PropertyChanged += comp_PropertyChanged;

                        LocalDBComps.Add(item);
                    }
                    SelectedComp = LocalDBComps.LastOrDefault();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.resfmtErrorDuringReadingDataFromOnlineDB, ex.Message),
                        Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

                cmbMainJudge.Items.Clear();
                foreach (var mainJudge in DBManagerApp
                                        .m_Entities
                                        .groups
                                        .Select(arg => arg.main_judge)
                                        .Where(arg => !string.IsNullOrEmpty(arg))
                                        .Distinct())
                {
                    cmbMainJudge.Items.Add(mainJudge);
                }

                cmbMainSecretary.Items.Clear();
                foreach (var mainSecretary in DBManagerApp
                                        .m_Entities
                                        .groups
                                        .Select(arg => arg.main_secretary)
                                        .Where(arg => !string.IsNullOrEmpty(arg))
                                        .Distinct())
                {
                    cmbMainSecretary.Items.Add(mainSecretary);
                }

                cmbRow6.Items.Clear();
                foreach (var row6 in DBManagerApp
                                        .m_Entities
                                        .groups
                                        .Select(arg => arg.row6)
                                        .Where(arg => !string.IsNullOrEmpty(arg))
                                        .Distinct())
                {
                    cmbRow6.Items.Add(row6);
                }
            }

            return true;
        }

        private void RefreshSheetNames()
        {
            if (!File.Exists(SelectedComp.SourceWorkbookName)
                || System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) == GlobalDefines.MAIN_WBK_EXTENSION
                || System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) == GlobalDefines.XLS_EXTENSION
                || System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) == GlobalDefines.XLSX_EXTENSION)
            {
                SheetNames.Clear();

                using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(Title,
                                                                                            Properties.Resources.resGettingSheetNames,
                                                                                            this,
                                                                                            CheckAccess()),
                                            asyncResult =>
                                            {
                                                if (asyncResult?.hFinishedSearchEvent != null)
                                                    asyncResult.hFinishedSearchEvent.Set();
                                            }))
                {
                    try
                    {
                        using (var excelApp = new DisposableWrapper<ExcelApplicationEx>(GlobalDefines.StartExcel(),
                                                                                        app =>
                                                                                        {
                                                                                            if (app != null)
                                                                                            {
                                                                                                app.App.DisplayAlerts = true;

                                                                                                if (app.NewAppCreated)
                                                                                                    app.App.Quit();

                                                                                                app = null;
                                                                                            }
                                                                                        }))
                        {
                            bool WbkOpened;
                            var wbk = WorkbookGenerator.OpenWbk(excelApp, SelectedComp.SourceWorkbookName, out WbkOpened);

                            if (wbk != null)
                            {
                                foreach (MSExcel.Worksheet wsh in wbk.Worksheets)
                                    SheetNames.Add(wsh.Name);
                            }
                            else
                            {
                                MessageBox.Show(this,
                                    string.Format(Properties.Resources.resfmtSourceWorkbookCouldNotBeOpened, SelectedComp.SourceWorkbookName),
                                    Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this,
                                        string.Format(Properties.Resources.resfmtErrorDurExcelOperation, ex.Message),
                                        Title,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show(this, Properties.Resources.resInvalidSourceWorkbookName, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDefaultDestCompFolderName(CompDescLocalWorkbook comp)
        {
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                return System.IO.Path.Combine(DBManagerApp.m_AppSettings.m_Settings.CompetitionsFolder,
                                              comp.GetDefaultDestCompFolderName());
            }
        }

        private bool CheckSettings()
        {
            if (cmbComp.SelectedItem == null)
            {
                MessageBox.Show(this, Properties.Resources.resNoSelectedComp, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CompGroups.Count == 0)
            {
                MessageBox.Show(this, Properties.Resources.resNoGroupsInComp, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if ((from gr in CompGroups
                 group gr by gr.WorkbookName into groupNames
                 where groupNames.Count() > 1
                 select groupNames.Key)
                .Any())
            {
                MessageBox.Show(this, Properties.Resources.resDuplicateWorkbookNames, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CompGroups.Any(
                arg =>
                    (string.IsNullOrWhiteSpace(arg.WorkbookName)
                        || arg.WorkbookName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)))
            {
                MessageBox.Show(this,
                    string.Format(Properties.Resources.resfmtInvalidWorkbookNames, string.Join(", ", System.IO.Path.GetInvalidFileNameChars())),
                    Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (!File.Exists(SelectedComp.SourceWorkbookName)
                && System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) != GlobalDefines.MAIN_WBK_EXTENSION
                && System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) != GlobalDefines.XLS_EXTENSION
                && System.IO.Path.GetExtension(SelectedComp.SourceWorkbookName) != GlobalDefines.XLSX_EXTENSION)
            {
                MessageBox.Show(this, Properties.Resources.resInvalidSourceWorkbookName, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CompGroups.Any(arg =>
                        string.IsNullOrWhiteSpace(arg.SheetName)
                        || string.IsNullOrWhiteSpace(arg.TLCell)
                        || string.IsNullOrWhiteSpace(arg.BRCell)
                        || arg.PersonalDataColumnIndex <= 0
                        || arg.TeamColumnIndex <= 0
                        || arg.YoBColumnIndex <= 0
                        || arg.GradeColumnIndex <= 0))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidSourceRangeForExporting, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CompGroups.Any(arg =>
                        arg.EndYear.HasValue
                        && arg.EndYear != (int)enEndYearSpecVals.AndElder
                        && arg.EndYear != (int)enEndYearSpecVals.AndYounger
                        && arg.StartYear >= arg.EndYear))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidGroupYears, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (CompGroups.Any(arg =>
                    arg.EndDate.HasValue
                    && arg.StartDate != arg.EndDate))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidGroupDates, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(SelectedComp.DestCompFolder)
                || (SelectedComp.DestCompFolder.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidDestCompFolder, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Directory.Exists(DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidWorkbookTemplateFolder, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string WorkbookTemplateFullPath = System.IO.Path.Combine(DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder,
                                                    DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateName);
            if (!File.Exists(WorkbookTemplateFullPath))
            {
                MessageBox.Show(this,
                                string.Format(Properties.Resources.resfmtInvalidWorkbookTemplateName, WorkbookTemplateFullPath),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void btnDestFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = true
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedComp.DestCompFolder = dlg.SelectedPath;
            }
        }

        private void btnSourceWorkbookName_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Properties.Resources.resAllExcellFilesFilter
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedComp.SourceWorkbookName = dlg.FileName;
            }
        }

        private void GenerationFromLocalWorkbookWnd_Loaded(object sender, RoutedEventArgs e)
        {
            txtDestCompFolder.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            txtDestCompFolder.Width = txtDestCompFolder.DesiredSize.Width;

            Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            MaxWidth = MinWidth = Width = DesiredSize.Width;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSettings())
                return;

            var dataExtractor = new LocalWorkbookDataExtractor();
            var generator = new WorkbookGenerator(dataExtractor);

            using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(Title,
                                                                                            string.Format(Properties.Resources.resImportingCompetitions,
                                                                                                            SelectedComp.Name,
                                                                                                            SelectedComp.DestCompFolder),
                                                                                            this,
                                                                                            CheckAccess()),
                                            asyncResult =>
                                            {
                                                if (asyncResult?.hFinishedSearchEvent != null)
                                                    asyncResult.hFinishedSearchEvent.Set();
                                            }))
            {
                string errorMessage = null;
                if (!dataExtractor.Extract(SelectedComp, CompGroups, out errorMessage))
                {
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.resfmtCouldNotExtractDataFromRemoteDB, errorMessage),
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                if (!generator.Generate(out errorMessage,
                        arg =>
                            CWaitingWnd.SetPrompt(((ShowAsyncResult)wrapper).WndID,
                                                    string.Format(Properties.Resources.resImportingCompetitionsWithGroupName,
                                                                   SelectedComp.Name,
                                                                   SelectedComp.DestCompFolder,
                                                                   arg.Key.Name))))
                {
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.resfmtCouldNotExtractDataToWbks, errorMessage),
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Показываем книгу в проводнике
                Process.Start(SelectedComp.DestCompFolder);

                MessageBox.Show(this,
                        Properties.Resources.resDataIsExtractedToWbksSuccessfully,
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        void comp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CompDescLocalWorkbook.StartDatePropertyName
                || e.PropertyName == CompDescLocalWorkbook.EndDatePropertyName)
            {
                (sender as CompDescLocalWorkbook).DestCompFolder = GetDefaultDestCompFolderName((sender as CompDescLocalWorkbook));
            }
            else if (e.PropertyName == CompDescLocalWorkbook.SourceWorkbookNamePropertyName)
            {
                RefreshSheetNames();
            }
        }

        private void lstvGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
