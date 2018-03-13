using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class StyleChangedEventArgs : EventArgs
	{
		public IShowedClass Source { get; private set; }
		public string PropertyName { get; private set; }

		public StyleChangedEventArgs(IShowedClass source, string propertyName)
		{
			Source = source;
			PropertyName = propertyName;
		}

		public StyleChangedEventArgs(StyleChangedEventArgs rhs)
		{
			Source = rhs.Source;
			PropertyName = rhs.PropertyName;
		}
	}


	public delegate void StyleChangedEventHandler(object sender, StyleChangedEventArgs e);
}
