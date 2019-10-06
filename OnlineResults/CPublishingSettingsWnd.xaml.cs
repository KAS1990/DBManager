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
using DBManager.Scanning.XMLDataClasses;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DBManager.SettingsWriter;

namespace DBManager.OnlineResults
{
    /// <summary>
    /// Interaction logic for CPublishingSettingsWnd.xaml
    /// </summary>
    public partial class CPublishingSettingsWnd : CNotifyPropertyChangedWnd
    {
        public class CPublishingGroupItem : INotifyPropertyChanged
        {
            #region GroupId
            private long m_GroupId = -1;
            /// <summary>
            ///
            /// </summary>
            public long GroupId
            {
                get { return m_GroupId; }
                private set
                {
                    if (m_GroupId != value)
                    {
                        m_GroupId = value;
                        IsSelected = false;
                        GroupName = null;
                    }
                }
            }
            #endregion


            #region IsSelected
            public static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CPublishingGroupItem>(m => m.IsSelected);
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


            #region GroupName
            private static readonly string GroupNamePropertyName = GlobalDefines.GetPropertyName<CPublishingGroupItem>(m => m.GroupName);
            private string m_GroupName = null;
            /// <summary>
            /// Название группы
            /// </summary>
            public string GroupName
            {
                get { return m_GroupName; }
                set
                {
                    if (m_GroupName != value)
                    {
                        m_GroupName = value;
                        OnPropertyChanged(GroupNamePropertyName);
                    }
                }
            }
            #endregion
                                    

            #region Constructors
            public CPublishingGroupItem()
            {
            }


            public CPublishingGroupItem(long groupId)
            {
                GroupId = groupId;
            }
            #endregion


            #region OnPropertyChanged and PropertyChanged event
            public event PropertyChangedEventHandler PropertyChanged;


            public virtual void OnPropertyChanged(string info)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
            #endregion
        }


        ObservableCollection<CPublishingGroupItem> m_Groups = new ObservableCollection<CPublishingGroupItem>();
        public ObservableCollection<CPublishingGroupItem> Groups
        {
            get { return m_Groups; }
        }


        public List<CPublishingGroupItem> SelectedGroups
        {
            get { return Groups.Where(arg => arg.IsSelected).ToList(); }
        }

                
        readonly long m_CompId = -1;


        #region Конструкторы
        public CPublishingSettingsWnd()
        {
            InitializeComponent();
        }


        public CPublishingSettingsWnd(long CompId, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups)
        {
            InitializeComponent();

            m_CompId = CompId;

            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                foreach (KeyValuePair<long, CKeyValuePairEx<long, CCompSettings>> item in CompGroups)
                {
                    CPublishingGroupItem GroupItem = new CPublishingGroupItem(item.Key)
                        {
                            GroupName = item.Value.Value.AgeGroup.FullGroupName,
                        };
                    CCompSpecificSets CompSets;
                    CPublishedGroupItemInSets SettingsPublishedGroupItem;
                    if (DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.TryGetValue(CompId, out CompSets) &&
                        CompSets.dictGroupsForAutopublish.TryGetValue(item.Key, out SettingsPublishedGroupItem))
                    {
                        GroupItem.IsSelected = SettingsPublishedGroupItem.IsSelected;
                    }
                    GroupItem.PropertyChanged += GroupItem_PropertyChanged;
                    Groups.Add(GroupItem);
                }
            }
        }
        #endregion


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                CCompSpecificSets CompSets = null;
                if (!DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.TryGetValue(m_CompId, out CompSets))
                {
                    CompSets = new CCompSpecificSets()
                    {
                        CompId = m_CompId,
                    };
                    DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.Add(m_CompId, CompSets);
                }

                // Проще каждый раз заново формировать этот словарь, т.к. он всегда мелкий
                CompSets.dictGroupsForAutopublish = new SerializableDictionary<long, CPublishedGroupItemInSets>();

                foreach (CPublishingGroupItem item in Groups)
                {
                    CompSets.dictGroupsForAutopublish.Add(item.GroupId,
                                                        new CPublishedGroupItemInSets()
                                                            {
                                                                GroupId = item.GroupId,
                                                                IsSelected = item.IsSelected
                                                            });
                }
                DBManagerApp.m_AppSettings.m_Settings.dictCompSettings[m_CompId] = CompSets;

                DBManagerApp.m_AppSettings.Write();
            }

            DialogResult = true;
        }


        private void CPublishingSettingsWnd_Loaded(object sender, RoutedEventArgs e)
        {
            lstvGroups.Width = lstvGroups.ActualWidth;
        }

        
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked.HasValue)
            {
                foreach (CPublishingGroupItem item in Groups)
                {
                    item.PropertyChanged -= GroupItem_PropertyChanged;
                    item.IsSelected = chkSelectAll.IsChecked.Value;
                    item.PropertyChanged += GroupItem_PropertyChanged;
                }
            }
        }


        private void GroupItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CPublishingGroupItem.IsSelectedPropertyName)
            {
                if (Groups.All(arg => arg.IsSelected))
                {	// Все элементы выбраны
                    chkSelectAll.IsChecked = true;
                }
                else if (Groups.All(arg => !arg.IsSelected))
                {	// Все элементы не выбраны
                    chkSelectAll.IsChecked = false;
                }
                else
                {	// Что-то выбрано, а что-то нет
                    chkSelectAll.IsChecked = null;
                }
            }
        }
    }
}
