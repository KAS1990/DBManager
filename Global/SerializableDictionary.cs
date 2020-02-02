using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace DBManager.Global
{
    /// <summary>
    /// Словарь, который можно сериализовать
    /// </summary>
    /// <typeparam name="TKey"> Тип ключей в словаре. </typeparam>
    /// <typeparam name="TValue"> Тип значений в словаре. </typeparam>
    [XmlRoot("Dictionary")]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private string m_ItemNodeName = "item";
        /// <summary>
        /// Название узла в XML, соотвествующего одному элементу словаря
        /// </summary>
        [XmlIgnore]
        public string ItemNodeName
        {
            get { return m_ItemNodeName; }
            set { m_ItemNodeName = value; }
        }


        private string m_KeyNodeName = "key";
        /// <summary>
        /// Название узла в XML, соотвествующего ключу
        /// </summary>
        [XmlIgnore]
        public string KeyNodeName
        {
            get { return m_KeyNodeName; }
            set { m_KeyNodeName = value; }
        }


        [XmlIgnore]
        private string m_ValueNodeName = "value";
        /// <summary>
        /// Название узла в XML, соотвествующего значению
        /// </summary>
        [XmlIgnore]
        public string ValueNodeName
        {
            get { return m_ValueNodeName; }
            set { m_ValueNodeName = value; }
        }


        public XmlSchema GetSchema()
        {
            return null;
        }


        #region События, происходящие при ошибках десериализации
        private void DictionarySerializer_UnreferencedObject(object sender, UnreferencedObjectEventArgs e)
        {
            throw new XmlException("SerializableDictionary: UnreferencedObject");
        }


        private void DictionarySerializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            throw new XmlException("SerializableDictionary: UnknownNode");
        }


        private void valueSerializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            throw new XmlException("SerializableDictionary: UnknownAttribute");
        }


        private void DictionarySerializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            throw new XmlException("SerializableDictionary: UnknownElement");
        }
        #endregion


        public void ReadXml(XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            // Привязываем события, которые позволяют определять ошибки, возникшие в ходе десериализации
            keySerializer.UnknownNode += DictionarySerializer_UnknownNode;
            keySerializer.UnknownElement += DictionarySerializer_UnknownElement;
            keySerializer.UnknownAttribute += valueSerializer_UnknownAttribute;
            keySerializer.UnreferencedObject += DictionarySerializer_UnreferencedObject;

            valueSerializer.UnknownNode += DictionarySerializer_UnknownNode;
            valueSerializer.UnknownElement += DictionarySerializer_UnknownElement;
            valueSerializer.UnknownAttribute += valueSerializer_UnknownAttribute;
            valueSerializer.UnreferencedObject += DictionarySerializer_UnreferencedObject;

            bool isEmpty = reader.IsEmptyElement;

            if (!reader.Read())
                throw new XmlException("SerializableDictionary: can't read xml-file");

            if (isEmpty)
                return;

            Type KeyType = typeof(TKey);
            bool UseAttrForKey = KeyType == typeof(int) ||
                                    KeyType == typeof(float) ||
                                    KeyType == typeof(double) ||
                                    KeyType == typeof(string) ||
                                    KeyType == typeof(long);

            Type ValueType = typeof(TValue);
            bool UseAttrForValue = ValueType == typeof(int) ||
                                    ValueType == typeof(float) ||
                                    ValueType == typeof(double) ||
                                    ValueType == typeof(string) ||
                                    ValueType == typeof(long);

            TKey key;
            TValue value;
            object obj = null;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (ItemNodeName != reader.Name)
                    ItemNodeName = reader.Name;

                /* Читаем ключ */
                if (UseAttrForKey)
                {   /* Читаем ключ как атрибут */
                    if (reader.HasAttributes && reader.AttributeCount <= 2 && reader.MoveToFirstAttribute())
                    {
                        if (KeyNodeName != reader.Name)
                            KeyNodeName = reader.Name;

                        if (KeyType == typeof(int))
                            obj = Convert.ToInt32(reader.Value);
                        else if (KeyType == typeof(float))
                            obj = Convert.ToSingle(reader.Value);
                        else if (KeyType == typeof(double))
                            obj = Convert.ToDouble(reader.Value);
                        else if (KeyType == typeof(long))
                            obj = Convert.ToInt64(reader.Value);
                        else
                            obj = reader.Value;
                        key = (TKey)obj;
                    }
                    else
                        throw new XmlException("SerializableDictionary: Key parse error");

                    if (reader.AttributeCount == 1)
                        /* Если больше атрибутов нет, то переходим к содержимому узла */
                        reader.MoveToContent();
                    else /* Есть атрибут для значения => переходим к нему */
                        reader.MoveToNextAttribute();
                }
                else
                {
                    reader.ReadStartElement(ItemNodeName);

                    if (KeyNodeName != reader.Name)
                        KeyNodeName = reader.Name;

                    /* Если попали сюда => ключ не лезет в атрибут => он находится в узле с названием KeyNodeName */
                    reader.ReadStartElement(KeyNodeName);
                    key = (TKey)keySerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }

                /* Читаем значение */
                if (UseAttrForValue && UseAttrForKey)
                {   /* Читаем значение как атрибут, только если ключ был атрибутом */
                    if (reader.HasAttributes && reader.AttributeCount == 2)
                    {
                        if (ValueNodeName != reader.Name)
                            ValueNodeName = reader.Name;

                        if (ValueType == typeof(int))
                            obj = Convert.ToInt32(reader.Value);
                        else if (ValueType == typeof(float))
                            obj = Convert.ToSingle(reader.Value);
                        else if (ValueType == typeof(double))
                            obj = Convert.ToDouble(reader.Value);
                        else if (ValueType == typeof(long))
                            obj = Convert.ToDouble(reader.Value);
                        else
                            obj = reader.Value;
                        value = (TValue)obj;
                    }
                    else
                        throw new XmlException("SerializableDictionary: value parse error");

                    reader.MoveToContent();

                    reader.ReadStartElement(ItemNodeName);
                }
                else
                {
                    if (UseAttrForKey)
                        reader.ReadStartElement(ItemNodeName);

                    if (ValueNodeName != reader.Name)
                        ValueNodeName = reader.Name;

                    if (!UseAttrForKey)
                        /* Если ключ в узле, то значение так же должно быть в узле */
                        reader.ReadStartElement(ValueNodeName);

                    value = (TValue)valueSerializer.Deserialize(reader);

                    if (!UseAttrForKey)
                        reader.ReadEndElement();
                }

                if (!ContainsKey(key))
                    Add(key, value);

                if (!(UseAttrForValue && UseAttrForKey))
                {   /* Завершаем чтение узла, если выполняли ReadStartElement */
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
            }

            reader.ReadEndElement();
        }


        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            Type KeyType = typeof(TKey);
            bool UseAttrForKey = KeyType == typeof(int) ||
                                    KeyType == typeof(float) ||
                                    KeyType == typeof(double) ||
                                    KeyType == typeof(string) ||
                                    KeyType == typeof(long);

            Type ValueType = typeof(TValue);
            bool UseAttrForValue = ValueType == typeof(int) ||
                                    ValueType == typeof(float) ||
                                    ValueType == typeof(double) ||
                                    ValueType == typeof(string) ||
                                    ValueType == typeof(long);

            foreach (var key in Keys)
            {
                writer.WriteStartElement(ItemNodeName);

                /* Пишем ключ */
                if (UseAttrForKey)
                    /* Ключ запишем в атрибут */
                    writer.WriteAttributeString(KeyNodeName, key.ToString());
                else
                {   /* Если попали сюда => ключ не лезет в атрибут => нужно создать для него отдельный узел, который будет называться KeyNodeName */
                    writer.WriteStartElement(KeyNodeName);
                    keySerializer.Serialize(writer, key);
                    writer.WriteEndElement();
                }

                /* Пишем значение */
                TValue value = this[key];
                if (UseAttrForValue && UseAttrForKey)
                    /* Значение запишем в атрибут, только в том случае,
						* если ключ тоже был атрибутом, иначе невозможно выполнить десериализацию */
                    writer.WriteAttributeString(ValueNodeName, value.ToString());
                else
                {
                    if (!UseAttrForKey)
                        /* Если ключ в узле, то для значения так же нужно создать свой узел */
                        writer.WriteStartElement(ValueNodeName);

                    valueSerializer.Serialize(writer, value);

                    if (!UseAttrForKey)
                        writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}
