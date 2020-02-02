using DBManager.Global;
using System.ComponentModel;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
    public abstract class CDBAdditionalClassBase : INotifyPropertyChanged, IShowedClass, ICanRefreshFrom
    {
        #region Place
        public static readonly string PlacePropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.Place);

        private int? m_Place = null;

        public int? Place
        {
            get { return m_Place; }
            set
            {
                if (m_Place != value)
                {
                    m_Place = value;
                    PlaceForShow = GlobalDefines.PlaceToString(m_Place);
                    IsWinnerForShow = m_Place < 4 && m_Place > 0;
                    OnPropertyChanged(PlacePropertyName);
                }
            }
        }
        #endregion


        #region PlaceInFilter
        private static readonly string PlaceInFilterPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceInFilter);

        private int? m_PlaceInFilter = null;

        public int? PlaceInFilter
        {
            get { return m_PlaceInFilter; }
            set
            {
                if (m_PlaceInFilter != value)
                {
                    m_PlaceInFilter = value;
                    PlaceInFilterForShow = GlobalDefines.PlaceToString(m_PlaceInFilter);
                    OnPropertyChanged(PlaceInFilterPropertyName);
                }
            }
        }
        #endregion


        #region Вместо конвертеров
        #region PlaceForShow
        private static readonly string PlaceForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceForShow);

        private string m_PlaceForShow = "";

        public string PlaceForShow
        {
            get { return m_PlaceForShow; }
            set
            {
                if (m_PlaceForShow != value)
                {
                    m_PlaceForShow = value;
                    OnPropertyChanged(PlaceForShowPropertyName);
                }
            }
        }
        #endregion


        #region PlaceInFilterForShow
        private static readonly string PlaceInFilterForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.PlaceInFilterForShow);

        private string m_PlaceInFilterForShow = "";

        public string PlaceInFilterForShow
        {
            get { return m_PlaceInFilterForShow; }
            set
            {
                if (m_PlaceInFilterForShow != value)
                {
                    m_PlaceInFilterForShow = value;
                    OnPropertyChanged(PlaceInFilterForShowPropertyName);
                }
            }
        }
        #endregion


        #region IsWinnerForShow
        private static readonly string IsWinnerForShowPropertyName = GlobalDefines.GetPropertyName<CDBAdditionalClassBase>(m => m.IsWinnerForShow);

        private bool m_IsWinnerForShow = false;

        public bool IsWinnerForShow
        {
            get { return m_IsWinnerForShow; }
            set
            {
                if (m_IsWinnerForShow != value)
                {
                    m_IsWinnerForShow = value;
                    OnPropertyChanged(IsWinnerForShowPropertyName);
                }
            }
        }
        #endregion
        #endregion


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion


        #region OnStyleChanged and StyleChanged event
        public event StyleChangedEventHandler StyleChanged;


        void IShowedClass.OnStyleChanged(IShowedClass source, string propertyName)
        {
            OnStyleChanged(source, propertyName);
        }


        protected void OnStyleChanged(IShowedClass source, string propertyName)
        {
            StyleChanged?.Invoke(this, new StyleChangedEventArgs(source, propertyName));
        }


        void IShowedClass.OnStyleChanged(StyleChangedEventArgs e)
        {
            OnStyleChanged(e);
        }


        protected void OnStyleChanged(StyleChangedEventArgs e)
        {
            StyleChanged?.Invoke(this, new StyleChangedEventArgs(e));
        }
        #endregion


        public virtual void RefreshFrom(ICanRefreshFrom rhs,
                                        bool SkipNullsForObjects,
                                        bool SkipNullsForNullables)
        {
            CDBAdditionalClassBase rhsDBAdditionalClassBase = rhs as CDBAdditionalClassBase;

            if (rhsDBAdditionalClassBase == null)
                return;

            if (!SkipNullsForNullables || rhsDBAdditionalClassBase.Place.HasValue)
                Place = rhsDBAdditionalClassBase.Place;

            if (!SkipNullsForNullables || rhsDBAdditionalClassBase.PlaceInFilter.HasValue)
                PlaceInFilter = rhsDBAdditionalClassBase.PlaceInFilter;
        }
    }
}
