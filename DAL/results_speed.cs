using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System;

namespace DBManager
{
    public partial class results_speed
    {
        private enChangedResult UpdateResult(CResult ResultInXML,
                                        enChangedResult[] ChangedResults,
                                        ref TimeSpan? RouteInDB,
                                        ref byte? CondFormatingInDB,
                                        ref long? EventInDB)
        {
            enChangedResult result = enChangedResult.None;

            if (ResultInXML != null)
            {
                ResultInXML.ResultInDB = this;

                if (RouteInDB != ResultInXML && RouteInDB != ResultInXML.Time)
                {
                    RouteInDB = ResultInXML.Time;
                    result |= ChangedResults[0];
                }
                if (CondFormatingInDB != (byte?)ResultInXML.CondFormating)
                {
                    CondFormatingInDB = (byte?)ResultInXML.CondFormating;
                    result |= ChangedResults[1];
                }
                if (EventInDB != (long?)ResultInXML.AdditionalEventTypes)
                {
                    if (EventInDB.HasValue)
                        EventInDB = (EventInDB.Value & ~(long)(enAdditionalEventTypes.DontAppear | enAdditionalEventTypes.Disqualif | enAdditionalEventTypes.BeyondQualif)) | ((long?)ResultInXML.AdditionalEventTypes ?? 0);
                    else
                        EventInDB = (long?)ResultInXML.AdditionalEventTypes;

                    result |= ChangedResults[2];
                }
            }
            else
            {
                if (RouteInDB != null)
                {
                    RouteInDB = null;
                    result |= ChangedResults[0];
                }
                if (CondFormatingInDB != null)
                {
                    CondFormatingInDB = null;
                    result |= ChangedResults[1];
                }

                if (EventInDB.HasValue)
                {
                    EventInDB = EventInDB.Value & ~(long)(enAdditionalEventTypes.DontAppear | enAdditionalEventTypes.Disqualif | enAdditionalEventTypes.BeyondQualif);

                    result |= ChangedResults[2];
                }
            }

            if (EventInDB.HasValue && EventInDB == 0)
            {
                EventInDB = null;
                result |= ChangedResults[2];
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResultInXML"></param>
        /// <returns>Что поменялось</returns>
        public enChangedResult UpdateResults(CMember ResultInXML)
        {
            // Массивы нужны, т.к. невозможно передавать свойства через ref
            TimeSpan?[] RouteInDB = { route1, route2, sum };
            byte?[] CondFormatingInDB = { cond_formating_1, cond_formating_2, cond_formating_sum };
            long?[] EventInDB = { event_1, event_2, event_sum };
            enChangedResult[][] ChangedResults =
            {
                new enChangedResult[] { enChangedResult.Route1Time, enChangedResult.Route1CondFormatting, enChangedResult.Route1AdditionalEvent },
                new enChangedResult[] { enChangedResult.Route2Time, enChangedResult.Route2CondFormatting, enChangedResult.Route2AdditionalEvent },
                new enChangedResult[] { enChangedResult.SumTime, enChangedResult.SumCondFormatting, enChangedResult.SumAdditionalEvent }
            };

            enChangedResult result = UpdateResult(ResultInXML.Route1Ext,
                                                    ChangedResults[0],
                                                    ref RouteInDB[0],
                                                    ref CondFormatingInDB[0],
                                                    ref EventInDB[0])
                                        | UpdateResult(ResultInXML.Route2Ext,
                                                        ChangedResults[1],
                                                        ref RouteInDB[1],
                                                        ref CondFormatingInDB[1],
                                                        ref EventInDB[1])
                                        | UpdateResult(ResultInXML.SumExt,
                                                        ChangedResults[2],
                                                        ref RouteInDB[2],
                                                        ref CondFormatingInDB[2],
                                                        ref EventInDB[2]);

            route1 = RouteInDB[0];
            cond_formating_1 = CondFormatingInDB[0];
            event_1 = EventInDB[0];

            route2 = RouteInDB[1];
            cond_formating_2 = CondFormatingInDB[1];
            event_2 = EventInDB[1];

            sum = RouteInDB[2];
            cond_formating_sum = CondFormatingInDB[2];
            event_sum = EventInDB[2];
            return result;
        }


        public bool IsWinnerInPair(results_speed rhs)
        {
            return sum < rhs.sum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ClearResults()
        {
            route1 = route2 = sum = null;
        }


        public void ClearCondFormating()
        {
            cond_formating_1 = cond_formating_2 = cond_formating_sum = null;
        }


        public long? ClearAdditionalEvent(long? EventInDB, enResultColumnNumber Columns, enAdditionalEventTypes flags)
        {
            if (EventInDB.HasValue)
            {
                if (Columns.HasFlag(enResultColumnNumber.Route1))
                    EventInDB = EventInDB & ~(long)flags;
                if ((enAdditionalEventTypes?)EventInDB == enAdditionalEventTypes.None)
                    EventInDB = null;
            }

            return EventInDB;
        }


        public void ClearAdditionalEvents(enResultColumnNumber Columns, enAdditionalEventTypes flags)
        {
            event_1 = ClearAdditionalEvent(event_1, Columns, flags);
            event_2 = ClearAdditionalEvent(event_2, Columns, flags);
            event_sum = ClearAdditionalEvent(event_sum, Columns, flags);
        }
    }
}
