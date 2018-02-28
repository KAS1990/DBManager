using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CRoundAndDate : CDBAdditionalClassBase
	{
		#region Date
		private string m_Date = GlobalDefines.DEFAULT_XML_STRING_VAL;

		public string Date
		{
			get { return m_Date; }
			set
			{
				if (m_Date != value)
				{
					m_Date = value;
					OnPropertyChanged("Date");
				}
			}
		}
		#endregion
		
		#region Name
		private string m_Name = GlobalDefines.DEFAULT_XML_STRING_VAL;

		public string Name
		{
			get { return m_Name; }
			set
			{
				if (m_Name != value)
				{
					m_Name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		#endregion
				
	}
}
