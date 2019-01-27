using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class DirectoryInfoExtensions
    {
        public static void ClearDirectory(this DirectoryInfo dir)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                di.ClearDirectory();
                di.Delete();
            }
        }
    }
}
