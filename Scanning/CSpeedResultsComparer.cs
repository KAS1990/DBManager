using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace DBManager.Scanning
{
	public class CSpeedResultsComparer : IComparer<results_speed>
	{
		/// <summary>
		/// Поле, по которому осуществляется сравнение
		/// </summary>
		[Flags]
		public enum enCompareProperty
		{
			None = 0,
			Sum = 1,
			Place = 2,
			Number = 4
		}


		public ListSortDirection SortDir = ListSortDirection.Ascending;
		public enCompareProperty CompareProperty = enCompareProperty.None;

		public int Compare(results_speed x, results_speed y)
		{
			if (CompareProperty == enCompareProperty.None)
				throw new ArgumentException("CSpeedResultsComparer.Compare: Value enCompareField.None is wrong for field CompareProperty", "CompareProperty");

			IComparable lhs = null, rhs = null; 
			
			switch (CompareProperty)
			{
				case enCompareProperty.Place:
					lhs = x.place.GetValueOrDefault(byte.MaxValue);
					rhs = y.place.GetValueOrDefault(byte.MaxValue);
					break;

				case enCompareProperty.Sum:
					lhs = x.sum.GetValueOrDefault(TimeSpan.MaxValue);
					rhs = y.sum.GetValueOrDefault(TimeSpan.MaxValue);
					break;

				case enCompareProperty.Number:
					lhs = x.number.GetValueOrDefault(byte.MaxValue);
					rhs = y.number.GetValueOrDefault(byte.MaxValue);
					break;
			}

			return SortDir == ListSortDirection.Ascending ? lhs.CompareTo(rhs) : rhs.CompareTo(lhs);
		}
	}
}
