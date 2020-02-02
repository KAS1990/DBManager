using DBManager.Global;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DBManager.Scanning.XMLDataClasses
{
    /// <summary>
    /// Класс, позволяющий выполнять сериализацию/десериализацию класса CAllExcelData
    /// </summary>
    public class CXMLDataSerializer
    {
        private CAllExcelData m_Data = null;
        public CAllExcelData Data
        {
            get { return m_Data; }
            private set { m_Data = value; }
        }

        /// <summary>
        /// Объект, который используется для синхронизации доступа к полю m_Settings
        /// </summary>
        private readonly object m_DataSyncObj = new object();
        public object DataSyncObj
        {
            get { return m_DataSyncObj; }
        }


        /// <summary>
        /// Путь к файлу настроек
        /// </summary>
        private string m_FullFilePath = GlobalDefines.DEFAULT_XML_STRING_VAL;
        public string FullFilePath
        {
            get { return m_FullFilePath; }
            set
            {
                if (m_FullFilePath != value)
                {
                    ClearData(); // Т.к. название файла сменилось, то удаляем прочитанные ранее данные
                    m_FullFilePath = value;
                }
            }
        }


        public CXMLDataSerializer()
        {
        }


        ~CXMLDataSerializer()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath">
        /// Если GlobalDefines.DEFAULT_XML_STRING_VAL, то используется значение из свойства FullFilePath
        /// </param>
        /// <returns></returns>
        public bool Write(string FilePath = GlobalDefines.DEFAULT_XML_STRING_VAL)
        {
            lock (DataSyncObj)
            {
                if (FilePath != GlobalDefines.DEFAULT_XML_STRING_VAL)
                {
                    m_FullFilePath = FilePath; /* Здесь обязательно нужно использовать поле,
												  т.к. при записи значения в свойство будет вызвана ClearData */
                }

                if (FullFilePath == GlobalDefines.DEFAULT_XML_STRING_VAL || Data == null)
                    return false;

                TextWriter writer = null;
                try
                {
                    writer = new StreamWriter(FullFilePath);
                    XmlSerializer ser = new XmlSerializer(typeof(CAllExcelData));
                    ser.Serialize(writer, Data, CXMLSerializerBase.StdSerializerNamespaces());

                    writer.Close();
                    writer = null;
                }
                catch (Exception ex)
                {   /* Произошла какая-то ошибка при записи данных в файл или файл недоступен для записи */
                    if (writer != null)
                        writer.Close();

                    ex.ToString(); // make compiler happy

                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath">
        /// Если GlobalDefines.DEFAULT_XML_STRING_VAL, то используется значение из свойства FullFilePath
        /// </param>
        /// <returns></returns>
        public bool Read(string FilePath = GlobalDefines.DEFAULT_XML_STRING_VAL)
        {
            lock (DataSyncObj)
            {
                if (FilePath != GlobalDefines.DEFAULT_XML_STRING_VAL)
                {
                    FullFilePath = FilePath;
                }
                ClearData();
                if (FullFilePath != GlobalDefines.DEFAULT_XML_STRING_VAL && File.Exists(FullFilePath))
                {
                    // Проверяем, чтобы к файлу был доступ
                    if (!GlobalDefines.CheckFileAccessForXMLReading(FullFilePath))
                        return false;

                    /* Нужно открывать файл для чтения именно так, если использовать StreamReader(FullFilePath), то процесс может не получить доступ к файлу,
					 * почему это так, написано здесь:
					 * http://stackoverflow.com/questions/1606349/does-a-streamreader-lock-a-text-file-whilst-it-is-in-use-can-i-prevent-this/1606370#1606370 */
                    using (FileStream fs = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        try
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(CAllExcelData));
                            Data = ser.Deserialize(reader) as CAllExcelData;
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            }

            return Data != null;
        }


        public void ClearData()
        {
            Data = null;
        }
    }
}
