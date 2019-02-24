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
using DBManager.InterfaceElements;
using System.IO;
using DBManager.SettingsWriter;
using DBManager.Global;
using System.ComponentModel;
using System.Drawing.Text;
using System.Drawing;

namespace DBManager.SettingWnds
{
	/// <summary>
	/// Interaction logic for CSettingsWnd.xaml
	/// </summary>
	public partial class CSettingsWnd : СCustomSettingsWnd
	{
		/// <summary>
		/// Запись в выпадающем списке соревнований
		/// </summary>
		class CCompItem
		{
			#region Dir
			private string m_Dir = null;

			public string Dir
			{
				get { return m_Dir; }
				set
				{
					if (m_Dir != value)
					{
						m_Dir = value;
					}
				}
			}
			#endregion

			
			#region Name
			private string m_Name = null;
			/// <summary>
			/// Название сорев. Выбирается из БД
			/// </summary>
			public string Name
			{
				get { return m_Name; }
				private set
				{
					if (m_Name != value)
					{
						m_Name = value;
					}
				}
			}
			#endregion
			
			
			#region StartDate
			private DateTime? m_StartDate = null;

			public DateTime? StartDate
			{
				get { return m_StartDate; }
				private set
				{
					if (m_StartDate != value)
					{
						m_StartDate = value;
					}
				}
			}
			#endregion

			
			#region EndDate
			private DateTime? m_EndDate = null;

			public DateTime? EndDate
			{
				get { return m_EndDate; }
				set
				{
					if (m_EndDate != value)
					{
						m_EndDate = value;
					}
				}
			}
			#endregion
				

			public bool HasInDB 
			{
				get { return !string.IsNullOrWhiteSpace(Name); }
			}

			
			#region IsSelected
			private bool m_IsSelected = false;

			public bool IsSelected
			{
				get { return m_IsSelected; }
				set
				{
					if (m_IsSelected != value)
					{
						m_IsSelected = value;
					}
				}
			}
			#endregion


			
			#region Index
			private int m_Index = -1;

			public int Index
			{
				get { return m_Index; }
				set
				{
					if (m_Index != value)
					{
						m_Index = value;
					}
				}
			}
			#endregion		


			public override string ToString()
			{
				if (HasInDB)
				{
					if (StartDate.HasValue && EndDate.HasValue)
					{
						if (StartDate.Value == EndDate.Value)
						{
							return string.Format(Properties.Resources.resfmtCompOneDateForSettingsWnd,
													Name,
													StartDate.Value.ToShortDateString(),
													Dir);
						}
						else
						{
							return string.Format(Properties.Resources.resfmtCompDateForSettingsWnd,
													Name,
													StartDate.Value.ToShortDateString(),
													EndDate.Value.ToShortDateString(),
													Dir);
						}
					}
					else
					{
						return string.Format(Properties.Resources.resfmtCompDateForSettingsWndWithoutDates,
												Name,
												Dir);
					}
				}
				else
				{	// Если соревнования нет в БД, то выводим просто путь к папке
					return Dir;
				}
			}


			public CCompItem()
			{
			}


			public CCompItem(string name, string dir, DateTime? startDate, DateTime? endDate)
			{
				Name = name;
				Dir = dir;
				StartDate = startDate;
				EndDate = endDate ?? startDate;
			}


			public override bool Equals(object o)
			{
				if (o is CCompItem)
					return this == (o as CCompItem);

				return false;
			}


			public override int GetHashCode()
			{
				return Dir.GetHashCode();
			}


			public static bool operator ==(CCompItem lhs, CCompItem rhs)
			{
				return ((object)lhs == null && (object)rhs == null) ||
						((object)lhs != null &&
							(object)rhs != null && lhs.Dir == rhs.Dir);
			}


			public static bool operator !=(CCompItem lhs, CCompItem rhs)
			{
				return !(lhs == rhs);
			}
		}


		public override bool Modified
		{
			get { return m_Modified; }
			set
			{
				m_Modified = value;
				if (m_Modified)
					ModifiedFromOpen = true;
				OnPropertyChanged("Modified");
				CommandManager.InvalidateRequerySuggested();
			}
		}


		CCompItem SelectedCompItem
		{
			get { return m_dictCompItems.Values.FirstOrDefault(arg => arg.IsSelected); }
		}


		ResourceDictionary m_GlobalResources = new ResourceDictionary()
		{
			Source = new Uri("\\Global\\GlobalResources.xaml", UriKind.RelativeOrAbsolute)
		};


		/// <summary>
		/// Словарь соревнований в списке. Заполняется в коде, а не с помощью binding'ов.
		/// Ключ - директория
		/// </summary>
		Dictionary<string, CCompItem> m_dictCompItems = new Dictionary<string, CCompItem>();

		Font m_lastSelectedFont = null;
		

		#region Команды
		/// <summary>
		/// cmdApply.Execute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ApplyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveSettings();
		}


		/// <summary>
		/// cmdApply.CanExecute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void ApplyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Modified;
		}
		/*----------------------------------------------------------*/
		#endregion


		void AddLastCompItem(string DirPath)
		{
			CCompItem Item = new CCompItem()
			{
				Dir = DirPath,
				Index = cmbComp.Items.Count,
				IsSelected = true
			};
			m_dictCompItems.Add(DirPath, Item);
			cmbComp.Items.Add(Item.ToString());

			cmbComp.SelectedIndex = Item.Index;
		}


		void SelectCompItem(CCompItem ItemToSelect)
		{
			foreach (KeyValuePair<string, CCompItem> item in m_dictCompItems)
			{
				if (item.Value == ItemToSelect)
				{
					item.Value.IsSelected = true;
					cmbComp.SelectionChanged -= cmbComp_SelectionChanged;
					cmbComp.SelectedIndex = item.Value.Index;
					cmbComp.SelectionChanged += cmbComp_SelectionChanged;
				}
				else
					item.Value.IsSelected = false;
			}
		}


		void CheckAndChangeLastItem(string DirPath)
		{
			cmbComp.SelectionChanged -= cmbComp_SelectionChanged;

			CCompItem NewItem = m_dictCompItems.Values.FirstOrDefault(arg => !arg.HasInDB);
			if (NewItem == null)
			{	// Все записи в словаре есть в БД => добавляем в словарь и выделяем
				SelectCompItem(null); // Делаем все записи не выделенными
				AddLastCompItem(DirPath);
			}
			else
			{	// Нужно заменить последнюю запись и выделить её
				m_dictCompItems.Remove(NewItem.Dir);
				
				cmbComp.SelectionChanged -= cmbComp_SelectionChanged;
				cmbComp.Items[NewItem.Index] = NewItem.Dir = DirPath;
				cmbComp.SelectionChanged += cmbComp_SelectionChanged;

				m_dictCompItems.Add(NewItem.Dir, NewItem);
				SelectCompItem(NewItem);
			}

			cmbComp.SelectionChanged += cmbComp_SelectionChanged;
		}


		public CSettingsWnd()
		{
			InitializeComponent();

			HasUnsavedChanges += () => { return Modified; };

			lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
			{
				// Ищем все соревы, имеющиеся в БД
				cmbComp.SelectedIndex = -1;
				List<descriptions> DescInDB = new List<descriptions>(DBManagerApp.m_Entities.descriptions);
				foreach (descriptions desc in DescInDB)
				{
					DateTime? StartDate = desc.groups.Count > 0 ? desc.groups.Min<groups, DateTime?>(arg => arg.comp_start_date) : null;
					DateTime? EndDate = desc.groups.Count > 0 ? desc.groups.Min<groups, DateTime?>(arg => arg.comp_end_date) : null;
					CCompItem Item = new CCompItem(desc.name, desc.dir, StartDate, EndDate)
					{
						Index = cmbComp.Items.Count
					};
					m_dictCompItems.Add(desc.dir, Item);
					cmbComp.Items.Add(Item.ToString());
					if (DBManagerApp.m_AppSettings.m_Settings.CompDir == desc.dir)
					{	// Эта запись выбрана в настройках
						Item.IsSelected = true;
						cmbComp.SelectedIndex = Item.Index;
					}
				}
				if (cmbComp.SelectedIndex < 0)
				{	// В настройках выбрана запись которой нет в БД => добавляем её в конец
					AddLastCompItem(DBManagerApp.m_AppSettings.m_Settings.CompDir);
				}
				cmbComp.SelectionChanged += cmbComp_SelectionChanged;
			
				chkHandleFileDeletion.IsChecked = DBManagerApp.m_AppSettings.m_Settings.HandleFileDeletion;
				chkAutodetectOnStart.IsChecked = DBManagerApp.m_AppSettings.m_Settings.AutodetectOnStart;
				cmbResultGradeCalcMethod.SelectedIndex = (int)DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod;

				chkOnly75PercentForCalcGrades.IsChecked = DBManagerApp.m_AppSettings.m_Settings.Only75PercentForCalcGrades;
				txtMinAgeToCalcResultGrade.Text = DBManagerApp.m_AppSettings.m_Settings.MinAgeToCalcResultGrade.ToString();
				RefreshMaxYearToCalcResultGrade(DBManagerApp.m_AppSettings.m_Settings.MinAgeToCalcResultGrade);

                txtWorkbookTemplateFolder.Text = DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder;

                fntstlInvatedToStart.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle;
				fntstlJustRecievedResult.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle;
				fntstlNextRoundMembersCount.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle;
				fntstlPreparing.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle;
				fntstlStayOnStart.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle;
				fntstlPlainResults.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle;
				fntstlFalsestart.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.FalsestartFontStyle;
                fntstlGridLines.FontStyleSettings = DBManagerApp.m_AppSettings.m_Settings.GridLinesFontStyle;

                fntstlInvatedToStart.FontSize =
					fntstlJustRecievedResult.FontSize =
					fntstlNextRoundMembersCount.FontSize =
					fntstlPreparing.FontSize =
					fntstlStayOnStart.FontSize =
					fntstlPlainResults.FontSize =
					fntstlFalsestart.FontSize =
                    fntstlGridLines.FontSize = DBManagerApp.m_AppSettings.m_Settings.FontSize;
				txtFontSize.Text = ((int)DBManagerApp.m_AppSettings.m_Settings.FontSize).ToString();

				lblFontFamilyName.Content =
					fntstlInvatedToStart.FontFamilyName =
					fntstlJustRecievedResult.FontFamilyName =
					fntstlNextRoundMembersCount.FontFamilyName =
					fntstlPreparing.FontFamilyName =
					fntstlStayOnStart.FontFamilyName =
					fntstlPlainResults.FontFamilyName =
					fntstlFalsestart.FontFamilyName =
                    fntstlGridLines.FontFamilyName = DBManagerApp.m_AppSettings.m_Settings.FontFamilyName;

				InstalledFontCollection installedFontCollection = new InstalledFontCollection();
				var family = installedFontCollection.Families.FirstOrDefault(arg => arg.Name == DBManagerApp.m_AppSettings.m_Settings.FontFamilyName)
									?? new System.Drawing.FontFamily(m_GlobalResources["DefaultFontFamilyName"].ToString());
				m_lastSelectedFont = new Font(family, 16, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);

				fntstlInvatedToStart.Modified =
					fntstlJustRecievedResult.Modified =
					fntstlNextRoundMembersCount.Modified =
					fntstlPreparing.Modified =
					fntstlStayOnStart.Modified =
					fntstlPlainResults.Modified =
					fntstlFalsestart.Modified =
                    fntstlGridLines.Modified = false;

				fntstlInvatedToStart.PropertyChanged += fntstl_PropertyChanged;
				fntstlJustRecievedResult.PropertyChanged += fntstl_PropertyChanged;
				fntstlNextRoundMembersCount.PropertyChanged += fntstl_PropertyChanged;
				fntstlPreparing.PropertyChanged += fntstl_PropertyChanged;
				fntstlStayOnStart.PropertyChanged += fntstl_PropertyChanged;
				fntstlPlainResults.PropertyChanged += fntstl_PropertyChanged;
				fntstlFalsestart.PropertyChanged += fntstl_PropertyChanged;
                fntstlGridLines.PropertyChanged += fntstl_PropertyChanged;
            }

			GlobalDefines.TuneComboboxWidth2(cmbResultGradeCalcMethod);

			ModifiedFromOpen = Modified = false;
		}


		void fntstl_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
            if (sender is CFontStylePicker)
            {
                if (e.PropertyName == CFontStylePicker.ModifiedPropertyName)
                    Modified |= (sender as CFontStylePicker).Modified;
                else if (e.PropertyName == CFontStylePicker.BackgroundColorPropertyName && sender == fntstlGridLines)
                {
                    (sender as CFontStylePicker).ForeColor = System.Windows.Media.Color.FromArgb(fntstlGridLines.BackgroundColor.A,
                                                                                                (byte)(fntstlGridLines.BackgroundColor.R ^ 0xFF),
                                                                                                (byte)(fntstlGridLines.BackgroundColor.G ^ 0xFF),
                                                                                                (byte)(fntstlGridLines.BackgroundColor.B ^ 0xFF));
                }
            }
		}


		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			/* Переводи фокус на кнопку ОК, чтобы сработала проверка во всех TextBoxEx.
			 * Затем проверяем результат проверки и ничего не делаем, если проверка прошла неудачно */
			IInputElement FocusedEl = FocusManager.GetFocusedElement(this);
			btnOK.Focus();

			if (FocusedEl is TextBoxEx && !(FocusedEl as TextBoxEx).IsRightInput)
				return;

			if (SaveSettings())
			{
				DialogResult = ModifiedFromOpen;
			}
		}

		
		private void btnBrowse_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog()
			{
				ShowNewFolderButton = true
			};
			lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
			{
				dlg.SelectedPath = DBManagerApp.m_AppSettings.m_Settings.CompDir;
			}

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				CCompItem NewItem = null;
				if (m_dictCompItems.Count == 0)
				{	// Записей нет
					NewItem = new CCompItem()
					{
						Dir = dlg.SelectedPath,
						IsSelected = true
					};
					m_dictCompItems.Add(dlg.SelectedPath, NewItem);
					cmbComp.Items.Add(NewItem.ToString());

					cmbComp.SelectionChanged -= cmbComp_SelectionChanged;
					cmbComp.SelectedIndex = cmbComp.Items.Count - 1;
					cmbComp.SelectionChanged += cmbComp_SelectionChanged;
				}
				else
				{	// Проверяем, есть ли запись с такой директорией в словаре 
					if (m_dictCompItems.TryGetValue(dlg.SelectedPath, out NewItem))
					{	// Есть => выделяем её
						SelectCompItem(NewItem);
					}
					else
					{
						CheckAndChangeLastItem(dlg.SelectedPath);
					}
				}
			}
		}


		private void txtFontSize_LostFocus(object sender, RoutedEventArgs e)
		{
			if (txtFontSize.Modified && txtFontSize.IsRightInput)
			{
				fntstlInvatedToStart.FontSize =
					fntstlJustRecievedResult.FontSize =
					fntstlNextRoundMembersCount.FontSize =
					fntstlPreparing.FontSize =
					fntstlStayOnStart.FontSize =
					fntstlPlainResults.FontSize =
					fntstlFalsestart.FontSize =
                    fntstlGridLines.FontSize = (int)txtFontSize.Value;
			}
		}


		private void txtFontSize_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (txtFontSize.IsRightInput)
			{
				int CurVal = (int)txtFontSize.Value;
				int NewVal = CurVal + e.Delta / 60;
				if (NewVal >= 10 && NewVal <= 36)
				{
					txtFontSize.Text = NewVal.ToString();
					txt_TextChanged(txtFontSize, null);
				}
			}
		}


		private void btnFontFamily_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog()
			{
				ShowColor = false,
				ShowEffects = false,
				Font = m_lastSelectedFont
			};
			System.Windows.Forms.DialogResult dr = fd.ShowDialog();
			if (dr != System.Windows.Forms.DialogResult.Cancel)
			{
				lblFontFamilyName.Content =
					fntstlInvatedToStart.FontFamilyName =
					fntstlJustRecievedResult.FontFamilyName =
					fntstlNextRoundMembersCount.FontFamilyName =
					fntstlPreparing.FontFamilyName =
					fntstlStayOnStart.FontFamilyName =
					fntstlPlainResults.FontFamilyName =
					fntstlFalsestart.FontFamilyName =
                    fntstlGridLines.FontFamilyName = fd.Font.Name;

				m_lastSelectedFont = fd.Font;

				Modified = true;
			}
		}
		

		private bool CheckSettings()
		{
			if (Modified)
			{
				if (SelectedCompItem != null)
				{
					if (!Directory.Exists(SelectedCompItem.Dir))
					{
						System.Windows.MessageBox.Show(this, Properties.Resources.resInvalidCompDir, Title, MessageBoxButton.OK, MessageBoxImage.Error);
						return false;
					}
				}

                if (!Directory.Exists(txtWorkbookTemplateFolder.Text))
                {
                    System.Windows.MessageBox.Show(this, Properties.Resources.resInvalidWorkbookTemplateFolder, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                foreach (var filename in DBManagerApp.m_AppSettings.m_Settings.FilesToCopyFromWorkbookTemplateFolder)
                {
                    if (!File.Exists(System.IO.Path.Combine(txtWorkbookTemplateFolder.Text, filename)))
                    {
                        System.Windows.MessageBox.Show(this,
                            string.Format(Properties.Resources.resfmtInvalidWorkbookTemplateFolder, filename),
                            Title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return false;
                    }
                }

                return true;
            }

			return Modified;
		}


		/// <summary>
		/// Метод, который производит сохранение настроек
		/// </summary>
		/// <returns></returns>
		protected override bool SaveSettings()
		{
			if (Modified)
			{
				if (cmbComp.SelectedIndex < 0 ||
					(!SelectedCompItem.HasInDB && cmbComp.SelectedIndex == cmbComp.Items.Count - 1))
				{	// Переписываем данные из выпадающего списка в m_dictCompItems
					CheckAndChangeLastItem(cmbComp.Text);
				}

				if (!CheckSettings())
					return false;

				lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
				{
					DBManagerApp.m_AppSettings.m_Settings.CompDir = SelectedCompItem == null ? null : SelectedCompItem.Dir;
					DBManagerApp.m_AppSettings.m_Settings.HandleFileDeletion = chkHandleFileDeletion.IsChecked.HasValue &&
																				chkHandleFileDeletion.IsChecked.Value;
					DBManagerApp.m_AppSettings.m_Settings.AutodetectOnStart = chkAutodetectOnStart.IsChecked.HasValue &&
																				chkAutodetectOnStart.IsChecked.Value;
					
					DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod = (enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex;

					DBManagerApp.m_AppSettings.m_Settings.Only75PercentForCalcGrades = chkOnly75PercentForCalcGrades.IsChecked.HasValue &&
																						chkOnly75PercentForCalcGrades.IsChecked.Value;
					DBManagerApp.m_AppSettings.m_Settings.MinAgeToCalcResultGrade = (int)txtMinAgeToCalcResultGrade.Value;

                    DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder = txtWorkbookTemplateFolder.Text;

                    DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle = fntstlInvatedToStart.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle = fntstlJustRecievedResult.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle = fntstlNextRoundMembersCount.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle = fntstlPreparing.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle = fntstlStayOnStart.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle = fntstlPlainResults.FontStyleSettings;
					DBManagerApp.m_AppSettings.m_Settings.FalsestartFontStyle = fntstlFalsestart.FontStyleSettings;
                    DBManagerApp.m_AppSettings.m_Settings.GridLinesFontStyle = fntstlGridLines.FontStyleSettings;

                    DBManagerApp.m_AppSettings.m_Settings.FontFamilyName = lblFontFamilyName.Content.ToString();
					DBManagerApp.m_AppSettings.m_Settings.FontSize = (int)txtFontSize.Value;

					DBManagerApp.m_AppSettings.Write();
				}

				fntstlInvatedToStart.Modified =
					fntstlJustRecievedResult.Modified =
					fntstlNextRoundMembersCount.Modified =
					fntstlPreparing.Modified =
					fntstlStayOnStart.Modified =
					fntstlPlainResults.Modified =
					fntstlFalsestart.Modified =
                    fntstlGridLines.Modified =
                    Modified = false;
			}
						
			return true;
		}


		private void btnToDefault_Click(object sender, RoutedEventArgs e)
		{
			txtFontSize.Text = m_GlobalResources["DefaultFontSize"].ToString();
			lblFontFamilyName.Content = m_GlobalResources["DefaultFontFamilyName"];

			fntstlInvatedToStart.FontSize =
					fntstlJustRecievedResult.FontSize =
					fntstlNextRoundMembersCount.FontSize =
					fntstlPreparing.FontSize =
					fntstlStayOnStart.FontSize =
					fntstlPlainResults.FontSize =
					fntstlFalsestart.FontSize =
                    fntstlGridLines.FontSize = (int)txtFontSize.Value;

			fntstlInvatedToStart.FontFamilyName =
				fntstlJustRecievedResult.FontFamilyName =
				fntstlNextRoundMembersCount.FontFamilyName =
				fntstlPreparing.FontFamilyName =
				fntstlStayOnStart.FontFamilyName =
				fntstlPlainResults.FontFamilyName =
				fntstlFalsestart.FontFamilyName =
                fntstlGridLines.FontFamilyName = lblFontFamilyName.Content.ToString();

            fntstlGridLines.FontStyleSettings = new CFontStyleSettings()
            {
                BackgroundColor = (m_GlobalResources["DataGridLinesBrush"] as SolidColorBrush).Color,
            };

            fntstlPlainResults.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["PlainResultsBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["PlainResultsForeBrush"] as SolidColorBrush).Color,
			};

			fntstlNextRoundMembersCount.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["NextRoundMembersCountBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["NextRoundMembersCountForeBrush"] as SolidColorBrush).Color,
			};

			fntstlInvatedToStart.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["InvitedToStartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["InvitedToStartForeBrush"] as SolidColorBrush).Color,
			};

			fntstlPreparing.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["PreparingBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["PreparingForeBrush"] as SolidColorBrush).Color,
			};

			fntstlStayOnStart.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["StayOnStartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["StayOnStartForeBrush"] as SolidColorBrush).Color,
			};

			fntstlJustRecievedResult.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["JustRecievedResultBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["JustRecievedResultForeBrush"] as SolidColorBrush).Color,
			};

			fntstlFalsestart.FontStyleSettings = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["FalsestartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["FalsestartForeBrush"] as SolidColorBrush).Color,
			};
		}

		
		private void cmbComp_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbComp.SelectedIndex < 0)
			{
				CheckAndChangeLastItem(cmbComp.Text);
			}
			else
			{
				SelectCompItem(m_dictCompItems.Values.First(arg => arg.Index == cmbComp.SelectedIndex));
			}
		}

		private void cmbComp_Loaded(object sender, RoutedEventArgs e)
		{
			TextBox txt = (TextBox)cmbComp.Template.FindName("PART_EditableTextBox", cmbComp);
			if (txt != null)
			{
				txt.TextChanged += txt_TextChanged;
				txt.TextWrapping = TextWrapping.Wrap;
			}
		}


		void RefreshMaxYearToCalcResultGrade(int MinAgeToCalcResultGrade)
		{
			lblMaxYearToCalcResultGrade.Content = string.Format(Properties.Resources.resfmtMaxYearToCalcResultGrade,
																DateTime.Today.Year - (int)txtMinAgeToCalcResultGrade.Value);
		}


		private void txtMinAgeToCalcResultGrade_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtMinAgeToCalcResultGrade.Modified && txtMinAgeToCalcResultGrade.IsRightInput)
			{
				RefreshMaxYearToCalcResultGrade((int)txtMinAgeToCalcResultGrade.Value);
			}
			base.txt_TextChanged(sender, e);
		}


        private void btnWorkbookTemplateFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog()
            {
                 ShowNewFolderButton = false
            };
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                dlg.SelectedPath = DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder;
            }

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtWorkbookTemplateFolder.Text = dlg.SelectedPath;
            }

        }
    }
}
