﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using DBManager.Global;
using Microsoft.Windows.Controls.Ribbon;
using System.Reflection;
using DBManager.SettingWnds;
using DBManager.Scanning;
using DBManager.Scanning.XMLDataClasses;
using System.IO;
using DBManager.SettingsWriter;
using System.Globalization;
using DBManager.Scanning.DBAdditionalDataClasses;
using System.Windows.Threading;
using System.Windows.Interop;
using DBManager.TrayNotification;
using DBManager.Global.Converters;
using DBManager.RoundMembers.Converters;
using DBManager.AttachedProperties;
using System.Windows.Controls.Primitives;
using DBManager.RoundResultsControl.FilterControl;
using WPFLocalization;
using DBManager.Stuff;
using DBManager.Excel.Exporting;
using DBManager.FTP;
using DBManager.FTP.SheetGenerators;
using DBManager.RightPanels;

namespace DBManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : CNotifyPropertyChangedWnd
	{
		const double SCROLL_VIEWER_SCROLL_PART = 0.9;

		bool m_ShowMsgBeforeClose = true;

		CDirScanner m_DirScanner = null;
		ResourceDictionary m_RightPanelTemplates = new ResourceDictionary()
		{
			Source = new Uri("RightPanels\\RightPanelTemplates.xaml", UriKind.RelativeOrAbsolute)
		};

		bool m_RestartingThreads = false;
				
		#region CurrentGroups
		private ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> m_CurrentGroups = new ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>>();
		/// <summary>
		/// Словарь, содержащий все группы
		/// </summary>
		public ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CurrentGroups
		{
			get { return m_CurrentGroups; }
		}
		#endregion


		#region CurrentRounds
		private ObservableDictionary<byte, CKeyValuePairEx<byte, CRoundAndDate>> m_CurrentRounds = new ObservableDictionary<byte, CKeyValuePairEx<byte, CRoundAndDate>>();
		/// <summary>
		/// Словарь, содержащий все раунды текущей группы
		/// </summary>
		public ObservableDictionary<byte, CKeyValuePairEx<byte, CRoundAndDate>> CurrentRounds
		{
			get { return m_CurrentRounds; }
		}
		#endregion


		#region HighlightTypes
		private ObservableCollectionEx<CKeyValuePairEx<enHighlightGradesType, string>> m_HighlightTypes = new ObservableCollectionEx<CKeyValuePairEx<enHighlightGradesType, string>>();
		/// <summary>
		/// Словарь, содержащий все типы подсветок разрядов в итоговом протоколе
		/// </summary>
		public ObservableCollectionEx<CKeyValuePairEx<enHighlightGradesType, string>> HighlightTypes
		{
			get { return m_HighlightTypes; }
		}
		#endregion


		#region СurrentRoundMembers
		/// <summary>
		/// Результаты запроса на получения списка участников текущего раунда
		/// </summary>
		IEnumerable<CDBAdditionalClassBase> m_CurrentRoundMembers = null;
		/// <summary>
		/// Source для vsrcCurrentRoundMembers
		/// </summary>
		ObservableCollectionEx<CDBAdditionalClassBase> collectionCurrentRoundMembers { get; set; }

		/// <summary>
		/// Source для vsrcCurrentRoundMembers2
		/// </summary>
		ObservableCollectionEx<CDBAdditionalClassBase> collectionCurrentRoundMembers2 { get; set; }
		#endregion
						
		
		#region SecondColName
		private static readonly string SecondColNamePropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.SecondColName);

		private string m_SecondColName = null;

		public string SecondColName
		{
			get { return m_SecondColName; }
			set
			{
				if (m_SecondColName != value)
				{
					m_SecondColName = value;
					OnPropertyChanged(SecondColNamePropertyName);
				}
			}
		}
		#endregion


		#region QualifFinished
		private static readonly string QualifFinishedPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.QualifFinished);

		private bool m_QualifFinished = false;

		public bool QualifFinished
		{
			get { return m_QualifFinished; }
			set
			{
				if (m_QualifFinished != value)
				{
					m_QualifFinished = value;
					OnPropertyChanged(SecondColNamePropertyName);
				}
			}
		}
		#endregion


		#region MembersFromQualif
		private static readonly string MembersFromQualifPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.MembersFromQualif);

		private int m_MembersFromQualif = 0;

		public int MembersFromQualif
		{
			get { return m_MembersFromQualif; }
			set
			{
				if (m_MembersFromQualif != value)
				{
					m_MembersFromQualif = value;
					OnPropertyChanged(MembersFromQualifPropertyName);
				}
			}
		}
		#endregion

		
		#region CurHighlightGradesType
		private static readonly string CurHighlightGradesTypePropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.CurHighlightGradesType);

		private enHighlightGradesType m_CurHighlightGradesType = enHighlightGradesType.None;

		public enHighlightGradesType CurHighlightGradesType
		{
			get { return m_CurHighlightGradesType; }
			set
			{
				if (m_CurHighlightGradesType != value)
				{
					m_CurHighlightGradesType = value;
					OnPropertyChanged(CurHighlightGradesTypePropertyName);
				}
			}
		}
		#endregion
				

		

		public CFontStyleSettings PlainResultsFontStyle
		{
			get { return RightPanel.PlainResultsFontStyle; }
		}
		public CFontStyleSettings InvitedToStartFontStyle
		{
			get { return RightPanel.InvitedToStartFontStyle; }
		}
		public CFontStyleSettings JustRecievedResultFontStyle
		{
			get { return RightPanel.JustRecievedResultFontStyle; }
		}
		public CFontStyleSettings NextRoundMembersCountFontStyle
		{
			get { return RightPanel.NextRoundMembersCountFontStyle; }
		}
		public CFontStyleSettings PreparingFontStyle
		{
			get { return RightPanel.PreparingFontStyle; }
		}
		public CFontStyleSettings StayOnStartFontStyle
		{
			get { return RightPanel.StayOnStartFontStyle; }
		}


		private ScrollViewer m_svwrDataGrid = null;
		private ScrollViewer m_svwrDataGrid2 = null;


		private CollectionViewSource vsrcCurrentRoundMembers
		{
			get { return Resources["vsrcCurrentRoundMembers"] as CollectionViewSource; }
		}


		private CollectionViewSource vsrcCurrentRoundMembers2
		{
			get { return Resources["vsrcCurrentRoundMembers2"] as CollectionViewSource; }
		}
		

		private bool IsTotal
		{
			get { return (enRounds)CurrentRounds.SelectedKey == enRounds.Total; }
		}


		/// <summary>
		/// Смещения, на которые нужно выполнять прокрутку
		/// </summary>
		private PushPullList<double> m_lstScrollingOffsets = new PushPullList<double>();

		/// <summary>
		/// Таймер, который выполняет автоматическую прокрутку списка
		/// </summary>
		private DispatcherTimer m_tmrAutoscroll = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 5) };


		int m_MembersInLeftGrid = -1;


		List<string> m_NamesToUnregister = new List<string>();

		/// <summary>
		/// Все активные сейчас фильтры
		/// </summary>
		Dictionary<enFilterTarget, List<FilterPredicate>> m_dictFilters = new Dictionary<enFilterTarget, List<FilterPredicate>>();
				

		/// <summary>
		/// Результаты фильтрации.
		/// Они не используются для вывода данных на экран
		/// </summary>
		List<CDBAdditionalClassBase> m_lstFilteredMembers = null;


		CLogWnd m_wndLog = null;


		CFTPExporter m_FTPExporter = new CFTPExporter();


		#region hsActiveFilters
		private static readonly string ActiveFiltersPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.ActiveFilters);

		private List<enFilterTarget> m_ActiveFilters = new List<enFilterTarget>();
		/// <summary>
		/// Все активные фильтры
		/// </summary>
		public List<enFilterTarget> ActiveFilters
		{
			get { return m_ActiveFilters; }
		}
		#endregion


		#region Типо команды

		/// <summary>
		/// Сообщаем интерфейсу о том, что значения свойств, отвечающих за доступность "типо команд", изменились.
		/// Этот метод заменяет CommandManager.InvalidateRequerySuggested()
		/// </summary>
		public void RefreshCommandEnable()
		{
			OnPropertyChanged(SettingsEnabledPropertyName);
			OnPropertyChanged(LogWindowEnabledPropertyName);
			OnPropertyChanged(RefreshEnabledPropertyName);
			OnPropertyChanged(AutoupdatingAvailablePropertyName);
			OnPropertyChanged(SyncDBWithFilesEnabledPropertyName);
			OnPropertyChanged(DBToGridEnabledPropertyName);
			OnPropertyChanged(ExportToXlsEnabledPropertyName);
			OnPropertyChanged(FTPEnabledPropertyName);
			OnPropertyChanged(ExportingToFTPNowPropertyName);
			OnPropertyChanged(CalcGradesEnabledPropertyName);
			OnPropertyChanged(CurHighlightGradesTypePropertyName);
						
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>
		/// Открытие настроек прибора
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SettingsCmdExecuted(object sender, RoutedEventArgs e)
		{
			CSettingsWnd wnd = new CSettingsWnd()
			{
				Owner = this,
				AlwaysAccountChanges = true
			};

			try
			{
				bool? res = wnd.ShowDialog();
				if (res.HasValue && res.Value)
				{
					m_RestartingThreads = true;
					RefreshCommandEnable();

					OnFontStyliesChanged();

					AutoResetEvent hFinishedSearchEvent = null;
					Thread th = null;

					if (CheckAccess())
					{
						CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
												out th,
												Title,
												string.Format(Properties.Resources.resfmtStoppingDirScanningThread, DBManagerApp.m_AppSettings.m_Settings.CompDir));
					}

					SetDesc(null); // Удаляем данные из таблицы
					CDirScanner.CSyncParam SyncParam = new CDirScanner.CSyncParam(DBManagerApp.m_AppSettings.m_Settings.CompDir, new List<CFileScannerSettings>());
					// Ищем все xml-файлы в папке DBManagerApp.m_AppSettings.m_Settings.CompDir
					string[] AllXMLFullFilePaths = Directory.GetFiles(SyncParam.m_Dir, "*.xml");
					foreach (string fullFilePath in AllXMLFullFilePaths)
					{
						SyncParam.m_lstFileScannerSettings.Add(new CFileScannerSettings()
							{
								FullFilePath = fullFilePath,
								GroupId = -1
							});
					}
					m_DirScanner.Restart(SyncParam.m_Dir, SyncParam);
					// Выводим информацию на форму
					DBToGrid();
					SyncStartStopBtnWithThState();
					
					m_RestartingThreads = false;
					RefreshCommandEnable();

					if (CurrentRounds.SelectedItem != null)
					{	// Применяем новые цвета
						CurrentRounds.SelectedItem.Command.DoExecute();
					}

					if (hFinishedSearchEvent != null)
						hFinishedSearchEvent.Set();
				}				
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}
		}

		private static readonly string SettingsEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.SettingsEnabled);
		public bool SettingsEnabled
		{
			get
			{
				return !m_RestartingThreads;
			}
		}


		/// <summary>
		/// Открытие окна лога
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogWindowCmdExecuted(object sender, RoutedEventArgs e)
		{
			m_wndLog = new CLogWnd()
			{
				Owner = this,
			};

			try
			{
				rbtnLogWindow.BorderBrush = Brushes.Transparent;
				txtblkErrLogItemChanged.Visibility = Visibility.Hidden;
				m_wndLog.ShowDialog();
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}

			m_wndLog = null;
		}

		private static readonly string LogWindowEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.LogWindowEnabled);
		public bool LogWindowEnabled
		{
			get
			{
				return !m_RestartingThreads;
			}
		}
		/*----------------------------------------------------------*/


		/// <summary>
		/// Обновить список
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RefreshCmdExecuted(object sender, RoutedEventArgs e)
		{
			m_RestartingThreads = true;
			RefreshCommandEnable();

			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

				if (!Directory.Exists(settings.CompDir))
				{
					return;
				}

				m_DirScanner.SyncWithFilesAndDB(new CDirScanner.CSyncParam(settings.CompDir,
																			settings.dictFileScannerSettings.Values.ToList()));
			}

			m_RestartingThreads = false;
			RefreshCommandEnable();
		}

		private static readonly string RefreshEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.RefreshEnabled);
		public bool RefreshEnabled
		{
			get
			{
				return m_DirScanner != null && m_DirScanner.State == enScanningThreadState.Worked && !m_RestartingThreads;
			}
		}
		/*----------------------------------------------------------*/


		/// <summary>
		/// Запустить автообновление списка
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartCmdExecuted(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(DBManagerApp.m_AppSettings.m_Settings.CompDir))
			{
				MessageBox.Show(this,
								Properties.Resources.resNoDirForScan,
								Properties.Resources.resSyncingDBWithFiles,
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				return;
			}

			ToStopStyle();
						
			m_RestartingThreads = true;
			RefreshCommandEnable();

			RefreshCmdExecuted(sender, e);
			m_DirScanner.Start(DBManagerApp.m_AppSettings.m_Settings.CompDir);
			SyncStartStopBtnWithThState();
			
			m_RestartingThreads = false;
			RefreshCommandEnable();
		}
				
		/// <summary>
		/// Остановить автообновление списка
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StopCmdExecuted(object sender, RoutedEventArgs e)
		{
			ToStartStyle();

			m_RestartingThreads = true;
			RefreshCommandEnable();

			m_DirScanner.Stop(false);
			SyncStartStopBtnWithThState();

			m_RestartingThreads = false;
			RefreshCommandEnable();
		}

		private static readonly string AutoupdatingAvailablePropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.AutoupdatingAvailable);
		public bool AutoupdatingAvailable
		{
			get
			{
				return !m_RestartingThreads;
			}
		}
		/*----------------------------------------------------------*/
		

		/// <summary>
		/// Переписать данные из файлов в БД
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SyncDBWithFilesCmdExecuted(object sender, RoutedEventArgs e)
		{
			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

				if (!Directory.Exists(settings.CompDir))
				{
					MessageBox.Show(this,
									Properties.Resources.resNoDirForScan,
									Properties.Resources.resSyncingDBWithFiles,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return;
				}

				m_DirScanner.Stop(false);

				if (m_DirScanner.SyncWithFilesAndDB(new CDirScanner.CSyncParam(settings.CompDir,
																				settings.dictFileScannerSettings.Values.ToList())))
				{
					MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtSyncSuccessfully, settings.CompDir),
									Properties.Resources.resSyncingDBWithFiles,
									MessageBoxButton.OK,
									MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtSyncFailed, settings.CompDir),
									Properties.Resources.resSyncingDBWithFiles,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
				}
			}
		}

		private static readonly string SyncDBWithFilesEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.SyncDBWithFilesEnabled);
		public bool SyncDBWithFilesEnabled
		{
			get
			{
				return m_DirScanner != null && !m_RestartingThreads && m_DirScanner.State != enScanningThreadState.Worked;
			}
		}


		/// <summary>
		/// Вывод данных из БД на экран, не используя файлы
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DBToGridCmdExecuted(object sender, RoutedEventArgs e)
		{
			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

				if (!Directory.Exists(settings.CompDir))
				{
					MessageBox.Show(this,
									Properties.Resources.resNoDirForScan,
									Properties.Resources.resSyncingDBWithFiles,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return;
				}

				m_DirScanner.Stop(false);
				DBToGrid();

				MessageBox.Show(this,
								string.Format(Properties.Resources.resfmtDBToGridCopiedSuccessfully, settings.CompDir),
								Properties.Resources.resSyncingDBWithFiles,
								MessageBoxButton.OK,
								MessageBoxImage.Information);
			}
		}

		private static readonly string DBToGridEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.DBToGridEnabled);
		public bool DBToGridEnabled
		{
			get
			{
				return m_DirScanner != null && !m_RestartingThreads && m_DirScanner.State != enScanningThreadState.Worked;
			}
		}


		/// <summary>
		/// Экспорт протоколов в Excel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportToXlsCmdExecuted(object sender, RoutedEventArgs e)
		{
			CExportToExcelWnd wnd = new CExportToExcelWnd(m_DirScanner.CompId, CurrentGroups)
			{
				Owner = this,
			};

			try
			{
				wnd.ShowDialog();
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}
		}

		private static readonly string ExportToXlsEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.ExportToXlsEnabled);
		public bool ExportToXlsEnabled
		{
			get
			{
				return m_DirScanner != null &&
					!m_RestartingThreads &&
					m_DirScanner.CompId != GlobalDefines.DEFAULT_XML_INT_VAL &&
					CurrentGroups.Count > 0;
			}
		}
		/*----------------------------------------------------------*/


		
		#region ExportingToFTPNow
		private static readonly string ExportingToFTPNowPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.ExportingToFTPNow);

		private bool m_ExportingToFTPNow = false;

		public bool ExportingToFTPNow
		{
			get { return m_ExportingToFTPNow; }
			set
			{
				if (m_ExportingToFTPNow != value)
				{
					m_ExportingToFTPNow = value;
					OnPropertyChanged(FTPEnabledPropertyName);
					OnPropertyChanged(ExportingToFTPNowPropertyName);
				}
			}
		}
		#endregion
				
		
		private static readonly string FTPEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.FTPEnabled);
		public bool FTPEnabled
		{
			get
			{
				return m_DirScanner != null &&
					!m_RestartingThreads &&
					m_DirScanner.CompId != GlobalDefines.DEFAULT_XML_INT_VAL &&
					CurrentRounds != null &&
					CurrentRounds.Count > 0 &&
					!ExportingToFTPNow;
			}
		}

		/// <summary>
		/// Открытие настроек FTP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FTPSettingsCmdExecuted(object sender, RoutedEventArgs e)
		{
			CFTPSettingsWnd wnd = new CFTPSettingsWnd(m_DirScanner.CompId, CurrentGroups)
			{
				Owner = this,
			};

			try
			{
				wnd.ShowDialog();
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}
		}
		
		/// <summary>
		/// Принудительная отправка данных на FTP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SendToFTPCmdExecuted(object sender, RoutedEventArgs e)
		{
			if (!SendRoundToFTP(false,
								m_DirScanner.CompId,
								(enFTPSheetGeneratorTypes)CurrentRounds.SelectedKey,
								(from key in CurrentRounds.Keys select (enRounds)key).ToList(),
								CurrentGroups.SelectedItem.Value,
								CurrentGroups.SelectedKey))
			{
				MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtNoGroupSetsForFTPSending, CurrentGroups.SelectedItem.Value.AgeGroup.FullGroupName),
									Properties.Resources.resFTPSending,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
			}
			else
			{
				MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtWbkSentToFTPSuccefully,
													CurrentRounds.SelectedItem.Value.Name,
													CurrentGroups.SelectedItem.Value.AgeGroup.FullGroupName),
									Properties.Resources.resFTPSending,
									MessageBoxButton.OK,
									MessageBoxImage.Information);
			}
		}
		
		/// <summary>
		/// Открытие окна лога FTP
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FTPLogCmdExecuted(object sender, RoutedEventArgs e)
		{
			CFTPLogWnd wnd = new CFTPLogWnd()
			{
				Owner = this,
			};

			try
			{
				rbtnFTPLogWindow.BorderBrush = Brushes.Transparent;
				wnd.ShowDialog();
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}
		}
		/*----------------------------------------------------------*/


		private static readonly string CalcGradesEnabledPropertyName = GlobalDefines.GetPropertyName<MainWindow>(m => m.CalcGradesEnabled);
		public bool CalcGradesEnabled
		{
			get
			{
				return m_DirScanner != null &&
					!m_RestartingThreads &&
					m_DirScanner.CompId != GlobalDefines.DEFAULT_XML_INT_VAL &&
					CurrentRounds != null &&
					CurrentRounds.Count > 0 &&
					CurrentRounds.SelectedKey == (byte)enRounds.Total;
			}
		}

		protected void CalcGradesCmdExecuted(object sender, RoutedEventArgs ee)
		{
			CCalcGradesWnd wnd = new CCalcGradesWnd(CurrentGroups.SelectedKey, m_CurrentRoundMembers.OfType<CMemberInTotal>().ToList())
			{
				Owner = this,
			};

			try
			{
				wnd.ShowDialog();

				if (wnd.GradesChangedFromOpen)
				{	// Разряды менялись => нужно обновить таблицу
					CurrentRounds.SelectedItem.Command.DoExecute();
				}
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, Title);
				return;
			}
		}
		/*----------------------------------------------------------*/
		

		/// <summary>
		/// Команда отображения выпадающего меню с параметрами фильтрации
		/// </summary>
		public static RoutedCommand cmdOpenFilterPopup = new RoutedCommand();

		/// <summary>
		/// cmdOpenFilterPopup.Execute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OpenFilterPopupCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CheckBox chkShowFilterPopup = e.OriginalSource as CheckBox;
			if (chkShowFilterPopup == null || !chkShowFilterPopup.IsChecked.Value)
				return;

			Popup pppFilter = (chkShowFilterPopup.Parent as Panel).Children.OfType<Popup>().FirstOrDefault();
			if (pppFilter == null)
				return;

			List<FilterPredicate> lstCurPredicates; // Текущие настройки фильтрации
			List<object> lstPredicatesInDB = null; // Категории на основе информации, имеющейся в БД 

			enFilterTarget FilterTarget = (enFilterTarget)e.Parameter;
			IValueConverter Converter = null;
			Type TargetType = null;
			FilterPredicateComparer Comparer = new FilterPredicateComparer();

			if (!m_dictFilters.TryGetValue(FilterTarget, out lstCurPredicates))
			{
				lstCurPredicates = new List<FilterPredicate>();
				m_dictFilters.Add(FilterTarget, lstCurPredicates);
			}

			IEnumerable<CDBAdditionalClassBase> PredicatesSource = null;
			/* Если фильтрация по колонке включена, то нужно показать для неё список,
			   который получается при фильтрации по предыдущим колонкам */
			int FilterInd = ActiveFilters.IndexOf(FilterTarget);
			if (FilterInd >= 0)
			{
				PredicatesSource = m_CurrentRoundMembers;
				for (int i = 0; i < FilterInd; i++)
				{
					IEnumerable<FilterPredicate> SelectedPredicates = m_dictFilters[ActiveFilters[i]].Where(arg => arg.IsSelected);
					switch (ActiveFilters[i])
					{
						case enFilterTarget.SecondCol:
							if (IsTotal)
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberInTotal>()
												   where SelectedPredicates.FirstOrDefault(arg => (arg.FilterValue as string) == result.MemberInfo.SecondCol) != null
												   select result;
							}
							else
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberAndResults>()
												   where SelectedPredicates.FirstOrDefault(arg => (arg.FilterValue as string) == result.MemberInfo.SecondCol) != null
												   select result;
							}
							break;

						case enFilterTarget.YearOfBirth:
							if (IsTotal)
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberInTotal>()
												   where SelectedPredicates.FirstOrDefault(arg =>
												   {
													   return (arg.FilterValue == null && (result.MemberInfo.YearOfBirth == null || result.MemberInfo.YearOfBirth == 0)) ||
															   ((short?)arg.FilterValue == result.MemberInfo.YearOfBirth);
												   }) != null
												   select result;
							}
							else
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberAndResults>()
												   where SelectedPredicates.FirstOrDefault(arg =>
												   {
													   return (arg.FilterValue == null && (result.MemberInfo.YearOfBirth == null || result.MemberInfo.YearOfBirth == 0)) ||
															   ((short?)arg.FilterValue == result.MemberInfo.YearOfBirth);
												   }) != null
												   select result;
							}
							break;

						case enFilterTarget.Grade:
							if (IsTotal)
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberInTotal>()
												   where SelectedPredicates.FirstOrDefault(arg =>
												   {
													   return (arg.FilterValue == null && (result.MemberInfo.InitGrade == null || (int)result.MemberInfo.InitGrade == (int)enGrade.None)) ||
																((byte?)arg.FilterValue == result.MemberInfo.InitGrade);
												   }) != null
												   select result;
							}
							else
							{
								PredicatesSource = from result in PredicatesSource.Cast<CMemberAndResults>()
												   where SelectedPredicates.FirstOrDefault(arg =>
												   {
													   return (arg.FilterValue == null && (result.MemberInfo.InitGrade == null || (int)result.MemberInfo.InitGrade == (int)enGrade.None)) ||
																((byte?)arg.FilterValue == result.MemberInfo.InitGrade);
												   }) != null
												   select result;
							}
							break;
					}
				}
			}
			else
				PredicatesSource = m_lstFilteredMembers;

			switch (FilterTarget)
			{
				case enFilterTarget.SecondCol:
					if (IsTotal)
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberInTotal>()
											  select member.MemberInfo.SecondCol as object).Distinct().ToList();
					}
					else
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberAndResults>()
											  select member.MemberInfo.SecondCol as object).Distinct().ToList();
					}
					TargetType = typeof(string);
					Comparer.CompareProperty = RoundResultsControl.FilterControl.enCompareProperty.FilterValue;
					Comparer.SortDir = ListSortDirection.Ascending;
					Comparer.NullFilterValue = "";
					break;

				case enFilterTarget.YearOfBirth:
					if (IsTotal)
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberInTotal>()
											 select member.MemberInfo.YearOfBirth as object).Distinct().ToList();
					}
					else
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberAndResults>()
											  select member.MemberInfo.YearOfBirth as object).Distinct().ToList();
					}
					Converter = new YearOfBirthMarkupConverter();
					TargetType = typeof(short?);
					Comparer.CompareProperty = RoundResultsControl.FilterControl.enCompareProperty.FilterValue;
					Comparer.SortDir = ListSortDirection.Ascending;
					Comparer.NullFilterValue = (short)0;
					break;

				case enFilterTarget.Grade:
					if (IsTotal)
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberInTotal>()
											  select member.MemberInfo.InitGrade as object).Distinct().ToList();
					}
					else
					{
						lstPredicatesInDB = (from member in PredicatesSource.Cast<CMemberAndResults>()
											  select member.MemberInfo.InitGrade as object).Distinct().ToList();
					}
					Converter = new GradeMarkupConverter();
					TargetType = typeof(byte?);
					Comparer.CompareProperty = RoundResultsControl.FilterControl.enCompareProperty.FilterValue;
					Comparer.SortDir = ListSortDirection.Descending;
					Comparer.NullFilterValue = enGrade.None;
					break;
			}

			if (lstPredicatesInDB != null)
			{
				bool HasSelectedItems = false;
				// Сравниваем категории в CathegoriesInDB и lstCurCathegories
				for (int i = 0; i < lstCurPredicates.Count; )
				{
					object Item = null;
					if (lstCurPredicates[i].FilterValue == null)
						Item = lstPredicatesInDB.FirstOrDefault(arg => arg == null);
					else
						Item = lstPredicatesInDB.FirstOrDefault(arg => arg.Equals(lstCurPredicates[i].FilterValue));

					if (Item == null)
					{	// Такой категории больше нет => удаляем её из lstCurCathegories
						lstCurPredicates.RemoveAt(i);
					}
					else
					{
						HasSelectedItems |= lstCurPredicates[i].IsSelected;
						string Name = Converter == null ?
										Item.ToString() :
										Converter.Convert(Item,
															TargetType,
															null,
															LocalizationManager.UICulture).ToString();
						lstCurPredicates[i].Name = string.IsNullOrWhiteSpace(Name) ? Properties.Resources.resEmpty : Name;
						i++;
					}
				}

				bool ItemsAdded = false;
				for (int i = 0; i < lstPredicatesInDB.Count; )
				{
					bool ItemExists = false;

					if (lstPredicatesInDB[i] == null)
						ItemExists = lstCurPredicates.Exists(arg => arg == null);
					else
						ItemExists = lstCurPredicates.Exists(arg => arg.FilterValue.Equals(lstPredicatesInDB[i]));
					if (!ItemExists)
					{	// Такой категории у нас нет, но она появилась => удаляем её из lstCurPredicates
						ItemsAdded = true;
						string Name = Converter == null ?
										lstPredicatesInDB[i].ToString() :
										Converter.Convert(lstPredicatesInDB[i],
															TargetType,
															null,
															LocalizationManager.UICulture).ToString();
						lstCurPredicates.Add(new FilterPredicate()
							{
								FilterValue = lstPredicatesInDB[i],
								Name = string.IsNullOrWhiteSpace(Name) ? Properties.Resources.resEmpty : Name
							});
					}
					else
						i++;
				}

				if (!HasSelectedItems)
				{	// Нужно выделить все записи, т.к. не может быть списка с невыделенными записями
					lstCurPredicates.ForEach(arg => arg.IsSelected = true);
				}
				if (ItemsAdded)
					lstCurPredicates.Sort(Comparer);

				CFilterControl FilterControl = new CFilterControl(pppFilter,
																	FilterTarget,
																	lstCurPredicates,
																	FilterControl_Filter,
																	FilterControl_DontFilter);

				pppFilter.Closed += pppFilter_Closed;
				pppFilter.PlacementTarget = chkShowFilterPopup;
				pppFilter.Child = FilterControl;
				pppFilter.Tag = chkShowFilterPopup;
				pppFilter.IsOpen = true;
			}
		}


		/// <summary>
		/// cmdOpenFilterPopup.CanExecute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OpenFilterPopupCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
		#endregion


		public MainWindow()
		{
			InitializeComponent();
		
			try
			{
				DBManagerApp.MainWnd = this;

				collectionCurrentRoundMembers = new ObservableCollectionEx<CDBAdditionalClassBase>();
				collectionCurrentRoundMembers2 = new ObservableCollectionEx<CDBAdditionalClassBase>();

				//dgrdRoundMembers.ItemContainerGenerator.StatusChanged += (s, e) =>
				//{
				//    if (dgrdRoundMembers.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
				//    {
				//        System.Diagnostics.Debug.WriteLine(GlobalDefines.m_swchGlobal.Elapsed.TotalSeconds);
				//    }
				//};
								
				CTaskBarIconTuning.ResetProgressValue(); // Чтобы объект создался в основном потоке
								
				#region Настройка пунктов стандартного меню Ribbon
				FieldInfo fi;
				/* Меняем названия пунктов в стандпртном меню Ribbon */
				fi = typeof(RibbonContextMenu).GetField("AddToQATText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_AddToQATText);
				fi = typeof(RibbonContextMenu).GetField("RemoveFromQATText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_RemoveFromQATText);
				fi = typeof(RibbonContextMenu).GetField("ShowQATAboveText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_ShowQATAboveText);
				fi = typeof(RibbonContextMenu).GetField("ShowQATBelowText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_ShowQATBelowText);
				fi = typeof(RibbonContextMenu).GetField("MaximizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_MaximizeTheRibbonText);
				fi = typeof(RibbonContextMenu).GetField("MinimizeTheRibbonText", (BindingFlags.NonPublic | BindingFlags.Static));
				fi.SetValue(null, Properties.Resources.RibbonContext_MinimizeTheRibbonText);

				/* Изменяем стандартное меню в Ribbon, оставляя только нужные пункты.
				 * Список стандартных пунктов можно узнать из кода, который описан здесь http://dotnetinside.com/framework/v4.0.30319/System.Windows.Controls.Ribbon/RibbonContextMenu
				 */
				RibbonContextMenu RibbonClientAreaContextMenu = new RibbonContextMenu();
				fi = typeof(RibbonContextMenu).GetField("_defaultRibbonClientAreaContextMenu", (BindingFlags.NonPublic | BindingFlags.Static));
				MethodInfo mi = typeof(RibbonContextMenu).GetMethod("GenerateMinimizeTheRibbonItem", (BindingFlags.NonPublic | BindingFlags.Static));
				object[] MethodsArgs = new object[] { RibbonClientAreaContextMenu };
				RibbonClientAreaContextMenu.Items.Add(mi.Invoke(null, MethodsArgs));
				fi.SetValue(null, RibbonClientAreaContextMenu);

				RibbonContextMenu RibbonControlContextMenu = new RibbonContextMenu();
				fi = typeof(RibbonContextMenu).GetField("_ribbonControlContextMenu", (BindingFlags.NonPublic | BindingFlags.Static));
				mi = typeof(RibbonContextMenu).GetMethod("GenerateMinimizeTheRibbonItem", (BindingFlags.NonPublic | BindingFlags.Static));
				MethodsArgs = new object[] { RibbonControlContextMenu };
				RibbonControlContextMenu.Items.Add(mi.Invoke(null, MethodsArgs));
				fi.SetValue(null, RibbonControlContextMenu);
				#endregion

				Title = string.Format(Properties.Resources.resfmtMainwndTitleNoComp, AppAttributes.Title, AppAttributes.Version);

				HighlightTypes.Add(new CKeyValuePairEx<enHighlightGradesType, string>(enHighlightGradesType.None, Properties.Resources.resHighlightNothing, HighlightGradeTypeCommamdHandler));
				HighlightTypes.Add(new CKeyValuePairEx<enHighlightGradesType, string>(enHighlightGradesType.ResultGrades, Properties.Resources.resHighlightResultGrades, HighlightGradeTypeCommamdHandler));
				HighlightTypes.Add(new CKeyValuePairEx<enHighlightGradesType, string>(enHighlightGradesType.CarryoutGrades, Properties.Resources.resHighlightCarryoutGrades, HighlightGradeTypeCommamdHandler));
				HighlightTypes.Add(new CKeyValuePairEx<enHighlightGradesType, string>(enHighlightGradesType.СonfirmGrades, Properties.Resources.resHighlightСonfirmGrades, HighlightGradeTypeCommamdHandler));
				
				CurrentGroups.CollectionChanged += CurrentGroups_CollectionChanged;
				CurrentRounds.CollectionChanged += CurrentRounds_CollectionChanged;

				if (DBManagerApp.m_AppSettings.m_Settings.AutodetectOnStart)
				{
					m_DirScanner = new CDirScanner(DBManagerApp.m_AppSettings.m_Settings.CompDir,
													null,
													DBManagerApp.m_AppSettings.m_Settings.CompDir != GlobalDefines.DEFAULT_XML_STRING_VAL,
													new CDirScanner.CSyncParam(DBManagerApp.m_AppSettings.m_Settings.CompDir,
																				DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Values.ToList()));
					m_DirScanner.DataChanged += DirScaner_DataChanged;
					DirScaner_DataChanged(m_DirScanner, m_DirScanner.LastDataChangedEventArgs);
					if (DBManagerApp.m_AppSettings.m_Settings.CompDir == GlobalDefines.DEFAULT_XML_STRING_VAL || m_DirScanner.SyncSuccessfully)
					{
						m_DirScanner.Start(DBManagerApp.m_AppSettings.m_Settings.CompDir);
					}
				}
				else
				{
					m_DirScanner = new CDirScanner(DBManagerApp.m_AppSettings.m_Settings.CompDir,
													null,
													true);
					m_DirScanner.DataChanged += DirScaner_DataChanged;
				}
				
				CurrentGroups.Clear();
				if (m_DirScanner.SyncSuccessfully ||
					m_DirScanner.State == enScanningThreadState.Worked ||
					!DBManagerApp.m_AppSettings.m_Settings.AutodetectOnStart)
				{
					DBToGrid();
				}

				OnFontStyliesChanged();

				SyncStartStopBtnWithThState();
				RefreshCommandEnable();

				rchkShowGroupHead_Click(rchkShowGroupHead, null);
								
				vsrcCurrentRoundMembers.Source = collectionCurrentRoundMembers;
				vsrcCurrentRoundMembers2.Source = collectionCurrentRoundMembers2;
				SetFilterFunc(null, false);

				m_tmrAutoscroll.Tick += m_tmrAutoscroll_Tick;
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}


		#region Перехват cообщений Windows
		private IntPtr HwndMessageHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
		{
			return IntPtr.Zero;
		}
		#endregion


		protected override void OnClosing(CancelEventArgs e)
		{
			if (!m_ShowMsgBeforeClose ||
				MessageBox.Show(this,
								string.Format(Properties.Resources.resmsgfmtCloseQuestion, AppAttributes.Title),
								AppAttributes.Title,
								MessageBoxButton.YesNo,
								MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				e.Cancel = false;

				if (m_FTPExporter != null)
					(m_FTPExporter as IDisposable).Dispose();
			}
			else
				e.Cancel = true;
			
			base.OnClosing(e);
		}


		#region Фильтрация
		void pppFilter_Closed(object sender, EventArgs e)
		{
			Popup pppFilter = sender as Popup;
			if (pppFilter != null && pppFilter.Tag != null && pppFilter.Child is CFilterControl)
			{
				pppFilter.Closed -= pppFilter_Closed;
				(pppFilter.Tag as CheckBox).IsChecked = false;
				pppFilter.Tag = null;

				CFilterControl FilterControl = pppFilter.Child as CFilterControl;

				switch (FilterControl.CloseReason)
				{
					case CFilterControl.enCloseReason.Cancel:
					case CFilterControl.enCloseReason.LostFocus: /* Нужно вернуть значение m_dictFilters[FilterControl.FilterTarget],
																  * которое было на момент открытия FilterControl */
						if (m_dictFilters[FilterControl.FilterTarget].All(arg => arg.IsSelected))
						{	// Фильтрация по столбцу отменена => удаляем его предикаты из m_dictFilters
							m_dictFilters.Remove(FilterControl.FilterTarget);
						}
						break;
				}
			}
		}


		/// <summary>
		/// Фильтруем
		/// </summary>
		/// <param name="sender"></param>
		void FilterControl_Filter(CFilterControl sender)
		{
			if (sender.FilterPredicates.All(arg => arg.IsSelected))
			{	// Фильтрация по столбцу отменена => удаляем его предикаты из m_dictFilters
				m_dictFilters.Remove(sender.FilterTarget);
				if (ActiveFilters.Remove(sender.FilterTarget))
					OnPropertyChanged(ActiveFiltersPropertyName);
			}
			else
			{	// Если есть хотя бы один не выделенный элемент, то фильтрация по этому столбцу включена
				m_dictFilters[sender.FilterTarget] = sender.FilterPredicates.ToList();

				if (!ActiveFilters.Contains(sender.FilterTarget))
				{
					ActiveFilters.Add(sender.FilterTarget);
					OnPropertyChanged(ActiveFiltersPropertyName);
				}
			}
			
			if (sender.PredicatesChanged)
			{	// Изменились условия фильтрации
				// Удаляем места во всех предыдущих результатах фильтрации
				m_lstFilteredMembers.ForEach(arg => arg.PlaceInFilter = null);
				
				// Результаты фильтрации
				m_lstFilteredMembers.Clear();
				foreach (CDBAdditionalClassBase Member in m_CurrentRoundMembers)
				{
					if (FilterFunc(Member))
						m_lstFilteredMembers.Add(Member);
				}

				if (m_lstFilteredMembers.Count != m_CurrentRoundMembers.Count())
				{	// Автоматически расставляем места, если что-то отфильтровали
					CDBAdditionalClassBaseComparer Comparer = new CDBAdditionalClassBaseComparer()
					{
						CompareProperty = CDBAdditionalClassBaseComparer.enCompareProperty.Place
					};
					// Сортируем результаты фильтрации, чтобы расставить места
					m_lstFilteredMembers.Sort(Comparer);
					// Расставляем места
					int CurPlace = 1;
					int ResultIndex = 0;
					int? PrevResult = null; // Такого результата не может быть
					foreach (CDBAdditionalClassBase Member in m_lstFilteredMembers)
					{
						ResultIndex++;

						if (!Member.Place.HasValue)
						{	// Пропускаем участников без мест
							continue;
						}

						if (Member.Place != PrevResult)
							CurPlace = ResultIndex;

						Member.PlaceInFilter = CurPlace;
						PrevResult = Member.Place.Value;

						if (Member is CMemberAndResults)
							(Member as CMemberAndResults).VisibilityInMainTable = Visibility.Visible;
					}
				}

				if (m_dictFilters.Count > 0)
				{	// Фильтрация включена
					RightPanel.FilteredMembersQ = m_lstFilteredMembers.Count;
					SetFilterFunc(FilterFunc, true);

					ShowRightDataGrid(false); // Чтобы упростить себе жизнь, будем выводить результаты фильтрации только в одном Grid
				}
				else
				{
					RightPanel.FilteredMembersQ = null;
					ResetFilters();
				}
			}

			sender.ParentPopup.IsOpen = false;
		}


		private void ResetFilters()
		{
			m_dictFilters.Clear();
			ActiveFilters.Clear();
			SetFilterFunc(null, true);
			m_MembersInLeftGrid = -1;
		
			OnPropertyChanged(ActiveFiltersPropertyName);

			if ((enRounds)CurrentRounds.SelectedKey == enRounds.Qualif || (enRounds)CurrentRounds.SelectedKey == enRounds.Qualif2)
			{	// Если фильтрация выключена, то, возможно, нужно показать второе окно с результатами
				grdRoundMembersHost_SizeChanged(grdRoundMembersHost, null);
			}
		}


		private void SetFilterFunc(Predicate<object> Func, bool Refresh)
		{
			Dispatcher.Invoke(new Action(delegate ()
			{
				if (vsrcCurrentRoundMembers.View != null)
				{
					vsrcCurrentRoundMembers.View.Filter = Func;
					if (Refresh)
						vsrcCurrentRoundMembers.View.Refresh();
				}
			}));
		}


		private bool FilterFunc(object item)
		{
			CFullMemberInfo CheckingMember = null;
			if (IsTotal)
				CheckingMember = (item as CMemberInTotal).MemberInfo;
			else
				CheckingMember = (item as CMemberAndResults).MemberInfo;

			foreach (KeyValuePair<enFilterTarget, List<FilterPredicate>> Predicates in m_dictFilters)
			{
				IEnumerable<FilterPredicate> SelectedPredicates = Predicates.Value.Where(arg => arg.IsSelected);
				switch (Predicates.Key)
				{
					case enFilterTarget.SecondCol:
						if (SelectedPredicates.FirstOrDefault(arg => (arg.FilterValue as string) == CheckingMember.SecondCol) == null)
							return false;
						break;

					case enFilterTarget.YearOfBirth:
						if (SelectedPredicates.FirstOrDefault(arg =>
						{
							return (arg.FilterValue == null && (CheckingMember.YearOfBirth == null || CheckingMember.YearOfBirth == 0)) ||
									((short?)arg.FilterValue == CheckingMember.YearOfBirth);
						}) == null)
						{
							return false;
						}
						break;

					case enFilterTarget.Grade:
						if (SelectedPredicates.FirstOrDefault(arg =>
						{
							return (arg.FilterValue == null && (CheckingMember.InitGrade == null || (int)CheckingMember.InitGrade == (int)enGrade.None)) ||
									((byte?)arg.FilterValue == CheckingMember.InitGrade);
						}) == null)
						{
							return false;
						}
						break;
				}
			}

			return true;
		}


		/// <summary>
		/// Не нужно ничего фильтровать => просто закрываем popup
		/// </summary>
		/// <param name="sender"></param>
		void FilterControl_DontFilter(CFilterControl sender)
		{
			RightPanel.FilteredMembersQ = null;
			sender.ParentPopup.IsOpen = false;
		}
		#endregion


		#region Управление стилями кнопки Старт/Стоп
		void ToStopStyle()
		{
			rbtnStartStop.Tag = "StopStyle";
			rbtnStartStop.Click -= StartCmdExecuted;
			rbtnStartStop.Click -= StopCmdExecuted;
			rbtnStartStop.Click += StopCmdExecuted;

#if TICKER
			tckrMembersOnStart.RunAnimation = true;
#endif
		}


		void ToStartStyle()
		{
			rbtnStartStop.Tag = "StartStyle";
			rbtnStartStop.Click -= StopCmdExecuted;
			rbtnStartStop.Click -= StartCmdExecuted;
			rbtnStartStop.Click += StartCmdExecuted;

#if TICKER
			tckrMembersOnStart.RunAnimation = false;
			tckrMembersOnStart.TickerText = "";
#endif
		}


		void SyncStartStopBtnWithThState()
		{
			switch (m_DirScanner.State)
			{
				case enScanningThreadState.Worked:
					ToStopStyle();
					break;

				case enScanningThreadState.Stopped:
					ToStartStyle();
					break;
			}
		}
		#endregion


		void OnFontStyliesChanged()
		{
			dgrdRoundMembers2.FontFamily =
				dgrdRoundMembers.FontFamily = new System.Windows.Media.FontFamily(DBManagerApp.m_AppSettings.m_Settings.FontFamilyName);
			dgrdRoundMembers2.FontSize =
				dgrdRoundMembers.FontSize = DBManagerApp.m_AppSettings.m_Settings.FontSize;
			dgrdRoundMembers2.ColumnHeaderHeight =
				dgrdRoundMembers.ColumnHeaderHeight = dgrdRoundMembers.FontSize * 30.0 / 14.0;
			
			RightPanel.PlainResultsFontStyle = DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle;
			RightPanel.InvitedToStartFontStyle = DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle;
			RightPanel.JustRecievedResultFontStyle = DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle;
			RightPanel.NextRoundMembersCountFontStyle = DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle;
			RightPanel.PreparingFontStyle = DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle;
			RightPanel.StayOnStartFontStyle = DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle;
			RightPanel.FalsestartFontStyle = DBManagerApp.m_AppSettings.m_Settings.FalsestartFontStyle;
			

			OnPropertyChanged(CRightPanelControl.PlainResultsFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.InvitedToStartFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.JustRecievedResultFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.NextRoundMembersCountFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.PreparingFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.StayOnStartFontStylePropertyName);
			OnPropertyChanged(CRightPanelControl.FalsestartFontStylePropertyName);
		}


		void DirScaner_DataChanged(CScannerBase sender, DataChangedEventArgs e)
		{
			if (e != null)
			{
				foreach (CDataChangedInfo Changing in e.Changes)
				{
					if (Changing.ChangedObjects.HasFlag(enDataChangedObjects.CompSettings))
					{	// Что-то поменялось в настройках какой-то группы/соревнования или была добавлена группа/соревнование
						if (Changing.GroupID == GlobalDefines.DEFAULT_XML_INT_VAL)
						{	// Добавлено/изменено соревнование
							Dispatcher.Invoke(new Action<descriptions>(SetDesc), Changing.Argument as descriptions);
						}
						else
						{	// Добавлена/изменена группа
							CFileScanner scanner = Changing.OriginalSource as CFileScanner;
							CCompSettings GroupSettings = null;
							lock (scanner.DataSyncObj)
								GroupSettings = new CCompSettings(scanner.DataFromXml.Settings);
							switch (Changing.ChangingType)
							{
								case enDataChangesTypes.Add:
									int CurrentCount = CurrentGroups.Count;
									if (CurrentGroups.TryAddValue(Changing.GroupID,
																	new CKeyValuePairEx<long, CCompSettings>(Changing.GroupID,
																											GroupSettings,
																											GroupCommamdHandler)))
									{
										if (CurrentCount == 0)
										{	// Выбираем первую группу
											Dispatcher.Invoke(new Action(delegate()
											{
												CurrentGroups[CurrentGroups.Keys.First()].Command.DoExecute();
											}));
										}
									}
									break;

								case enDataChangesTypes.Changing:
									{
										CKeyValuePairEx<long, CCompSettings> CurrentGroupSettings;
										if (CurrentGroups.TryGetValue(Changing.GroupID, out CurrentGroupSettings))
										{
											// Если настройки группы изменятся, то сработает событие
											CurrentGroupSettings.PropertyChanged += CurrentGroupSettings_PropertyChanged;
											CurrentGroupSettings.Value = GroupSettings;
											CurrentGroupSettings.PropertyChanged -= CurrentGroupSettings_PropertyChanged;
										}
										break;
									}
							}
						}
					}

					if (Changing.ChangedObjects.HasFlag(enDataChangedObjects.Group))
					{	// Добавлена/изменена/удалена группа
						switch (Changing.ChangingType)
						{
							case enDataChangesTypes.Add:
								{
									CFileScanner scanner = Changing.OriginalSource as CFileScanner;
									CCompSettings GroupSettings = null;
									lock (scanner.DataSyncObj)
										GroupSettings = new CCompSettings(scanner.DataFromXml.Settings);

									int CurrentCount = CurrentGroups.Count;
									if (CurrentGroups.TryAddValue(Changing.GroupID,
																	new CKeyValuePairEx<long, CCompSettings>(Changing.GroupID,
																											GroupSettings,
																											GroupCommamdHandler)))
									{
										if (CurrentCount == 0)
										{	// Выбираем первую группу
											Dispatcher.Invoke(new Action(delegate()
											{
												CurrentGroups[CurrentGroups.Keys.First()].Command.DoExecute();
											}));
										}
									}
									break;
								}

							case enDataChangesTypes.Changing:
								{
									CFileScanner scanner = Changing.OriginalSource as CFileScanner;
									CCompSettings GroupSettings = null;
									lock (scanner.DataSyncObj)
										GroupSettings = new CCompSettings(scanner.DataFromXml.Settings);

									CKeyValuePairEx<long, CCompSettings> CurrentGroupSettings;
									if (CurrentGroups.TryGetValue(Changing.GroupID, out CurrentGroupSettings))
									{
										// Если настройки группы изменятся, то сработает событие
										CurrentGroupSettings.PropertyChanged += CurrentGroupSettings_PropertyChanged;
										CurrentGroupSettings.Value = GroupSettings;
										CurrentGroupSettings.PropertyChanged -= CurrentGroupSettings_PropertyChanged;
									}
									break;
								}

							case enDataChangesTypes.Delete:
								{
									long CurSelectedGroup = CurrentGroups.SelectedKey;
									CurrentGroups.Remove(Changing.GroupID);
									if (Changing.GroupID == CurSelectedGroup)
									{
										CurrentRounds.Clear();
										if (CurrentGroups.Count > 0)
										{
											Dispatcher.Invoke(new Action(delegate ()
											{
												CurrentGroups[CurrentGroups.Keys.First()].Command.DoExecute();
											}));
										}										
									}

									// Удаляем группу из DBManagerApp.m_AppSettings.m_Settings
									lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
									{
										if (CurrentGroups.Count == 0)
											DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.Remove(m_DirScanner.CompId);
									}
								}
								break;
						}
					}

					if (Changing.ChangedObjects.HasFlag(enDataChangedObjects.Members) ||
						Changing.ChangedObjects.HasFlag(enDataChangedObjects.Results))
					{	// Сменились результаты
						if (CurrentGroups.SelectedItem != null && Changing.GroupID == CurrentGroups.SelectedKey)
						{
							if (CurrentRounds.SelectedItem == null)
							{	// Раунд не выбран, но скорее всего он появился
								Dispatcher.Invoke(new Action<CKeyValuePairEx<long, CCompSettings>>(GroupCommamdHandler),
													CurrentGroups.SelectedItem);
							}
							else
							{	// Сейчас выбран какой-то раунд
								if (Changing.Argument is enChangeReason || Changing.ID == CurrentRounds.SelectedKey)
								{	// Выбран именно тот раунд, результаты в котором изменились
									Dispatcher.Invoke(new Action<CKeyValuePairEx<byte, CRoundAndDate>>(RoundCommamdHandler), CurrentRounds.SelectedItem);
									if (Changing.Argument is enChangeReason)
									{	// Нужно обновить общее число участников
										Dispatcher.Invoke(new Action(delegate()
										{
											RightPanel.WholeMembersQ = (from part in DBManagerApp.m_Entities.participations
																		where part.Group == CurrentGroups.SelectedKey
																		select part.id_participation).Count();
										}));
									}
								}

								if (Changing.ChangingType == enDataChangesTypes.RoundFinished || !CurrentRounds.ContainsKey((byte)enRounds.Qualif))
								{	// В соревнования добавился новый раунд или пока ещё не добавлена квалификация => нужно изменить список в выпадающем меню
									Dispatcher.Invoke(new Action<CKeyValuePairEx<long, CCompSettings>>(GroupCommamdHandler),
														CurrentGroups.SelectedItem);
								}
							}
						}

						// Проверяем, нужно ли отправлять этот раунд на FTP
						CCompSpecificSets CompSets = null;
						CFTPGroupItemInSets FTPGroupItemInSets = null;
											
						lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
						{
							if (!m_FTPExporter.IsStarted ||
								!DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.TryGetValue(m_DirScanner.CompId, out CompSets) ||
								!CompSets.dictGroupsForAutosendToFTP.TryGetValue(Changing.GroupID, out FTPGroupItemInSets) ||
								!FTPGroupItemInSets.CheckFTPWbkFullPath() ||
								!FTPGroupItemInSets.IsSelected)
							{	/* Настроек для группы нет или её не нужно автоматически отправлять на сервер =>
								 * отправка на сервер невозможна */
								break;
							}

							List<enRounds> GroupRounds = (from result in DBManagerApp.m_Entities.results_speed
														  join part in DBManagerApp.m_Entities.participations on result.participation equals part.id_participation
														  where part.Group == Changing.GroupID
														  group result by result.round into groupRounds
														  orderby groupRounds.Key
														  select (enRounds)groupRounds.Key).ToList();
							GroupRounds.Add(enRounds.Total); // Итоговый протокол всегда есть

							switch (Changing.ChangingType)
							{
								case enDataChangesTypes.Add:
								case enDataChangesTypes.Delete:
									SendRoundToFTP(true,
											m_DirScanner.CompId,
											enFTPSheetGeneratorTypes.Qualif, // Эти операции могут быть только в первой квалификации
											GroupRounds,
											CurrentGroups[Changing.GroupID].Value,
											Changing.GroupID);
									break;

								case enDataChangesTypes.QualifSorted:
								case enDataChangesTypes.RoundFinished:
								case enDataChangesTypes.AddManyPcs:
									SendRoundToFTP(true,
												m_DirScanner.CompId,
												(enFTPSheetGeneratorTypes)(Changing.ID),
												GroupRounds,
												CurrentGroups[Changing.GroupID].Value,
												Changing.GroupID);
									break;

								case enDataChangesTypes.Changing:
									SendRoundToFTP(true,
													m_DirScanner.CompId,
													(Changing.ChangedObjects == enDataChangedObjects.Results) ? (enFTPSheetGeneratorTypes)(Changing.ID) : enFTPSheetGeneratorTypes.Qualif,
													GroupRounds,
													CurrentGroups[Changing.GroupID].Value,
													Changing.GroupID);
									break;
							}
						}
					}

					if (Changing.ChangedObjects.HasFlag(enDataChangedObjects.Exception))
					{	// В лог было что-то добавлено =>
						// меняем фон кнопки открытия лога или обновляем лог
						Dispatcher.Invoke(new Action(delegate()
						{
							if (m_wndLog != null)
								m_wndLog.RefreshItems();
							else
							{
								rbtnLogWindow.BorderBrush = Brushes.Red;
								txtblkErrLogItemChanged.Visibility = Visibility.Visible;
							}
						}));
					}
				}
			}
		}


		void CurrentGroups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (CurrentGroups.Count == 0)
			{
				rmbtnGroup.Label = Properties.Resources.resSelectGroup;
				SetDesc(null);
			}
			HighlightTypes[0].Command.DoExecute();
			OnPropertyChanged(FTPEnabledPropertyName);
			OnPropertyChanged(CalcGradesEnabledPropertyName);
		}


		void CurrentRounds_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (CurrentRounds.Count == 0)
			{
				rmbtnRound.Label = Properties.Resources.resSelectRound;
				lblRoundDate.Content = lblRoundName.Content = "";
				
				m_CurrentRoundMembers = null;
				collectionCurrentRoundMembers.Clear();
				collectionCurrentRoundMembers2.Clear();
				dgrdRoundMembers.Columns.Clear();
				dgrdRoundMembers.Style = null;
				RightPanel.ClearTemplate();
			}
			HighlightTypes[0].Command.DoExecute();
			OnPropertyChanged(FTPEnabledPropertyName);
			OnPropertyChanged(CalcGradesEnabledPropertyName);
		}


		void CurrentGroupSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CKeyValuePairEx<long, CCompSettings>.ValuePropertyName)
			{
				CKeyValuePairEx<long, CCompSettings> ChangedGroupSettings = sender as CKeyValuePairEx<long, CCompSettings>;
				if (CurrentGroups.SelectedKey == ChangedGroupSettings.Key)
				{	// Изменилась выбранная сейчас группа => нужно отобразить изменения
					Dispatcher.Invoke(new Action<CKeyValuePairEx<long, CCompSettings>>(GroupCommamdHandler), ChangedGroupSettings);
				}
			}
		}


		void SetDesc(descriptions Desc)
		{
			if (Desc == null)
			{
				Title = string.Format(Properties.Resources.resfmtMainwndTitleNoComp, AppAttributes.Title, AppAttributes.Version);
				lblCompName.Content = lblMainJudge.Content = lblMainSecretary.Content = "";
				lblRow6.Content = null;
				CurrentRounds.Clear();
				CurrentGroups.Clear();
			}
			else
			{
				Title = string.Format(Properties.Resources.resfmtMainwndTitleWithComp,
										AppAttributes.Title,
										AppAttributes.Version,
										Desc.name);
				lblCompName.Content = Desc.name;
			}
		}


		void DBToGrid()
		{
			if (m_DirScanner == null)
				return;

			descriptions CurDesc = null;
			if (m_DirScanner.CompId == GlobalDefines.DEFAULT_XML_INT_VAL)
			{	// Файлов в папке нет, но может быть что-то есть в БД
				CurDesc = DBManagerApp.m_Entities.descriptions.Where(arg => arg.dir == DBManagerApp.m_AppSettings.m_Settings.CompDir).FirstOrDefault();
			}
			else
			{
				CurDesc = DBManagerApp.m_Entities.descriptions.Where(arg => arg.id_desc == m_DirScanner.CompId).FirstOrDefault();
			}

			SetDesc(CurDesc);

			if (CurDesc != null)
			{	// Выводим группы
				m_DirScanner.CompId = CurDesc.id_desc;
				foreach (groups group in CurDesc.groups)
				{
					CCompSettings GroupSettings = null;

					KeyValuePair<string, CFileScanner> ScannerPair = m_DirScanner.FileScanners.Where(arg => (arg.Value.Group != null && arg.Value.Group.id_group == group.id_group)).FirstOrDefault();
					if (!ScannerPair.Equals(default(KeyValuePair<string, CFileScanner>)))
					{
						// Копируем сведения о группе
						lock (ScannerPair.Value.DataSyncObj)
							GroupSettings = new CCompSettings(ScannerPair.Value.DataFromXml.Settings);
					}
					else
						GroupSettings = new CCompSettings(group);

					CurrentGroups.TryAddValue(group.id_group,
												new CKeyValuePairEx<long, CCompSettings>(group.id_group,
																						GroupSettings,
																						GroupCommamdHandler));
				}

				if (CurrentGroups.Count > 0)
				{	// Выбираем первую группу
					CurrentGroups[CurrentGroups.Keys.First()].Command.DoExecute();
				}
			}
		}


		void GroupCommamdHandler(CKeyValuePairEx<long, CCompSettings> sender)
		{
			ResetFilters(); // Очищаем все фильтры при переходе к новой группе

			bool GroupChanged = CurrentGroups.SelectedKey != sender.Key;
			CurrentGroups.SelectedKey = sender.Key;
			
			SecondColName = sender.Value.SecondColName;
									
			rmbtnGroup.Label = sender.Value.AgeGroup.FullGroupName;
			lblMainJudge.Content = sender.Value.MainJudge ?? Properties.Resources.resDontSet;
			lblMainSecretary.Content = sender.Value.MainSecretary ?? Properties.Resources.resDontSet;
			lblRow6.Content = sender.Value.Row6;

			// Заводим промежуточный массив, чтобы умешить число вызовов OnPropertyChanged при добавлении в m_CurrentRounds
			ObservableDictionary<byte, CKeyValuePairEx<byte, CRoundAndDate>> GroupRounds = new ObservableDictionary<byte, CKeyValuePairEx<byte, CRoundAndDate>>();

			List<KeyValuePair<string, string>> RoundDates = sender.Value.RoundDates;
			foreach (dynamic RoundInfo in from result in DBManagerApp.m_Entities.results_speed
										  join part in DBManagerApp.m_Entities.participations on result.participation equals part.id_participation
										  where part.Group == sender.Key
										  group result by result.round into groupRounds
										  orderby groupRounds.Key
										  select new
										  {
											  RoundID = groupRounds.Key,
											  RoundName = (from round in DBManagerApp.m_Entities.rounds
														   join groupRound in groupRounds on round.id_round equals groupRound.round
														   where round.id_round == groupRounds.Key
														   select round.name).FirstOrDefault()
										  })
			{
				if (RoundInfo.RoundName == null)
					continue;

				byte RoundID = RoundInfo.RoundID;
				
				CRoundAndDate RoundAndDate = new CRoundAndDate()
				{
					Name = RoundInfo.RoundName.Replace('_', ' ')
				};

				if (RoundDates == null)
					RoundAndDate.Date = sender.Value.StartDate.Date.ToLongDateString();
				else
					RoundAndDate.Date = RoundDates.First(arg => arg.Key == RoundInfo.RoundName).Value;
				GroupRounds.Add(RoundID, new CKeyValuePairEx<byte, CRoundAndDate>(RoundID, RoundAndDate, RoundCommamdHandler));
			}

			// Проверяем, расставлены ли итоговые места у всех участников
			int? RoundFinishedFlags = DBManagerApp.m_Entities.groups.First(arg => arg.id_group == sender.Key).round_finished_flags;

			//if (RoundFinishedFlags.HasValue && GlobalDefines.IsRoundFinished(RoundFinishedFlags.Value, enRounds.Final))
			{	// Итоговый протокол сформирован => добавляем его в список
				CRoundAndDate RoundAndDate = new CRoundAndDate()
				{
					Name = GlobalDefines.TOTAL_NODE_NAME.Replace('_', ' '),
				};
				RoundAndDate.Date = GlobalDefines.CreateCompDate(sender.Value.StartDate,
																	sender.Value.EndDate == null ? (DateTime?)null : sender.Value.EndDate.Date);
				GroupRounds.Add((byte)enRounds.Total,
								new CKeyValuePairEx<byte, CRoundAndDate>((byte)enRounds.Total, RoundAndDate, RoundCommamdHandler));
			}
			
			byte CurSelectedRound = CurrentRounds.SelectedKey;
			CurrentRounds.Clear();
			CurrentRounds.AddRange(GroupRounds);

			if (CurrentRounds.Count > 0)
			{	// Выбираем первый раунд
				if (!GroupChanged && CurrentRounds.ContainsKey(CurSelectedRound))
					CurrentRounds.SelectedKey = CurSelectedRound;
				else
					CurrentRounds.SelectedKey = CurrentRounds.Keys.First();

				// Общее число участников
				RightPanel.WholeMembersQ = (from part in DBManagerApp.m_Entities.participations
											where part.Group == CurrentGroups.SelectedKey
											select part.id_participation).Count();

				CurrentRounds.SelectedItem.Command.DoExecute();
			}
			else
				RightPanel.ClearTemplate();
		}


		void RoundCommamdHandler(CKeyValuePairEx<byte, CRoundAndDate> sender)
		{
			//GlobalDefines.m_swchGlobal.Restart();
			ResetFilters(); // Очищаем все фильтры при переходе к новому раунду

			CurrentRounds.SelectedKey = sender.Key;

			rmbtnRound.Label = sender.Value.Name;
			m_MembersInLeftGrid = -1;
									
			enRounds SelectedRound = (enRounds)CurrentRounds.SelectedKey;

			lblRoundDate.Content = sender.Value.Date;

			foreach (string Name in m_NamesToUnregister)
				UnregisterName(Name);
			m_NamesToUnregister.Clear();

			if (IsTotal)
			{
#if TICKER
				tckrMembersOnStart.Visibility = Visibility.Collapsed;
#endif
				scrlvwrAdditionalDataGridHeader.Visibility = Visibility.Visible;
				
				lblRoundName.Content = string.Format("{0} - {1}.",
														CurrentGroups.SelectedItem.Value.AgeGroup.FullGroupName,
														Properties.Resources.resSpeed);
																
				List<results_speed> AllGroupResultsInDB = (from part in DBManagerApp.m_Entities.participations
														   join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
														   where part.Group == CurrentGroups.SelectedKey
														   select result).ToList();

				// Последние участники в каждом раунде. id_participation.
				// Из просмотра исключаем итоговый протокол, квалификацию и полуфинал, т.к. они нам тут не нужны
				Dictionary<byte, long> LastRoundMembers = new Dictionary<byte, long>();
				foreach (byte RoundId in CurrentRounds.Keys.Where(arg => arg != (byte)enRounds.Total && arg != (byte)enRounds.Qualif && arg != (byte)enRounds.SemiFinal))
				{
					byte? MaxRoundPlace = AllGroupResultsInDB.Where(arg => arg.round == RoundId).Max(arg => arg.place);
					if (MaxRoundPlace.HasValue)
					{	// Ищем всех участников с такими местами, т.к. их может быть несколько, например в квалификации
						IEnumerable<results_speed> MembersWithMaxPlaces = AllGroupResultsInDB.Where(arg => arg.round == RoundId && arg.place.HasValue && arg.place == MaxRoundPlace);
						// Выбираем из них того, который имеет максимальный стартовый номер
						results_speed LastRoundMember = MembersWithMaxPlaces.FirstOrDefault(arg => arg.number == MembersWithMaxPlaces.Max(arg1 => arg1.number));
						if (LastRoundMember != null)
							LastRoundMembers.Add(RoundId, LastRoundMember.participation);
					}
				}
				// В финале и полуфинале последний человек в итоговом протоколе один и тот же
				if (LastRoundMembers.ContainsKey((byte)enRounds.Final))
					LastRoundMembers.Add((byte)enRounds.SemiFinal, LastRoundMembers[(byte)enRounds.Final]);

				m_CurrentRoundMembers = (from member in DBManagerApp.m_Entities.members
										 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
										 where part.Group == CurrentGroups.SelectedKey
										 orderby part.result_place
										 select new CMemberInTotal
										 {
											 MemberInfo = new CFullMemberInfo()
											 {
												 IDMember = member.id_member,
												 Surname = member.surname,
												 Name = member.name,
												 YearOfBirth = member.year_of_birth,
												 Coach = part.coach,
												 Team = part.team,
												 InitGrade = part.init_grade,
											 },

											 TotalGrade = part.result_grade,
											 Place = part.result_place,
											 id_part = part.id_participation,
										 }).ToList();

				// Перебираем всех участников соревнования
				foreach (CMemberInTotal MemberInTotal in m_CurrentRoundMembers)
				{
					if (CurrentGroups.SelectedItem.Value.SecondColNameType == enSecondColNameType.Coach)
						MemberInTotal.MemberInfo.SecondCol = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == MemberInTotal.MemberInfo.Coach).name;
					else
						MemberInTotal.MemberInfo.SecondCol = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == MemberInTotal.MemberInfo.Team).name;

					// Заносим результат всех раундов для участника
					IEnumerable<results_speed> CurMemberResults = AllGroupResultsInDB.Where(arg => arg.participation == MemberInTotal.id_part);
					foreach (results_speed MemberResult in CurMemberResults)
					{
						long LastRoundMemberPart = -1;
						if (!LastRoundMembers.TryGetValue(MemberResult.round, out LastRoundMemberPart))
							LastRoundMemberPart = -1;

						MemberInTotal.SetResultsForRound(MemberResult.round,
														new COneRoundResults()
														{
															m_Round = (enRounds)MemberResult.round,
															Route1 = new CResult()
															{
																ResultColumnNumber = enResultColumnNumber.Route1,
																CondFormating = (enCondFormating?)MemberResult.cond_formating_1,
																AdditionalEventTypes = (enAdditionalEventTypes?)MemberResult.event_1,
																Time = MemberResult.route1,
																ResultPossible = true
															},
															Route2 = new CResult()
															{
																ResultColumnNumber = enResultColumnNumber.Route2,
																CondFormating = (enCondFormating?)MemberResult.cond_formating_2,
																AdditionalEventTypes = (enAdditionalEventTypes?)MemberResult.event_2,
																Time = MemberResult.route2,
																ResultPossible = true
															},
															Sum = new CResult()
															{
																ResultColumnNumber = enResultColumnNumber.Sum,
																CondFormating = (enCondFormating?)MemberResult.cond_formating_sum,
																AdditionalEventTypes = (enAdditionalEventTypes?)MemberResult.event_sum,
																Time = MemberResult.sum,
																ResultPossible = true
															},
															IsLastMember = MemberInTotal.id_part == LastRoundMemberPart
														});

						if (MemberResult.round == CurMemberResults.Max(arg => arg.round))
							MemberInTotal.BallsForPlaces = MemberResult.balls;
					}

					// Отмечаем выбывших участников. Тех, кто выбыл в финале не отмечаем
					IEnumerable<COneRoundResults> Loosers = MemberInTotal.AllFilledResults.Where(arg => arg.m_Round != enRounds.Final);
					if (Loosers.Count() > 0)
						Loosers.Last().IsLooser = true;
				}
			}
			else
			{
				enRounds PrevRound = enRounds.None;

#if TICKER
				tckrMembersOnStart.Visibility = Visibility.Visible;
#endif
				scrlvwrAdditionalDataGridHeader.Visibility = Visibility.Collapsed;

				lblRoundName.Content = string.Format("{0} - {1}. {2}",
														CurrentGroups.SelectedItem.Value.AgeGroup.FullGroupName,
														Properties.Resources.resSpeed,
														sender.Value.Name);

#if TICKER
				string TickerText = "";
#endif

				if (SelectedRound >= enRounds.OneEighthFinal && SelectedRound <= enRounds.Final)
				{
					if (CurrentRounds.SelectedKey ==
						DBManagerApp.m_Entities.groups.First(arg => arg.id_group == CurrentGroups.SelectedKey).round_after_qualif)
					{	// Текущий раунд идёт за квалификацией => предыдущим раундом была квалификация
						PrevRound = enRounds.Qualif;
					}
					else
					{
						List<byte> RoundIds = CurrentRounds.Keys.ToList();
						RoundIds.Sort();
						PrevRound = (enRounds)(RoundIds[RoundIds.IndexOf(CurrentRounds.SelectedKey) - 1]);
					}
				}

				falsestarts_rules RuleForCurRound = (from rule in DBManagerApp.m_Entities.falsestarts_rules
													 where rule.Group == CurrentGroups.SelectedKey
															 && rule.start_round <= CurrentRounds.SelectedKey
															 && CurrentRounds.SelectedKey <= rule.end_round
													 select rule).FirstOrDefault();
				byte StartRoundForFalsestarts = RuleForCurRound == null ? CurrentRounds.SelectedKey : RuleForCurRound.start_round;
				byte EndRoundForFalsestarts = RuleForCurRound == null ? CurrentRounds.SelectedKey : RuleForCurRound.end_round;

				List<members> MembersWithFalsestarts = (from member in DBManagerApp.m_Entities.members
														join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
														join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
														where result.round >= StartRoundForFalsestarts
																 && result.round <= EndRoundForFalsestarts
																 && part.Group == CurrentGroups.SelectedKey
																 && ((result.event_1.HasValue && ((result.event_1.Value & (long)enAdditionalEventTypes.Falsestart) != 0))
																	 || (result.event_2.HasValue && ((result.event_2.Value & (long)enAdditionalEventTypes.Falsestart) != 0)))
														select member).ToList();

				// Список участников раунда со всей необходимой информацией 
				m_CurrentRoundMembers = (from member in DBManagerApp.m_Entities.members
										 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
										 join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
										 where result.round == CurrentRounds.SelectedKey && part.Group == CurrentGroups.SelectedKey
										 select new CMemberAndResults
										 {
											 MemberInfo = new CFullMemberInfo()
											 {
												 IDMember = member.id_member,
												 Surname = member.surname,
												 Name = member.name,
												 YearOfBirth = member.year_of_birth,
												 Coach = part.coach,
												 Team = part.team,
												 InitGrade = part.init_grade,
											 },

											 Results = new COneRoundResults()
											 {
												 m_Round = (enRounds)result.round,
												 Route1 = new CResult()
												 {
													 ResultColumnNumber = enResultColumnNumber.Route1,
													 CondFormating = (enCondFormating?)result.cond_formating_1,
													 AdditionalEventTypes = (enAdditionalEventTypes?)result.event_1,
													 Time = result.route1,
												 },
												 Route2 = new CResult()
												 {
													 ResultColumnNumber = enResultColumnNumber.Route2,
													 CondFormating = (enCondFormating?)result.cond_formating_2,
													 AdditionalEventTypes = (enAdditionalEventTypes?)result.event_2,
													 Time = result.route2,
												 },
												 Sum = new CResult()
												 {
													 ResultColumnNumber = enResultColumnNumber.Sum,
													 CondFormating = (enCondFormating?)result.cond_formating_sum,
													 AdditionalEventTypes = (enAdditionalEventTypes?)result.event_sum,
													 Time = result.sum,
												 },
											 },

											 StartNumber = result.number,
											 Place = result.place,
											 id_part = result.participation
										 }).ToList();

				IEnumerable<results_speed> PrevRoundResults = null;
				if (PrevRound != enRounds.None)
				{
					PrevRoundResults = from result in DBManagerApp.m_Entities.results_speed
									   join part in DBManagerApp.m_Entities.participations on result.participation equals part.id_participation
									   where part.Group == CurrentGroups.SelectedKey && result.round == (byte)PrevRound
									   select result;
				}
				
				// В основном запросе заполнить эти поля почему-то не получилось
				foreach (CMemberAndResults item in m_CurrentRoundMembers)
				{
					if (PrevRoundResults != null)
					{
						results_speed PrevRoundResult = PrevRoundResults.FirstOrDefault(arg => arg.participation == item.id_part);
						if (PrevRoundResult != null)
							item.PrevNumber = PrevRoundResult.place;
					}

					if (CurrentGroups.SelectedItem.Value.SecondColNameType == enSecondColNameType.Coach)
						item.MemberInfo.SecondCol = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == item.MemberInfo.Coach).name;
					else
						item.MemberInfo.SecondCol = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == item.MemberInfo.Team).name;

#if TICKER
					if ((item.Results.Route1.CondFormating.HasValue && item.Results.Route1.CondFormating.Value == enCondFormating.JustRecievedResult) ||
						(item.Results.Route2.CondFormating.HasValue && item.Results.Route2.CondFormating.Value == enCondFormating.JustRecievedResult) ||
						(item.Results.Sum.CondFormating.HasValue && item.Results.Sum.CondFormating.Value == enCondFormating.JustRecievedResult))
					{
						TickerText += item.StringForTicker() + "\t";
					}
#endif

					item.HasFalsestart = MembersWithFalsestarts.Exists(arg => arg.id_member == item.MemberInfo.IDMember);
				}

				switch (SelectedRound)
				{
					case enRounds.Qualif:
					case enRounds.Qualif2:
						break;

					case enRounds.OneEighthFinal:
					case enRounds.QuaterFinal:
					case enRounds.SemiFinal:
					case enRounds.Final:
						// Первые участники пар
						List<CMemberAndResults> lstFirstMembersInPairs = (from RoundMembers in m_CurrentRoundMembers.OfType<CMemberAndResults>()
																		  where RoundMembers.StartNumber.Value % 2 == 1
																		  orderby RoundMembers.StartNumber
																		  select RoundMembers).ToList();
						// Вторые участники пар
						List<CMemberAndResults> lstSecondMembersInPairs = (from RoundMembers in m_CurrentRoundMembers.OfType<CMemberAndResults>()
																		   where RoundMembers.StartNumber.Value % 2 == 0
																		   orderby RoundMembers.StartNumber
																		   select RoundMembers).ToList();
						// Пары
						List<CMembersPair> lstPairs = new List<CMembersPair>(lstFirstMembersInPairs.Count / 2);

						// Разбиваем участников на пары
						for (int i = 0; i < lstFirstMembersInPairs.Count; i++)
						{
							lstPairs.Add(new CMembersPair()
							{
								First = lstFirstMembersInPairs[i],
								Second = lstSecondMembersInPairs[i]
							});
						}

						m_CurrentRoundMembers = lstPairs;
						break;
				}

#if TICKER
				tckrMembersOnStart.TickerText = string.IsNullOrWhiteSpace(TickerText) ? null : TickerText.Left(TickerText.Length - 1);
#endif
			}

			dgrdRoundMembers.Columns.Clear();
			DataGridColumn[] columns = null; 
			
			CMemberAndResultsComparer Comparer1 = new CMemberAndResultsComparer();
			CMemberAndResultsComparer Comparer2 = new CMemberAndResultsComparer();
			switch (SelectedRound)
			{
				case enRounds.None:
					RightPanel.ClearTemplate();
					ShowRightDataGrid(false);
					break;
				
				case enRounds.Qualif:
					QualifFinished = GlobalDefines.IsRoundFinished(DBManagerApp.m_Entities.groups.First(arg => arg.id_group == CurrentGroups.SelectedKey).round_finished_flags,
																	enRounds.Qualif);
					if ((int)CurrentGroups[CurrentGroups.SelectedKey].Value.MembersFrom1stQualif != GlobalDefines.DEFAULT_XML_BYTE_VAL)
						MembersFromQualif = RightPanel.NextRoundMembersQ = CurrentGroups[CurrentGroups.SelectedKey].Value.MembersFrom1stQualif;
					else
						MembersFromQualif = RightPanel.NextRoundMembersQ = 0;
					RightPanel.Template = m_RightPanelTemplates["QualifRightPanel"] as ControlTemplate;
					if (QualifFinished)
					{
						Comparer1.CompareProperty = CMemberAndResultsComparer.enCompareProperty.Place;
						m_CurrentRoundMembers = m_CurrentRoundMembers.OfType<CMemberAndResults>().OrderBy(n => n, Comparer1);
						RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					}
					else
					{
						// В квалификации нужно отсортировать сначала по возрастанию времён, а для тех,
						// у кого результата нет => по возрастанию номеров, чтобы вначале отображались те, у кого уже есть результат
						Comparer1.CompareProperty = CMemberAndResultsComparer.enCompareProperty.Sum;
						Comparer2.CompareProperty = CMemberAndResultsComparer.enCompareProperty.StartNumber;
						m_CurrentRoundMembers = m_CurrentRoundMembers.OfType<CMemberAndResults>().OrderBy(m => m, Comparer1).ThenBy(n => n, Comparer2);

						IEnumerable<CMemberAndResults> MembersToHighlight = from member in m_CurrentRoundMembers.OfType<CMemberAndResults>()
																			where member.Results.Route1.CondFormating != null &&
																					(member.Results.Route1.CondFormating.Value == enCondFormating.StayOnStart ||
																					member.Results.Route1.CondFormating.Value == enCondFormating.Preparing)
																			select member;
						CMemberAndResults Member = MembersToHighlight.FirstOrDefault(arg => arg.Results.Route1.CondFormating.Value == enCondFormating.StayOnStart);
						if (Member != null)
						{
							RightPanel.InvitedToStartMember = string.Format("{0}. {1} {2} {3}",
																			Member.StartNumber.HasValue ?  Member.StartNumber.Value.ToString() : "",
																			Member.MemberInfo.SurnameAndName,
																			Member.MemberInfo.YearOfBirthForShow,
																			Member.MemberInfo.SecondCol);
						}
						else
						{
							RightPanel.InvitedToStartMember = null;
						}
								
						Member = MembersToHighlight.FirstOrDefault(arg => arg.Results.Route1.CondFormating.Value == enCondFormating.Preparing);
						if (Member != null)
						{
							RightPanel.PreparingMember = string.Format("{0}. {1} {2} {3}",
																			Member.StartNumber.HasValue ? Member.StartNumber.Value.ToString() : "",
																			Member.MemberInfo.SurnameAndName,
																			Member.MemberInfo.YearOfBirthForShow,
																			Member.MemberInfo.SecondCol);
						}
						else
						{
							RightPanel.PreparingMember = null;
						}
					}
										
					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count();	// Число участников в раунде

					foreach (CMemberAndResults item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 25.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsQualifStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 3;
						columns = Resources["QualifColumns"] as DataGridColumn[];

						dgrdRoundMembers2.RowHeight = dgrdRoundMembers.FontSize * 25.0 / 14.0;
					}
					
					grdRoundMembersHost_SizeChanged(grdRoundMembersHost, null);
					break;
				
				case enRounds.Qualif2:
					QualifFinished = GlobalDefines.IsRoundFinished(DBManagerApp.m_Entities.groups.First(arg => arg.id_group == CurrentGroups.SelectedKey).round_finished_flags,
																	enRounds.Qualif2);
					if (CurrentGroups[CurrentGroups.SelectedKey].Value.MembersFrom2ndQualif != GlobalDefines.DEFAULT_XML_BYTE_VAL)
						MembersFromQualif = RightPanel.NextRoundMembersQ = CurrentGroups[CurrentGroups.SelectedKey].Value.MembersFrom2ndQualif;
					else
						MembersFromQualif = RightPanel.NextRoundMembersQ = 0;
					RightPanel.Template = m_RightPanelTemplates["QualifRightPanel"] as ControlTemplate;
					if (QualifFinished)
					{
						Comparer1.CompareProperty = CMemberAndResultsComparer.enCompareProperty.Place;
						m_CurrentRoundMembers = m_CurrentRoundMembers.OfType<CMemberAndResults>().OrderBy(n => n, Comparer1);
						RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					}
					else
					{
						// В квалификации нужно отсортировать сначала по возрастанию времён, а для тех,
						// у кого результата нет => по возрастанию номеров, чтобы вначале отображались те, у кого уже есть результат
						Comparer1.CompareProperty = CMemberAndResultsComparer.enCompareProperty.Sum;
						Comparer2.CompareProperty = CMemberAndResultsComparer.enCompareProperty.StartNumber;
						m_CurrentRoundMembers = m_CurrentRoundMembers.OfType<CMemberAndResults>().OrderBy(m => m, Comparer1).ThenBy(n => n, Comparer2);

						IEnumerable<CMemberAndResults> MembersToHighlight = from member in m_CurrentRoundMembers.OfType<CMemberAndResults>()
																			where member.Results.Route1.CondFormating != null &&
																					(member.Results.Route1.CondFormating.Value == enCondFormating.StayOnStart ||
																					member.Results.Route1.CondFormating.Value == enCondFormating.Preparing)
																			select member;
						CMemberAndResults Member = MembersToHighlight.FirstOrDefault(arg => arg.Results.Route1.CondFormating.Value == enCondFormating.StayOnStart);
						if (Member != null)
						{
							RightPanel.InvitedToStartMember = string.Format("{0}. {1} {2} {3}",
																			Member.StartNumber.HasValue ? Member.StartNumber.Value.ToString() : "",
																			Member.MemberInfo.SurnameAndName,
																			Member.MemberInfo.YearOfBirthForShow,
																			Member.MemberInfo.SecondCol);
						}
						else
							RightPanel.InvitedToStartMember = null;

						Member = MembersToHighlight.FirstOrDefault(arg => arg.Results.Route1.CondFormating.Value == enCondFormating.Preparing);
						if (Member != null)
						{
							RightPanel.PreparingMember = string.Format("{0}. {1} {2} {3}",
																			Member.StartNumber.HasValue ? Member.StartNumber.Value.ToString() : "",
																			Member.MemberInfo.SurnameAndName,
																			Member.MemberInfo.YearOfBirthForShow,
																			Member.MemberInfo.SecondCol);
						}
						else
							RightPanel.PreparingMember = null;
					}

					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count();	// Число участников в раунде

					foreach (CMemberAndResults item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 25.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsQualifStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 4;
						columns = Resources["QualifColumns"] as DataGridColumn[];

						dgrdRoundMembers2.RowHeight = dgrdRoundMembers.FontSize * 25.0 / 14.0;
					}

					grdRoundMembersHost_SizeChanged(grdRoundMembersHost, null);
					break;
				
				case enRounds.OneEighthFinal:
					RightPanel.NextRoundMembersQ = 8;
					RightPanel.Template = m_RightPanelTemplates["MiddleRoundsRightPanel"] as ControlTemplate;
					RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					// Пары уже отсортированы при добавлении их в m_CurrentRoundMembers

					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count() * 2;	// Число участников в раунде

					foreach (CMembersPair item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						ShowRightDataGrid(false);

						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 50.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsMiddleSheetsStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 4;
						columns = Resources["MiddleSheetsColumns"] as DataGridColumn[];
					}
					break;
				
				case enRounds.QuaterFinal:
					RightPanel.NextRoundMembersQ = 4;
					RightPanel.Template = m_RightPanelTemplates["MiddleRoundsRightPanel"] as ControlTemplate;
					RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					// Пары уже отсортированы при добавлении их в m_CurrentRoundMembers

					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count() * 2;	// Число участников в раунде

					foreach (CMembersPair item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						ShowRightDataGrid(false);

						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 50.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsMiddleSheetsStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 4;
						columns = Resources["MiddleSheetsColumns"] as DataGridColumn[];
					}
					break;
				
				case enRounds.SemiFinal:
					RightPanel.NextRoundMembersQ = 4;
					RightPanel.Template = m_RightPanelTemplates["MiddleRoundsRightPanel"] as ControlTemplate;
					RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					// Пары уже отсортированы при добавлении их в m_CurrentRoundMembers

					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count() * 2;	// Число участников в раунде

					foreach (CMembersPair item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						ShowRightDataGrid(false);

						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 50.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsMiddleSheetsStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 4;
						columns = Resources["MiddleSheetsColumns"] as DataGridColumn[];
					}
					break;
				
				case enRounds.Final:
					RightPanel.NextRoundMembersQ = 0;
					RightPanel.Template = m_RightPanelTemplates["FinalRightPanel"] as ControlTemplate;
					RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
					// Пары уже отсортированы при добавлении их в m_CurrentRoundMembers

					RightPanel.RoundMembersQ = m_CurrentRoundMembers.Count() * 2;	// Число участников в раунде

					foreach (CMembersPair item in m_CurrentRoundMembers)
						item.RefreshColors();

					if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
					{
						ShowRightDataGrid(false);

						dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 50.0 / 14.0;
						dgrdRoundMembers.Style = Resources["RoundResultsFinalStyle"] as Style;
						dgrdRoundMembers.FrozenColumnCount = 2;
						columns = Resources["FinalColumns"] as DataGridColumn[];
					}
					break;

				case enRounds.Total:
					{
						if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey)
						{
							spAdditionalDataGridHeader.Children.RemoveRange(1, spAdditionalDataGridHeader.Children.Count - 2);

							IEnumerable<byte> RoundsWithoutTotal = CurrentRounds.Keys.Where(arg => arg != (byte)enRounds.Total);

							RightPanel.NextRoundMembersQ = 0;
							RightPanel.Template = m_RightPanelTemplates["TotalRightPanel"] as ControlTemplate;
							RightPanel.InvitedToStartMember = RightPanel.PreparingMember = null;
							// Участники уже были отсортированы при их добавлении в m_CurrentRoundMembers

							ShowRightDataGrid(false);

							dgrdRoundMembers.RowHeight = dgrdRoundMembers.FontSize * 25.0 / 14.0;
							dgrdRoundMembers.Style = Resources["RoundResultsTotalStyle"] as Style;
							dgrdRoundMembers.FrozenColumnCount = 5;

							// Добавляем колонки в массив
							columns = new DataGridColumn[RoundsWithoutTotal.Count() * 3 + 7];

							int ColNumber = 0;

							columns[ColNumber] = Resources["TotalColumnsPlace"] as DataGridColumn;
							RegisterName("TotalColumnsPlace", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsPlace");
							ColNumber++;

							columns[ColNumber] = Resources["TotalColumnsSurnameAndName"] as DataGridColumn;
							RegisterName("TotalColumnsSurnameAndName", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsSurnameAndName");
							ColNumber++;

							columns[ColNumber] = Resources["TotalColumnsSecondCol"] as DataGridColumn;
							RegisterName("TotalColumnsSecondCol", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsSecondCol");
							ColNumber++;

							columns[ColNumber] = Resources["TotalColumnsYearOfBirth"] as DataGridColumn;
							RegisterName("TotalColumnsYearOfBirth", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsYearOfBirth");
							ColNumber++;

							columns[ColNumber] = Resources["TotalColumnsInitGrade"] as DataGridColumn;
							RegisterName("TotalColumnsInitGrade", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsInitGrade");
							ColNumber++;

							MultiBinding bindWidth = new MultiBinding()
							{
								Converter = new ResultsExtraHeaderWidthMarkupConverter()
							};
							bindWidth.Bindings.Add(new Binding("ActualWidth")
								{
									ElementName = "TotalColumnsPlace"
								});
							bindWidth.Bindings.Add(new Binding("ActualWidth")
								{
									ElementName = "TotalColumnsSurnameAndName"
								});
							bindWidth.Bindings.Add(new Binding("ActualWidth")
							{
								ElementName = "TotalColumnsSecondCol"
							});
							bindWidth.Bindings.Add(new Binding("ActualWidth")
								{
									ElementName = "TotalColumnsYearOfBirth"
								});
							bindWidth.Bindings.Add(new Binding("ActualWidth")
							{
								ElementName = "TotalColumnsInitGrade"
							});
							BindingOperations.SetBinding(lblLeftDummy, Label.WidthProperty, bindWidth);

							Setter TriggerSetter = new Setter(DataGridCell.BorderThicknessProperty, new Thickness(0));
							Setter Trigger2Setter = new Setter(RoundResultsAttachedProps.ExtraBorderBrushProperty, Brushes.Transparent);
							Setter SumTriggerSetter = new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1, 0, 1, 1));
							Setter SumTrigger2Setter = new Setter(DataGridCell.MarginProperty, new Thickness(0, 0, -0.5, 0));
							Setter SumTrigger3Setter = new Setter(DataGridCell.BorderThicknessProperty, new Thickness(1, 0, 1, 3));
							foreach (byte RoundId in RoundsWithoutTotal.OrderBy(arg => arg))
							{
								string RoundEnumName = Enum.GetName(typeof(enRounds), (enRounds)RoundId);
								DataTrigger trig = null;

								// Трасса 1
								Style ResultCellStyle = new Style()
								{
									BasedOn = Resources["RndResDataGridCellTotalStyle"] as Style,
									TargetType = typeof(DataGridCell),
								};
								trig = new DataTrigger()
								{
									Binding = new Binding(string.Format("{0}Results", RoundEnumName))
									{
										Converter = new HasResultMarkupConverter(),
										ConverterParameter = enResultColumnNumber.Route1
									},
									Value = false,
								};
								trig.Setters.Add(TriggerSetter);
								trig.Setters.Add(Trigger2Setter);
								ResultCellStyle.Triggers.Add(trig);

								columns[ColNumber] = new DataGridTextColumn()
								{
									Header = Properties.Resources.resRoute1,
									Binding = new Binding(string.Format("{0}Results.Route1.ResultForShow", RoundEnumName)),
									CellStyle = ResultCellStyle
								};
								RegisterName(RoundEnumName + "Route1", columns[ColNumber]);
								m_NamesToUnregister.Add(RoundEnumName + "Route1");
								ColNumber++;


								// Трасса 2
								ResultCellStyle = new Style()
								{
									BasedOn = Resources["RndResDataGridCellTotalStyle"] as Style,
									TargetType = typeof(DataGridCell),
								};
								trig = new DataTrigger()
								{
									Binding = new Binding(string.Format("{0}Results", RoundEnumName))
									{
										Converter = new HasResultMarkupConverter(),
										ConverterParameter = enResultColumnNumber.Route2
									},
									Value = false,
								};
								trig.Setters.Add(TriggerSetter);
								trig.Setters.Add(Trigger2Setter);
								ResultCellStyle.Triggers.Add(trig);

								columns[ColNumber] = new DataGridTextColumn()
								{
									Header = Properties.Resources.resRoute2,
									Binding = new Binding(string.Format("{0}Results.Route2.ResultForShow", RoundEnumName)),
									CellStyle = ResultCellStyle
								};
								RegisterName(RoundEnumName + "Route2", columns[ColNumber]);
								m_NamesToUnregister.Add(RoundEnumName + "Route2");
								ColNumber++;


								// Сумма
								ResultCellStyle = new Style()
								{
									BasedOn = Resources["RndResDataGridCellTotalStyle"] as Style,
									TargetType = typeof(DataGridCell),
								};
								trig = new DataTrigger()
								{
									Binding = new Binding(string.Format("{0}Results", RoundEnumName))
									{
										Converter = new HasResultMarkupConverter(),
										ConverterParameter = enResultColumnNumber.Sum
									},
									Value = false,
								};
								trig.Setters.Add(TriggerSetter);
								trig.Setters.Add(Trigger2Setter);
								ResultCellStyle.Triggers.Add(trig);
								trig = new DataTrigger()
								{
									Binding = new Binding(string.Format("{0}Results.IsLooser", RoundEnumName)),
									Value = true,
								};
								trig.Setters.Add(SumTriggerSetter);
								trig.Setters.Add(SumTrigger2Setter);
								ResultCellStyle.Triggers.Add(trig);
								MultiDataTrigger multitrig = new MultiDataTrigger();
								multitrig.Conditions.Add(new Condition(new Binding(string.Format("{0}Results.IsLooser", RoundEnumName)),
																		true));
								multitrig.Conditions.Add(new Condition(new Binding(string.Format("{0}Results.IsLastMember", RoundEnumName)),
																		true));
								multitrig.Setters.Add(SumTrigger3Setter);
								ResultCellStyle.Triggers.Add(multitrig);

								columns[ColNumber] = new DataGridTextColumn()
								{
									Header = Properties.Resources.resSum,
									Binding = new Binding(string.Format("{0}Results.Sum.ResultForShow", RoundEnumName)),
									CellStyle = ResultCellStyle
								};
								RegisterName(RoundEnumName + "Sum", columns[ColNumber]);
								m_NamesToUnregister.Add(RoundEnumName + "Sum");
								ColNumber++;


								// Заголовок для трёх столбцов раунда
								Label lblExtraResultsHeader = new Label()
								{
									Content = GlobalDefines.ROUND_NAMES[RoundId].Replace('_', ' '),
									HorizontalContentAlignment = HorizontalAlignment.Center,
									BorderBrush = Resources["DataGridLinesBrush"] as SolidColorBrush,
									BorderThickness = new Thickness(1, 1, 1, 0),
									Padding = new Thickness(2, 2, 2, 2)
								};
								spAdditionalDataGridHeader.Children.Insert(spAdditionalDataGridHeader.Children.Count - 1, lblExtraResultsHeader);

								bindWidth = new MultiBinding()
								{
									Converter = new ResultsExtraHeaderWidthMarkupConverter()
								};
								bindWidth.Bindings.Add(new Binding("ActualWidth")
									{
										ElementName = RoundEnumName + "Route1"
									});
								bindWidth.Bindings.Add(new Binding("ActualWidth")
									{
										ElementName = RoundEnumName + "Route2"
									});
								bindWidth.Bindings.Add(new Binding("ActualWidth")
									{
										ElementName = RoundEnumName + "Sum"
									});
								BindingOperations.SetBinding(lblExtraResultsHeader, Label.WidthProperty, bindWidth);
							}

							columns[ColNumber] = Resources["TotalColumnsTotalGrade"] as DataGridColumn;
							RegisterName("TotalColumnsTotalGrade", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsTotalGrade");
							ColNumber++;

							columns[ColNumber] = Resources["TotalColumnsBalls"] as DataGridColumn;
							RegisterName("TotalColumnsBalls", columns[ColNumber]);
							m_NamesToUnregister.Add("TotalColumnsBalls");
							ColNumber++;

							bindWidth = new MultiBinding()
							{
								Converter = new ResultsExtraHeaderWidthMarkupConverter()
							};
							bindWidth.Bindings.Add(new Binding("ActualWidth")
							{
								ElementName = "TotalColumnsTotalGrade"
							});
							bindWidth.Bindings.Add(new Binding("ActualWidth")
							{
								ElementName = "TotalColumnsBalls"
							});
							BindingOperations.SetBinding(lbRightDummy, Label.WidthProperty, bindWidth);
						}
						break;
					}
			}

			if (CurrentRounds.PrevSelectedKey != CurrentRounds.SelectedKey && columns != null)
			{
				foreach (DataGridColumn item in columns)
					dgrdRoundMembers.Columns.Add(item);
			}

			collectionCurrentRoundMembers.ReplaceRange(m_CurrentRoundMembers);
			m_lstFilteredMembers = m_CurrentRoundMembers.ToList(); // Изначально ничего не отфильтровано 

			HighlightTypes[0].Command.DoExecute(); // При смене раундов подсветку разрядов выключаем
			OnPropertyChanged(CalcGradesEnabledPropertyName);

			// Таймер нужен, чтобы успело пересчитаться свойство m_svwrDataGrid/m_svwrDataGrid2.ComputedVerticalScrollBarVisibility
			DispatcherTimer tmrHack = new DispatcherTimer()
			{
				Interval = new TimeSpan(0, 0, 1)
			};
			tmrHack.Tick += (s, ev) =>
			{
				tmrHack.Stop();
				RefreshScrollingOffsets();
			};
			tmrHack.Start();

			//GlobalDefines.m_swchGlobal.Stop();
			//System.Diagnostics.Debug.WriteLine(GlobalDefines.m_swchGlobal.Elapsed.TotalSeconds);
		}


		void HighlightGradeTypeCommamdHandler(CKeyValuePairEx<enHighlightGradesType, string> sender)
		{
			mbtnHighlightGrades.Label = sender.Value;
			CurHighlightGradesType = sender.Key;
		}
		
								
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CTaskBarIconTuning.hWnd = (new WindowInteropHelper(this)).Handle;

			HwndSource ThisWndSource = HwndSource.FromHwnd(CTaskBarIconTuning.hWnd);
			ThisWndSource.AddHook(HwndMessageHook);

			/* Скрываем верхнюю строку Ribbon'a. Эта та, в которой располагаются клавиши быстрого доступа, но т.к. их у нас нет, то строка не нужна.
			 * Это действие нужно делать именно в событии Loaded, т.к. в конструкторе формы элементы интерфейса ещё не загружены, поэтому FindName вернёт null */
			(Ribbon.Template.FindName("11_T", Ribbon) as FrameworkElement).Visibility =
				(Ribbon.Template.FindName("titleBarBackground", Ribbon) as FrameworkElement).Visibility = System.Windows.Visibility.Collapsed;

			m_svwrDataGrid = GlobalDefines.GetChildScrollViewer(dgrdRoundMembers);
			m_svwrDataGrid2 = GlobalDefines.GetChildScrollViewer(dgrdRoundMembers2);

			dgrdRoundMembers2.Style = Resources["RoundResultsQualifStyle2"] as Style;
			dgrdRoundMembers2.FrozenColumnCount = 3;
			DataGridColumn[] columns = Resources["QualifColumns2"] as DataGridColumn[];

			if (columns != null)
				foreach (DataGridColumn item in columns)
					dgrdRoundMembers2.Columns.Add(item);

			RefreshScrollingOffsets();
		}

		
		private void rsmiAbout_Click(object sender, RoutedEventArgs e)
		{
			if (e.Source is RibbonSplitButton)
			{
				//CAboutWindow wnd = new CAboutWindow()
				//{
				//    Owner = this
				//};

				//try
				//{
				//    wnd.ShowDialog();
				//    e.Handled = true;
				//}
				//catch (Exception ex)
				//{
				//    DumpMaker.HandleExceptionAndClose(ex, Title);
				//    return;
				//}
			}
		}

		
		/// <summary>
		/// Показывать шапку листа
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void rchkShowGroupHead_Click(object sender, RoutedEventArgs e)
		{
			for (int row = 0; row < grdGroupHead.RowDefinitions.Count; row++)
			{
				if (row == 2)
				{	// Строка с названием раунда всегда будет видна
					continue;
				}
				
				if (rchkShowGroupHead.IsChecked.HasValue && rchkShowGroupHead.IsChecked.Value)
					grdGroupHead.RowDefinitions[row].Height = new GridLength(0, GridUnitType.Auto);
				else
					grdGroupHead.RowDefinitions[row].Height = new GridLength(0, GridUnitType.Pixel);
			}
		}


		private void dgrdRoundMembers_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (scrlvwrAdditionalDataGridHeader.Visibility == Visibility.Visible)
				scrlvwrAdditionalDataGridHeader.ScrollToHorizontalOffset(e.HorizontalOffset);
		}


		private void ShowRightDataGrid(bool Show)
		{
			if (Show)
			{
				if (grdspltrRoundMembers.Visibility != Visibility.Visible)
				{	/* Если правая поле не отображается, то показываем её,
					 * в противном случае - ничего не делаем, чтобы не менять выставленную ширину полей */ 
					grdRoundMembersHost.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
					grdRoundMembersHost.ColumnDefinitions[2].MinWidth = grdRoundMembersHost.ColumnDefinitions[0].MinWidth;
					grdspltrRoundMembers.Visibility = Visibility.Visible;
				}
			}
			else
			{
				if (grdspltrRoundMembers.Visibility != Visibility.Collapsed)
				{
					grdRoundMembersHost.ColumnDefinitions[2].MinWidth = 0;
					grdRoundMembersHost.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Pixel);
					grdspltrRoundMembers.Visibility = Visibility.Collapsed;
				}
			}
		}


		private void grdRoundMembersHost_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (m_CurrentRoundMembers == null)
				return;

			if ((e == null || e.HeightChanged) &&
				(enRounds)CurrentRounds.SelectedKey == enRounds.Qualif || (enRounds)CurrentRounds.SelectedKey == enRounds.Qualif2)
			{
				bool RightGridShown = false;
				List<CMemberAndResults> CurrentRoundMembers2 = new List<CMemberAndResults>();

				// Делаем так, чтобы в левом поле не было вертикальной полосы прогрутки
				int MembersInLeftGrid = (int)Math.Floor((GlobalDefines.GetActualControlHeight(grdRoundMembersHost) - dgrdRoundMembers.ColumnHeaderHeight - 5.0) /
														dgrdRoundMembers.RowHeight);
				if (MembersInLeftGrid < 0)
				{
					MembersInLeftGrid = 0;
					RightGridShown = false;
				}
				else
				{
					if (MembersInLeftGrid >= 0 && MembersInLeftGrid <= 3)
						MembersInLeftGrid = 3;	// Чтобы в левом Grid всегда было хотябы трое призёров

					RightGridShown = MembersInLeftGrid > 0 && MembersInLeftGrid < m_CurrentRoundMembers.Count();
				}
								
				if (m_MembersInLeftGrid == MembersInLeftGrid)
					return;

				m_MembersInLeftGrid = MembersInLeftGrid;

				ShowRightDataGrid(RightGridShown);

				if (m_CurrentRoundMembers != null)
				{
					for (int i = 0; i < m_CurrentRoundMembers.Count(); i++)
					{
						CMemberAndResults item = m_CurrentRoundMembers.ElementAt(i) as CMemberAndResults;
						if (i < m_MembersInLeftGrid || m_MembersInLeftGrid < 0)
						{
							item.VisibilityInMainTable = Visibility.Visible;
						}
						else
						{
							item.VisibilityInMainTable = Visibility.Collapsed;
							CurrentRoundMembers2.Add(item);
						}
					}

					if (RightGridShown)
						collectionCurrentRoundMembers2.ReplaceRange(CurrentRoundMembers2);
					else
						collectionCurrentRoundMembers2.Clear();
				}
			}
		}


		#region Автопрокрутка списка участников
		private void dgrdRoundMembers_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if ((enRounds)CurrentRounds.SelectedKey == enRounds.Total && e.HeightChanged)
			{
				RefreshScrollingOffsets();
			}
		}


		private void dgrdRoundMembers2_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (((enRounds)CurrentRounds.SelectedKey == enRounds.Qualif || (enRounds)CurrentRounds.SelectedKey == enRounds.Qualif2) &&
				e.HeightChanged)
			{
				RefreshScrollingOffsets();
			}
		}


		/// <summary>
		/// Обновляем список m_lstScrollingOffsets
		/// </summary>
		private void RefreshScrollingOffsets()
		{
			m_tmrAutoscroll.IsEnabled = false;

			m_lstScrollingOffsets.Clear();

			if (m_svwrDataGrid == null || m_svwrDataGrid2 == null || !rchkAutoscrollEnabled.IsChecked.Value)
				return;

			ScrollViewer svwr = null;
			if ((enRounds)CurrentRounds.SelectedKey == enRounds.Qualif || (enRounds)CurrentRounds.SelectedKey == enRounds.Qualif2)
				svwr = m_svwrDataGrid2;
			else
				svwr = m_svwrDataGrid;

			if (svwr.ComputedVerticalScrollBarVisibility == Visibility.Visible)
			{	// Прокрутка нужна только в том случае, если есть полоса прокрутки
				double ScrollOffset = 0;
				double ScrollPart = SCROLL_VIEWER_SCROLL_PART * svwr.ViewportHeight;
				int ViewportsQ = (int)Math.Floor(svwr.ExtentHeight / Math.Max(0.0001, ScrollPart));

				for (int i = 0; i < ViewportsQ; i++)
				{
					m_lstScrollingOffsets.Add(ScrollOffset);
					ScrollOffset += ScrollPart;
				}

				ScrollOffset += ScrollPart;
				if (ScrollOffset > svwr.ExtentHeight)
					m_lstScrollingOffsets.Add(svwr.ExtentHeight); // Чтобы точно докрутить до конца

				m_tmrAutoscroll.IsEnabled = rchkAutoscrollEnabled.IsChecked.Value;
				if (m_tmrAutoscroll.IsEnabled)
					m_tmrAutoscroll_Tick(m_tmrAutoscroll, EventArgs.Empty);
			}
		}

		
		private void rchkAutoscrollEnabled_CheckedUnchecked(object sender, RoutedEventArgs e)
		{
			m_tmrAutoscroll.IsEnabled = rchkAutoscrollEnabled.IsChecked.Value;
			RefreshScrollingOffsets();
		}


		private void m_tmrAutoscroll_Tick(object sender, EventArgs e)
		{
			if ((enRounds)CurrentRounds.SelectedKey == enRounds.Qualif || (enRounds)CurrentRounds.SelectedKey == enRounds.Qualif2)
			{
				if (m_lstScrollingOffsets.Count > 0 && m_svwrDataGrid2 != null)
					m_svwrDataGrid2.ScrollToVerticalOffset(m_lstScrollingOffsets.Next());
			}
			else
			{
				if (m_lstScrollingOffsets.Count > 0 && m_svwrDataGrid != null)
					m_svwrDataGrid.ScrollToVerticalOffset(m_lstScrollingOffsets.Next());
			}
		}
		#endregion


		#region Отправка данных на FTP
		private void rchkAutoSendToFTP_Click(object sender, RoutedEventArgs e)
		{
			if (rchkAutoSendToFTP.IsChecked.Value)
				m_FTPExporter.Start();
			else
				m_FTPExporter.Stop();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="DeferredExport">
		/// True - данные будут переданы на FTP не сразу, а будут помещены в очередь заданий на отправку
		/// </param>
		/// <param name="CompId"></param>
		/// <param name="RoundToSend"></param>
		/// <param name="AllGroupRounds"></param>
		/// <param name="Group"></param>
		/// <param name="GroupId"></param>
		/// <returns></returns>
		private bool SendRoundToFTP(bool DeferredExport,
									long CompId,
									enFTPSheetGeneratorTypes RoundToSend,
									List<enRounds> AllGroupRounds,
									CCompSettings Group,
									long GroupId)
		{
			CCompSpecificSets CompSets = null;
			CFTPGroupItemInSets FTPGroupItemInSets = null;
			CQueueItem Item = null;

			lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
			{
				if (!DBManagerApp.m_AppSettings.m_Settings.dictCompSettings.TryGetValue(CompId, out CompSets) ||
					!CompSets.dictGroupsForAutosendToFTP.TryGetValue(GroupId, out FTPGroupItemInSets) ||
					!FTPGroupItemInSets.CheckFTPWbkFullPath())
				{	// Настроек для группы нет => отправка на сервер невозможна
					return false;
				}

				string Dir = GlobalDefines.STD_FTP_WORKBOOKS_DIR + CompId.ToString() + "\\";
				if (!Directory.Exists(Dir))
					Directory.CreateDirectory(Dir);
								
				IEnumerable<CDBAdditionalClassBase> Members = null;
				if ((enRounds)RoundToSend == enRounds.Total)
				{
					List<results_speed> AllGroupResultsInDB = (from part in DBManagerApp.m_Entities.participations
															   join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
															   where part.Group == GroupId
															   select result).ToList();

					Members = (from member in DBManagerApp.m_Entities.members
							   join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
							   where part.Group == GroupId
							   orderby part.result_place
							   select new CMemberInTotal
							   {
								   MemberInfo = new CFullMemberInfo()
								   {
									   IDMember = member.id_member,
									   Surname = member.surname,
									   Name = member.name,
									   YearOfBirth = member.year_of_birth,
									   Coach = part.coach,
									   Team = part.team,
									   InitGrade = part.init_grade,
								   },

								   TotalGrade = part.result_grade,
								   Place = part.result_place,
								   id_part = part.id_participation,
							   }).ToList();
					// Перебираем всех участников соревнования
					foreach (CMemberInTotal MemberInTotal in Members)
					{
						if (Group.SecondColNameType == enSecondColNameType.Coach)
							MemberInTotal.MemberInfo.SecondCol = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == MemberInTotal.MemberInfo.Coach).name;
						else
							MemberInTotal.MemberInfo.SecondCol = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == MemberInTotal.MemberInfo.Team).name;

						// Заносим результат всех раундов для участника
						IEnumerable<results_speed> CurMemberResults = AllGroupResultsInDB.Where(arg => arg.participation == MemberInTotal.id_part);
						foreach (results_speed MemberResult in CurMemberResults)
						{
							MemberInTotal.SetResultsForRound(MemberResult.round,
															new COneRoundResults()
															{
																m_Round = (enRounds)MemberResult.round,
																Route1 = new CResult()
																{
																	ResultColumnNumber = enResultColumnNumber.Route1,
																	Time = MemberResult.route1,
																},
																Route2 = new CResult()
																{
																	ResultColumnNumber = enResultColumnNumber.Route2,
																	Time = MemberResult.route2,
																},
																Sum = new CResult()
																{
																	ResultColumnNumber = enResultColumnNumber.Sum,
																	Time = MemberResult.sum,
																},
															});
						}
					}

					Item = new CQueueItem()
					{
						GeneratorTask = new CTotalGenerator.CTotalTask()
						{
							m_GeneratorType = RoundToSend,
							m_lstCompRounds = AllGroupRounds,
							m_lstMembers = Members.ToList(),
							m_SecondColName = Group.SecondColName,
							m_GroupId = GroupId,
							m_CompId = CompId,
							m_FirstMiddleSheetRoundMembers = AllGroupRounds.Contains(enRounds.Qualif2) ? Group.MembersFrom2ndQualif : Group.MembersFrom1stQualif,
							m_MembersAfter1stQualif = Group.MembersFrom1stQualif
						},
						FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
						PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
					};
				}
				else
				{
					// Список участников раунда со всей необходимой информацией 
					Members = (from member in DBManagerApp.m_Entities.members
							   join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
							   join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
							   where result.round == (byte)RoundToSend && part.Group == GroupId
							   select new CMemberAndResults
							   {
								   MemberInfo = new CFullMemberInfo()
								   {
									   IDMember = member.id_member,
									   Surname = member.surname,
									   Name = member.name,
									   YearOfBirth = member.year_of_birth,
									   Coach = part.coach,
									   Team = part.team,
									   InitGrade = part.init_grade,
								   },

								   Results = new COneRoundResults()
								   {
									   m_Round = (enRounds)result.round,
									   Route1 = new CResult()
									   {
										   ResultColumnNumber = enResultColumnNumber.Route1,
										   Time = result.route1,
									   },
									   Route2 = new CResult()
									   {
										   ResultColumnNumber = enResultColumnNumber.Route2,
										   Time = result.route2,
									   },
									   Sum = new CResult()
									   {
										   ResultColumnNumber = enResultColumnNumber.Sum,
										   Time = result.sum,
									   },
								   },

								   StartNumber = result.number,
								   Place = result.place,
							   }).ToList();

					// В основном запросе заполнить эти поля почему-то не получилось
					foreach (CMemberAndResults item in Members)
					{
						if (Group.SecondColNameType == enSecondColNameType.Coach)
							item.MemberInfo.SecondCol = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == item.MemberInfo.Coach).name;
						else
							item.MemberInfo.SecondCol = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == item.MemberInfo.Team).name;
					}

					switch (RoundToSend)
					{
						case enFTPSheetGeneratorTypes.Qualif:
							if (!m_FTPExporter.HasStartlist)
							{	// Нужно добавить ещё и стартовый протокол
								Members = Members.OrderBy(arg => (arg as CMemberAndResults).StartNumber);
								Item = new CQueueItem()
								{
									GeneratorTask = new CQualifGenerator.CQualifTask()
									{
										m_GeneratorType = enFTPSheetGeneratorTypes.Start,
										m_lstCompRounds = AllGroupRounds,
										m_lstMembers = Members.ToList(),
										m_SecondColName = Group.SecondColName,
										m_GroupId = GroupId,
										m_CompId = CompId
									},
									FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
									PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
								};
								if (!m_FTPExporter.HandleItem(Item))
									return false;
							}
							Members = Members.OrderBy((arg) => 
								{
									if ((arg as CMemberAndResults).Results.Sum.Time.HasValue)
										return (arg as CMemberAndResults).Results.Sum.Time.Value;
									else
										return TimeSpan.MaxValue;
								}).ThenBy(arg => (arg as CMemberAndResults).StartNumber);

							Item = new CQueueItem()
							{
								GeneratorTask = new CQualifGenerator.CQualifTask()
								{
									m_GeneratorType = RoundToSend,
									m_lstCompRounds = AllGroupRounds,
									m_lstMembers = Members.ToList(),
									m_SecondColName = Group.SecondColName,
									m_GroupId = GroupId,
									m_CompId = CompId,
									m_MembersAfterQualif = Group.MembersFrom1stQualif,
								},
								FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
								PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
							};
							break;

						case enFTPSheetGeneratorTypes.Qualif2:
							Members = Members.OrderBy((arg) =>
							{
								if ((arg as CMemberAndResults).Results.Sum.Time.HasValue)
									return (arg as CMemberAndResults).Results.Sum.Time.Value;
								else
									return TimeSpan.MaxValue;
							}).ThenBy(arg => (arg as CMemberAndResults).StartNumber);
							Item = new CQueueItem()
							{
								GeneratorTask = new CQualifGenerator.CQualifTask()
								{
									m_GeneratorType = RoundToSend,
									m_lstCompRounds = AllGroupRounds,
									m_lstMembers = Members.ToList(),
									m_SecondColName = Group.SecondColName,
									m_GroupId = GroupId,
									m_CompId = CompId,
									m_MembersAfterQualif = Group.MembersFrom2ndQualif,
								},
								FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
								PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
							};
							break;

						case enFTPSheetGeneratorTypes.OneEighthFinal:
						case enFTPSheetGeneratorTypes.QuaterFinal:
						case enFTPSheetGeneratorTypes.SemiFinal:
							Members = Members.OrderBy(arg => (arg as CMemberAndResults).StartNumber);
							Item = new CQueueItem()
							{
								GeneratorTask = new CMiddleSheetsGenerator.CMiddleSheetsTask()
								{
									m_GeneratorType = RoundToSend,
									m_lstCompRounds = AllGroupRounds,
									m_lstMembers = Members.ToList(),
									m_SecondColName = Group.SecondColName,
									m_GroupId = GroupId,
									m_CompId = CompId,
								},
								FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
								PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
							};
							break;

						case enFTPSheetGeneratorTypes.Final:
							Members = Members.OrderBy(arg => (arg as CMemberAndResults).StartNumber);
							Item = new CQueueItem()
							{
								GeneratorTask = new CFinalGenerator.CFinalTask()
								{
									m_GeneratorType = RoundToSend,
									m_lstCompRounds = AllGroupRounds,
									m_lstMembers = Members.ToList(),
									m_SecondColName = Group.SecondColName,
									m_GroupId = GroupId,
									m_CompId = CompId,
								},
								FTPWbkFullPath = FTPGroupItemInSets.FTPWbkPath,
								PCWbkFullPath = Dir + GroupId.ToString() + GlobalDefines.XLS_EXTENSION,
							};
							break;

						default:
							break;
					}
				}
			}

			return DeferredExport ? m_FTPExporter.HandleItem(Item) : m_FTPExporter.AddItemToQueue(Item);
		}
		#endregion
	}


	/// <summary>
	/// Число элементов в коллекции равно 0 => false или true, если IsInverse = true
	/// </summary>
	public class CollectionsCountToBoolMarkupConverter : MarkupConverterBase
	{
		bool m_IsInverse = false;
		/// <summary>
		/// Инверсное преобразование, т.е. если true, то UnvisibleValue -> true 
		/// </summary>
		public bool IsInverse
		{
			get { return m_IsInverse; }
			set { m_IsInverse = value; }
		}


		public CollectionsCountToBoolMarkupConverter(): base()
		{
		}


		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return (int)value == 0 ? IsInverse : !IsInverse;
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in CollectionsCountToBoolConverter"); 
		}
	}
}
