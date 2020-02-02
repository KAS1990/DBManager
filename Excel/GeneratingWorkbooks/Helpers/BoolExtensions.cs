using System;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class BoolExtensions
    {
        public static Int32 ToInt32(this bool f)
        {
            return f ? 1 : 0;
        }
    }
}
