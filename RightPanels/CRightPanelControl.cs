using DBManager.Global;
using DBManager.SettingsWriter;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DBManager.RightPanels
{
    /// <summary>
    /// Базовый класс для правой панели
    /// </summary>
    public class CRightPanelControl : Control, INotifyPropertyChanged
    {
        private readonly ResourceDictionary m_GlobalResources = new ResourceDictionary()
        {
            Source = new Uri("\\Global\\GlobalResources.xaml", UriKind.RelativeOrAbsolute)
        };


        #region WholeMembersQ
        private static readonly string WholeMembersQPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.WholeMembersQ);

        private int m_WholeMembersQ = 0;

        public int WholeMembersQ
        {
            get { return m_WholeMembersQ; }
            set
            {
                if (m_WholeMembersQ != value)
                {
                    m_WholeMembersQ = value;
                    OnPropertyChanged("WholeMembersQ");
                }
            }
        }
        #endregion


        #region RoundMembersQ
        private static readonly string RoundMembersQPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.RoundMembersQ);

        private int m_RoundMembersQ = 0;
        /// <summary>
        /// Число учасников в раунде
        /// </summary>
        public int RoundMembersQ
        {
            get { return m_RoundMembersQ; }
            set
            {
                if (m_RoundMembersQ != value)
                {
                    m_RoundMembersQ = value;
                    OnPropertyChanged(RoundMembersQPropertyName);
                }
            }
        }
        #endregion


        #region NextRoundMembersQ
        private static readonly string NextRoundMembersQPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.NextRoundMembersQ);

        private int m_NextRoundMembersQ = 0;
        /// <summary>
        /// Число участников, проходящих в следующий тур
        /// </summary>
        public int NextRoundMembersQ
        {
            get { return m_NextRoundMembersQ; }
            set
            {
                if (m_NextRoundMembersQ != value)
                {
                    m_NextRoundMembersQ = value;
                    OnPropertyChanged(NextRoundMembersQPropertyName);
                }
            }
        }
        #endregion


        #region FilteredMembersQ
        private static readonly string FilteredMembersQPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.FilteredMembersQ);

        private int? m_FilteredMembersQ = null;
        /// <summary>
        /// Число отфильтрованный участников 
        /// </summary>
        public int? FilteredMembersQ
        {
            get { return m_FilteredMembersQ; }
            set
            {
                if (m_FilteredMembersQ != value)
                {
                    m_FilteredMembersQ = value;
                    OnPropertyChanged(FilteredMembersQPropertyName);
                }
            }
        }
        #endregion


        #region NextRoundMembersCountFontStyle
        public static readonly string NextRoundMembersCountFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.NextRoundMembersCountFontStyle);

        private CFontStyleSettings m_NextRoundMembersCountFontStyle = new CFontStyleSettings();

        public CFontStyleSettings NextRoundMembersCountFontStyle
        {
            get { return m_NextRoundMembersCountFontStyle; }
            set
            {
                if (m_NextRoundMembersCountFontStyle != value)
                {
                    m_NextRoundMembersCountFontStyle = value;
                    OnPropertyChanged(NextRoundMembersCountFontStylePropertyName);
                }
            }
        }
        #endregion


        #region InvitedToStartFontStyle
        public static readonly string InvitedToStartFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.InvitedToStartFontStyle);

        private CFontStyleSettings m_InvitedToStartFontStyle = new CFontStyleSettings();

        public CFontStyleSettings InvitedToStartFontStyle
        {
            get { return m_InvitedToStartFontStyle; }
            set
            {
                if (m_InvitedToStartFontStyle != value)
                {
                    m_InvitedToStartFontStyle = value;
                    OnPropertyChanged(InvitedToStartFontStylePropertyName);
                }
            }
        }
        #endregion


        #region PreparingFontStyle
        public static readonly string PreparingFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PreparingFontStyle);

        private CFontStyleSettings m_PreparingFontStyle = new CFontStyleSettings();

        public CFontStyleSettings PreparingFontStyle
        {
            get { return m_PreparingFontStyle; }
            set
            {
                if (m_PreparingFontStyle != value)
                {
                    m_PreparingFontStyle = value;
                    OnPropertyChanged(PreparingFontStylePropertyName);
                }
            }
        }
        #endregion


        #region StayOnStartFontStyle
        public static readonly string StayOnStartFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.StayOnStartFontStyle);

        private CFontStyleSettings m_StayOnStartFontStyle = new CFontStyleSettings();

        public CFontStyleSettings StayOnStartFontStyle
        {
            get { return m_StayOnStartFontStyle; }
            set
            {
                if (m_StayOnStartFontStyle != value)
                {
                    m_StayOnStartFontStyle = value;
                    OnPropertyChanged(StayOnStartFontStylePropertyName);
                }
            }
        }
        #endregion


        #region FalsestartFontStyle
        public static readonly string FalsestartFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.FalsestartFontStyle);

        private CFontStyleSettings m_FalsestartFontStyle = new CFontStyleSettings();

        public CFontStyleSettings FalsestartFontStyle
        {
            get { return m_FalsestartFontStyle; }
            set
            {
                if (m_FalsestartFontStyle != value)
                {
                    m_FalsestartFontStyle = value;
                    OnPropertyChanged(FalsestartFontStylePropertyName);
                }
            }
        }
        #endregion


        #region JustRecievedResultFontStyle
        public static readonly string JustRecievedResultFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.JustRecievedResultFontStyle);

        private CFontStyleSettings m_JustRecievedResultFontStyle = new CFontStyleSettings();

        public CFontStyleSettings JustRecievedResultFontStyle
        {
            get { return m_JustRecievedResultFontStyle; }
            set
            {
                if (m_JustRecievedResultFontStyle != value)
                {
                    m_JustRecievedResultFontStyle = value;
                    OnPropertyChanged(JustRecievedResultFontStylePropertyName);
                }
            }
        }
        #endregion


        #region InvitedToStartMember
        public static readonly string InvitedToStartMemberPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.InvitedToStartMember);

        private string m_InvitedToStartMember = null;

        public string InvitedToStartMember
        {
            get { return m_InvitedToStartMember; }
            set
            {
                if (m_InvitedToStartMember != value)
                {
                    m_InvitedToStartMember = value;
                    OnPropertyChanged(InvitedToStartMemberPropertyName);
                }
            }
        }
        #endregion


        #region PreparingMember
        public static readonly string PreparingMemberPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PreparingMember);

        private string m_PreparingMember = null;

        public string PreparingMember
        {
            get { return m_PreparingMember; }
            set
            {
                if (m_PreparingMember != value)
                {
                    m_PreparingMember = value;
                    OnPropertyChanged(PreparingMemberPropertyName);
                }
            }
        }
        #endregion


        #region PlainResultsFontStyle
        public static readonly string PlainResultsFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PlainResultsFontStyle);

        private CFontStyleSettings m_PlainResultsFontStyle = new CFontStyleSettings();

        public CFontStyleSettings PlainResultsFontStyle
        {
            get { return m_PlainResultsFontStyle; }
            set
            {
                if (m_PlainResultsFontStyle != value)
                {
                    m_PlainResultsFontStyle = value;
                    OnPropertyChanged(PlainResultsFontStylePropertyName);
                }
            }
        }
        #endregion


        public CRightPanelControl()
        {
            DataContext = this;

            m_PlainResultsFontStyle = DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle;
            m_NextRoundMembersCountFontStyle = DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle;
            m_InvitedToStartFontStyle = DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle;
            m_PreparingFontStyle = DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle;
            m_StayOnStartFontStyle = DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle;
            m_JustRecievedResultFontStyle = DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle;
            m_FalsestartFontStyle = DBManagerApp.m_AppSettings.m_Settings.FalsestartFontStyle;
        }


        static CRightPanelControl()
        {
            // Чтобы WPF могла применять стили к компоненту
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CRightPanelControl), new FrameworkPropertyMetadata(typeof(CRightPanelControl)));
        }


        public void ClearTemplate()
        {
            Template = null;
            InvitedToStartMember = PreparingMember = null;
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
