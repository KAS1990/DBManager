using System.IO;

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
