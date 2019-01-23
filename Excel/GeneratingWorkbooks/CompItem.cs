using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    /// <summary>
    /// Запись в выпадающем списке соревнований
    /// </summary>
    public class CompItem : INotifyPropertyChanged
    {
        #region Desc
        private static readonly string DescPropertyName = GlobalDefines.GetPropertyName<CompItem>(m => m.Desc);
        public CompDesc Desc { get; } = new CompDesc();
        #endregion


        #region Groups
        public ObservableCollection<GroupItem> Groups { get; private set; } = new ObservableCollection<GroupItem>();
        #endregion

        public CompItem()
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
