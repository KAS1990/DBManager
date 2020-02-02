using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DBManager.ReportGenerators
{
    public class CPersonalResultsComparer : IComparer<CPersonalItem>
    {
        public ListSortDirection SortDir = ListSortDirection.Ascending;
        public enPersRepWinnerDetection CompareProperty = enPersRepWinnerDetection.BestPlace;
        public enPriorityCompetitionKind CompareProperty1 = enPriorityCompetitionKind.None;

        public int Compare(CPersonalItem x, CPersonalItem y)
        {
            if (SortDir == ListSortDirection.Ascending)
            {
                if (x == null)
                    return y == null ? 0 : -1;
                else if (y == null)
                    return 1;
            }
            else
            {
                if (x == null)
                    return y == null ? 0 : 1;
                else if (y == null)
                    return -1;
            }

            IComparable lhs = null, rhs = null;

            switch (CompareProperty)
            {
                case enPersRepWinnerDetection.BestPlace:
                    lhs = Math.Min(x.m_SpeedPlace, x.m_LeadPlace);
                    rhs = Math.Min(y.m_SpeedPlace, y.m_LeadPlace);
                    break;

                case enPersRepWinnerDetection.LeadPriority:
                    lhs = x.m_LeadPlace;
                    rhs = y.m_LeadPlace;
                    break;

                case enPersRepWinnerDetection.LessDifference:
                    lhs = Math.Abs(x.m_SpeedPlace - x.m_LeadPlace);
                    rhs = Math.Abs(y.m_SpeedPlace - y.m_LeadPlace);
                    break;

                case enPersRepWinnerDetection.SpeedPriority:
                    lhs = x.m_SpeedPlace;
                    rhs = y.m_SpeedPlace;
                    break;
            }

            if (lhs.CompareTo(rhs) == 0)
            {
                switch (CompareProperty1)
                {
                    case enPriorityCompetitionKind.Speed:
                        lhs = x.m_SpeedPlace;
                        rhs = y.m_SpeedPlace;
                        break;

                    case enPriorityCompetitionKind.Lead:
                        lhs = x.m_LeadPlace;
                        rhs = y.m_LeadPlace;
                        break;
                }
            }

            return SortDir == ListSortDirection.Ascending ? lhs.CompareTo(rhs) : rhs.CompareTo(lhs);
        }
    }
}
