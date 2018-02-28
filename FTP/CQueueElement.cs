using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.FTP.SheetGenerators;

namespace DBManager.FTP
{
	public class CQueueItem
	{
		#region PCWbkFullPath
		private string m_PCWbkFullPath = null;

		public string PCWbkFullPath
		{
			get { return m_PCWbkFullPath; }
			set
			{
				if (m_PCWbkFullPath != value)
				{
					m_PCWbkFullPath = value;
				}
			}
		}
		#endregion


		#region FTPWbkFullPath
		private string m_FTPWbkFullPath = null;

		public string FTPWbkFullPath
		{
			get { return m_FTPWbkFullPath; }
			set
			{
				if (m_FTPWbkFullPath != value)
				{
					m_FTPWbkFullPath = value;
				}
			}
		}
		#endregion


		#region GeneratorTask
		private CFTPSheetGeneratorBase.CTask m_GeneratorTask = null;

		public CFTPSheetGeneratorBase.CTask GeneratorTask
		{
			get { return m_GeneratorTask; }
			set
			{
				if (m_GeneratorTask != value)
				{
					m_GeneratorTask = value;
				}
			}
		}
		#endregion
	}
}
