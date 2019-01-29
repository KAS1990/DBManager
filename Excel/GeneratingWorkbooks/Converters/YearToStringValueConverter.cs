using DBManager.Excel.GeneratingWorkbooks.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class YearToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int? || value is int) || value == null)
                return null;

            if (value is int? && (value as int?) == null)
                return null;

            return value is int? ? (value as int?).CreateYearInString() : ((int?)value).CreateYearInString();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is int? || value is int) && (targetType == typeof(int?) || targetType == typeof(int)))
                return value;

            if (!(value is string) || (targetType != typeof(int) && targetType != typeof(int?)) || value == null)
                return null;

            int res = 0;
            if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndElder;
            else if (value.ToString() == Properties.Resources.resAndElder)
                res = (int)enEndYearSpecVals.AndYounger;
            else if (!int.TryParse(value.ToString(), out res))
                return null;

            if (targetType == typeof(int))
                return res;
            else
                return (int?)res;
        }
    }
}
