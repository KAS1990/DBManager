using DBManager.Global;
using DBManager.SettingsWriter;
using DBManager.Stuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace DBManager.Scanning
{
    /// <summary>
    /// Поток, который ищет изменения в папке, содержащей все книги, в которых содержатся результаты участников
    /// </summary>
    public class CDirScanner : CScannerBase
    {
        public class CSyncParam : CScannerBase.CSyncParamBase
        {
            public string m_Dir = GlobalDefines.DEFAULT_XML_STRING_VAL;
            public List<CFileScannerSettings> m_lstFileScannerSettings = new List<CFileScannerSettings>();


            public CSyncParam(string Dir, List<CFileScannerSettings> lstFileScannerSettings) :
                base()
            {
                m_Dir = Dir;
                m_lstFileScannerSettings = lstFileScannerSettings;
            }
        }


        /// <summary>
        /// Объект, который следит за изменениями в файловой системе.
        /// Все изменения отслеживаются с помощью событий, которые вызываются не в потоке интерфейса,
        /// а в каком-то отдельном, созданном FileSystemWatcher
        /// </summary>
        private readonly FileSystemWatcher m_PathWatcher = new FileSystemWatcher()
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite, /* нас интересует только изменение названия файла,
                                                                              * а также дата последнего изменения */
            Filter = "*.xml",	// отслеживаем только xml-файлы, т.к. в них пишет инофрмацю Excel
        }; // инициализацию m_PathWatcher нужно делать именно здесь, а не в конструкторе, т.к метод Start вызывется раньше создания класса


        #region FileScanners
        /// <summary>
        /// Словарь, содержащий все сканеры файлов, в которых есть результаты участнокиков, относящихся к соревам
        /// Ключ - полный путь к файлу
        /// </summary>
        private readonly Dictionary<string, CFileScanner> m_FileScanners = new Dictionary<string, CFileScanner>();
        public Dictionary<string, CFileScanner> FileScanners
        {
            get { return m_FileScanners; }
        }
        #endregion


        #region CompId
        protected long m_CompId = GlobalDefines.DEFAULT_XML_INT_VAL;
        /// <summary>
        /// Идентификатор соревнований в БД
        /// </summary>
        public long CompId
        {
            get { return m_CompId; }
            set
            {
                LastException = null;

                if (m_CompId != value)
                {
                    if (m_CompId < 0 && value >= 0)
                    {	// Меняем идентификатор в БД
                        try
                        {
                            IEnumerable<descriptions> DescInDB = (from desc in DBManagerApp.m_Entities.descriptions
                                                                  where (CompId == desc.id_desc)
                                                                  select desc).ToList();
                            if (DescInDB.Count() > 0)
                            {
                                DescInDB.First().id_desc = value;
                                DBManagerApp.m_Entities.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            List<CDataChangedInfo> Changes = new List<CDataChangedInfo>();
                            OnException(ref Changes, ex, value);
                        }
                    }
                }

                m_CompId = value;
            }
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ScanningDir">
        /// Путь к папке
        /// </param>
        /// <param name="ParentScanner">
        /// 
        /// </param>
        /// <param name="MadeChanges"></param>
        /// <param name="Sync">
        /// Нужно ли выполнить синхронизацию объекта с БД и файлами, уже имеющимися в каталоге.
        /// При этом вызывается функция SyncWithFilesAndDB
        /// </param>
        /// <param name="SyncParam">
        /// 
        /// </param>
        public CDirScanner(string ScanningDir,
                            CScannerBase ParentScanner,
                            bool Sync,
                            CSyncParam SyncParam = null) :
            base(ScanningDir, ParentScanner, Sync, SyncParam)
        {
            m_PathWatcher.Created += m_PathWatcher_Created;
            m_PathWatcher.Deleted += m_PathWatcher_Deleted;
            m_PathWatcher.Changed += m_PathWatcher_Changed;
            m_PathWatcher.Renamed += m_PathWatcher_Renamed;
        }


        /// <summary>
        /// Запустить сканирование
        /// </summary>
        /// <param name="ScanningDir"></param>
        /// <param name="MadeChanges"></param>
        /// <returns></returns>
        public override bool Start(string ScanningDir)
        {
            LastException = null;

            if (State == enScanningThreadState.Worked)
                // Поток в данный момент работает
                return false;

            if (!Directory.Exists(ScanningDir))
                return false;

            ScanningPath = ScanningDir;

            // Пытаемся запустить все все сканеры файлов
            List<string> ScannersPaths = m_FileScanners.Keys.ToList();
            for (int i = 0; i < ScannersPaths.Count;)
            {
                CFileScanner scanner = m_FileScanners[ScannersPaths[i]];
                scanner.ScanningPath = ScannersPaths[i];
                if (scanner.Start(scanner.ScanningPath))
                    i++;
                else
                {	// Запустить сканер почему-то не получилось
                    lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                        DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(scanner.ScanningPath);

                    m_FileScanners.Remove(ScannersPaths[i]);
                    ScannersPaths.RemoveAt(i);
                }
            }

            m_PathWatcher.BeginInit();
            m_PathWatcher.Path = ScanningDir;
            m_PathWatcher.EnableRaisingEvents = true; // Эту операцию нужно делать после запуска всех сканеров файлов
            m_PathWatcher.EndInit();

            State = enScanningThreadState.Worked;

            return true;
        }


        /// <summary>
        /// Остановить сканирование
        /// </summary>
        public override void Stop(bool OnRestart)
        {
            LastException = null;

            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;

            if ((DBManagerApp.MainWnd as DispatcherObject).CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        DBManagerApp.MainWnd.Title,
                                        string.Format(Properties.Resources.resfmtStoppingDirScanningThread, ScanningPath),
                                        DBManagerApp.MainWnd);
            }

            lock (EventsCS)	// Ждём, когда завершаться все события
            {
                if (State != enScanningThreadState.Stopped)
                {
                    m_PathWatcher.EnableRaisingEvents = false; // Останавливаем сканирование
                    ScanningPath = "";

                    foreach (KeyValuePair<string, CFileScanner> Scanner in m_FileScanners)
                        Scanner.Value.Stop(false);

                    State = enScanningThreadState.Stopped;
                }

                if (OnRestart)
                {
                    m_FileScanners.Clear();
                    CompId = GlobalDefines.DEFAULT_XML_INT_VAL;
                }
            }

            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="MadeChanges"></param>
        public override bool SyncWithFilesAndDB(CScannerBase.CSyncParamBase Param)
        {
            LastException = null;

            CSyncParam SyncParam = Param as CSyncParam;

            if (SyncParam == null ||
                SyncParam.m_Dir == GlobalDefines.DEFAULT_XML_STRING_VAL ||
                !Directory.Exists(SyncParam.m_Dir))
            {
                return false;
            }

            using (var wrapper = new DisposableWrapper<ShowAsyncResult>(CWaitingWnd.ShowAsync(DBManagerApp.MainWnd.Title,
                                                                                            string.Format(Properties.Resources.resfmtSyncingDir, SyncParam.m_Dir),
                                                                                            DBManagerApp.MainWnd,
                                                                                            true),
                                            asyncResult =>
                                            {
                                                if (asyncResult?.hFinishedSearchEvent != null)
                                                    asyncResult.hFinishedSearchEvent.Set();
                                            }))
            {
                lock (EventsCS)
                {
                    if (State == enScanningThreadState.Worked)
                    {   // Синхронизацию можно проводить только при незапущенном сканировании
                        return false;
                    }

                    m_PathWatcher.EnableRaisingEvents = false;

                    List<string> ScannedFilesFullPaths = new List<string>();

                    try
                    {
                        if (SyncParam.m_lstFileScannerSettings != null)
                        {
                            bool AllFilesSync = true;

                            foreach (CFileScannerSettings ScannerSettings in SyncParam.m_lstFileScannerSettings)
                            {
                                if (Path.GetDirectoryName(ScannerSettings.FullFilePath) != SyncParam.m_Dir)
                                {   // Файл не находится в просматриваемой папке => он нам не нужен
                                    AllFilesSync = false;
                                    lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                                        DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(ScannerSettings.FullFilePath);
                                    continue;
                                }

                                string FullScannerFilePath = Path.Combine(SyncParam.m_Dir, ScannerSettings.FullFilePath);
                                ScannedFilesFullPaths.Add(FullScannerFilePath);

                                CFileScanner Scanner = null;
                                if (m_FileScanners.TryGetValue(FullScannerFilePath, out Scanner))
                                {
                                    m_FileScanners[FullScannerFilePath] =
                                        Scanner = new CFileScanner(ScannerSettings.FullFilePath,
                                                                    this,
                                                                    true,
                                                                    new CFileScanner.CSyncParam(ScannerSettings.GroupId,
                                                                                                FullScannerFilePath));
                                    if (!Scanner.SyncSuccessfully)
                                    {   // Синхронизироваться не удалось
                                        m_FileScanners.Remove(FullScannerFilePath);
                                        AllFilesSync = false;
                                        lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                                            DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(ScannerSettings.FullFilePath);
                                    }
                                }
                                else
                                {
                                    Scanner = new CFileScanner(ScannerSettings.FullFilePath,
                                                                        this,
                                                                        true,
                                                                        new CFileScanner.CSyncParam(ScannerSettings.GroupId,
                                                                                                    FullScannerFilePath));
                                    if (Scanner.SyncSuccessfully)
                                        m_FileScanners.Add(ScannerSettings.FullFilePath, Scanner);
                                    else
                                    {   // Синхронизироваться не удалось
                                        AllFilesSync = false;
                                        lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                                            DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(ScannerSettings.FullFilePath);
                                    }
                                }
                            }

                            if (!AllFilesSync)
                                DBManagerApp.m_AppSettings.Write();
                        }

                        // Пытаемся загрузить данные из всех остальных XML-файлов, имеющихся в папке
                        string[] AllXMLFullFilePaths = Directory.GetFiles(SyncParam.m_Dir, "*.xml");
                        foreach (string FullFilePath in from xmlFileL in AllXMLFullFilePaths
                                                        join xmlFileR in ScannedFilesFullPaths on xmlFileL equals xmlFileR into XMLFiles
                                                        from scannedFile in XMLFiles.DefaultIfEmpty()
                                                        where scannedFile == null
                                                        select xmlFileL)
                        {
                            CFileScanner Scanner = null;
                            if (m_FileScanners.TryGetValue(FullFilePath, out Scanner))
                            {
                                m_FileScanners[FullFilePath] =
                                    Scanner = new CFileScanner(FullFilePath,
                                                                this,
                                                                true,
                                                                new CFileScanner.CSyncParam(m_FileScanners[FullFilePath].Group == null ?
                                                                                                GlobalDefines.NO_OUR_COMP_IN_DB :
                                                                                                m_FileScanners[FullFilePath].Group.id_group,
                                                                                            FullFilePath));
                                if (!Scanner.SyncSuccessfully)
                                {   // Синхронизироваться не удалось
                                    m_FileScanners.Remove(FullFilePath);
                                    lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                                        DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.Remove(FullFilePath);
                                    continue;
                                }
                            }
                            else
                            {
                                Scanner = new CFileScanner(FullFilePath,
                                                            this,
                                                            true,
                                                            new CFileScanner.CSyncParam(GlobalDefines.NO_OUR_COMP_IN_DB,
                                                                                        FullFilePath));

                                if (Scanner.SyncSuccessfully)
                                {   // Удалось синхронизироваться => добавляем сканер в m_FileScanners и в файл настроек
                                    m_FileScanners.Add(FullFilePath, Scanner);
                                }
                            }

                            if (Scanner.Group != null)
                            {
                                CFileScannerSettings ScannerSettings = new CFileScannerSettings()
                                {
                                    FullFilePath = FullFilePath,
                                    GroupId = Scanner.Group.id_group
                                };
                                lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                                    DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.TryAddValue(FullFilePath, ScannerSettings);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        List<CDataChangedInfo> Changes = new List<CDataChangedInfo>();
                        OnException(ref Changes, ex, CompId);
                    }

                    if (Directory.Exists(m_PathWatcher.Path))
                        m_PathWatcher.EnableRaisingEvents = true;
                }
            }

            return true;
        }


        #region События m_PathWatcher
        private void m_PathWatcher_Created(object sender, FileSystemEventArgs e)
        {
            LastException = null;

            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;

            if ((DBManagerApp.MainWnd as DispatcherObject).CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        DBManagerApp.MainWnd.Title,
                                        string.Format(Properties.Resources.resfmtScanningFile, e.FullPath),
                                        DBManagerApp.MainWnd);
            }

            lock (EventsCS)
            {
                if (State != enScanningThreadState.Worked)
                    return;

                m_PathWatcher.EnableRaisingEvents = false;

                CFileScanner Scanner;
                if (m_FileScanners.TryGetValue(e.FullPath, out Scanner))
                    Scanner.Start(e.FullPath);
                else
                {
                    Scanner = new CFileScanner(e.FullPath, this, false);
                    if (Scanner.State == enScanningThreadState.Worked)
                        m_FileScanners.Add(e.FullPath, Scanner);
                }

                DBManagerApp.m_Entities.SaveChanges();

                m_PathWatcher.EnableRaisingEvents = true;
            }

            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();
        }


        private void m_PathWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            LastException = null;

            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                if (!DBManagerApp.m_AppSettings.m_Settings.HandleFileDeletion)
                    return;

            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;

            if ((DBManagerApp.MainWnd as DispatcherObject).CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        DBManagerApp.MainWnd.Title,
                                        string.Format(Properties.Resources.resfmtHandlingFileDeletion, e.FullPath),
                                        DBManagerApp.MainWnd);
            }

            lock (EventsCS)
            {
                if (State != enScanningThreadState.Worked)
                    return;

                CFileScanner Scanner;
                if (m_FileScanners.TryGetValue(e.FullPath, out Scanner))
                {
                    m_PathWatcher.EnableRaisingEvents = false;

                    if (Scanner.Group != null)
                    {
                        long GroupId = Scanner.Group.id_group;

                        if (Scanner.DeleteGroup()) // DeleteGroup автоматически вызовет Stop
                        {
                            m_FileScanners.Remove(e.FullPath);

                            List<CDataChangedInfo> Changes = new List<CDataChangedInfo>();
                            Changes.Add(new CDataChangedInfo(this)
                            {
                                ChangedObjects = enDataChangedObjects.Paths | enDataChangedObjects.Group,
                                ChangingType = enDataChangesTypes.Delete,
                                ID = GroupId,
                                Argument = e.FullPath,
                                GroupID = GroupId
                            });
                            RaiseDataChangedEvent(new DataChangedEventArgs(Changes));
                        }
                    }

                    m_PathWatcher.EnableRaisingEvents = true;
                }
            }

            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();
        }


        private void m_PathWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //GlobalDefines.m_swchGlobal.Restart();

            LastException = null;

            AutoResetEvent hFinishedSearchEvent = null;
            Thread th = null;

            if ((DBManagerApp.MainWnd as DispatcherObject).CheckAccess())
            {
                CWaitingWnd.ShowAsync(out hFinishedSearchEvent,
                                        out th,
                                        DBManagerApp.MainWnd.Title,
                                        string.Format(Properties.Resources.resfmtScanningFile, e.FullPath),
                                        DBManagerApp.MainWnd);
            }

            lock (EventsCS)
            {
                if (State != enScanningThreadState.Worked)
                    return;

                CFileScanner Scanner;
                if (m_FileScanners.TryGetValue(e.FullPath, out Scanner))
                {
                    m_PathWatcher.EnableRaisingEvents = false; // т.к. в XMLFileChanged может быть перезапись файла, обрабатывать которую не нужно

                    List<CDataChangedInfo> MadeChanges = Scanner.XMLFileChanged();
                    if (MadeChanges != null)
                    {
                        try
                        {
                            DBManagerApp.m_Entities.SaveChanges();
                        }
                        catch
                        { }

                        RaiseDataChangedEvent(new DataChangedEventArgs(MadeChanges));
                    }

                    m_PathWatcher.EnableRaisingEvents = true;
                }
            }

            if (hFinishedSearchEvent != null)
                hFinishedSearchEvent.Set();
        }


        private void m_PathWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            LastException = null;

            lock (EventsCS)
            {
                if (State != enScanningThreadState.Worked)
                    return;

                CFileScanner Scanner;
                if (m_FileScanners.TryGetValue(e.OldFullPath, out Scanner))
                {
                    m_PathWatcher.EnableRaisingEvents = false;

                    Scanner.ScanningPath = e.FullPath;

                    if (Scanner.LastException == null)
                    {	// Удалось сменить название файла
                        m_FileScanners.Remove(e.OldFullPath);
                        m_FileScanners.Add(Scanner.ScanningPath, Scanner);

                        lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                        {
                            CFileScannerSettings settings;
                            if (DBManagerApp.m_AppSettings.m_Settings.dictFileScannerSettings.TryGetValue(e.OldFullPath, out settings))
                            {	// Информация об этом файле в настройках есть => нужно изменить название файла
                                settings.FullFilePath = e.FullPath;
                                DBManagerApp.m_AppSettings.Write();
                            }
                        }
                    }

                    m_PathWatcher.EnableRaisingEvents = true;
                }
            }
        }
        #endregion
    }
}
