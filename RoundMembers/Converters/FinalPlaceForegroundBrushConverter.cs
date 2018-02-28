using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DBManager.Global.Converters;

namespace DBManager.RoundMembers.Converters
{
	public class FinalPlaceForegroundBrushMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return value == null || (int)value == 4 ?
					new SolidColorBrush(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle.ForeColor) :
					new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0x00));
		}


		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in FinalPlaceForegroundBrushMarkupConverter");
		}


		public FinalPlaceForegroundBrushMarkupConverter()
		{
		}
	}
}
