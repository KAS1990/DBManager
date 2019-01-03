using DBManager.Global;
using DBManager.OnlineDB;
using DBManager.Stuff;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<GenerationFromOnlineBDWnd>(m => m.SecectedCompGroups);
        private ObservableCollection<GroupItem> m_SecectedCompGroups = new ObservableCollection<GroupItem>();
        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<GroupItem> SecectedCompGroups
        {
            get { return m_SecectedCompGroups; }
        }
        #endregion

        public List<int> StartYears { get; set; } = new List<int>();
        public List<int> EndYears { get; set; } = new List<int>();

        public GenerationFromOnlineBDWnd()
        {
            InitializeComponent();

            InitWndControls();
        }

        private void InitWndControls()
        {
            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;

            if (CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        Title,
                                        Properties.Resources.resFillingGenerationFromOnlineBDWnd);
            }

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
                    var item = new CompItem()
                    {
                        ID = comp.id,
                        Name = comp.name,
                        StartDate = comp.date1,
                        EndDate = comp.date2
                    };

                    foreach (var group in speedGroups.Where(gr => gr.events.Any(ev => ev.id == comp.id)))
                    {
                        if (DBManagerApp.m_AppSettings.m_Settings.AvailableGroupNames.Any(arg => string.Compare(arg.GroupName, group.name, true) == 0))
                        {
                            item.Groups.Add(new GroupItem()
                            {
                                ID = group.id,
                                Name = group.name,
                                Sex = (enOnlineSex)(group.sex ? 1 : 0),
                                StartYear = DateTime.Now.Year - group.year2,
                                EndYear = group.year1.HasValue ? DateTime.Now.Year - group.year1 : null,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate
                            });
                        }
                    }

                    RemoteDBComps.Add(item);
                }
                cmbComp.SelectedIndex = 0;
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

            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();
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
                 group gr by gr.Name into groupNames
                 where groupNames.Count() > 1
                 select groupNames.Key)
                .Any())
            {
                MessageBox.Show(this, Properties.Resources.resDuplicateGroupNames, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (SecectedCompGroups.Any(arg =>
                    arg.EndYear.HasValue
                        && arg.EndYear != (int)enEndYearSpecVals.AndElder
                        && arg.EndYear != (int)enEndYearSpecVals.AndYounger
                        && arg.StartYear >= arg.EndYear))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidGroupYears, Title, MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void chkRow6_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            cmbRow6.IsEnabled = chkRow6.IsChecked ?? false;
        }

        private void btnDestFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = true
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDestCompFolder.Text = dlg.SelectedPath;
            }
        }

        private void GenerationFromOnlineBDWnd_Loaded(object sender, RoutedEventArgs e)
        {
            if (!m_DBManager.IsConnectedToRemoteDB)
            {
                string msg = string.Format(DBManager.Properties.Resources.resrmtCantConnectToRemoteDB, OnlineDBManager.Instance.ConnectionString);
                MessageBox.Show(msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            txtDestCompFolder.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            txtDestCompFolder.Width = txtDestCompFolder.DesiredSize.Width;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckSettings())
                return;
        }

        private void cmbComp_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = (TextBox)cmbComp.Template.FindName("PART_EditableTextBox", cmbComp);
            if (txt != null)
            {
                txt.TextWrapping = TextWrapping.Wrap;
            }
        }

        private void cmbComp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbComp.SelectedItem != null)
            {
                var compItem = (cmbComp.SelectedItem as CompItem);
                dtpckrStartDate.Value = compItem.StartDate;
                dtpckrEndDate.Value = compItem.EndDate;

                SecectedCompGroups.Clear();
                foreach (var group in compItem.Groups)
                {   // Создаём копию, чтобы не затирать исходные данные
                    SecectedCompGroups.Add(new GroupItem(group));
                }
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

            int year = value is int? ? (value as int?).Value : (int)value;
            switch (year)
            {
                case (int)enEndYearSpecVals.AndElder:
                    return Properties.Resources.resAndElder;

                case (int)enEndYearSpecVals.AndYounger:
                    return Properties.Resources.resAndYounger;

                default:
                    return year.ToString();
            }
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
