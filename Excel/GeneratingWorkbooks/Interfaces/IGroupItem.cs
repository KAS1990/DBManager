using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Interfaces
{
    public interface IGroupItem
    {
        string Name { get; }
        enSex Sex { get; }
        int StartYear { get; }
        int? EndYear { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }
        string WorkbookName { get; }
    }
}
