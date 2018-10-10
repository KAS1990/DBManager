using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Global;
using System.ComponentModel;
using DBManager.DAL;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	/// <summary>
	/// Результаты участника в одном раунде
	/// </summary>
	public class COneRoundResults : CDBAdditionalClassBase, ICanRefreshFrom
	{
		public enRounds m_Round = enRounds.None;

		#region Route1
		private static readonly string Route1PropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.Route1);

		private CResult m_Route1 = null;

		public CResult Route1
		{
			get { return m_Route1; }
			set
			{
				if (m_Route1 != value)
				{
					if (m_Route1 != null)
					{
						m_Route1.StyleChanged -= Result_StyleChanged;
					}

					m_Route1 = value;

					if (m_Route1 != null)
					{
						m_Route1.StyleChanged += Result_StyleChanged;
					}

					OnPropertyChanged(Route1PropertyName);
				}
			}
		}
		#endregion
		
		#region Route2
		private static readonly string Route2PropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.Route2);

		private CResult m_Route2 = null;

		public CResult Route2
		{
			get { return m_Route2; }
			set
			{
				if (m_Route2 != value)
				{
					if (m_Route2 != null)
					{
						m_Route2.StyleChanged -= Result_StyleChanged;
					}

					m_Route2 = value;

					if (m_Route2 != null)
					{
						m_Route2.StyleChanged += Result_StyleChanged;
					}

					OnPropertyChanged(Route2PropertyName);
				}
			}
		}
		#endregion

		#region Sum
		private static readonly string SumPropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.Sum);

		private CResult m_Sum = null;

		public CResult Sum
		{
			get { return m_Sum; }
			set
			{
				if (m_Sum != value)
				{
					if (m_Sum != null)
					{
						m_Sum.StyleChanged -= Result_StyleChanged;
					}

					m_Sum = value;

					if (m_Sum != null)
					{
						m_Sum.StyleChanged += Result_StyleChanged;
					}

					OnPropertyChanged(SumPropertyName);
				}
			}
		}
		#endregion

		#region IsLastMember
		private static readonly string IsLastMemberPropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.IsLastMember);

		private bool m_IsLastMember = false;
		/// <summary>
		/// Является ли данные результат последним в раунде.
		/// Это свойство нужно для итогового протокола
		/// </summary>
		public bool IsLastMember
		{
			get { return m_IsLastMember; }
			set
			{
				if (m_IsLastMember != value)
				{
					m_IsLastMember = value;
					OnPropertyChanged(IsLastMemberPropertyName);
				}
			}
		}
		#endregion

		#region IsLooser
		private static readonly string IsLooserPropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.IsLooser);

		private bool m_IsLooser = false;
		/// <summary>
		/// Выбыл ли участник в раунде
		/// </summary>
		public bool IsLooser
		{
			get { return m_IsLooser; }
			set
			{
				if (m_IsLooser != value)
				{
					m_IsLooser = value;
					OnPropertyChanged(IsLooserPropertyName);
				}
			}
		}
		#endregion

		#region Вместо конвертеров
		#region ResultsForShow
		private static readonly string ResultsForShowPropertyName = GlobalDefines.GetPropertyName<COneRoundResults>(m => m.ResultsForShow);

		private COneRoundResultsForShow m_ResultsForShow = new COneRoundResultsForShow();

		public COneRoundResultsForShow ResultsForShow
		{
			get { return m_ResultsForShow; }
			set
			{
				if (m_ResultsForShow != value)
				{
					m_ResultsForShow = value;
					OnPropertyChanged(ResultsForShowPropertyName);
				}
			}
		}
		#endregion
		#endregion

		results_speed ResultInDB
		{
			get
			{
				return Route1?.ResultInDB ?? Route2?.ResultInDB ?? Sum?.ResultInDB;
			}
		}

		void Result_StyleChanged(object sender, StyleChangedEventArgs e)
		{
			OnStyleChanged(e);
		}

		/// <summary>
		/// Для вывода в бегущую строку
		/// </summary>
		/// <returns></returns>
		public string StringForTicker()
		{
			string result = "";

			if (Route1 != null && Route1.Time.HasValue && Route1.CondFormating.HasValue && Route1.CondFormating.Value == enCondFormating.JustRecievedResult)
				result += "(" + Properties.Resources.resRoute1.ToLower() + ") " + Route1.Time.Value.ToString(@"mm\:ss\,ff") + " ";

			if (Route2 != null && Route2.Time.HasValue && Route2.CondFormating.HasValue && Route2.CondFormating.Value == enCondFormating.JustRecievedResult)
				result += "(" + Properties.Resources.resRoute2.ToLower() + ") " + Route2.Time.Value.ToString(@"mm\:ss\,ff") + " ";

			if (Sum != null && Sum.Time.HasValue && Sum.CondFormating.HasValue && Sum.CondFormating.Value == enCondFormating.JustRecievedResult)
				result += "(" + Properties.Resources.resSum.ToLower() + ") " + Sum.Time.Value.ToString(@"mm\:ss\,ff") + " ";

			return result.Trim();
		}

		public override void RefreshFrom(ICanRefreshFrom rhs,
										bool SkipNullsForObjects,
										bool SkipNullsForNullables)
		{
			base.RefreshFrom(rhs, SkipNullsForObjects, SkipNullsForNullables);

			COneRoundResults rhsOneRoundResults = rhs as COneRoundResults;

			if (rhsOneRoundResults == null)
				return;

			if (Route1 == null)
				Route1 = rhsOneRoundResults.Route1;
			else if (rhsOneRoundResults.Route1 == null)
			{
				if (!SkipNullsForObjects)
					Route1 = null;
			}
			else
				Route1.RefreshFrom(rhsOneRoundResults.Route1, SkipNullsForObjects, SkipNullsForNullables);

			if (Route2 == null)
				Route2 = rhsOneRoundResults.Route2;
			else if (rhsOneRoundResults.Route2 == null)
			{
				if (!SkipNullsForObjects)
					Route2 = null;
			}
			else
				Route2.RefreshFrom(rhsOneRoundResults.Route2, SkipNullsForObjects, SkipNullsForNullables);

			if (Sum == null)
				Sum = rhsOneRoundResults.Sum;
			else if (rhsOneRoundResults.Sum == null)
			{
				if (!SkipNullsForObjects)
					Sum = null;
			}
			else
				Sum.RefreshFrom(rhsOneRoundResults.Sum, SkipNullsForObjects, SkipNullsForNullables);

			m_Round = rhsOneRoundResults.m_Round;
			IsLastMember = rhsOneRoundResults.IsLastMember;
			IsLooser = rhsOneRoundResults.IsLooser;
		}
	}
}
