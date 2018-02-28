using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace DBManager.Global
{
	/// <summary>
	/// ObservableCollection с дополнительными возможностями, которые нужны в данном проекте
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObservableCollectionEx<T> : ObservableCollection<T>
	{
		private const string COUNT_STRING = "Count";
		private const string INDEXER_NAME = "Item[]";

		/// <summary>
		/// Поле, позволяющее избежать вызова OnCollectionChanged при каждом изменении коллекции,
		/// если данные в нем меняются не по-одному а диапазоном
		/// </summary>
		private bool m_IsInProcessRange = false;
		
		/// <summary>
		/// Поле, позволяющее выполнять операции с коллекцией не только из того потока, в котором она была создана
		/// </summary>
		private SynchronizationContext m_SyncContext = SynchronizationContext.Current;

		
		protected enum ProcessRangeAction
		{
			Add,
			Replace,
			Remove,
		};

		public ObservableCollectionEx()
			: base()
		{
		}

		public ObservableCollectionEx(IEnumerable<T> collection)
			: base(collection)
		{
		}

		public ObservableCollectionEx(List<T> list)
			: base(list)
		{
		}


		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (SynchronizationContext.Current == m_SyncContext)
				RaiseCollectionChanged(e);
			else
				m_SyncContext.Send(RaiseCollectionChanged, e);
		}


		private void RaiseCollectionChanged(object param)
		{
			if (!m_IsInProcessRange)
				base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
		}


		protected override void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (SynchronizationContext.Current == m_SyncContext)
				RaisePropertyChanged(e);
			else
				m_SyncContext.Send(RaisePropertyChanged, e);
		}

		private void RaisePropertyChanged(object param)
		{
			base.OnPropertyChanged((PropertyChangedEventArgs)param);
		}

		
		protected virtual void ProcessRange(IEnumerable<T> collection, ProcessRangeAction action)
		{
			if (collection == null)
				throw new ArgumentNullException("collection");

			var items = collection as IList<T> ?? collection.ToList();
			if (!items.Any())
				return;

			CheckReentrancy();

			m_IsInProcessRange = true;

			if (action == ProcessRangeAction.Replace)
				Items.Clear();
			
			foreach (var item in items)
			{
				if (action == ProcessRangeAction.Remove)
					Items.Remove(item);
				else
					Items.Add(item);
			}
			m_IsInProcessRange = false;

			OnPropertyChanged(new PropertyChangedEventArgs(COUNT_STRING));
			OnPropertyChanged(new PropertyChangedEventArgs(INDEXER_NAME));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.ProcessRange(collection, ProcessRangeAction.Add);
		}
		
		/// <summary>
		/// Очистка всей коллекции и затем добавления в неё <paramref name="collection"/>
		/// </summary>
		/// <param name="collection"></param>
		public void ReplaceRange(IEnumerable<T> collection)
		{
			this.ProcessRange(collection, ProcessRangeAction.Replace);
		}

		public void RemoveRange(IEnumerable<T> collection)
		{
			this.ProcessRange(collection, ProcessRangeAction.Remove);
		}
	}
}
