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
        #region Consts
        const string SETUP_SHEET_NAME = "Setup";

        const string RN_FLAGS = "FLAGS";
        const string RN_ON_SHEET_FLAGS = "OnSheetFlags";
        
        const string RN_INIT_ON_SHEET_FLAGS_VALUE = "InitOnSheetFlagsValue";
        const string RN_REQUEST = "Request";

        const int REQUEST_LOAD_FLAGS = 1;
        const int REQUEST_CLEAR_BOOK_SILENTLY = 2;
        const int CLEAR_WBK_FLAGS_VALUE = 64;

        #endregion

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
                            var wshSetup = wbk.Worksheets[SETUP_SHEET_NAME];
                            wshSetup.Range[RN_FLAGS].Value = CLEAR_WBK_FLAGS_VALUE;
                            wshSetup.Range[RN_ON_SHEET_FLAGS].Value = wshSetup.Range[RN_INIT_ON_SHEET_FLAGS_VALUE].Value;
                            wshSetup.Range[RN_REQUEST].Value = REQUEST_LOAD_FLAGS; // Посылаем запрос на сохранение флагов

                            Thread.Sleep(100);

                            wshSetup.Range[RN_REQUEST].Value = REQUEST_CLEAR_BOOK_SILENTLY; // Посылаем запрос на молчаливую очистку книги при следующем открытии
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
                            var wshSetup = wbk.Worksheets[SETUP_SHEET_NAME];


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
