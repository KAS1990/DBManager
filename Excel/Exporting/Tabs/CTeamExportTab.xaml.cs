using DBManager.Global;
using DBManager.ReportGenerators;
using DBManager.Scanning.XMLDataClasses;
using DBManager.SettingsWriter;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DBManager.Excel.Exporting.Tabs
{
    /// <summary>
    /// Командный зачёт
    /// </summary>
    public partial class CTeamExportTab : CExportingTabBase
    {
        #region MenInGroup
        private static readonly string MenInGroupPropertyName = GlobalDefines.GetPropertyName<CTeamExportTab>(m => m.MenInGroup);

        private int m_MenInGroup = 0;

        public int MenInGroup
        {
            get { return m_MenInGroup; }
            set
            {
                if (m_MenInGroup != value)
                {
                    m_MenInGroup = value;
                    OnPropertyChanged(MenInGroupPropertyName);
                }
            }
        }
        #endregion


        #region WomenInGroup
        private static readonly string WomenInGroupPropertyName = GlobalDefines.GetPropertyName<CTeamExportTab>(m => m.WomenInGroup);

        private int m_WomenInGroup = 0;

        public int WomenInGroup
        {
            get { return m_WomenInGroup; }
            set
            {
                if (m_WomenInGroup != value)
                {
                    m_WomenInGroup = value;
                    OnPropertyChanged(WomenInGroupPropertyName);
                }
            }
        }
        #endregion


        private descriptions CompDesc
        {
            get { return m_ParentWnd == null ? null : m_ParentWnd.m_CompDesc; }
        }

        #region Teams
        private static readonly string TeamsPropertyName = GlobalDefines.GetPropertyName<CTeamExportTab>(m => m.Teams);
        private ObservableCollection<CTeamItem> m_Teams = new ObservableCollection<CTeamItem>();
        // Список всех комнд соревнования
        public ObservableCollection<CTeamItem> Teams
        {
            get { return m_Teams; }
            private set
            {
                m_Teams = value;
                OnPropertyChanged(TeamsPropertyName);
            }
        }
        #endregion

        #region TeamsForReport
        private readonly ObservableCollection<CTeamForReportItem> m_TeamsForReport = new ObservableCollection<CTeamForReportItem>();
        // Список комнд, по которым будет составлен командный зачёт
        public ObservableCollection<CTeamForReportItem> TeamsForReport
        {
            get { return m_TeamsForReport; }
        }
        #endregion


        public CTeamExportTab()
        {
            InitializeComponent();
        }


        public CTeamExportTab(CExportToExcelWnd ParentWnd, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups) :
            base(ParentWnd, CompGroups)
        {
            InitializeComponent();

            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

                foreach (groups gr1 in CompDesc.groups)
                {
                    CKeyValuePairEx<long, CCompSettings> GroupInfo;
                    if (CompGroups.TryGetValue(gr1.id_group, out GroupInfo))
                    {
                        switch (GroupInfo.Value.SecondColNameType)
                        {
                            case enSecondColNameType.Team:
                                // Добавляем в Teams тех команд, которых там нет
                                foreach (teams team in (from part in gr1.participations
                                                        join tm in DBManagerApp.m_Entities.teams on part.team equals tm.id_team
                                                        join tm2 in Teams on tm.id_team equals tm2.id into allTeams
                                                        from tm2 in allTeams.DefaultIfEmpty()
                                                        where tm2 == null
                                                        select tm).Distinct().ToList())
                                {
                                    Teams.Add(new CTeamItem(team.id_team)
                                    {
                                        Name = team.name
                                    });
                                }
                                break;

                            case enSecondColNameType.Coach:
                                // Добавляем в Teams тех команд, которых там нет
                                foreach (coaches coach in (from part in gr1.participations
                                                           join coach in DBManagerApp.m_Entities.coaches on part.coach equals coach.id_coach
                                                           join coach2 in Teams on coach.id_coach equals coach2.id into allCoaches
                                                           from coach2 in allCoaches.DefaultIfEmpty()
                                                           where coach2 == null
                                                           select coach).Distinct().ToList())
                                {
                                    Teams.Add(new CTeamItem(coach.id_coach)
                                    {
                                        Name = coach.name
                                    });
                                }
                                break;
                        }
                    }
                }
                Teams = new ObservableCollection<CTeamItem>(Teams.OrderBy(arg => arg.Name));

                CCompSpecificSets CompSettings;
                if (settings.dictCompSettings.TryGetValue(CompDesc.id_desc, out CompSettings))
                {
                    if (CompSettings.WomenInGroup != GlobalDefines.DEFAULT_XML_INT_VAL)
                        WomenInGroup = CompSettings.WomenInGroup;
                    else
                        WomenInGroup = settings.DefaultCompSettings.WomenInGroup;

                    if (CompSettings.MenInGroup != GlobalDefines.DEFAULT_XML_INT_VAL)
                        MenInGroup = CompSettings.MenInGroup;
                    else
                        MenInGroup = settings.DefaultCompSettings.MenInGroup;

                    // Удаляем команды, которых больше нет в соревновании и
                    // заполняем свойства, которые будут использованы для вывода команд на экран
                    if (CompGroups.Count > 0 && CompSettings.lstTeamsForTeamReport != null)
                    {
                        bool AppSettingsChanged = false;
                        for (int n = 0; n < CompSettings.lstTeamsForTeamReport.Count;)
                        {
                            if (CompSettings.lstTeamsForTeamReport[n].SubteamsIds != null)
                            {
                                CTeamForReportItem TeamForReport = new CTeamForReportItem(TeamForReportItem_CommandHandler)
                                {
                                    Name = CompSettings.lstTeamsForTeamReport[n].Name,
                                };

                                for (int m = 0; m < CompSettings.lstTeamsForTeamReport[n].SubteamsIds.Count;)
                                {
                                    long TeamId = CompSettings.lstTeamsForTeamReport[n].SubteamsIds[m];
                                    CTeamItem TeamItem = Teams.FirstOrDefault(arg => arg.id == TeamId);
                                    if (TeamItem == null)
                                    {   // Такой команды больше нет
                                        CompSettings.lstTeamsForTeamReport[n].SubteamsIds.RemoveAt(m);
                                        AppSettingsChanged = true;
                                    }
                                    else
                                    {
                                        TeamForReport.Subteams.Add(new CTeamItem(TeamId)
                                        {
                                            Name = TeamItem.Name.Trim()
                                        });
                                        Teams.Remove(TeamItem); // Команды в Teams и TeamsForReport повторяться не должны
                                        m++;
                                    }
                                }
                                if (CompSettings.lstTeamsForTeamReport[n].SubteamsIds.Count == 0)
                                {   // Команд больше не осталось 
                                    CompSettings.lstTeamsForTeamReport.RemoveAt(n);
                                    AppSettingsChanged = true;
                                }
                                else
                                {
                                    TeamsForReport.Add(TeamForReport);
                                    n++;
                                }
                            }
                            else
                            {
                                CompSettings.lstTeamsForTeamReport.RemoveAt(n);
                                AppSettingsChanged = true;
                            }
                        }

                        if (AppSettingsChanged)
                            DBManagerApp.m_AppSettings.Write();
                    }
                }
                else
                {
                    WomenInGroup = settings.DefaultCompSettings.WomenInGroup;
                    MenInGroup = settings.DefaultCompSettings.MenInGroup;
                }
            }
        }

        private void TeamForReportItem_CommandHandler(CTeamForReportItem sender)
        {
            foreach (CTeamItem SelectedTeam in Teams.Where(arg => arg.IsSelected).ToList())
            {
                Teams.Remove(SelectedTeam);

                SelectedTeam.IsSelected = false; // В правой части команда не должна быть выделена
                sender.Subteams.Add(SelectedTeam);
            }
        }


        public override bool CheckSettings()
        {
            if (CreateReport)
            {
                if (ExportToAnotherWbk)
                {
                    if (string.IsNullOrWhiteSpace(XlsPath) ||
                        !Directory.Exists(System.IO.Path.GetDirectoryName(XlsPath)) ||
                        System.IO.Path.GetExtension(XlsPath) != GlobalDefines.XLSX_EXTENSION)
                    {
                        MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resInvalidXlsxPath,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return false;
                    }

                    if (File.Exists(XlsPath))
                    {   // Перезаписывать файлы нельзя
                        MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resXlsxFileAlreadyExists,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        return false;
                    }
                }

                if (TeamsForReport.Count == 0)
                {   // Ничего не выделено
                    MessageBox.Show(m_ParentWnd,
                                        Properties.Resources.resNoTeamsToExport,
                                        (Parent as TabItem).Header.ToString(),
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    return false;
                }

                if (!txtMenInGroup.IsRightInput || !txtWomenInGroup.IsRightInput)
                    return false;
            }

            return true;
        }


        public override void BeforeExporting()
        {
            if (CreateReport)
            {
                lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                {
                    AppSettings settings = DBManagerApp.m_AppSettings.m_Settings;

                    CCompSpecificSets CompSettings = null;
                    if (!settings.dictCompSettings.TryGetValue(CompDesc.id_desc, out CompSettings))
                    {
                        CompSettings = new CCompSpecificSets()
                        {
                            CompId = CompDesc.id_desc,
                        };
                    }

                    CompSettings.WomenInGroup = WomenInGroup;
                    CompSettings.MenInGroup = MenInGroup;
                    CompSettings.lstTeamsForTeamReport = new List<CTeamForTeamReport>();

                    foreach (CTeamForReportItem TeamForReport in TeamsForReport)
                    {
                        CTeamForTeamReport TeamToAdd = new CTeamForTeamReport()
                        {
                            Name = TeamForReport.Name.Trim(),
                            SubteamsIds = new List<long>()
                        };
                        foreach (CTeamItem Team in TeamForReport.Subteams)
                        {
                            TeamToAdd.SubteamsIds.Add(Team.id);
                        }
                        CompSettings.lstTeamsForTeamReport.Add(TeamToAdd);
                    }
                    settings.dictCompSettings.TryAddValue(CompSettings.CompId, CompSettings);
                }

                DBManagerApp.m_AppSettings.Write();
            }
        }


        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            string SelectedPath;

            if (BaseBrowse_Click(out SelectedPath, Properties.Resources.resXlsxFilesFilter, false))
                XlsPath = SelectedPath;
        }


        /// <summary>
        /// Переносим команды в правую часть
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            CTeamForReportItem TeamForReport = new CTeamForReportItem(TeamForReportItem_CommandHandler);
            foreach (CTeamItem SelectedTeam in Teams.Where(arg => arg.IsSelected).ToList())
            {
                Teams.Remove(SelectedTeam);

                TeamForReport.Name += SelectedTeam.Name + " ";
                SelectedTeam.IsSelected = false; // В правой части команда не должна быть выделена
                TeamForReport.Subteams.Add(SelectedTeam);
            }
            TeamForReport.Name = TeamForReport.Name.Trim();

            if (TeamForReport.Subteams.Count > 0)
            {  // Что-то выделили в левой части
                TeamsForReport.Add(TeamForReport);
            }
        }


        /// <summary>
        /// Переносим команды в левую часть
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < TeamsForReport.Count;)
            {
                CTeamForReportItem TeamForReport = TeamsForReport[n];
                for (int m = 0; m < TeamForReport.Subteams.Count;)
                {
                    CTeamItem TeamR = TeamForReport.Subteams[m];
                    if (TeamR.IsSelected)
                    {   // Переносим команду влево
                        TeamForReport.Subteams.RemoveAt(m);
                        TeamR.IsSelected = false;
                        Teams.Add(TeamR);
                    }
                    else
                        m++;
                }

                if (TeamForReport.Subteams.Count == 0)
                    TeamsForReport.RemoveAt(n);
                else
                    n++;
            }

            Teams = new ObservableCollection<CTeamItem>(Teams.OrderBy(arg => arg.Name));
        }


        /// <summary>
        /// Переносим команды в правую часть "по одной"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllByOne_Click(object sender, RoutedEventArgs e)
        {
            foreach (CTeamItem Team in Teams.ToList())
            {
                CTeamForReportItem TeamForReport = new CTeamForReportItem(TeamForReportItem_CommandHandler)
                {
                    Name = Team.Name,
                };
                Teams.Remove(Team);

                Team.IsSelected = false; // В правой части команда не должна быть выделена
                TeamForReport.Subteams.Add(Team);
                TeamsForReport.Add(TeamForReport);
            }
        }


        /// <summary>
        /// Очитстить правый список
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearTeamsForReport_Click(object sender, RoutedEventArgs e)
        {
            foreach (CTeamForReportItem TeamForReport in TeamsForReport)
            {
                foreach (CTeamItem TeamR in TeamForReport.Subteams)
                    Teams.Add(TeamR);
            }

            Teams = new ObservableCollection<CTeamItem>(Teams.OrderBy(arg => arg.Name));
            TeamsForReport.Clear();
        }
    }
}
