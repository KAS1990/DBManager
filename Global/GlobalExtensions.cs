using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.XMLDataClasses;
using System.Data.Objects;
using System.Reflection;
using System.Data.EntityClient;
using System.Data.Common;
using System.Collections;
using System.Drawing;

namespace DBManager.Global
{
	public static class GlobalExtensions
	{
		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}


		public static string ReplaceAt(this string source, int index, char newChar)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (index >= source.Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "string.ReplaceAt(): Index must be less then string length");
			}
			
			StringBuilder builder = new StringBuilder(source);
			builder[index] = newChar;
			return builder.ToString();
		}

		
		public static string ReplaceAt(this string source, int index, string newString)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (index >= source.Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "string.ReplaceAt(): Index must be less then string length");
			}

			source = source.Remove(index, newString.Length);
			return source = source.Insert(index, newString); ;
		}


		/// <summary>
		/// Переводит в верхний регистр несколько символов, начиная с заданного
		/// </summary>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string ToUpper(this string source, int index, int length = 1)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (index >= source.Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "string.ToUpper(): Index must be less then string length");
			}

			string ChangedString = source.Substring(index, length).ToUpper();
			return source = source.ReplaceAt(index, ChangedString);
		}


		/// <summary>
		/// Переводит в нижний регистр несколько символов, начиная с заданного
		/// </summary>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static string ToLower(this string source, int index, int length = 1)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			if (index >= source.Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "string.ToLower(): Index must be less then string length");
			}

			string ChangedString = source.Substring(index, length).ToLower();
			return source = source.ReplaceAt(index, ChangedString);
		}


		public static string Left(this string source, int length)
		{
			return source.Substring(0, Math.Min(length, source.Length));
		}


		public static string Right(this string source, int length)
		{
			return source.Substring(source.Length - Math.Min(length, source.Length));
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="ResultInDB"></param>
		/// <param name="ResultInXML"></param>
		/// <returns>Что поменялось</returns>
		public static enChangedResult UpdateResults(this results_speed ResultInDB, CMember ResultInXML)
		{
			enChangedResult result = enChangedResult.None;

			if (ResultInXML.Route1Ext != null)
			{
				if (ResultInDB.route1 != ResultInXML.Route1Ext)
				{
					ResultInDB.route1 = ResultInXML.Route1Ext.Time;
					result |= enChangedResult.Route1Time;
				}
				if (ResultInDB.cond_formating_1 != (byte?)ResultInXML.Route1Ext.CondFormating)
				{
					ResultInDB.cond_formating_1 = (byte?)ResultInXML.Route1Ext.CondFormating;
					result |= enChangedResult.Route1CondFormatting;
				}
			}
			else
			{
				if (ResultInDB.route1 != null)
				{
					ResultInDB.route1 = null;
					result |= enChangedResult.Route1Time;
				}
				if (ResultInDB.cond_formating_1 != null)
				{
					ResultInDB.cond_formating_1 = null;
					result |= enChangedResult.Route1CondFormatting;
				}
			}

			if (ResultInXML.Route2Ext != null)
			{
				if (ResultInDB.route2 != ResultInXML.Route2Ext)
				{
					ResultInDB.route2 = ResultInXML.Route2Ext.Time;
					result |= enChangedResult.Route2Time;
				}
				if (ResultInDB.cond_formating_2 != (byte?)ResultInXML.Route2Ext.CondFormating)
				{
					ResultInDB.cond_formating_2 = (byte?)ResultInXML.Route2Ext.CondFormating;
					result |= enChangedResult.Route2CondFormatting;
				}
			}
			else
			{
				if (ResultInDB.route2 != null)
				{
					ResultInDB.route2 = null;
					result |= enChangedResult.Route2Time;
				}
				if (ResultInDB.cond_formating_2 != null)
				{
					ResultInDB.cond_formating_2 = null;
					result |= enChangedResult.Route2CondFormatting;
				}
			}

			if (ResultInXML.SumExt != null)
			{
				if (ResultInDB.sum != ResultInXML.SumExt)
				{
					ResultInDB.sum = ResultInXML.SumExt.Time;
					result |= enChangedResult.SumTime;
				}
				if (ResultInDB.cond_formating_sum != (byte?)ResultInXML.SumExt.CondFormating)
				{
					ResultInDB.cond_formating_sum = (byte?)ResultInXML.SumExt.CondFormating;
					result |= enChangedResult.SumCondFormatting;
				}
			}
			else
			{
				if (ResultInDB.sum != null)
				{
					ResultInDB.sum = null;
					result |= enChangedResult.SumTime;
				}
				if (ResultInDB.cond_formating_sum != null)
				{
					ResultInDB.cond_formating_sum = null;
					result |= enChangedResult.SumCondFormatting;
				}
			}

			return result;
		}


		public static bool IsWinnerInPair(this results_speed lhs, results_speed rhs)
		{
			return lhs.sum < rhs.sum;
		}


		public static bool TryAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
			{
				return false;
			}
			else
			{
				dict.Add(key, value);
				return true;
			}
		}

				
		/// <summary>
		/// Равны ли данные об участниках без учёта индексов
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool OnlyDataFieldsEqual(this members lhs, members rhs)
		{
			return lhs.name == rhs.name &&
					lhs.surname == rhs.surname &&
					lhs.year_of_birth == rhs.year_of_birth &&
					lhs.sex == rhs.sex;
		}


		/// <summary>
		/// Равны ли данные, которые заносятся в таблицу participations из xml-файла
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static bool OnlyFillFromXMLFieldsEqual(this participations lhs, CMember rhs, CCompSettings CompSettings)
		{
			bool result = lhs.init_grade == (byte)rhs.GradeInEnum;
			
			if (result)
			{
				if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
				{
					if (rhs.SecondCol == null)
					{
						if (lhs.coach != null)
							result = false;
					}
					else if (lhs.coach == null)
						result = false;
					else
					{	// Проверяем, не изменилось ли название тренера
						coaches CurCoachInDB = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == lhs.coach);
						result = CurCoachInDB.name == rhs.SecondCol;
					}
				}
				else
				{
					if (rhs.SecondCol == null)
					{
						if (lhs.team != null)
							result = false;
					}
					else if (lhs.team == null)
						result = false;
					else
					{	// Проверяем, не изменилось ли название тренера
						teams CurTeamInDB = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == lhs.team);
						result = CurTeamInDB.name == rhs.SecondCol;
					}
				}
			}

			return result;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public static void ClearResults(this results_speed lhs)
		{
			lhs.route1 = lhs.route2 = lhs.sum = null;
		}


		public static void ClearCondFormating(this results_speed lhs)
		{
			lhs.cond_formating_1 = lhs.cond_formating_2 = lhs.cond_formating_sum = null;
		}
		

		/// <summary>
		/// Сравнение TimeSpan с учётом того, что в БД хранятся не миллисекунды а сотые доли секунды
		/// </summary>
		/// <param name="DBTime">
		/// Время из БД, где сотые
		/// </param>
		/// <param name="StdTime">
		/// Стандартное время, где миллисекунды
		/// </param>
		/// <returns></returns>
		public static bool TimeSpanEqualsForDB(this TimeSpan? DBTime, TimeSpan? StdTime)
		{
			switch (GlobalDefines.ObjectBaseEquals(DBTime, StdTime))
			{
				case enObjectBaseEqualsResult.True:
					return true;

				case enObjectBaseEqualsResult.False:
					return false;

				default:
					return DBTime.Value.Hours == StdTime.Value.Hours &&
							DBTime.Value.Minutes == StdTime.Value.Minutes &&
							DBTime.Value.Seconds == StdTime.Value.Seconds &&
							DBTime.Value.Milliseconds / 10.0 == StdTime.Value.Milliseconds;
			}
		}


		/// <summary>
		/// Сравнение TimeSpan с учётом того, что в БД хранятся не миллисекунды а сотые доли секунды
		/// </summary>
		/// <param name="DBTime">
		/// Время из БД, где сотые
		/// </param>
		/// <param name="StdTime">
		/// Стандартное время, где миллисекунды
		/// </param>
		/// <returns></returns>
		public static bool TimeSpanEqualsForDB(this TimeSpan DBTime, TimeSpan? StdTime)
		{
			if (StdTime == null)
				return false;
			else
			{
				return DBTime.Hours == StdTime.Value.Hours &&
						DBTime.Minutes == StdTime.Value.Minutes &&
						DBTime.Seconds == StdTime.Value.Seconds &&
						DBTime.Milliseconds / 10.0 == StdTime.Value.Milliseconds;
			}
		}


		private static readonly string entityAssemblyName =
            "system.data.entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public static string ToTraceString(this IQueryable query)
        {
            System.Reflection.MethodInfo toTraceStringMethod = query.GetType().GetMethod("ToTraceString");

            if (toTraceStringMethod != null)
                return toTraceStringMethod.Invoke(query, null).ToString();
            else
                return "";
        }

        public static string ToTraceString(this ObjectContext ctx)
        {
            Assembly entityAssemly = Assembly.Load(entityAssemblyName);

            Type updateTranslatorType = entityAssemly.GetType(
                "System.Data.Mapping.Update.Internal.UpdateTranslator");

            Type functionUpdateCommandType = entityAssemly.GetType(
                "System.Data.Mapping.Update.Internal.FunctionUpdateCommand");

            Type dynamicUpdateCommandType = entityAssemly.GetType(
                "System.Data.Mapping.Update.Internal.DynamicUpdateCommand");

            object[] ctorParams = new object[]
                        {
                            ctx.ObjectStateManager,
                            ((EntityConnection)ctx.Connection).GetMetadataWorkspace(),
                            (EntityConnection)ctx.Connection,
                            ctx.CommandTimeout
                        };

            object updateTranslator = Activator.CreateInstance(updateTranslatorType,
                BindingFlags.NonPublic | BindingFlags.Instance, null, ctorParams, null);

            MethodInfo produceCommandsMethod = updateTranslatorType
                .GetMethod("ProduceCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            object updateCommands = produceCommandsMethod.Invoke(updateTranslator, null);

            List<DbCommand> dbCommands = new List<DbCommand>();

            foreach (object o in (IEnumerable)updateCommands)
            {
                if (functionUpdateCommandType.IsInstanceOfType(o))
                {
                    FieldInfo m_dbCommandField = functionUpdateCommandType.GetField(
                        "m_dbCommand", BindingFlags.Instance | BindingFlags.NonPublic);

                    dbCommands.Add((DbCommand)m_dbCommandField.GetValue(o));
                }
                else if (dynamicUpdateCommandType.IsInstanceOfType(o))
                {
                    MethodInfo createCommandMethod = dynamicUpdateCommandType.GetMethod(
                        "CreateCommand", BindingFlags.Instance | BindingFlags.NonPublic);

                    object[] methodParams = new object[]
                    {
                        updateTranslator,
                        new Dictionary<int, object>()
                    };

                    dbCommands.Add((DbCommand)createCommandMethod.Invoke(o, methodParams));
                }
                else
                {
                    throw new NotSupportedException("Unknown UpdateCommand Kind");
                }
            }


            StringBuilder traceString = new StringBuilder();
            foreach (DbCommand command in dbCommands)
            {
                traceString.AppendLine("=============== BEGIN COMMAND ===============");
                traceString.AppendLine();

                traceString.AppendLine(command.CommandText);
                foreach (DbParameter param in command.Parameters)
                {
                    traceString.AppendFormat("{0} = {1}", param.ParameterName, param.Value);
                    traceString.AppendLine();
                }

                traceString.AppendLine();
                traceString.AppendLine("=============== END COMMAND ===============");
            }

            return traceString.ToString();
        }


		/// <summary>
		/// Компоненты цвета в Excel почему-то идут в обратном порядке
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static int ToExcelColor(this Color color)
		{
			return ((int)(color.A) << 24) |
					((int)(color.B) << 16) |
					((int)(color.G) << 8) |
					((int)(color.R));
		}
	}
}
