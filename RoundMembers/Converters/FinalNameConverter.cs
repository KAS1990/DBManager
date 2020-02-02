using DBManager.Global.Converters;
using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Globalization;

namespace DBManager.RoundMembers.Converters
{
    public class FinalNameConverter : MarkupConverterBase
    {
        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            CMembersPair Pair = value as CMembersPair;
            if (Pair == null || Pair.PairNumber == 0)
                return "";
            else
            {
                // Первая пара в финале борется за 3-4 места
                if (Pair.PairNumber == 1)
                    return string.Format(Properties.Resources.resfmtFinalName, 3, 4);
                else
                    return string.Format(Properties.Resources.resfmtFinalName, 1, 2);
            }
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotFiniteNumberException("ConvertBack is not implemented in FinalNameConverter");
        }


        public FinalNameConverter() :
            base()
        {
        }
    }
}
