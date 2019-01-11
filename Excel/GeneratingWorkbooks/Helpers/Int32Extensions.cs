using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class Int32Extensions
    {
        public static bool ToBool(this Int32 number)
        {
            return number != 0;
        }
    }
}
