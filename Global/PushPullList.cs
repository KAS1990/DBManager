using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DBManager.Global
{
	public class PushPullList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
	{
		List<T> m_list = new List<T>();
		int m_PushPullIndex = 0;
		int m_PushPullDir = 1;

		public PushPullList()
		{
		}

		public PushPullList(IEnumerable<T> collection)
		{
			m_list = new List<T>(collection);
		}

		public PushPullList(int capacity)
		{
			m_list = new List<T>(capacity);
		}

		
		public int Capacity
		{
			get { return m_list.Capacity; }
			set
			{
				m_list.Capacity = value;
				Rewind();
			}
		}
		
		
		public int Count
		{
			get { return m_list.Count; }
		}

				
		public int PushPullIndex
		{
			get { return m_PushPullIndex; }
		}

		
		public T this[int index]
		{
			get { return m_list[index]; }
			set { m_list[index] = value; }
		}

		
		public void Add(T item)
		{
			m_list.Add(item);
			Rewind();
		}
		
		
		public void AddRange(IEnumerable<T> collection)
		{
			m_list.AddRange(collection);
			Rewind();
		}


		public void Rewind()
		{
			m_PushPullIndex = 0;
			m_PushPullDir = 1;
		}


		public T Next()
		{
			if (m_PushPullIndex < 0 || m_PushPullIndex >= Count)
				return default(T);

			T result = m_list[m_PushPullIndex];

			if (m_PushPullIndex == 0)
				m_PushPullDir = 1;
			else if (m_PushPullIndex == Count - 1)
				m_PushPullDir = -1;

			m_PushPullIndex += m_PushPullDir;

			return result;
		}


		public void Clear()
		{
			m_list.Clear();
			Rewind();
		}

		
		public bool Contains(T item)
		{
			return m_list.Contains(item);
		}


		public void CopyTo(T[] array)
		{
			m_list.CopyTo(array);
		}
		
		
		public void CopyTo(T[] array, int arrayIndex)
		{
			m_list.CopyTo(array, arrayIndex);
		}
		
		
		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			m_list.CopyTo(index, array, arrayIndex, count);
		}
		
		
		public bool Exists(Predicate<T> match)
		{
			return m_list.Exists(match);
		}
		
		
		public T Find(Predicate<T> match)
		{
			return m_list.Find(match);
		}
		
		
		public int FindIndex(Predicate<T> match)
		{
			return m_list.FindIndex(match);
		}
		
		
		public int FindIndex(int startIndex, Predicate<T> match)
		{
			return m_list.FindIndex(startIndex, match);
		}
		
		
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			return m_list.FindIndex(startIndex, count, match);
		}
		
		
		public T FindLast(Predicate<T> match)
		{
			return m_list.FindLast(match);
		}
		
		
		public int FindLastIndex(Predicate<T> match)
		{
			return m_list.FindLastIndex(match);
		}
		
		
		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			return m_list.FindLastIndex(startIndex, match);
		}
		
		
		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			return m_list.FindLastIndex(startIndex, count, match);
		}


		public int IndexOf(T item)
		{
			return m_list.IndexOf(item);
		}
		
		
		public int IndexOf(T item, int index)
		{
			return m_list.IndexOf(item, index);
		}
		
		
		public int IndexOf(T item, int index, int count)
		{
			return m_list.IndexOf(item, index, count);
		}
		
		
		public void Insert(int index, T item)
		{
			m_list.Insert(index, item);
			Rewind();
		}
		
		
		public int LastIndexOf(T item)
		{
			return m_list.LastIndexOf(item);
		}
		
		
		public int LastIndexOf(T item, int index)
		{
			return m_list.LastIndexOf(item, index);
		}
		
		
		public int LastIndexOf(T item, int index, int count)
		{
			return m_list.LastIndexOf(item, index, count);
		}
		
		
		public bool Remove(T item)
		{
			if (m_list.Remove(item))
			{
				Rewind();
				return true;
			}
			else
				return false;
		}
		
		
		public int RemoveAll(Predicate<T> match)
		{
			int result = m_list.RemoveAll(match);
			if (result > 0)
				Rewind();
			
			return result;
		}
		
		
		public void RemoveAt(int index)
		{
			m_list.RemoveAt(index);
			Rewind();
		}
		
		
		public void Reverse()
		{
			m_list.Reverse();
			Rewind();
		}
		
		
		public T[] ToArray()
		{
			return m_list.ToArray();
		}


		#region IList Members
		bool IList.IsReadOnly
		{
			get
			{
				return (m_list as IList).IsReadOnly; 
			}
		}


		bool IList.IsFixedSize
		{
			get { return false; }
		}


		object IList.this[int index]
		{
			get { return (m_list as IList)[index]; }
			set { (m_list as IList)[index] = value; }
		}


		int IList.Add(object value)
		{
			int result = (m_list as IList).Add(value);
			if (result >= 0)
				Rewind();

			return result;
		}


		bool IList.Contains(object value)
		{
			return (m_list as IList).Contains(value);
		}


		int IList.IndexOf(object value)
		{
			return (m_list as IList).IndexOf(value);
		}


		void IList.Insert(int index, object value)
		{
			(m_list as IList).Insert(index, value);
		}


		void IList.Remove(object value)
		{
			(m_list as IList).Remove(value);
		}
		#endregion


		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)m_list).GetEnumerator();
		}
		#endregion


		#region ICollection<T> Members
		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return (m_list as ICollection<T>).IsReadOnly;
			}
		}
		#endregion


		#region ICollection Members
		int ICollection.Count
		{
			get
			{
				return (m_list as ICollection).Count;
			}
		}


		bool ICollection.IsSynchronized
		{
			get
			{
				return (m_list as ICollection).IsSynchronized;
			}
		}


		object ICollection.SyncRoot
		{
			get
			{
				return (m_list as ICollection).SyncRoot;
			}
		}


		void ICollection.CopyTo(Array array, int index)
		{
			(m_list as ICollection).CopyTo(array, index);
		}
		#endregion


		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}
		#endregion
	}
}
