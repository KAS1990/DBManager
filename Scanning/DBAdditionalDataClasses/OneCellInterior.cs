﻿using DBManager.Global;
using DBManager.RoundMembers.Converters;
using DBManager.Scanning.XMLDataClasses;
using DBManager.SettingsWriter;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
    public class OneCellInterior : INotifyPropertyChanged
    {
        private readonly enCellType m_CellType = enCellType.None;

        #region Background
        private static readonly string BackgroundPropertyName = GlobalDefines.GetPropertyName<OneCellInterior>(m => m.Background);

        private Brush m_Background = Brushes.Transparent;

        public Brush Background
        {
            get { return m_Background; }
            set
            {
                if (m_Background != value)
                {
                    m_Background = value;
                    OnPropertyChanged(BackgroundPropertyName);
                }
            }
        }
        #endregion


        #region Foreground
        private static readonly string ForegroundPropertyName = GlobalDefines.GetPropertyName<OneCellInterior>(m => m.Foreground);

        private Brush m_Foreground = Brushes.Black;

        public Brush Foreground
        {
            get { return m_Foreground; }
            set
            {
                if (m_Foreground != value)
                {
                    m_Foreground = value;
                    OnPropertyChanged(ForegroundPropertyName);
                }
            }
        }
        #endregion


        #region FontWeight
        private static readonly string FontWeightPropertyName = GlobalDefines.GetPropertyName<OneCellInterior>(m => m.FontWeight);

        private FontWeight m_FontWeight = FontWeights.Normal;

        public FontWeight FontWeight
        {
            get { return m_FontWeight; }
            set
            {
                if (m_FontWeight != value)
                {
                    m_FontWeight = value;
                    OnPropertyChanged(FontWeightPropertyName);
                }
            }
        }
        #endregion


        #region FontStyle
        private static readonly string FontStylePropertyName = GlobalDefines.GetPropertyName<OneCellInterior>(m => m.FontStyle);

        private System.Windows.FontStyle m_FontStyle = FontStyles.Normal;

        public System.Windows.FontStyle FontStyle
        {
            get { return m_FontStyle; }
            set
            {
                if (m_FontStyle != value)
                {
                    m_FontStyle = value;
                    OnPropertyChanged(FontStylePropertyName);
                }
            }
        }
        #endregion


        public void RefreshFields(CMemberAndResults Member,
                                    COneRoundResults RouteResults,
                                    CResult RouteResult,
                                    CFontStyleSettings RowFontStyle,
                                    out bool PlainStyleSetted)
        {
            MemberInteriorConverter.CConverterResult ConverterResult = new MemberInteriorConverter.CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
            PlainStyleSetted = true;
            switch (m_CellType)
            {
                case enCellType.Route1:
                case enCellType.Route2:
                case enCellType.Sum:
                    switch (RouteResults.m_Round)
                    {
                        case enRounds.Qualif:
                        case enRounds.Qualif2:
                            ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                                RouteResult,
                                                                                RouteResults.m_Round,
                                                                                DBManagerApp.MainWnd.MembersFromQualif,
                                                                                m_CellType,
                                                                                out PlainStyleSetted);
                            break;

                        case enRounds.OneEighthFinal:
                        case enRounds.QuaterFinal:
                        case enRounds.SemiFinal:
                        case enRounds.Final:
                            ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                                RouteResult,
                                                                                RouteResults.m_Round,
                                                                                null,
                                                                                m_CellType,
                                                                                out PlainStyleSetted);
                            break;
                    }
                    break;

                case enCellType.StartNumber:
                    if (RouteResults.m_Round == enRounds.Qualif || RouteResults.m_Round == enRounds.Qualif2)
                    {
                        ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                            RouteResult,
                                                                            RouteResults.m_Round,
                                                                            DBManagerApp.MainWnd.MembersFromQualif,
                                                                            m_CellType,
                                                                            out PlainStyleSetted);
                    }
                    else
                    {
                        ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                            RouteResult,
                                                                            RouteResults.m_Round,
                                                                            null,
                                                                            m_CellType,
                                                                            out PlainStyleSetted);
                    }
                    break;

                case enCellType.SurnameAndName:
                    if (RouteResults.m_Round == enRounds.Qualif || RouteResults.m_Round == enRounds.Qualif2)
                    {
                        ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                            RouteResult,
                                                                            RouteResults.m_Round,
                                                                            DBManagerApp.MainWnd.MembersFromQualif,
                                                                            m_CellType,
                                                                            out PlainStyleSetted);
                    }
                    else
                    {
                        ConverterResult = MemberInteriorConverter.Convert(Member,
                                                                            RouteResult,
                                                                            RouteResults.m_Round,
                                                                            null,
                                                                            m_CellType,
                                                                            out PlainStyleSetted);
                    }
                    break;

                default:
                    break;
            }

            if (PlainStyleSetted)
            {
                Background = new SolidColorBrush(RowFontStyle.BackgroundColor);
                Foreground = new SolidColorBrush(RowFontStyle.ForeColor);
                FontWeight = RowFontStyle.FontWeight;
                FontStyle = RowFontStyle.FontStyle;
            }
            else
            {
                Background = ConverterResult.Background;
                Foreground = ConverterResult.Foreground;
                FontWeight = ConverterResult.FontWeight;
                FontStyle = ConverterResult.FontStyle;
            }
        }


        public OneCellInterior(enCellType CellType)
        {
            m_CellType = CellType;
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
