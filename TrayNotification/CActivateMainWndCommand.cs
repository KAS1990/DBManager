using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DBManager.TrayNotification
{
	public class CActivateMainWndCommand : ICommand
	{
		public void Execute(object parameter)
		{
			DBManagerApp.MainWnd.WindowState = System.Windows.WindowState.Maximized;
			DBManagerApp.MainWnd.Activate();
		}

		
		public bool CanExecute(object parameter)
		{
			return true;
		}

		
		public event EventHandler CanExecuteChanged;
	}
}
