using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.ComponentModel;
using DBManager.Global;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;

namespace DBManager.Scanning.XMLDataClasses
{
	/// <summary>
	/// Результаты всех раундов соревнований
	/// </summary>
	[Serializable]
	public class CSpeedResults : CXMLSerializerBase, IXmlSerializable
	{
		#region Сериализуемые свойства
		[XmlAttribute()]
		[DefaultValue(enChangeReason.crNone)]
		public enChangeReason ChangeReason { get; set; }

		/// <summary>
		/// Аргумент для ChangeReason
		/// </summary>
		[XmlAttribute()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Argument { get; set; }
		public bool ShouldSerializeArgument()
		{
			return ShouldSerializeProperty("Argument");
		}

		/// <summary>
		/// Строки, данные в которых изменились
		/// </summary>
		[XmlArray()]
		[DefaultValue(null)]
		public List<int> ChangedRows { get; private set; }

		/// <summary>
		/// Участники и их результаты
		/// </summary>
		[XmlArray()]
		[DefaultValue(null)]
		public List<CMember> Results { get; private set; }
		#endregion

		#region Сериализаторы
		XmlSerializer MemberSerializer = new XmlSerializer(typeof(CMember));
		#endregion

		string m_NodeName = GlobalDefines.DEFAULT_XML_STRING_VAL;
		/// <summary>
		/// Название узла в xml-файле.
		/// Заполняется либо из вне или при десериализации
		/// </summary>
		[XmlIgnore()]
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string NodeName
		{
			get { return m_NodeName; }
			set
			{
				if (m_NodeName != value)
				{
					m_NodeName = value;
					byte RoundInd;
					if (m_NodeName != null && GlobalDefines.ROUND_IDS.TryGetValue(m_NodeName, out RoundInd))
						RoundInEnum = (enRounds)RoundInd;
					else
						RoundInEnum = enRounds.None;
				}
			}
		}


		/// <summary>
		/// Тип раунда в xml-файле.
		/// Заполняется при изменении NodeName
		/// </summary>
		[XmlIgnore()]
		[DefaultValue(enRounds.None)]
		public enRounds RoundInEnum { get; private set; }
		


		#region События, происходящие при ошибках десериализации
		private void Serializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
		{
			throw new XmlException("CSpeedResults: UnreferencedObject");
		}


		private void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			throw new XmlException("CSpeedResults: UnknownNode");
		}


		private void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			throw new XmlException("CSpeedResults: UnknownAttribute");
		}


		private void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
		{
			throw new XmlException("CSpeedResults: UnknownElement");
		}
		#endregion


		Dictionary<string, object> m_DefaultValues = new Dictionary<string, object>();


		public CSpeedResults()
		{
			foreach (PropertyInfo pi in this.GetType().GetProperties())
			{
				if (Attribute.IsDefined(pi, typeof(DefaultValueAttribute)))
				{	// Устанавливаем значение по умолчанию для свойства. Значение берём из атрибута DefaultValue
					DefaultValueAttribute attr = Attribute.GetCustomAttribute(pi, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
					pi.SetValue(this, attr.Value, null);
					m_DefaultValues.Add(pi.Name, attr.Value);
				}
			}
			
			// Привязываем события, которые позволяют определять ошибки, возникшие в ходе десериализации
			MemberSerializer.UnknownNode += Serializer_UnknownNode;
			MemberSerializer.UnknownElement += Serializer_UnknownElement;
			MemberSerializer.UnknownAttribute += Serializer_UnknownAttribute;
			MemberSerializer.UnreferencedObject += Serializer_UnreferencedObject;
		}


		public XmlSchema GetSchema()
		{
			return null;
		}


		public void ReadXml(XmlReader reader)
		{
			// Устанавливаем значение по умолчанию для всех свойств.
			foreach (KeyValuePair<string, object> DefaultVal in m_DefaultValues)
				this.GetType().GetProperty(DefaultVal.Key).SetValue(this, DefaultVal.Value, null);

			if (reader.IsEmptyElement)
				return;

			Results = new List<CMember>();
			ChangedRows = new List<int>();

			NodeName = reader.Name;

			// Читаем значение атрибута ChangeReason
			if (reader.HasAttributes && reader.MoveToFirstAttribute() && reader.Name == "ChangeReason" && reader.HasValue)
			{
				int val;
				if (int.TryParse(reader.Value, out val))
					ChangeReason = (enChangeReason)val;

				if (reader.MoveToNextAttribute() && reader.Name == "Argument" && reader.HasValue)
				{
					Argument = reader.Value;
					reader.MoveToNextAttribute();
				}

				int row = 1;
				while (reader.Name == $"Changed_Row_{row}" && reader.HasValue)
				{
					if (int.TryParse(reader.Value, out val))
						ChangedRows.Add(val);
					reader.MoveToNextAttribute();
					row++;
				}
			}
			reader.Read(); // Переходим к содержимому узла
			string ElementTypeName = typeof(CMember).Name;
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				string node = reader.ReadOuterXml(); // Читаем весь элемент из xml для его модификации 
				// Заменяем название узла на название класса элемента (ElementTypeName)
				int OpenTagIndex = node.IndexOf("<");
				int FirstSpaceIndex = node.IndexOf(' ', OpenTagIndex);
				string MemberNumber = node.Substring(OpenTagIndex + 1, FirstSpaceIndex - OpenTagIndex - 1); // Номер участника в виде строки
				node = node.Replace(MemberNumber, ElementTypeName);
				
				StringReader sr = new StringReader(node); // Специальный Stream для десериализации элемента списка

				CMember Member = MemberSerializer.Deserialize(sr) as CMember;
				Member.Number = byte.Parse(MemberNumber.Substring(1)); // пропускаем первый символ подчёркивания
				Results.Add(Member);
			}

			reader.ReadEndElement();
		}


		public void WriteXml(XmlWriter writer)
		{
			if (NodeName == GlobalDefines.DEFAULT_XML_STRING_VAL)
			{	/* Десериализация невозможна */
				throw new ArgumentNullException("NodeName", "NodeName shouldn't has default value");
			}
			
			writer.WriteAttributeString("ChangeReason", ((int)ChangeReason).ToString()); // Пишем значение поля ChangeReason
			if (ShouldSerializeArgument())
				writer.WriteAttributeString("Argument", Argument.ToString());

			if (ChangedRows != null)
			{
				for (int row = 0; row < ChangedRows.Count; row++)
				{
					writer.WriteAttributeString($"Changed_Row_{row + 1}", ChangedRows[row].ToString());
				}
			}
			
			if (Results != null)
			{
				StringWriter sw = new StringWriter(); // Специальный Stream для сериализации элементов списка
				string ElementTypeName = typeof(CMember).Name;
				for (int i = 0; i < Results.Count; i++)
				{
					sw.GetStringBuilder().Clear(); // Очищаем поток
					MemberSerializer.Serialize(sw, Results[i], StdSerializerNamespaces()); // Сериализуем в него элемент
					string ResultInXml = sw.ToString();
					// Вычленяем из него только атрибуты и закрывающий tag
					ResultInXml = ResultInXml.Substring(ResultInXml.IndexOf(ElementTypeName) + ElementTypeName.Length + 1);
					// Открывающий tag добавляем таким образом, т.к. WriteStartElement в данном случае будет добавлять лишнюю инфу
					ResultInXml = string.Format("\n\t<_{0} {1}", Results[i].Number, ResultInXml);
					
					writer.WriteRaw(ResultInXml);
				}
				writer.WriteRaw("\n  ");
			}
		}
	}
}
