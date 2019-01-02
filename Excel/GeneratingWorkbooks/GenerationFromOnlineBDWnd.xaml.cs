using DBManager.Global;
using DBManager.OnlineDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Interaction logic for GenerationFromOnlineBDWnd.xaml
    /// </summary>
    public partial class GenerationFromOnlineBDWnd : CNotifyPropertyChangedWnd
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

        /// <summary>
        /// Запись в выпадающем списке соревнований
        /// </summary>
        public class CompItem : INotifyPropertyChanged
        {
            #region Name
            private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.Name);
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


            #region StartDate
            private static readonly string StartDatePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.StartDate);
            private DateTime m_StartDate;

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
            private static readonly string EndDatePropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.EndDate);
            private DateTime? m_EndDate;

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


            #region ID
            private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.ID);
            private int m_ID = -1;

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


            #region Groups
            public ObservableCollection<GroupItem> Groups { get; private set; } = new ObservableCollection<GroupItem>(); 
            #endregion

            public CompItem()
            {
            }


            #region OnPropertyChanged and PropertyChanged event
            public event PropertyChangedEventHandler PropertyChanged;
            
            public virtual void OnPropertyChanged(string info)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
            #endregion
        }

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

            EndYears.Add((int)enEndYearSpecVals.AndYounger);
            EndYears.Add((int)enEndYearSpecVals.AndElder);
            for (int i = DateTime.Now.Year - 7; i > DateTime.Now.Year - 100; i--)
            {
                StartYears.Add(i);
                EndYears.Add(i);
            }

            // Заполняем выпадающие списки текущими значениями
            if (OnlineDBManager.Instance.IsConnectedToRemoteDB)
            {
                RemoteDBComps.Clear();

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

            foreach (var mainJudge in DBManagerApp
                                    .m_Entities
                                    .groups
                                    .Select(arg => arg.main_judge)
                                    .Where(arg => !string.IsNullOrEmpty(arg))
                                    .Distinct())
            {
                cmbMainJudge.Items.Add(mainJudge);
            }

            foreach (var mainSecretary in DBManagerApp
                                    .m_Entities
                                    .groups
                                    .Select(arg => arg.main_secretary)
                                    .Where(arg => !string.IsNullOrEmpty(arg))
                                    .Distinct())
            {
                cmbMainSecretary.Items.Add(mainSecretary);
            }

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

        private bool CheckSettings()
        {
            if (cmbComp.SelectedItem == null)
            {
                MessageBox.Show(this, Properties.Resources.resNoSelectedComp, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Directory.Exists(txtDestCompFolder.Text))
            {
                MessageBox.Show(this, Properties.Resources.resInvalidDestCompFolder, Title, MessageBoxButton.OK, MessageBoxImage.Error);
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
                ShowNewFolderButton = false
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
