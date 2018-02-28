﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.RoundMembers.Converters;
using DBManager.Global;
using System.Windows;
using System.Windows.Media;
using DBManager.Scanning.XMLDataClasses;
using System.ComponentModel;
using DBManager.SettingsWriter;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class COneRoundResultsForShow : INotifyPropertyChanged
	{
		#region Route1
		private static readonly string Route1PropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Route1);

		private OneCellInterior m_Route1 = new OneCellInterior(enCellType.Route1);

		public OneCellInterior Route1
		{
			get { return m_Route1; }
			set
			{
				if (m_Route1 != value)
				{
					m_Route1 = value;
					OnPropertyChanged(Route1PropertyName);
				}
			}
		}
		#endregion


		#region Route2
		private static readonly string Route2PropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Route2);

		private OneCellInterior m_Route2 = new OneCellInterior(enCellType.Route2);

		public OneCellInterior Route2
		{
			get { return m_Route2; }
			set
			{
				if (m_Route2 != value)
				{
					m_Route2 = value;
					OnPropertyChanged(Route2PropertyName);
				}
			}
		}
		#endregion


		#region Sum
		private static readonly string SumPropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Sum);

		private OneCellInterior m_Sum = new OneCellInterior(enCellType.Sum);

		public OneCellInterior Sum
		{
			get { return m_Sum; }
			set
			{
				if (m_Sum != value)
				{
					m_Sum = value;
					OnPropertyChanged(SumPropertyName);
				}
			}
		}
		#endregion


		#region StartNumber
		private static readonly string StartNumberPropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.StartNumber);

		private OneCellInterior m_StartNumber = new OneCellInterior(enCellType.StartNumber);

		public OneCellInterior StartNumber
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


		#region SurnameAndName
		private static readonly string SurnameAndNamePropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.SurnameAndName);

		private OneCellInterior m_SurnameAndName = new OneCellInterior(enCellType.SurnameAndName);

		public OneCellInterior SurnameAndName
		{
			get { return m_SurnameAndName; }
			set
			{
				if (m_SurnameAndName != value)
				{
					m_SurnameAndName = value;
					OnPropertyChanged(SurnameAndNamePropertyName);
				}
			}
		}
		#endregion


		public void RefreshFields(CDBAdditionalClassBase Parent, COneRoundResults RouteResults, CFontStyleSettings RowFontStyle)
		{
			Route1.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle);
			Route2.RefreshFields(Parent, RouteResults, RouteResults.Route2, RowFontStyle);
			Sum.RefreshFields(Parent, RouteResults, RouteResults.Sum, RowFontStyle);

			StartNumber.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle);
			SurnameAndName.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle);
		}


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		public void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}
