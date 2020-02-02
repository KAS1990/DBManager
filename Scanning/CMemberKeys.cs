using DBManager.Global;
using DBManager.Scanning.DBAdditionalDataClasses;

namespace DBManager.Scanning
{
    /// <summary>
    /// Различные ключи в БД, связанные со спортсменом
    /// </summary>
    public class CMemberKeys
    {
        public string Name = GlobalDefines.DEFAULT_XML_STRING_VAL;
        public string Surname = GlobalDefines.DEFAULT_XML_STRING_VAL;
        public string SurnameAndName
        {
            get
            {
                if (Name != GlobalDefines.DEFAULT_XML_STRING_VAL && Surname != GlobalDefines.DEFAULT_XML_STRING_VAL)
                    return GlobalDefines.CreateSurnameAndName(Surname, Name);
                else
                    return GlobalDefines.DEFAULT_XML_STRING_VAL;
            }
        }

        /// <summary>
        /// Строка в таблице members
        /// </summary>
        public members Member;
        /// <summary>
        /// Строка в таблице participations
        /// </summary>
        public participations Participation;


        public CMemberKeys(string name = GlobalDefines.DEFAULT_XML_STRING_VAL,
                            string surname = GlobalDefines.DEFAULT_XML_STRING_VAL,
                            CMemberAndPart MemberAndPart = null)
        {
            Name = name;
            Surname = surname;
            if (MemberAndPart != null)
            {
                Member = MemberAndPart.Member;
                Participation = MemberAndPart.Participation;
            }
        }
    }
}
