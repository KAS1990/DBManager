﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;
using DBManager.Global;

namespace DBManager.RightPanels
{
	public class GradesContentConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is enHighlightGradesType)
			{
				switch ((enHighlightGradesType)value)
				{
					case enHighlightGradesType.CarryoutGrades:
						return Properties.Resources.resMembersCarryiedoutGrades;

					case enHighlightGradesType.СonfirmGrades:
						return Properties.Resources.resMembersСonfirmedGrades;

					default:
						return null;
				}
			}
			else
				return null;
		}


		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotFiniteNumberException("ConvertBack is not implemented in GradesContentConverter");
		}


		public GradesContentConverter() :
			base()
		{
		}
	}
}
