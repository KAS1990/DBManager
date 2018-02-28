using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Global.Converters;
using System.Globalization;
using DBManager.Scanning.DBAdditionalDataClasses;

namespace DBManager.RoundMembers.Converters
{
	public class HasResultMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			COneRoundResults result = value as COneRoundResults;
			
			if (result == null)
				return false;
			switch ((enResultColumnNumber)parameter)
			{
				case enResultColumnNumber.Route1:
					return result.Route1 != null && result.Route1.ResultPossible;

				case enResultColumnNumber.Route2:
					return result.Route2 != null && result.Route2.ResultPossible;

				case enResultColumnNumber.Sum:
					return result.Sum != null && result.Sum.ResultPossible;
			}
			return false;
		}


		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in HasResultMarkupConverter");
		}


		public HasResultMarkupConverter() :
			base()
		{
		}
	}
}
