using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DBManager.Global;
using System.IO;

namespace DBManager.Scanning
{
	public abstract class CScannerBase
	{
		public abstract class CSyncParamBase
		{

		}


		object m_EventsCS = new object();
		/// <summary>
		/// Объект синхронизаии, используемый в обработчиках событий от m_PathWatcher
		/// </summary>
		protected object EventsCS
		{
			get { return m_EventsCS; }
		}


		CScannerBase m_Parent = null;
		protected CScannerBase Parent
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		
		private enScanningThreadState m_State = enScanningThreadState.Stopped;
		/// <summary>
		/// Состояние работы потока
		/// </summary>
		public enScanningThreadState State
		{
			get { return m_State; }
			protected set { m_State = value; }
		}


		protected string m_ScanningPath = "";
		/// <summary>
		/// Полный путь к файлу и папке, с которыми работает поток
		/// </summary>
		public virtual string ScanningPath
		{
			get { return m_ScanningPath; }
			set { m_ScanningPath = value; }
		}


		private bool m_SyncSuccessfully = false;
		public bool SyncSuccessfully
		{
			get { return m_SyncSuccessfully; }
			protected set { m_SyncSuccessfully = value; }
		}


		#region LastException
		Exception m_LastException = null;
		public Exception LastException
		{
			get { return m_LastException; }
			protected set
			{
				m_LastException = value;
				if (m_LastException == null)
					LastDataChangedEventArgs = null;
			}
		}
		#endregion


		#region LastDataChangedEventArgs
		DataChangedEventArgs m_LastDataChangedEventArgs = null;
		public DataChangedEventArgs LastDataChangedEventArgs
		{
			get { return m_LastDataChangedEventArgs; }
			private set { m_LastDataChangedEventArgs = value; }
		}
		#endregion

		
				
		/// <summary>
		/// Событие вызываемое, когда меняется объект, который сканирует класс
		/// </summary>
		public event DataChangedEventHandler DataChanged;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="Path">
		/// Путь к файлу или папке
		/// </param>
		/// <param name="ParentScanner">
		/// 
		/// </param>
		/// <param name="Sync">
		/// Нужно ли выполнить синхронизацию объекта с БД и файлами, уже имеющимися в каталоге.
		/// При этом вызывается функция SyncWithFilesAndDB, а Start не вызавается
		/// </param>
		/// <param name="SyncParam">
		/// Параметр, необходимый для функции SyncWithFilesAndDB
		/// </param>
		public CScannerBase(string Path,
							CScannerBase ParentScanner,
							bool Sync,
							CSyncParamBase SyncParam = null)
		{
			Parent = ParentScanner;

			// Выполняем синхронизацию или запускаем сканирование
			if (Sync)
				SyncSuccessfully = SyncWithFilesAndDB(SyncParam);
			else
				Start(Path);
		}


		~CScannerBase()
		{
			Stop(false);
		}


		/// <summary>
		/// Запустить сканирование
		/// </summary>
		/// <param name="Path">
		/// Путь к файлу или папке, которую сканирует поток
		/// </param>
		/// <returns></returns>
		public abstract bool Start(string Path);


		/// <summary>
		/// Остановить сканирование
		/// </summary>
		/// <param name="OnRestart">
		/// true - функция вызвана из метода Restart
		/// </param>
		public abstract void Stop(bool OnRestart);


		public virtual bool Restart(string Path, CSyncParamBase SyncParam)
		{
			Stop(true);
			if (SyncWithFilesAndDB(SyncParam))
				return Start(Path); // Запускаем сканирование 

			return false;
		}


		/// <summary>
		/// Синхронизация сканера с БД и файлами.
		/// Эта функция нужна в том случае, если в xml-файлах были сделаны какие-то изменения в тот момент,
		/// когда данная программа была закрыта
		/// </summary>
		/// <param name="Param"></param>
		public abstract bool SyncWithFilesAndDB(CSyncParamBase Param);


		public virtual void RaiseDataChangedEvent(DataChangedEventArgs e)
		{
			LastDataChangedEventArgs = e;
			if (DataChanged != null)
				DataChanged(this, e);
		}


		object m_scStackOvDetetion = new object();
		protected void OnException(ref List<CDataChangedInfo> lst, Exception ex, long id)
		{
			if (!Monitor.TryEnter(m_scStackOvDetetion))
			{	// Предотвращаем бесконечную рекурсию
				return;
			}
			
			try
			{
				Stop(false);
			}
			catch // Т.к. в методе Stop тоже может быть exception, что приведёт к StackOverflow
			{ }
			lst.Clear();
			lst.Add(new CDataChangedInfo(this)
			{
				ChangedObjects = enDataChangedObjects.Exception,
				ChangingType = enDataChangesTypes.ExceptionInScanner,
				Argument = LastException = ex,
				ID = id
			});

			try
			{
				using (TextWriter sw = new StreamWriter(GlobalDefines.STD_ERROR_LOG_FILE_PATH, true))
				{
					sw.WriteLine(string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()));

					int ExceptionNum = 1;
					Exception IntEx = ex;
					while (IntEx != null)
					{
						WriteExceptionToFile(IntEx, sw, ExceptionNum++);
						IntEx = IntEx.InnerException;
					}

					sw.WriteLine(GlobalDefines.LOG_EXCEPTION_TERMINAL_LINE);
				}
			}
			catch
			{ }

			Monitor.Exit(m_scStackOvDetetion);
		}


		private void AddStackToFile(Exception ex, TextWriter sw)
		{
			if (sw == null)
				return;

			if (ex != null)
			{
				const int STACK_TRACE_WRITE_PORTION = 1000;

				sw.WriteLine("StackTrace: ");

				/* Записываем кадр стека в файл порциями, потому что StreamWriter может писать данные в файл кусками ограниченного размера.
				 * Размер ограничения узнать не удалось, поэтому выбрал STACK_TRACE_WRITE_PORTION, чтобы точно было меньше */
				char[] arrStackTrace = ex.StackTrace.ToCharArray();
				int BytesWritten = 0;
				while (BytesWritten < arrStackTrace.Length)
				{
					if (BytesWritten + STACK_TRACE_WRITE_PORTION < arrStackTrace.Length)
						sw.Write(arrStackTrace, BytesWritten, STACK_TRACE_WRITE_PORTION);
					else
						sw.Write(arrStackTrace, BytesWritten, arrStackTrace.Length - BytesWritten);
					BytesWritten += STACK_TRACE_WRITE_PORTION;
					sw.Flush();
				}
				sw.WriteLine("");
			}
			else
			{
				sw.Write("ex == null\n");
				sw.Flush();
			}
		}


		private void WriteExceptionToFile(Exception ex, TextWriter sw, int ExceptionNum)
		{
			if (ex != null)
			{
				sw.WriteLine("\n" + GlobalDefines.LOG_EXCEPTION_START_LINE + ExceptionNum.ToString());
				sw.WriteLine("Message: " + ex.Message);
				sw.WriteLine("Method: " + ex.TargetSite.ToString());
				sw.WriteLine("Source: " + ex.Source);
				if (ex.Data != null)
				{
					sw.WriteLine("Data: ");
					foreach (object val in ex.Data)
						if (val is System.Collections.DictionaryEntry)
						{
							System.Collections.DictionaryEntry DictEntry = (System.Collections.DictionaryEntry)val;
							sw.WriteLine(string.Format("\tSystem.Collections.DictionaryEntry: Key = {0}, value = {1}",
														DictEntry.Key.ToString(),
														DictEntry.Value == null ? "null" : DictEntry.Value.ToString()));
						}
						else
							sw.WriteLine("\t" + val.ToString());
				}
				else
					sw.WriteLine("Data = null");

				AddStackToFile(ex, sw);
			}
			else
			{
				sw.Write("ex == null");
				sw.Flush();
			}
		}
	}
}
