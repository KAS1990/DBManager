using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBManager.Global;
using DBManager.Commands;
using System.Collections.ObjectModel;

namespace DBManager.ReportGenerators
{
	public class CTeamForReportItem : INotifyPropertyChanged
	{
		#region Name
		private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CTeamForReportItem>(m => m.Name);

		private string m_Name = "";

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


		#region Subteams
		private ObservableCollection<CTeamItem> m_Subteams = new ObservableCollection<CTeamItem>();

		public ObservableCollection<CTeamItem> Subteams
		{
			get { return m_Subteams; }
		}
		#endregion


		#region Command
		private CCommand m_Command = null;

		public CCommand Command
		{
			get { return m_Command; }
			private set
			{
				if (m_Command != value)
					m_Command = value;
			}
		}
		#endregion


		public CTeamForReportItem()
		{
		}


		public CTeamForReportItem(TeamItemForReportCommandHandler commandFunc)
		{
			Command = new CCommand(() => { commandFunc(this); });
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


	/// <summary>
	/// Функция, которая вызывается при срабатывании команды.
	/// </summary>
	public delegate void TeamItemForReportCommandHandler(CTeamForReportItem sender);
}
