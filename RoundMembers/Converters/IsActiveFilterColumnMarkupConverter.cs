using DBManager.Global;
using DBManager.Global.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DBManager.RoundMembers.Converters
{
    internal class IsActiveFilterColumnMarkupConverter : MarkupMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is enFilterTarget) || !(values[1] is List<enFilterTarget>))
                return false;

            return (values[1] as List<enFilterTarget>).Contains((enFilterTarget)values[0]);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("IsActiveFilterColumnMarkupConverter.ConvertBack is not implemented");
        }


        public IsActiveFilterColumnMarkupConverter() :
            base()
        {
        }
    }
}
