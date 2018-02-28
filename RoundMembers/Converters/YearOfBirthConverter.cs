using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;
using System.Windows.Data;

namespace DBManager.RoundMembers.Converters
{
	public class YearOfBirthMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return value == null || (short)value == 0 ? "" : value.ToString();
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in YearOfBirthMarkupConverter");
		}

		public YearOfBirthMarkupConverter() :
			base()
		{
		}
	}
}
