using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DBManager.Stuff
{
    /// <summary>
    /// Interaction logic for CLogWnd.xaml
    /// </summary>
    public partial class CLogWnd : CNotifyPropertyChangedWnd
    {
        public class CExceptionItem
        {
            #region Index
            private int m_Index = 0;

            public int Index
            {
                get { return m_Index; }
                set
                {
                    if (m_Index != value)
                    {
                        m_Index = value;
                    }
                }
            }
            #endregion


            #region Text
            private string m_Text = null;

            public string Text
            {
                get { return m_Text; }
                set
                {
                    if (m_Text != value)
                    {
                        m_Text = value;
                    }
                }
            }
            #endregion


            #region IndexInString
            public string IndexInString
            {
                get { return "№" + Index.ToString(); }
            }
            #endregion
        }


        public class CLogItem
        {
            #region CreationDate
            private DateTime m_CreationDate = DateTime.Now;

            public DateTime CreationDate
            {
                get { return m_CreationDate; }
                set
                {
                    if (m_CreationDate != value)
                    {
                        m_CreationDate = value;
                    }
                }
            }

            public string CreationDateInString
            {
                get { return CreationDate.ToString(); }
            }
            #endregion


            #region Exceptions
            private List<CExceptionItem> m_Exceptions = null;
            /// <summary>
            /// Список исключений
            /// </summary>
            public List<CExceptionItem> Exceptions
            {
                get { return m_Exceptions; }
                set
                {
                    if (m_Exceptions != value)
                    {
                        m_Exceptions = value;
                    }
                }
            }
            #endregion
        }

        #region Items
        private static readonly string ItemsPropertyName = GlobalDefines.GetPropertyName<CLogWnd>(m => m.Items);
        private readonly ObservableCollectionEx<CLogItem> m_Items = new ObservableCollectionEx<CLogItem>();
        /// <summary>
        /// Словарь, содержащий все группы
        /// </summary>
        public ObservableCollectionEx<CLogItem> Items
        {
            get { return m_Items; }
        }
        #endregion


        #region Свойство LastHeaderClicked
        private static readonly string LastHeaderClickedPropertyName = GlobalDefines.GetPropertyName<CLogWnd>(m => m.LastHeaderClicked);
        private GridViewColumnHeader m_LastHeaderClicked = null;
        /// <summary>
        /// Заголовок, на котором нажимали последний раз
        /// </summary>
        public GridViewColumnHeader LastHeaderClicked
        {
            get { return m_LastHeaderClicked; }
            set
            {
                if (m_LastHeaderClicked != value)
                {
                    m_LastHeaderClicked = value;
                    OnPropertyChanged(LastHeaderClickedPropertyName);
                }
            }
        }
        #endregion


        #region Свойство LastSortDirection
        private static readonly string LastSortDirectionPropertyName = GlobalDefines.GetPropertyName<CLogWnd>(m => m.LastSortDirection);
        private ListSortDirection m_LastSortDirection = ListSortDirection.Ascending;
        /// <summary>
        /// Последний выбранный вариант сортировки столбца LastHeaderClicked
        /// </summary>
        public ListSortDirection LastSortDirection
        {
            get { return m_LastSortDirection; }
            set
            {
                if (m_LastSortDirection != value)
                {
                    m_LastSortDirection = value;
                    OnPropertyChanged(LastSortDirectionPropertyName);
                }
            }
        }
        #endregion


        public void RefreshItems()
        {
            Items.Clear();
            List<CLogItem> lstItems = new List<CLogItem>();
            try
            {
                using (TextReader tr = new StreamReader(GlobalDefines.STD_ERROR_LOG_FILE_PATH))
                {
                    string line = null;
                    CLogItem CurLogItem = null;
                    CExceptionItem CurExeptionItem = null;
                    DateTime ItemTime = DateTime.Now;

                    do
                    {
                        line = tr.ReadLine();

                        if (line == null)
                        {   // Файл закончился
                            break;
                        }
                        else if (line == GlobalDefines.LOG_EXCEPTION_TERMINAL_LINE)
                        {   // Запись в логе закончилась
                            if (CurExeptionItem != null)
                            {
                                CurLogItem.Exceptions.Add(CurExeptionItem);
                                CurExeptionItem = null;
                            }
                            lstItems.Add(CurLogItem);
                            CurLogItem = null;
                        }
                        else if (DateTime.TryParse(line, out ItemTime))
                        {   // Начинается новая запись
                            CurLogItem = new CLogItem()
                            {
                                CreationDate = ItemTime,
                                Exceptions = new List<CExceptionItem>()
                            };
                        }
                        else if (!string.IsNullOrWhiteSpace(line) && CurLogItem != null)
                        {   // Запись продолжается
                            if (line.StartsWith(GlobalDefines.LOG_EXCEPTION_START_LINE))
                            {   // Началось новое исключение
                                if (CurExeptionItem != null)
                                {
                                    CurLogItem.Exceptions.Add(CurExeptionItem);
                                    CurExeptionItem = null;
                                }

                                CurExeptionItem = new CExceptionItem()
                                {
                                    Index = CurLogItem.Exceptions.Count + 1
                                };
                            }
                            else if (CurExeptionItem != null)
                            {
                                CurExeptionItem.Text += line + "\n";
                            }
                        }
                    }
                    while (line != null);
                }
            }
            catch
            { }

            lstItems.Sort((lhs, rhs) => -lhs.CreationDate.CompareTo(rhs.CreationDate));
            Items.AddRange(lstItems);
        }


        public CLogWnd()
        {
            InitializeComponent();

            txtLogFilePath.Text = GlobalDefines.STD_ERROR_LOG_FILE_PATH;

            LastHeaderClicked = grdcolhdrCreationDate;
            LastSortDirection = ListSortDirection.Ascending;
            if (LastHeaderClicked.Column != null)
                LastHeaderClicked.Column.HeaderTemplate = DBManagerApp.m_AppSettings.m_Settings.m_GlobalResources["ListViewHeaderTemplateAsc"] as DataTemplate;

            RefreshItems();
        }


        #region Сортировка
        private void UpdateListBinding()
        {
            BindingBase bindBase = BindingOperations.GetBindingBase(lstvItems, ListView.ItemsSourceProperty);

            BindingOperations.ClearBinding(lstvItems, ListView.ItemsSourceProperty);
            lstvItems.SetBinding(ListView.ItemsSourceProperty, bindBase);
        }


        private void lstvItems_ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader HeaderClicked = e.OriginalSource as GridViewColumnHeader;
            if (HeaderClicked != null && HeaderClicked == grdcolhdrCreationDate)
            {
                ListSortDirection SortDir;
                if (HeaderClicked != LastHeaderClicked)
                    SortDir = ListSortDirection.Ascending;
                else
                    SortDir = LastSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

                string SortProperty = "";
                if (HeaderClicked == grdcolhdrCreationDate)
                    SortProperty = "CreationDate";
                Sort(SortProperty, SortDir);

                // Remove arrow from previously sorted header
                if (LastHeaderClicked != null && LastHeaderClicked != HeaderClicked)
                    LastHeaderClicked.Column.HeaderTemplate = null;

                HeaderClicked.Column.HeaderTemplate =
                        DBManagerApp.m_AppSettings.m_Settings.m_GlobalResources[SortDir == ListSortDirection.Ascending ? "ListViewHeaderTemplateAsc" : "ListViewHeaderTemplateDesc"] as DataTemplate;

                LastHeaderClicked = HeaderClicked;
                LastSortDirection = SortDir;

                UpdateListBinding();
            }
        }


        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lstvItems.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion


        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshItems();
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this,
                                Properties.Resources.resClearLogFileQuestion,
                                Title,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question,
                                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Items.Clear();

                try
                {
                    File.Delete(GlobalDefines.STD_ERROR_LOG_FILE_PATH);
                }
                catch
                { }
            }
        }

        private bool m_IsLoaded = false;

        private void CLogWnd_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!m_IsLoaded)
                return;

            if (e.HeightChanged)
            {
                double NewSize = lstvItems.Height + (e.NewSize.Height - e.PreviousSize.Height);
                if (NewSize >= lstvItems.MinHeight)
                    lstvItems.Height = NewSize;
            }

            if (e.WidthChanged)
            {
                double NewSize = lstvItems.Width + (e.NewSize.Width - e.PreviousSize.Width);
                if (NewSize >= lstvItems.MinWidth)
                    lstvItems.Width = NewSize;
            }
        }


        private void CLogWnd_Loaded(object sender, RoutedEventArgs e)
        {
            Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
            MinWidth = DesiredSize.Width;
            MinHeight = DesiredSize.Height;

            lstvItems.Width = lstvItems.MinWidth = GlobalDefines.GetActualControlWidth(grdItems);
            lstvItems.Height = lstvItems.MinHeight = GlobalDefines.GetActualControlHeight(grdItems);

            lstvItems.MaxHeight = lstvItems.MaxWidth = double.PositiveInfinity;

            (txtblkExceptionsHeader.Parent as Control).HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;

            if (m_Items.Count > 0)
                LastHeaderClicked.Column.HeaderTemplate = DBManagerApp.m_AppSettings.m_Settings.m_GlobalResources["ListViewHeaderTemplateAsc"] as DataTemplate;

            m_IsLoaded = true;
        }
    }
}
