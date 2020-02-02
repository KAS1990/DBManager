using System;
using static DBManager.Scanning.XMLDataClasses.CAgeGroup;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class Int32Extensions
    {
        public static bool ToBool(this Int32 number)
        {
            return number != 0;
        }

        public static string ToLatinCapitalLetter(this Int32 number)
        {
            return ((char)('A' + number - 1)).ToString();
        }

        public static string CreateYearInString(this int year)
        {
            switch (year)
            {
                case (int)enEndYearSpecVals.AndElder:
                    return Properties.Resources.resAndElder;

                case (int)enEndYearSpecVals.AndYounger:
                    return Properties.Resources.resAndYounger;

                default:
                    return year.ToString();
            }
        }

        public static string CreateYearInString(this int? year)
        {
            if (year == null)
                return null;

            return year.Value.CreateYearInString();
        }
    }
}
