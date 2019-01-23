using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Interfaces
{
    public interface IDataExtractor
    {
        ICompDesc CompDesc { get; }
        List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>> GroupsMembers { get; }
    }
}
