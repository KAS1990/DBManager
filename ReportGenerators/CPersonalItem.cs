using DBManager.Global;

namespace DBManager.ReportGenerators
{
    /// <summary>
    /// Строка протокола двоеборья
    /// </summary>
    public class CPersonalItem
    {
        public int m_Place = -1;
        public string m_SurnameAndName = null;
        public long? m_Team = -1;
        public short? m_YearOfBirth = -1;
        public byte? m_Grade = null;
        public int m_LeadPlace = -1;
        public int m_SpeedPlace = -1;
        public int m_TotalPlace = -1;

        // Для командного зачёта

        public long m_GroupId = -1;
        public float m_Balls = -1;
        public enSex m_Sex = enSex.None;
    }
}
