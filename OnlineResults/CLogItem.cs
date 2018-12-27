using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using System.ComponentModel;

namespace DBManager.FTP
{
	public class CLogItem : INotifyPropertyChanged
	{
		#region Type
		private static readonly string TypePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.Type);

		private enFTPLogItemType m_Type = enFTPLogItemType.None;

		public enFTPLogItemType Type
		{
			get { return m_Type; }
			set
			{
				if (m_Type != value)
				{
					m_Type = value;
					OnPropertyChanged(TypePropertyName);
				}
			}
		}
		#endregion
				
		#region CreationDate
		private static readonly string CreationDatePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.CreationDate);
		private DateTime m_CreationDate = DateTime.Now;

		public DateTime CreationDate
		{
			get { return m_CreationDate; }
			set
			{
				if (m_CreationDate != value)
				{
					m_CreationDate = value;
					OnPropertyChanged(CreationDatePropertyName);
				}
			}
		}

		public string CreationDateInString
		{
			get { return CreationDate.ToString(); }
		}
		#endregion


		#region PCWbkName
		private static readonly string PCWbkNamePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.PCWbkName);
		private string m_PCWbkName = null;
		/// <summary>
		/// Название книги на ПК
		/// </summary>
		public string PCWbkName
		{
			get { return m_PCWbkName; }
			set
			{
				if (m_PCWbkName != value)
				{
					m_PCWbkName = value;
					OnPropertyChanged(PCWbkNamePropertyName);
				}
			}
		}
		#endregion


		#region FTPWbkFullPath
		private static readonly string FTPWbkFullPathPropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.FTPWbkFullPath);
		private string m_FTPWbkFullPath = null;
		/// <summary>
		/// Путь к книге на сервере
		/// </summary>
		public string FTPWbkFullPath
		{
			get { return m_FTPWbkFullPath; }
			set
			{
				if (m_FTPWbkFullPath != value)
				{
					m_FTPWbkFullPath = value;
					OnPropertyChanged(FTPWbkFullPathPropertyName);
				}
			}
		}
		#endregion


		#region Text
		private static readonly string TextPropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.Text);
		private string m_Text = null;
		/// <summary>
		/// Сообщение
		/// </summary>
		public string Text
		{
			get { return m_Text; }
			set
			{
				if (m_Text != value)
				{
					m_Text = value;
					OnPropertyChanged(TextPropertyName);
				}
			}
		}
		#endregion


		public string ToLogFileString()
		{
			// Переносы для удобства в логе не используем
			return Type.ToString() +
					GlobalDefines.FTP_LOG_FIELDS_SEPARATOR +
					CreationDate.ToString() +
					GlobalDefines.FTP_LOG_FIELDS_SEPARATOR +
					PCWbkName.Replace("\r", "").Replace('\n', GlobalDefines.FTP_LOG_LFCR_SYMBOL) +
					GlobalDefines.FTP_LOG_FIELDS_SEPARATOR +
					FTPWbkFullPath.Replace("\r", "").Replace('\n', GlobalDefines.FTP_LOG_LFCR_SYMBOL) + 
					GlobalDefines.FTP_LOG_FIELDS_SEPARATOR +
					Text.Replace("\r", "").Replace('\n', GlobalDefines.FTP_LOG_LFCR_SYMBOL);
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
