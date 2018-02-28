using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media;
using DBManager.Scanning.XMLDataClasses;
using System.Windows.Data;
using DBManager.Global.Converters;

namespace DBManager.RoundMembers.Converters
{
	public class IsMemberPassToNextRoundMiddleSheetsConverter : MarkupMultiConverterBase
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2 && parameter != null)
			{
				CResult FirstSum = values[0] is CResult ? values[0] as CResult : null;
				CResult SecondSum = values[1] is CResult ? values[1] as CResult : null;
				int RowNumber = (int)parameter;

				if (FirstSum == null || SecondSum == null || FirstSum.Time == null || SecondSum.Time == null)
					return false;
				else
				{
					lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
					{
						if (FirstSum.Time < SecondSum.Time)
							return RowNumber == 0;
						else
							return RowNumber != 0;
					}
				}
			}
			else
				return false;
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new Exception("IsMemberPassToNextRoundMiddleSheetsConverter.ConvertBack is not implemented");
		}


		public IsMemberPassToNextRoundMiddleSheetsConverter()
		{
		}
	}
}
