using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DBManager.Global
{
	public class ThreadManager
	{
		private static ThreadManager instance;
		private SynchronizationContext _uiSynchronizationContext;

		public ThreadManager()
		{
		}

		public SynchronizationContext UISynchronizationContext
		{
			get { return _uiSynchronizationContext; }
			set { _uiSynchronizationContext = value; }
		}

		public static ThreadManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ThreadManager();
				}
				return instance;
			}
		}

		/// <summary>
		/// Process code in UI thread synchronously and block calling thread
		/// </summary>
		/// <param name="action"></param>
		public void InvokeUI(Action<object> action, object param)
		{
			if (_uiSynchronizationContext == null)
			{
				throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
			}

			if (_uiSynchronizationContext != SynchronizationContext.Current)
				_uiSynchronizationContext.Send(new SendOrPostCallback(o => action(o)), param);
			else
				action(param);
		}
		
		
		/// <summary>
		/// Process code in UI thread asynchronously
		/// </summary>
		/// <param name="action"></param>
		public void InvokeUIAsync(Action<object> action, object param)
		{
			if (_uiSynchronizationContext == null)
			{
				throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
			}

			if (_uiSynchronizationContext != SynchronizationContext.Current)
				_uiSynchronizationContext.Post(new SendOrPostCallback(o => action(o)), param);
			else
				action(param);
		}
	}
}
