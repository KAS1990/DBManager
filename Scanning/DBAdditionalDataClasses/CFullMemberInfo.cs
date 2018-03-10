using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Global;
using System.ComponentModel;
using System.Globalization;
using DBManager.RoundMembers.Converters;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CFullMemberInfo : CDBAdditionalClassBase
	{
		static YearOfBirthMarkupConverter m_convYearOfBirth = new YearOfBirthMarkupConverter();
		static GradeMarkupConverter m_convGrade = new GradeMarkupConverter();
		

		#region IDMember
		private static readonly string IDMemberPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.IDMember);

		private long m_IDMember;
		public long IDMember
		{
			get { return m_IDMember; }
			set
			{
				if (m_IDMember != value)
				{
					m_IDMember = value;
					OnPropertyChanged(IDMemberPropertyName);
				}
			}
		}
		#endregion
				
		#region Name
		private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.Name);

		private string m_Name = GlobalDefines.DEFAULT_XML_STRING_VAL;

		public string Name
		{
			get { return m_Name; }
			set
			{
				if (m_Name != value)
				{
					m_Name = value;
					OnPropertyChanged(NamePropertyName);
					OnPropertyChanged(SurnameAndNamePropertyName);
				}
			}
		}
		#endregion
				
		#region Surname
		private static readonly string SurnamePropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.Surname);

		private string m_Surname = GlobalDefines.DEFAULT_XML_STRING_VAL;

		public string Surname
		{
			get { return m_Surname; }
			set
			{
				if (m_Surname != value)
				{
					m_Surname = value;
					OnPropertyChanged(SurnamePropertyName);
					OnPropertyChanged(SurnameAndNamePropertyName);
				}
			}
		}
		#endregion
				
		#region YearOfBirth
		private static readonly string YearOfBirthPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.YearOfBirth);

		private short? m_YearOfBirth = null;

		public short? YearOfBirth
		{
			get { return m_YearOfBirth; }
			set
			{
				if (m_YearOfBirth != value)
				{
					m_YearOfBirth = value;

					YearOfBirthForShow = m_convYearOfBirth.Convert(m_YearOfBirth, YearOfBirthForShow.GetType(), null, CultureInfo.CurrentCulture) as string;
					
					OnPropertyChanged(YearOfBirthPropertyName);
				}
			}
		}
		#endregion
		
		#region Coach
		private static readonly string CoachPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.Coach);

		private long? m_Coach = null;

		public long? Coach
		{
			get { return m_Coach; }
			set
			{
				if (m_Coach != value)
				{
					m_Coach = value;
					OnPropertyChanged(CoachPropertyName);
				}
			}
		}
		#endregion
		
		#region Team
		private static readonly string TeamPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.Team);

		private long? m_Team = null;

		public long? Team
		{
			get { return m_Team; }
			set
			{
				if (m_Team != value)
				{
					m_Team = value;
					OnPropertyChanged(TeamPropertyName);
				}
			}
		}
		#endregion
		
		#region InitGrade
		private static readonly string InitGradePropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.InitGrade);

		private byte? m_InitGrade = null;

		public byte? InitGrade
		{
			get { return m_InitGrade; }
			set
			{
				if (m_InitGrade != value)
				{
					m_InitGrade = value;

					InitGradeForShow = m_convGrade.Convert(m_InitGrade, InitGradeForShow.GetType(), null, CultureInfo.CurrentCulture) as string;
					
					OnPropertyChanged(InitGradePropertyName);
				}
			}
		}
		#endregion

				
		private static readonly string SurnameAndNamePropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.SurnameAndName);
		public string SurnameAndName
		{
			get { return GlobalDefines.CreateSurnameAndName(Surname, Name); }
		}


		#region SecondCol
		private static readonly string SecondColPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.SecondCol);

		private string m_SecondCol = null;

		public string SecondCol
		{
			get { return m_SecondCol; }
			set
			{
				if (m_SecondCol != value)
				{
					m_SecondCol = value;
					OnPropertyChanged(SecondColPropertyName);
				}
			}
		}
		#endregion


		#region Вместо конвертеров
		#region YearOfBirthForShow
		private static readonly string YearOfBirthForShowPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.YearOfBirthForShow);

		private string m_YearOfBirthForShow = "";

		public string YearOfBirthForShow
		{
			get { return m_YearOfBirthForShow; }
			set
			{
				if (m_YearOfBirthForShow != value)
				{
					m_YearOfBirthForShow = value;
					OnPropertyChanged(YearOfBirthForShowPropertyName);
				}
			}
		}
		#endregion


		#region InitGradeForShow
		private static readonly string InitGradeForShowPropertyName = GlobalDefines.GetPropertyName<CFullMemberInfo>(m => m.InitGradeForShow);

		private string m_InitGradeForShow = "";

		public string InitGradeForShow
		{
			get { return m_InitGradeForShow; }
			set
			{
				if (m_InitGradeForShow != value)
				{
					m_InitGradeForShow = value;
					OnPropertyChanged(InitGradeForShowPropertyName);
				}
			}
		}
		#endregion
		#endregion


		public override bool Equals(object o)
		{
			if (o is CFullMemberInfo)
				return this == (o as CFullMemberInfo);
			else if (o is CMember)
					return this == (o as CMember);

			return false;
		}


		public override int GetHashCode()
		{
			string[] KeyFields = new string[] { SurnameAndName, YearOfBirth.ToString(), InitGrade.ToString() };
			return KeyFields.GetHashCode();
		}


		public static bool operator == (CFullMemberInfo lhs, CMember rhs)
		{
			switch (GlobalDefines.ObjectBaseEquals(lhs, rhs))
			{
				case enObjectBaseEqualsResult.True:
					return true;

				case enObjectBaseEqualsResult.False:
					return false;

				default:
				{
					bool result = lhs.Name == rhs.Name &&
									lhs.Surname == rhs.Surname &&
									lhs.YearOfBirth == (rhs.YearOfBirthInShort < 0 ? (short)0 : rhs.YearOfBirthInShort);

					if (result)
						result = ((lhs.InitGrade == null && rhs.GradeInEnum == enGrade.None) || (lhs.InitGrade.Value == (byte)rhs.GradeInEnum));
					
					if (result)
					{
						// Т.к. тип второй колонки здесь поменяться не может, то делаем так
						if (lhs.Coach == null)
							result = ((lhs.Team == null && string.IsNullOrEmpty(rhs.SecondCol)) ||
										(lhs.Team == GlobalDefines.GetTeamId(rhs.SecondCol, false)));
						else
							result = ((lhs.Coach == null && string.IsNullOrEmpty(rhs.SecondCol)) ||
										(lhs.Coach == GlobalDefines.GetCoachId(rhs.SecondCol, false)));
					}

					return result;
				}
			}
		}


		public static bool operator ==(CMember lhs, CFullMemberInfo rhs)
		{
			return rhs == lhs;
		}


		public static bool operator !=(CFullMemberInfo lhs, CMember rhs)
		{
			return !(lhs == rhs);
		}


		public static bool operator !=(CMember lhs, CFullMemberInfo rhs)
		{
			return !(lhs == rhs);
		}
	}
}
