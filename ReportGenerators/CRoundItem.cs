using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DBManager.Global;

namespace DBManager.ReportGenerators
{
	public class CRoundItem : INotifyPropertyChanged
	{
		#region id
		private readonly enRounds m_id = enRounds.None;

		public enRounds id
		{
			get { return m_id; }
		}
		#endregion


		#region RoundName
		private static readonly string RoundNamePropertyName = GlobalDefines.GetPropertyName<CRoundItem>(m => m.RoundName);

		public string RoundName
		{
			get
			{
				if (GlobalDefines.ROUND_NAMES.ContainsKey((byte)id))
					return GlobalDefines.ROUND_NAMES[(byte)id].Replace('_', ' ');
				else
					return null;
			}
		}
		#endregion


		#region SheetName
		private static readonly string SheetNamePropertyName = GlobalDefines.GetPropertyName<CRoundItem>(m => m.SheetName);

		private string m_SheetName = "";

		public string SheetName
		{
			get { return m_SheetName; }
			set
			{
				if (m_SheetName != value)
				{
					m_SheetName = value;
					OnPropertyChanged(SheetNamePropertyName);
				}
			}
		}
		#endregion


		#region IsSelected
		public static readonly string IsSelectedPropertyName = GlobalDefines.GetPropertyName<CRoundItem>(m => m.IsSelected);

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

				
		#region ExtraOption
		private static readonly string ExtraOptionPropertyName = GlobalDefines.GetPropertyName<CRoundItem>(m => m.ExtraOption);

		private CExtraOption m_ExtraOption = new CExtraOption();
		/// <summary>
		/// Дополнительная опция, зависящая от раунда
		/// </summary>
		public CExtraOption ExtraOption
		{
			get { return m_ExtraOption; }
			set
			{
				if (m_ExtraOption != value)
				{
					m_ExtraOption = value;
					OnPropertyChanged(ExtraOptionPropertyName);
				}
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


		public CRoundItem()
		{
		}

		public CRoundItem(enRounds id)
		{
			m_id = id;
			m_ExtraOption = new CExtraOption(m_id);
			OnPropertyChanged(RoundNamePropertyName);
		}
	}
}
