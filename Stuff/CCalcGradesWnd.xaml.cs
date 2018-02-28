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
using DBManager.Scanning.DBAdditionalDataClasses;
using System.Collections.ObjectModel;
using DBManager.RoundResultsControl.FilterControl;
using System.ComponentModel;
using DBManager.Global;
using DBManager.RoundMembers.Converters;
using DBManager.SettingsWriter;

namespace DBManager.Stuff
{
	/// <summary>
	/// Interaction logic for CCalcGradesWnd.xaml
	/// </summary>
	public partial class CCalcGradesWnd : CNotifyPropertyChangedWnd
	{
		readonly List<CMemberInTotal> m_CompMembers = null;

		readonly Dictionary<enGrade, Label> m_dictLblsPlaces = new Dictionary<enGrade, Label>();
		readonly long m_GroupId = -1;

		List<KeyValuePair<enGrade, int>> m_MinPlaceForNewGrade = new List<KeyValuePair<enGrade, int>>();

		#region FilterCathegories
		readonly private ObservableCollection<FilterPredicate> m_CompYears = new ObservableCollection<FilterPredicate>();
		/// <summary>
		/// Словарь, содержащий все года рождения участников
		/// </summary>
		public ObservableCollection<FilterPredicate> CompYears
		{
			get { return m_CompYears; }
		}
		#endregion

		
		#region CalcEnabled
		private static readonly string CalcEnabledPropertyName = GlobalDefines.GetPropertyName<CCalcGradesWnd>(m => m.CalcEnabled);

		private bool m_CalcEnabled = false;

		public bool CalcEnabled
		{
			get { return m_CalcEnabled; }
			set
			{
				if (m_CalcEnabled != value)
				{
					m_CalcEnabled = value;
					OnPropertyChanged(CalcEnabledPropertyName);
				}
			}
		}
		#endregion


		List<int> m_SelectedYears = new List<int>();
		private List<int> SelectedYears
		{
			get
			{
				if (m_SelectedYears.Count == 0)
				{
					m_SelectedYears = (from year in CompYears
									   where year.IsSelected
									   select (int)year.FilterValue).ToList();
				}

				return m_SelectedYears;
			}
		}

		
		#region GradesChangedFromOpen
		private bool m_GradesChangedFromOpen = false;

		public bool GradesChangedFromOpen
		{
			get { return m_GradesChangedFromOpen; }
			private set
			{
				if (m_GradesChangedFromOpen != value)
					m_GradesChangedFromOpen = value;
			}
		}
		#endregion
				

		public CCalcGradesWnd(long GroupId, List<CMemberInTotal> CompMembers)
		{
			InitializeComponent();

			m_CompMembers = CompMembers;
			m_GroupId = GroupId;

			foreach (int YearOfBirth in (from member in m_CompMembers
											where member.MemberInfo.YearOfBirth.HasValue
											group member by member.MemberInfo.YearOfBirth.Value into memberYearOfBirths
											orderby memberYearOfBirths.Key
											select memberYearOfBirths.Key))
			{
				FilterPredicate NewPredicate = new FilterPredicate()
				{
					Name = YearOfBirth.ToString(),
					FilterValue = YearOfBirth,
					IsSelected = YearOfBirth <= (DateTime.Today.Year - DBManagerApp.m_AppSettings.m_Settings.MinAgeToCalcResultGrade)
				};
				NewPredicate.PropertyChanged += CompYear_PropertyChanged;
				CompYears.Add(NewPredicate);
			}

			CompYear_PropertyChanged(this, null);

			GlobalDefines.TuneComboboxWidth2(cmbResultGradeCalcMethod);

			int row = 0;
			foreach (KeyValuePair<enGrade, string> Grade in GlobalDefines.GRADE_NAMES)
			{
				if (Grade.Key == enGrade.None ||
					Grade.Key == enGrade.WithoutGrade ||
					Grade.Key == enGrade.Master ||
					Grade.Key == enGrade.BeforeMaster)
				{
					continue;
				}

				Label lbl = new Label()
				{
					Content = Grade.Value,
					Background = Grade.Key < enGrade.Adult3 ?
									new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle.BackgroundColor) :
									new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle.BackgroundColor),
					BorderThickness = new Thickness(1, 0, 1, 1),
					BorderBrush = Brushes.Gray
				};
				Grid.SetColumn(lbl, 0);
				Grid.SetRow(lbl, row + 1);
				grdCalcGradesResults.Children.Add(lbl);

				lbl = new Label()
				{
					Background = Grade.Key < enGrade.Adult3 ?
									new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle.BackgroundColor) :
									new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle.BackgroundColor),
					BorderThickness = new Thickness(0, 0, 1, 1),
					BorderBrush = Brushes.Gray
				};
				Grid.SetColumn(lbl, 1);
				Grid.SetRow(lbl, row + 1);
				grdCalcGradesResults.Children.Add(lbl);

				m_dictLblsPlaces.Add(Grade.Key, lbl);
				
				row++;
			}

			cmbResultGradeCalcMethod.SelectedIndex = (int)DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod;
			chkOnly75PercentForCalcGrades.IsChecked = DBManagerApp.m_AppSettings.m_Settings.Only75PercentForCalcGrades;

			btnCalcPlaces_Click(this, null);
		}


		private void CompYear_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (CompYears.All(arg => arg.IsSelected))
			{	// Все элементы выбраны
				chkSelectAll.IsChecked = true;
			}
			else if (CompYears.All(arg => !arg.IsSelected))
			{	// Все элементы не выбраны
				chkSelectAll.IsChecked = false;
			}
			else
			{	// Что-то выбрано, а что-то нет
				chkSelectAll.IsChecked = null;
			}

			CalcEnabled = CompYears.Count > 0 && (chkSelectAll.IsChecked == null || chkSelectAll.IsChecked.Value);
			SelectedYears.Clear();
		}


		private void chkSelectAll_Click(object sender, RoutedEventArgs e)
		{
			if (chkSelectAll.IsChecked.HasValue)
			{
				foreach (FilterPredicate cathegory in CompYears)
				{
					cathegory.PropertyChanged -= CompYear_PropertyChanged;
					cathegory.IsSelected = chkSelectAll.IsChecked.Value;
					cathegory.PropertyChanged += CompYear_PropertyChanged;
				}

				CalcEnabled = CompYears.Count > 0 && (chkSelectAll.IsChecked == null || chkSelectAll.IsChecked.Value);
				SelectedYears.Clear();
			}
		}


		private void btnCalcPlaces_Click(object sender, RoutedEventArgs e)
		{
			// Присвоение разрядов
			IEnumerable<CMemberInTotal> MembersForGradesCalc = from member in m_CompMembers
															   where member.MemberInfo.InitGrade.HasValue && SelectedYears.Contains(member.MemberInfo.YearOfBirth.Value)
															   where member.Place.HasValue
															   select member;
			if (chkOnly75PercentForCalcGrades.IsChecked.Value)
			{	// Учитываем только 75% участников
				MembersForGradesCalc = (from member in MembersForGradesCalc
										orderby member.Place
										select member).Take((int)(Math.Floor(MembersForGradesCalc.Count() * 0.75)));
			}
			Dictionary<enGrade?, int> GradesStat = (from member in MembersForGradesCalc
													group member by member.MemberInfo.InitGrade into MembersGrades
													select new DBManager.Scanning.CGroupResultsManager.CGradeStat
													{
														Grade = (enGrade?)MembersGrades.Key,
														MembersWithGrade = MembersGrades.Count(arg => arg.MemberInfo.InitGrade == MembersGrades.Key)
													}).ToDictionary(key => key.Grade, item => item.MembersWithGrade);
			int tmp = 0;

			for (enGrade grade = enGrade.WithoutGrade; grade <= enGrade.Master; grade++)
			{
				if (!GradesStat.TryGetValue(grade, out tmp))
					GradesStat[grade] = 0;
			}

			m_MinPlaceForNewGrade.Clear();

			// 1 разряд
			tmp = GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
											1.0 * GradesStat[enGrade.Master] +
											0.8 * GradesStat[enGrade.BeforeMaster] +
											0.4 * GradesStat[enGrade.Adult1] +
											0.2 * GradesStat[enGrade.Adult2]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult1, tmp));
			m_dictLblsPlaces[enGrade.Adult1].Content = tmp == 0 ? null : tmp.ToString();

			// 2 разряд
			tmp += GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
														0.2 * GradesStat[enGrade.Adult1] +
														0.4 * GradesStat[enGrade.Adult2] +
														0.2 * GradesStat[enGrade.Adult3]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult2, tmp));
			m_dictLblsPlaces[enGrade.Adult2].Content = tmp == 0 ? null : tmp.ToString();

			// 3 разряд
			tmp += GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
														0.2 * GradesStat[enGrade.Adult2] +
														0.4 * GradesStat[enGrade.Adult3] +
														0.3 * GradesStat[enGrade.Young1]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult3, tmp));
			m_dictLblsPlaces[enGrade.Adult3].Content = tmp == 0 ? null : tmp.ToString();

			// 1 ю разряд
			tmp += GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
														0.2 * GradesStat[enGrade.Adult3] +
														0.4 * GradesStat[enGrade.Young1] +
														0.2 * GradesStat[enGrade.Young2]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young1, tmp));
			m_dictLblsPlaces[enGrade.Young1].Content = tmp == 0 ? null : tmp.ToString();

			// 2 ю разряд
			tmp += GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
														0.2 * GradesStat[enGrade.Young1] +
														0.4 * GradesStat[enGrade.Young2] +
														0.2 * GradesStat[enGrade.Young3]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young2, tmp));
			m_dictLblsPlaces[enGrade.Young2].Content = tmp == 0 ? null : tmp.ToString();

			// 3 ю разряд
			tmp += GlobalDefines.CalcMinPlaceForNewGrade((enResultGradeCalcMethod)cmbResultGradeCalcMethod.SelectedIndex,
														0.2 * GradesStat[enGrade.Young2] +
														0.4 * GradesStat[enGrade.Young3] +
														0.3 * GradesStat[enGrade.WithoutGrade]);
			m_MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young3, tmp));
			m_dictLblsPlaces[enGrade.Young3].Content = tmp == 0 ? null : tmp.ToString();
		}

		private void btnSetGrades_Click(object sender, RoutedEventArgs e)
		{
			btnCalcPlaces_Click(sender, e);

			if (GlobalDefines.IsRoundFinished(DBManagerApp.m_Entities.groups.First(arg => arg.id_group == m_GroupId).round_finished_flags, enRounds.Final))
			{	// Можно расставить разряды, т.к. соревы закончились
				List<participations> Members = (from member in DBManagerApp.m_Entities.members
												join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
												where part.Group == m_GroupId
												orderby part.result_place
												select part).ToList();
				if (chkOnly75PercentForCalcGrades.IsChecked.Value)
					Members = new List<participations>(Members.Take((int)Math.Floor(Members.Count * 0.75)));
				byte PlaceInYear = 1;

				foreach (participations part in Members)
				{
					part.result_grade = null;

					if (part.result_place.HasValue && part.members.year_of_birth.HasValue && SelectedYears.Contains(part.members.year_of_birth.Value))
					{	// Участник подходит по возрасту
						for (int i = 0; i < m_MinPlaceForNewGrade.Count; i++)
						{
							if (PlaceInYear <= m_MinPlaceForNewGrade[i].Value)
							{
								part.result_grade = (byte)m_MinPlaceForNewGrade[i].Key;
								break;
							}
						}

						PlaceInYear++;
					}
				}

				DBManagerApp.m_Entities.SaveChanges();

				GradesChangedFromOpen = true;
			}
			else
			{
				MessageBox.Show(this, Properties.Resources.resCantSetGrades, Title, MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
