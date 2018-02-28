using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using DBManager.Global.Converters;
using System.Windows;
using System.Windows.Media;

namespace DBManager.RoundMembers.Converters
{
	public class IsMemberPassToNextRoundQualifConverter : MarkupMultiConverterBase
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2)
			{
				int RoundPlace = values[0] is int ? (int)values[0] : 0;
				int MembersFromQualif = values[1] is int ? (int)values[1] : 0;

				return RoundPlace > 0 && RoundPlace <= MembersFromQualif;
			}
			else
				return false;
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new Exception("IsMemberPassToNextRoundQualifConverter.ConvertBack is not implemented");
		}


		public IsMemberPassToNextRoundQualifConverter()
		{
		}
	}
}
