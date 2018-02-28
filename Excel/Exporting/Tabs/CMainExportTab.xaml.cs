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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBManager.Global;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Global.Converters;
using System.Globalization;
using DBManager.ReportGenerators;

namespace DBManager.Excel.Exporting.Tabs
{
	/// <summary>
	/// Interaction logic for CMainExportTab.xaml
	/// </summary>
	public partial class CMainExportTab : CExportingTabBase
	{
		#region Groups
		private ObservableCollection<CGroupItem> m_Groups = new ObservableCollection<CGroupItem>();

		public ObservableCollection<CGroupItem> Groups
		{
			get { return m_Groups; }
		}
		#endregion


		descriptions CompDesc
		{
			get { return m_ParentWnd == null ? null : m_ParentWnd.m_CompDesc; }
		}


		public List<CGroupItem> SelectedGroups
		{
			get
			{
				return new List<CGroupItem>(Groups.Where(group => group.Rounds.FirstOrDefault(round => round.IsSelected) != null));
			}
		}
		

		public CMainExportTab()
		{
			InitializeComponent();
		}


		public CMainExportTab(CExportToExcelWnd ParentWnd, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups) :
			base(ParentWnd, CompGroups)
		{
			InitializeComponent();

			CreateReport = true; // Эта вкладка по умолчанию будет активной

			if (CompDesc != null)
			{
				// Добавляем группы в колекцию
				foreach (KeyValuePair<long, CKeyValuePairEx<long, CCompSettings>> Group in CompGroups)
				{
					CGroupItem GroupItem = new CGroupItem(Group.Key)
					{
						Name = Group.Value.Value.AgeGroup.FullGroupName,
						Sex = Group.Value.Value.AgeGroup.SexInEnum
					};
					CRoundItem RoundItem = null;

					if (Group.Value.Value.AgeGroup.GroupType == CAgeGroup.enGroupType.Children)
					{	// Есть года рождения
						if (Group.Value.Value.AgeGroup.StartYear == GlobalDefines.DEFAULT_XML_INT_VAL ||
							Group.Value.Value.AgeGroup.wEndYear == GlobalDefines.DEFAULT_XML_INT_VAL)
						{	// Но пока их почему-то не ввели
							GroupItem.CanSelectYears = false;
						}
						else
						{
							int EndYear = 0, StartYear = 0;
							switch (Group.Value.Value.AgeGroup.wEndYear)
							{
								case (int)DBManager.Scanning.XMLDataClasses.CAgeGroup.enEndYearSpecVals.AndElder:
									StartYear = GlobalDefines.MIN_GROUP_YEAR;
									EndYear = Group.Value.Value.AgeGroup.StartYear;
									break;

								case (int)DBManager.Scanning.XMLDataClasses.CAgeGroup.enEndYearSpecVals.AndYounger:
									StartYear = Group.Value.Value.AgeGroup.StartYear;
									EndYear = GlobalDefines.MAX_GROUP_YEAR;
									break;

								default:
									StartYear = Group.Value.Value.AgeGroup.StartYear;
									EndYear = Group.Value.Value.AgeGroup.wEndYear;
									break;
							}
							for (int i = StartYear; i <= EndYear; i++)
								GroupItem.YearsOfBirth.Add(i);
							GroupItem.StartYearIndToExport = 0;
							GroupItem.EndYearIndToExport = GroupItem.YearsOfBirth.Count - 1;
						}
					}
					else
					{	// Годов рождения нет
						for (int i = GlobalDefines.MIN_GROUP_YEAR; i <= GlobalDefines.MAX_GROUP_YEAR; i++)
							GroupItem.YearsOfBirth.Add(i);
						GroupItem.StartYearIndToExport = 0;
						GroupItem.EndYearIndToExport = GroupItem.YearsOfBirth.Count - 1;
					}

					// Список всех раундов без итогового протокола
					foreach (dynamic RoundInfo in from result in DBManagerApp.m_Entities.results_speed
												  join part in DBManagerApp.m_Entities.participations on result.participation equals part.id_participation
												  where part.Group == Group.Key
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

						RoundItem = new CRoundItem((enRounds)RoundInfo.RoundID)
						{
							SheetName = GroupItem.Name + " " + RoundInfo.RoundName
						};
						RoundItem.SheetName = RoundItem.SheetName.Left(m_ParentWnd.MaxSheetNameLen);

						GroupItem.Rounds.Add(RoundItem);
					}

					RoundItem = new CRoundItem(enRounds.Total)
					{
						SheetName = GroupItem.Name // По умолчанию для итогового протокола
						// название листа будет равно названию группы
					};
					RoundItem.SheetName = RoundItem.SheetName.Left(m_ParentWnd.MaxSheetNameLen);

					GroupItem.Rounds.Add(RoundItem);

					Groups.Add(GroupItem);
				}
			}
		}


		public override bool CheckSettings()
		{
			if (CreateReport)
			{
				if (SelectedGroups.Count() == 0)
				{	// Ничего не выделено
					MessageBox.Show(m_ParentWnd,
										Properties.Resources.resNoRoundsToExport,
										(Parent as TabItem).Header.ToString(),
										MessageBoxButton.OK,
										MessageBoxImage.Error);
					return false;
				}

				if (SelectedGroups.Where(arg => arg.StartYearIndToExport > arg.EndYearIndToExport && arg.CanSelectYears).Count() > 0)
				{	// Есть ошибки в задании годов
					MessageBox.Show(m_ParentWnd,
									Properties.Resources.resInvalidYearsToExport,
									(Parent as TabItem).Header.ToString(),
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return false;
				}

				List<CRoundItem> AllSelectedRounds = new List<CRoundItem>();
				foreach (CGroupItem SelectedGroup in SelectedGroups)
				{
					AllSelectedRounds.AddRange(SelectedGroup.Rounds.Where(arg => arg.IsSelected));
				}
				if ((from Round in AllSelectedRounds
					 group Round by Round.SheetName into AllSelectedSheets
					 where AllSelectedSheets.Count() > 1
					 select AllSelectedSheets.Count()).Count() > 0)
				{	// Есть повторяющие названия листов
					MessageBox.Show(m_ParentWnd,
									Properties.Resources.resDuplicateSheetNames,
									(Parent as TabItem).Header.ToString(),
									MessageBoxButton.OK,
									MessageBoxImage.Error);
					return false;
				}
			}

			return true;
		}
	}


	public class ShowBallsInTotalConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value is enRounds)
				return (enRounds)value == enRounds.Total ? Visibility.Visible : Visibility.Collapsed;
			else
				return Visibility.Collapsed;
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in ShowBallsInTotalConverter");
		}


		public ShowBallsInTotalConverter() :
			base()
		{
		}
	}
}
