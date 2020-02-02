using DBManager.Global;
using DBManager.OnlineDB;
using System.Linq;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в таблице групп
    /// </summary>
    public class GroupItemRemoteDB : GroupItemBase
    {
        #region IsSelected
        private static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.IsSelected);
        private bool m_IsSelected = false;
        /// <summary>
        ///
        /// </summary>
        public bool IsSelected
        {
            get { return m_IsSelected; }
            set
            {
                if (m_IsSelected != value)
                {
                    m_IsSelected = value;
                    OnPropertyChanged(IsSelectedPropertyName);
                }
            }
        }
        #endregion

        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.ID);
        private int m_ID = 0;
        /// <summary>
        /// ID in remote DB
        /// </summary>
        public int ID
        {
            get { return m_ID; }
            set
            {
                if (m_ID != value)
                {
                    m_ID = value;
                    RefreshMembersCount();
                    OnPropertyChanged(IDPropertyName);
                }
            }
        }
        #endregion

        #region MembersCount
        private static readonly string MembersCountPropertyName = GlobalDefines.GetPropertyName<GroupItemRemoteDB>(m => m.MembersCount);
        private int? m_MembersCount = null;
        /// <summary>
        /// 
        /// </summary>
        public int MembersCount
        {
            get
            {
                if (m_MembersCount == null)
                    RefreshMembersCount();

                return m_MembersCount ?? 0;
            }
        }
        #endregion

        public GroupItemRemoteDB(CompDescRemoteDB compDesc) :
            base(compDesc)
        {
        }

        public GroupItemRemoteDB(GroupItemRemoteDB rhs) :
            base(rhs)
        {
            ID = rhs.ID;
        }

        public void RefreshMembersCount()
        {
            int compID = (CompDesc as CompDescRemoteDB)?.ID ?? 0;
            if (OnlineDBManager.Instance.IsConnectedToRemoteDB && compID != 0)
            {
                m_MembersCount = OnlineDBManager
                    .Instance
                    .Entities
                    .participants
                    .Count(arg => arg.group_id == ID
                            && arg.competition_id == compID
                            && arg.participants_kind.Any(kind => kind.kind_id == (int)enOnlineDBKind.Speed));
                OnPropertyChanged(MembersCountPropertyName);
            }
        }
    }
}
