using DBManager.Global;
using DBManager.RoundMembers.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
    public class CMemberInTotal : CDBAdditionalClassBase, ICanRefreshFrom
    {
        private static readonly GradeMarkupConverter m_convGrade = new GradeMarkupConverter();
        private static readonly Dictionary<enRounds, string> PropertyNames = new Dictionary<enRounds, string>();
        private readonly Dictionary<enRounds, COneRoundResults> m_RoundResults = new Dictionary<enRounds, COneRoundResults>();

        #region MemberInfo
        private static readonly string MemberInfoPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.MemberInfo);

        private CFullMemberInfo m_MemberInfo = null;
        /// <summary>
        /// Сведения об участнике
        /// </summary>
        public CFullMemberInfo MemberInfo
        {
            get { return m_MemberInfo; }
            set
            {
                if (m_MemberInfo != value)
                {
                    m_MemberInfo = value;
                    OnPropertyChanged(MemberInfoPropertyName);
                }
            }
        }
        #endregion

        #region QualifResults
        private static readonly string QualifResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.QualifResults);

        public COneRoundResults QualifResults
        {
            get { return GetResultsForRound(enRounds.Qualif); }
            set
            {
                if (GetResultsForRound(enRounds.Qualif) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.Qualif);
                    else
                        m_RoundResults[enRounds.Qualif] = value;

                    OnPropertyChanged(QualifResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region Qualif2Results
        private static readonly string Qualif2ResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.Qualif2Results);

        public COneRoundResults Qualif2Results
        {
            get { return GetResultsForRound(enRounds.Qualif2); }
            set
            {
                if (GetResultsForRound(enRounds.Qualif2) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.Qualif2);
                    else
                        m_RoundResults[enRounds.Qualif2] = value;

                    OnPropertyChanged(Qualif2ResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region OneEighthFinalResults
        private static readonly string OneEighthFinalResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.OneEighthFinalResults);

        public COneRoundResults OneEighthFinalResults
        {
            get { return GetResultsForRound(enRounds.OneEighthFinal); }
            set
            {
                if (GetResultsForRound(enRounds.OneEighthFinal) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.OneEighthFinal);
                    else
                        m_RoundResults[enRounds.OneEighthFinal] = value;

                    OnPropertyChanged(OneEighthFinalResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region QuaterFinalResults
        private static readonly string QuaterFinalResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.QuaterFinalResults);

        public COneRoundResults QuaterFinalResults
        {
            get { return GetResultsForRound(enRounds.QuaterFinal); }
            set
            {
                if (GetResultsForRound(enRounds.QuaterFinal) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.QuaterFinal);
                    else
                        m_RoundResults[enRounds.QuaterFinal] = value;

                    OnPropertyChanged(QuaterFinalResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region SemiFinalResults
        private static readonly string SemiFinalResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.SemiFinalResults);

        public COneRoundResults SemiFinalResults
        {
            get { return GetResultsForRound(enRounds.SemiFinal); }
            set
            {
                if (GetResultsForRound(enRounds.SemiFinal) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.SemiFinal);
                    else
                        m_RoundResults[enRounds.SemiFinal] = value;

                    OnPropertyChanged(SemiFinalResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region FinalResults
        private static readonly string FinalResultsPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.FinalResults);

        public COneRoundResults FinalResults
        {
            get { return GetResultsForRound(enRounds.Final); }
            set
            {
                if (GetResultsForRound(enRounds.Final) != value)
                {
                    if (value == null)
                        m_RoundResults.Remove(enRounds.Final);
                    else
                        m_RoundResults[enRounds.Final] = value;

                    OnPropertyChanged(FinalResultsPropertyName);
                    OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
                }
            }
        }
        #endregion

        #region TotalGrade
        private static readonly string TotalGradePropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.TotalGrade);

        private byte? m_TotalGrade = null;

        public byte? TotalGrade
        {
            get { return m_TotalGrade; }
            set
            {
                if (m_TotalGrade != value)
                {
                    m_TotalGrade = value;

                    TotalGradeForShow = m_convGrade.Convert(m_TotalGrade, TotalGradeForShow.GetType(), null, CultureInfo.CurrentCulture) as string;

                    OnPropertyChanged(TotalGradePropertyName);
                }
            }
        }
        #endregion

        #region BallsForPlaces
        private static readonly string BallsForPlacesPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.BallsForPlaces);

        private float? m_BallsForPlaces = null;

        public float? BallsForPlaces
        {
            get { return m_BallsForPlaces; }
            set
            {
                if (m_BallsForPlaces != value)
                {
                    m_BallsForPlaces = value;
                    OnPropertyChanged(BallsForPlacesPropertyName);
                }
            }
        }
        #endregion

        /// <summary>
        /// Вспомогательное поле
        /// </summary>
        public long id_part = 0;

        #region IsLastMemberInAnyRound
        private static readonly string IsLastMemberInAnyRoundPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.IsLastMemberInAnyRound);

        public bool IsLastMemberInAnyRound
        {
            get { return AllFilledResults.Any(arg => arg.IsLastMember); }
        }
        #endregion

        #region Вместо конвертеров
        #region TotalGradeForShow
        private static readonly string TotalGradeForShowPropertyName = GlobalDefines.GetPropertyName<CMemberInTotal>(m => m.TotalGradeForShow);

        private string m_TotalGradeForShow = "";

        public string TotalGradeForShow
        {
            get { return m_TotalGradeForShow; }
            set
            {
                if (m_TotalGradeForShow != value)
                {
                    m_TotalGradeForShow = value;
                    OnPropertyChanged(TotalGradeForShowPropertyName);
                }
            }
        }
        #endregion
        #endregion

        #region Конструкторы

        static CMemberInTotal()
        {
            PropertyNames.Add(enRounds.Qualif, QualifResultsPropertyName);
            PropertyNames.Add(enRounds.Qualif2, Qualif2ResultsPropertyName);
            PropertyNames.Add(enRounds.OneEighthFinal, OneEighthFinalResultsPropertyName);
            PropertyNames.Add(enRounds.QuaterFinal, QuaterFinalResultsPropertyName);
            PropertyNames.Add(enRounds.SemiFinal, SemiFinalResultsPropertyName);
            PropertyNames.Add(enRounds.Final, FinalResultsPropertyName);
        }

        #endregion

        public override void RefreshFrom(ICanRefreshFrom rhs,
                                        bool SkipNullsForObjects,
                                        bool SkipNullsForNullables)
        {
            base.RefreshFrom(rhs, SkipNullsForObjects, SkipNullsForNullables);

            CMemberInTotal rhsMemberInTotal = rhs as CMemberInTotal;

            if (rhsMemberInTotal == null)
                return;

            if (MemberInfo == null)
                MemberInfo = rhsMemberInTotal.MemberInfo;
            else if (rhsMemberInTotal.MemberInfo == null)
            {
                if (!SkipNullsForObjects)
                    MemberInfo = null;
            }
            else
                MemberInfo.RefreshFrom(rhsMemberInTotal.MemberInfo, SkipNullsForObjects, SkipNullsForNullables);

            if (QualifResults == null)
                QualifResults = rhsMemberInTotal.QualifResults;
            else if (rhsMemberInTotal.QualifResults == null)
            {
                if (!SkipNullsForObjects)
                    QualifResults = null;
            }
            else
                QualifResults.RefreshFrom(rhsMemberInTotal.QualifResults, SkipNullsForObjects, SkipNullsForNullables);

            if (Qualif2Results == null)
                Qualif2Results = rhsMemberInTotal.Qualif2Results;
            else if (rhsMemberInTotal.Qualif2Results == null)
            {
                if (!SkipNullsForObjects)
                    Qualif2Results = null;
            }
            else
                Qualif2Results.RefreshFrom(rhsMemberInTotal.Qualif2Results, SkipNullsForObjects, SkipNullsForNullables);

            if (OneEighthFinalResults == null)
                OneEighthFinalResults = rhsMemberInTotal.OneEighthFinalResults;
            else if (rhsMemberInTotal.OneEighthFinalResults == null)
            {
                if (!SkipNullsForObjects)
                    OneEighthFinalResults = null;
            }
            else
                OneEighthFinalResults.RefreshFrom(rhsMemberInTotal.OneEighthFinalResults, SkipNullsForObjects, SkipNullsForNullables);

            if (QuaterFinalResults == null)
                QuaterFinalResults = rhsMemberInTotal.QuaterFinalResults;
            else if (rhsMemberInTotal.QuaterFinalResults == null)
            {
                if (!SkipNullsForObjects)
                    QuaterFinalResults = null;
            }
            else
                QuaterFinalResults.RefreshFrom(rhsMemberInTotal.QuaterFinalResults, SkipNullsForObjects, SkipNullsForNullables);

            if (SemiFinalResults == null)
                SemiFinalResults = rhsMemberInTotal.SemiFinalResults;
            else if (rhsMemberInTotal.SemiFinalResults == null)
            {
                if (!SkipNullsForObjects)
                    SemiFinalResults = null;
            }
            else
                SemiFinalResults.RefreshFrom(rhsMemberInTotal.SemiFinalResults, SkipNullsForObjects, SkipNullsForNullables);

            if (FinalResults == null)
                FinalResults = rhsMemberInTotal.FinalResults;
            else if (rhsMemberInTotal.FinalResults == null)
            {
                if (!SkipNullsForObjects)
                    FinalResults = null;
            }
            else
                FinalResults.RefreshFrom(rhsMemberInTotal.FinalResults, SkipNullsForObjects, SkipNullsForNullables);


            if (!SkipNullsForNullables || rhsMemberInTotal.TotalGrade.HasValue)
                TotalGrade = rhsMemberInTotal.TotalGrade;

            if (!SkipNullsForNullables || rhsMemberInTotal.BallsForPlaces.HasValue)
                BallsForPlaces = rhsMemberInTotal.BallsForPlaces;

            id_part = rhsMemberInTotal.id_part;
        }


        public IEnumerable<COneRoundResults> AllFilledResults
        {
            get
            {
                if (QualifResults != null)
                    yield return QualifResults;
                if (Qualif2Results != null)
                    yield return Qualif2Results;
                if (OneEighthFinalResults != null)
                    yield return OneEighthFinalResults;
                if (QuaterFinalResults != null)
                    yield return QuaterFinalResults;
                if (SemiFinalResults != null)
                    yield return SemiFinalResults;
                if (FinalResults != null)
                    yield return FinalResults;
            }
        }


        public COneRoundResults GetResultsForRound(byte RoundId)
        {
            return GetResultsForRound((enRounds)RoundId);
        }


        public COneRoundResults GetResultsForRound(enRounds round)
        {
            COneRoundResults result = null;
            if (!m_RoundResults.TryGetValue(round, out result))
                result = null;

            return result;
        }


        public void SetResultsForRound(byte RoundId, COneRoundResults Results)
        {
            SetResultsForRound((enRounds)RoundId, Results);
        }


        public void SetResultsForRound(enRounds round, COneRoundResults Results)
        {
            m_RoundResults[round] = Results;
            OnPropertyChanged(PropertyNames[round]);
            OnPropertyChanged(IsLastMemberInAnyRoundPropertyName);
        }
    }
}
