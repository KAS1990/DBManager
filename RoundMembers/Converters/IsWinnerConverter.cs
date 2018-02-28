using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;

namespace DBManager.RoundMembers.Converters
{
	public class IsWinnerConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return value != null && (int)value < 4 && (int)value > 0;
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in IsWinnerConverter");
		}


		public IsWinnerConverter() :
			base()
		{
		}
	}
}
