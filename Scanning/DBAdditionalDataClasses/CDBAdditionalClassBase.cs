using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBManager.Global;
using DBManager.RoundMembers.Converters;
using System.Globalization;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CDBAdditionalClassBase : INotifyPropertyChanged
	{	
		#region Place
		public static readonly string PlacePropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.Place);

		private int? m_Place = null;

		public int? Place
		{
			get { return m_Place; }
			set
			{
				if (m_Place != value)
				{
					m_Place = value;
					PlaceForShow = GlobalDefines.PlaceToString(m_Place);
					IsWinnerForShow = m_Place < 4 && m_Place > 0;
					OnPropertyChanged(PlacePropertyName);
				}
			}
		}
		#endregion


		#region PlaceInFilter
		private static readonly string PlaceInFilterPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceInFilter);

		private int? m_PlaceInFilter = null;

		public int? PlaceInFilter
		{
			get { return m_PlaceInFilter; }
			set
			{
				if (m_PlaceInFilter != value)
				{
					m_PlaceInFilter = value;
					PlaceInFilterForShow = GlobalDefines.PlaceToString(m_Place);
					OnPropertyChanged(PlaceInFilterPropertyName);
				}
			}
		}
		#endregion


		#region Вместо конвертеров
		#region PlaceForShow
		private static readonly string PlaceForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceForShow);

		private string m_PlaceForShow = "";

		public string PlaceForShow
		{
			get { return m_PlaceForShow; }
			set
			{
				if (m_PlaceForShow != value)
				{
					m_PlaceForShow = value;
					OnPropertyChanged(PlaceForShowPropertyName);
				}
			}
		}
		#endregion


		#region PlaceInFilterForShow
		private static readonly string PlaceInFilterForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceInFilterForShow);

		private string m_PlaceInFilterForShow = "";

		public string PlaceInFilterForShow
		{
			get { return m_PlaceInFilterForShow; }
			set
			{
				if (m_PlaceInFilterForShow != value)
				{
					m_PlaceInFilterForShow = value;
					OnPropertyChanged(PlaceInFilterForShowPropertyName);
				}
			}
		}
		#endregion


		#region IsWinnerForShow
		private static readonly string IsWinnerForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.IsWinnerForShow);

		private bool m_IsWinnerForShow = false;

		public bool IsWinnerForShow
		{
			get { return m_IsWinnerForShow; }
			set
			{
				if (m_IsWinnerForShow != value)
				{
					m_IsWinnerForShow = value;
					OnPropertyChanged(IsWinnerForShowPropertyName);
				}
			}
		}
		#endregion
		#endregion


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		protected void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}
