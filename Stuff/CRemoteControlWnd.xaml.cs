using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace DBManager.Stuff
{
	/// <summary>
	/// Interaction logic for RemoteControlWnd.xaml
	/// </summary>
	public partial class CRemoteControlWnd : CNotifyPropertyChangedWnd
	{
		readonly MainWindow m_mainWnd = null;

		#region Конструкторы

		public CRemoteControlWnd()
		{
			InitializeComponent();
		}

		public CRemoteControlWnd(MainWindow mainWnd)
		{
			InitializeComponent();

			m_mainWnd = mainWnd;
			m_mainWnd.PropertyChanged += m_mainWnd_PropertyChanged;

			InitControls();
		}

		#endregion

		private void InitControls()
		{
			chkAutoscrollEnabled.IsChecked = m_mainWnd.rchkAutoscrollEnabled.IsChecked;
			m_mainWnd.rchkAutoscrollEnabled.Checked += m_mainWnd_rchkAutoscrollEnabled_CheckedOrUnchecked;
			m_mainWnd.rchkAutoscrollEnabled.Unchecked += m_mainWnd_rchkAutoscrollEnabled_CheckedOrUnchecked;

			chkShowGroupHead.IsChecked = m_mainWnd.rchkShowGroupHead.IsChecked;
			m_mainWnd.rchkShowGroupHead.Checked += m_mainWnd_rchkShowGroupHead_CheckedOrUnchecked;
			m_mainWnd.rchkShowGroupHead.Unchecked += m_mainWnd_rchkShowGroupHead_CheckedOrUnchecked;

			chkAutoSendToFTP.IsChecked = m_mainWnd.rchkAutoSendToFTP.IsChecked;
			m_mainWnd.rchkAutoSendToFTP.Checked += m_mainWnd_rchkAutoSendToFTP_CheckedOrUnchecked;
			m_mainWnd.rchkAutoSendToFTP.Unchecked += m_mainWnd_rchkAutoSendToFTP_CheckedOrUnchecked;

			foreach (var group in m_mainWnd.CurrentGroups)
			{
				var cmbi = new ComboBoxItem()
				{
					Content = group.Value.Value.AgeGroup.FullGroupName,
					Tag = group.Key
				};
				cmbGroups.Items.Add(cmbi);

				if (m_mainWnd.CurrentGroups.SelectedKey == group.Key)
					cmbGroups.SelectedItem = cmbi;
			}

			foreach (var round in m_mainWnd.CurrentRounds)
			{
				var cmbi = new ComboBoxItem()
				{
					Content = round.Value.Value.Name,
					Tag = round.Key
				};
				cmbRounds.Items.Add(cmbi);

				if (m_mainWnd.CurrentGroups.SelectedKey == round.Key)
					cmbRounds.SelectedItem = cmbi;
			}

			OnPropertyChanged(MainWindow.RefreshEnabledPropertyName);
			OnPropertyChanged(MainWindow.SyncDBWithFilesEnabledPropertyName);
			OnPropertyChanged(MainWindow.DBToGridEnabledPropertyName);
			OnPropertyChanged(GroupSelectionEnabledPropertyName);
			OnPropertyChanged(RoundSelectionEnabledPropertyName);
		}

		private void btnAutoupdating_Click(object sender, RoutedEventArgs e)
		{
			
		}

		#region btnRefresh

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.RefreshCmdExecuted(sender, e);
		}

		public bool RefreshEnabled => m_mainWnd?.RefreshEnabled ?? false;

		#endregion

		#region btnSyncDBWithFiles

		private void btnSyncDBWithFiles_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.SyncDBWithFilesCmdExecuted(sender, e);
		}

		public bool SyncDBWithFilesEnabled => m_mainWnd?.SyncDBWithFilesEnabled ?? false;

		#endregion

		#region btnDBToGrid

		private void btnDBToGrid_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.DBToGridCmdExecuted(sender, e);
		}

		public bool DBToGridEnabled => m_mainWnd?.DBToGridEnabled ?? false;

		#endregion

		#region btnFTPSettings

		private void btnFTPSettings_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.FTPSettingsCmdExecuted(sender, e);
		}

		#endregion

		#region btnSendToFTP

		private void btnSendToFTP_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.SendToFTPCmdExecuted(sender, e);
		}

		#endregion

		#region btnFTPLog

		private void btnFTPLog_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.FTPLogCmdExecuted(sender, e);
		}

		#endregion

		#region chkAutoSendToFTP

		private void chkAutoSendToFTP_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.rchkAutoSendToFTP.IsChecked = chkAutoSendToFTP.IsChecked;
			m_mainWnd.rchkAutoSendToFTP_Click(sender, e);
		}

		private void m_mainWnd_rchkAutoSendToFTP_CheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			chkAutoSendToFTP.IsChecked = m_mainWnd.rchkAutoSendToFTP.IsChecked;
		}

		#endregion

		#region chkAutoscrollEnabled

		private void chkAutoscrollEnabled_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.rchkAutoscrollEnabled.IsChecked = chkAutoscrollEnabled.IsChecked;
		}

		private void m_mainWnd_rchkAutoscrollEnabled_CheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			chkAutoscrollEnabled.IsChecked = m_mainWnd.rchkAutoscrollEnabled.IsChecked;
		}

		#endregion

		#region chkShowGroupHead

		private void chkShowGroupHead_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.rchkShowGroupHead.IsChecked = chkShowGroupHead.IsChecked;
			m_mainWnd.rchkShowGroupHead_Click(sender, e);
		}

		private void m_mainWnd_rchkShowGroupHead_CheckedOrUnchecked(object sender, RoutedEventArgs e)
		{
			chkShowGroupHead.IsChecked = m_mainWnd.rchkShowGroupHead.IsChecked;
		}

		#endregion

		#region cmbGroups

		private void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_mainWnd.CurrentGroups.SelectedKey = Convert.ToByte((cmbGroups.SelectedItem as ComboBoxItem).Tag);
			m_mainWnd.CurrentGroups.SelectedItem.Command.DoExecute();
		}

		private static readonly string GroupSelectionEnabledPropertyName = GlobalDefines.GetPropertyName<CRemoteControlWnd>(m => m.GroupSelectionEnabled);
		public bool GroupSelectionEnabled
		{
			get
			{
				if (m_mainWnd == null)
					return false;

				var conv = new CollectionsCountToBoolMarkupConverter();
				return (bool)conv.Convert(m_mainWnd.CurrentGroups.Count, typeof(bool), null, CultureInfo.CurrentCulture);
			}
		}

		#endregion

		#region cmbRounds

		private void cmbRounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_mainWnd.CurrentRounds.SelectedKey = Convert.ToByte((cmbRounds.SelectedItem as ComboBoxItem).Tag);
			m_mainWnd.CurrentRounds.SelectedItem.Command.DoExecute();
		}

		private static readonly string RoundSelectionEnabledPropertyName = GlobalDefines.GetPropertyName<CRemoteControlWnd>(m => m.RoundSelectionEnabled);
		public bool RoundSelectionEnabled
		{
			get
			{
				if (m_mainWnd == null)
					return false;

				var conv = new CollectionsCountToBoolMarkupConverter();
				return (bool)conv.Convert(m_mainWnd.CurrentRounds.Count, typeof(bool), null, CultureInfo.CurrentCulture);
			}
		}

		#endregion

		void m_mainWnd_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{	// Транслируем изменение свойства в эту форму 
			OnPropertyChanged(e.PropertyName);
		}
	}
}
