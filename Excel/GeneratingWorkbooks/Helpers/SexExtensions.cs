using DBManager.Global;
using DBManager.OnlineDB;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class SexExtensions
    {
        public static enSex ToLocalSexValue(this enOnlineSex onlineSex)
        {
            switch (onlineSex)
            {
                case enOnlineSex.Female:
                    return enSex.Female;

                case enOnlineSex.Male:
                    return enSex.Male;

                case enOnlineSex.None:
                default:
                    return enSex.None;
            }
        }
    }
}
