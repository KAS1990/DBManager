using DBManager.Global;
using DBManager.Global.Converters;
using System;
using System.Globalization;

namespace DBManager.RoundMembers.Converters
{
    /// <summary>
    /// Преобразует номер разряда в его название
    /// </summary>
    public class GradeMarkupConverter : MarkupConverterBase
    {
        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string GradeInEnum;

            if (value != null && GlobalDefines.GRADE_NAMES.TryGetValue((enGrade)(System.Convert.ToInt32(value)), out GradeInEnum))
                return GradeInEnum;
            else
                return "";
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotFiniteNumberException("ConvertBack is not implemented in GradeMarkupConverter");
        }


        public GradeMarkupConverter() :
            base()
        {
        }
    }
}
