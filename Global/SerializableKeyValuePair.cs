using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DBManager.Global
{
	/// <summary>
	/// KeyValuePair, которую можно сериализовать, т.к. стандартная почему-то не сериализуется
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		[XmlAttribute]
		public TKey Key;
		
		[XmlAttribute]
		public TValue Value;
		
		public SerializableKeyValuePair()
		{
			Key = default(TKey);
			Value = default(TValue);
		}


		public SerializableKeyValuePair(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}


		public override string ToString()
		{
			return "{" + Key.ToString() + ", " + Value.ToString() + "}";
		}
	}
}
