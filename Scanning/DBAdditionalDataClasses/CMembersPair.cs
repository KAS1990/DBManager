using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using System.ComponentModel;
using System.Windows.Media;
using DBManager.RoundMembers.Converters;
using System.Globalization;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CMembersPair : CDBAdditionalClassBase
	{
		#region PairNumber
		private static readonly string PairNumberPropertyName = GlobalDefines.GetPropertyName<CMembersPair>(m => m.PairNumber);

		private int m_PairNumber = 0;
		/// <summary>
		/// Номер пары
		/// </summary>
		public int PairNumber
		{
			get { return m_PairNumber; }
			set
			{
				if (m_PairNumber != value)
				{
					m_PairNumber = value;
					OnPropertyChanged(PairNumberPropertyName);
				}
			}
		}
		#endregion
		
		
		#region First
		private static readonly string FirstPropertyName = GlobalDefines.GetPropertyName<CMembersPair>(m => m.First);

		private CMemberAndResults m_First = null;
		/// <summary>
		/// Первый участник из пары
		/// </summary>
		public CMemberAndResults First
		{
			get { return m_First; }
			set
			{
				if (m_First != value)
				{
					if (m_First != null)
						m_First.PropertyChanged -= Member_PropertyChanged;
					m_First = value;
					if (m_First != null)
					{
						PairNumber = StartNumber2PairNumber(m_First.StartNumber);
						m_First.PropertyChanged -= Member_PropertyChanged;
					}
					else
						PairNumber = 0;

					OnPropertyChanged(FirstPropertyName);
				}
			}
		}
		#endregion


		#region Second
		private static readonly string SecondPropertyName = GlobalDefines.GetPropertyName<CMembersPair>(m => m.First);

		private CMemberAndResults m_Second = null;
		/// <summary>
		/// Второй участник из пары
		/// </summary>
		public CMemberAndResults Second
		{
			get { return m_Second; }
			set
			{
				if (m_Second != value)
				{
					if (m_Second != null)
						m_Second.PropertyChanged -= Member_PropertyChanged;
					m_Second = value;
					if (m_Second != null)
					{
						PairNumber = StartNumber2PairNumber(m_Second.StartNumber);
						m_Second.PropertyChanged += Member_PropertyChanged;
					}
					else
						PairNumber = 0;

					OnPropertyChanged(SecondPropertyName);
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
		#endregion


		void Member_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == CMemberAndResults.StartNumberPropertyName)
			{	// Сменился стартовый номер => меняем номер пары
				PairNumber = StartNumber2PairNumber((sender as CMemberAndResults).StartNumber);
			}
		}


		int StartNumber2PairNumber(byte? StartNumber)
		{
			return StartNumber.HasValue ? (int)Math.Ceiling((float)StartNumber.Value / 2.0) : 0;
		}


		public void RefreshColors()
		{
			BackgroundForShow = new SolidColorBrush(DBManagerApp.MainWnd.PlainResultsFontStyle.BackgroundColor);
			ForegroundForShow = new SolidColorBrush(DBManagerApp.MainWnd.PlainResultsFontStyle.ForeColor);

			bool? WinnerIsFirst = null;
					
			if (First.Results.Sum != null && Second.Results.Sum != null && First.Results.Sum.Time != null && Second.Results.Sum.Time != null)
				WinnerIsFirst = First.Results.Sum.Time < Second.Results.Sum.Time;
						
			if (First != null)
				First.RefreshColors(WinnerIsFirst ?? false);

			if (Second != null)
				Second.RefreshColors(!WinnerIsFirst ?? false);
		}
	}
}
