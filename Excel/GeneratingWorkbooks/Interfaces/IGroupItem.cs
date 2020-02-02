using DBManager.Global;
using System;

namespace DBManager.Excel.GeneratingWorkbooks.Interfaces
{
    public interface IGroupItem
    {
        ICompDesc CompDesc { get; }

        string Name { get; }
        enSex Sex { get; }
        int StartYear { get; }
        int? EndYear { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }
        string WorkbookName { get; }
    }
}
