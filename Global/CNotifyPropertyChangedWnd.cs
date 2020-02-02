using System.ComponentModel;
using System.Windows;

namespace DBManager.Global
{
    /// <summary>
    /// Базовый класс для всех окон, у которых необходимо реализовать интерфейс INotifyPropertyChanged
    /// </summary>
    public class CNotifyPropertyChangedWnd : Window, INotifyPropertyChanged
    {
        public CNotifyPropertyChangedWnd()
        {
            DataContext = this;
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
