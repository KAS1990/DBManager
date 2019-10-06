using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global;
using DBManager.Excel.Exporting.Tabs;
using MSExcel = Microsoft.Office.Interop.Excel;
using DBManager.Scanning.XMLDataClasses;
using System.Windows;
using DBManager.RoundMembers.Converters;
using System.Globalization;
using DBManager.ReportGenerators;

namespace DBManager.Excel.Exporting.ExportingClasses
{
    public class CPersonalExporter : CReportExporterBase
    {
        #region Номера столбцов в листе Excel
        private int EXCEL_PLACE_COL_NUM = 1;
        private int EXCEL_PERSONAL_COL_NUM = 2;
        private int EXCEL_TEAM_COL_NUM = 3;
        private int EXCEL_YEAR_OF_BIRTH_COL_NUM = 4;
        private int EXCEL_GRADE_COL_NUM = 5;
        #endregion

        protected string RN_RESULT = "Result";
        protected string RN_LEAD = "Lead";
        protected string RN_SPEED = "Speed";


        public class CPersonalTask : CReportExporterBase.CTask
        {
            public enPersRepPlacesAggregationMethod m_PlaceAggregationMethod;
            public enPersRepWinnerDetection m_WinnerDetection;
            public enPriorityCompetitionKind m_PriorityCompetitionKind;
            public List<CGroupItem> m_Groups;

            /// <summary>
            /// Настройки протокола трудности
            /// </summary>
            public CLeadReportInfo m_LeadReportInfo = new CLeadReportInfo();
        }
        
        
        CPersonalTask CurTask
        {
            get
            {
                if (m_Task is CPersonalTask)
                    return m_Task as CPersonalTask;
                else
                    throw new InvalidCastException("Invalid of type m_Task in DBManager.Excel.Exporting.ExportingClasses.CPersonalExporter. Requires CPersonalExporter.CPersonalTask");
            }
        }

                
        public CPersonalExporter(CTask Task) :
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
            CPersonalResultsComparer Comparer = new CPersonalResultsComparer()
            {
                CompareProperty = CurTask.m_WinnerDetection,
                CompareProperty1 = CurTask.m_PriorityCompetitionKind
            };

            foreach (CGroupItem GroupItem in CurTask.m_Groups)
            {
                CGroupItem LeadGroupItem = null;

                MSExcel.Worksheet wsh = null;
                lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
                {
                    wsh = wbkTemplates.Worksheets[DBManagerApp.m_AppSettings.m_Settings.ExcelSettings.dictReportTemplates[enReportTypes.Personal]];
                }
                wsh.Copy(After: wbkTarget.Worksheets[wbkTarget.Worksheets.Count]);

                // Лист, в который нужно будет вставлять данные
                wsh = wbkTarget.Worksheets[wbkTarget.Worksheets.Count];
                wsh.Name = GroupItem.SheetName;

                groups GroupInDB = CurTask.m_CompDesc.groups.First(arg => arg.id_group == GroupItem.id);
                CCompSettings CompSettings = new CCompSettings(GroupInDB);

                wsh.Range[RN_COMP_NAME].Value = CompSettings.CompName;
                                
                wsh.Range[RN_MAIN_JUDGE].Value = CompSettings.MainJudge;
                wsh.Range[RN_MAIN_SECRETARY].Value = CompSettings.MainSecretary;
                wsh.Range[RN_SECOND_COL_NAME].Value = CompSettings.SecondColName;
                
                string ReportName = wsh.Range[RN_REPORT_NAME].Value;
                int SelectedStartYear, SelectedEndYear;
                wsh.Range[RN_REPORT_NAME].Value = ReportName.Replace(GlobalDefines.EXCEL_REPORT_NAME_TEXT_TO_REPLACE,
                                                                    CreateGroupName(GroupItem,
                                                                                    CompSettings.AgeGroup,
                                                                                    out SelectedStartYear,
                                                                                    out SelectedEndYear));

                // Формируем список спортсменов, принявших участие в соревновании в данной возрастной группе

                List<CPersonalItem> lstResults = (from member in DBManagerApp.m_Entities.members
                                                  join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
                                                  where part.Group == GroupInDB.id_group &&
                                                        part.result_place.HasValue
                                                  orderby part.result_place
                                                  select new CPersonalItem
                                                  {
                                                      m_SurnameAndName = member.surname + " " + member.name,
                                                      m_Team = CompSettings.SecondColNameType == enSecondColNameType.Coach ? part.coach : part.team,
                                                      m_YearOfBirth = member.year_of_birth,
                                                      m_Grade = part.init_grade
                                                  }).ToList();

                /* Расставляем места в скорости:
                 * при фильтрации по годам рождения места должны быть не общими, а в рамках выбранных годов рождения */
                for (int i = 0; i < lstResults.Count;)
                {
                    if (lstResults[i].m_YearOfBirth >= SelectedStartYear && lstResults[i].m_YearOfBirth <= SelectedEndYear)
                    {
                        lstResults[i].m_SpeedPlace = i + 1;
                        i++;
                    }
                    else
                    {
                        lstResults.RemoveAt(i);
                    }
                }

                bool HasGroupInLead = CurTask.m_LeadReportInfo.m_wbkLeadReport != null &&
                                        CurTask.m_LeadReportInfo.m_dictLeadGroupInfos.TryGetValue(GroupItem.id, out LeadGroupItem) &&
                                        LeadGroupItem.LeadSheetIndex >= 0 &&
                                        LeadGroupItem.LeadSheetIndex < CurTask.m_LeadReportInfo.m_LeadSheets.Count;

                DateTime? StartDate = CompSettings.StartDate.Date;
                DateTime? EndDate = null;
                if (CompSettings.EndDate != null)
                    EndDate = CompSettings.EndDate.Date;

                if (HasGroupInLead)
                {	// Такая группа есть в протоколе трудности
                    // Трудность есть => учитываем её при подсчёте дат
                    if (CompSettings.StartDate.Date > LeadGroupItem.StartDate)
                        StartDate = LeadGroupItem.StartDate;

                    if (EndDate == null)
                    {
                        if (LeadGroupItem.EndDate.HasValue)
                            EndDate = LeadGroupItem.EndDate;
                    }
                    else
                    {
                        if (LeadGroupItem.EndDate.HasValue && EndDate < LeadGroupItem.EndDate)
                            EndDate = LeadGroupItem.EndDate;
                    }

                    MSExcel.Worksheet wshLead = CurTask.m_LeadReportInfo.m_wbkLeadReport.Worksheets[LeadGroupItem.LeadSheetIndex + 1];

                    // Просматриваем всех участников трудности и ищем их в скорости
                    int skippedQ = 0;
                    for (int Row = 0;
                        wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value != null &&
                        !string.IsNullOrWhiteSpace(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value.ToString());
                        Row++)
                    {
                        int PlaceInLead;
                        if (!int.TryParse(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PlaceColumnIndex].Value.ToString(), out PlaceInLead))
                        {   // У участника какое-то неонятное место => пропускаем
                            skippedQ++;
                            continue;
                        }

                        // Ищем участника в скорости
                        string[] arrNameAndSurname;
                        string NameAndSurnameInLead = GlobalDefines.CorrectSurnameAndName(wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_PersonalDataColumnIndex].Value,
                                                                                            out arrNameAndSurname);
                        int YoBInLead;
                        string strYoBInLead = wshLead.Cells[Row + CurTask.m_LeadReportInfo.m_FirstMemberRow, CurTask.m_LeadReportInfo.m_YearOfBirthColumnIndex].Value.ToString();
                        CPersonalItem item = null;
                        if (!int.TryParse(strYoBInLead, out YoBInLead))
                        {   // Какой-то неверный год рождения => пропускаем
                            skippedQ++;
                            MessageBox.Show(string.Format(Properties.Resources.resfmtInvalidYoBInLead,
                                                            NameAndSurnameInLead,
                                                            strYoBInLead),
                                            DBManagerApp.MainWnd.Title,
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                            continue;
                        }
                        
                        item = lstResults.Find(arg => arg.m_SurnameAndName == NameAndSurnameInLead && arg.m_YearOfBirth == YoBInLead);
                        if (item == null)
                        {   // Не нашли => он не может участвовать в многоборье
                            if (!(YoBInLead >= SelectedStartYear && YoBInLead <= SelectedEndYear))
                                skippedQ++;
                            continue;
                        }
                        item.m_LeadPlace = PlaceInLead - skippedQ;

                        // Определяем m_ResultPlace
                        switch (CurTask.m_PlaceAggregationMethod)
                        {
                            case enPersRepPlacesAggregationMethod.Sum:
                                item.m_TotalPlace = item.m_LeadPlace + item.m_SpeedPlace;
                                break;
                            
                            case enPersRepPlacesAggregationMethod.Mul:
                                item.m_TotalPlace = item.m_LeadPlace * item.m_SpeedPlace;
                                break;
                        }
                    }

                    // Оставляем только тех, кто участвовал и в трудности и скорости
                    lstResults.RemoveAll(arg => arg.m_TotalPlace == -1);
                    lstResults.Sort((lhs, rhs) => (lhs.m_TotalPlace.CompareTo(rhs.m_TotalPlace)));
                    lstResults.Add(new CPersonalItem()); // Добавляем барьерный элемент

                    // Определяем места участников в двоеборье
                    int TrainStartIndex = 0;
                    List<CPersonalItem> Train = new List<CPersonalItem>(); // Участники, входящие в паровоз
                    for (int i = 0; i < lstResults.Count - 1; i++)
                    {
                        Train.Add(lstResults[i]);

                        if (lstResults[i].m_TotalPlace != lstResults[i + 1].m_TotalPlace)
                        {	// Паровоз закончился => определяем места у всех его членов
                            if (Train.Count > 1)
                            {
                                Train.Sort(Comparer); // Сортируем паровоз по дополнительному критерию
                                int CurPlace = TrainStartIndex + 1;
                                CPersonalItem PrevResult = null;
                                foreach (CPersonalItem TrainItem in Train)
                                {
                                    if (Comparer.Compare(TrainItem, PrevResult) != 0)
                                        CurPlace = TrainStartIndex + 1;
                                    TrainItem.m_Place = CurPlace;
                                    PrevResult = TrainItem;
                                    TrainStartIndex++;
                                }
                            }
                            else
                            {	// паровоз состоит из 1 человека - lstResults[i]
                                lstResults[i].m_Place = ++TrainStartIndex;
                            }

                            Train.Clear();
                        }
                    }
                    lstResults.RemoveAt(lstResults.Count - 1);

                    // Окончательно сортируем участников
                    lstResults.Sort((lhs, rhs) => (lhs.m_Place.CompareTo(rhs.m_Place)));
                }
                                                
                // Выводим полученные места в протокол двоеборья
                int FirstRowInSpeed = wsh.Range[RN_FIRST_DATA_ROW].Row;
                int RowInSpeed = 0;
                foreach (CPersonalItem PersonalItem in lstResults)
                {
                    if (PersonalItem.m_Place > 0)
                        wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_PLACE_COL_NUM].Value = GlobalDefines.EncodePlace((byte)PersonalItem.m_Place);
                    else
                        wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_PLACE_COL_NUM].Value = null;
                    wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_PERSONAL_COL_NUM].Value = PersonalItem.m_SurnameAndName;
                    if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
                        wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == PersonalItem.m_Team).name;
                    else
                        wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_TEAM_COL_NUM].Value = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == PersonalItem.m_Team).name;
                    wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_YEAR_OF_BIRTH_COL_NUM].Value = PersonalItem.m_YearOfBirth;

                    GradeMarkupConverter conv = new GradeMarkupConverter();
                    wsh.Cells[FirstRowInSpeed + RowInSpeed, EXCEL_GRADE_COL_NUM].Value = conv.Convert(PersonalItem.m_Grade,
                                                                                                        PersonalItem.m_Grade.GetType(),
                                                                                                        null,
                                                                                                        CultureInfo.GetCultureInfo(GlobalDefines.RUSSIAN_CULTURE_NAME));

                    wsh.Range[RN_SPEED].Offset[RowInSpeed].Value = PersonalItem.m_SpeedPlace;
                    if (PersonalItem.m_LeadPlace > 0)
                        wsh.Range[RN_LEAD].Offset[RowInSpeed].Value = PersonalItem.m_LeadPlace;
                    if (PersonalItem.m_TotalPlace > 0)
                        wsh.Range[RN_RESULT].Offset[RowInSpeed].Value = PersonalItem.m_TotalPlace;
                    else
                    {	// Вставляем формулу, чтобы потом было бастрее работать с протоколом
                        switch (CurTask.m_PlaceAggregationMethod)
                        {
                            case enPersRepPlacesAggregationMethod.Sum:
                                wsh.Range[RN_RESULT].Offset[RowInSpeed].FormulaR1C1 = "=RC[-2] + RC[-1]";
                                break;

                            case enPersRepPlacesAggregationMethod.Mul:
                                wsh.Range[RN_RESULT].Offset[RowInSpeed].FormulaR1C1 = "=RC[-2] * RC[-1]";
                                break;
                        }
                    }

                    RowInSpeed++;
                }

                // Удаляем лишние строки
                wsh.Rows[(FirstRowInSpeed + RowInSpeed).ToString() + ":" + (EXCEL_MAX_LINES_IN_REPORTS + FirstRowInSpeed - 1).ToString()].Delete(MSExcel.XlDirection.xlUp);

                // Выводим дату соревнований в самом конце
                wsh.Range[RN_ROUND_DATE].Value = GlobalDefines.CreateCompDate(StartDate.Value, EndDate);
            }

            return true;
        }
    }
}
