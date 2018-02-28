using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace DBManager.Global
{
	/// <summary>
	/// KeyValuePair, поддерживающее INotifyPropertyChanged и команды
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[Serializable]
	[DebuggerDisplay("Key = {Key}, Value = {Value}")]
	public class CKeyValuePairEx<TKey, TValue> : INotifyPropertyChanged
	{
		#region Key
		private static readonly string KeyPropertyName = GlobalDefines.GetPropertyName<CKeyValuePairEx<TKey, TValue>>(m => m.Key);

		private TKey m_Key = default(TKey);
          
		public TKey Key
		{
			get { return m_Key; }
			set
			{
				if ((m_Key == null && value != null) || (m_Key != null && value == null) || !m_Key.Equals(value))
				{
					m_Key = value;
					OnPropertyChanged(KeyPropertyName);
				}
			}
		}
		#endregion
		

		#region Value
		public static readonly string ValuePropertyName = GlobalDefines.GetPropertyName<CKeyValuePairEx<TKey, TValue>>(m => m.Value);

		private TValue m_Value = default(TValue);

		public TValue Value
		{
			get { return m_Value; }
			set
			{
				if ((m_Value == null && value != null) || (m_Value != null && value == null) || !m_Value.Equals(value))
				{
					m_Value = value;
					OnPropertyChanged(ValuePropertyName);
				}
			}
		}
		#endregion
		
		
		#region Command
		private CCommand m_Command = null;
		[XmlIgnore]
		public CCommand Command
		{
			get { return m_Command; }
			private set
			{
				if (m_Command != value)
					m_Command = value;
			}
		}
		#endregion
		
		
		public CKeyValuePairEx()
		{
		}


		public CKeyValuePairEx(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}


		public CKeyValuePairEx(TKey key, TValue value, KeyValuePairExCommandHandler<TKey, TValue> commandFunc)
		{
			Key = key;
			Value = value;
			Command = new CCommand(() => { commandFunc(this); });
		}


		public CKeyValuePairEx(TKey key, TValue value, KeyValuePairExParamCommandHandler<TKey, TValue> commandParamFunc)
		{
			Key = key;
			Value = value;
			Command = new CCommand((param) => { commandParamFunc(this, param); });
		}


		public override string ToString()
		{
			return "{" + Key.ToString() + ", " + Value.ToString() + "}";
		}


		#region OnPropertyChanged and PropertyChanged event
		public event PropertyChangedEventHandler PropertyChanged;


		public void OnPropertyChanged(string info)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
		#endregion
	}


	/// <summary>
	/// Функция, которая вызывается при срабатывании команды.
	/// </summary>
	public delegate void KeyValuePairExCommandHandler<TKey, TValue>(CKeyValuePairEx<TKey, TValue> sender);

	/// <summary>
	/// Функция, которая вызывается при срабатывании команды.
	/// </summary>
	public delegate void KeyValuePairExParamCommandHandler<TKey, TValue>(CKeyValuePairEx<TKey, TValue> sender, object parameter);
}
