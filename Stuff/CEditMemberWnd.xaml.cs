using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using DBManager.SettingWnds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Stuff
{
    /// <summary>
    /// Interaction logic for CEditMemberWnd.xaml
    /// </summary>
    public partial class CEditMemberWnd : СCustomSettingsWnd
    {
        public EditMemberViewModel MemberData { get; private set; }

        #region Неизменяемые списки

        public List<string> Surnames { get; private set; }
        public List<string> Names { get; private set; } = new List<string>();
        public List<KeyValuePair<long, string>> SecondColumnData { get; private set; } = new List<KeyValuePair<long, string>>();
        public List<short> YearsOfBirth { get; private set; } = new List<short>();
        public Dictionary<enGrade, string> Grades { get; private set; } = new Dictionary<enGrade, string>();

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
            /* Переводи фокус на кнопку ОК, чтобы сработала проверка во всех TextBoxEx.
			 * Затем проверяем результат проверки и ничего не делаем, если проверка прошла неудачно */
            IInputElement FocusedEl = FocusManager.GetFocusedElement(this);
            btnOK.Focus();

            if (SaveSettings())
            {
                DialogResult = ModifiedFromOpen;
            }
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


        public CEditMemberWnd()
        {
            InitializeComponent();

            ModifiedFromOpen = Modified = false;
        }


        public CEditMemberWnd(EditMemberViewModel memberToEdit, long groupId, CCompSettings compSettings)
        {
            DataContext = MemberData = memberToEdit;

            InitializeComponent();

            HasUnsavedChanges += () => { return Modified; };

            var membersWithSex = from member in DBManagerApp.m_Entities.members
                                 where member.sex == compSettings.AgeGroup.Sex
                                 select member;
            Surnames = membersWithSex
                        .Select(arg => arg.surname)
                        .OrderBy(arg => arg)
                        .Distinct()
                        .ToList();
            Names = membersWithSex
                        .Select(arg => arg.name)
                        .OrderBy(arg => arg)
                        .Distinct()
                        .ToList();

            SecondColumnData = (compSettings.SecondColNameType == enSecondColNameType.Coach
                        ? (from part in DBManagerApp.m_Entities.participations
                           join coach in DBManagerApp.m_Entities.coaches on part.coach equals coach.id_coach
                           where part.Group == groupId
                           orderby coach.name
                           select new
                           {
                               key = coach.id_coach,
                               value = coach.name
                           })
                        : (from part in DBManagerApp.m_Entities.participations
                           join team in DBManagerApp.m_Entities.teams on part.team equals team.id_team
                           where part.Group == groupId
                           orderby team.name
                           select new
                           {
                               key = team.id_team,
                               value = team.name
                           }))
                   .Distinct()
                   .ToList()
                   .Select(arg => new KeyValuePair<long, string>(arg.key, arg.value))
                   .ToList();

            int EndYear = 0, StartYear = 0;
            switch (compSettings.AgeGroup.wEndYear)
            {
                case (int)enEndYearSpecVals.AndElder:
                    StartYear = GlobalDefines.MIN_GROUP_YEAR;
                    EndYear = compSettings.AgeGroup.StartYear;
                    break;

                case (int)enEndYearSpecVals.AndYounger:
                    StartYear = compSettings.AgeGroup.StartYear;
                    EndYear = GlobalDefines.MAX_GROUP_YEAR;
                    break;

                default:
                    StartYear = compSettings.AgeGroup.StartYear;
                    EndYear = compSettings.AgeGroup.wEndYear;
                    break;
            }
            for (int i = StartYear; i <= EndYear; i++)
                YearsOfBirth.Add((short)i);

            Grades = GlobalDefines.GRADE_NAMES;

            lblSecondColumn.Content = compSettings.SecondColName;

            MemberData.PropertyChanged += (s, e) => Modified = true;

            ModifiedFromOpen = Modified = false;
        }


        private bool CheckSettings()
        {
            if (Modified)
            {
                if (string.IsNullOrEmpty(MemberData.Name) || string.IsNullOrEmpty(MemberData.Surname) || string.IsNullOrEmpty(MemberData.SecondColumn))
                {
                    MessageBox.Show(this, Properties.Resources.resRequiredFieldsAreEmpty, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
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

                string curMemberInfo = MemberData.MemberInDB.ToInfoString();
                if (MemberData.PartInDB.init_grade.HasValue)
                    curMemberInfo += " " + GlobalDefines.GRADE_NAMES[(enGrade)MemberData.PartInDB.init_grade.Value];
                curMemberInfo += " "
                        + (MemberData.SecondColNameType == enSecondColNameType.Coach
                            ? MemberData.PartInDB.coaches.name
                            : MemberData.PartInDB.teams.name);

                string newMemberInfo = MemberData.ToString();
                if (MessageBox.Show(this,
                                    string.Format(Properties.Resources.resfmtChangeMemberQuestion, curMemberInfo, newMemberInfo),
                                    Title,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question,
                                    MessageBoxResult.Yes) == MessageBoxResult.No)
                {
                    return false;
                }

                try
                {
                    var ChangedMembersInDB = (from member in DBManagerApp.m_Entities.members
                                              where member.surname == MemberData.Surname && member.name == MemberData.Name
                                              select member).FirstOrDefault();
                    if (ChangedMembersInDB == null || ChangedMembersInDB.id_member == MemberData.MemberInDB.id_member)
                    {   // Участника с такими данными нет в БД => просто меняем данные текущего участника
                        MemberData.MemberInDB.name = MemberData.Name;
                        MemberData.MemberInDB.surname = MemberData.Surname;
                    }
                    else
                    {   // Такой спортсмен уже есть в БД => используем его
                        MemberData.MemberInDB = ChangedMembersInDB;
                        MemberData.PartInDB.member = MemberData.MemberInDB.id_member;
                    }

                    MemberData.MemberInDB.year_of_birth = MemberData.YearOfBirth;
                    MemberData.PartInDB.init_grade = MemberData.Grade == enGrade.None ? null : (byte?)MemberData.Grade;
                    if (MemberData.SecondColNameType == enSecondColNameType.Coach)
                    {
                        MemberData.PartInDB.coach = GlobalDefines.GetCoachId(MemberData.SecondColumn, true);
                        MemberData.PartInDB.team = null;
                    }
                    else
                    {
                        MemberData.PartInDB.team = GlobalDefines.GetTeamId(MemberData.SecondColumn, true);
                        MemberData.PartInDB.coach = null;
                    }

                    DBManagerApp.m_Entities.SaveChanges();

                    Modified = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this,
                        $"{Properties.Resources.resExceptionDuringSaveMember}: {ex.ToString()}",
                        Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }
    }
}
