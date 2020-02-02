using DBManager.Global;
using DBManager.Global.Converters;
using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Globalization;
using System.Windows.Media;

namespace DBManager.RoundMembers.Converters
{
    public class HighlightResultGradesConverter : MarkupMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is CMemberInTotal) || !(values[1] is enHighlightGradesType) || values[0] == null)
                return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.BackgroundColor);
            CMemberInTotal Member = values[0] as CMemberInTotal;
            enHighlightGradesType HighlightType = (enHighlightGradesType)values[1];

            if (Member.Place == null ||
                Member.Place <= 0 ||
                Member.TotalGrade == null ||
                Member.TotalGrade <= 0 ||
                HighlightType == enHighlightGradesType.None)
            {
                return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.BackgroundColor);
            }
            else
            {
                switch (HighlightType)
                {
                    case enHighlightGradesType.ResultGrades:
                        // Участник выполнил какой-то разряд
                        if (Member.TotalGrade.Value < (byte)enGrade.Adult3)
                        {   // 3 юн, 2 юн, 1 юн
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle.BackgroundColor);
                        }
                        else
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle.BackgroundColor);

                    case enHighlightGradesType.СonfirmGrades:
                        if (Member.MemberInfo.InitGrade != null && Member.TotalGrade.Value == Member.MemberInfo.InitGrade)
                        {   // Участник подтвердил разряд
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle.BackgroundColor);
                        }
                        else
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.BackgroundColor);

                    case enHighlightGradesType.CarryoutGrades:
                        if (Member.MemberInfo.InitGrade != null && Member.TotalGrade.Value > Member.MemberInfo.InitGrade)
                        {   // Участник выполнил новый разряд
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle.BackgroundColor);
                        }
                        else
                            return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.BackgroundColor);

                    default:
                        return new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.BackgroundColor);
                }
            }
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotFiniteNumberException("ConvertBack is not implemented in HighlightResultGradesConverter");
        }


        public HighlightResultGradesConverter() :
            base()
        {
        }
    }
}
