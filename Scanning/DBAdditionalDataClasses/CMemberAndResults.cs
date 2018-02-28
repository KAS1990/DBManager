using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System.Windows;
using DBManager.RoundMembers.Converters;
using System.Globalization;
using System.Windows.Media;
using System.ComponentModel;
using DBManager.SettingsWriter;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CMemberAndResults : CDBAdditionalClassBase
	{
		#region MemberInfo
		private static readonly string MemberInfoPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.MemberInfo);

		private CFullMemberInfo m_MemberInfo = null;
		/// <summary>
		/// Сведения об участнике
		/// </summary>
		public CFullMemberInfo MemberInfo
		{
			get { return m_MemberInfo; }
			set
			{
				if (m_MemberInfo != value)
				{
					m_MemberInfo = value;
					OnPropertyChanged(MemberInfoPropertyName);
				}
			}
		}
		#endregion


		#region Results
		private static readonly string ResultsPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.Results);

		private COneRoundResults m_Results = null;

		public COneRoundResults Results
		{
			get { return m_Results; }
			set
			{
				if (m_Results != value)
				{
					m_Results = value;
					OnPropertyChanged(ResultsPropertyName);
				}
			}
		}
		#endregion

		
		#region StartNumber
		public static readonly string StartNumberPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.StartNumber);

		private byte? m_StartNumber = null;

		public byte? StartNumber
		{
			get { return m_StartNumber; }
			set
			{
				if (m_StartNumber != value)
				{
					m_StartNumber = value;
					OnPropertyChanged(StartNumberPropertyName);
				}
			}
		}
		#endregion


		#region PrevNumber
		private static readonly string PrevNumberPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.PrevNumber);

		private byte? m_PrevNumber = null;

		public byte? PrevNumber
		{
			get { return m_PrevNumber; }
			set
			{
				if (m_PrevNumber != value)
				{
					m_PrevNumber = value;
					OnPropertyChanged(PrevNumberPropertyName);
				}
			}
		}
		#endregion

				
		#region VisibilityInMainTable
		private static readonly string VisibilityInMainTablePropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.VisibilityInMainTable);

		private Visibility m_VisibilityInMainTable = Visibility.Visible;

		public Visibility VisibilityInMainTable
		{
			get { return m_VisibilityInMainTable; }
			set
			{
				if (m_VisibilityInMainTable != value)
				{
					m_VisibilityInMainTable = value;
					OnPropertyChanged(VisibilityInMainTablePropertyName);
				}
			}
		}
		#endregion


		#region Вместо конвертеров
		#region BackgroundForShow
		private static readonly string BackgroundForShowPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.BackgroundForShow);

		private Brush m_BackgroundForShow = Brushes.Transparent;

		public Brush BackgroundForShow
		{
			get { return m_BackgroundForShow; }
			set
			{
				if (m_BackgroundForShow != value)
				{
					m_BackgroundForShow = value;
					OnPropertyChanged(BackgroundForShowPropertyName);
				}
			}
		}
		#endregion


		#region ForegroundForShow
		private static readonly string ForegroundForShowPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.ForegroundForShow);

		private Brush m_ForegroundForShow = Brushes.Black;

		public Brush ForegroundForShow
		{
			get { return m_ForegroundForShow; }
			set
			{
				if (m_ForegroundForShow != value)
				{
					m_ForegroundForShow = value;
					OnPropertyChanged(ForegroundForShowPropertyName);
				}
			}
		}
		#endregion


		#region FontWeightForShow
		private static readonly string FontWeightForShowPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.FontWeightForShow);

		private FontWeight m_FontWeightForShow = FontWeights.Normal;

		public FontWeight FontWeightForShow
		{
			get { return m_FontWeightForShow; }
			set
			{
				if (m_FontWeightForShow != value)
				{
					m_FontWeightForShow = value;
					OnPropertyChanged(FontWeightForShowPropertyName);
				}
			}
		}
		#endregion


		#region FontStyleForShow
		private static readonly string FontStyleForShowPropertyName = GlobalDefines.GetPropertyName<CMemberAndResults>(m => m.FontStyleForShow);

		private FontStyle m_FontStyleForShow = FontStyles.Normal;

		public FontStyle FontStyleForShow
		{
			get { return m_FontStyleForShow; }
			set
			{
				if (m_FontStyleForShow != value)
				{
					m_FontStyleForShow = value;
					OnPropertyChanged(FontStyleForShowPropertyName);
				}
			}
		}
		#endregion
		#endregion


		/// <summary>
		/// Вспомогательное поле
		/// </summary>
		public long id_part = 0;


		/// <summary>
		/// Для вывода в бегущую строку
		/// </summary>
		/// <returns></returns>
		public string StringForTicker()
		{
			string result = "";

			if (!StartNumber.HasValue || MemberInfo == null || Results == null)
				return result;

			result = StartNumber.Value.ToString() + ". " + MemberInfo.SurnameAndName + " " + Results.StringForTicker();

			return result.Trim();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="IsWinner">
		/// Необходимо только для 1/8, 1/4, 1/2 и финала.
		/// True - текущий объект описывает победителя
		/// </param>
		public void RefreshColors(bool IsWinner = false)
		{
			CFontStyleSettings RowFontStyle = DBManagerApp.MainWnd.PlainResultsFontStyle;

			switch ((enRounds)DBManagerApp.MainWnd.CurrentRounds.SelectedKey)
			{
				case enRounds.Qualif:
				case enRounds.Qualif2:
					if (Place > 0 && Place <= DBManagerApp.MainWnd.MembersFromQualif)
					{
						RowFontStyle = DBManagerApp.MainWnd.NextRoundMembersCountFontStyle;
					}
					break;

				case enRounds.OneEighthFinal:
				case enRounds.QuaterFinal:
				case enRounds.SemiFinal:
				case enRounds.Final:
					if (IsWinner)
					{
						RowFontStyle = DBManagerApp.MainWnd.NextRoundMembersCountFontStyle;
					}
					break;

				default:
					break;
			}

			BackgroundForShow = new SolidColorBrush(RowFontStyle.BackgroundColor);
			ForegroundForShow = new SolidColorBrush(RowFontStyle.ForeColor);
			FontWeightForShow = RowFontStyle.FontWeight;
			FontStyleForShow = RowFontStyle.FontStyle;

			if (Results != null)
				Results.ResultsForShow.RefreshFields(this, Results, RowFontStyle);
		}
	}
}
