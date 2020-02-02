using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace DBManager.Scanning.XMLDataClasses
{
    /// <summary>
    /// Базовый класс для всех классов, которые должны заполняться данными, находящимися в xml-файле
    /// </summary>
    [Serializable]
    public abstract class CXMLSerializerBase : INotifyPropertyChanged
    {
        public CXMLSerializerBase()
        {

        }


        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="rhs"></param>
        public CXMLSerializerBase(CXMLSerializerBase rhs)
        {
            Type[] types = new Type[1];
            object[] ctorParams = new object[1];

            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                if (!Attribute.IsDefined(pi, typeof(XmlIgnoreAttribute)))
                {	// Свойство сериализуется => копируем его
                    if (pi.PropertyType.BaseType == GetType().BaseType)
                    {	// Нужно вызвать конструктор копирования => деллаем это через reflection
                        types[0] = pi.PropertyType;
                        ctorParams[0] = pi.GetValue(rhs, null);
                        ConstructorInfo ctori = pi.PropertyType.GetConstructor(types);
                        if (ctorParams[0] == null)
                            pi.SetValue(this, null, null);
                        else
                            pi.SetValue(this, ctori.Invoke(ctorParams), null);
                    }
                    else
                        pi.SetValue(this, pi.GetValue(rhs, null), null);
                }
            }
        }


        /// <summary>
        /// Устанавливает для всех свойств значения по умолчанию, взятые из атрибута DefaultValue
        /// </summary>
        /// <param name="obj"></param>
        public static void SetDefaultValsForAllProps(object obj)
        {
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                if (Attribute.IsDefined(pi, typeof(DefaultValueAttribute)))
                {	// Устанавливаем значение по умолчанию для свойства. Значение берём из атрибута DefaultValue
                    DefaultValueAttribute attr = Attribute.GetCustomAttribute(pi, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                    pi.SetValue(obj, attr.Value, null);
                }
            }
        }


        /// <summary>
        /// Стандартные пространства имён для сериализатора.
        /// Функция задаёт для них пустые названия, тем самым исключает добавление 
        /// xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
        /// в документ
        /// </summary>
        /// <returns></returns>
        public static XmlSerializerNamespaces StdSerializerNamespaces()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);
            ns.Add(string.Empty, string.Empty);
            return ns;
        }


        protected bool ShouldSerializeProperty(string PropertyName)
        {
            PropertyInfo pi = this.GetType().GetProperty(PropertyName);
            if (pi == null)
            {
                return true;
            }
            else
            {
                DefaultValueAttribute attr = Attribute.GetCustomAttribute(pi, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                object val = pi.GetValue(this, null);
                if (attr != null)
                    return val != attr.Value;
                else
                    return true;
            }
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
