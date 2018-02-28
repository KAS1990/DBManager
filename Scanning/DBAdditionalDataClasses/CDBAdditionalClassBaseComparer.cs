using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CDBAdditionalClassBaseComparer : IComparer<CDBAdditionalClassBase>
	{
		/// <summary>
		/// Поле, по которому осуществляется сравнение
		/// </summary>
		public enum enCompareProperty
		{
			None = 0,
			Place = 1,
			PlaceInFilter = 2,
		}

		public ListSortDirection SortDir = ListSortDirection.Ascending;
		public enCompareProperty CompareProperty = enCompareProperty.None;

		public int Compare(CDBAdditionalClassBase x, CDBAdditionalClassBase y)
		{
			IComparable lhs = null, rhs = null;

			switch (CompareProperty)
			{
				case enCompareProperty.Place:
					if (x.Place == null && y.Place == null)
						return 0;

					// null всегда больше любых других элементов
					if (x.Place == null && y.Place != null)
						return SortDir == ListSortDirection.Ascending ? 1 : -1;
					if (y.Place == null && x.Place != null)
					{
						return SortDir == ListSortDirection.Ascending ? -1 : 1;
					}

					lhs = x.Place as IComparable;
					rhs = y.Place as IComparable;
					break;

				case enCompareProperty.PlaceInFilter:
					if (x.PlaceInFilter == null && y.PlaceInFilter == null)
						return 0;

					// null всегда больше любых других элементов
					if (x.PlaceInFilter == null && y.PlaceInFilter != null)
						return SortDir == ListSortDirection.Ascending ? 1 : -1;
					if (y.PlaceInFilter == null && x.PlaceInFilter != null)
					{
						return SortDir == ListSortDirection.Ascending ? -1 : 1;
					}

					lhs = x.PlaceInFilter as IComparable;
					rhs = y.PlaceInFilter as IComparable;
					break;
			}

			return SortDir == ListSortDirection.Ascending ? lhs.CompareTo(rhs) : rhs.CompareTo(lhs);
		}
	}
}
