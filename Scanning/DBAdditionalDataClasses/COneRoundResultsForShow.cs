using DBManager.Global;
using DBManager.SettingsWriter;
using System.ComponentModel;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
    public class COneRoundResultsForShow : INotifyPropertyChanged
    {
        private CMemberAndResults m_Parent = null;
        private CFontStyleSettings m_RowFontStyle = null;

        #region Route1
        private static readonly string Route1PropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Route1);

        private OneCellInterior m_Route1 = new OneCellInterior(enCellType.Route1);

        public OneCellInterior Route1
        {
            get { return m_Route1; }
            set
            {
                if (m_Route1 != value)
                {
                    m_Route1 = value;
                    OnPropertyChanged(Route1PropertyName);
                }
            }
        }
        #endregion


        #region Route2
        private static readonly string Route2PropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Route2);

        private OneCellInterior m_Route2 = new OneCellInterior(enCellType.Route2);

        public OneCellInterior Route2
        {
            get { return m_Route2; }
            set
            {
                if (m_Route2 != value)
                {
                    m_Route2 = value;
                    OnPropertyChanged(Route2PropertyName);
                }
            }
        }
        #endregion


        #region Sum
        private static readonly string SumPropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.Sum);

        private OneCellInterior m_Sum = new OneCellInterior(enCellType.Sum);

        public OneCellInterior Sum
        {
            get { return m_Sum; }
            set
            {
                if (m_Sum != value)
                {
                    m_Sum = value;
                    OnPropertyChanged(SumPropertyName);
                }
            }
        }
        #endregion


        #region StartNumber
        private static readonly string StartNumberPropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.StartNumber);

        private OneCellInterior m_StartNumber = new OneCellInterior(enCellType.StartNumber);

        public OneCellInterior StartNumber
        {
            get { return m_StartNumber; }
            set
            {
                if (m_StartNumber != value)
                {
                    m_StartNumber = value;
                    OnPropertyChanged(StartNumberPropertyName);
                }
            }
        }
        #endregion


        #region SurnameAndName
        private static readonly string SurnameAndNamePropertyName = GlobalDefines.GetPropertyName<COneRoundResultsForShow>(m => m.SurnameAndName);

        private OneCellInterior m_SurnameAndName = new OneCellInterior(enCellType.SurnameAndName);

        public OneCellInterior SurnameAndName
        {
            get { return m_SurnameAndName; }
            set
            {
                if (m_SurnameAndName != value)
                {
                    m_SurnameAndName = value;
                    OnPropertyChanged(SurnameAndNamePropertyName);
                }
            }
        }
        #endregion


        public void RefreshFields(COneRoundResults RouteResults, CMemberAndResults Parent = null, CFontStyleSettings RowFontStyle = null)
        {
            if (m_Parent == null || Parent != null)
                m_Parent = Parent;
            else
                Parent = m_Parent;

            if (m_RowFontStyle == null || RowFontStyle != null)
                m_RowFontStyle = RowFontStyle;
            else
                RowFontStyle = m_RowFontStyle;

            bool PlainStyleSetted = false;

            Route1.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle, out PlainStyleSetted);
            Route2.RefreshFields(Parent, RouteResults, RouteResults.Route2, RowFontStyle, out PlainStyleSetted);
            Sum.RefreshFields(Parent, RouteResults, RouteResults.Sum, RowFontStyle, out PlainStyleSetted);

            StartNumber.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle, out PlainStyleSetted);

            SurnameAndName.RefreshFields(Parent, RouteResults, RouteResults.Route1, RowFontStyle, out PlainStyleSetted);

            switch (RouteResults.m_Round)
            {
                #region Qualif, Qualif2
                case enRounds.Qualif:
                case enRounds.Qualif2:
                    if (RouteResults.Sum != null && RouteResults.Sum.CondFormating.HasValue)
                    {
                        switch (RouteResults.Sum.CondFormating.Value)
                        {
                            case enCondFormating.StayOnStart: // Находится на старте
                            case enCondFormating.Preparing: // Участник готовится
                                break;

                            case enCondFormating.JustRecievedResult: // Только что полученный результат
                                StartNumber.RefreshFields(Parent, RouteResults, RouteResults.Sum, RowFontStyle, out PlainStyleSetted);
                                SurnameAndName.RefreshFields(Parent, RouteResults, RouteResults.Sum, RowFontStyle, out PlainStyleSetted);
                                break;

                            default:
                                break;
                        }
                    }
                    break;
                #endregion

                #region OneEighthFinal, QuaterFinal, SemiFinal, Final
                case enRounds.OneEighthFinal:
                case enRounds.QuaterFinal:
                case enRounds.SemiFinal:
                case enRounds.Final:
                    break;
                    #endregion
            }
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
