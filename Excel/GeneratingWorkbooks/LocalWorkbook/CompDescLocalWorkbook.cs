using DBManager.Excel.GeneratingWorkbooks.Interfaces;
using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class CompDescLocalWorkbook : CompDescBase
    {
        #region ID
        private static readonly string IDPropertyName = GlobalDefines.GetPropertyName<CompDescLocalWorkbook>(m => m.ID);
        private long m_ID = -1;

        public long ID
        {
            get { return m_ID; }
            set
            {
                if (m_ID != value)
                {
                    m_ID = value;
                    OnPropertyChanged(IDPropertyName);
                }
            }
        }
        #endregion

        #region SourceWorkbookName
        public static readonly string SourceWorkbookNamePropertyName = GlobalDefines.GetPropertyName<CompDescLocalWorkbook>(m => m.SourceWorkbookName);
        private string m_SourceWorkbookName = null;
        /// <summary>
        /// 
        /// </summary>
        public string SourceWorkbookName
        {
            get { return m_SourceWorkbookName; }
            set
            {
                if (m_SourceWorkbookName != value)
                {
                    m_SourceWorkbookName = value;
                    OnPropertyChanged(SourceWorkbookNamePropertyName);
                }
            }
        }
        #endregion

        public CompDescLocalWorkbook()
        {
        }

        public override void CopyCompSpecificFields(ICompDesc src)
        {
            base.CopyCompSpecificFields(src);
            SourceWorkbookName = (src as CompDescLocalWorkbook).SourceWorkbookName;
        }
    }
}
