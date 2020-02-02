using System;
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
        public void InvokeUI(Action action)
        {
            if (_uiSynchronizationContext == null)
            {
                throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
            }

            //

            if (_uiSynchronizationContext != SynchronizationContext.Current)
                _uiSynchronizationContext.Send(new SendOrPostCallback(o => action()), null);
            else
                action();
        }

        /// <summary>
        /// Process code in UI thread synchronously and block calling thread
        /// </summary>
        /// <param name="action"></param>
        public void InvokeUI<TParam>(Action<TParam> action, TParam args)
        {
            if (_uiSynchronizationContext == null)
            {
                throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
            }

            //

            if (_uiSynchronizationContext != SynchronizationContext.Current)
                _uiSynchronizationContext.Send(new SendOrPostCallback(o => action((TParam)o)), args);
            else
                action(args);
        }


        /// <summary>
        /// Process code in UI thread synchronously and block calling thread
        /// </summary>
        /// <param name="action"></param>
        public void InvokeUI<TParam1, TParam2>(Action<TParam1, TParam2> action, TParam1 arg1, TParam2 arg2)
        {
            if (_uiSynchronizationContext == null)
            {
                throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
            }

            //

            if (_uiSynchronizationContext != SynchronizationContext.Current)
                _uiSynchronizationContext.Send(new SendOrPostCallback(o => action(arg1, arg2)), null);
            else
                action(arg1, arg2);
        }

        /// <summary>
        /// Process code in UI thread synchronously and block calling thread
        /// </summary>
        /// <param name="action"></param>
        public void InvokeUI<TParam1, TParam2, TParam3>(Action<TParam1, TParam2, TParam3> action, TParam1 arg1, TParam2 arg2, TParam3 arg3)
        {
            if (_uiSynchronizationContext == null)
            {
                throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
            }

            //

            if (_uiSynchronizationContext != SynchronizationContext.Current)
                _uiSynchronizationContext.Send(new SendOrPostCallback(o => action(arg1, arg2, arg3)), null);
            else
                action(arg1, arg2, arg3);
        }


        /// <summary>
        /// Process code in UI thread asynchronously
        /// </summary>
        /// <param name="action"></param>
        public void InvokeUIAsync<TParam>(Action<TParam> action, TParam args)
        {
            if (_uiSynchronizationContext == null)
            {
                throw new InvalidOperationException("You must specify UISynchronizationContext before using this method.");
            }

            if (_uiSynchronizationContext != SynchronizationContext.Current)
                _uiSynchronizationContext.Post(new SendOrPostCallback(o => action((TParam)o)), args);
            else
                action(args);
        }
    }
}
