using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBManager.Global;
using System.Diagnostics;

namespace DBManager.RoundResultsControl.FilterControl
{
	[DebuggerDisplay("FilterValue = {FilterValue}, Name = {Name}, IsSelected = {IsSelected}")]
	public class FilterPredicate : INotifyPropertyChanged
	{
		#region IsSelected
		private static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<FilterPredicate>(m => m.IsSelected);

		private bool m_IsSelected = false;
		/// <summary>
		/// Выделена ли категория
		/// </summary>
		public bool IsSelected
		{
			get { return m_IsSelected; }
			set
			{
				if (m_IsSelected != value)
				{
					m_IsSelected = value;
					OnPropertyChanged(IsSelectedPropertyName);
				}
			}
		}
		#endregion
		
		
		#region FilterValue
		private static readonly string FilterValuePropertyName = GlobalDefines.GetPropertyName<FilterPredicate>(m => m.FilterValue);

		private object m_FilterValue = null;
		/// <summary>
		/// То, по чему фильтруем
		/// </summary>
		public object FilterValue
		{
			get { return m_FilterValue; }
			set
			{
				if (m_FilterValue != value)
				{
					m_FilterValue = value;
					OnPropertyChanged(FilterValuePropertyName);
				}
			}
		}
		#endregion
		

		#region Name
		private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<FilterPredicate>(m => m.Name);

		private string m_Name = null;
		/// <summary>
		/// Текстовое название категории
		/// </summary>
		public string Name
		{
			get { return m_Name; }
			set
			{
				if (m_Name != value)
				{
					m_Name = value;
					OnPropertyChanged(NamePropertyName);
				}
			}
		}
		#endregion


		public FilterPredicate()
		{
		}


		public FilterPredicate(FilterPredicate rhs)
		{
			IsSelected = rhs.IsSelected;
			FilterValue = rhs.FilterValue;
			Name = rhs.Name;
		}


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		void OnPropertyChanged(string info)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}
}