using DBManager.OnlineDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.OnlineDB
{
    public class OnlineDBManager
    {
        static OnlineDBManager m_Instance = null;
        public static OnlineDBManager Instance
        {
            get
            {
                return m_Instance = m_Instance ?? new OnlineDBManager();
            }
        }


        onlineEntities m_Entities = null;

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

        public bool IsConnectedToRemoteDB => m_Entities != null;

        private OnlineDBManager()
        {
            ConnectToRemoteDB();
        }

        ~OnlineDBManager()
        {
            DisconnectFromRemoteDB();
        }

        void DisconnectFromRemoteDB()
        {
            m_Entities.Dispose();
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
