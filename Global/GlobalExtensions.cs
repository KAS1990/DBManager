using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Common;
using System.Collections;
using System.Drawing;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using MySql.Data.MySqlClient;

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


        public static enCondFormating GetCondFormating(this byte ExcelCondFormatingFlags)
        {
            return (enCondFormating)(ExcelCondFormatingFlags & 0x0000000F);
        }


        public static enAdditionalEventTypes GetAdditionalEventTypes(this byte ExcelCondFormatingFlags)
        {
            return (enAdditionalEventTypes)(ExcelCondFormatingFlags >> 4);
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


        //private static readonly string entityAssemblyName =
        //	"system.data.entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

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
            return "";
            //Assembly entityAssemly = Assembly.Load(entityAssemblyName);

            //Type updateTranslatorType = entityAssemly.GetType(
            //	"System.Data.Mapping.Update.Internal.UpdateTranslator");

            //Type functionUpdateCommandType = entityAssemly.GetType(
            //	"System.Data.Mapping.Update.Internal.FunctionUpdateCommand");

            //Type dynamicUpdateCommandType = entityAssemly.GetType(
            //	"System.Data.Mapping.Update.Internal.DynamicUpdateCommand");

            //object[] ctorParams = new object[]
            //			{
            //				ctx.ObjectStateManager,
            //				ctx.Connection.GetMetadataWorkspace(),
            //				ctx.Connection,
            //				ctx.CommandTimeout
            //			};

            //object updateTranslator = Activator.CreateInstance(updateTranslatorType,
            //	BindingFlags.NonPublic | BindingFlags.Instance, null, ctorParams, null);

            //MethodInfo produceCommandsMethod = updateTranslatorType
            //	.GetMethod("ProduceCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            //object updateCommands = produceCommandsMethod.Invoke(updateTranslator, null);

            //List<DbCommand> dbCommands = new List<DbCommand>();

            //foreach (object o in (IEnumerable)updateCommands)
            //{
            //	if (functionUpdateCommandType.IsInstanceOfType(o))
            //	{
            //		FieldInfo m_dbCommandField = functionUpdateCommandType.GetField(
            //			"m_dbCommand", BindingFlags.Instance | BindingFlags.NonPublic);

            //		dbCommands.Add((DbCommand)m_dbCommandField.GetValue(o));
            //	}
            //	else if (dynamicUpdateCommandType.IsInstanceOfType(o))
            //	{
            //		MethodInfo createCommandMethod = dynamicUpdateCommandType.GetMethod(
            //			"CreateCommand", BindingFlags.Instance | BindingFlags.NonPublic);

            //		object[] methodParams = new object[]
            //		{
            //			updateTranslator,
            //			new Dictionary<int, object>()
            //		};

            //		dbCommands.Add((DbCommand)createCommandMethod.Invoke(o, methodParams));
            //	}
            //	else
            //	{
            //		throw new NotSupportedException("Unknown UpdateCommand Kind");
            //	}
            //}


            //StringBuilder traceString = new StringBuilder();
            //foreach (DbCommand command in dbCommands)
            //{
            //	traceString.AppendLine("=============== BEGIN COMMAND ===============");
            //	traceString.AppendLine();

            //	traceString.AppendLine(command.CommandText);
            //	foreach (DbParameter param in command.Parameters)
            //	{
            //		traceString.AppendFormat("{0} = {1}", param.ParameterName, param.Value);
            //		traceString.AppendLine();
            //	}

            //	traceString.AppendLine();
            //	traceString.AppendLine("=============== END COMMAND ===============");
            //}

            //return traceString.ToString();
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


        public static TimeSpan? NormalizeMs(this TimeSpan? time, bool forOnlineDB)
        {
            return time.HasValue ? NormalizeMs(time.Value, forOnlineDB) : time;
        }


        public static TimeSpan NormalizeMs(this TimeSpan time, bool forOnlineDB)
        {
            if (forOnlineDB)
            {   // Только так на сайте правильно отображается время, если милисекунд меньше 10
                return new TimeSpan(0,
                                      time.Hours,
                                      time.Minutes,
                                      time.Seconds,
                                      time.Milliseconds / 10);
            }
            else
            {
                return new TimeSpan(0,
                      time.Hours,
                      time.Minutes,
                      time.Seconds,
                      time.Milliseconds % 10 != 0
                          ? time.Milliseconds * 10 :
                          time.Milliseconds);
            }
        }
    }
}
