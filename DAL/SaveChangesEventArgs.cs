using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DBManager
{
	public class OnePropertyChanging
	{
		public object OldValue { get; private set; }
		public object NewValue { get; private set; }

		public OnePropertyChanging(object oldValue, object newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}

	public class OneEntityChanging
	{
		public object Entity { get; private set; }
		public Dictionary<string, OnePropertyChanging> PropertiesHasBeenChanged { get; private set; }

		public OneEntityChanging(object entity)
		{
			Entity = entity;
			PropertiesHasBeenChanged = new Dictionary<string, OnePropertyChanging>();
		}
	}

	public class SaveChangesEventArgs
	{
		public ReadOnlyCollection<OneEntityChanging> Changes { get; private set; }

		public SaveChangesEventArgs(List<OneEntityChanging> changes)
		{
			Changes = new ReadOnlyCollection<OneEntityChanging>(changes);
		}
	}


	public delegate void SaveChangesEventHandler(object sender, SaveChangesEventArgs e);
}
