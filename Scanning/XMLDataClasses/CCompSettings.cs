using DBManager.Global;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DBManager.Scanning.XMLDataClasses
{
    /// <summary>
    /// Настройки соревнований
    /// </summary>
    [Serializable]
    public class CCompSettings : CXMLSerializerBase
    {
        #region Сериализуемые свойства
        #region CompName
        private string m_CompName = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = "Name")]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string CompName
        {
            get { return m_CompName; }
            set
            {
                if (m_CompName != value)
                {
                    m_CompName = value;
                    if (m_CompName != null)
                        m_CompName = m_CompName.Trim();
                    OnPropertyChanged("CompName");
                }
            }
        }
        public bool ShouldSerializeCompName()
        {
            return ShouldSerializeProperty("CompName");
        }
        #endregion

        #region SecondColName
        private string m_SecondColName = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute()]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string SecondColName
        {
            get { return m_SecondColName; }
            set
            {
                if (m_SecondColName != value)
                {
                    m_SecondColName = value;
                    if (string.IsNullOrWhiteSpace(m_SecondColName))
                        SecondColNameType = enSecondColNameType.None;
                    else
                    {
                        m_SecondColName = m_SecondColName.Trim();
                        switch (m_SecondColName.ToLower())
                        {
                            case "команда":
                                SecondColNameType = enSecondColNameType.Team;
                                break;

                            case "тренер":
                                SecondColNameType = enSecondColNameType.Coach;
                                break;

                            case GlobalDefines.DEFAULT_XML_STRING_VAL:
                            default:
                                SecondColNameType = enSecondColNameType.None;
                                break;
                        }
                    }
                    OnPropertyChanged("SecondColName");
                }
            }
        }
        public bool ShouldSerializeSecondColName()
        {
            return ShouldSerializeProperty("SecondColName");
        }
        #endregion

        #region MainJudge
        private string m_MainJudge = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute()]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string MainJudge
        {
            get { return m_MainJudge; }
            set
            {
                if (m_MainJudge != value)
                {
                    m_MainJudge = value;
                    if (m_MainJudge != null)
                        m_MainJudge = m_MainJudge.Trim();
                    if (string.IsNullOrEmpty(m_MainJudge))
                        m_MainJudge = GlobalDefines.DEFAULT_XML_STRING_VAL;

                    OnPropertyChanged("MainJudge");
                }
            }
        }
        public bool ShouldSerializeMainJudge()
        {
            return ShouldSerializeProperty("MainJudge");
        }
        #endregion

        #region MainSecretary
        private string m_MainSecretary = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute()]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string MainSecretary
        {
            get { return m_MainSecretary; }
            set
            {
                if (m_MainSecretary != value)
                {
                    m_MainSecretary = value;
                    if (m_MainSecretary != null)
                        m_MainSecretary = m_MainSecretary.Trim();
                    if (string.IsNullOrEmpty(m_MainSecretary))
                        m_MainSecretary = GlobalDefines.DEFAULT_XML_STRING_VAL;

                    OnPropertyChanged("MainSecretary");
                }
            }
        }
        public bool ShouldSerializeMainSecretary()
        {
            return ShouldSerializeProperty("MainSecretary");
        }
        #endregion

        #region Row6
        private string m_Row6 = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute()]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string Row6
        {
            get { return m_Row6; }
            set
            {
                if (m_Row6 != value)
                {
                    m_Row6 = value;
                    if (m_Row6 != null)
                        m_Row6 = m_Row6.Trim();
                    if (string.IsNullOrEmpty(m_Row6))
                        m_Row6 = GlobalDefines.DEFAULT_XML_STRING_VAL;

                    OnPropertyChanged("Row6");
                }
            }
        }
        public bool ShouldSerializeRow6()
        {
            return ShouldSerializeProperty("Row6");
        }
        #endregion

        #region MembersFrom1stQualif
        private byte m_MembersFrom1stQualif = GlobalDefines.DEFAULT_XML_BYTE_VAL;
        [XmlAttribute("MembersFromFirstQualif")]
        [DefaultValue(GlobalDefines.DEFAULT_XML_BYTE_VAL)]
        public byte MembersFrom1stQualif
        {
            get { return m_MembersFrom1stQualif; }
            set
            {
                if (m_MembersFrom1stQualif != value)
                {
                    m_MembersFrom1stQualif = value;
                    OnPropertyChanged("MembersFrom1stQualif");
                }
            }
        }
        public bool ShouldSerializeMembersFromFirstQualif()
        {
            return ShouldSerializeProperty("MembersFrom1stQualif");
        }
        #endregion

        #region MembersFrom2ndQualif
        private byte m_MembersFrom2ndQualif = GlobalDefines.DEFAULT_XML_BYTE_VAL;
        [XmlAttribute("MembersFromSecondQualif")]
        [DefaultValue(GlobalDefines.DEFAULT_XML_BYTE_VAL)]
        public byte MembersFrom2ndQualif
        {
            get { return m_MembersFrom2ndQualif; }
            set
            {
                if (m_MembersFrom2ndQualif != value)
                {
                    m_MembersFrom2ndQualif = value;
                    OnPropertyChanged("MembersFrom2ndQualif");
                }
            }
        }
        public bool ShouldSerializeMembersFromSecondQualif()
        {
            return ShouldSerializeProperty("MembersFrom2ndQualif");
        }
        #endregion

        #region Даты проведения отдельных этов
        #region QualifDate
        private string m_QualifDate = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = "Квалификация")]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string QualifDate
        {
            get { return m_QualifDate; }
            set
            {
                if (m_QualifDate != value)
                {
                    m_QualifDate = value;
                    OnPropertyChanged("QualifDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        public bool ShouldSerializeQualifDate()
        {
            return ShouldSerializeProperty("QualifDate");
        }
        #endregion

        #region OneEighthFinalDate
        private string m_OneEighthFinalDate = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = GlobalDefines.ONE_EIGHTH_FINAL_NODE_NAME)]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string OneEighthFinalDate
        {
            get { return m_OneEighthFinalDate; }
            set
            {
                if (m_OneEighthFinalDate != value)
                {
                    m_OneEighthFinalDate = value;
                    OnPropertyChanged("OneEighthFinalDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        public bool ShouldSerializeOneEighthFinalDate()
        {
            return ShouldSerializeProperty("OneEighthFinalDate");
        }
        #endregion

        #region QuaterFinalDate
        private string m_QuaterFinalDate = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = GlobalDefines.QUATER_FINAL_NODE_NAME)]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string QuaterFinalDate
        {
            get { return m_QuaterFinalDate; }
            set
            {
                if (m_QuaterFinalDate != value)
                {
                    m_QuaterFinalDate = value;
                    OnPropertyChanged("QuaterFinalDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        public bool ShouldSerializeQuaterFinalDate()
        {
            return ShouldSerializeProperty("QuaterFinalDate");
        }
        #endregion

        #region SemiFinalDate
        private string m_SemiFinalDate = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = GlobalDefines.SEMI_FINAL_NODE_NAME)]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string SemiFinalDate
        {
            get { return m_SemiFinalDate; }
            set
            {
                if (m_SemiFinalDate != value)
                {
                    m_SemiFinalDate = value;
                    OnPropertyChanged("SemiFinalDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        public bool ShouldSerializeSemiFinalDate()
        {
            return ShouldSerializeProperty("SemiFinalDate");
        }
        #endregion

        #region FinalDate
        private string m_FinalDate = GlobalDefines.DEFAULT_XML_STRING_VAL;
        [XmlAttribute(AttributeName = GlobalDefines.FINAL_NODE_NAME)]
        [DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
        public string FinalDate
        {
            get { return m_FinalDate; }
            set
            {
                if (m_FinalDate != value)
                {
                    m_FinalDate = value;
                    OnPropertyChanged("FinalDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        public bool ShouldSerializeFinalDate()
        {
            return ShouldSerializeProperty("FinalDate");
        }
        #endregion
        #endregion

        #region AgeGroup
        private CAgeGroup m_AgeGroup = null;
        [XmlElement(IsNullable = false)]
        [DefaultValue(null)]
        public CAgeGroup AgeGroup
        {
            get { return m_AgeGroup; }
            set
            {
                if (m_AgeGroup != value)
                {
                    m_AgeGroup = value;
                    OnPropertyChanged("AgeGroup");
                }
            }
        }
        #endregion

        #region StartDate
        private CCompDate m_StartDate = null;
        [XmlElement(IsNullable = false)]
        [DefaultValue(null)]
        public CCompDate StartDate
        {
            get { return m_StartDate; }
            set
            {
                if (m_StartDate != value)
                {
                    m_StartDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }
        #endregion

        #region EndDate
        private CCompDate m_EndDate = null;
        [XmlElement(IsNullable = false)]
        [DefaultValue(null)]
        public CCompDate EndDate
        {
            get { return m_EndDate; }
            set
            {
                if (m_EndDate != value)
                {
                    m_EndDate = value;
                    OnPropertyChanged("EndDate");
                    OnPropertyChanged("RoundDates");
                }
            }
        }
        #endregion
        #endregion


        #region Не сериализуемые свойства
        private enSecondColNameType m_SecondColNameType = enSecondColNameType.None;
        /// <summary>
        /// Тип второй колонки.
        /// Если название колонки в файле неверное, то поле будет иметь значение enSecondColNameType.None.
        /// </summary>
        [XmlIgnore]
        public enSecondColNameType SecondColNameType
        {
            get { return m_SecondColNameType; }
            private set
            {
                m_SecondColNameType = value;
                OnPropertyChanged("SecondColNameType");
            }
        }

        [XmlIgnore]
        public List<KeyValuePair<string, string>> RoundDates
        {
            get
            {
                if (EndDate == null)
                    return null;

                List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
                result.Add(new KeyValuePair<string, string>(GlobalDefines.QUALIF_NODE_NAME, QualifDate));
                result.Add(new KeyValuePair<string, string>(GlobalDefines.QUALIF2_NODE_NAME, QualifDate));
                result.Add(new KeyValuePair<string, string>(GlobalDefines.ONE_EIGHTH_FINAL_NODE_NAME, OneEighthFinalDate));
                result.Add(new KeyValuePair<string, string>(GlobalDefines.QUATER_FINAL_NODE_NAME, QuaterFinalDate));
                result.Add(new KeyValuePair<string, string>(GlobalDefines.SEMI_FINAL_NODE_NAME, SemiFinalDate));
                result.Add(new KeyValuePair<string, string>(GlobalDefines.FINAL_NODE_NAME, FinalDate));

                return result;
            }
        }
        #endregion


        public CCompSettings()
        {
            CXMLSerializerBase.SetDefaultValsForAllProps(this);
        }


        /// <summary>
        /// Конструктор копирования
        /// </summary>
        /// <param name="rhs"></param>
        public CCompSettings(CCompSettings rhs) :
            base(rhs)
        {
        }


        public CCompSettings(groups GroupInDB)
        {
            CompName = GroupInDB.descriptions.name;

            AgeGroup = new CAgeGroup()
            {
                Name = GroupInDB.name,
                StartYear = GroupInDB.start_year.HasValue ? GroupInDB.start_year.Value : GlobalDefines.DEFAULT_XML_INT_VAL,
                Sex = GroupInDB.sex
            };

            StartDate = new Scanning.XMLDataClasses.CCompDate()
            {
                Day = GroupInDB.comp_start_date.Day,
                Month = GroupInDB.comp_start_date.Month - 1,
                Year = GroupInDB.comp_start_date.Year
            };

            SecondColName = GroupInDB.second_col_name;

            MainJudge = GroupInDB.main_judge;
            MainSecretary = GroupInDB.main_secretary;
            Row6 = GroupInDB.row6;

            if (GroupInDB.from_1_qualif.HasValue)
                MembersFrom1stQualif = GroupInDB.from_1_qualif.Value;

            if (GroupInDB.from_2_qualif.HasValue)
                MembersFrom2ndQualif = GroupInDB.from_2_qualif.Value;

            if (GroupInDB.end_year.HasValue)
            {
                switch (GroupInDB.end_year)
                {
                    case (int)CAgeGroup.enEndYearSpecVals.AndElder:
                        AgeGroup.EndYear = Properties.Resources.resAndElder;
                        break;

                    case (int)CAgeGroup.enEndYearSpecVals.AndYounger:
                        AgeGroup.EndYear = Properties.Resources.resAndYounger;
                        break;

                    default:
                        AgeGroup.EndYear = GroupInDB.end_year.Value.ToString();
                        break;
                }
            }

            if (GroupInDB.comp_end_date.HasValue)
            {
                EndDate = new Scanning.XMLDataClasses.CCompDate()
                {
                    Day = GroupInDB.comp_end_date.Value.Day,
                    Month = GroupInDB.comp_end_date.Value.Month - 1,
                    Year = GroupInDB.comp_end_date.Value.Year
                };

                foreach (round_dates RoundDate in GroupInDB.round_dates)
                {
                    switch ((enRounds)RoundDate.round)
                    {
                        case enRounds.Qualif:
                            QualifDate = RoundDate.date;
                            break;

                        case enRounds.OneEighthFinal:
                            OneEighthFinalDate = RoundDate.date;
                            break;

                        case enRounds.QuaterFinal:
                            QuaterFinalDate = RoundDate.date;
                            break;

                        case enRounds.SemiFinal:
                            SemiFinalDate = RoundDate.date;
                            break;

                        case enRounds.Final:
                            FinalDate = RoundDate.date;
                            break;
                    }
                }
            }
        }


        #region Методы сравнения
        /// <summary>
        /// Равны ли все сериализуемые свойства?
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool OnlySerializablePropsEquals(CCompSettings rhs)
        {
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                if (!Attribute.IsDefined(pi, typeof(XmlIgnoreAttribute)))
                {   // Свойство сериализуется
                    object lhsVal = pi.GetValue(this, null);
                    object rhsVal = pi.GetValue(rhs, null);
                    if (!(lhsVal == null && rhsVal == null))
                    {
                        if (lhsVal == null || rhsVal == null)
                            return false;
                        else if (!lhsVal.Equals(rhsVal))
                            return false;
                    }
                }
            }

            return true;
        }


        public bool GroupPropsEquals(groups rhs, string FullFilePath)
        {
            bool result = SecondColName == rhs.second_col_name &&
                        MainJudge == rhs.main_judge &&
                        MainSecretary == rhs.main_secretary &&
                        Row6 == rhs.row6 &&
                        rhs.xml_file_name == FullFilePath &&
                        (MembersFrom1stQualif == rhs.from_1_qualif ||
                            (MembersFrom1stQualif == GlobalDefines.DEFAULT_XML_BYTE_VAL && rhs.from_1_qualif == null)) &&
                        (MembersFrom2ndQualif == rhs.from_2_qualif ||
                            (MembersFrom2ndQualif == GlobalDefines.DEFAULT_XML_BYTE_VAL && rhs.from_2_qualif == null));
            if (result)
            {
                if (AgeGroup == null)
                {   // В текущих нстройках группы нет
                    result = string.IsNullOrWhiteSpace(rhs.name) &&
                            (rhs.start_year == null || rhs.start_year < GlobalDefines.MIN_GROUP_YEAR || rhs.start_year > GlobalDefines.MAX_GROUP_YEAR) &&
                            (rhs.end_year == null || rhs.end_year < GlobalDefines.MIN_GROUP_YEAR || rhs.end_year > GlobalDefines.MAX_GROUP_YEAR);
                }
                else
                {
                    result = rhs == AgeGroup;
                }
            }

            if (result)
            {
                if (StartDate == null)
                {   // Начальной даты нет
                    result = rhs.comp_start_date == null;
                }
                else
                {
                    result = StartDate.Date == rhs.comp_start_date;
                }
            }

            if (result)
            {
                if (EndDate == null)
                {   // Начальной даты нет
                    result = rhs.comp_end_date == null;
                }
                else
                {
                    result = EndDate.Date == rhs.comp_end_date;
                }
            }

            return result && RoundDatesEquals(rhs);
        }


        public bool DescriptionPropsEquals(CCompSettings rhs)
        {
            return CompName == rhs.CompName;
        }


        public bool DescriptionPropsEquals(descriptions rhs)
        {
            return CompName == rhs.name;
        }


        public bool RoundDatesEquals(groups rhs)
        {
            bool result = true;

            List<KeyValuePair<string, string>> roundDates = RoundDates; // Чтобы свойство вычислялось только 1 раз
            if (roundDates == null || roundDates.Count == 0)
            {
                result = rhs.round_dates == null || rhs.round_dates.Count == 0;
            }
            else
            {
                foreach (KeyValuePair<string, string> RoundDate in roundDates)
                {
                    round_dates DateInDB = rhs.round_dates.FirstOrDefault(arg => arg.round == GlobalDefines.ROUND_IDS[RoundDate.Key]);
                    if (RoundDate.Value == null)
                        result = DateInDB == null;
                    else
                        result = RoundDate.Value == DateInDB.date;

                    if (!result)
                        break;
                }
            }

            return result;
        }


        public bool RoundDatesEquals(CCompSettings rhs)
        {
            bool result = true;

            List<KeyValuePair<string, string>> roundDates = RoundDates; // Чтобы свойство вычислялось только 1 раз

            if (roundDates == null || roundDates.Count == 0)
            {
                result = rhs.RoundDates == null || rhs.RoundDates.Count == 0;
            }
            else
            {
                List<KeyValuePair<string, string>?> roundDatesrhs = rhs.RoundDates.Cast<KeyValuePair<string, string>?>().ToList(); // Чтобы свойство вычислялось только 1 раз
                foreach (KeyValuePair<string, string> RoundDate in roundDates)
                {
                    KeyValuePair<string, string>? RoundDaterhs = roundDatesrhs.FirstOrDefault(arg => arg.Value.Key == RoundDate.Key);
                    if (RoundDate.Value == null)
                        result = RoundDaterhs == null;
                    else
                        result = RoundDate.Value == RoundDaterhs.Value.Value;

                    if (!result)
                        break;
                }
            }

            return result;
        }


        public override bool Equals(object o)
        {
            if (o is CCompSettings)
            {
                CCompSettings rhs = o as CCompSettings;
                return OnlySerializablePropsEquals(rhs) && RoundDatesEquals(rhs);
            }

            return false;
        }


        public override int GetHashCode()
        {
            object[] KeyFields = new object[] { CompName,
                                                SecondColName,
                                                MainJudge,
                                                MainSecretary,
                                                Row6,
                                                MembersFrom1stQualif,
                                                MembersFrom2ndQualif,
                                                RoundDates,
                                                AgeGroup,
                                                StartDate,
                                                EndDate };
            return KeyFields.GetHashCode();
        }

        #endregion
    }
}
