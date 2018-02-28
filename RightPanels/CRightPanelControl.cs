using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using DBManager.SettingsWriter;
using System.Windows.Media;

namespace DBManager.RightPanels
{
	/// <summary>
	/// Базовый класс для правой панели
	/// </summary>
	public class CRightPanelControl : Control, INotifyPropertyChanged
	{
		ResourceDictionary m_GlobalResources = new ResourceDictionary()
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
		private static readonly string NextRoundMembersCountFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.NextRoundMembersCountFontStyle);

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
		private static readonly string InvitedToStartFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.InvitedToStartFontStyle);

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
		private static readonly string PreparingFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PreparingFontStyle);

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
		private static readonly string StayOnStartFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.StayOnStartFontStyle);

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

		
		#region JustRecievedResultFontStyle
		private static readonly string JustRecievedResultFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.JustRecievedResultFontStyle);

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
		private static readonly string InvitedToStartMemberPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.InvitedToStartMember);

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
		private static readonly string PreparingMemberPropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PreparingMember);

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
		private static readonly string PlainResultsFontStylePropertyName = GlobalDefines.GetPropertyName<CRightPanelControl>(m => m.PlainResultsFontStyle);

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

			if (m_GlobalResources != null && m_GlobalResources.Count > 0)
			{
				if (m_GlobalResources["PlainResultsBrush"] is SolidColorBrush)
				{
					m_PlainResultsFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["PlainResultsBrush"] as SolidColorBrush).Color
					};
				}

				if (m_GlobalResources["NextRoundMembersCountBrush"] is SolidColorBrush)
				{
					m_NextRoundMembersCountFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["NextRoundMembersCountBrush"] as SolidColorBrush).Color
					};
				}

				if (m_GlobalResources["InvitedToStartBrush"] is SolidColorBrush)
				{
					m_InvitedToStartFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["InvitedToStartBrush"] as SolidColorBrush).Color
					};
				}

				if (m_GlobalResources["PreparingBrush"] is SolidColorBrush)
				{
					m_PreparingFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["PreparingBrush"] as SolidColorBrush).Color
					};
				}

				if (m_GlobalResources["StayOnStartBrush"] is SolidColorBrush)
				{
					m_StayOnStartFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["StayOnStartBrush"] as SolidColorBrush).Color
					};
				}

				if (m_GlobalResources["JustRecievedResultBrush"] is SolidColorBrush)
				{
					m_JustRecievedResultFontStyle = new CFontStyleSettings()
					{
						BackgroundColor = (m_GlobalResources["JustRecievedResultBrush"] as SolidColorBrush).Color
					};
				}
			}
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
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}
