using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using System.Xml.Serialization;
using DBManager.RoundMembers.Converters;
using System.Globalization;
using DBManager.Commands;

namespace DBManager.Scanning.XMLDataClasses
{
	[Flags]
	public enum enResultColumnNumber
	{
		Route1 = 1 << 0,
		Route2 = 1 << 1,
		Sum = 1 << 2,
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
		private static readonly string CondFormatingRoutePropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.CondFormating);

		private enCondFormating? m_CondFormating = null;
		[XmlIgnore]
		public enCondFormating? CondFormating
		{
			get { return m_CondFormating; }
			set
			{
				if (value.HasValue && value == enCondFormating.None)
					value = null;

				if (m_CondFormating != value)
				{
					m_CondFormating = value;
					OnPropertyChanged(CondFormatingRoutePropertyName);
				}
			}
		}
		#endregion

		
		#region AdditionalEventTypes
		private static readonly string AdditionalEventTypesPropertyName = GlobalDefines.GetPropertyName<CResult>(m => m.AdditionalEventTypes);

		private enAdditionalEventTypes? m_AdditionalEventTypes = null;
		[XmlIgnore]
		public enAdditionalEventTypes? AdditionalEventTypes
		{
			get { return m_AdditionalEventTypes; }
			set
			{
				if (value.HasValue && value == enAdditionalEventTypes.None)
					value = null;
				if (m_AdditionalEventTypes != value)
				{
					m_AdditionalEventTypes = value;
					ResultForShow = RouteResultsMarkupConverter.Convert(this);
					RemoveFalsestart.CanExecute = m_AdditionalEventTypes.HasValue && m_AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.Falsestart);

					OnPropertyChanged(AdditionalEventTypesPropertyName);
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


		#region RemoveFalsestart
		private CCommand m_RemoveFalsestart = null;
		[XmlIgnore]
		public CCommand RemoveFalsestart
		{
			get { return m_RemoveFalsestart; }
			private set
			{
				if (m_RemoveFalsestart != value)
					m_RemoveFalsestart = value;
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
			RemoveFalsestart = new CCommand(RemoveFalsestart_Executed,
				AdditionalEventTypes.HasValue && AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.Falsestart));
		}


		public CResult(CResult rhs):
			base(rhs)
		{
			RemoveFalsestart = new CCommand(RemoveFalsestart_Executed,
				AdditionalEventTypes.HasValue && AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.Falsestart));
		}


		private void RemoveFalsestart_Executed()
		{
			AdditionalEventTypes = AdditionalEventTypes.Value ^ enAdditionalEventTypes.Falsestart;
		}



		public void SetCondFormating(string ExcelCondFormating)
		{
			if (string.IsNullOrWhiteSpace(ExcelCondFormating))
			{
				if (AdditionalEventTypes != null)
				{   // Удаляем все флаги кроме "неявка"
					AdditionalEventTypes = AdditionalEventTypes.Value & enAdditionalEventTypes.DontAppear;
				}
				CondFormating = null;
			}
			else
			{
				byte res = 0;
				if (byte.TryParse(ExcelCondFormating, out res))
				{
					enAdditionalEventTypes EventTypes = res.GetAdditionalEventTypes();
					enCondFormating Formating = res.GetCondFormating();

					if (Enum.IsDefined(typeof(enCondFormating), Formating))
						CondFormating = Formating == enCondFormating.None ? null : (enCondFormating?)Formating;
					else
						CondFormating = null;

					if (Enum.IsDefined(typeof(enAdditionalEventTypes), EventTypes))
					{
						if (AdditionalEventTypes.HasValue)
							AdditionalEventTypes = (AdditionalEventTypes.Value & enAdditionalEventTypes.DontAppear) | EventTypes;
						else
							AdditionalEventTypes = EventTypes;
					}
					else
					{
						if (AdditionalEventTypes != null)
						{   // Удаляем все флаги кроме "неявка"
							AdditionalEventTypes = AdditionalEventTypes.Value & enAdditionalEventTypes.DontAppear;
						}
					}
				}
			}
		}


		public void SetTime(ref string ExcelTime)
		{
			ExcelTime = ExcelTime.Trim();
			enAdditionalEventTypes EventType;
			Time = GlobalDefines.RouteResultToTime(ExcelTime, out EventType);

			if (EventType != enAdditionalEventTypes.None)
			{
				if (AdditionalEventTypes.HasValue)
					AdditionalEventTypes = (AdditionalEventTypes & ~enAdditionalEventTypes.DontAppear) | EventType;
				else
					AdditionalEventTypes = EventType;
			}
			else
			{
				if (AdditionalEventTypes.HasValue)
					AdditionalEventTypes = AdditionalEventTypes & ~enAdditionalEventTypes.DontAppear;
			}
		}


		public void ClearTime()
		{
			Time = null;
			if (AdditionalEventTypes.HasValue)
			{
				AdditionalEventTypes = AdditionalEventTypes & ~enAdditionalEventTypes.DontAppear;
			}
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
