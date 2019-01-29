using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DBManager.Excel.GeneratingWorkbooks.Converters
{
    [ValueConversion(typeof(enSecondColNameType), typeof(bool))]
    public class SecondColNameType2NullBoolConverter : IValueConverter
    {
        /// <summary>
        /// Это значение конвертируется в true и обратно
        /// </summary>
        public enSecondColNameType TrueValue { get; set; } = enSecondColNameType.None;

        /// <summary>
        /// Это значение конвертируется в false и обратно
        /// </summary>
        public enSecondColNameType FalseValue { get; set; } = enSecondColNameType.None;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is enSecondColNameType) || (value is null))
                return null;

            var secondColNameType = (enSecondColNameType)value;

            if (secondColNameType == TrueValue)
                return (bool?)true;

            if (secondColNameType == FalseValue)
                return (bool?)false;

            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool?))
                return null;

            var val = (bool?)value;
            if (val.HasValue)
                return val.Value ? TrueValue : FalseValue;
            else
                return enSecondColNameType.None;
        }
    }
}
