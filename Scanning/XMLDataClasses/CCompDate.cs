using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;
using DBManager.Global;
using System.Reflection;

namespace DBManager.Scanning.XMLDataClasses
{
	/// <summary>
	/// Информация о дате проведения соревнований
	/// </summary>
	[Serializable]
	public class CCompDate : CXMLSerializerBase
	{
		#region Сериализуемые свойства
		int m_Day = GlobalDefines.DEFAULT_XML_INT_VAL;
		/// <summary>
		/// 1 - 31
		/// </summary>
		[XmlAttribute("DD")]
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int Day
		{
			get { return m_Day; } 
			set
			{
				if (m_Day != value)
				{
					m_Day = value;
					SetDate();
				}
			}
		}

		int m_Month = GlobalDefines.DEFAULT_XML_INT_VAL;
		/// <summary>
		/// 0 - 11
		/// </summary>
		[XmlAttribute("MMMM")]
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int Month
		{
			get { return m_Month; }
			set
			{
				if (m_Month != value)
				{
					m_Month = value;
					SetDate();
				}
			}
		}

		int m_Year = GlobalDefines.DEFAULT_XML_INT_VAL;
		[XmlAttribute("YYYY")]
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int Year
		{
			get { return m_Year; }
			set
			{
				if (m_Year != value)
				{
					m_Year = value;
					SetDate();
				}
			}
		}
		#endregion


		#region Не сериализуемые свойства
		DateTime m_Date = GlobalDefines.DEFAULT_XML_DATE_TIME_VAL;
		/// <summary>
		/// Дата, полученная после чтения xml-файла.
		/// Если дата в файле неверная, то поле будет иметь значение GlobalDefines.DEFAULT_XML_DATE_TIME_VAL.
		/// </summary>
		[XmlIgnore]
		public DateTime Date
		{
			get { return m_Date; }
			private set
			{
				if (m_Date != value)
				{
					m_Date = value;
					OnPropertyChanged("Date");
				}
			}
		}
		#endregion

		public CCompDate()
		{
			CXMLSerializerBase.SetDefaultValsForAllProps(this);
		}


		/// <summary>
		/// Конструктор копирования
		/// </summary>
		/// <param name="rhs"></param>
		public CCompDate(CCompDate rhs) :
			base(rhs)
		{
		}


		void SetDate()
		{
			if (Day != GlobalDefines.DEFAULT_XML_INT_VAL &&
				Month != GlobalDefines.DEFAULT_XML_INT_VAL &&
				Year != GlobalDefines.DEFAULT_XML_INT_VAL)
			{
				try
				{
					Date = new DateTime(Year, Month + 1, Day);
				}
				catch
				{
					Date = GlobalDefines.DEFAULT_XML_DATE_TIME_VAL;
				}
			}
		}


		public override bool Equals(object o)
		{
			if (o is CCompDate)
				return this == (o as CCompDate);
			
			return false;
		}


		public override int GetHashCode()
		{
			return Date.GetHashCode();
		}


		public static bool operator ==(CCompDate lhs, CCompDate rhs)
		{
			return ((object)lhs == null && (object)rhs == null) ||
					((object)lhs != null && (object)rhs != null && lhs.Date == rhs.Date);
		}


		public static bool operator !=(CCompDate lhs, CCompDate rhs)
		{
			return !(lhs == rhs);
		}


		public static implicit operator DateTime(CCompDate rhs)
		{
			try
			{
				return rhs.Date;
			}
			catch
			{	// Неверная дата
				return GlobalDefines.DEFAULT_XML_DATE_TIME_VAL;
			}
		}
	}
}
