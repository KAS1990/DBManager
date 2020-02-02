using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в выпадающем списке соревнований
    /// </summary>
    public class CompItemRemoteDB : INotifyPropertyChanged
    {
        #region Desc
        private static readonly string DescPropertyName = GlobalDefines.GetPropertyName<CompItemRemoteDB>(m => m.Desc);
        public ICompDesc Desc { get; set; } = null;
        #endregion

        #region Groups
        public ObservableCollection<GroupItemRemoteDB> Groups { get; private set; } = new ObservableCollection<GroupItemRemoteDB>();
        #endregion

        public CompItemRemoteDB()
        {
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
