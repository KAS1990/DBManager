using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using DBManager.Global;
using DBManager.Global.Converters;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.RoundMembers.Converters
{
	public static class RouteResultsMarkupConverter
	{
		public static string Convert(CResult result)
		{
			if (result == null || result.Time == null)
				return "";
			if (result.AdditionalEventTypes.HasValue)
			{
				if (result.AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.DontAppear))
					return GlobalDefines.ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.DontAppear].short_name;
				else if (result.AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.Disqualif))
					return GlobalDefines.ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.Disqualif].short_name;
			}

			if (result.Time == GlobalDefines.FALL_TIME_SPAN_VAL)
				return Properties.Resources.resFall;
			else
			{
				// Иногда милисекунды приходят в формате 250, а иногда 25. А должно быть 250
				TimeSpan timeToConvert = new TimeSpan(0,
													result.Time.Value.Hours,
													result.Time.Value.Minutes,
													result.Time.Value.Seconds,
													result.Time.Value.Milliseconds % 10 != 0
														? result.Time.Value.Milliseconds * 10 :
														result.Time.Value.Milliseconds);

				if (result.Time > GlobalDefines.FALL_ON_ROUTE_2_TIME_SPAN_VAL)
				{   /* Участник сорвался на второй трассе =>
				 * мы конвертируем результат суммы двух трасс, т.к. время больше GlobalDefines.FALL_ON_ROUTE_2_TIME_SPAN_VAL */
					return timeToConvert.ToString(@"mm\:ss\,ff\*");
				}
				else
					return timeToConvert.ToString(@"mm\:ss\,ff");
			}
		}
	}
}
