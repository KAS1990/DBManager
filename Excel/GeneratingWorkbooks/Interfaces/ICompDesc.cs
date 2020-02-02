using DBManager.Global;
using System;

namespace DBManager.Excel.GeneratingWorkbooks.Interfaces
{
    public interface ICompDesc
    {
        string Name { get; }
        DateTime StartDate { get; }
        DateTime? EndDate { get; }
        string MainJudge { get; }
        string MainSecretary { get; }
        bool ShowRow6 { get; }
        string Row6 { get; }
        enSecondColNameType SecondColNameType { get; }
        string DestCompFolder { get; }

        void CopyCompSpecificFields(ICompDesc src);
        string GetDefaultDestCompFolderName();
    }
}
