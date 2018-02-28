using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;
using DBManager.Global;
using System.Windows;

namespace DBManager.RightPanels
{
	public class GradesVisibilityConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is enHighlightGradesType && parameter != null && parameter is string)
			{
				switch (parameter as string)
				{
					case "0":
						return (enHighlightGradesType)value == enHighlightGradesType.ResultGrades ? Visibility.Visible : Visibility.Collapsed;

					case "1":
						return (enHighlightGradesType)value == enHighlightGradesType.СonfirmGrades ||
								(enHighlightGradesType)value == enHighlightGradesType.CarryoutGrades ? Visibility.Visible : Visibility.Collapsed;

					default:
						return Visibility.Collapsed;
				}
			}
			else
				return Visibility.Collapsed;
		}


		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in GradesVisibilityConverter");
		}


		public GradesVisibilityConverter() :
			base()
		{
		}
	}
}
