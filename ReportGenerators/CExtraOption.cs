using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBManager.Global;

namespace DBManager.ReportGenerators
{
	public class CExtraOption : INotifyPropertyChanged
	{
		#region id
		private readonly enRounds m_id = enRounds.None;

		public enRounds id
		{
			get { return m_id; }
		}
		#endregion
		
		
		#region Value
		private static readonly string ValuePropertyName = GlobalDefines.GetPropertyName<CExtraOption>(m => m.Value);

		private bool m_Value = false;

		public bool Value
		{
			get { return m_Value; }
			set
			{
				if (m_Value != value)
				{
					m_Value = value;
					OnPropertyChanged(ValuePropertyName);
				}
			}
		}
		#endregion


		
		#region Show
		private static readonly string ShowPropertyName = GlobalDefines.GetPropertyName<CExtraOption>(m => m.Show);

		public bool Show
		{
			get
			{
				if (GlobalDefines.ROUND_NAMES.ContainsKey((byte)id))
				{
					switch (id)
					{
						case enRounds.Qualif:
						case enRounds.Qualif2:
						case enRounds.Total:
							return true;

						case enRounds.OneEighthFinal:
						case enRounds.QuaterFinal:
						case enRounds.SemiFinal:
						case enRounds.Final:
							return false;

						default:
							return false;
					}
				}
				else
					return false;
			}
		}
		#endregion
				


		#region Name
		private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<CExtraOption>(m => m.Name);

		public string Name
		{
			get
			{
				if (GlobalDefines.ROUND_NAMES.ContainsKey((byte)id))
				{
					switch (id)
					{
						case enRounds.Qualif:
						case enRounds.Qualif2:
							return Properties.Resources.resOnlyStartList;

						case enRounds.OneEighthFinal:
						case enRounds.QuaterFinal:
						case enRounds.SemiFinal:
						case enRounds.Final:
							return null;

						case enRounds.Total:
							return Properties.Resources.resShowBallsInTotal;

						default:
							return null;
					}
				}
				else
					return null;
			}
		}
		#endregion


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		public virtual void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion


		public CExtraOption()
		{
		}

		public CExtraOption(enRounds id)
		{
			m_id = id;
			OnPropertyChanged(NamePropertyName);
			OnPropertyChanged(ShowPropertyName);
		}
	}
}
