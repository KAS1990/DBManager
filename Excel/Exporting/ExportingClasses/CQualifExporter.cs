using DBManager.Excel.Exporting.Tabs;
using DBManager.Global;
using DBManager.RoundMembers.Converters;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.Scanning.XMLDataClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Excel.Exporting.ExportingClasses
{
    public class CQualifExporter : CReportExporterBase
    {
        #region Номера столбцов в листе Excel
        private readonly int EXCEL_PLACE_COL_NUM = 1;
        private readonly int EXCEL_PERSONAL_COL_NUM = 2;
        private readonly int EXCEL_TEAM_COL_NUM = 3;
        private readonly int EXCEL_YEAR_OF_BIRTH_COL_NUM = 4;
        private readonly int EXCEL_GRADE_COL_NUM = 5;
        private readonly int EXCEL_ROUTE1_COL_NUM = 6;
        private readonly int EXCEL_ROUTE2_COL_NUM = 7;
        private readonly int EXCEL_SUM_COL_NUM = 8;
        #endregion


        public class CQualifTask : CReportExporterBase.CTask
        {
            public CGroupItem m_GroupToExport;
        }

        private CQualifTask CurTask
        {
            get
            {
                if (m_Task is CQualifTask)
                    return m_Task as CQualifTask;
                else
                    throw new InvalidCastException("Invalid of type m_Task in DBManager.Excel.Exporting.ExportingClasses.CQualifExporter. Requires CQualifExporter.CQualifTask");
            }
        }


        public CQualifExporter(CTask Task) :
            base(Task)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelApp">
        /// Приложение Excel, в котором открыты книги
        /// <paramref name="wbkTarget"/> и <paramref name="wbkTemplates"/>
        /// </param>
        /// <param name="wbkTarget">
        /// Книга, в которую добавляется отчёт. Добавление всегда происходит в конец
        /// </param>
        /// <param name="wbkTemplates">
        /// Книга с шаблонами отчётов
        /// </param>
        /// <returns></returns>
        public override bool CreateReport(MSExcel.Application excelApp,
                                            MSExcel.Workbook wbkTarget,
                                            MSExcel.Workbook wbkTemplates)
        {
            bool IsOnlyStartList = CurTask.m_GroupToExport.Rounds.First(arg => arg.id == (enRounds)CurTask.m_ReportType).ExtraOption.Value;

            // Копируем в конец новой книги лист-шаблон
            MSExcel.Worksheet wsh = null;
            lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
            {
                wsh = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictReportTemplates[IsOnlyStartList ? enReportTypes.StartList : CurTask.m_ReportType]];
            }
            wsh.Copy(After: wbkTarget.Worksheets[wbkTarget.Worksheets.Count]);

            // Лист, в который нужно будет вставлять данные
            wsh = wbkTarget.Worksheets[wbkTarget.Worksheets.Count];
            wsh.Name = CurTask.m_GroupToExport.Rounds.First(arg => arg.id == (enRounds)CurTask.m_ReportType).SheetName;

            groups GroupInDB = CurTask.m_CompDesc.groups.First(arg =>
                {
                    return arg.id_group == CurTask.m_GroupToExport.id;
                });
            CCompSettings CompSettings = new CCompSettings(GroupInDB);

            wsh.Range[RN_COMP_NAME].Value = CompSettings.CompName;
            if (CompSettings.RoundDates == null)
                wsh.Range[RN_ROUND_DATE].Value = CompSettings.StartDate.Date.ToLongDateString();
            else
                wsh.Range[RN_ROUND_DATE].Value = CompSettings.QualifDate;
            wsh.Range[RN_MAIN_JUDGE].Value = CompSettings.MainJudge;
            wsh.Range[RN_MAIN_SECRETARY].Value = CompSettings.MainSecretary;
            wsh.Range[RN_SECOND_COL_NAME].Value = CompSettings.SecondColName;
            if (CompSettings.Row6 == GlobalDefines.DEFAULT_XML_STRING_VAL)
                wsh.Rows[6].Delete();
            else
                wsh.Range[RN_ROW_6].Value = CompSettings.Row6;

            string ReportName = wsh.Range[RN_REPORT_NAME].Value;
            int SelectedStartYear, SelectedEndYear;
            wsh.Range[RN_REPORT_NAME].Value = ReportName.Replace(GlobalDefines.EXCEL_REPORT_NAME_TEXT_TO_REPLACE,
                                                                CreateGroupName(CurTask.m_GroupToExport,
                                                                                CompSettings.AgeGroup,
                                                                                out SelectedStartYear,
                                                                                out SelectedEndYear));
            if (CurTask.m_ReportType == enReportTypes.Qualif2)
            {
                ReportName = wsh.Range[RN_REPORT_NAME].Value;
                ReportName = ReportName.Left(ReportName.Length - 1);
                ReportName += " 2.";
                wsh.Range[RN_REPORT_NAME].Value = ReportName;
            }

            if (IsOnlyStartList)
            {   //выделяет жирным СКОРОСТЬ
                ReportName = wsh.Range[RN_REPORT_NAME].Value;
                int BoldTextIndex = ReportName.IndexOf("СКОРОСТЬ") + 1;
                wsh.Range[RN_REPORT_NAME].Characters[BoldTextIndex, "СКОРОСТЬ".Length].Font.Bold = true;
            }

            // Выводим участников соревнования
            List<CMemberAndResults> lstResults = null;
            if (IsOnlyStartList)
            {   // Нужно показать только стартовый протокол
                lstResults = (from member in DBManagerApp.m_Entities.members
                              join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
                              join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
                              where result.round == (byte)CurTask.m_ReportType &&
                                      part.Group == GroupInDB.id_group &&
                                      member.year_of_birth >= SelectedStartYear &&
                                      member.year_of_birth <= SelectedEndYear
                              orderby result.number
                              select new CMemberAndResults
                              {
                                  MemberInfo = new CFullMemberInfo()
                                  {
                                      IDMember = member.id_member,
                                      Surname = member.surname,
                                      Name = member.name,
                                      YearOfBirth = member.year_of_birth,
                                      Coach = part.coach,
                                      Team = part.team,
                                      InitGrade = part.init_grade,
                                  },

                                  Results = new COneRoundResults()
                                  {
                                      m_Round = (enRounds)CurTask.m_ReportType,
                                      Route1 = new CResult()
                                      {
                                          ResultInDB = result,
                                      },
                                      Route2 = new CResult()
                                      {
                                          ResultInDB = result,
                                      },
                                      Sum = new CResult()
                                      {
                                          ResultInDB = result,
                                      },
                                  },

                                  Place = result.place
                              }).ToList();
            }
            else
            {
                lstResults = (from member in DBManagerApp.m_Entities.members
                              join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
                              join result in DBManagerApp.m_Entities.results_speed on part.id_participation equals result.participation
                              where result.round == (byte)CurTask.m_ReportType &&
                                      part.Group == GroupInDB.id_group &&
                                      member.year_of_birth >= SelectedStartYear &&
                                      member.year_of_birth <= SelectedEndYear &&
                                      result.place.HasValue
                              orderby result.place
                              select new CMemberAndResults
                              {
                                  MemberInfo = new CFullMemberInfo()
                                  {
                                      IDMember = member.id_member,
                                      Surname = member.surname,
                                      Name = member.name,
                                      YearOfBirth = member.year_of_birth,
                                      Coach = part.coach,
                                      Team = part.team,
                                      InitGrade = part.init_grade,
                                  },

                                  Results = new COneRoundResults()
                                  {
                                      m_Round = (enRounds)result.round,
                                      Route1 = new CResult()
                                      {
                                          ResultInDB = result,
                                          Time = result.route1,
                                      },
                                      Route2 = new CResult()
                                      {
                                          ResultInDB = result,
                                          Time = result.route2,
                                      },
                                      Sum = new CResult()
                                      {
                                          ResultInDB = result,
                                          Time = result.sum,
                                      },
                                  },

                                  Place = result.place
                              }).ToList();
            }

            int FirstRow = wsh.Range[RN_FIRST_DATA_ROW].Row;
            byte CurPlace = 1;
            byte ResultIndex = 0;
            TimeSpan? PrevResult = null; // Такого результата не может быть
            byte MembersToNextRound = CurTask.m_ReportType == enReportTypes.Qualif ?
                                        CompSettings.MembersFrom1stQualif :
                                        CompSettings.MembersFrom2ndQualif;
            foreach (CMemberAndResults MemberAndResults in lstResults)
            {
                ResultIndex++;

                if (!IsOnlyStartList)
                {
                    if (MemberAndResults.Results.Sum.Time.Value != PrevResult)
                        CurPlace = ResultIndex;

                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_PLACE_COL_NUM].Value = GlobalDefines.EncodePlace(CurPlace);
                }

                wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_PERSONAL_COL_NUM].Value = MemberAndResults.MemberInfo.SurnameAndName;
                if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == MemberAndResults.MemberInfo.Coach).name;
                else
                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == MemberAndResults.MemberInfo.Team).name;
                wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = MemberAndResults.MemberInfo.YearOfBirth;

                GradeMarkupConverter conv = new GradeMarkupConverter();
                wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_GRADE_COL_NUM].Value = conv.Convert(MemberAndResults.MemberInfo.InitGrade,
                                                                                                MemberAndResults.MemberInfo.InitGrade.GetType(),
                                                                                                null,
                                                                                                CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));

                if (!IsOnlyStartList)
                {
                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_ROUTE1_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route1.Time, MemberAndResults.Results.Route1.AdditionalEventTypes);
                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_ROUTE2_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Route2.Time, MemberAndResults.Results.Route2.AdditionalEventTypes);
                    wsh.Cells[ResultIndex + FirstRow - 1, EXCEL_SUM_COL_NUM].Value = GlobalDefines.EncodeSpeedResult(MemberAndResults.Results.Sum.Time, MemberAndResults.Results.Sum.AdditionalEventTypes);

                    PrevResult = MemberAndResults.Results.Sum.Time.Value;
                }
            }

            if (!IsOnlyStartList &&
                MembersToNextRound > 0 &&
                CurTask.m_GroupToExport.StartYearIndToExport == 0 &&
                CurTask.m_GroupToExport.EndYearIndToExport == CurTask.m_GroupToExport.YearsOfBirth.Count - 1)
            {
                MSExcel.Range rng = wsh.Range[RN_FIRST_DATA_ROW].Offset[MembersToNextRound - 1];
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlThick;
            }

            // Удаляем лишние строки
            wsh.Rows[(ResultIndex + FirstRow).ToString() + ":" + (EXCEL_MAX_LINES_IN_REPORTS + FirstRow - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);

            return true;
        }
    }
}
