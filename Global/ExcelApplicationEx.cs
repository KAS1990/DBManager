using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Global
{
    public class ExcelApplicationEx
    {
        public MSExcel.Application App;
        public bool NewAppCreated;
    }
}
