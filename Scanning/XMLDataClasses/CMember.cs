﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using DBManager.Global;

namespace DBManager.Scanning.XMLDataClasses
{
	/// <summary>
	/// Участник соревнований и его результаты
	/// </summary>
	[Serializable]
	public class CMember : CXMLSerializerBase
	{
		#region Сериализуемые свойства
		#region PrevRoundNumber
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int PrevRoundNumber { get; set; }
		public bool ShouldSerializePrevRoundNumber()
		{
			return ShouldSerializeProperty("PrevRoundNumber");
		}
		#endregion

		#region Информация об участнике

		#region SurnameAndName
		string m_SurnameAndName = GlobalDefines.DEFAULT_XML_STRING_VAL;
		/// <summary>
		/// Имя и фамилия
		/// </summary>
		[XmlAttribute(AttributeName = "Name")]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string SurnameAndName
		{
			get { return m_SurnameAndName; }
			set
			{
				if (m_SurnameAndName != value)
					m_SurnameAndName = GlobalDefines.CorrectSurnameAndName(value, out m_arrNameAndSurname);
			}
		}
		public bool ShouldSerializeSurnameAndName()
		{
			return ShouldSerializeProperty("SurnameAndName");
		}
		#endregion

		#region SecondCol
		string m_SecondCol = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute(AttributeName = "Team")]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string SecondCol
		{
			get { return m_SecondCol; }
			set
			{
				if (m_SecondCol != value)
					m_SecondCol = GlobalDefines.CorrectTeam(value);
			}
		}
		public bool ShouldSerializeSecondCol()
		{
			return ShouldSerializeProperty("SecondCol");
		}
		#endregion

		#region YearOfBirth
		string m_YearOfBirth = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string YearOfBirth
		{
			get { return m_YearOfBirth; }
			set
			{
				if (m_YearOfBirth != value)
				{
					m_YearOfBirth = value;
					if (string.IsNullOrWhiteSpace(m_YearOfBirth))
						YearOfBirthInShort = GlobalDefines.DEFAULT_XML_INT_VAL;
					else
					{
						m_YearOfBirth = m_YearOfBirth.Trim();
						if (!short.TryParse(m_YearOfBirth, out m_YearOfBirthInShort) || m_YearOfBirthInShort < GlobalDefines.MIN_YEAR_OF_BIRTH)
							YearOfBirthInShort = GlobalDefines.DEFAULT_XML_INT_VAL;
					}
				}
			}
		}
		public bool ShouldSerializeYearOfBirth()
		{
			return ShouldSerializeProperty("YearOfBirth");
		}
		#endregion

		#region Grade
		string m_Grade = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Grade
		{
			get { return m_Grade; }
			set
			{
				if (m_Grade != value)
				{
					m_Grade = value;
					GradeInEnum = enGrade.None;
					if (m_Grade != null)
					{
						m_Grade = m_Grade.Trim();
						// Исправляем ошибки в названии разрядов
						if (m_Grade.Contains('б') && m_Grade.Contains('р'))
						{
							m_Grade = "б/р";
							GradeInEnum = enGrade.WithoutGrade;
						}
						else if (m_Grade.Contains('3'))
						{
							if (m_Grade.Contains('ю'))
							{
								m_Grade = "3 ю";
								GradeInEnum = enGrade.Young3;
							}
							else
							{
								m_Grade = "3";
								GradeInEnum = enGrade.Adult3;
							}
						}
						else if (m_Grade.Contains('2'))
						{
							if (m_Grade.Contains('ю'))
							{
								m_Grade = "2 ю";
								GradeInEnum = enGrade.Young2;
							}
							else
							{
								m_Grade = "2";
								GradeInEnum = enGrade.Adult2;
							}
						}
						else if (m_Grade.Contains('1'))
						{
							if (m_Grade.Contains('ю'))
							{
								m_Grade = "1 ю";
								GradeInEnum = enGrade.Young1;
							}
							else
							{
								m_Grade = "1";
								GradeInEnum = enGrade.Adult1;
							}
						}
						else if (m_Grade.Contains("к", StringComparison.OrdinalIgnoreCase))
						{
							m_Grade = "КМС";
							GradeInEnum = enGrade.BeforeMaster;
						}
						else if (m_Grade.Contains("м", StringComparison.OrdinalIgnoreCase))
						{
							m_Grade = "МС";
							GradeInEnum = enGrade.Master;
						}
						else // Неизвестный разряд
						{
							m_Grade = GlobalDefines.DEFAULT_XML_STRING_VAL;
						}
					}
				}
			}
		}
		public bool ShouldSerializeGrade()
		{
			return ShouldSerializeProperty("Grade");
		}

		#region Sex
		[XmlAttribute]
		[DefaultValue(GlobalDefines.DEFAULT_XML_BYTE_VAL)]
		public byte Sex { get; set; }
		public bool ShouldSerializeSex()
		{
			return ShouldSerializeProperty("Sex");
		}
		#endregion
		#endregion
		#endregion

		#region Результаты участника
		#region Route1
		string m_Route1 = GlobalDefines.DEFAULT_XML_STRING_VAL;
		/// <summary>
		/// Результат на первой трассе.
		/// Т.к. может быть "срыв", то используем string
		/// </summary>
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Route1
		{
			get { return m_Route1; }
			set
			{
				if (m_Route1 != value)
				{
					m_Route1 = value;

					if (Route1Ext == null)
					{
						Route1Ext = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Route1
						};
					}
					
					if (string.IsNullOrWhiteSpace(m_Route1))
						Route1Ext.Time = null;
					else
					{
						m_Route1 = m_Route1.Trim();
						Route1Ext.Time = GlobalDefines.RouteResultToTime(m_Route1);
					}
				}
			}
		}
		public bool ShouldSerializeRoute1()
		{
			return ShouldSerializeProperty("Route1");
		}
		#endregion

		#region Route2
		string m_Route2 = GlobalDefines.DEFAULT_XML_STRING_VAL;
		/// <summary>
		/// Результат на второй трассе.
		/// Т.к. может быть "срыв", то используем string
		/// </summary>
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Route2
		{
			get { return m_Route2; }
			set
			{
				if (m_Route2 != value)
				{
					m_Route2 = value;

					if (Route2Ext == null)
					{
						Route2Ext = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Route2
						};
					}
					
					if (string.IsNullOrWhiteSpace(m_Route2))
						Route2Ext.Time = null;
					else
					{
						m_Route2 = m_Route2.Trim();
						Route2Ext.Time = GlobalDefines.RouteResultToTime(m_Route2);
					}
				}
			}
		}
		public bool ShouldSerializeRoute2()
		{
			return ShouldSerializeProperty("Route2");
		}
		#endregion

		#region Sum
		string m_Sum = GlobalDefines.DEFAULT_XML_STRING_VAL;
		/// <summary>
		/// Сумма результатов на двух трассах.
		/// Т.к. может быть "срыв", то используем string
		/// </summary>
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Sum
		{
			get { return m_Sum; }
			set
			{
				if (m_Sum != value)
				{
					m_Sum = value;

					if (SumExt == null)
					{
						SumExt = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Sum
						};
					}

					if (string.IsNullOrWhiteSpace(m_Sum))
						SumExt.Time = null;
					else
					{
						m_Sum = m_Sum.Trim();
						SumExt.Time = GlobalDefines.RouteResultToTime(m_Sum);
					}
				}
			}
		}
		public bool ShouldSerializeSum()
		{
			return ShouldSerializeProperty("Sum");
		}
		#endregion
		#endregion

		#region Для выделения участника цветом
		#region CondFormatingRoute1
		string m_CondFormatingRoute1 = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string CondFormatingRoute1
		{
			get { return m_CondFormatingRoute1; }
			set
			{
				if (m_CondFormatingRoute1 != value)
				{
					m_CondFormatingRoute1 = value;

					if (Route1Ext == null)
					{
						Route1Ext = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Route1
						};
					}

					if (string.IsNullOrWhiteSpace(m_CondFormatingRoute1))
						Route1Ext.CondFormating = null;
					else
					{
						byte res = 0;
						if (byte.TryParse(m_CondFormatingRoute1, out res) && Enum.IsDefined(typeof(enCondFormating), (enCondFormating)res))
							Route1Ext.CondFormating = (enCondFormating)res;
						else
							Route1Ext.CondFormating = null;
					}
				}
			}
		}
		public bool ShouldSerializeCondFormatingRoute1()
		{
			return ShouldSerializeProperty("CondFormatingRoute1");
		}
		#endregion

		#region CondFormatingRoute2
		string m_CondFormatingRoute2 = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string CondFormatingRoute2
		{
			get { return m_CondFormatingRoute2; }
			set
			{
				if (m_CondFormatingRoute2 != value)
				{
					m_CondFormatingRoute2 = value;

					if (Route2Ext == null)
					{
						Route2Ext = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Route2
						};
					}

					if (string.IsNullOrWhiteSpace(m_CondFormatingRoute2))
						Route2Ext.CondFormating = null;
					else
					{
						byte res = 0;
						if (byte.TryParse(m_CondFormatingRoute2, out res) && Enum.IsDefined(typeof(enCondFormating), (enCondFormating)res))
							Route2Ext.CondFormating = (enCondFormating)res;
						else
							Route2Ext.CondFormating = null;
					}
				}
			}
		}
		public bool ShouldSerializeCondFormatingRoute2()
		{
			return ShouldSerializeProperty("CondFormatingRoute2");
		}
		#endregion

		#region CondFormatingSum
		string m_CondFormatingSum = GlobalDefines.DEFAULT_XML_STRING_VAL;
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string CondFormatingSum
		{
			get { return m_CondFormatingSum; }
			set
			{
				if (m_CondFormatingSum != value)
				{
					m_CondFormatingSum = value;

					if (SumExt == null)
					{
						SumExt = new CResult()
						{
							ResultColumnNumber = enResultColumnNumber.Sum
						};
					}

					if (string.IsNullOrWhiteSpace(m_CondFormatingSum))
						SumExt.CondFormating = null;
					else
					{
						byte res = 0;
						if (byte.TryParse(m_CondFormatingSum, out res) && Enum.IsDefined(typeof(enCondFormating), (enCondFormating)res))
							SumExt.CondFormating = (enCondFormating)res;
						else
							SumExt.CondFormating = null;
					}
				}
			}
		}
		public bool ShouldSerializeCondFormatingSum()
		{
			return ShouldSerializeProperty("CondFormatingSum");
		}
		#endregion
		#endregion
		#endregion

		#region Не сериализуемые свойства
		/// <summary>
		/// Фамилия и имя участника в виде массива из двух элементов
		/// Если этой информации в xml-файле нет, то поле будет иметь значение null.
		/// </summary>
		string[] m_arrNameAndSurname = null;

		#region Name
		/// <summary>
		/// Имя участника
		/// </summary>
		[XmlIgnore]
		public string Name
		{
			get
			{
				if (m_arrNameAndSurname == null || m_arrNameAndSurname.Length < 2)
					return GlobalDefines.DEFAULT_XML_STRING_VAL;

				// Имя должно идти после фамилии
				return m_arrNameAndSurname[1];
			}
		}
		#endregion

		#region Surname
		/// <summary>
		/// Фамилия участника
		/// </summary>
		[XmlIgnore]
		public string Surname
		{
			get
			{
				if (m_arrNameAndSurname == null || m_arrNameAndSurname.Length < 2)
					return GlobalDefines.DEFAULT_XML_STRING_VAL;

				// Фамилия должна быть первой
				return m_arrNameAndSurname[0];
			}
		}
		#endregion

		#region Number
		byte m_Number = GlobalDefines.DEFAULT_XML_BYTE_VAL;
		/// <summary>
		/// Номер участника
		/// </summary>
		[XmlIgnore]
		public byte Number
		{
			get { return m_Number; }
			set { m_Number = value; }
		}
		#endregion

		#region SexInEnum
		/// <summary>
		/// Пол участника в виде перечисления.
		/// Если название группы в файле неверное, то поле будет иметь значение enSex.None.
		/// </summary>
		[XmlIgnore]
		public enSex SexInEnum
		{
			get { return GlobalDefines.SexToEnum(Sex); }
		}
		#endregion

		#region Route1Ext
		CResult m_Route1Ext = null;
		/// <summary>
		/// Результат участника на первой трассе.
		[XmlIgnore]
		public CResult Route1Ext
		{
			get { return m_Route1Ext; }
			private set { m_Route1Ext = value; }
		}
		#endregion

		#region Route2Ext
		/// <summary>
		/// Результат участника на второй трассе.
		/// </summary>
		CResult m_Route2Ext = null;
		[XmlIgnore]
		public CResult Route2Ext
		{
			get { return m_Route2Ext; }
			private set { m_Route2Ext = value; }
		}
		#endregion

		#region SumExt
		/// <summary>
		/// Сумма рузультатов участника на двух трассах.
		/// </summary>
		CResult m_SumExt = null;
		[XmlIgnore]
		public CResult SumExt
		{
			get { return m_SumExt; }
			private set { m_SumExt = value; }
		}
		#endregion
		
		#region YearOfBirthInShort
		short m_YearOfBirthInShort = GlobalDefines.DEFAULT_XML_INT_VAL;
		[XmlIgnore]
		public short YearOfBirthInShort
		{
			get { return m_YearOfBirthInShort; }
			private set { m_YearOfBirthInShort = value; }
		}
		#endregion

		#region GradeInEnum
		enGrade m_GradeInEnum = enGrade.None;
		[XmlIgnore]
		public enGrade GradeInEnum
		{
			get { return m_GradeInEnum; }
			private set { m_GradeInEnum = value; }
		}
		#endregion


		public bool HasResultsSpeed
		{
			get
			{
				return (Route1Ext != null && Route1Ext.Time.HasValue) ||
						(Route2Ext != null && Route2Ext.Time.HasValue) ||
						(SumExt != null && SumExt.Time.HasValue);
			}
		}
		#endregion


		public CMember()
		{
			CXMLSerializerBase.SetDefaultValsForAllProps(this);
		}


		#region Методы преоразования класса в другой тип
		/// <summary>
		/// Неявный оператор преобразования в members
		/// </summary>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static implicit operator members(CMember rhs)
		{
			members result = new members()
			{
				name = rhs.Name,
				surname = rhs.Surname,
				year_of_birth = rhs.YearOfBirthInShort < 0 ? (short)0 : rhs.YearOfBirthInShort,
				sex = rhs.Sex
			};

			return result;
		}


		/// <summary>
		/// Данная функция не заполняет поля result_grade и result_place
		/// </summary>
		/// <param name="MemberId"></param>
		/// <param name="GroupId"></param>
		/// <param name="SecondColNameType"></param>
		/// <returns></returns>
		public participations ToParticipation(long MemberId, long GroupId, enSecondColNameType SecondColNameType)
		{
			participations result = new participations()
			{
				member = MemberId,
				Group = GroupId,
			};
			if (GradeInEnum != enGrade.None)
				result.init_grade = (byte)GradeInEnum;

			switch (SecondColNameType)
			{
				case enSecondColNameType.Coach:
					result.coach = GlobalDefines.GetCoachId(SecondCol, true);
					break;

				case enSecondColNameType.Team:
				default:	// по умолчанию будет "команда"
					result.team = GlobalDefines.GetTeamId(SecondCol, true);
					break;
			}

			return result;
		}


		/// <summary>
		/// Данная функция не заполняет поля place и balls
		/// </summary>
		/// <param name="MemberId"></param>
		/// <param name="GroupId"></param>
		/// <param name="SecondColNameType"></param>
		/// <returns></returns>
		public results_speed ToResults_Speed(long ParticipationId, byte RoundId)
		{
			results_speed result = new results_speed()
			{
				participation = ParticipationId,
				round = RoundId,
				number = Number,
			};

			if (Route1Ext != null)
			{
				result.route1 = Route1Ext.Time;
				result.cond_formating_1 = (byte?)Route1Ext.CondFormating;
			}

			if (Route2Ext != null)
			{
				result.route2 = Route2Ext.Time;
				result.cond_formating_2 = (byte?)Route2Ext.CondFormating;
			}

			if (SumExt != null)
			{
				result.sum = SumExt.Time;
				result.cond_formating_sum = (byte?)SumExt.CondFormating;
			}

			return result;
		}
		#endregion

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="MemberInDB"></param>
		/// <returns></returns>
		public void CopyMembersInfoToDB(members MemberInDB)
		{
			MemberInDB.name = Name;
			MemberInDB.surname = Surname;
			MemberInDB.year_of_birth = YearOfBirthInShort < 0 ? (short)0 : YearOfBirthInShort;
			MemberInDB.sex = Sex;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="PartInDB"></param>
		/// <returns></returns>
		public void CopyPartToDB(participations PartInDB, enSecondColNameType SecondColNameType)
		{
			PartInDB.init_grade = GradeInEnum == enGrade.None ? null : (byte?)GradeInEnum;

			switch (SecondColNameType)
			{
				case enSecondColNameType.Coach:
					PartInDB.team = null;
					PartInDB.coach = GlobalDefines.GetCoachId(SecondCol, true);
					break;

				case enSecondColNameType.Team:
				default:	// по умолчанию будет "команда"
					PartInDB.coach = null;
					PartInDB.team = GlobalDefines.GetTeamId(SecondCol, true);
					break;
			}
		}


		public void ClearCondFormating()
		{
			if (Route1Ext != null)
				Route1Ext.CondFormating = null;

			if (Route2Ext != null)
				Route2Ext.CondFormating = null;

			if (SumExt != null)
				SumExt.CondFormating = null;
		}
	}
}
