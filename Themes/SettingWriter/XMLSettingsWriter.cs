using System;
using System.IO;
using System.Xml.Serialization;

namespace DBManager.SettingsWriter
{
    public class XMLSettingsWriter
    {
        public AppSettings m_Settings = null;
        /// <summary>
        /// Объект, который используется для синхронизации доступа к полю m_Settings
        /// </summary>
        public object m_SettingsSyncObj = new object();


        /// <summary>
        /// Путь к файлу настроек
        /// </summary>
        private string m_FileName = Directory.GetCurrentDirectory() + "\\InterfaceSettings.xml";


        public XMLSettingsWriter()
        {
            Read();
        }


        ~XMLSettingsWriter()
        {
            /* Сохранять настройки нужно только, если не выполняем перезагрузку программы,
             * т.к. сохранение уже сделано перед перезагрузкой, и если тут выполнять сохранение, то выпадим с ошибкой "Файл настроек занят" */
            if (!m_Settings.IsRestarting)
                Write();
        }


        /// <summary>
        /// Доступен ли файл для записи
        /// </summary>
        /// <returns></returns>
        public bool FileAvailableToWrite()
        {
            lock (m_SettingsSyncObj)
            {
                TextWriter writer = null;
                try
                {
                    writer = new StreamWriter(m_FileName);
                    writer.Close();
                    return true;
                }
                catch (Exception ex)
                {	/* Произошла какая-то ошибка при записи данных в файл или файл недоступен для записи */
                    writer.Close();

                    ex.ToString(); // make compiler happy

                    return false;
                }
            }
        }


        public void Write()
        {
            lock (m_SettingsSyncObj)
            {
                TextWriter writer = null;
                try
                {
                    writer = new StreamWriter(m_FileName);
                    XmlSerializer ser = new XmlSerializer(typeof(AppSettings));
                    ser.Serialize(writer, m_Settings);

                    writer.Close();
                    writer = null;
                }
                catch (Exception ex)
                {	/* Произошла какая-то ошибка при записи данных в файл или файл недоступен для записи */
                    if (writer != null)
                        writer.Close();

                    ex.ToString(); // make compiler happy
                }
            }
        }


        public void Read()
        {
            lock (m_SettingsSyncObj)
            {
                if (File.Exists(m_FileName))
                {
                    /* Нужно открывать файл для чтения именно так, если использовать StreamReader(m_FileName), то процесс может не получить доступ к файлу,
                     * почему это так, написано здесь:
                     * http://stackoverflow.com/questions/1606349/does-a-streamreader-lock-a-text-file-whilst-it-is-in-use-can-i-prevent-this/1606370#1606370 */
                    using (FileStream fs = new FileStream(m_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        try
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(AppSettings));
                            m_Settings = ser.Deserialize(reader) as AppSettings;
                        }
                        catch (Exception ex)
                        {
                            ex.ToString(); // make compiler happy
                        }
                    }
                }

                if (m_Settings == null)
                {   /* Не удалось прочитать настройки */
                    m_Settings = new AppSettings();
                    m_Settings.ToDefault();
                }
                else
                    /* Заполняем непрочитанные поля значениями по умолчанию */
                    m_Settings.CheckAndToDefault();
            }
        }


        /// <summary>
        /// Обновить имя с файлом настроек после того как сменилась текущая директория
        /// </summary>
        public void CurDirectoryChanged()
        {
            m_FileName = Directory.GetCurrentDirectory() + "\\InterfaceSettings.xml";
            Read();
        }
    }
}
