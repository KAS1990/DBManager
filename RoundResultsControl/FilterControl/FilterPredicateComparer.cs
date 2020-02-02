using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DBManager.RoundResultsControl.FilterControl
{
    /// <summary>
    /// Поле, по которому осуществляется сравнение
    /// </summary>
    public enum enCompareProperty
    {
        None = 0,
        FilterValue = 1,
        Name = 2,
    }


    public class FilterPredicateComparer : IComparer<FilterPredicate>
    {
        public ListSortDirection SortDir = ListSortDirection.Ascending;
        public enCompareProperty CompareProperty = enCompareProperty.None;
        public object NullFilterValue = null;

        public int Compare(FilterPredicate x, FilterPredicate y)
        {
            IComparable lhs = null, rhs = null;

            switch (CompareProperty)
            {
                case enCompareProperty.FilterValue:
                    if (x.FilterValue == null && y.FilterValue == null)
                        return 0;

                    // null или "пусто" всегда больше любых других элементов
                    if ((x.FilterValue == null || x.FilterValue.Equals(NullFilterValue)) &&
                        (y.FilterValue != null && !y.FilterValue.Equals(NullFilterValue)))
                    {
                        return SortDir == ListSortDirection.Ascending ? 1 : -1;
                    }
                    if ((y.FilterValue == null || y.FilterValue.Equals(NullFilterValue)) &&
                        (x.FilterValue != null && !x.FilterValue.Equals(NullFilterValue)))
                    {
                        return SortDir == ListSortDirection.Ascending ? -1 : 1;
                    }

                    lhs = x.FilterValue as IComparable;
                    rhs = y.FilterValue as IComparable;
                    break;

                case enCompareProperty.Name:
                    if (x.Name == null && y.Name == null)
                        return 0;

                    // null всегда больше любых других элементов
                    if (string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(y.Name))
                        return SortDir == ListSortDirection.Ascending ? 1 : -1;
                    if (!string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(y.Name))
                        return SortDir == ListSortDirection.Ascending ? -1 : 1;

                    lhs = x.Name;
                    rhs = y.Name;
                    break;
            }

            return SortDir == ListSortDirection.Ascending ? lhs.CompareTo(rhs) : rhs.CompareTo(lhs);
        }
    }
}
