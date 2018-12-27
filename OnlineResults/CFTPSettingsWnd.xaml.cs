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

namespace DBManager.FTP
{
	/// <summary>
	/// Interaction logic for CFTPSettingsWnd.xaml
	/// </summary>
	public partial class CFTPSettingsWnd : CNotifyPropertyChangedWnd
	{
		public class CFTPGroupItem : INotifyPropertyChanged
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
						WbkPath = GroupName = null;
					}
				}
			}
			#endregion


			#region IsSelected
			public static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CFTPGroupItem>(m => m.IsSelected);
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
			private static readonly string GroupNamePropertyName = GlobalDefines.GetPropertyName<CFTPGroupItem>(m => m.GroupName);
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


			#region WbkFullPath
			private static readonly string WbkFullPathPropertyName = GlobalDefines.GetPropertyName<CFTPGroupItem>(m => m.WbkPath);
			private string m_WbkPath = null;
			/// <summary>
			/// Путь к книге на сервере
			/// </summary>
			public string WbkPath
			{
				get { return m_WbkPath; }
				set
				{
					if (m_WbkPath != value)
					{
						m_WbkPath = value;
						OnPropertyChanged(WbkFullPathPropertyName);
					}
				}
			}
			#endregion


			#region
			public CFTPGroupItem()
			{
			}


			public CFTPGroupItem(long groupId)
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


		ObservableCollection<CFTPGroupItem> m_Groups = new ObservableCollection<CFTPGroupItem>();
		public ObservableCollection<CFTPGroupItem> Groups
		{
			get { return m_Groups; }
		}


		public List<CFTPGroupItem> SelectedGroups
		{
			get { return Groups.Where(arg => arg.IsSelected).ToList(); }
		}


		#region Настройки соединения по FTP
		#region FTPHost
		private static readonly string FTPHostPropertyName = GlobalDefines.GetPropertyName<CFTPSettingsWnd>(m => m.FTPHost);
		public string FTPHost
		{
			get
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					return DBManagerApp.m_AppSettings.m_Settings.FTPHost;
			}
		}
		#endregion
		

		#region FTPPort
		private static readonly string FTPPortPropertyName = GlobalDefines.GetPropertyName<CFTPSettingsWnd>(m => m.FTPPort);
		public int FTPPort
		{
			get
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					return DBManagerApp.m_AppSettings.m_Settings.FTPPort;
			}
		}
		#endregion

		
		#region FTPUsername
		private static readonly string FTPUsernamePropertyName = GlobalDefines.GetPropertyName<CFTPSettingsWnd>(m => m.FTPUsername);
		public string FTPUsername
		{
			get
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					return DBManagerApp.m_AppSettings.m_Settings.FTPUsername;
			}
		}
		#endregion

		
		#region FTPPassword
		private static readonly string FTPPasswordPropertyName = GlobalDefines.GetPropertyName<CFTPSettingsWnd>(m => m.FTPPassword);

		public string FTPPassword
		{
			get
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					return DBManagerApp.m_AppSettings.m_Settings.FTPPassword;
			}
		}
		#endregion

		
		#region FTPTemplateFullPath
		private static readonly string FTPTemplateFullPathPropertyName = GlobalDefines.GetPropertyName<CFTPSettingsWnd>(m => m.FTPTemplateFullPath);

		public string FTPTemplateFullPath
		{
			get
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					return GlobalDefines.STD_APP_CONFIGS_DIR + "\\" + DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.FTPTemplatesWbkName;
			}
		}
		#endregion
		

		void RefreshFTPConnectionProps()
		{
			OnPropertyChanged(FTPHostPropertyName);
			OnPropertyChanged(FTPPortPropertyName);
			OnPropertyChanged(FTPUsernamePropertyName);
			OnPropertyChanged(FTPPasswordPropertyName);
			OnPropertyChanged(FTPTemplateFullPathPropertyName);
		}
		#endregion


		readonly long m_CompId = -1;


		#region Конструкторы
		public CFTPSettingsWnd()
		{
			InitializeComponent();

			RefreshFTPConnectionProps();
		}


		public CFTPSettingsWnd(long CompId, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups)
		{
			InitializeComponent();

			RefreshFTPConnectionProps();

			m_CompId = CompId;

			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				foreach (KeyValuePair<long, CKeyValuePairEx<long, CCompSettings>> item in CompGroups)
				{
					CFTPGroupItem GroupItem = new CFTPGroupItem(item.Key)
						{
							GroupName = item.Value.Value.AgeGroup.FullGroupName,
						};
					CCompSpecificSets CompSets;
					CFTPGroupItemInSets SettingsFTPGroupItem;
					if (DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.TryGetValue(CompId, out CompSets) &&
						CompSets.dictGroupsForAutosendToFTP.TryGetValue(item.Key, out SettingsFTPGroupItem))
					{
						GroupItem.WbkPath = SettingsFTPGroupItem.FTPWbkPath;
						GroupItem.IsSelected = SettingsFTPGroupItem.IsSelected;
					}
					GroupItem.PropertyChanged += GroupItem_PropertyChanged;
					Groups.Add(GroupItem);
				}
			}
		}
		#endregion


		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			if (Groups.Any((arg) =>
				{
					return arg.IsSelected &&
							(string.IsNullOrWhiteSpace(arg.WbkPath) ||
								System.IO.Path.GetExtension(arg.WbkPath) != GlobalDefines.XLS_EXTENSION);
				}))
			{	// Ошибки в задании путей к файлам на сервере
				MessageBox.Show(this, Properties.Resources.resInvalidFTPPaths, Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if ((from gr in Groups
				 group gr by gr.WbkPath into WbkPaths
				 where WbkPaths.Count() > 1
				 select WbkPaths.Count()).Count() > 0)
			{	// Есть повторяющиеся пути к книгам
				MessageBox.Show(this, Properties.Resources.resDuplicateFTPPaths, Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			
			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
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
				CompSets.dictGroupsForAutosendToFTP = new SerializableDictionary<long, CFTPGroupItemInSets>();

				foreach (CFTPGroupItem item in Groups)
				{
					CompSets.dictGroupsForAutosendToFTP.Add(item.GroupId,
														new CFTPGroupItemInSets()
															{
																GroupId = item.GroupId,
																FTPWbkPath = item.WbkPath,
																IsSelected = item.IsSelected
															});
				}
				DBManagerApp.m_AppSettings.m_Settings.dictCompSettings[m_CompId] = CompSets;

				DBManagerApp.m_AppSettings.Write();
			}

			DialogResult = true;
		}


		private void CFTPSettingsWnd_Loaded(object sender, RoutedEventArgs e)
		{
			lstvGroups.Width = lstvGroups.ActualWidth;
		}

		
		private void chkSelectAll_Click(object sender, RoutedEventArgs e)
		{
			if (chkSelectAll.IsChecked.HasValue)
			{
				foreach (CFTPGroupItem item in Groups)
				{
					item.PropertyChanged -= GroupItem_PropertyChanged;
					item.IsSelected = chkSelectAll.IsChecked.Value;
					item.PropertyChanged += GroupItem_PropertyChanged;
				}
			}
		}


		private void GroupItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CFTPGroupItem.IsSelectedPropertyName)
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
