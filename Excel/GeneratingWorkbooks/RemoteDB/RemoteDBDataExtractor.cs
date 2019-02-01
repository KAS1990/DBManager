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

        public bool Extract(ICompDesc compDesc, IEnumerable<IGroupItem> compGroups, out string message)
        {
            message = null;

            CompDescRemoteDB remoteCompDesc = compDesc as CompDescRemoteDB;

            GroupsMembers = new List<KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>>();
            var entities = OnlineDBManager.Instance.Entities;

            foreach (var @group in compGroups.Cast<GroupItemRemoteDB>().Where(arg => arg.IsSelected))
            {
                try
                {
                    var pupils =
                        (from part in entities.participants
                         join pupil in entities.pupil on part.pupil_id equals pupil.id
                         where part.competition_id == remoteCompDesc.ID && part.group_id == @group.ID
                         select pupil)
                         .ToList();

                    var members = pupils
                                    .Select(pupil =>
                                            new CFullMemberInfo()
                                            {
                                                Surname = pupil.lastname,
                                                Name = pupil.firstname,
                                                YearOfBirth = pupil.age,
                                                SecondCol = pupil.team,
                                                InitGrade = (byte)((enOnlineGrade)pupil.rang_id).ToLocalGradeValue()
                                            })
                                    .ToList();
                    
                    GroupsMembers.Add(new KeyValuePair<IGroupItem, IEnumerable<CFullMemberInfo>>(@group, members));
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }                
            }

            if (GroupsMembers.Count == 0)
            {
                message = Properties.Resources.resNoGroupsToGenerateWorkbooks;
                return false;
            }

            var groupsWoMembers = GroupsMembers.Where(arg => arg.Value.Count() == 0);
            if (groupsWoMembers.Any())
            {
                message = string.Format(Properties.Resources.resfmtNoMembersInSomeGroups,
                                        string.Join(", ", groupsWoMembers.Select(arg => arg.Key.Name)));
                return false;
            }

            CompDesc = compDesc;
            return true;
        }
    }
}
