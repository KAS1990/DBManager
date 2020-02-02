using DBManager.OnlineDB.Data;
using System;

namespace DBManager.OnlineDB
{
    public class OnlineDBManager
    {
        private static OnlineDBManager m_Instance = null;
        public static OnlineDBManager Instance
        {
            get
            {
                return m_Instance = m_Instance ?? new OnlineDBManager();
            }
        }

        private onlineEntities m_Entities = null;

        public onlineEntities Entities
        {
            get
            {
                if (m_Entities == null)
                {
                    ConnectToRemoteDB();
                }

                return m_Entities;
            }
        }

        public string ConnectionString => m_Entities?.Database.Connection.ConnectionString;
        public bool IsConnectedToRemoteDB => m_Entities != null;

        private OnlineDBManager()
        {
            ConnectToRemoteDB();
        }

        ~OnlineDBManager()
        {
            DisconnectFromRemoteDB();
        }

        private void DisconnectFromRemoteDB()
        {
            m_Entities?.Dispose();
            m_Entities = null;
        }

        public bool ConnectToRemoteDB()
        {
            if (IsConnectedToRemoteDB)
                return true;

            m_Entities = new onlineEntities();

            try
            {
                if (!m_Entities.Database.Exists())
                {
                    throw new InvalidOperationException();
                }
            }
            catch
            {   // Невозможно подключиться к БД 
                m_Entities = null;
                return false;
            }

            return true;
        }
    }
}
