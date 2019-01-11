using DBManager.Excel.GeneratingWorkbooks.Helpers;
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
    public class GeneratorBase
    {
        protected bool CopyFilesToNewFolder(string destFolderFullPath, string sourceFolderFullPath, string[] sourceFileRelativePaths)
        {
            try
            {
                if (!Directory.Exists(destFolderFullPath))
                    Directory.CreateDirectory(destFolderFullPath);

                foreach (var relativePath in sourceFileRelativePaths)
                {
                    string fileName = Path.GetFileName(relativePath);
                    string[] pathDirs = Path.GetDirectoryName(relativePath).Split(Path.DirectorySeparatorChar);

                    foreach (var pathDir in pathDirs)
                    {
                        string destDirPath = Path.Combine(destFolderFullPath, pathDir);
                        if (!Directory.Exists(destDirPath))
                            Directory.CreateDirectory(destDirPath);
                    }
                    File.Copy(Path.Combine(destFolderFullPath, relativePath), Path.Combine(sourceFolderFullPath, relativePath), true);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        protected bool PrepareTemplateWorkbook(string wbkFullPath)
        {
            try
            {
                bool NewAppCreated;
                using (var excelApp = new DisposableWrapper<MSExcel.Application>(GlobalDefines.StartExcel(out NewAppCreated),
                                                app =>
                                                {
                                                    if (app != null)
                                                    {
                                                        app.DisplayAlerts = true;

                                                        if (NewAppCreated)
                                                            app.Quit();

                                                        app = null;
                                                    }
                                                }))
                {
                    MSExcel.Application app = (MSExcel.Application)excelApp;
                    if (app != null)
                    {
                        app.DisplayAlerts = false; // Отключаем различные сообщения

                        bool wbkOpened;
                        var wbk = OpenWbk(app, NewAppCreated, wbkFullPath, out wbkOpened);
                        if (wbk != null)
                        {
                            var helper = new SetupWorksheetHelper(wbk);

                            wbk.Save();
                            wbk.Close(); // Закрываем книгу, в следующий раз она молчаливо себя почистит

                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        private MSExcel.Workbook OpenWbk(MSExcel.Application excelApp, bool NewAppCreated, string wbkFullPath, out bool WbkOpened)
        {
            MSExcel.Workbook result = null;

            WbkOpened = false;

            // Открываем книгу
            lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
            {
                if (NewAppCreated)
                {   // Без этого книга не открывается
                    excelApp.Visible = true;
                    excelApp.WindowState = MSExcel.XlWindowState.xlMinimized;
                }
                foreach (MSExcel.Workbook book in excelApp.Workbooks)
                {
                    if (book.FullName == wbkFullPath)
                    {   // Книга уже открыта => используем её
                        result = book;
                        break;
                    }
                }
                if (result == null)
                {
                    result = excelApp.Workbooks.Open(wbkFullPath);
                    WbkOpened = true;
                }
                if (NewAppCreated)
                {
                    excelApp.Visible = false;
                    excelApp.WindowState = MSExcel.XlWindowState.xlNormal;
                }
            }

            return result;
        }

        protected bool ExportDataToWbk(string wbkFullPath,
            string wbkTemplateFullPath,
            string compName,
            GroupItem groupDesc,
            IEnumerable<CFullMemberInfo> data)
        {
            short WbkFlagsValue = 0;

            // Создаём книгу
            File.Copy(wbkTemplateFullPath, wbkFullPath, true);

            try
            {
                bool NewAppCreated;
                using (var excelApp = new DisposableWrapper<MSExcel.Application>(GlobalDefines.StartExcel(out NewAppCreated),
                                            app =>
                                            {
                                                if (app != null)
                                                {
                                                    app.DisplayAlerts = true;

                                                    if (NewAppCreated)
                                                        app.Quit();

                                                    app = null;
                                                }
                                            }))
                {
                    MSExcel.Application app = (MSExcel.Application)excelApp;
                    if (app != null)
                    {
                        app.DisplayAlerts = false; // Отключаем различные сообщения

                        bool wbkOpened;
                        var wbk = OpenWbk(app, NewAppCreated, wbkFullPath, out wbkOpened);
                        if (wbk != null)
                        {
                            
                            wbk.Save();
                            wbk.Close(); // Закрываем книгу, в следующий раз она молчаливо себя почистит

                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
    }
}
