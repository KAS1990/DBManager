﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace DBManager.Global
{
	/// <summary>
	/// Базовый класс для всех UserControl'ов, у которых необходимо реализовать интерфейс INotifyPropertyChanged
	/// </summary>
	public class CNotifyPropertyChangedUserCtrl : UserControl, INotifyPropertyChanged
	{
		public CNotifyPropertyChangedUserCtrl()
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
