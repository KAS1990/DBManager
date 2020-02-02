using DBManager.Commands;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace DBManager.OnlineResults
{
    /// <summary>
    /// Interaction logic for CPublishingResultsLogWnd.xaml
    /// </summary>
    public partial class CPublishingResultsLogWnd : CNotifyPropertyChangedWnd
    {
        public class CGroupItem : INotifyPropertyChanged
        {
            #region id
            private long m_id = -1;
            /// <summary>
            /// 
            /// </summary>
            public long id
            {
                get { return m_id; }
                private set { m_id = value; }
            }
            #endregion


            #region GroupName
            private static readonly string GroupNamePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.GroupName);
            private string m_GroupName = null;
            /// <summary>
            /// Название группы
            /// </summary>
            public string GroupName
            {
                get { return m_GroupName; }
                set
                {
                    if (m_GroupName != value)
                    {
                        m_GroupName = value;
                        OnPropertyChanged(GroupNamePropertyName);
                    }
                }
            }
            #endregion


            #region FileName
            private static readonly string FileNamePropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.FileName);
            private string m_FileName = null;
            /// <summary>
            /// Название файла
            /// </summary>
            public string FileName
            {
                get { return m_FileName; }
                set
                {
                    if (m_FileName != value)
                    {
                        m_FileName = value;
                        OnPropertyChanged(FileNamePropertyName);
                    }
                }
            }
            #endregion


            #region ClearCommaand
            private static readonly string ClearCommaandPropertyName = GlobalDefines.GetPropertyName<CGroupItem>(m => m.FileName);
            private CCommand m_ClearCommaand = null;
            /// <summary>
            /// Очистка лога группы и его удаление
            /// </summary>
            public CCommand ClearCommaand
            {
                get { return m_ClearCommaand; }
                set
                {
                    if (m_ClearCommaand != value)
                    {
                        m_ClearCommaand = value;
                        OnPropertyChanged(ClearCommaandPropertyName);
                    }
                }
            }
            #endregion


            #region Items
            private ObservableCollectionEx<CLogItem> m_Items = new ObservableCollectionEx<CLogItem>();
            /// <summary>
            /// Список элементов
            /// </summary>
            public ObservableCollectionEx<CLogItem> Items
            {
                get { return m_Items; }
                set
                {
                    if (m_Items != value)
                    {
                        m_Items = value;
                    }
                }
            }
            #endregion


            public CGroupItem(long GroupId)
            {
                id = GroupId;
            }


            #region OnPropertyChanged and PropertyChanged event
            public event PropertyChangedEventHandler PropertyChanged;


            public virtual void OnPropertyChanged(string info)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
            #endregion
        }


        public class CCompItem : INotifyPropertyChanged
        {
            #region id
            private long m_id = -1;
            /// <summary>
            /// 
            /// </summary>
            public long id
            {
                get { return m_id; }
                private set { m_id = value; }
            }
            #endregion


            #region CompName
            private static readonly string CompNamePropertyName = GlobalDefines.GetPropertyName<CCompItem>(m => m.CompName);
            private string m_CompName = null;
            /// <summary>
            /// Название соревнования
            /// </summary>
            public string CompName
            {
                get { return m_CompName; }
                set
                {
                    if (m_CompName != value)
                    {
                        m_CompName = value;
                        OnPropertyChanged(CompNamePropertyName);
                    }
                }
            }
            #endregion


            #region FolderName
            private static readonly string FolderNamePropertyName = GlobalDefines.GetPropertyName<CCompItem>(m => m.FolderName);
            private string m_FolderName = null;
            /// <summary>
            /// Название папки
            /// </summary>
            public string FolderName
            {
                get { return m_FolderName; }
                set
                {
                    if (m_FolderName != value)
                    {
                        m_FolderName = value;
                        OnPropertyChanged(FolderNamePropertyName);
                    }
                }
            }
            #endregion


            #region Groups
            private ObservableCollectionEx<CGroupItem> m_Groups = new ObservableCollectionEx<CGroupItem>();
            /// <summary>
            /// Список групп
            /// </summary>
            public ObservableCollectionEx<CGroupItem> Groups
            {
                get { return m_Groups; }
                set
                {
                    if (m_Groups != value)
                    {
                        m_Groups = value;
                    }
                }
            }
            #endregion


            public CCompItem(long CompId)
            {
                id = CompId;

                Groups.CollectionChanged += Groups_CollectionChanged;
            }


            private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        if (e.NewItems != null)
                        {
                            Groups.CollectionChanged -= Groups_CollectionChanged;

                            foreach (CGroupItem item in e.NewItems)
                            {
                                item.ClearCommaand = new CCommand(() =>
                                {
                                    if (MessageBox.Show(Properties.Resources.resDeleteLogQuestion,
                                                        Properties.Resources.resDeletion,
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question,
                                                        MessageBoxResult.No) == MessageBoxResult.Yes)
                                    {
                                        GlobalDefines.CheckPublishingDirExists();

                                        File.Delete(string.Format("{0}{1}\\{2}{3}",
                                                                    GlobalDefines.STD_PUBLISHING_LOG_DIR,
                                                                    id,
                                                                    item.id,
                                                                    GlobalDefines.PUBLISHING_LOG_FILE_EXTENSION));
                                        Groups.Remove(item);

                                        if (Groups.Count == 0)
                                        {
                                            Directory.Delete(string.Format("{0}{1}",
                                                                            GlobalDefines.STD_PUBLISHING_LOG_DIR,
                                                                            id));
                                            RaiseDeleteMe();
                                        }
                                    }
                                });
                            }

                            Groups.CollectionChanged += Groups_CollectionChanged;
                        }
                        break;
                }
            }


            public event EventHandler DeleteMe;

            private void RaiseDeleteMe()
            {
                if (DeleteMe != null)
                    DeleteMe(this, new EventArgs());
            }


            #region OnPropertyChanged and PropertyChanged event
            public event PropertyChangedEventHandler PropertyChanged;


            public virtual void OnPropertyChanged(string info)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
            #endregion
        }


        #region Comps
        private static readonly string CompsPropertyName = GlobalDefines.GetPropertyName<CPublishingResultsLogWnd>(m => m.Comps);
        private readonly ObservableCollectionEx<CCompItem> m_Comps = new ObservableCollectionEx<CCompItem>();
        /// <summary>
        /// Коллекция, содержащий все соревнования
        /// </summary>
        public ObservableCollectionEx<CCompItem> Comps
        {
            get { return m_Comps; }
        }
        #endregion


        /// <summary>
        /// Объект, который следит за изменениями в файловой системе.
        /// Все изменения отслеживаются с помощью событий, которые вызываются не в потоке интерфейса,
        /// а в каком-то отдельном, созданном FileSystemWatcher
        /// </summary>
        private readonly FileSystemWatcher m_LogFileWatcher = new FileSystemWatcher()
        {
            NotifyFilter = NotifyFilters.LastWrite, /* нас интересует только изменение названия файла,
                                                     * а также дата последнего изменения */
            Filter = "*" + GlobalDefines.PUBLISHING_LOG_FILE_EXTENSION,	// отслеживаем только xml-файлы, т.к. в них пишет инофрмацю Excel
            IncludeSubdirectories = true
        }; // инициализацию m_PathWatcher нужно делать именно здесь, а не в конструкторе, т.к метод Start вызывется раньше создания класса


        public CPublishingResultsLogWnd()
        {
            InitializeComponent();

            GlobalDefines.CheckPublishingDirExists();

            txtLogFilePath.Text = GlobalDefines.STD_PUBLISHING_LOG_DIR;

            m_LogFileWatcher.Changed += m_LogFileWatcher_Changed;

            m_LogFileWatcher.BeginInit();
            m_LogFileWatcher.Path = GlobalDefines.STD_PUBLISHING_LOG_DIR;
            m_LogFileWatcher.EnableRaisingEvents = true; // Эту операцию нужно делать после запуска всех сканеров файлов
            m_LogFileWatcher.EndInit();

            Comps.CollectionChanged += Comps_CollectionChanged;

            RefreshItems();
        }


        private void Comps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        Comps.CollectionChanged -= Comps_CollectionChanged;

                        foreach (CCompItem item in e.NewItems)
                        {
                            item.DeleteMe += (s, ev) =>
                            {
                                Comps.Remove(s as CCompItem);
                            };
                        }

                        Comps.CollectionChanged += Comps_CollectionChanged;
                    }
                    break;
            }
        }

        private CGroupItem AddNewGroup(CCompItem Comp, long GroupId)
        {
            CGroupItem result = new CGroupItem(GroupId)
            {
                FileName = GroupId.ToString() + GlobalDefines.PUBLISHING_LOG_FILE_EXTENSION
            };
            groups gr = DBManagerApp.m_Entities.groups.FirstOrDefault(arg => arg.id_group == GroupId);
            if (gr == null)
                return null;
            else
            {
                CCompSettings Group = new CCompSettings(gr);
                result.GroupName = Group.AgeGroup.FullGroupName;
            }
            Comp.Groups.Add(result);

            return result;
        }

        private bool HandleFile(string FullFilePath)
        {
            CGroupItem GroupChanged = null;
            CCompItem Comp = null;

            long id = -1;
            string FileDir = System.IO.Path.GetDirectoryName(FullFilePath);
            int LastSlash = FileDir.LastIndexOf('\\');
            if (long.TryParse(FileDir.Right(FileDir.Length - LastSlash - 1), out id))
            {
                Comp = Comps.FirstOrDefault(arg => arg.id == id);
                if (Comp == null)
                {	// Такого соревнования нет => его нужно добавить
                    Comp = new CCompItem(id)
                    {
                        FolderName = id.ToString() + "\\"
                    };
                    descriptions desc = DBManagerApp.m_Entities.descriptions.FirstOrDefault(arg => arg.id_desc == id);
                    if (desc == null)
                        return false;
                    else
                        Comp.CompName = desc.name;

                    if (long.TryParse(System.IO.Path.GetFileNameWithoutExtension(FullFilePath), out id))
                    {
                        GroupChanged = AddNewGroup(Comp, id);
                        if (GroupChanged == null)
                            return false;
                    }
                    else
                    {	// неверное название файла
                        return false;
                    }

                    Comps.Add(Comp);
                }
                else
                {
                    if (long.TryParse(System.IO.Path.GetFileNameWithoutExtension(FullFilePath), out id))
                    {
                        GroupChanged = Comp.Groups.FirstOrDefault(arg => arg.id == id);
                        if (GroupChanged == null)
                        {	// Группы нет => добавляем
                            GroupChanged = AddNewGroup(Comp, id);
                            if (GroupChanged == null)
                                return false;
                        }
                    }
                }
            }
            else
            {	// неверное название файла
                return false;
            }

            GroupChanged.Items.Clear();

            GlobalDefines.CheckPublishingDirExists();

            List<CLogItem> lstItems = new List<CLogItem>();
            try
            {
                using (TextReader tr = new StreamReader(string.Format("{0}{1}\\{2}{3}",
                                                                        GlobalDefines.STD_PUBLISHING_LOG_DIR,
                                                                        Comp.id,
                                                                        GroupChanged.id,
                                                                        GlobalDefines.PUBLISHING_LOG_FILE_EXTENSION)))
                {
                    string line = null;
                    CLogItem CurLogItem = null;
                    DateTime ItemDateTime = DateTime.Now;
                    enOnlineResultsLogItemType ItemType = enOnlineResultsLogItemType.None;

                    do
                    {
                        line = tr.ReadLine();

                        if (line == null)
                        {	// Файл закончился
                            break;
                        }
                        else
                        {
                            string[] Fields = line.Split(GlobalDefines.PUBLISHING_LOG_FIELDS_SEPARATOR);
                            // Переносы строк не используем в логе для удобства, поэтому добавляем их там, где они нужны
                            for (int i = 0; i < Fields.Length; i++)
                                Fields[i] = Fields[i].Replace(GlobalDefines.PUBLISHING_LOG_LFCR_SYMBOL, '\n');
                            int Index = 0;
                            if (Enum.TryParse<enOnlineResultsLogItemType>(Fields[Index++], out ItemType) &&
                                DateTime.TryParse(Fields[Index++], out ItemDateTime))
                            {
                                CurLogItem = new CLogItem()
                                {
                                    Type = ItemType,
                                    CreationDate = ItemDateTime,
                                };

                                if (Fields.Length > Index)
                                    CurLogItem.PCWbkName = Fields[Index].Trim();
                                Index++;

                                if (Fields.Length > Index)
                                    CurLogItem.Text = Fields[Index].Trim();
                                Index++;

                                lstItems.Add(CurLogItem);
                            }
                        }
                    }
                    while (line != null);
                }
            }
            catch
            { }

            lstItems.Sort((lhs, rhs) => -lhs.CreationDate.CompareTo(rhs.CreationDate));
            GroupChanged.Items.AddRange(lstItems);

            return true;
        }

        private bool RefreshItems(string FullFilePath = null)
        {
            if (FullFilePath == null)
            {
                Comps.Clear();

                GlobalDefines.CheckPublishingDirExists();

                foreach (string Dir in Directory.EnumerateDirectories(GlobalDefines.STD_PUBLISHING_LOG_DIR))
                {
                    try
                    {
                        foreach (string FullPath in Directory.EnumerateFiles(Dir))
                            HandleFile(FullPath);
                    }
                    catch
                    { }
                }
                return true;
            }
            else
                return HandleFile(FullFilePath);
        }


        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshItems();
        }

        private bool m_IsLoaded = false;

        private void CPublishingResultsLogWnd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!m_IsLoaded)
                return;

            if (e.HeightChanged)
            {
                double NewSize = tctrlComps.Height + (e.NewSize.Height - e.PreviousSize.Height);
                if (NewSize >= tctrlComps.MinHeight)
                    tctrlComps.Height = NewSize;
            }

            if (e.WidthChanged)
            {
                double NewSize = tctrlComps.Width + (e.NewSize.Width - e.PreviousSize.Width);
                if (NewSize >= tctrlComps.MinWidth)
                    tctrlComps.Width = NewSize;
            }
        }


        private void CPublishingResultsLogWnd_Loaded(object sender, RoutedEventArgs e)
        {
            Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            MinWidth = DesiredSize.Width;
            MinHeight = DesiredSize.Height;

            tctrlComps.Width = tctrlComps.MinWidth = GlobalDefines.GetActualControlWidth(grdItems);
            tctrlComps.Height = tctrlComps.MinHeight = GlobalDefines.GetActualControlHeight(grdItems);

            tctrlComps.MaxHeight = tctrlComps.MaxWidth = double.PositiveInfinity;

            m_IsLoaded = true;
        }


        private void m_LogFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            m_LogFileWatcher.EnableRaisingEvents = false;

            RefreshItems();

            m_LogFileWatcher.EnableRaisingEvents = true;
        }
    }
}
