using DBManager.Excel.GeneratingWorkbooks.Helpers;
using DBManager.Global;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DBManager.Excel.GeneratingWorkbooks
{
    public class GroupItemLocalWorkbook : GroupItemBase
    {
        #region Name
        public override string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NamePropertyName);
                    FillWorkbookName();
                    Sex = DBManagerApp.m_AppSettings.m_Settings.AvailableGroupNames.First(arg => arg.GroupName == m_Name).Sex;
                }
            }
        }
        #endregion

        #region SheetName
        private static readonly string SheetNamePropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.SheetName);
        private string m_SheetName = "";
        /// <summary>
        /// 
        /// </summary>
        public string SheetName
        {
            get { return m_SheetName; }
            set
            {
                if (m_SheetName != value)
                {
                    m_SheetName = value;
                    OnPropertyChanged(SheetNamePropertyName);
                }
            }
        }
        #endregion

        #region TLCell
        private static readonly string TLCellPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.TLCell);
        private string m_TLCell = "";
        /// <summary>
        /// 
        /// </summary>
        public string TLCell
        {
            get { return m_TLCell; }
            set
            {
                if (m_TLCell != value)
                {
                    if (CheckCellAddress(value))
                        m_TLCell = AdjustCellAddress(value);
                    OnPropertyChanged(TLCellPropertyName);
                }
            }
        }
        #endregion

        #region BRCell
        private static readonly string BRCellPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.BRCell);
        private string m_BRCell = "";
        /// <summary>
        /// 
        /// </summary>
        public string BRCell
        {
            get { return m_BRCell; }
            set
            {
                if (m_BRCell != value)
                {
                    if (CheckCellAddress(value))
                        m_BRCell = AdjustCellAddress(value);
                    OnPropertyChanged(BRCellPropertyName);
                }
            }
        }
        #endregion

        #region PersonalDataColumnIndex
        private static readonly string PersonalDataColumnIndexPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.PersonalDataColumnIndex);
        private int m_PersonalDataColumnIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public int PersonalDataColumnIndex
        {
            get { return m_PersonalDataColumnIndex; }
            set
            {
                if (m_PersonalDataColumnIndex != value)
                {
                    m_PersonalDataColumnIndex = value;
                    OnPropertyChanged(PersonalDataColumnIndexPropertyName);
                }
            }
        }
        #endregion

        #region TeamColumnIndex
        private static readonly string TeamColumnIndexPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.TeamColumnIndex);
        private int m_TeamColumnIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public int TeamColumnIndex
        {
            get { return m_TeamColumnIndex; }
            set
            {
                if (m_TeamColumnIndex != value)
                {
                    m_TeamColumnIndex = value;
                    OnPropertyChanged(PersonalDataColumnIndexPropertyName);
                }
            }
        }
        #endregion

        #region YoBColumnIndex
        private static readonly string YoBColumnIndexPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.YoBColumnIndex);
        private int m_YoBColumnIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public int YoBColumnIndex
        {
            get { return m_YoBColumnIndex; }
            set
            {
                if (m_YoBColumnIndex != value)
                {
                    m_YoBColumnIndex = value;
                    OnPropertyChanged(YoBColumnIndexPropertyName);
                }
            }
        }
        #endregion

        #region GradeColumnIndex
        private static readonly string GradeColumnIndexPropertyName = GlobalDefines.GetPropertyName<GroupItemLocalWorkbook>(m => m.GradeColumnIndex);
        private int m_GradeColumnIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        public int GradeColumnIndex
        {
            get { return m_GradeColumnIndex; }
            set
            {
                if (m_GradeColumnIndex != value)
                {
                    m_GradeColumnIndex = value;
                    OnPropertyChanged(GradeColumnIndexPropertyName);
                }
            }
        }
        #endregion

        private bool CheckCellAddress(string address)
        {
            return Regex.Match(address, @"[A-Za-z]{1,3}\d+").Value == address;
        }

        private string AdjustCellAddress(string address)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ch in address)
            {
                sb.Append(ch.ToCapitalLatinLetter());
            }
            return sb.ToString();
        }

        public GroupItemLocalWorkbook(CompDescLocalWorkbook compDesc) :
            base(compDesc)
        {

        }
    }
}
