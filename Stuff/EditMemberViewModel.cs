using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.Scanning.XMLDataClasses;
using System.ComponentModel;
using System.Linq;

namespace DBManager.Stuff
{
    public class EditMemberViewModel : INotifyPropertyChanged
    {
        #region MemberInDB

        private static readonly string MemberInDBPropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.MemberInDB);

        private members m_MemberInDB = null;

        public members MemberInDB
        {
            get { return m_MemberInDB; }
            set
            {
                if (m_MemberInDB != value)
                {
                    m_MemberInDB = value;
                    OnPropertyChanged(MemberInDBPropertyName);
                }
            }
        }

        #endregion

        #region PartInDB

        private static readonly string PartInDBPropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.PartInDB);

        private participations m_PartInDB = null;

        public participations PartInDB
        {
            get { return m_PartInDB; }
            set
            {
                if (m_PartInDB != value)
                {
                    m_PartInDB = value;
                    OnPropertyChanged(PartInDBPropertyName);
                }
            }
        }

        #endregion

        #region Name

        private static readonly string NamePropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.Name);

        private string m_Name = GlobalDefines.DEFAULT_XML_STRING_VAL;

        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                    OnPropertyChanged(NamePropertyName);
                }
            }
        }

        #endregion

        #region Surname

        private static readonly string SurnamePropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.Surname);

        private string m_Surname = GlobalDefines.DEFAULT_XML_STRING_VAL;

        public string Surname
        {
            get { return m_Surname; }
            set
            {
                if (m_Surname != value)
                {
                    m_Surname = value;
                    OnPropertyChanged(SurnamePropertyName);
                }
            }
        }

        #endregion

        #region SecondColumn

        private static readonly string SecondColumnPropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.SecondColumn);

        private string m_SecondColumn = GlobalDefines.DEFAULT_XML_STRING_VAL;

        public string SecondColumn
        {
            get { return m_SecondColumn; }
            set
            {
                if (m_SecondColumn != value)
                {
                    m_SecondColumn = value;
                    OnPropertyChanged(SecondColumnPropertyName);
                }
            }
        }

        #endregion

        #region SecondColumn

        private static readonly string SecondColNameTypePropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.SecondColNameType);
        private enSecondColNameType m_SecondColNameType = enSecondColNameType.None;
        /// <summary>
        /// Тип второй колонки.
        /// Если название колонки в файле неверное, то поле будет иметь значение enSecondColNameType.None.
        /// </summary>
        public enSecondColNameType SecondColNameType
        {
            get { return m_SecondColNameType; }
            private set
            {
                m_SecondColNameType = value;
                OnPropertyChanged(SecondColNameTypePropertyName);
            }
        }

        #endregion

        #region YearOfBirth

        private static readonly string YearOfBirthPropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.YearOfBirth);

        private short? m_YearOfBirth = null;

        public short? YearOfBirth
        {
            get { return m_YearOfBirth; }
            set
            {
                if (m_YearOfBirth != value)
                {
                    m_YearOfBirth = value;
                    OnPropertyChanged(YearOfBirthPropertyName);
                }
            }
        }

        #endregion

        #region Grade

        private static readonly string GradePropertyName = GlobalDefines.GetPropertyName<EditMemberViewModel>(m => m.Grade);

        private enGrade? m_Grade = null;

        public enGrade? Grade
        {
            get { return m_Grade; }
            set
            {
                if (m_Grade != value)
                {
                    m_Grade = value;
                    OnPropertyChanged(GradePropertyName);
                }
            }
        }

        #endregion

        public EditMemberViewModel(CFullMemberInfo memberInfo, long groupId, CCompSettings compSettings)
        {
            MemberInDB = DBManagerApp.m_Entities.members.FirstOrDefault(arg => arg.id_member == memberInfo.IDMember);
            PartInDB = MemberInDB.participations.FirstOrDefault(arg => arg.Group == groupId);

            Name = memberInfo.Name;
            Surname = memberInfo.Surname;
            SecondColumn = memberInfo.SecondCol;
            SecondColNameType = compSettings.SecondColNameType;
            YearOfBirth = memberInfo.YearOfBirth;
            Grade = (enGrade?)memberInfo.InitGrade;
        }

        #region OnPropertyChanged and PropertyChanged event

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion

        public override string ToString()
        {
            return $"{Surname} {Name} {SecondColumn} {YearOfBirth}"
                + (Grade == null ? "" : GlobalDefines.GRADE_NAMES[Grade.Value]);
        }
    }
}
