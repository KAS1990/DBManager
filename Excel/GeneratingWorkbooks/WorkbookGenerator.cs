using DBManager.Excel.GeneratingWorkbooks.Helpers;
using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class WorkbookGenerator
    {
        class RunWbkActionResult<TResult> where TResult : IComparable
        {
            public TResult Result { get; }
            public string ErrorMessage { get; } = null;

            public RunWbkActionResult()
            {
            }

            public RunWbkActionResult(TResult result)
            {
                Result = result;
            }

            public RunWbkActionResult(TResult result, string errorMessage)
            {
                Result = result;
                ErrorMessage = errorMessage;
            }
        }

        private readonly IDataExtractor m_DataExtractor = null;

        public WorkbookGenerator(IDataExtractor dataExtractor)
        {
            m_DataExtractor = dataExtractor;
        }

        private MSExcel.Workbook OpenWbk(ExcelApplicationEx excelApp, string wbkFullPath)
        {
            bool WbkOpened;
            return OpenWbk(excelApp, wbkFullPath, out WbkOpened);
        }
        
        private MSExcel.Workbook OpenWbk(ExcelApplicationEx excelApp, string wbkFullPath, out bool WbkOpened)
        {
            MSExcel.Workbook result = null;

            WbkOpened = false;

            // Открываем книгу
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                if (excelApp.NewAppCreated)
                {   // Без этого книга не открывается
                    excelApp.App.Visible = true;
                    excelApp.App.WindowState = MSExcel.XlWindowState.xlMinimized;
                }
                foreach (MSExcel.Workbook book in excelApp.App.Workbooks)
                {
                    if (book.FullName == wbkFullPath)
                    {   // Книга уже открыта => используем её
                        result = book;
                        break;
                    }
                }
                if (result == null)
                {
                    result = excelApp.App.Workbooks.Open(wbkFullPath);
                    WbkOpened = true;
                }
                if (excelApp.NewAppCreated)
                {
                    excelApp.App.Visible = false;
                    excelApp.App.WindowState = MSExcel.XlWindowState.xlNormal;
                }
            }

            return result;
        }

        private bool RunWbkOperation<TResult>(ExcelApplicationEx excelApp,
                    string wbkFullPath,
                    bool closeWbkAfterAction,
                    TResult trueValue,
                    Func<MSExcel.Workbook, RunWbkActionResult<TResult>> action,
                    out string message)
            where TResult : IComparable
        {
            message = null;
            try
            {
                using (var wrapper = new DisposableWrapper<MSExcel.Application>(excelApp.App,
                                            app =>
                                            {
                                                if (app != null)
                                                {
                                                    app.DisplayAlerts = true;
                                                }
                                            }))
                {
                    if (excelApp != null)
                    {
                        excelApp.App.DisplayAlerts = false; // Отключаем различные сообщения

                        var wbk = OpenWbk(excelApp, wbkFullPath);
                        if (wbk != null)
                        {
                            var res = action(wbk);
                            if (res.Result.CompareTo(trueValue) != 0)
                            {
                                message = res.ErrorMessage;
                                return false;
                            }

                            wbk.Save();
                            if (closeWbkAfterAction)
                                wbk.Close(); // Закрываем книгу, в следующий раз она молчаливо себя почистит

                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = $"exception in RunWbkOperation: {ex.Message}";
            }

            return false;
        }

        private bool CopyFilesToNewFolder(string destFolderFullPath,
            string sourceFolderFullPath, 
            string[] sourceFileRelativePaths,
            out string message)
        {
            message = null;

            try
            {
                if (!Directory.Exists(destFolderFullPath))
                    Directory.CreateDirectory(destFolderFullPath);

                // Очищаем папку с соревами
                (new DirectoryInfo(destFolderFullPath)).ClearDirectory();

                foreach (var relativePath in sourceFileRelativePaths)
                {
                    string fileName = Path.GetFileName(relativePath);
                    string[] pathDirs = Path
                                            .GetDirectoryName(relativePath)
                                            .Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var pathDir in pathDirs)
                    {
                        string destDirPath = Path.Combine(destFolderFullPath, pathDir);
                        if (!Directory.Exists(destDirPath))
                            Directory.CreateDirectory(destDirPath);
                    }
                    File.Copy(Path.Combine(sourceFolderFullPath, relativePath), Path.Combine(destFolderFullPath, relativePath), true);
                }
            }
            catch (Exception ex)
            {
                message = $"exception in CopyFilesToNewFolder: {ex.Message}";
                return false;
            }

            return true;
        }

        private bool PrepareTemplateWorkbook(ExcelApplicationEx excelApp, string wbkTemplateFullPath, out string message)
        {
            return RunWbkOperation<bool>(excelApp, wbkTemplateFullPath, true, true,
                wbk =>
                {

                    try
                    {
                        var helper = new SetupWorksheetHelper(wbk);
                        string error;
                        var result = helper.PrepareSheetToClearWorkbook(out error);
                        return new RunWbkActionResult<bool>(result, error);
                    }
                    catch (Exception ex)
                    {
                        return new RunWbkActionResult<bool>(false, $"exception in PrepareTemplateWorkbook: {ex.Message}");
                    }
                },
                out message);
        }

        private bool CreateGroupWbkAndWriteGroupDesc(ExcelApplicationEx excelApp,
            string wbkTemplateFullPath,
            string wbkFullPath,
            ICompDesc compDesc,
            IGroupItem groupDesc,
            out string message)
        {
            message = null;

            if (!File.Exists(wbkTemplateFullPath))
            {
                message = $"error in CreateGroupWbkAndWriteGroupDesc: {wbkTemplateFullPath} is not existed";
                return false;
            }

            File.Copy(wbkTemplateFullPath, wbkFullPath, true);

            return RunWbkOperation<bool>(excelApp, wbkFullPath, false, true,
                wbk =>
                {
                    try
                    {
                        #region WorkbookDataFileWrapper

                        var dataFileWrapper = new WorkbookDataFileWrapper(compDesc.DestCompFolder);

                        #endregion

                        #region SetupWorksheetHelper

                        var wshHelper = new SetupWorksheetHelper(dataFileWrapper, wbk);

                        wshHelper.CompName = compDesc.Name;

                        wshHelper.MainJudge = compDesc.MainJudge;
                        wshHelper.MainSecretary = compDesc.MainSecretary;
                        wshHelper.Row6 = compDesc.ShowRow6 ? compDesc.Row6 : null;

                        wshHelper.StartCompDate = groupDesc.StartDate;
                        wshHelper.EndCompDate = groupDesc.EndDate;

                        wshHelper.StartGroupYear = groupDesc.StartYear;
                        wshHelper.EndGroupYear = groupDesc.EndYear;

                        #endregion

                        string errorMessage;
                        if (!dataFileWrapper.Save(out errorMessage))
                            return new RunWbkActionResult<bool>(false, errorMessage);

                        #region Заполняем FLAGS

                        var flagsHelper = new WorkbookFlagsWrapper()
                        {
                            Value = wshHelper.FLAGS
                        };

                        flagsHelper.SetFlagsByCompDates(groupDesc.StartDate, groupDesc.EndDate);
                        flagsHelper.SecondColNameType = compDesc.SecondColNameType;
                        flagsHelper.ShowRow6 = compDesc.ShowRow6;

                        lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                        {
                            var availableGroupName = DBManagerApp
                                                        .m_AppSettings
                                                        .m_Settings
                                                        .AvailableGroupNames
                                                        .First(arg => string.Compare(arg.GroupName, groupDesc.Name, true) == 0);
                            flagsHelper.Sex = availableGroupName.Sex;
                            flagsHelper.GroupYearsAreNeeded = availableGroupName.YearsRangeCanBeSet;
                            flagsHelper.GroupIndexValue = availableGroupName.ValueInWbkFlags;
                        }
                        flagsHelper.WorkbookIsOpenedFirstTime = false;

                        wshHelper.FLAGS = flagsHelper.Value;

                        #endregion

                        wshHelper.SaveAllFlags();
                        wbk.Save();
                        wshHelper.SendRequestToFillWbkBasedOnSetupSheet();

                        return new RunWbkActionResult<bool>(true);
                    }
                    catch (Exception ex)
                    {
                        return new RunWbkActionResult<bool>(false, $"exception in CreateGroupWbkAndWriteGroupDesc: {ex.Message}");
                    }
                },
                out message);
        }

        private bool ExportDataWriteMembersToWbk(ExcelApplicationEx excelApp,
            string wbkFullPath,
            IEnumerable<CFullMemberInfo> data,
            out string message)
        {
            return RunWbkOperation<bool>(excelApp, wbkFullPath, true, true,
                wbk =>
                {
                    try
                    {
                        var list = data.OrderBy(arg => arg.Surname).ToList();

                        var helper = new StartListWorksheetHelper(wbk);
                        helper.PrepareTable(list.Count);

                        for (int i = 0; i < list.Count; i++)
                        {
                            helper.SetMember(list[i], i);
                        }

                        return new RunWbkActionResult<bool>(true);
                    }
                    catch (Exception ex)
                    {
                        return new RunWbkActionResult<bool>(false, $"exception in ExportDataWriteMembersToWbk: {ex.Message}");
                    }
                },
                out message);
        }

        public bool Generate(out string message, Action<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>> beforeExportingGroup)
        {
            message = null;

            if ((m_DataExtractor == null) || (m_DataExtractor.CompDesc == null) || ((m_DataExtractor.GroupsMembers?.Count ?? 0) == 0))
            {
                message = "error in Generate: extractor is not set, or CompDesc is not set, or there are not group members";
                return false;
            }

            // Создаём папку с соревнованиями
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                if (!CopyFilesToNewFolder(m_DataExtractor.CompDesc.DestCompFolder,
                        DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateFolder,
                        DBManagerApp.m_AppSettings.m_Settings.FilesToCopyFromWorkbookTemplateFolder,
                        out message))
                {
                    return false;
                }
            }

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
                    string wbkTemplateFullPath = null;
                    lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                    {
                        wbkTemplateFullPath = Path.Combine(m_DataExtractor.CompDesc.DestCompFolder,
                                                            DBManagerApp.m_AppSettings.m_Settings.WorkbookTemplateName);
                    }
                    if (!PrepareTemplateWorkbook(excelApp.Object, wbkTemplateFullPath, out message))
                        return false;

                    foreach (var group in m_DataExtractor.GroupsMembers)
                    {
                        beforeExportingGroup(group);

                        string wbkFullPath = Path.Combine(m_DataExtractor.CompDesc.DestCompFolder, group.Key.WorkbookName);

                        if (!CreateGroupWbkAndWriteGroupDesc(excelApp.Object,
                                                            wbkTemplateFullPath,
                                                            wbkFullPath,
                                                            m_DataExtractor.CompDesc,
                                                            group.Key,
                                                            out message))
                        {
                            return false;
                        }

                        if (!ExportDataWriteMembersToWbk(excelApp.Object, wbkFullPath, group.Value, out message))
                            return false;
                    }

                    // Удаляем TemplateWorkbook
                    File.Delete(wbkTemplateFullPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                message = $"exception in Generate: {ex.Message}";
                return false;
            }
        }
    }
}
