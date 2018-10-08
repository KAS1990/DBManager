using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

			RefreshGroups();
			m_mainWnd.CurrentGroups.PropertyChanged += m_mainWnd_CurrentGroups_PropertyChanged;
			m_mainWnd.CurrentGroups.CollectionChanged += m_mainWnd_CurrentGroups_CollectionChanged;

			RefreshRounds();
			m_mainWnd.CurrentRounds.PropertyChanged += m_mainWnd_CurrentRounds_PropertyChanged;
			m_mainWnd.CurrentRounds.CollectionChanged += m_mainWnd_CurrentRounds_CollectionChanged;

			cmbHighlightTypes.ItemsSource = m_mainWnd.HighlightTypes;
			cmbHighlightTypes.SelectedValue = m_mainWnd.CurHighlightGradesType;
			
			OnPropertyChanged(MainWindow.ScannerStoppedPropertyName);
			OnPropertyChanged(MainWindow.RefreshEnabledPropertyName);
			OnPropertyChanged(MainWindow.SyncDBWithFilesEnabledPropertyName);
			OnPropertyChanged(MainWindow.DBToGridEnabledPropertyName);

			OnPropertyChanged(GroupSelectionEnabledPropertyName);
			OnPropertyChanged(RoundSelectionEnabledPropertyName);

			OnPropertyChanged(MainWindow.FTPEnabledPropertyName);

			OnPropertyChanged(MainWindow.SettingsEnabledPropertyName);
			OnPropertyChanged(MainWindow.LogWindowEnabledPropertyName);
			OnPropertyChanged(MainWindow.FalsestartRulesEnabledPropertyName);

			OnPropertyChanged(MainWindow.CalcGradesEnabledPropertyName);

			OnPropertyChanged(MainWindow.ExportToXlsEnabledPropertyName);
			
			SetTopMost();
		}

		#region btnAutoupdating

		public bool ScannerStopped => m_mainWnd.ScannerStopped;

		private void btnAutoupdating_Click(object sender, RoutedEventArgs e)
		{
			if (m_mainWnd.ScannerStopped)
				m_mainWnd.StartCmdExecuted(sender, e);
			else
				m_mainWnd.StopCmdExecuted(sender, e);
		}

		#endregion

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

		#region btnOpenWorkbook

		private void btnOpenWorkbook_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.OpenWorkbookCmdExecuted(sender, e);
		}

		#endregion

		public bool FTPEnabled => m_mainWnd?.FTPEnabled ?? false;

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

		private void RefreshGroups()
		{
			cmbGroups.SelectionChanged -= cmbGroups_SelectionChanged;

			cmbGroups.Items.Clear();

			foreach (var group in m_mainWnd.CurrentGroups)
			{
				
				var cmbi = GlobalDefines.AddItemToCmb<long>(cmbGroups, group.Value.Value.AgeGroup.FullGroupName, group.Key);
				
				if (m_mainWnd.CurrentGroups.SelectedKey == group.Key)
					cmbGroups.SelectedItem = cmbi;
			}

			if (m_mainWnd.CurrentGroups.Count == 0)
			{
				GlobalDefines.AddItemToCmb<long>(cmbGroups, Properties.Resources.resSelectGroup, -1);
				cmbGroups.SelectedIndex = 0;
			}

			OnPropertyChanged(GroupSelectionEnabledPropertyName);

			cmbGroups.SelectionChanged += cmbGroups_SelectionChanged;
		}

		private void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_mainWnd.CurrentGroups.PropertyChanged -= m_mainWnd_CurrentGroups_PropertyChanged;

			long selectedGroup = Convert.ToInt64((cmbGroups.SelectedItem as ComboBoxItem).Tag);
			if (m_mainWnd.CurrentGroups.ContainsKey(selectedGroup))
				m_mainWnd.CurrentGroups[selectedGroup].Command.DoExecute();

			m_mainWnd.CurrentGroups.PropertyChanged += m_mainWnd_CurrentGroups_PropertyChanged;

			RefreshRounds();
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

		void m_mainWnd_CurrentGroups_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			cmbGroups.SelectionChanged -= cmbGroups_SelectionChanged;

			bool found = false;

			foreach (ComboBoxItem cmbi in cmbGroups.Items)
			{
				if ((long)cmbi.Tag == m_mainWnd.CurrentGroups.SelectedKey)
				{
					cmbGroups.SelectedItem = cmbi;
					found = true;
					break;
				}
			}

			if (!found)
				cmbGroups.SelectedIndex = -1;

			cmbGroups.SelectionChanged += cmbGroups_SelectionChanged;

			RefreshRounds();
		}

		void m_mainWnd_CurrentGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshGroups();
		}

		#endregion

		#region cmbRounds

		private void RefreshRounds()
		{
			cmbRounds.SelectionChanged -= cmbRounds_SelectionChanged;

			cmbRounds.Items.Clear();

			foreach (var round in m_mainWnd.CurrentRounds)
			{
				var cmbi = GlobalDefines.AddItemToCmb<byte>(cmbRounds, round.Value.Value.Name, round.Key);
				
				if (m_mainWnd.CurrentRounds.SelectedKey == round.Key)
					cmbRounds.SelectedItem = cmbi;
			}

			if (m_mainWnd.CurrentRounds.Count == 0)
			{
				GlobalDefines.AddItemToCmb<byte>(cmbRounds, Properties.Resources.resSelectRound, 255);
				cmbRounds.SelectedIndex = 0;
			}

			OnPropertyChanged(RoundSelectionEnabledPropertyName);

			cmbRounds.SelectionChanged += cmbRounds_SelectionChanged;
		}

		private void cmbRounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_mainWnd.CurrentRounds.PropertyChanged -= m_mainWnd_CurrentRounds_PropertyChanged;

			byte selectedRound = Convert.ToByte((cmbRounds.SelectedItem as ComboBoxItem).Tag);
			if (m_mainWnd.CurrentRounds.ContainsKey(selectedRound))
				m_mainWnd.CurrentRounds[selectedRound].Command.DoExecute();

			m_mainWnd.CurrentRounds.PropertyChanged += m_mainWnd_CurrentRounds_PropertyChanged;
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

		void m_mainWnd_CurrentRounds_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			cmbRounds.SelectionChanged -= cmbRounds_SelectionChanged;

			bool found = false;

			foreach (ComboBoxItem cmbi in cmbRounds.Items)
			{
				if ((byte)cmbi.Tag == m_mainWnd.CurrentRounds.SelectedKey)
				{
					cmbRounds.SelectedItem = cmbi;
					found = true;
					break;
				}
			}

			if (!found)
				cmbRounds.SelectedIndex = -1;

			cmbRounds.SelectionChanged += cmbRounds_SelectionChanged;
		}

		void m_mainWnd_CurrentRounds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshRounds();
		}

		#endregion

		#region btnSettings

		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.SettingsCmdExecuted(sender, e);
		}

		public bool SettingsEnabled => m_mainWnd?.SettingsEnabled ?? false;

		#endregion

		#region btnLogWindow

		private void btnLogWindow_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.LogWindowCmdExecuted(sender, e);
		}

		public bool LogWindowEnabled => m_mainWnd?.LogWindowEnabled ?? false;

		#endregion

		#region btnFalsestartRules

		private void btnFalsestartRules_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.FalsestartRulesCmdExecuted(sender, e);
		}

		public bool FalsestartRulesEnabled => m_mainWnd?.FalsestartRulesEnabled ?? false;

		#endregion

		public bool CalcGradesEnabled => m_mainWnd?.CalcGradesEnabled ?? false;

		#region btnCalcGrades

		private void btnCalcGrades_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.CalcGradesCmdExecuted(sender, e);
		}

		#endregion

		#region cmbHighlightTypes

		private void cmbHighlightTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			m_mainWnd.HighlightTypes[cmbHighlightTypes.SelectedIndex].Command.DoExecute();
		}

		#endregion

		#region btnExportToXls

		private void btnExportToXls_Click(object sender, RoutedEventArgs e)
		{
			m_mainWnd.ExportToXlsCmdExecuted(sender, e);
		}

		public bool ExportToXlsEnabled => m_mainWnd?.ExportToXlsEnabled ?? false;

		#endregion

		void m_mainWnd_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{	// Транслируем изменение свойства в эту форму 
			OnPropertyChanged(e.PropertyName);

			if (e.PropertyName == MainWindow.CurHighlightGradesTypePropertyName)
			{
				cmbHighlightTypes.SelectionChanged -= cmbHighlightTypes_SelectionChanged;

				cmbHighlightTypes.SelectedValue = m_mainWnd.CurHighlightGradesType;

				cmbHighlightTypes.SelectionChanged += cmbHighlightTypes_SelectionChanged;
			}
		}

		#region TopMost

		private void SetTopMost()
		{
			Topmost = chkTopMost.IsChecked.Value;
		}

		private void chkTopMost_Click(object sender, RoutedEventArgs e)
		{
			SetTopMost();
		}


		#endregion

		
	}
}
