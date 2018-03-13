using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Objects;
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

namespace DBManager.SettingWnds
{
	/// <summary>
	/// Interaction logic for FalsestartRules.xaml
	/// </summary>
	public partial class FalsestartRules : СCustomSettingsWnd
	{
		public class FalsestartRule : INotifyPropertyChanged
		{
			#region Number
			private static readonly string NumberPropertyName = GlobalDefines.GetPropertyName<FalsestartRule>(m => m.Number);

			private int m_Number = 0;

			/// <summary>
			/// Номер правила. Начинается с 1
			/// </summary>
			public int Number
			{
				get { return m_Number; }
				set
				{
					if (m_Number != value)
					{
						m_Number = value;
						OnPropertyChanged(NumberPropertyName);
					}
				}
			}
			#endregion


			#region StartRound
			private static readonly string StartRoundPropertyName = GlobalDefines.GetPropertyName<FalsestartRule>(m => m.StartRound);

			private byte? m_StartRound = null;

			public byte? StartRound
			{
				get { return m_StartRound; }
				set
				{
					if (m_StartRound != value)
					{
						m_StartRound = value;
						OnPropertyChanged(StartRoundPropertyName);
					}
				}
			}
			#endregion


			#region EndRound
			private static readonly string EndRoundPropertyName = GlobalDefines.GetPropertyName<FalsestartRule>(m => m.EndRound);

			private byte? m_EndRound = null;

			public byte? EndRound
			{
				get { return m_EndRound; }
				set
				{
					if (m_EndRound != value)
					{
						m_EndRound = value;
						OnPropertyChanged(EndRoundPropertyName);
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


			public FalsestartRule(int number)
			{
				Number = number;
			}
		}

		readonly long m_GroupId = -1;

		public List<KeyValuePair<byte, string>> Rounds { get; private set; }

		#region Rules
		ObservableCollection<FalsestartRule> m_Rules = new ObservableCollection<FalsestartRule>();
		public ObservableCollection<FalsestartRule> Rules
		{
			get { return m_Rules; }
		}
		#endregion


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

		
		/// <summary>
		/// Команда "Добавить".
		/// </summary>
		public static RoutedCommand cmdAdd = new RoutedCommand();


		/// <summary>
		/// cmdAdd.Execute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			FalsestartRule NewRule = new FalsestartRule(Rules.Count + 1);
			Rules.Add(NewRule);
			lstvRules.SelectedItem = NewRule;
		}


		/// <summary>
		/// cmdAdd.CanExecute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
		/*----------------------------------------------------------*/


		/// <summary>
		/// Команда "Удалить".
		/// </summary>
		public static RoutedCommand cmdDel = new RoutedCommand();


		/// <summary>
		/// cmdDel.Execute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DelCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (MessageBox.Show(this,
								string.Format(Properties.Resources.resfmtDeleteFalsestartRuleQuestion, (lstvRules.SelectedItem as FalsestartRule).Number),
											Title,
											MessageBoxButton.YesNo,
											MessageBoxImage.Question,
											MessageBoxResult.Yes) == MessageBoxResult.Yes)
			{
				int NewSelIndex = lstvRules.SelectedIndex == 0 ? lstvRules.SelectedIndex : lstvRules.SelectedIndex - 1;
				Rules.RemoveAt(lstvRules.SelectedIndex);
				lstvRules.SelectedIndex = NewSelIndex;
			}
		}


		/// <summary>
		/// cmdDel.CanExecute
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DelCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = lstvRules.SelectedIndex >= 0;
		}
		/*----------------------------------------------------------*/
		#endregion


		public FalsestartRules()
		{
			InitializeComponent();
						
			ModifiedFromOpen = Modified = false;
		}


		public FalsestartRules(long GroupId, CAgeGroup Group)
		{
			InitializeComponent();

			Rounds = GlobalDefines.ROUND_NAMES.ToList();
			Rounds.RemoveAt(Rounds.Count - 1); // Удаляем итоговый протокол

			m_GroupId = GroupId;

			Title = string.Format(Properties.Resources.resfmtFalsestartRulesWndTitle, Group.FullGroupName);

			HasUnsavedChanges += () => { return Modified; };

			// Заполняем список уже имеющимися правилами
			int i = 1;
			foreach (falsestarts_rules rule in (from rule in DBManagerApp.m_Entities.falsestarts_rules
												where rule.Group == m_GroupId
												select rule).ToList())
			{
				FalsestartRule Rule = new FalsestartRule(i)
				{
					StartRound = rule.start_round,
					EndRound = rule.end_round,
				};
				Rule.PropertyChanged += RuleCopy_PropertyChanged;
				Rules.Add(Rule);

				i++;
			}
									
			Rules.CollectionChanged += Rules_CollectionChanged;


			CommandBinding cmdb = new CommandBinding()
			{
				Command = cmdAdd
			};
			cmdb.Executed += AddCmdExecuted;
			cmdb.CanExecute += AddCmdCanExecute;
			CommandBindings.Add(cmdb);

			cmdb = new CommandBinding()
			{
				Command = cmdDel
			};
			cmdb.Executed += DelCmdExecuted;
			cmdb.CanExecute += DelCmdCanExecute;
			CommandBindings.Add(cmdb);


			InputBinding inpb = new InputBinding(cmdAdd, new KeyGesture(Key.Insert));
			InputBindings.Add(inpb);
			inpb = new InputBinding(cmdDel, new KeyGesture(Key.Delete));
			InputBindings.Add(inpb);


			ModifiedFromOpen = Modified = false;
		}


		private void RuleCopy_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Modified = true;
		}


		public void Rules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Modified = true;
		}


		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			/* Переводи фокус на кнопку ОК, чтобы сработала проверка во всех TextBoxEx.
			 * Затем проверяем результат проверки и ничего не делаем, если проверка прошла неудачно */
			IInputElement FocusedEl = FocusManager.GetFocusedElement(this);
			btnOK.Focus();

			if (SaveSettings())
			{
				DialogResult = ModifiedFromOpen;
			}
		}


		private void btnToDefault_Click(object sender, RoutedEventArgs e)
		{
			Rules.Clear();
		}


		private bool CheckSettings()
		{
			if (Modified)
			{
				int[] RoundsInRules = new int[Rounds.Count];
				foreach (FalsestartRule rule in Rules)
				{
					if ((rule.StartRound == null) || (rule.EndRound == null) || (rule.StartRound > rule.EndRound))
					{
						MessageBox.Show(this,
										string.Format(Properties.Resources.resfmtInvalidFalsestartRule, rule.Number),
										Title,
										MessageBoxButton.OK,
										MessageBoxImage.Error);
						return false;
					}

					// round начинается с 1
					for (int round = rule.StartRound.Value; round <= rule.EndRound.Value; round++)
					{
						if (RoundsInRules[round - 1] > 0)
						{
							MessageBox.Show(this,
											string.Format(Properties.Resources.resfmtFalsestartRulesAreIntersected, RoundsInRules[round - 1], rule.Number),
											Title,
											MessageBoxButton.OK,
											MessageBoxImage.Error);
							return false;
						}
						else
							RoundsInRules[round - 1] = rule.Number;
					}
				}
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
				if (!CheckSettings())
					return false;

				// Удаляем все правила для текущей группы, чтобы заменить их новыми
				EntityConnection entityConnection = (EntityConnection)DBManagerApp.m_Entities.Connection;
				DbConnection conn = entityConnection.StoreConnection;

				ConnectionState initialState = conn.State;
				try
				{
					if (initialState != ConnectionState.Open)
						conn.Open();  // open connection if not already open
					using (DbCommand cmd = conn.CreateCommand())
					{
						cmd.CommandText = $"DELETE FROM falsestarts_rules WHERE falsestarts_rules.Group = {m_GroupId}";
						cmd.ExecuteNonQuery();
					}
				}
				catch (Exception ex)
				{
					if (initialState != ConnectionState.Open)
						conn.Close(); // only close connection if not initially open

					MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtCantUpdateFalsestartRules, ex.Message),
									Title,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return false;
				}

				if (initialState != ConnectionState.Open)
					conn.Close(); // only close connection if not initially open

				// Get all objects in statemanager with entityKey 
				// (context.Refresh will throw an exception otherwise) 
				var refreshableObjects = (from entry in DBManagerApp.m_Entities.ObjectStateManager.GetObjectStateEntries(
															EntityState.Deleted
														  | EntityState.Modified
														  | EntityState.Unchanged)
										  where entry.EntityKey != null
										  select entry.Entity);

				DBManagerApp.m_Entities.Refresh(RefreshMode.StoreWins, refreshableObjects);

				foreach (FalsestartRule rule in Rules)
				{
					DBManagerApp.m_Entities.AddTofalsestarts_rules(new falsestarts_rules()
					{
						Group = m_GroupId,
						start_round = (byte)rule.StartRound,
						end_round = (byte)rule.EndRound,
					});
				}

				try
				{
					DBManagerApp.m_Entities.SaveChanges();
				}
				catch (Exception ex)
				{
					MessageBox.Show(this,
									string.Format(Properties.Resources.resfmtCantUpdateFalsestartRules, ex.Message),
									Title,
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return false;
				}

				Modified = false;
			}

			return true;
		}


		private void lstvRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}
