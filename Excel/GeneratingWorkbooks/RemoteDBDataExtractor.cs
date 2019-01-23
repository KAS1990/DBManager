using DBManager.Excel.GeneratingWorkbooks.Helpers;
using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.OnlineDB;
using DBManager.Scanning.DBAdditionalDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class RemoteDBDataExtractor : IDataExtractor
    {
        public ICompDesc CompDesc { get; private set; }
        public List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>> GroupsMembers { get; private set; }

        public bool Extract(CompDesc compDesc, IEnumerable<GroupItemRemoteDB> compGroups)
        {
            GroupsMembers = new List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>>();
            var entities = OnlineDBManager.Instance.Entities;

            foreach (var @group in compGroups.Where(arg => arg.IsSelected))
            {
                try
                {
                    var members =
                        (from part in entities.participants
                         join pupil in entities.pupil on part.pupil_id equals pupil.id
                         where part.competition_id == compDesc.ID && part.group_id == @group.ID
                         select new CFullMemberInfo()
                         {
                             Surname = pupil.lastname,
                             Name = pupil.firstname,
                             YearOfBirth = pupil.age,
                             SecondCol = pupil.team,
                             InitGrade = Convert.ToByte(pupil.rang_id)
                         })
                         .ToList();
                    members.ForEach(arg => arg.InitGrade = (byte)((enOnlineGrade)arg.InitGrade).ToLocalGradeValue());

                    GroupsMembers.Add(new KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>(@group, members));
                }
                catch
                {
                    return false;
                }                
            }

            if (GroupsMembers.Count == 0)
                return false;

            return true;
        }
    }
}
