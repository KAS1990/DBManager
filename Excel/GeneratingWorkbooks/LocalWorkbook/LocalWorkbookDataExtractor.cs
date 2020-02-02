using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class LocalWorkbookDataExtractor : IDataExtractor
    {
        public ICompDesc CompDesc { get; private set; }
        public List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>> GroupsMembers { get; private set; }

        public bool Extract(ICompDesc compDesc, IEnumerable<IGroupItem> compGroups, out string message)
        {
            CompDescLocalWorkbook compDescLocal = compDesc as CompDescLocalWorkbook;

            message = null;

            GroupsMembers = new List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>>();

            try
            {
                using (var excelApp = new DisposableWrapper<ExcelApplicationEx>(GlobalDefines.StartExcel(),
                                                                                app =>
                                                                                {
                                                                                    if (app != null)
                                                                                    {
                                                                                        app.App.DisplayAlerts = true;

                                                                                        if (app.NewAppCreated)
                                                                                            app.App.Quit();

                                                                                        app = null;
                                                                                    }
                                                                                }))
                {
                    bool WbkOpened;
                    var wbk = WorkbookGenerator.OpenWbk(excelApp, compDescLocal.SourceWorkbookName, out WbkOpened);

                    if (wbk == null)
                    {
                        message = string.Format(Properties.Resources.resfmtSourceWorkbookCouldNotBeOpened,
                                                compDescLocal.SourceWorkbookName);
                        return false;
                    }

                    foreach (var @group in compGroups.Cast<GroupItemLocalWorkbook>())
                    {
                        MSExcel.Worksheet wsh = wbk.Worksheets[@group.SheetName];
                        MSExcel.Range rng = wsh.Range[$"{@group.TLCell}:{@group.BRCell}"];

                        List<CFullMemberInfo> members = new List<CFullMemberInfo>();
                        for (int row = 0; row < rng.Rows.Count; row++)
                        {
                            string[] NameAndSurname;
                            GlobalDefines.CorrectSurnameAndName(rng[row + 1, @group.PersonalDataColumnIndex].Value, out NameAndSurname);

                            enGrade grade;
                            GlobalDefines.ParseGrade(rng[row + 1, @group.GradeColumnIndex].Value?.ToString(), out grade);

                            short? yearOfBirth = rng[row + 1, @group.YoBColumnIndex].Value == null
                                                    ? null
                                                    : (short?)Convert.ToUInt16(rng[row + 1, @group.YoBColumnIndex].Value);

                            members.Add(new CFullMemberInfo()
                            {
                                Surname = NameAndSurname[0],
                                Name = NameAndSurname[1],
                                YearOfBirth = yearOfBirth,
                                SecondCol = rng[row + 1, @group.TeamColumnIndex].Value,
                                InitGrade = grade == enGrade.None ? null : (byte?)grade
                            });
                        }

                        GroupsMembers.Add(new KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>(@group, members));
                    }
                }

                CompDesc = compDesc;
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
