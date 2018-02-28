using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.DBAdditionalDataClasses;
using System.Threading;
using System.ComponentModel;
using System.Net;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Global;
using DBManager.FTP.SheetGenerators;
using System.IO;

namespace DBManager.FTP
{
	public class CFTPExporter : IDisposable
	{
		const int REQUEST_TIMEOUT_MS = 3000;

		public const string TEMP_SHEET_NAME = "Sheet1";

		private bool m_Disposed = false;
		
		object m_csTasksToExport = new object();
		
		Queue<CQueueItem> m_quTasksToExport = new Queue<CQueueItem>();

		Thread m_thExporter = null;

		volatile bool m_ThreadGo = false;
		ManualResetEvent m_evHasData = new ManualResetEvent(false);

		MSExcel.Application m_excellApp = null;
		MSExcel.Workbook m_wbkPC = null;
		MSExcel.Workbook m_wbkTemplates = null;


		public int MaxQueueLength { get; set; }

		Dictionary<enFTPSheetGeneratorTypes, CFTPSheetGeneratorBase> m_dictGenerators = new Dictionary<enFTPSheetGeneratorTypes, CFTPSheetGeneratorBase>();

		/// <summary>
		/// Есть ли в текущей книге стартовый протокол
		/// </summary>
		public bool HasStartlist
		{
			get
			{
				if (m_wbkPC == null)
					return false;

				if (m_dictGenerators.ContainsKey(enFTPSheetGeneratorTypes.Start) ||
					m_wbkPC.Worksheets[1].Name == Properties.Resources.resStartList)
				{
					return true;
				}

				return false;
			}
		}


		private bool m_IsStarted = false;
		public bool IsStarted
		{
			get { return m_IsStarted; }
		}


		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_Disposed)
			{
				if (disposing)
				{
					// Free other state (managed objects).
					StopThread();

					foreach (KeyValuePair<enFTPSheetGeneratorTypes, CFTPSheetGeneratorBase> item in m_dictGenerators)
						item.Value.DetachFromSheet();

					if (m_excellApp != null)
					{
						try
						{
							while (m_excellApp.Workbooks.Count > 0)
								m_excellApp.Workbooks[1].Close(true);

							m_excellApp.Quit();
						}
						catch
						{ }

						m_excellApp = null;
					}

					m_dictGenerators.Clear();
				}
								
				// Free your own state (unmanaged objects).
				// Set large fields to null.
				m_Disposed = true;
			}
		}


		public CFTPExporter()
		{
			m_thExporter = new Thread(m_thExporter_ThreadProc)
			{
				IsBackground = false,
			};
			MaxQueueLength = 1;

			m_ThreadGo = true;
			m_thExporter.Start();
		}


		~CFTPExporter()
		{
			Dispose(false);
		}


		void m_thExporter_ThreadProc()
		{
			while (m_ThreadGo)
			{
				m_evHasData.WaitOne();
				if (!m_ThreadGo)
					break;

				CQueueItem Item = null;

				lock (m_csTasksToExport)
				{
					if (m_quTasksToExport.Count == 0)
					{
						m_evHasData.Reset();
						continue;
					}

					Item = m_quTasksToExport.Dequeue();
				}
					
				// Обработка полученного из очереди элемента
				HandleItem(Item);

				lock (m_csTasksToExport)
				{					
					if (m_quTasksToExport.Count == 0)
						m_evHasData.Reset();
				}
			}

			m_evHasData.Reset();
		}


		/// <summary>
		/// Эту функцию нужно обязательно вызывать перед закрытием приложения.
		/// Без этого поток нормально не завершится
		/// </summary>
		void StopThread()
		{
			Stop();
			m_ThreadGo = m_IsStarted = false;
			m_evHasData.Set();
			m_thExporter.Join();
		}


		public void Start()
		{
			ClearQueue();
			m_IsStarted = true;
		}


		public void Stop()
		{
			lock (m_csTasksToExport)
			{
				ClearQueue();

				try
				{
					if (m_wbkTemplates != null)
						m_wbkTemplates.Close();
				}
				catch
				{ }

				try
				{
					if (m_wbkPC != null)
						m_wbkPC.Close(true);
				}
				catch
				{ }
				m_wbkPC = m_wbkTemplates = null;
			}
			m_IsStarted = false;
		}


		/// <summary>
		/// Обработка 1 элемента
		/// </summary>
		/// <param name="Item"></param>
		public bool HandleItem(CQueueItem Item)
		{
			DBManagerApp.MainWnd.ExportingToFTPNow = true;

			CLogItem LogItem = new CLogItem()
			{
				CreationDate = DateTime.Now,
				FTPWbkFullPath = Item.FTPWbkFullPath,
				PCWbkName = Item.PCWbkFullPath
			};

			try
			{
				m_excellApp.ScreenUpdating = false;
				m_excellApp.EnableEvents = false;
				m_excellApp.DisplayAlerts = false;
			}
			catch
			{	// Возможно закрыли Excel
				try
				{
					m_excellApp.Quit();
				}
				catch
				{ }
				m_excellApp = null;
			}

			if (m_excellApp == null)
			{
				foreach (KeyValuePair<enFTPSheetGeneratorTypes, CFTPSheetGeneratorBase> item in m_dictGenerators)
					item.Value.DetachFromSheet();
				m_dictGenerators.Clear();
				
				m_excellApp = new MSExcel.Application()
				{
					Visible = true,
					ScreenUpdating = false,
					EnableEvents = false,
					DisplayAlerts = false
				};
			}

			if (m_wbkTemplates == null)
			{
				try
				{
					m_wbkTemplates = m_excellApp.Workbooks[GlobalDefines.STD_APP_CONFIGS_DIR +
															"\\" +
															DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.FTPTemplatesWbkName];
				}
				catch
				{	// Книги нет => обработка записи невозможно 
					try
					{
						m_wbkTemplates = m_excellApp.Workbooks.Open(GlobalDefines.STD_APP_CONFIGS_DIR +
																	"\\" +
																	DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.FTPTemplatesWbkName);
					}
					catch
					{
						m_excellApp.DisplayAlerts = true;
						m_excellApp.EnableEvents = true;
						m_excellApp.ScreenUpdating = true;
						ClearExcelFields();
						DBManagerApp.MainWnd.ExportingToFTPNow = false;
						return false;
					}
				}
			}

			if (m_wbkPC != null && m_wbkPC.FullName != Item.PCWbkFullPath)
			{
				foreach (KeyValuePair<enFTPSheetGeneratorTypes, CFTPSheetGeneratorBase> item in m_dictGenerators)
					item.Value.DetachFromSheet();
				m_dictGenerators.Clear();

				m_wbkPC.Save();
				m_wbkPC = null;
			}

			if (m_wbkPC == null)
			{
				try
				{
					m_wbkPC = m_excellApp.Workbooks[Item.PCWbkFullPath];
				}
				catch
				{	// Книга не открыта => открываем её
					try
					{
						m_wbkPC = m_excellApp.Workbooks.Open(Item.PCWbkFullPath);
					}
					catch
					{	// Книги нет => создаём её
						m_wbkPC = m_excellApp.Workbooks.Add(MSExcel.XlWBATemplate.xlWBATWorksheet);
					}
				}
			}
			
			// Книга создана, если в ней есть хотя бы один лист, в названии которого есть слово "Лист"
			bool CreateWorkbook = false;
			foreach (MSExcel.Worksheet wsh in m_wbkPC.Worksheets)
			{
				if (wsh.Name.Contains("Лист") &&
					wsh.Visible == MSExcel.XlSheetVisibility.xlSheetVisible)
				{
					CreateWorkbook = true;
			        break;
				}
			}

			if (CreateWorkbook)
			{	//Оставляем только 1 скрытый лист
				while (m_wbkPC.Worksheets.Count > 1)
					m_wbkPC.Worksheets[1].Delete();

				// Чтобы не было повторяющихся листов
				m_wbkPC.Worksheets[1].Name = TEMP_SHEET_NAME;
			}
												
			CFTPSheetGeneratorBase Generator;
			if (!m_dictGenerators.TryGetValue(Item.GeneratorTask.m_GeneratorType, out Generator))
				Generator = CFTPSheetGeneratorBase.GetGeneratorClass(Item.GeneratorTask.m_GeneratorType);
			try
			{
				Generator.AddDataToWbk(m_excellApp, m_wbkPC, m_wbkTemplates, Item.GeneratorTask);
			}
			catch (Exception ex)
			{
				LogItem.Type = enFTPLogItemType.Error;
				LogItem.Text = string.Format("Error in AddDataToWbk:\n{0}", ex.Message);
				AddItemToLog(LogItem, Item);
				ClearExcelFields();
				DBManagerApp.MainWnd.ExportingToFTPNow = false;
				return false;
			}

			if (CreateWorkbook)
			{
				if (m_wbkPC.Worksheets.Count > 1)
				{	// Удаляем лист с именем TEMP_SHEET_NAME
					try
					{
						m_wbkPC.Worksheets[TEMP_SHEET_NAME].Delete();
					}
					catch
					{ }
				}

				try
				{
					m_wbkPC.SaveAs(Item.PCWbkFullPath, MSExcel.XlFileFormat.xlAddIn);
				}
				catch (Exception ex)
				{
					m_excellApp.DisplayAlerts = true;
					m_excellApp.EnableEvents = true;
					m_excellApp.ScreenUpdating = true;
					ClearExcelFields();
					DBManagerApp.MainWnd.ExportingToFTPNow = false;
					return false;
				}
			}
			else
			{
				try
				{
					m_wbkPC.Save();
				}
				catch (Exception ex)
				{
					m_excellApp.DisplayAlerts = true;
					m_excellApp.EnableEvents = true;
					m_excellApp.ScreenUpdating = true;
					ClearExcelFields();
					DBManagerApp.MainWnd.ExportingToFTPNow = false;
					return false;
				}
			}
			m_wbkPC.Close(); // Без этого нельзя будет скопировать файл на сервер
			m_wbkPC = null;

			m_excellApp.DisplayAlerts = true;
			m_excellApp.EnableEvents = true;
			m_excellApp.ScreenUpdating = true;

			try
			{
				bool result = SendWbkToServer(DBManagerApp.m_AppSettings.m_Settings.FTPHost,
												DBManagerApp.m_AppSettings.m_Settings.FTPUsername,
												DBManagerApp.m_AppSettings.m_Settings.FTPPassword,
												Item);
				DBManagerApp.MainWnd.ExportingToFTPNow = false;
				return result;
			}
			catch (Exception ex)
			{
				LogItem.Type = enFTPLogItemType.Error;
				LogItem.Text = string.Format("Error in SendWbkToServer:\n{0}", ex.Message);
				AddItemToLog(LogItem, Item);
				DBManagerApp.MainWnd.ExportingToFTPNow = false;
				return false;
			}
		}


		public bool AddItemToQueue(CQueueItem Item)
		{
			lock (m_csTasksToExport)
			{
				if (m_quTasksToExport.Count < MaxQueueLength)
				{
					m_quTasksToExport.Enqueue(Item);
					m_evHasData.Set();
					return true;
				}
				else
					return false;
			}
		}


		public void ClearQueue()
		{
			lock (m_csTasksToExport)
			{
				m_evHasData.Reset();
				m_quTasksToExport.Clear();
			}
		}


		bool SendWbkToServer(string FTPHost, string FTPUser, string FTPPassword, CQueueItem Item)
		{
			CLogItem LogItem = new CLogItem()
			{
				CreationDate = DateTime.Now,
				FTPWbkFullPath = Item.FTPWbkFullPath,
				PCWbkName = Item.PCWbkFullPath
			};

			if (!CheckFtpDirectory(System.IO.Path.GetDirectoryName(Item.FTPWbkFullPath), FTPHost, FTPUser, FTPPassword))
			{	// Не удалось создать папку => пишем об этом в лог
				LogItem.Type = enFTPLogItemType.Error;
				LogItem.Text = string.Format("Can't create folder \"{0}\" on FTP server",
												System.IO.Path.GetDirectoryName(Item.FTPWbkFullPath));
				AddItemToLog(LogItem, Item);
				return false;
			}

			FtpWebResponse response = null;
			NetworkCredential Credential = new NetworkCredential(FTPUser, FTPPassword);

			string FullServerPath = CombineFTPPath(FTPHost, Item.FTPWbkFullPath);
			
			// Проверяем наличие файла на сервере
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FullServerPath);
			request.Credentials = Credential;
			request.Timeout = REQUEST_TIMEOUT_MS;
			request.Method = WebRequestMethods.Ftp.DownloadFile;
			try
			{
				// Выполняем команду на сервере
				using (response = (FtpWebResponse)request.GetResponse());

				// Файл есть на сервере => переименовываем его
				request = (FtpWebRequest)WebRequest.Create(FullServerPath);
				request.Credentials = Credential;
				request.Timeout = REQUEST_TIMEOUT_MS;
				request.Method = WebRequestMethods.Ftp.Rename;
				request.RenameTo = System.IO.Path.GetFileName(Item.FTPWbkFullPath) + ".tmp"; // Имя файла должно выглядеть именно так
				try
				{
					// Выполняем команду на сервере
					using (response = (FtpWebResponse)request.GetResponse());
				}
				catch (Exception ex)
				{	// Не удалось переименовать файл  => добавляем запись в лог
					LogItem.Type = enFTPLogItemType.Error;
					LogItem.Text = string.Format("Can't rename workbook on FTP server to \"{0}\". Error:\n{1}", request.RenameTo, ex.Message);
					AddItemToLog(LogItem, Item);
					return false;
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.Timeout)
					return false;
			}
			catch (Exception ex)
			{	// Файла нет
				
			}

			// Теперь пишем файл
			request = (FtpWebRequest)WebRequest.Create(CombineFTPPath(FTPHost, Item.FTPWbkFullPath));
			request.Credentials = Credential;
			
			// Поток для загрузки файла 
			using (FileStream uploadedFile = File.OpenRead(Item.PCWbkFullPath))
			{
				try
				{
					request.Timeout = REQUEST_TIMEOUT_MS;
					request.Method = WebRequestMethods.Ftp.UploadFile;

					byte [] RBuf = new byte[10 * 1024]; // Будем писать файл такими кусками, чтобы не закончилась оперативка
					using (Stream writer = request.GetRequestStream())
					{
						while (true)
						{
							int BytesReaded = uploadedFile.Read(RBuf, 0, RBuf.Length);

							if (BytesReaded > 0)
								writer.Write(RBuf, 0, BytesReaded);

							if (BytesReaded < RBuf.Length)
							{	// файл закончился
								break;
							}
						}
					}

					// Выполняем передачу данных на сервер
					using (response = (FtpWebResponse)request.GetResponse());

					// Удаляем старый файл
					request = (FtpWebRequest)WebRequest.Create(FullServerPath + ".tmp");
					request.Credentials = Credential;
					request.Timeout = REQUEST_TIMEOUT_MS;
					request.Method = WebRequestMethods.Ftp.DeleteFile;
					try
					{
						// Выполняем команду на сервере
						using (response = (FtpWebResponse)request.GetResponse());
					}
					catch (WebException ex)
					{
						if (ex.Status == WebExceptionStatus.Timeout)
							return false;
					}
					catch (Exception ex)
					{
					}
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.Timeout)
						return false;
				}
				catch (Exception ex)
				{	// Не удалось записать файл => добавляем запись в лог
					LogItem.Type = enFTPLogItemType.Error;
					LogItem.Text = string.Format("Can't write workbook \"{0}\" to FTP server or read workbook \"{1}\" from PC. Error:\n{1}",
													Item.FTPWbkFullPath,
													Item.PCWbkFullPath,
													ex.Message);
					AddItemToLog(LogItem, Item);
					return false;
				}
			}

			// Запись прошла успешно => добавляем запись в лог
			LogItem.Type = enFTPLogItemType.OK;
			LogItem.Text = string.Format("Workbook \"{0}\" has written to FTP server", Item.PCWbkFullPath);
			AddItemToLog(LogItem, Item);

			return true;
		}


		/// <summary>
		/// Проверка наличия папки на сервере и создания её в случае её отсутствия
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="ftpUser"></param>
		/// <param name="ftpPassword"></param>
		/// <returns></returns>
		public static bool CheckFtpDirectory(string directoryPath, string ftpHost, string ftpUser, string ftpPassword)
		{
			FtpWebResponse response = null;
			NetworkCredential Credential = new NetworkCredential(ftpUser, ftpPassword);
			bool IsExists = true;

			try
			{
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(CombineFTPPath(ftpHost, directoryPath));
				request.Credentials = Credential;
				request.Timeout = REQUEST_TIMEOUT_MS;
				request.Method = WebRequestMethods.Ftp.ListDirectory;

				// Выполняем команду на сервере
				using (response = (FtpWebResponse)request.GetResponse());
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.Timeout)
					return false;
				using (response = (FtpWebResponse)ex.Response)
				{
					if (response != null && response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
					{	// Папки нет => создаём её
						IsExists = false;
					}
				}
			}

			if (!IsExists)
			{
				string TempPath = CombineFTPPath(ftpHost, "");
				// Проходим все папки в пути и создаём те, которых нет
				foreach (string Dir in directoryPath.Split('/'))
				{
					if (string.IsNullOrWhiteSpace(Dir))
						continue;

					TempPath += Dir + "/";

					try
					{
						FtpWebRequest request = (FtpWebRequest)WebRequest.Create(TempPath);
						request.Credentials = Credential;
						request.Timeout = REQUEST_TIMEOUT_MS;
						request.Method = WebRequestMethods.Ftp.ListDirectory;
						
						// Выполняем команду на сервере
						using (response = (FtpWebResponse)request.GetResponse());
					}
					catch (WebException ex)
					{
						if (ex.Status == WebExceptionStatus.Timeout)
							return false;

						response = (FtpWebResponse)ex.Response;
						if (response != null && response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
						{	// Папки нет => создаём её
							response.Close();

							try
							{
								FtpWebRequest request = (FtpWebRequest)WebRequest.Create(TempPath);
								request.Credentials = Credential;
								request.Timeout = REQUEST_TIMEOUT_MS;
								request.Method = WebRequestMethods.Ftp.MakeDirectory;
								
								// Выполняем команду на сервере
								using (response = (FtpWebResponse)request.GetResponse());
							}
							catch (WebException exSub)
							{	// не удалось создать папку
								return false;
							}
						}
						else
							response.Close();
					}
				}
			}

			return true;
		}


		void AddItemToLog(CLogItem LogItem, CQueueItem Item)
		{
			GlobalDefines.CheckFTPDirExists();

			string Dir = GlobalDefines.STD_FTP_LOG_DIR + Item.GeneratorTask.m_CompId.ToString() + "\\";
			if (!Directory.Exists(Dir))
				Directory.CreateDirectory(Dir);

			try
			{
				using (TextWriter tw = new StreamWriter(string.Format("{0}{1}\\{2}{3}",
																	GlobalDefines.STD_FTP_LOG_DIR,
																	Item.GeneratorTask.m_CompId,
																	Item.GeneratorTask.m_GroupId,
																	GlobalDefines.FTP_LOG_FILE_EXTENSION), true))
				{
					tw.WriteLine(LogItem.ToLogFileString());
				}
			}
			catch (Exception ex)
			{
			}
		}


		void ClearExcelFields()
		{
			try
			{
				m_wbkPC.Close(false);
			}
			catch
			{ }
			try
			{
				m_wbkTemplates.Close();
			}
			catch
			{ }
			m_wbkPC = m_wbkTemplates = null;
			m_excellApp = null;
		}


		public static string CombineFTPPath(string FTPHost, string FullPath)
		{
			return Uri.UriSchemeFtp +
					Uri.SchemeDelimiter +
					"www." +
					FTPHost +
					"/" +
					FullPath.Replace('\\', '/') +
					(System.IO.Path.HasExtension(FullPath) ? "" : "/");
		}
	}
}
