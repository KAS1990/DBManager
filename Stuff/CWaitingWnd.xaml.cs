﻿using System.Windows;
using System.Threading;
using System;
using DBManager;
using System.Windows.Threading;
using System.Windows.Interop;
using System.ComponentModel;
using DBManager.TrayNotification;
using DBManager.Global;

namespace DBManager.Stuff
{
	/// <summary>
	/// Логика взаимодействия для CWaitingWnd.xaml
	/// </summary>
	public partial class CWaitingWnd : Window
	{
		public const int TimerIntervalInMs = 300;

		bool m_AllowClose = false;
		AutoResetEvent m_CloseEvent = null;
		Window m_OwnerWindow = null;
		
		int m_RemTimersCountForShow = 0;
		IntPtr m_hwnd = new IntPtr();
		IntPtr m_OldParent = new IntPtr();
		double m_OldTop = 0;
		double m_OldLeft = 0;
		
		
		public CWaitingWnd()
		{
			InitializeComponent();
		}


		public void ClearOwner()
		{
			m_OwnerWindow = null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="CloseEvent"></param>
		/// <param name="OwnerWindow"></param>
		/// <param name="Prompt"></param>
		/// <param name="WndTitle"></param>
		/// <param name="ShowingPauseInMs">
		/// Через сколько милисекунд отобразиться окно. Нужно задавать значения кратные TimerIntervalInMs
		/// </param>
		public CWaitingWnd(AutoResetEvent CloseEvent, Window OwnerWindow, string Prompt, string WndTitle, int ShowingPauseInMs = 0)
		{
			InitializeComponent();

			lblPleaseWait.Content = Properties.Resources.resPleaseWait; /* В xaml делать вывод этого текста (как обычно {Loc resPleaseWait}) нельзя,
																		 * т.к. в этом случае почему-то LocalizationManager привязывается к другому потоку и
																		 * больше не даёт менять свой язык */
			Title = WndTitle;
			txtblkPrompt.Text = Prompt;

			m_CloseEvent = CloseEvent;
			m_OwnerWindow = OwnerWindow;
			
			CTaskBarIconTuning.SetProgressState(enTaskbarStates.Indeterminate);

			DispatcherTimer tmrSearching = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, TimerIntervalInMs) };
			tmrSearching.Tick += (s, ev) =>
			{
				if (m_RemTimersCountForShow-- == 0)
				{
					MyWin32Interop.SetParent(m_hwnd, m_OldParent);
					Top = m_OldTop;
					Left = m_OldLeft;
					Activate();
				}

				if (m_CloseEvent.WaitOne(0))
				{
					tmrSearching.Stop();
					m_AllowClose = true;
					Close();

					if (OwnerWindow != null && Application.Current != null && DBManagerApp.Current.Dispatcher != null)
						ThreadManager.Instance.InvokeUI(new Action(() =>
						{
							// Этот try...catch обязателен, т.к. без него вылетим в ошибку при перезагрузке программы при смене конфигурации
							try
							{
								if (OwnerWindow != DBManagerApp.MainWnd || !OwnerWindow.IsLoaded)
								{
									if (!OwnerWindow.IsLoaded)
										DBManagerApp.MainWnd.Activate();
								}
								if (OwnerWindow.IsLoaded)
								{
									OwnerWindow.Activate();
								}
							}
							catch
							{ }
						}));
				}
			};
			m_RemTimersCountForShow = ShowingPauseInMs / (int)Math.Max(1, tmrSearching.Interval.TotalMilliseconds);
			tmrSearching.Start();
		}


		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !m_AllowClose;

			if (!e.Cancel)
			{
				if (m_csShowCounter > 0)
					m_csShowCounter--;
				CTaskBarIconTuning.ResetProgressValue();
				CTaskBarIconTuning.Flash();
			}
			
			base.OnClosing(e);
		}


		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;

			if (hwndSource != null)
			{
				m_hwnd = hwndSource.Handle;
				m_OldParent = MyWin32Interop.SetParent(m_hwnd, (IntPtr)MyWin32Interop.HWND_MESSAGE);
				m_OldTop = Top;
				m_OldLeft = Left;
				Left = 10000;
				Top = 10000;
			}
		}


		class CShowAsyncParam
		{
			public AutoResetEvent m_hFinishedSearchEvent;
			public string m_WndTitle;
			public string m_WndText;
		}

		static int? m_csShowCounter = 0;
		/// <summary>
		/// Запуск формы в другом потоке
		/// </summary>
		/// <param name="hFinishedSearchEvent">
		/// Событие, установка которого закрывает окно
		/// </param>
		/// <param name="th">
		/// Поток, в котором создаётся окно
		/// </param>
		/// <param name="WndTitle"></param>
		/// <param name="WndText"></param>
		public static void ShowAsync(out AutoResetEvent hFinishedSearchEvent, out Thread th, string WndTitle, string WndText)
		{
			hFinishedSearchEvent = new AutoResetEvent(false);
			th = new Thread(ShowAsyncThreadFunc)
			{
				IsBackground = true
			};

			if (m_csShowCounter.Value > 0)
				return;

			m_csShowCounter++;
						
			CShowAsyncParam ThreadParam = new CShowAsyncParam()
			{
				m_hFinishedSearchEvent = hFinishedSearchEvent,
				m_WndTitle = WndTitle,
				m_WndText = WndText
			};
						
			th.SetApartmentState(ApartmentState.STA);
			th.Start(ThreadParam);

			GlobalDefines.DoEvents(DBManagerApp.MainWnd); // Чтобы не зависла главная форма и в её заголовке не было написано "(Не отвечает)"
		}

        public static ShowAsyncResult ShowAsync(string WndTitle, string WndText, bool isAllowedAccess)
        {
            var res = new ShowAsyncResult();

            if (isAllowedAccess)
                ShowAsync(out res.hFinishedSearchEvent, out res.th, WndTitle, WndText);

            return res;
        }


        /// <summary>
        /// Функция потока.
        /// Отдельная функция сделана для того, чтобы в неё можно было передать "out AutoResetEvent hFinishedSearchEvent"
        /// </summary>
        /// <param name="Parameter"></param>
        static void ShowAsyncThreadFunc(object Parameter)
		{
			CShowAsyncParam ThreadParam = Parameter as CShowAsyncParam;
			CWaitingWnd wnd = new CWaitingWnd(ThreadParam.m_hFinishedSearchEvent,
												DBManagerApp.MainWnd,
												ThreadParam.m_WndText,
												ThreadParam.m_WndTitle,
												300);
			try
			{
				wnd.ShowDialog();
			}
			catch (Exception ex)
			{
				DumpMaker.HandleExceptionAndClose(ex, AppAttributes.Title);
			}
		}
	}

    public class ShowAsyncResult
    {
        public AutoResetEvent hFinishedSearchEvent = null;
        public Thread th = null;
    }
}
