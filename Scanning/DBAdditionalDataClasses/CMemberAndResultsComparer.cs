using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using DBManager.Global;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CMemberAndResultsComparer : IComparer<CMemberAndResults>, IComparer
	{
		/// <summary>
		/// Поле, по которому осуществляется сравнение
		/// </summary>
		public enum enCompareProperty
		{
			None = 0,
			Sum = 1,
			StartNumber = 2,
			Place = 3,
			PairNumber = 4
		}


		public ListSortDirection SortDir = ListSortDirection.Ascending;
		public enCompareProperty CompareProperty = enCompareProperty.None;

		public int Compare(CMemberAndResults x, CMemberAndResults y)
		{
			if (CompareProperty == enCompareProperty.None)
				throw new ArgumentException("CMemberAndResultsComparer.Compare: Value enCompareField.None is wrong for field CompareProperty", "CompareProperty");

			IComparable lhs = null, rhs = null; 
			
			switch (CompareProperty)
			{
				case enCompareProperty.Sum:
					lhs = x.Results.Sum.Time.GetValueOrDefault(TimeSpan.MaxValue);
					rhs = y.Results.Sum.Time.GetValueOrDefault(TimeSpan.MaxValue);
					break;

				case enCompareProperty.StartNumber:
					lhs = x.StartNumber.GetValueOrDefault(byte.MaxValue);
					rhs = y.StartNumber.GetValueOrDefault(byte.MaxValue);
					break;

				case enCompareProperty.Place:
					lhs = x.Place.GetValueOrDefault(int.MaxValue);
					rhs = y.Place.GetValueOrDefault(int.MaxValue);
					break;
			}

			return SortDir == ListSortDirection.Ascending ? lhs.CompareTo(rhs) : rhs.CompareTo(lhs);
		}


		public int Compare(object x, object y)
		{
			if (GlobalDefines.ObjectBaseEquals(x, y) == enObjectBaseEqualsResult.True 
				|| !((x is CMemberAndResults) && (y is CMemberAndResults)))
				return 0;

			return Compare(x as CMemberAndResults, y as CMemberAndResults);
		}
	}
}
