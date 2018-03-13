using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Scanning
{
	public interface IShowedClass
	{
		#region OnStyleChanged and OnStyleChanged event
		event StyleChangedEventHandler StyleChanged;


		void OnStyleChanged(IShowedClass source, string propertyName);

		void OnStyleChanged(StyleChangedEventArgs e);
		#endregion
	}
}
