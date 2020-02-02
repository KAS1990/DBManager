using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System;

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
                TimeSpan timeToConvert = result.Time.Value.NormalizeMs(false);

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
