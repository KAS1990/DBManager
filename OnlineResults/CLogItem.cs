using DBManager.Global;
using System;
using System.ComponentModel;

namespace DBManager.OnlineResults
{
    public class CLogItem : INotifyPropertyChanged
    {
        #region Type
        private static readonly string TypePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.Type);

        private enOnlineResultsLogItemType m_Type = enOnlineResultsLogItemType.None;

        public enOnlineResultsLogItemType Type
        {
            get { return m_Type; }
            set
            {
                if (m_Type != value)
                {
                    m_Type = value;
                    OnPropertyChanged(TypePropertyName);
                }
            }
        }
        #endregion

        #region CreationDate
        private static readonly string CreationDatePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.CreationDate);
        private DateTime m_CreationDate = DateTime.Now;

        public DateTime CreationDate
        {
            get { return m_CreationDate; }
            set
            {
                if (m_CreationDate != value)
                {
                    m_CreationDate = value;
                    OnPropertyChanged(CreationDatePropertyName);
                }
            }
        }

        public string CreationDateInString
        {
            get { return CreationDate.ToString(); }
        }
        #endregion


        #region PCWbkName
        private static readonly string PCWbkNamePropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.PCWbkName);
        private string m_PCWbkName = null;
        /// <summary>
        /// Название книги на ПК
        /// </summary>
        public string PCWbkName
        {
            get { return m_PCWbkName; }
            set
            {
                if (m_PCWbkName != value)
                {
                    m_PCWbkName = value;
                    OnPropertyChanged(PCWbkNamePropertyName);
                }
            }
        }
        #endregion


        #region Text
        private static readonly string TextPropertyName = GlobalDefines.GetPropertyName<CLogItem>(m => m.Text);
        private string m_Text = null;
        /// <summary>
        /// Сообщение
        /// </summary>
        public string Text
        {
            get { return m_Text; }
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    OnPropertyChanged(TextPropertyName);
                }
            }
        }
        #endregion


        public string ToLogFileString()
        {
            // Переносы для удобства в логе не используем
            return Type.ToString() +
                    GlobalDefines.PUBLISHING_LOG_FIELDS_SEPARATOR +
                    CreationDate.ToString() +
                    GlobalDefines.PUBLISHING_LOG_FIELDS_SEPARATOR +
                    PCWbkName.Replace("\r", "").Replace('\n', GlobalDefines.PUBLISHING_LOG_LFCR_SYMBOL) +
                    GlobalDefines.PUBLISHING_LOG_FIELDS_SEPARATOR +
                    Text.Replace("\r", "").Replace('\n', GlobalDefines.PUBLISHING_LOG_LFCR_SYMBOL);
        }


        #region OnPropertyChanged and PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;


        public virtual void OnPropertyChanged(string info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }

}
