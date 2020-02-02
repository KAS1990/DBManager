using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DBManager.Global
{
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
                                                        ICollection<KeyValuePair<TKey, TValue>>,
                                                        IEnumerable<KeyValuePair<TKey, TValue>>,
                                                        IDictionary,
                                                        ICollection,
                                                        IEnumerable,
                                                        INotifyCollectionChanged,
                                                        INotifyPropertyChanged
    {
        private const string COUNT_STRING = "Count";
        private const string INDEXER_NAME = "Item[]";
        private const string KEYS_NAME = "Keys";
        private const string VALUES_NAME = "Values";

        private IDictionary<TKey, TValue> _Dictionary;
        protected IDictionary<TKey, TValue> Dictionary
        {
            get { return _Dictionary; }
        }

        #region SelectedKey
        private static readonly string SelectedKeyPropertyName = GlobalDefines.GetPropertyName<ObservableDictionary<TKey, TValue>>(m => m.SelectedKey);
        private TKey _SelectedKey = default(TKey);
        public TKey SelectedKey
        {
            get { return _SelectedKey; }
            set
            {
                if ((_SelectedKey == null && value != null) || (_SelectedKey != null && value == null) || !_SelectedKey.Equals(value))
                {
                    PrevSelectedKey = _SelectedKey;
                    _SelectedKey = value;
                    OnPropertyChanged(SelectedKeyPropertyName);
                    OnPropertyChanged(SelectedItemPropertyName);
                }
            }
        }
        #endregion


        #region SelectedItem
        private static readonly string SelectedItemPropertyName = GlobalDefines.GetPropertyName<ObservableDictionary<TKey, TValue>>(m => m.SelectedItem);
        public TValue SelectedItem
        {
            get
            {
                TValue result;
                if (TryGetValue(SelectedKey, out result))
                    return result;
                else
                    return default(TValue);
            }
        }
        #endregion


        #region PrevSelectedKey
        private static readonly string PrevSelectedKeyPropertyName = GlobalDefines.GetPropertyName<ObservableDictionary<TKey, TValue>>(m => m.PrevSelectedKey);
        private TKey _PrevSelectedKey = default(TKey);
        public TKey PrevSelectedKey
        {
            get { return _PrevSelectedKey; }
            set
            {
                if ((_PrevSelectedKey == null && value != null) || (_PrevSelectedKey != null && value == null) || !_PrevSelectedKey.Equals(value))
                {
                    _PrevSelectedKey = value;
                    OnPropertyChanged(PrevSelectedKeyPropertyName);
                }
            }
        }
        #endregion


        #region Constructors
        public ObservableDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }
        #endregion


        #region IDictionary<TKey,TValue> Members
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return Dictionary.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TValue value;
            bool removed = false;
            if (Dictionary.TryGetValue(key, out value))
            {
                removed = Dictionary.Remove(key);
                if (removed)
                {
                    if ((key == null && SelectedKey == null) || SelectedKey.Equals(key))
                        SelectedKey = default(TKey);

                    OnCollectionChanged();
                }
            }

            return removed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public bool TryAddValue(TKey key, TValue value)
        {
            bool added = !ContainsKey(key);
            if (added)
                Insert(key, value, true);

            return added;
        }

        public ICollection<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return Dictionary[key];
            }
            set
            {
                Insert(key, value, false);
            }
        }
        #endregion


        #region ICollection<KeyValuePair<TKey,TValue>> Members
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                SelectedKey = default(TKey);
                OnCollectionChanged();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)this).Remove(item.Key);
        }
        #endregion


        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion


        #region IDictionary Members
        void IDictionary.Add(object key, object value)
        {
            if (!(key is TKey))
                throw new ArgumentException("The type of parameter \"key\" must be " + typeof(TKey).Name, "key");
            if (!(value is TValue))
                throw new ArgumentException("The type of parameter \"value\" must be " + typeof(TValue).Name, "value");

            Insert((TKey)key, (TValue)value, true);
        }

        bool IDictionary.Contains(object key)
        {
            if (!(key is TKey))
                throw new ArgumentException("The type of parameter \"key\" must be " + typeof(TKey).Name, "key");

            return Dictionary.ContainsKey((TKey)key);
        }

        ICollection IDictionary.Keys
        {
            get { return (ICollection)Dictionary.Keys; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return Dictionary.IsReadOnly; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)Dictionary).IsFixedSize; }
        }

        void IDictionary.Remove(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!(key is TKey))
                throw new ArgumentException("The type of parameter \"key\" must be " + typeof(TKey).Name, "key");

            TValue value;
            if (Dictionary.TryGetValue((TKey)key, out value))
            {
                if (Dictionary.Remove((TKey)key))
                {
                    if ((key == null && SelectedKey == null) || SelectedKey.Equals(key))
                        SelectedKey = default(TKey);
                    OnCollectionChanged();
                }
            }
        }

        void IDictionary.Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                SelectedKey = default(TKey);
                OnCollectionChanged();
            }
        }

        ICollection IDictionary.Values
        {
            get { return (ICollection)Dictionary.Values; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (!(key is TKey))
                    throw new ArgumentException("The type of parameter \"key\" must be " + typeof(TKey).Name, "key");

                return Dictionary[(TKey)key];
            }
            set
            {
                if (!(key is TKey))
                    throw new ArgumentException("The type of parameter \"key\" must be " + typeof(TKey).Name, "key");
                if (!(value is TValue))
                    throw new ArgumentException("The type of parameter \"value\" must be " + typeof(TValue).Name, "value");

                Insert((TKey)key, (TValue)value, false);
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator)Dictionary.GetEnumerator();
        }
        #endregion


        #region ICollection Members
        void ICollection.CopyTo(Array array, int index)
        {
            Dictionary.CopyTo((KeyValuePair<TKey, TValue>[])array, index);
        }

        int ICollection.Count
        {
            get { return Dictionary.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)Dictionary).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)Dictionary).SyncRoot; }
        }
        #endregion


        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Dictionary).GetEnumerator();
        }
        #endregion


        #region INotifyCollectionChanged Members
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        public IEqualityComparer<TKey> Comparer
        {
            get { return ((Dictionary<TKey, TValue>)Dictionary).Comparer; }
        }


        public bool ContainsValue(TValue value)
        {
            return ((Dictionary<TKey, TValue>)Dictionary).ContainsValue(value);
        }


        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            if (items.Count > 0)
            {
                if (Dictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                        foreach (KeyValuePair<TKey, TValue> item in items)
                            Dictionary.Add(item);
                }
                else
                    _Dictionary = new Dictionary<TKey, TValue>(items);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
            }
        }


        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TValue item;
            if (Dictionary.TryGetValue(key, out item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.", "key");
                if (Equals(item, value))
                    return;
                Dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }


        #region OnPropertyChanged
        private void OnPropertyChanged()
        {
            OnPropertyChanged(COUNT_STRING);
            OnPropertyChanged(INDEXER_NAME);
            OnPropertyChanged(KEYS_NAME);
            OnPropertyChanged(VALUES_NAME);
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            ThreadManager.Instance.InvokeUI((arg) =>
                {
                    RaisePropertyChanged(arg as string);
                },
                propertyName);
        }


        private void RaisePropertyChanged(object param)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(param.ToString()));
        }
        #endregion


        #region OnCollectionChanged
        private void OnCollectionChanged()
        {
            OnPropertyChanged();
            if (CollectionChanged != null)
            {
                ThreadManager.Instance.InvokeUI(() =>
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    });
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null)
            {
                ThreadManager.Instance.InvokeUI(() =>
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
                    });
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
        {
            OnPropertyChanged();
            if (CollectionChanged != null)
            {
                ThreadManager.Instance.InvokeUI(() =>
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
                    });
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            OnPropertyChanged();
            if (CollectionChanged != null)
            {
                ThreadManager.Instance.InvokeUI(() =>
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
                    });
            }
        }
        #endregion
    }
}
