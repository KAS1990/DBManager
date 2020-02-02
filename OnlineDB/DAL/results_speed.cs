using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;

namespace DBManager.OnlineDB.Data
{
    public partial class results_speed
    {
        /// <summary>
        /// Этот конструктор я оставил на всякий случай
        /// </summary>
        public results_speed()
        {
        }

        public results_speed(CMemberAndResults localResult, string groupFullName, enRounds roundId)
        {
            UpdateFromLocalData(localResult);

            groups = groupFullName;
            round = GlobalDefines.ROUND_NAMES[(byte)roundId];
        }

        public bool IsEqualWithoutIdentificationProperties(CMemberAndResults rhs)
        {
            bool res = rhs.MemberInfo.Name == name
                        && rhs.MemberInfo.Surname == surname
                        && rhs.MemberInfo.InitGradeForShow == rang
                        && rhs.MemberInfo.YearOfBirth == age
                        && rhs.MemberInfo.SecondCol == team

                        && rhs.StartNumber == number
                        && rhs.Place == place

                        && rhs.Results.Route1.Time == route1
                        && rhs.Results.Route2.Time == route2
                        && rhs.Results.Sum.Time == sum;

            return res;
        }

        public void UpdateFromLocalData(CFullMemberInfo localMemberInfo)
        {
            local_member_id = (int)localMemberInfo.IDMember;
            name = localMemberInfo.Name;
            surname = localMemberInfo.Surname;
            rang = localMemberInfo.InitGradeForShow;
            age = localMemberInfo.YearOfBirth;
            team = localMemberInfo.SecondCol;
        }

        public void UpdateFromLocalData(COneRoundResults localResults)
        {
            route1 = localResults.Route1.Time.NormalizeMs(true);
            route2 = localResults.Route2.Time.NormalizeMs(true);
            sum = localResults.Sum.Time.NormalizeMs(true);
        }

        public void UpdateFromLocalData(CMemberAndResults localResult)
        {
            UpdateFromLocalData(localResult.MemberInfo);

            number = localResult.StartNumber ?? 0;
            place = (sbyte?)localResult.Place;

            UpdateFromLocalData(localResult.Results);
        }
    }
}
