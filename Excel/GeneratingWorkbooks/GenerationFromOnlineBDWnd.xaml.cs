using DBManager.Excel.GeneratingWorkbooks.Helpers;
using DBManager.Global;
using DBManager.OnlineDB;
using DBManager.Stuff;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Interaction logic for GenerationFromOnlineBDWnd.xaml
    /// </summary>
    public partial class GenerationFromOnlineBDWnd : CNotifyPropertyChangedWnd
    {                        
        OnlineDBManager m_DBManager = OnlineDBManager.Instance;

        public ObservableCollectionEx<CompItem> RemoteDBComps { get; set; } = new ObservableCollectionEx<CompItem>();

        #region SecectedCompGroups
        private static readonly string SecectedCompGroupsPropertyName = GlobalDefines.GetPropertyName<GenerationFromOnlineBDWnd>(m => m.SecectedCompGroups);
        private ObservableCollection<GroupItemRemoteDB> m_SecectedCompGroups = new ObservableCollection<GroupItemRemoteDB>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GroupItemRemoteDB> SecectedCompGroups
        {
            get { return m_SecectedCompGroups; }
        }
        #endregion

        #region SelectedComp
        private static readonly string SecectedCompPropertyName = GlobalDefines.GetPropertyName<GenerationFromOnlineBDWnd>(m => m.SelectedComp);
        private CompItem m_SelectedComp = null;
        /// <summary>
        /// 
        /// </summary>
        public CompItem SelectedComp
        {
            get { return m_SelectedComp; }
            set
            {
                if (m_SelectedComp?.Desc.ID != value?.Desc.ID)
                {
                    SecectedCompGroups.Clear();
                    if (value != null)
                    {
                        if (m_SelectedComp != null)
                            value.Desc.CopyNonRemoteFields(m_SelectedComp.Desc);

                        value.Desc.UpdateDatesFromRemoteOnes();
                        foreach (var group in value.Groups)
                        {   // Создаём копию, чтобы не затирать исходные данные
                            SecectedCompGroups.Add(new GroupItemRemoteDB(group));
                        }

                        lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                        {
                            value.Desc.DestCompFolder = DBManagerApp.m_AppSettings.m_Settings.CompetitionsFolder;

                            if (value.Desc.EndDate.HasValue && value.Desc.StartDate != value.Desc.EndDate.Value)
                            {
                                if (value.Desc.StartDate.Year == value.Desc.EndDate.Value.Year)
                                {
                                    if (value.Desc.StartDate.Month == value.Desc.EndDate.Value.Month)
                                    {
                                        value.Desc.DestCompFolder = System.IO.Path.Combine(value.Desc.DestCompFolder,
                                                                                string.Format("{0:D2}-{1:D2}.{2:D2}.{3:D2}",
                                                                                                value.Desc.StartDate.Day,
                                                                                                value.Desc.EndDate.Value.Day,
                                                                                                value.Desc.StartDate.Month,
                                                                                                value.Desc.StartDate.Year));
                                    }
                                    else
                                    {
                                        value.Desc.DestCompFolder = System.IO.Path.Combine(value.Desc.DestCompFolder,
                                                                                string.Format("{0:D2}.{1:D2}-{2:D2}.{3:D2}.{4:D2}",
                                                                                                value.Desc.StartDate.Day,
                                                                                                value.Desc.StartDate.Month,
                                                                                                value.Desc.EndDate.Value.Day,
                                                                                                value.Desc.EndDate.Value.Month,
                                                                                                value.Desc.StartDate.Year));
                                    }
                                }
                            }
                            else
                            {
                                value.Desc.DestCompFolder = System.IO.Path.Combine(value.Desc.DestCompFolder,
                                                                               string.Format("{0:D2}.{1:D2}.{2:D2}",
                                                                                               value.Desc.StartDate.Day,
                                                                                               value.Desc.StartDate.Month,
                                                                                               value.Desc.StartDate.Year));
                            }
                        }
                    }

                    m_SelectedComp = value;

                    OnPropertyChanged(SecectedCompPropertyName);
                }
            }
        }
        #endregion
                
        public List<int> StartYears { get; set; } = new List<int>();
        public List<int> EndYears { get; set; } = new List<int>();

        public GenerationFromOnlineBDWnd()
        {
            InitializeComponent();
        }

        public bool InitWndControls()
        {
            using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(Title,
                                                                                            Properties.Resources.resFillingGenerationFromOnlineBDWnd,
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

                // Заполняем выпадающие списки текущими значениями
                RemoteDBComps.Clear();
                if (OnlineDBManager.Instance.IsConnectedToRemoteDB)
                {
                    try
                    {
                        var speedGroups = m_DBManager
                                            .Entities
                                            .group
                                            .Where(gr =>
                                                    gr
                                                        .participants
                                                        .SelectMany(part => part.participants_kind)
                                                        .Any(kind => kind.kind_id == (int)enOnlineDBKind.Speed))
                                            .ToList();

                        // Выбираем только соревы на скорость
                        foreach (var comp in speedGroups.SelectMany(gr => gr.events).Distinct().ToList())
                        {
                            var item = new CompItem();
                            item.Desc.ID = comp.id;
                            item.Desc.Name = comp.name;
                            item.Desc.RemoteStartDate = comp.date1;
                            item.Desc.RemoteEndDate = comp.date2;
                            item.Desc.UpdateDatesFromRemoteOnes();

                            foreach (var group in speedGroups.Where(gr => gr.events.Any(ev => ev.id == comp.id)))
                            {
                                if (DBManagerApp.m_AppSettings.m_Settings.AvailableGroupNames.Any(arg => string.Compare(arg.GroupName, group.name, true) == 0))
                                {
                                    var groupItem = new GroupItemRemoteDB(item.Desc)
                                    {
                                        ID = group.id,
                                        Name = group.name,
                                        Sex = ((enOnlineSex)(group.sex ? 1 : 0)).ToLocalSexValue(),
                                        StartYear = DateTime.Now.Year - group.year2,
                                        EndYear = group.year1.HasValue ? DateTime.Now.Year - group.year1 : null,
                                        StartDate = item.Desc.StartDate,
                                        EndDate = item.Desc.EndDate
                                    };
                                    groupItem.FillWorkbookName();
                                    item.Groups.Add(groupItem);
                                }
                            }

                            RemoteDBComps.Add(item);
                        }
                        SelectedComp = RemoteDBComps.LastOrDefault();
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

        private bool CheckSettings()
        {
            if (cmbComp.SelectedItem == null)
            {
                MessageBox.Show(this, Properties.Resources.resNoSelectedComp, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (SecectedCompGroups.Count == 0 || SecectedCompGroups.All(arg => !arg.IsSelected))
            {
                MessageBox.Show(this, Properties.Resources.resNoGroupsInComp, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if ((from gr in SecectedCompGroups
                 where gr.IsSelected
                 group gr by gr.WorkbookName into groupNames
                 where groupNames.Count() > 1
                 select groupNames.Key)
                .Any())
            {
                MessageBox.Show(this, Properties.Resources.resDuplicateWorkbookNames, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
                        
            if (SecectedCompGroups.Any(
                arg =>
                    arg.IsSelected
                        && (string.IsNullOrWhiteSpace(arg.WorkbookName)
                            || arg.WorkbookName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)))
            {
                MessageBox.Show(this,
                    string.Format(Properties.Resources.resfmtInvalidWorkbookNames, string.Join(", ", System.IO.Path.GetInvalidFileNameChars())),
                    Title,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }

            if (SecectedCompGroups.Any(arg =>
                    arg.IsSelected
                        && arg.EndYear.HasValue
                        && arg.EndYear != (int)enEndYearSpecVals.AndElder
                        && arg.EndYear != (int)enEndYearSpecVals.AndYounger
                        && arg.StartYear >= arg.EndYear))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidGroupYears, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (SecectedCompGroups.Any(arg =>
                    arg.IsSelected
                        && arg.EndDate.HasValue
                        && arg.StartDate != arg.EndDate))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidGroupDates, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(SelectedComp.Desc.DestCompFolder)
                || (SelectedComp.Desc.DestCompFolder.IndexOfAny(System.IO.Path.GetInvalidPathChars()) != -1))
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
                SelectedComp.Desc.DestCompFolder = dlg.SelectedPath;
            }
        }

        private void GenerationFromOnlineBDWnd_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_DBManager.IsConnectedToRemoteDB)
            {
                string msg = string.Format(DBManager.Properties.Resources.resrmtCantConnectToRemoteDB, OnlineDBManager.Instance.ConnectionString);
                MessageBox.Show(this, msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtDestCompFolder.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            txtDestCompFolder.Width = txtDestCompFolder.DesiredSize.Width;

            Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            MaxWidth = MinWidth = Width = DesiredSize.Width;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSettings())
                return;

            var dataExtractor = new RemoteDBDataExtractor();
            var generator = new WorkbookGenerator(dataExtractor);

            using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(Title,
                                                                                            string.Format(Properties.Resources.resImportingCompetitions,
                                                                                                            SelectedComp.Desc.Name,
                                                                                                            SelectedComp.Desc.DestCompFolder),
                                                                                            CheckAccess()),
                                            asyncResult =>
                                            {
                                                if (asyncResult?.hFinishedSearchEvent != null)
                                                    asyncResult.hFinishedSearchEvent.Set();
                                            }))
            {
                string errorMessage = null;
                if (!dataExtractor.Extract(SelectedComp.Desc, SecectedCompGroups, out errorMessage))
                {
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.resfmtCouldNotExtractDataFromRemoteDB, errorMessage),
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                if (!generator.Generate(out errorMessage))
                {
                    MessageBox.Show(this,
                        string.Format(Properties.Resources.resfmtCouldNotExtractDataToWbks, errorMessage),
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Показываем книгу в проводнике
                Process.Start(SelectedComp.Desc.DestCompFolder);

                MessageBox.Show(this,
                        Properties.Resources.resDataIsExtractedToWbksSuccessfully,
                        AppAttributes.Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var group in SecectedCompGroups)
            {
                group.IsSelected = chkSelectAll.IsChecked ?? false;
            }
        }
    }


    [ValueConversion(typeof(int), typeof(string))]
    public class YearToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int? || value is int) || value == null)
                return null;

            if (value is int? && (value as int?) == null)
                return null;

            return GroupItemRemoteDB.CreateYearInString(value is int? ? (value as int?) : (int?)value);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string) || (targetType != typeof(int) && targetType != typeof(int?)) || value == null)
                return null;

            int res = 0;
            if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndElder;
            else if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndYounger;
            else if (!int.TryParse(value.ToString(), out res))
                return null;

            if (targetType == typeof(int))
                return res;
            else
                return (int?)res;
        }
    }

    [ValueConversion(typeof(enSecondColNameType), typeof(bool))]
    public class SecondColNameType2NullBoolConverter : IValueConverter
    {
        /// <summary>
        /// Это значение конвертируется в true и обратно
        /// </summary>
        public enSecondColNameType TrueValue { get; set; } = enSecondColNameType.None;

        /// <summary>
        /// Это значение конвертируется в false и обратно
        /// </summary>
        public enSecondColNameType FalseValue { get; set; } = enSecondColNameType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is enSecondColNameType) || (value is null))
                return null;

            var secondColNameType = (enSecondColNameType)value;

            if (secondColNameType == TrueValue)
                return (bool?)true;

            if (secondColNameType == FalseValue)
                return (bool?)false;

            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return null;

            var val = (bool?)value;
            if (val.HasValue)
                return val.Value ? TrueValue : FalseValue;
            else
                return enSecondColNameType.None;
        }
    }

    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class MembersCountToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int) || value == null)
                return Brushes.Transparent;

            int count = (int)value;
            if (count < 4)
                return Brushes.Maroon;
            else
                return Brushes.Transparent;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string) || (targetType != typeof(int) && targetType != typeof(int?)) || value == null)
                return null;

            int res = 0;
            if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndElder;
            else if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndYounger;
            else if (!int.TryParse(value.ToString(), out res))
                return null;

            if (targetType == typeof(int))
                return res;
            else
                return (int?)res;
        }
    }
}
