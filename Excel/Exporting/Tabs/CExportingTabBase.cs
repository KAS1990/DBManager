using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.Excel.Exporting.Tabs
{
    public class CExportingTabBase : CNotifyPropertyChangedUserCtrl
    {
        public readonly CExportToExcelWnd m_ParentWnd = null;


        #region CreateReport
        protected static readonly string CreateReportPropertyName = GlobalDefines.GetPropertyName<CExportingTabBase>(m => m.CreateReport);

        private bool m_CreateReport = false;

        public bool CreateReport
        {
            get { return m_CreateReport; }
            set
            {
                if (m_CreateReport != value)
                {
                    m_CreateReport = value;
                    OnPropertyChanged(CreateReportPropertyName);
                }
            }
        }
        #endregion


        #region ExportToAnotherWbk
        protected static readonly string ExportToAnotherWbkPropertyName = GlobalDefines.GetPropertyName<CExportingTabBase>(m => m.ExportToAnotherWbk);

        private bool m_ExportToAnotherWbk = false;

        public bool ExportToAnotherWbk
        {
            get { return m_ExportToAnotherWbk; }
            set
            {
                if (m_ExportToAnotherWbk != value)
                {
                    m_ExportToAnotherWbk = value;
                    OnPropertyChanged(ExportToAnotherWbkPropertyName);
                }
            }
        }
        #endregion


        #region XlsPath
        protected static readonly string XlsPathPropertyName = GlobalDefines.GetPropertyName<CExportingTabBase>(m => m.XlsPath);

        private string m_XlsPath = "";

        public string XlsPath
        {
            get { return m_XlsPath; }
            set
            {
                if (m_XlsPath != value)
                {
                    m_XlsPath = value;
                    OnPropertyChanged(XlsPathPropertyName);
                }
            }
        }
        #endregion


        #region HasLeadReport
        protected static readonly string HasLeadReportPropertyName = GlobalDefines.GetPropertyName<CExportingTabBase>(m => m.HasLeadReport);

        private bool m_HasLeadReport = false;

        public bool HasLeadReport
        {
            get { return m_HasLeadReport; }
            set
            {
                if (m_HasLeadReport != value)
                {
                    m_HasLeadReport = value;
                    OnPropertyChanged(HasLeadReportPropertyName);
                }
            }
        }
        #endregion


        public CExportingTabBase()
        {
        }


        public CExportingTabBase(CExportToExcelWnd ParentWnd, ObservableDictionary<long, CKeyValuePairEx<long, CCompSettings>> CompGroups)
        {
            m_ParentWnd = ParentWnd;
        }


        public virtual bool CheckSettings()
        {
            return true;
        }


        public virtual void BeforeExporting()
        {
        }


        protected bool BaseBrowse_Click(out string Path, string filter, bool IsOpenDlg)
        {
            Path = null;
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                if (IsOpenDlg)
                {
                    System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog()
                    {
                        CheckFileExists = false,
                        Multiselect = false,
                        AddExtension = true,
                        InitialDirectory = DBManagerApp.m_AppSettings.m_Settings.CompDir,
                        ValidateNames = true,
                        Filter = filter,
                        DefaultExt = GlobalDefines.XLSX_EXTENSION
                    };

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Path = dlg.FileName;
                        return true;
                    }
                }
                else
                {
                    System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog()
                    {
                        CheckFileExists = false,
                        CreatePrompt = false,
                        AddExtension = true,
                        InitialDirectory = DBManagerApp.m_AppSettings.m_Settings.CompDir,
                        OverwritePrompt = true,
                        ValidateNames = true,
                        Filter = filter,
                        DefaultExt = GlobalDefines.XLSX_EXTENSION
                    };

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Path = dlg.FileName;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
