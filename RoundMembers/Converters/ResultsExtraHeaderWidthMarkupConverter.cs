﻿using DBManager.Global.Converters;
using System;
using System.Globalization;
using System.Linq;

namespace DBManager.RoundMembers.Converters
{
    public class ResultsExtraHeaderWidthMarkupConverter : MarkupMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double TotalWidth = 0;
            foreach (double width in values.OfType<double>())
                TotalWidth += width;

            return TotalWidth;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("ResultsExtraHeaderWidthMarkupConverter.ConvertBack is not implemented");
        }


        public ResultsExtraHeaderWidthMarkupConverter()
        {
        }
    }
}
