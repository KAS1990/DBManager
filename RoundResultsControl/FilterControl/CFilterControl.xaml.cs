using DBManager.Commands;
using DBManager.Global;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DBManager.RoundResultsControl.FilterControl
{
    /// <summary>
    /// Interaction logic for CFilterControl.xaml
    /// </summary>
    public partial class CFilterControl : CNotifyPropertyChangedUserCtrl
    {
        /// <summary>
        /// Причина закрытия окна со списком предикатов
        /// </summary>
        public enum enCloseReason
        {
            OK,
            Cancel,
            LostFocus
        }

        public Popup ParentPopup { get; private set; }

        public enFilterTarget FilterTarget { get; private set; }

        public enCloseReason CloseReason { get; private set; }

        private readonly List<FilterPredicate> m_FilterPredicatesOnOpen;

        /// <summary>
        /// Были ли изменения с момента открытия компонента
        /// </summary>
        public bool PredicatesChanged
        {
            get
            {
                for (int i = 0; i < m_FilterPredicatesOnOpen.Count; i++)
                {
                    if (m_FilterPredicatesOnOpen[i].IsSelected != FilterPredicates[i].IsSelected)
                        return true;
                }

                return false;
            }
        }


        #region FilterCathegories
        private readonly ObservableCollection<FilterPredicate> m_FilterPredicates = new ObservableCollection<FilterPredicate>();
        /// <summary>
        /// Словарь, содержащий все группы
        /// </summary>
        public ObservableCollection<FilterPredicate> FilterPredicates
        {
            get { return m_FilterPredicates; }
        }
        #endregion


        #region FilterCommand
        private static readonly string FilterCommandPropertyName = GlobalDefines.GetPropertyName<CFilterControl>(m => m.FilterCommand);

        private CCommand m_FilterCommand = null;
        /// <summary>
        /// Нажали ОК
        /// </summary>
        public CCommand FilterCommand
        {
            get { return m_FilterCommand; }
            set
            {
                if (m_FilterCommand != value)
                {
                    m_FilterCommand = value;
                    OnPropertyChanged(FilterCommandPropertyName);
                }
            }
        }
        #endregion


        #region CancelCommand
        private static readonly string CancelCommandPropertyName = GlobalDefines.GetPropertyName<CFilterControl>(m => m.CancelCommand);

        private CCommand m_CancelCommand = null;
        /// <summary>
        /// Нажали отмену
        /// </summary>
        public CCommand CancelCommand
        {
            get { return m_CancelCommand; }
            set
            {
                if (m_CancelCommand != value)
                {
                    m_CancelCommand = value;
                    OnPropertyChanged(CancelCommandPropertyName);
                }
            }
        }
        #endregion


        public CFilterControl()
        {
            InitializeComponent();
        }


        public CFilterControl(Popup pppParent,
                                enFilterTarget filterTarget,
                                List<FilterPredicate> predicates,
                                FilterControlCommandHandler FilterCommandFunc,
                                FilterControlCommandHandler CancelCommandFunc)
        {
            InitializeComponent();

            ParentPopup = pppParent;
            FilterTarget = filterTarget;
            CloseReason = enCloseReason.LostFocus;

            m_FilterPredicatesOnOpen = new List<FilterPredicate>();

            foreach (FilterPredicate predicate in predicates)
            {
                FilterPredicate NewPredicate = new FilterPredicate(predicate);
                NewPredicate.PropertyChanged += Predicate_PropertyChanged;
                FilterPredicates.Add(NewPredicate);

                m_FilterPredicatesOnOpen.Add(new FilterPredicate(predicate));
            }

            CancelCommand = new CCommand(() =>
            {
                CloseReason = enCloseReason.Cancel;
                CancelCommandFunc(this);
            });
            FilterCommand = new CCommand(() =>
            {
                CloseReason = enCloseReason.OK;
                FilterCommandFunc(this);
            });
            Predicate_PropertyChanged(this, null);
        }


        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked.HasValue)
            {
                foreach (FilterPredicate cathegory in FilterPredicates)
                {
                    cathegory.PropertyChanged -= Predicate_PropertyChanged;
                    cathegory.IsSelected = chkSelectAll.IsChecked.Value;
                    cathegory.PropertyChanged += Predicate_PropertyChanged;
                }

                FilterCommand.CanExecute = FilterPredicates.Count > 0 && chkSelectAll.IsChecked.Value;
            }
        }


        private void Predicate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FilterPredicates.All(arg => arg.IsSelected))
            {   // Все элементы выбраны
                chkSelectAll.IsChecked = true;
            }
            else if (FilterPredicates.All(arg => !arg.IsSelected))
            {   // Все элементы не выбраны
                chkSelectAll.IsChecked = false;
            }
            else
            {   // Что-то выбрано, а что-то нет
                chkSelectAll.IsChecked = null;
            }

            FilterCommand.CanExecute = FilterPredicates.Count > 0 && (chkSelectAll.IsChecked == null || chkSelectAll.IsChecked.Value);
        }


        private void CFilterControl_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach (FilterPredicate predicate in FilterPredicates)
                predicate.PropertyChanged -= Predicate_PropertyChanged;
        }
    }


    /// <summary>
    /// Функция, которая вызывается при срабатывании команды нажатия на кнопки.
    /// param name="CommandType" - тип команды; имеет тип enCommandType
    /// </summary>
    public delegate void FilterControlCommandHandler(CFilterControl sender);
}
