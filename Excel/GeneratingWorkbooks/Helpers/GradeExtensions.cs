using DBManager.Global;
using DBManager.OnlineDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks.Helpers
{
    public static class GradeExtensions
    {
        public static enGrade ToLocalGradeValue(this enOnlineGrade onlineGrade)
        {
            switch (onlineGrade)
            {
                case enOnlineGrade.WithoutGrade:
                    return enGrade.WithoutGrade;

                case enOnlineGrade.Young3:
                    return enGrade.Young3;

                case enOnlineGrade.Young2:
                    return enGrade.Young2;

                case enOnlineGrade.Young1:
                    return enGrade.Young1;

                case enOnlineGrade.Adult3:
                    return enGrade.Adult3;

                case enOnlineGrade.Adult2:
                    return enGrade.Adult2;

                case enOnlineGrade.Adult1:
                    return enGrade.Adult1;

                case enOnlineGrade.BeforeMaster:
                    return enGrade.BeforeMaster;

                case enOnlineGrade.Master:
                    return enGrade.Master;

                case enOnlineGrade.InternationalMaster:
                    return enGrade.None;

                case enOnlineGrade.None:
                default:
                    return enGrade.None;
            }
        }
    }
}
