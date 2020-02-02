using DBManager.Global;
using System.ComponentModel;

namespace DBManager.ReportGenerators
{
    public class CTeamItem : INotifyPropertyChanged
    {
        #region id
        private readonly long m_id = 0;

        public long id
        {
            get { return m_id; }
        }
        #endregion


        #region Name
        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CTeamItem>(m => m.Name);

        private string m_Name = null;

        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NamePropertyName);
                }
            }
        }
        #endregion


        #region IsSelected
        public static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CTeamItem>(m => m.IsSelected);

        private bool m_IsSelected = false;

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


        public CTeamItem()
        {
        }


        public CTeamItem(long id)
        {
            m_id = id;
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public virtual void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
