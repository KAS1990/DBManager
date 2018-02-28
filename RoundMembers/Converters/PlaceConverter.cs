using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;
using System.Windows.Data;
using DBManager.Global;

namespace DBManager.RoundMembers.Converters
{
	public class PlaceMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return GlobalDefines.PlaceToString((int?)value);
		}


		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in PlaceMarkupConverter");
		}


		public PlaceMarkupConverter() :
			base()
		{
		}
	}


	public class PlaceMarkupMultiConverter : MarkupMultiConverterBase
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string result = "";
			
			switch (values.Length)
			{
				case 1:
					result = GlobalDefines.PlaceToString((int?)values[0]);
					break;

				case 2:
					result = GlobalDefines.PlaceToString((int?)values[0]);
					if (!string.IsNullOrEmpty(result))
					{
						string PlaceInFilter = GlobalDefines.PlaceToString((int?)values[1]);
						if (!string.IsNullOrEmpty(PlaceInFilter))
							result += " (" + PlaceInFilter + ")";
					}
					break;
			}

			return result;
		}


		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in PlaceMarkupMultiConverter");
		}


		public PlaceMarkupMultiConverter() :
			base()
		{
		}
	}
}
