using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using System.Xml.Serialization;
using DBManager.RoundMembers.Converters;
using System.Globalization;

namespace DBManager.Scanning.XMLDataClasses
{
	public enum enResultColumnNumber
	{
		Route1,
		Route2,
		Sum
	}


	public class CResult : CXMLSerializerBase
	{
		/// <summary>
		/// Тип колонки с результатом
		/// </summary>
		[XmlIgnore]
		public enResultColumnNumber ResultColumnNumber { get; set; }

		#region Time
		private static readonly string InTimePropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.Time);

		private TimeSpan? m_Time = null;
		/// Результат участника на трассе в виде времени.
		/// При срыве поле будет иметь значение GlobalDefines.FALL_TIME_SPAN_VAL.
		/// Если время в файле неверное, то поле будет иметь значение null.
		[XmlIgnore]
		public TimeSpan? Time
		{
			get { return m_Time; }
			set
			{
				if (m_Time != value)
				{
					m_Time = value;

					ResultForShow = RouteResultsMarkupConverter.Convert(this);

					OnPropertyChanged(InTimePropertyName);
				}
			}
		}
		#endregion


		#region CondFormating
		private static readonly string CondFormatingRoute1PropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.CondFormating);

		private enCondFormating? m_CondFormating = null;
		[XmlIgnore]
		public enCondFormating? CondFormating
		{
			get { return m_CondFormating; }
			set
			{
				if (m_CondFormating != value)
				{
					m_CondFormating = value;
					OnPropertyChanged(CondFormatingRoute1PropertyName);
				}
			}
		}
		#endregion


		#region ResultPossible
		private static readonly string ResultPossiblePropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.ResultPossible);

		private bool m_ResultPossible = false;
		[XmlIgnore]
		public bool ResultPossible
		{
			get { return m_ResultPossible; }
			set
			{
				if (m_ResultPossible != value)
				{
					m_ResultPossible = value;
					OnPropertyChanged(ResultPossiblePropertyName);
				}
			}
		}
		#endregion


		#region Вместо конвертеров
		#region ResultForShow
		private static readonly string ResultForShowPropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.ResultForShow);

		private string m_ResultForShowForShow = "";

		public string ResultForShow
		{
			get { return m_ResultForShowForShow; }
			set
			{
				if (m_ResultForShowForShow != value)
				{
					m_ResultForShowForShow = value;
					OnPropertyChanged(ResultForShowPropertyName);
				}
			}
		}
		#endregion
		#endregion


		public CResult()
		{
		}


		public CResult(CResult rhs):
			base(rhs)
		{
		}


		public override bool Equals(object o)
		{
			if (o is CResult)
				return this == (o as CResult);
			else if (o is TimeSpan?)
				return this == (o as TimeSpan?);
			else if (o is TimeSpan)
				return this == (TimeSpan)o;

			return false;
		}


		public override int GetHashCode()
		{
			string[] KeyFields = new string[] {
												Time.HasValue ? Time.Value.ToString() : "null",
												CondFormating.HasValue ? CondFormating.Value.ToString() : "null"
											  };
			return KeyFields.GetHashCode();
		}


		#region Операторы ==
		public static bool operator ==(CResult lhs, TimeSpan? rhs)
		{
			switch (GlobalDefines.ObjectBaseEquals(lhs, rhs))
			{
				case enObjectBaseEqualsResult.True:
					return true;

				case enObjectBaseEqualsResult.False:
					return false;

				default:
					return rhs.TimeSpanEqualsForDB(lhs.Time);
			}
		}


		public static bool operator ==(TimeSpan? lhs, CResult rhs)
		{
			return rhs == lhs;
		}


		public static bool operator ==(CResult lhs, TimeSpan rhs)
		{
			if (lhs == null)
				return false;

			return rhs.TimeSpanEqualsForDB(lhs.Time);
		}


		public static bool operator ==(TimeSpan lhs, CResult rhs)
		{
			return rhs == lhs;
		}
		#endregion


		#region Операторы !=
		public static bool operator !=(TimeSpan? lhs, CResult rhs)
		{
			return !(lhs == rhs);
		}


		public static bool operator !=(CResult lhs, TimeSpan? rhs)
		{
			return !(lhs == rhs);
		}


		public static bool operator !=(TimeSpan lhs, CResult rhs)
		{
			return !(lhs == rhs);
		}


		public static bool operator !=(CResult lhs, TimeSpan rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
	}
}
