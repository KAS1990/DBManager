using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using DBManager.Global;

namespace DBManager.Scanning.XMLDataClasses
{
	/// <summary>
	/// Информация о возрастной группе
	/// </summary>
	[Serializable]
	public class CAgeGroup : CXMLSerializerBase
	{
		/// <summary>
		///Специальные значения для конечного года группы
		/// </summary>
		public enum enEndYearSpecVals
		{
			/// <summary>
			/// и моложе
			/// </summary>
			AndYounger = -2,
			/// <summary>
			/// и старше
			/// </summary>
			AndElder = -3,
		}


		public enum enGroupType
		{
			None = 0,
			/// <summary>
			/// Мужчины всех возрастов
			/// </summary>
			Men = 1,
			/// <summary>
			/// Женщины всех возрастов
			/// </summary>
			Women = 2,
			/// <summary>
			/// Дети, поэтому указаны года рождения
			/// </summary>
			Children = 3
		}


		#region Сериализуемые свойства
		string m_Name = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Name
		{
			get { return m_Name; }
			set
			{
				if (m_Name != value)
				{
					m_Name = value;
					if (string.IsNullOrWhiteSpace(m_Name))
						GroupType = enGroupType.None;
					else
					{
						m_Name = m_Name.Trim();
						switch (m_Name.ToLower())
						{
							case GlobalDefines.DEFAULT_XML_STRING_VAL:
								GroupType = enGroupType.None;
								break;

							case "мужской":
								GroupType = enGroupType.Men;
								break;

							case "женский":
								GroupType = enGroupType.Women;
								break;

							default:
								GroupType = enGroupType.Children;
								break;
						}
					}

					OnPropertyChanged("Name");
					OnPropertyChanged("FullGroupName");
				}
			}
		}

		int m_StartYear = GlobalDefines.DEFAULT_XML_INT_VAL;
		[XmlAttribute]
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int StartYear
		{
			get { return m_StartYear; }
			set
			{
				if (m_StartYear != value)
				{
					m_StartYear = value;
					if (m_StartYear == GlobalDefines.DEFAULT_XML_INT_VAL ||
						m_StartYear < GlobalDefines.MIN_GROUP_YEAR ||
						m_StartYear > GlobalDefines.MAX_GROUP_YEAR)
					{
						m_StartYear = GlobalDefines.DEFAULT_XML_INT_VAL;
					}

					OnPropertyChanged("StartYear");
					OnPropertyChanged("FullGroupName");
				}
			}
		}

		string m_EndYear = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string EndYear
		{
			get { return m_EndYear; }
			set
			{
				if (m_EndYear != value)
				{
					m_EndYear = value;
					if (string.IsNullOrWhiteSpace(m_EndYear))
						wEndYear = GlobalDefines.DEFAULT_XML_INT_VAL;
					else
					{
						switch (m_EndYear.ToLower())
						{
							case GlobalDefines.DEFAULT_XML_STRING_VAL:
								wEndYear = GlobalDefines.DEFAULT_XML_INT_VAL;
								break;

							case "и моложе":
								wEndYear = (int)enEndYearSpecVals.AndYounger;
								break;

							case "и старше":
								wEndYear = (int)enEndYearSpecVals.AndElder;
								break;

							default:
								if (!short.TryParse(m_EndYear, out m_wEndYear))
									wEndYear = GlobalDefines.DEFAULT_XML_INT_VAL;
								else
									if (StartYear == GlobalDefines.DEFAULT_XML_INT_VAL ||
										wEndYear < StartYear ||
										wEndYear < GlobalDefines.MIN_GROUP_YEAR ||
										wEndYear > GlobalDefines.MAX_GROUP_YEAR)
									{
										wEndYear = GlobalDefines.DEFAULT_XML_INT_VAL;
									}
								break;
						}
					}

					OnPropertyChanged("EndYear");
					OnPropertyChanged("FullGroupName");
				}
			}
		}

		byte m_Sex = GlobalDefines.DEFAULT_XML_BYTE_VAL;
		[XmlAttribute]
		[DefaultValue(GlobalDefines.DEFAULT_XML_BYTE_VAL)]
		public byte Sex
		{
			get { return m_Sex; }
			set
			{
				if (m_Sex != value)
				{
					m_Sex = value;
					OnPropertyChanged("Sex");
					OnPropertyChanged("SexInEnum");
				}
			}
		}
		#endregion


		#region Не сериализуемые свойства
		enGroupType m_GroupType = enGroupType.None;
		/// <summary>
		/// Тип группы.
		/// Если название группы в файле неверное, то поле будет иметь значение enGroupType.None.
		/// </summary>
		[XmlIgnore]
		public enGroupType GroupType
		{
			get { return m_GroupType; }
			private set
			{
				if (m_GroupType != value)
				{
					m_GroupType = value;
					OnPropertyChanged("GroupType");
				}
			}
		}


		short m_wEndYear = GlobalDefines.DEFAULT_XML_INT_VAL;
		/// <summary>
		/// Последний год группы в виде числа.
		/// Если год в файле неверный, то поле будет иметь значение GlobalDefines.DEFAULT_XML_INT_VAL.
		/// </summary>
		[XmlIgnore]
		public short wEndYear
		{
			get { return m_wEndYear; }
			private set
			{
				if (m_wEndYear != value)
				{
					m_wEndYear = value;
					OnPropertyChanged("wEndYear");
				}
			}
		}


		/// <summary>
		/// Пол группы в виде перечисления.
		/// Если название группы в файле неверное, то поле будет иметь значение enSex.None.
		/// </summary>
		[XmlIgnore]
		public enSex SexInEnum
		{
			get { return GlobalDefines.SexToEnum(Sex); }
		}

		
		[XmlIgnore]
		public string FullGroupName
		{
			get
			{
				string result = Name;

				switch (GroupType)
				{
					case enGroupType.Men:
						result = Properties.Resources.resMen;
						break;

					case enGroupType.Women:
						result = Properties.Resources.resWomen;
						break;

					case enGroupType.Children:
						if (StartYear != GlobalDefines.DEFAULT_XML_INT_VAL && wEndYear != GlobalDefines.DEFAULT_XML_INT_VAL)
						{
							result += " " + StartYear.ToString();
							switch (wEndYear)
							{
								case (int)enEndYearSpecVals.AndElder:
									result += " г. р. " + Properties.Resources.resAndElder;
									break;

								case (int)enEndYearSpecVals.AndYounger:
									result += " г. р. " + Properties.Resources.resAndYounger;
									break;

								default:
									result += "-" + wEndYear.ToString() + " г. р.";
									break;
							}
						}
						break;
				}

				return result;
			}
		}
		#endregion


		public CAgeGroup()
		{
			CXMLSerializerBase.SetDefaultValsForAllProps(this);
		}


		/// <summary>
		/// Конструктор копирования
		/// </summary>
		/// <param name="rhs"></param>
		public CAgeGroup(CAgeGroup rhs) :
			base(rhs)
		{
		}


		public override bool Equals(object o)
		{
			if (o is CAgeGroup)
				return this == (o as CAgeGroup);

			return false;
		}


		public override int GetHashCode()
		{
			string[] KeyFields = new string[] {
												Name ?? "null",
												StartYear.ToString(),
												EndYear ?? "null",
												Sex.ToString()
											  };
			return KeyFields.GetHashCode();
		}


		public static bool operator ==(CAgeGroup lhs, CAgeGroup rhs)
		{
			return ((object)lhs == null && (object)rhs == null) ||
					((object)lhs != null &&
						(object)rhs != null &&
						lhs.Name == rhs.Name &&
						lhs.StartYear == rhs.StartYear &&
						lhs.EndYear == rhs.EndYear &&
						lhs.Sex == rhs.Sex);
		}


		public static bool operator !=(CAgeGroup lhs, CAgeGroup rhs)
		{
			return !(lhs == rhs);
		}


		public static bool operator ==(groups lhs, CAgeGroup rhs)
		{
			return ((object)lhs == null && (object)rhs == null) ||
					((object)lhs != null &&
						(object)rhs != null &&
						lhs.name == rhs.Name &&
						lhs.start_year == rhs.StartYear &&
						lhs.end_year == rhs.wEndYear &&
						lhs.sex == rhs.Sex);
		}


		public static bool operator !=(groups lhs, CAgeGroup rhs)
		{
			return !(lhs == rhs);
		}
	}
}
