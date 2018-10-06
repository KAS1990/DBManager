using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Scanning.XMLDataClasses;
using DBManager.Global;
using System.ComponentModel;
using DBManager.Scanning.DBAdditionalDataClasses;
using DBManager.Stuff;

namespace DBManager.Scanning
{
	/// <summary>
	/// Выполняет перенос информации из CSpeedResults в БД для группы соревнований
	/// </summary>
	public class CGroupResultsManager
	{
		public class CGradeStat
		{
			public enGrade? Grade = null;
			public int MembersWithGrade = 0;
		}

		private readonly CScannerBase m_ParentScanner = null;

		/// <summary>
		/// Группа, результаты которой заносит экземпляр класса
		/// </summary>
		private readonly groups m_DBGroup = null;

		/// <summary>
		/// Сравниватель, используемый в сортировках результатов участников
		/// </summary>
		private readonly CSpeedResultsComparer m_SpeedResultsComparer = new CSpeedResultsComparer();

		/// <summary>
		/// Словарь из id_member.
		/// Ключ - Results.SurnameAndName.
		/// Значение 
		/// </summary>
		private Dictionary<string, CMemberKeys> m_MembersIds = new Dictionary<string, CMemberKeys>();
		public Dictionary<string, CMemberKeys> MembersIds
		{
			get { return m_MembersIds; }
			private set { m_MembersIds = value; }
		}


		public CGroupResultsManager(CScannerBase ParentScanner,
									CSpeedResults Results,
									groups DBGroup,
									CCompSettings CompSettings,
									bool IsNewGroup,
									out List<CDataChangedInfo> MadeChanges)
		{
			if (DBGroup == null || DBGroup.id_group < 0)
			{
				throw new ArgumentNullException("DBGroup");
			}

			m_ParentScanner = ParentScanner;
			m_DBGroup = DBGroup;
			MembersIds.Clear();

			if (!(Results.RoundInEnum == enRounds.Qualif && IsNewGroup))
			{	// Нужно инициализировать словарь MembersIds
				foreach (CMemberAndPart info in from part in DBManagerApp.m_Entities.participations
												join member in DBManagerApp.m_Entities.members on part.member equals member.id_member
												where m_DBGroup.id_group == part.Group
												select new CMemberAndPart
												{
													Member = member,
													Participation = part
												})
				{
					CMemberKeys keys = new CMemberKeys(info.Member.name, info.Member.surname)
					{
						Member = info.Member,
						Participation = info.Participation
					};
					MembersIds.Add(keys.SurnameAndName, keys);
				}
			}

			HandleResults(Results, CompSettings, IsNewGroup, out MadeChanges);
		}
						
		private void MakeBallsForPlaces(List<participations> Members)
		{
			// Сортируем всем участников, у которых есть итоговые места по возрастанию мест
			Members = Members.Where(arg => arg.result_place.HasValue).ToList();
			Members.Sort((lhs, rhs) =>
				{
					return lhs.result_place < rhs.result_place ? -1 : (lhs.result_place > rhs.result_place ? 1 : 0);
				});

			// Так как баллы начисляются НЕ БОЛЕЕ, чем 75% участников,
			// т.е. ВЕСЬ паровоз, выходящий за эту границу в расчет не принимается
			int MembersToCalcCount = (Members.Count * 3) / 4;

			// Последнее место участника в той части, где рассчитываем баллы
			byte LastPlace = Members[MembersToCalcCount - 1].result_place.Value;
			int AfterLastBalls = 0; // Число баллов, даваемых за место, следующее после LastPlace
			int SpecialTrainEndInd = 0; // Номер строки, на которой заканчивается паровоз, _
										// выходящий за пределы 30-ого места
			int TrainBallsSumm = 0; // Сумма "баллов за места" участников паровоза
			int TrainMembersQ = 0; // Число спортсменов в паровозе
			int TrainStartInd = 0; // Начальная строка паровоза
			int CurPlace = 0; // Текущее место

			if (MembersToCalcCount < GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS)
			{
				// Проверяем факт того, что паровоз выходит за границу MembersToCalcCount
				if (Members[MembersToCalcCount - 1].result_place == Members[MembersToCalcCount].result_place)
				{
					// Исключаем весь паровоз из расчёта
					MembersToCalcCount--;
					while (Members[MembersToCalcCount - 1].result_place == Members[MembersToCalcCount].result_place &&
						MembersToCalcCount > 0)
					{
						MembersToCalcCount--;
					}
					if (MembersToCalcCount == 0)
					{	// Все заняли первое место, поэтому расчитать баллы нельзя
						return;
					}
				}
			}
			else
			{	// После 30-ого места баллы все равно не начисляются
				MembersToCalcCount = GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS;
			}
						
			if (MembersToCalcCount == GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS)
			{	// В этом случае возможен паровоз, выходящий за границу 30-ого места
				AfterLastBalls = 0; // После 30-ого места баллы не начисляются
				CurPlace = GlobalDefines.LAST_RESULT_PLACE_TO_CALC_BALLS;
				SpecialTrainEndInd = MembersToCalcCount - 1;
				// Поиск конца паровоза
				while (Members[SpecialTrainEndInd].result_place == Members[SpecialTrainEndInd + 1].result_place)
					SpecialTrainEndInd++;
				if (SpecialTrainEndInd >= MembersToCalcCount)
				{	// Есть выходящий за 30-ое место паровоз
					TrainBallsSumm = 0;
					MembersToCalcCount--;
					// Ищем начало паровоза и вычисляем сумму баллов участников паровоза
					while (Members[MembersToCalcCount].result_place == Members[MembersToCalcCount - 1].result_place)
					{
						TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
						MembersToCalcCount--;
						CurPlace--;
					}
					TrainMembersQ = SpecialTrainEndInd - MembersToCalcCount + 1;
					// Расставляем баллы участникам паровоза.
					for (int MemberInd = MembersToCalcCount; MemberInd <= SpecialTrainEndInd; MemberInd++)
					{
						(from member in Members[MemberInd].results_speed
						where member.round == Members[MemberInd].results_speed.Max(arg => arg.round)
						select member).First().balls = GlobalDefines.MakeBalls(TrainBallsSumm, TrainMembersQ);
					}
					MembersToCalcCount--; // Чтобы MembersToCalcCount содержал номер строки с участником перед паровозом 
				}
			}
			else
				AfterLastBalls = GlobalDefines.BALLS_FOR_PLACES[LastPlace + 1];

			// Просмотр всех остальных участников
			CurPlace = 1;
			TrainBallsSumm = 0;
			TrainMembersQ = 1;	// Минимальный паровоз имеет 1 участника
			TrainStartInd = 0;
			for (int MemberInd = 0; MemberInd < MembersToCalcCount; MemberInd++)
			{
				if (Members[MemberInd].result_place != Members[MemberInd + 1].result_place)
				{	// Паровоз закончился
					// Добавляем баллы последнего участника паровоза
					TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
					// Вычитаем число баллов участника "после последнего"
					TrainBallsSumm -= AfterLastBalls * TrainMembersQ;

					for (int TrainInd = TrainStartInd;
						TrainInd < TrainStartInd + TrainMembersQ;
						TrainInd++)
					{
						(from member in Members[TrainInd].results_speed
						 where member.round == Members[TrainInd].results_speed.Max(arg => arg.round)
						 select member).First().balls = GlobalDefines.MakeBalls(TrainBallsSumm, TrainMembersQ);
					}

					TrainStartInd = MemberInd + 1; // Новый паровоз начинается со следующей строки
					TrainMembersQ = 1; // Минимальный паровоз имеет 1 участника
					TrainBallsSumm = 0;
				}
				else
				{	// Паровоз продолжается
					TrainMembersQ++;
					// Добавляем баллы текущего участника паровоза
					TrainBallsSumm += GlobalDefines.BALLS_FOR_PLACES[CurPlace];
				}
  
				CurPlace++; // Каждая новая строка - это новое место
			}
		}


		/// <summary>
		/// Загрузка новых результатов в класс с последующей загрузкой их в БД
		/// </summary>
		/// <param name="roundResults"></param>
		/// <param name="CompSettings"></param>
		/// <param name="IsNewGroup">
		/// true - группа участников только что была добавлена в соревнования, поэтому по-любому нужно внести участников в БД
		/// </param>
		/// <param name="MadeChanges">
		/// Сделанные изменения
		/// </param>
		/// <returns></returns>
		public bool HandleResults(CSpeedResults roundResults,
									CCompSettings CompSettings,
									bool IsNewGroup,
									out List<CDataChangedInfo> MadeChanges)
		{
			MadeChanges = new List<CDataChangedInfo>();

			if (roundResults == null)
				return false;

			byte RoundId = (byte)roundResults.RoundInEnum;

			if (roundResults.RoundInEnum == enRounds.Qualif)
			{	// Заносим результаты квалификации => возможно нужно добавить новых участников или удалить кого-то
				if (IsNewGroup)
				{
					MembersIds.Clear();

					// Добавляем спортсменов в таблицу members
					/* Чтобы не обращаться повторно к БД, создаём словарь,
					 * т.к. DBManagerApp.m_Entities.SaveChanges() заполнит поля id_member и id_participation всех элементов словаря */
					Dictionary<string, CMemberAndPart> AddedMembers = new Dictionary<string, CMemberAndPart>();
					foreach (CMember result in roundResults.Results)
					{
						IEnumerable<members> MemberInDB = from mmbr in DBManagerApp.m_Entities.members
														  where mmbr.name == result.Name && mmbr.surname == result.Surname
														  select mmbr;
						members member = null;
						if (MemberInDB.Count() > 0)
						{	// Такой участник уже есть в БД
							member = MemberInDB.First();
							if (!member.OnlyDataFieldsEqual(result))
							{	// И его данные поменялись => копируем новые данные в существующую запись, новую создавать нельзя!!!
								result.CopyMembersInfoToDB(member);
							}
						}
						else
						{
							member = result;
							DBManagerApp.m_Entities.members.Add(member);
						}
						AddedMembers.Add(result.SurnameAndName,
											new CMemberAndPart() { Member = member });
					}
					DBManagerApp.m_Entities.SaveChanges(); // заполняем их поля id_member

					// Добавляем спортсменов в таблицу participation
					foreach (CMember result in roundResults.Results)
					{
						CMemberAndPart MemberAndPart = AddedMembers[result.SurnameAndName];
						MemberAndPart.Participation = result.ToParticipation(MemberAndPart.Member.id_member,
																			m_DBGroup.id_group,
																			CompSettings.SecondColNameType);
						DBManagerApp.m_Entities.participations.Add(MemberAndPart.Participation);
						
					}
					DBManagerApp.m_Entities.SaveChanges(); // заполняем их поля id_participation

					// Добавляем ключ из этих таблиц в словарь MembersIds
					foreach (CMember result in roundResults.Results)
					{
						CMemberAndPart MemberAndPart = AddedMembers[result.SurnameAndName];
						CMemberKeys keys = new CMemberKeys(result.Name, result.Surname)
						{
							Member = MemberAndPart.Member,
							Participation = MemberAndPart.Participation
						};
						MembersIds.Add(result.SurnameAndName, keys);
					}
					AddedMembers.Clear(); // Этот словарь больше не нужен

					// Добавляем результаты участников
					foreach (CMember result in roundResults.Results)
					{
						DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(MembersIds[result.SurnameAndName].Participation.id_participation,
																						RoundId));
					}
					DBManagerApp.m_Entities.SaveChanges();

					MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
					{
						ChangingType = enDataChangesTypes.AddManyPcs,
						ChangedObjects = enDataChangedObjects.Members | enDataChangedObjects.Results,
						ID = RoundId,
						GroupID = m_DBGroup.id_group
					});
				}
			}
			else
				IsNewGroup = false;

			if (GlobalDefines.IsRoundFinished(m_DBGroup.round_finished_flags, enRounds.Final) ||
				((roundResults.ChangeReason != enChangeReason.crWholeContent) &&
				GlobalDefines.IsRoundFinished(m_DBGroup.round_finished_flags, roundResults.RoundInEnum)))
			{	// Соревнование уже завершено или раунд уже ранее был завершён => менять результаты в нём что-либо нельзя
				return true;
			}

			bool SumChanged = false;
			bool HasChanges = false;

			switch (roundResults.ChangeReason)
			{
				#region crQualifSorted
				case enChangeReason.crQualifSorted: // Расставляем места в первой или второй квалификации
					if (!IsNewGroup)
					{	// Если IsNewGroup == true, то мы уже занесли все результаты ранее
						foreach (CMember result in roundResults.Results)
						{
							CMemberKeys keys;
							if (!MembersIds.TryGetValue(result.SurnameAndName, out keys))
							{
								throw new ArgumentNullException("keys", 
																string.Format("Member with number {0} for round \"{1}\" is not found in DB",
																				result.Number,
																				roundResults.NodeName));
							}

							IEnumerable<results_speed> MemberResultInDB = from results in keys.Participation.results_speed
																		  where results.round == RoundId
																		  select results;
							switch (MemberResultInDB.Count())
							{
								case 0: // У этого участника ещё нет результатов в данном раунде => добавляем их в БД
									DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																				RoundId));
									HasChanges = true;
									SumChanged |= result.SumExt != null && result.SumExt.Time != null;
									break;

								case 1: // Меняем результаты в БД, если они изменились
									enChangedResult Changes = MemberResultInDB.First().UpdateResults(result);
									HasChanges |= Changes != enChangedResult.None;
									SumChanged |= Changes.HasFlag(enChangedResult.SumTime);
									break;

								default:	// у участника не может быть больше одного результата в одном раунде
									throw new InvalidOperationException(string.Format("There are more then 1 result for member \"{0}\" for round \"{1}\"",
																						result.SurnameAndName,
																						roundResults.NodeName));
							}

							result.ClearCondFormating();
						}
					}

					IEnumerable<results_speed> RoundResultsInDb = RoundResults(RoundId,
																				CSpeedResultsComparer.enCompareProperty.Sum);

					if (SumChanged)
					{	// Расставляем места
						if (!StdDefinePlaces(RoundResultsInDb))
							return false;
					}

					foreach (results_speed result in RoundResultsInDb)
						result.ClearCondFormating();
					
					MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
					{
						ChangingType = enDataChangesTypes.QualifSorted,
						ChangedObjects = enDataChangedObjects.Members,
						ChangeReason = roundResults.ChangeReason,
						ID = RoundId,
						GroupID = m_DBGroup.id_group
					});
					break;
				#endregion

				#region crResultsChanged
				case enChangeReason.crResultsChanged:	// Добавляем результаты участников в БД
					if (!IsNewGroup)
					{	// Если IsNewGroup == true, то мы уже занесли все результаты ранее
						foreach (CMember result in roundResults.Results)
						{
							CMemberKeys keys;
							if (result.SurnameAndName == null || !MembersIds.TryGetValue(result.SurnameAndName, out keys))
							{   // Неизвестный участник => просто пропускаем его
								continue;
							}

							IEnumerable<results_speed> MemberResultInDB = from results in keys.Participation.results_speed
																		  where results.round == RoundId
																		  select results;
							switch (MemberResultInDB.Count())
							{
								case 0:	// У этого участника ещё нет результатов в данном раунде => добавляем их в БД
									DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																				RoundId));
									HasChanges = true;
									SumChanged |= result.SumExt != null && result.SumExt.Time != null;
									break;

								case 1: // Меняем результаты в БД, если они изменились
									enChangedResult Changes = MemberResultInDB.First().UpdateResults(result);
									HasChanges |= Changes != enChangedResult.None;
									SumChanged |= Changes.HasFlag(enChangedResult.SumTime);
									break;

								default:	// у участника не может быть больше одного результата в одном раунде
									throw new InvalidOperationException(string.Format("There are more then 1 result for member \"{0}\" for round \"{1}\"",
																						result.SurnameAndName,
																						roundResults.NodeName));
							}
						}

						if (SumChanged)
						{
							switch (roundResults.RoundInEnum)
							{
								case enRounds.Qualif:
								case enRounds.Qualif2:	// Нужно выполнить пересчёт мест
									{
										DBManagerApp.m_Entities.SaveChanges(); // Чтобы сделанные изменения применились

										IEnumerable<results_speed> MemberResultInDB = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Sum);
										// Удаляем места у всех, у кого нет результатов
										foreach (results_speed result in MemberResultInDB.Where(arg => arg.sum == null))
											result.place = null;

										// Расставляем места
										if (!StdDefinePlaces(MemberResultInDB.Where(arg => arg.sum.HasValue)))
											return false;
										break;
									}

								case enRounds.Final: // В Финале нужно расставлять места по мере появления результатов
									DBManagerApp.m_Entities.SaveChanges(); // Чтобы сделанные изменения применились

									DefinePlacesInFinal(RoundResults((int)enRounds.Final, CSpeedResultsComparer.enCompareProperty.Number).ToList());
									break;
							}
						}

						if (HasChanges)
						{	// Что-то поменялось
							MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
							{
								ChangingType = enDataChangesTypes.Changing,
								ChangedObjects = enDataChangedObjects.Results,
								ChangeReason = roundResults.ChangeReason,
								ID = RoundId,
								GroupID = m_DBGroup.id_group
							});
						}
					}
					break;
				#endregion

				#region crRoundFinished
				case enChangeReason.crRoundFinished:	// Раунд завершён => добавляем участников в следующий
					{
						// Список участников раунда RoundId, отсортированный по возрастанию номеров участников
						List<results_speed> lstRoundResults = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Number).ToList();

						// Если раунд завершён, то больше условное форматирование нам не нужно
						foreach (results_speed result in lstRoundResults)
							result.ClearCondFormating();
						m_DBGroup.round_finished_flags = GlobalDefines.SetRoundFinished(m_DBGroup.round_finished_flags, RoundId);

						foreach (CMember result in roundResults.Results)
							result.ClearCondFormating();

						enRounds NextRound = enRounds.None;
						byte bNextRound = 0;
						if (roundResults.Argument != null && GlobalDefines.ROUND_IDS.TryGetValue(roundResults.Argument, out bNextRound))
							NextRound = (enRounds)bNextRound;
						
						switch (roundResults.RoundInEnum)
						{
							#region Qualif и Qualif2
							case enRounds.Qualif:
								m_DBGroup.round_after_qualif = bNextRound;

								goto case enRounds.Qualif2;

							case enRounds.Qualif2:	/* Завершилась квалификация =>
													 * нужно перенести участников в следующий раунд, который указан в NextRound */
								{
									switch (NextRound)
									{
										case enRounds.Qualif2:
											{
												byte MemberNumber = 1;
												for (int i = 0; i < lstRoundResults.Count; i++)
												{				
													if (lstRoundResults[i].place <= CompSettings.MembersFrom1stQualif)
													{	/* Переносим участника во вторую квалификацию
														 * c сохранением последовательности следования участников */
														ReplaceOrAddSpeedResult(new results_speed()
														{
															participation = lstRoundResults[i].participation,
															round = (byte)NextRound,
															number = MemberNumber,
														});

														MemberNumber++;
													}
													else
													{	// Вычисляем итоговое место участника. Оно равно месту, занятому в квалификации
														lstRoundResults[i].participations.result_place = lstRoundResults[i].place;
													}
												}
												break;
											}

										case enRounds.OneEighthFinal:
										case enRounds.QuaterFinal:
										case enRounds.SemiFinal:
										case enRounds.Final:
											{
												byte[] RowsNumbers = GlobalDefines.ROW_SEQUENCE[NextRound];
												m_SpeedResultsComparer.CompareProperty = CSpeedResultsComparer.enCompareProperty.Place;
												m_SpeedResultsComparer.SortDir = ListSortDirection.Ascending;
												lstRoundResults.Sort(m_SpeedResultsComparer); // Сортируем список по возрастанию мест

												for (int i = 0; i < RowsNumbers.Length; i++)
												{
													ReplaceOrAddSpeedResult(new results_speed()
													{
														participation = lstRoundResults[RowsNumbers[i] - 1].participation,
														round = (byte)NextRound,
														number = (byte)(i + 1),
													});
												}

												for (int i = RowsNumbers.Length; i < lstRoundResults.Count; i++)
												{	// Вычисляем итоговое место участника. Оно равно месту, занятому в квалификации
													lstRoundResults[i].participations.result_place = lstRoundResults[i].place;
												}
												break;
											}
									}
									break;
								}
							#endregion

							#region OneEighthFinal и QuaterFinal
							case enRounds.OneEighthFinal:
							case enRounds.QuaterFinal:		/* Завершилcя раунд соревнований =>
															 * расставляем места,
															 * переносим участников в следующий и
															 * расставляем итоговые места */
								SeparateMembersByPairs(lstRoundResults, NextRound);
								break;
							#endregion

							#region SemiFinal
							case enRounds.SemiFinal:	/* Завершилcя полуфинал =>
														 * расставляем места,
														 * переносим участников в следующий */
								{
									// Делим участников на победителей и проигравших
									List<results_speed> lstWinners = new List<results_speed>();
									List<results_speed> lstLoosers = new List<results_speed>();
									for (int i = 0; i < lstRoundResults.Count; i += 2)
									{
										if (lstRoundResults[i].IsWinnerInPair(lstRoundResults[i + 1]))
										{
											lstWinners.Add(lstRoundResults[i]);
											lstLoosers.Add(lstRoundResults[i + 1]);
										}
										else
										{
											lstLoosers.Add(lstRoundResults[i]);
											lstWinners.Add(lstRoundResults[i + 1]);
										}
									}
							
									// Сортируем их по возрастанию результатов
									m_SpeedResultsComparer.CompareProperty = CSpeedResultsComparer.enCompareProperty.Sum;
									m_SpeedResultsComparer.SortDir = ListSortDirection.Ascending;
									lstWinners.Sort(m_SpeedResultsComparer);
									lstLoosers.Sort(m_SpeedResultsComparer);
							
									// Расставляем места.
									// TO DO: добавить учёт одинаковых мест
									for (int i = 0; i < lstWinners.Count; i++)
									{
										lstWinners[i].place = (byte)(i + 1);
										lstLoosers[i].place = (byte)(lstWinners.Count + i + 1);
									}
							
									// Переносим участников в финал
									for (int i = 0; i < lstWinners.Count; i++)
									{
										// Добавление в финал за 3-4
										ReplaceOrAddSpeedResult(new results_speed()
										{
											participation = lstLoosers[i].participation,
											round = (byte)NextRound,
											number = (byte)(i + 1),
										});
										
										// Добавление в финал за 1-2
										ReplaceOrAddSpeedResult(new results_speed()
										{
											participation = lstWinners[i].participation,
											round = (byte)NextRound,
											number = (byte)(i + 3),
										});
									}
									break;
								}
							#endregion

							#region Final
							case enRounds.Final:	// Завершился Финал => нужно расставить места, посчитать баллы и присвоить разряды
								{
									// первые 2 записи - 3-4 место, вторые - 1-2 
									if (lstRoundResults[0].IsWinnerInPair(lstRoundResults[1]))
									{
										lstRoundResults[0].place = 3;
										lstRoundResults[1].place = 4;
									}
									else
									{
										lstRoundResults[0].place = 4;
										lstRoundResults[1].place = 3;
									}

									if (lstRoundResults[2].IsWinnerInPair(lstRoundResults[3]))
									{
										lstRoundResults[2].place = 1;
										lstRoundResults[3].place = 2;
									}
									else
									{
										lstRoundResults[2].place = 2;
										lstRoundResults[3].place = 1;
									}

									/* Расставляем итоговые места.
									 * Это нужно делать только для финалистов, т.к. для остальных участников это было сделано ранее */
									foreach (results_speed result in lstRoundResults)
										result.participations.result_place = result.place;

									int MinAgeToCalcResultGrade = 0;
									lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
									{
										MinAgeToCalcResultGrade = DBManagerApp.m_AppSettings.m_Settings.MinAgeToCalcResultGrade;
									}

									// Присвоение разрядов
									IEnumerable<dynamic> MembersForGradesCalc = from member in DBManagerApp.m_Entities.members
																				join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																				where part.Group == m_DBGroup.id_group
																						&& member.year_of_birth <= (DateTime.Today.Year - MinAgeToCalcResultGrade)
																				select new
																				{
																					member,
																					part
																				};
									lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
									{
										if (DBManagerApp.m_AppSettings.m_Settings.Only75PercentForCalcGrades)
										{   // Учитываем только 75% участников
											int MaxPlace = (int)(Math.Floor(MembersForGradesCalc.Count() * 0.75));
											MembersForGradesCalc = (from memberAndPart in MembersForGradesCalc
																	orderby memberAndPart.part.result_place
																	where memberAndPart.part.result_place <= MaxPlace
																	select memberAndPart);
										}
									}
									Dictionary<enGrade?, int> GradesStat = (from memberAndPart in MembersForGradesCalc
																			group memberAndPart.part by memberAndPart.part.init_grade into MembersGrades
																			 select new CGradeStat
																			 {
																				 Grade = (enGrade?)MembersGrades.Key,
																				 MembersWithGrade = MembersGrades.Count(arg => arg.init_grade == MembersGrades.Key)
																			 }).ToDictionary(key => key.Grade, item => item.MembersWithGrade);
									int tmp = 0;
									
									for (enGrade grade = enGrade.WithoutGrade; grade <= enGrade.Master; grade++)
									{
										if (!GradesStat.TryGetValue(grade, out tmp))
											GradesStat[grade] = 0;
									}

									List<KeyValuePair<enGrade, int>> MinPlaceForNewGrade = new List<KeyValuePair<enGrade, int>>();
									// 1 разряд
									tmp = GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				1.0 * GradesStat[enGrade.Master] +
																				0.8 * GradesStat[enGrade.BeforeMaster] +
																				0.4 * GradesStat[enGrade.Adult1] +
																				0.2 * GradesStat[enGrade.Adult2]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult1, tmp));

									// 2 разряд
									tmp += GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				0.2 * GradesStat[enGrade.Adult1] +
																				0.4 * GradesStat[enGrade.Adult2] +
																				0.2 * GradesStat[enGrade.Adult3]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult2, tmp));

									// 3 разряд
									tmp += GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				0.2 * GradesStat[enGrade.Adult2] +
																				0.4 * GradesStat[enGrade.Adult3] +
																				0.3 * GradesStat[enGrade.Young1]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Adult3, tmp));

									// 1 ю разряд
									tmp += GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				0.2 * GradesStat[enGrade.Adult3] +
																				0.4 * GradesStat[enGrade.Young1] +
																				0.2 * GradesStat[enGrade.Young2]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young1, tmp));

									// 2 ю разряд
									tmp += GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				0.2 * GradesStat[enGrade.Young1] +
																				0.4 * GradesStat[enGrade.Young2] +
																				0.2 * GradesStat[enGrade.Young3]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young2, tmp));

									// 3 ю разряд
									tmp += GlobalDefines.CalcMinPlaceForNewGrade(DBManagerApp.m_AppSettings.m_Settings.ResultGradeCalcMethod, 
																				0.2 * GradesStat[enGrade.Young2] +
																				0.4 * GradesStat[enGrade.Young3] +
																				0.3 * GradesStat[enGrade.WithoutGrade]);
									MinPlaceForNewGrade.Add(new KeyValuePair<enGrade, int>(enGrade.Young3, tmp));

									List<participations> Members = (from member in DBManagerApp.m_Entities.members
																	 join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member
																	 where part.Group == m_DBGroup.id_group
																			&& member.year_of_birth <= (DateTime.Today.Year - MinAgeToCalcResultGrade)
																	 select part).ToList();
									foreach (participations part in Members)
									{
										part.result_grade = null;

										if (part.result_place.HasValue)
										{
											for (int i = 0; i < MinPlaceForNewGrade.Count; i++)
											{
												if (part.result_place <= MinPlaceForNewGrade[i].Value)
												{
													part.result_grade = (byte)MinPlaceForNewGrade[i].Key;
													break;
												}
											}
										}

										foreach (results_speed result in part.results_speed)
											result.balls = null;
									}

									// Расстановка баллов
									MakeBallsForPlaces(Members);
									break;
								}
							#endregion

							default:
								break;
						}

						MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
						{
							ChangingType = enDataChangesTypes.RoundFinished,
							ChangedObjects = enDataChangedObjects.Results,
							ChangeReason = roundResults.ChangeReason,
							ID = RoundId,
							CurVal = NextRound,
							GroupID = m_DBGroup.id_group
						});
					}
					break;
				#endregion

				#region crRowAdded
				case enChangeReason.crRowAdded: // Добавляем участника. Это действие можно делать только из первой квалификации
					byte InsertAfter;
					if (!IsNewGroup && byte.TryParse(roundResults.Argument, out InsertAfter) && roundResults.RoundInEnum == enRounds.Qualif)
					{	// Если IsNewGroup == true, то мы уже занесли все результаты ранее
						CMember InsertedMember = roundResults.Results.FirstOrDefault(arg => arg.Number == InsertAfter + 1);
						IEnumerable<members> InsertedMembersInDB = from member in DBManagerApp.m_Entities.members
																   where member.surname == InsertedMember.Surname && member.name == InsertedMember.Name
																   select member;
						CMemberKeys keys = new CMemberKeys(InsertedMember.Name, InsertedMember.Surname);
						if (InsertedMembersInDB.Count() > 0)
						{	// Такой участник есть в БД
							keys.Member = InsertedMembersInDB.First();
							if (!keys.Member.OnlyDataFieldsEqual(InsertedMember))
							{	// И его данные поменялись => копируем новые данные в существующую запись, новую создавать нельзя!!!
								InsertedMember.CopyMembersInfoToDB(keys.Member);
							}
						}
						else
						{
							keys.Member = InsertedMember;
							DBManagerApp.m_Entities.members.Add(keys.Member);
							DBManagerApp.m_Entities.SaveChanges(); // получаем id_member добавленного участника
						}
						keys.Participation = InsertedMember.ToParticipation(keys.Member.id_member,
																			m_DBGroup.id_group,
																			CompSettings.SecondColNameType);
						MembersIds.Add(InsertedMember.SurnameAndName, keys);

						// Добавляем участника в таблицу participation
						DBManagerApp.m_Entities.participations.Add(keys.Participation);
						DBManagerApp.m_Entities.SaveChanges(); // получаем id_participation добавленного участника

						// Заменяем номера у всех участников, расположенных после добавленного
						foreach (results_speed result in (from part in DBManagerApp.m_Entities.participations
														 join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
														 where results.round == RoundId && m_DBGroup.id_group == part.Group && results.number > InsertAfter
														 select results).ToList())
						{
							result.number++;
						}
								
						// Добавляем участника в results_speed
						DBManagerApp.m_Entities.results_speed.Add(InsertedMember.ToResults_Speed(keys.Participation.id_participation,
																									RoundId));

						if (InsertedMember.SumExt != null && InsertedMember.SumExt.Time != null)
						{	// Нужно пересчитать места
							DBManagerApp.m_Entities.SaveChanges();

							IEnumerable<results_speed> MemberResultInDB = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Sum);
							if (!StdDefinePlaces(MemberResultInDB.Where(arg => arg.sum.HasValue)))
								return false;
						}

						MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
						{
							ChangingType = enDataChangesTypes.Add,
							ChangedObjects = enDataChangedObjects.Members | enDataChangedObjects.Results,
							ChangeReason = roundResults.ChangeReason,
							Argument = InsertAfter,
							ID = keys.Participation.id_participation,
							GroupID = m_DBGroup.id_group
						});
					}
					break;
				#endregion

				#region crRowChanged
				case enChangeReason.crRowChanged: /* Меняем информацию об участнике в таблице members.
												   * Это действие можно делать только из первой квалификации. */
					byte ChangedRow;
					if (!IsNewGroup && byte.TryParse(roundResults.Argument, out ChangedRow) && roundResults.RoundInEnum == enRounds.Qualif)
					{
						CMember ChangedMember = roundResults.Results.FirstOrDefault(arg => arg.Number == ChangedRow);
						IEnumerable<CFullMemberInfo> ChangedMembersInDB = from results in DBManagerApp.m_Entities.results_speed
																			join part in DBManagerApp.m_Entities.participations on results.participation equals part.id_participation
																			join member in DBManagerApp.m_Entities.members on part.member equals member.id_member
																			where results.number == ChangedRow && results.round == RoundId && part.Group == m_DBGroup.id_group
																			select new CFullMemberInfo
																			{
																				IDMember = member.id_member,
																				Name = member.name,
																				Surname = member.surname,
																				YearOfBirth = member.year_of_birth,
																				InitGrade = part.init_grade,
																				Team = part.team,
																				Coach = part.coach,
																			};
						CFullMemberInfo MemberForChangeInDB = ChangedMembersInDB.First(); // Информация об изменённом участнике в БД
						if (MemberForChangeInDB != ChangedMember)
						{	// Что-то поменялось => меняем в БД и MembersIds
							CMemberKeys keys = MembersIds[MemberForChangeInDB.SurnameAndName];
							
							keys.Name = keys.Member.name = ChangedMember.Name;
							keys.Surname = keys.Member.surname = ChangedMember.Surname;
							keys.Member.year_of_birth = ChangedMember.YearOfBirthInShort < 0 ? (short)0 : ChangedMember.YearOfBirthInShort;
							keys.Participation.init_grade = ChangedMember.GradeInEnum == enGrade.None ? null : (byte?)ChangedMember.GradeInEnum;
							if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
							{
								keys.Participation.coach = GlobalDefines.GetCoachId(ChangedMember.SecondCol, true);
								keys.Participation.team = null;
							}
							else
							{
								keys.Participation.team = GlobalDefines.GetTeamId(ChangedMember.SecondCol, true);
								keys.Participation.coach = null;
							}

							// Меняем участника в словаре MembersIds
							MembersIds.Remove(MemberForChangeInDB.SurnameAndName);
							MembersIds.Add(keys.SurnameAndName, keys);

							DBManagerApp.m_Entities.SaveChanges(); /* Сохраняем сделанные изменения, чтобы можно было удалить тренеров и команд,
																	* которые больше не задействованы в соревнованиях */
							
							GlobalDefines.DeleteUnusedCoaches();
							GlobalDefines.DeleteUnusedTeams();

							MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
							{
								ChangingType = enDataChangesTypes.Changing,
								ChangedObjects = enDataChangedObjects.Members | enDataChangedObjects.Results,
								ChangeReason = roundResults.ChangeReason,
								Argument = ChangedRow,
								ID = keys.Participation.id_participation,
								GroupID = m_DBGroup.id_group
							});
						}
					}
					break;
				#endregion

				#region crRowDeleted
				case enChangeReason.crRowDeleted: /* Удаляем участника из results_speed и participation,
												   * а так же из members, если он больше нигде не участвовал.
												   * Так же удаляем записи из coaches и teams, если они нигде не используются.
												   * Это действие можно делать только из первой квалификации. */
					byte DeletedRow;
					if (!IsNewGroup && byte.TryParse(roundResults.Argument, out DeletedRow) && roundResults.RoundInEnum == enRounds.Qualif)
					{
						IEnumerable<members> MemberToDeleteInDB = from results in DBManagerApp.m_Entities.results_speed
																	join part in DBManagerApp.m_Entities.participations on results.participation equals part.id_participation
																	join member in DBManagerApp.m_Entities.members on part.member equals member.id_member
																	where results.number == DeletedRow && results.round == RoundId && part.Group == m_DBGroup.id_group
																	select member;
						members DeletedMemberInDB = MemberToDeleteInDB.First(); // Удалённый участник
						
						CMemberKeys keys = MembersIds[GlobalDefines.CreateSurnameAndName(DeletedMemberInDB.surname, DeletedMemberInDB.name)];

						MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
						{
							ChangingType = enDataChangesTypes.Delete,
							ChangedObjects = enDataChangedObjects.Members | enDataChangedObjects.Results,
							ChangeReason = roundResults.ChangeReason,
							Argument = DeletedRow,
							ID = keys.Participation.id_participation,
							GroupID = m_DBGroup.id_group
						});

						// Если у удаляемого удаляемого участника был результат, то нужно пересчитать места
						bool RedefinePlaces = keys.Participation.results_speed.
													Where(arg => arg.round == (byte)enRounds.Qualif || arg.round == (byte)enRounds.Qualif2).
													Any(arg => arg.sum.HasValue);

						DeleteMember(keys, CompSettings, true, true);

						if (RedefinePlaces)
						{
							IEnumerable<results_speed> MemberResultInDB = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Sum);
							if (!StdDefinePlaces(MemberResultInDB.Where(arg => arg.sum.HasValue)))
								return false;
						}
					}
					break;
				#endregion

				#region crWholeContent
				case enChangeReason.crWholeContent:
					if (!IsNewGroup)
					{
						bool AutoChangeMemberNumbers = false;
						bool ShowMsg = true;

						switch (roundResults.RoundInEnum)
						{
							case enRounds.Qualif:	/* Нужно синхронизировать содержимое таблицы Members для текущей группы, с тем,
													 * что есть в roundResults: удалить тех, которых нет в roundResults,
													 * добавить тех, что нет в БД и изменить тех, у которых информация не совпадает */
								foreach (CMember result in roundResults.Results)
								{
									CMemberKeys keys = null;
									IEnumerable<members> MemberInDB = from member in DBManagerApp.m_Entities.members
																	  join part in DBManagerApp.m_Entities.participations on member.id_member equals part.member into AllMembers
																	  from Part in AllMembers.DefaultIfEmpty()
																	  where member.name == result.Name && member.surname == result.Surname
																	  select member;
									if (MemberInDB.Count() > 0)
									{	// Такой участник есть в БД
										CMemberAndPart MemberFullInfo = new CMemberAndPart()
										{ 
											Member = MemberInDB.First()
										};
										// Ищем участие спортсмена в наших соревах
										MemberFullInfo.Participation = (from part in MemberFullInfo.Member.participations
																	   where part.Group == m_DBGroup.id_group
																	   select part).FirstOrDefault();

										if (MembersIds.TryGetValue(result.SurnameAndName, out keys))
										{
											keys.Member = MemberFullInfo.Member;
											keys.Participation = MemberFullInfo.Participation;
										}
										else
										{	// А в словаре его нет
											keys = new CMemberKeys(result.Name, result.Surname, MemberFullInfo);
											MembersIds.Add(result.SurnameAndName, keys);
										}
										
										if (!keys.Member.OnlyDataFieldsEqual(result))
										{	// И его данные поменялись => копируем новые данные в существующую запись, новую создавать нельзя!!!
											HasChanges = true;
											result.CopyMembersInfoToDB(keys.Member);
										}

										if (keys.Participation == null)
										{	// Участник в БД есть, но он не участвовал в наших соревах
											keys.Participation = result.ToParticipation(keys.Member.id_member,
																						m_DBGroup.id_group,
																						CompSettings.SecondColNameType);

											HasChanges = true;
											DBManagerApp.m_Entities.participations.Add(keys.Participation);
											DBManagerApp.m_Entities.SaveChanges(); // получаем id_participation
										}
										else
										{	// Сравниваем данные в таблице participation
											if (!keys.Participation.OnlyFillFromXMLFieldsEqual(result, CompSettings))
											{	// Что-то поменялось => копируем новые данные в существующую запись, новую создавать нельзя!!!
												HasChanges = true;
												result.CopyPartToDB(keys.Participation, CompSettings.SecondColNameType);
											}
										}

										// сравниваем данные в таблице results_speed
										results_speed ResultsSpeedInDB = (from res_sp in keys.Participation.results_speed
																		  where res_sp.round == RoundId
																		  select res_sp).FirstOrDefault();
										if (result.HasResultsSpeed)
										{
											if (ResultsSpeedInDB == null)
											{	// Результатов не было а теперь они появились
												DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																							RoundId));
												HasChanges = true;
												SumChanged |= result.SumExt != null && result.SumExt.Time != null;
											}
											else
											{
												enChangedResult Changes = ResultsSpeedInDB.UpdateResults(result);
												HasChanges |= Changes != enChangedResult.None;
												SumChanged |= Changes.HasFlag(enChangedResult.SumTime);

												if (!GlobalDefines.IsRoundFinished(m_DBGroup.round_finished_flags, roundResults.RoundInEnum) &&
													!((ResultsSpeedInDB.number == null && result.Number == GlobalDefines.DEFAULT_XML_BYTE_VAL) || ResultsSpeedInDB.number == result.Number))
												{	// Сменился номер, но раунд ещё не завершён
													byte? NewNumber = result.Number == GlobalDefines.DEFAULT_XML_BYTE_VAL ?
																		null :
																		(byte?)result.Number;

													if (ResultsSpeedInDB.round == (byte)enRounds.Qualif || ResultsSpeedInDB.round == (byte)enRounds.Qualif2)
													{
														if (AutoChangeMemberNumbers)
															ResultsSpeedInDB.number = NewNumber;
														else if (ShowMsg)
														{
															CMessageBoxEx msg = new CMessageBoxEx(string.Format(Properties.Resources.resfmtChangeStartNumbersQuestion,
																												result.SurnameAndName,
																												ResultsSpeedInDB.number,
																												NewNumber.HasValue ? NewNumber.ToString() : Properties.Resources.resEmpty),
																								Properties.Resources.resScanning,
																								DBManager.Stuff.CMessageBoxEx.MessageBoxButton.YesNo,
																								System.Windows.MessageBoxImage.Question,
																								DBManager.Stuff.CMessageBoxEx.MessageBoxResult.No,
																								new string[] { Properties.Resources.resYesToAll, Properties.Resources.resNoToAll });
															msg.ShowDialog();
															if (msg.DialogResult.HasValue && msg.DialogResult.Value)
															{
																switch (msg.Result)
																{
																	case CMessageBoxEx.MessageBoxResult.Yes:
																		ResultsSpeedInDB.number = NewNumber;
																		break;

																	case CMessageBoxEx.MessageBoxResult.No:
																		break;

																	case CMessageBoxEx.MessageBoxResult.AdditionalButton:
																		switch (msg.AdditionalButtonNum)
																		{
																			case 0: // Да для всех
																				AutoChangeMemberNumbers = true;
																				ShowMsg = false;
																				ResultsSpeedInDB.number = NewNumber;
																				break;

																			case 1: // Нет для всех
																				AutoChangeMemberNumbers = false;
																				ShowMsg = false;
																				break;
																		}
																		break;
																}
															}
														}
													}
													else
														ResultsSpeedInDB.number = NewNumber;
												}
											}
										}
										else
										{
											if (ResultsSpeedInDB != null)
											{	// Результаты есть, а теперь их не стало
												HasChanges = true;
												SumChanged |= ResultsSpeedInDB.sum != null;
												ResultsSpeedInDB.number = result.Number == GlobalDefines.DEFAULT_XML_BYTE_VAL ? null : (byte?)result.Number;
												ResultsSpeedInDB.ClearResults();
											}
											else
											{	// Участника нет в квалификации => его нужно туда обязательно добавить
												DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																							RoundId));
												HasChanges = true;
											}
										}
									}
									else
									{	// Участника нет => его нужно добавить
										keys = new CMemberKeys(result.Name, result.Surname)
										{
											Member = result,
										};
										DBManagerApp.m_Entities.members.Add(keys.Member);
										DBManagerApp.m_Entities.SaveChanges(); // получаем id_member

										keys.Participation = result.ToParticipation(keys.Member.id_member,
																					m_DBGroup.id_group,
																					CompSettings.SecondColNameType);
										MembersIds.Add(result.SurnameAndName, keys);

										DBManagerApp.m_Entities.participations.Add(keys.Participation);
										DBManagerApp.m_Entities.SaveChanges(); // получаем id_participation

										DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																					RoundId));

										HasChanges = true;
										SumChanged |= result.SumExt != null && result.SumExt.Time != null;
									}
								}
								DBManagerApp.m_Entities.SaveChanges();

								// Удаляем спортсменов, которых больше нет
								foreach (string SurnameAndName in (from curentMember in MembersIds
																  join newMember in roundResults.Results on curentMember.Value.SurnameAndName equals newMember.SurnameAndName into allMembers
																  from member in allMembers.DefaultIfEmpty()
																  where member == null
																   select curentMember.Value.SurnameAndName).ToList())
								{
									CMemberKeys MemberToDelete = MembersIds[SurnameAndName];

									HasChanges = true;
									SumChanged |= MemberToDelete.Participation.results_speed.
														Where(arg => arg.round == (byte)enRounds.Qualif || arg.round == (byte)enRounds.Qualif2).
														Any(arg => arg.sum.HasValue);

									DeleteMember(MemberToDelete, CompSettings, false, false);
								}

								DBManagerApp.m_Entities.SaveChanges(); /* Сохраняем сделанные изменения, чтобы можно было удалить тренеров и команд,
																		* которые больше не задействованы в соревнованиях */

								GlobalDefines.DeleteUnusedCoaches();
								GlobalDefines.DeleteUnusedTeams();

								if (SumChanged)
								{   // Нужно пересчитать места
									IEnumerable<results_speed> MemberResultInDB = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Sum);
									// Удаляем места у всех, у кого нет результатов
									foreach (results_speed result in MemberResultInDB.Where(arg => arg.sum == null))
										result.place = null;

									// Расставляем места
									if (!StdDefinePlaces(MemberResultInDB.Where(arg => arg.sum.HasValue)))
										return false;
								}

								if (HasChanges)
								{
									MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
									{
										ChangingType = enDataChangesTypes.AddManyPcs,
										ChangedObjects = enDataChangedObjects.Members | enDataChangedObjects.Results,
										ChangeReason = roundResults.ChangeReason,
										ID = RoundId,
										GroupID = m_DBGroup.id_group
									});
								}
								break;
						}
					}
					break;
				#endregion

				#region crOnlySomeRowsChanged
				case enChangeReason.crOnlySomeRowsChanged:   // Обновились только результаты у части участников =>
															 // обновляем только их
					int Argument;
					enOnlySomeRowsChangedReason OnlySomeRowsChangedReason = enOnlySomeRowsChangedReason.srcrNone;

					if (!IsNewGroup
						&& int.TryParse(roundResults.Argument, out Argument)
						&& Enum.IsDefined(typeof(enOnlySomeRowsChangedReason), Argument)
						&& (enOnlySomeRowsChangedReason)Argument != enOnlySomeRowsChangedReason.srcrNone
						&& roundResults.ChangedRows != null)
					{   // Если IsNewGroup == true, то мы уже занесли все результаты ранее
						OnlySomeRowsChangedReason = (enOnlySomeRowsChangedReason)Argument;
						switch (OnlySomeRowsChangedReason)
						{
							case enOnlySomeRowsChangedReason.srcrSetStartupPosition:
								switch (roundResults.RoundInEnum)
								{
									case enRounds.Qualif:
									case enRounds.Qualif2:
									case enRounds.OneEighthFinal:
									case enRounds.QuaterFinal:
									case enRounds.SemiFinal:
									case enRounds.Final:
										foreach (CMember result in roundResults.Results)
										{
											CMemberKeys keys;
											if (result.SurnameAndName == null
												|| !MembersIds.TryGetValue(result.SurnameAndName, out keys))
											{   // Неизвестный участник => просто пропускаем его
												continue;
											}

											IEnumerable<results_speed> MemberResultInDB = from results in keys.Participation.results_speed
																						  where results.round == RoundId
																						  select results;
											switch (MemberResultInDB.Count())
											{
												case 0: // У этого участника ещё нет результатов в данном раунде => добавляем их в БД
													DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																								RoundId));
													HasChanges = true;
													break;

												case 1: // Меняем результаты в БД, если они изменились
													enChangedResult Changes = MemberResultInDB.First().UpdateResults(result);
													HasChanges |= Changes != enChangedResult.None;
													break;

												default:    // у участника не может быть больше одного результата в одном раунде
													throw new InvalidOperationException(string.Format("There are more then 1 result for member \"{0}\" for round \"{1}\"",
																										result.SurnameAndName,
																										roundResults.NodeName));
											}
										}
										break;
								}
								break;

							case enOnlySomeRowsChangedReason.srcrGotAutoscanResults:
								foreach (int row in roundResults.ChangedRows)
								{
									CMember result = roundResults.Results.FirstOrDefault(arg => arg.Number == row);
									CMemberKeys keys;
									if (result == null || result.SurnameAndName == null || !MembersIds.TryGetValue(result.SurnameAndName, out keys))
									{   // Неизвестный участник => просто пропускаем его
										continue;
									}

									IEnumerable<results_speed> MemberResultInDB = from results in keys.Participation.results_speed
																				  where results.round == RoundId
																				  select results;
									switch (MemberResultInDB.Count())
									{
										case 0: // У этого участника ещё нет результатов в данном раунде => добавляем их в БД
											DBManagerApp.m_Entities.results_speed.Add(result.ToResults_Speed(keys.Participation.id_participation,
																						RoundId));
											HasChanges = true;
											SumChanged |= result.SumExt != null && result.SumExt.Time != null;
											break;

										case 1: // Меняем результаты в БД, если они изменились
											enChangedResult Changes = MemberResultInDB.First().UpdateResults(result);
											HasChanges |= Changes != enChangedResult.None;
											SumChanged |= Changes.HasFlag(enChangedResult.SumTime);
											break;

										default:    // у участника не может быть больше одного результата в одном раунде
											throw new InvalidOperationException(string.Format("There are more then 1 result for member \"{0}\" for round \"{1}\"",
																								result.SurnameAndName,
																								roundResults.NodeName));
									}
								}

								if (SumChanged)
								{
									switch (roundResults.RoundInEnum)
									{
										case enRounds.Qualif:
										case enRounds.Qualif2:  // Нужно выполнить пересчёт мест
											{
												DBManagerApp.m_Entities.SaveChanges(); // Чтобы сделанные изменения применились

												IEnumerable<results_speed> MemberResultInDB = RoundResults(RoundId, CSpeedResultsComparer.enCompareProperty.Sum);
												// Удаляем места у всех, у кого нет результатов
												foreach (results_speed result in MemberResultInDB.Where(arg => arg.sum == null))
													result.place = null;

												// Расставляем места
												if (!StdDefinePlaces(MemberResultInDB.Where(arg => arg.sum.HasValue)))
													return false;
												break;
											}

										case enRounds.Final: // В Финале нужно расставлять места по мере появления результатов
											DBManagerApp.m_Entities.SaveChanges(); // Чтобы сделанные изменения применились

											DefinePlacesInFinal(RoundResults((int)enRounds.Final, CSpeedResultsComparer.enCompareProperty.Number).ToList());
											break;
									}
								}
								break;
						}

						if (HasChanges)
						{   // Что-то поменялось
							MadeChanges.Add(new CDataChangedInfo(m_ParentScanner)
							{
								ChangingType = enDataChangesTypes.OnlySomeRowsChanged,
								ChangedObjects = enDataChangedObjects.Results,
								ChangeReason = roundResults.ChangeReason,
								Argument = OnlySomeRowsChangedReason,
								ID = RoundId,
								GroupID = m_DBGroup.id_group,
								ListArguments = roundResults.ChangedRows.Cast<object>().ToList()
							});
						}

					}
					break;
				#endregion

				#region crNone, default
				case enChangeReason.crNone:
				default:
					break;
				#endregion
			}
												
			return true;
		}

				
		/// <summary>
		/// Все результаты раунда для текущей группы
		/// </summary>
		/// <param name="RoundId"></param>
		/// <param name="SortByNumber"></param>
		/// <param name="SortBySum"></param>
		/// <param name="SortDir"></param>
		/// <returns></returns>
		private IEnumerable<results_speed> RoundResults(byte RoundId,
														DBManager.Scanning.CSpeedResultsComparer.enCompareProperty SortProperty,
														ListSortDirection SortDir = ListSortDirection.Ascending)
		{
			switch (SortProperty)
			{
				case DBManager.Scanning.CSpeedResultsComparer.enCompareProperty.Number:
					if (SortDir == ListSortDirection.Ascending)
					{
						return from part in DBManagerApp.m_Entities.participations
							   join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
							   where results.round == RoundId && m_DBGroup.id_group == part.Group
							   orderby results.number
							   select results;
					}
					else
					{
						return from part in DBManagerApp.m_Entities.participations
							   join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
							   where results.round == RoundId && m_DBGroup.id_group == part.Group
							   orderby results.number descending
							   select results;
					}

				case DBManager.Scanning.CSpeedResultsComparer.enCompareProperty.Sum:
					if (SortDir == ListSortDirection.Ascending)
					{
						return from part in DBManagerApp.m_Entities.participations
							   join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
							   where results.round == RoundId && m_DBGroup.id_group == part.Group
							   orderby results.sum
							   select results;
					}
					else
					{
						return from part in DBManagerApp.m_Entities.participations
							   join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
							   where results.round == RoundId && m_DBGroup.id_group == part.Group
							   orderby results.sum descending
							   select results;
					}

				default:
					return from part in DBManagerApp.m_Entities.participations
					   join results in DBManagerApp.m_Entities.results_speed on part.id_participation equals results.participation
					   where results.round == RoundId && m_DBGroup.id_group == part.Group
					   select results;
			}
		}


		/// <summary>
		/// Расстановка мест стандартным способом с учётом паровозов
		/// </summary>
		public static bool StdDefinePlaces(IEnumerable<results_speed> SortedResults, int FirstPlace = 1)
		{
			byte CurPlace = (byte)FirstPlace;
			byte ResultIndex = 0;
			TimeSpan? PrevResult = null; // Такого результата не может быть
			foreach (results_speed result in SortedResults)
			{
				ResultIndex++;

				if (!result.sum.HasValue)
					return false;

				if (result.sum != PrevResult)
					CurPlace = ResultIndex;

				result.place = CurPlace;
				PrevResult = result.sum.Value;
			}

			return true;
		}


		/// <summary>
		/// Расстановка мест в финале
		/// </summary>
		private void DefinePlacesInFinal(List<results_speed> lstSortedRoundResults)
		{
			// первые 2 записи - 3-4 место, вторые - 1-2 
			if (lstSortedRoundResults[0].sum == null || lstSortedRoundResults[1].sum == null)
			{	// Если результатов нет, то и мест тоже
				lstSortedRoundResults[0].place = lstSortedRoundResults[1].place = null;
			}
			else
			{
				if (lstSortedRoundResults[0].IsWinnerInPair(lstSortedRoundResults[1]))
				{
					lstSortedRoundResults[0].place = 3;
					lstSortedRoundResults[1].place = 4;
				}
				else
				{
					lstSortedRoundResults[0].place = 4;
					lstSortedRoundResults[1].place = 3;
				}
			}

			if (lstSortedRoundResults[2].sum == null || lstSortedRoundResults[3].sum == null)
			{	// Если результатов нет, то и мест тоже
				lstSortedRoundResults[2].place = lstSortedRoundResults[3].place = null;
			}
			else
			{
				if (lstSortedRoundResults[2].IsWinnerInPair(lstSortedRoundResults[3]))
				{
					lstSortedRoundResults[2].place = 1;
					lstSortedRoundResults[3].place = 2;
				}
				else
				{
					lstSortedRoundResults[2].place = 2;
					lstSortedRoundResults[3].place = 1;
				}
			}
		}


		/// <summary>
		/// Определение победителей в каждой паре, расстановка итоговых мест и перенос участников в следующий раунд
		/// </summary>
		/// <param name="lstRoundResults">список спортсменов, участвоваших в текущем раунде</param>
		/// <param name="NextRound">Раунд, в который нужно перенести спортсменов</param>
		private void SeparateMembersByPairs(List<results_speed> lstRoundResults, enRounds NextRound)
		{
			// Делим участников на победителей и проигравших
			List<results_speed> lstWinners = new List<results_speed>();
			List<results_speed> lstLoosers = new List<results_speed>();
			for (int i = 0; i < lstRoundResults.Count; i += 2)
			{
				if (lstRoundResults[i].IsWinnerInPair(lstRoundResults[i + 1]))
				{
					lstWinners.Add(lstRoundResults[i]);
					lstLoosers.Add(lstRoundResults[i + 1]);
				}
				else
				{
					lstLoosers.Add(lstRoundResults[i]);
					lstWinners.Add(lstRoundResults[i + 1]);
				}
			}
			
			// Сортируем их по возрастанию результатов
			m_SpeedResultsComparer.CompareProperty = CSpeedResultsComparer.enCompareProperty.Sum;
			m_SpeedResultsComparer.SortDir = ListSortDirection.Ascending;
			lstWinners.Sort(m_SpeedResultsComparer);
			lstLoosers.Sort(m_SpeedResultsComparer);
			
			// Расставляем места.
			// TO DO: добавить учёт одинаковых мест
			for (int i = 0; i < lstWinners.Count; i++)
			{
				lstWinners[i].place = (byte)(i + 1);
				lstLoosers[i].place = (byte)(lstWinners.Count + i + 1);
			}
			
			// Расставляем итоговые места для проигравших. Они равны местам, занятым ими в текущем раунде
			foreach (results_speed Looser in lstLoosers)
				Looser.participations.result_place = Looser.place;
			
			// Переносим участников в следующий раунд
			byte[] RowsNumbers = GlobalDefines.ROW_SEQUENCE[NextRound];
			for (int i = 0; i < RowsNumbers.Length; i++)
			{
				ReplaceOrAddSpeedResult(new results_speed()
				{
					participation = lstWinners[RowsNumbers[i] - 1].participation,
					round = (byte)NextRound,
					number = (byte)(i + 1),
				});
			}
		}


		private void ReplaceOrAddSpeedResult(results_speed NewResult)
		{
			results_speed resultInDB = DBManagerApp.m_Entities.results_speed.FirstOrDefault(arg => arg.participation == NewResult.participation && arg.round == NewResult.round);
			if (resultInDB != null)
			{	// Результат для этого участника уже есть => заменяем его
				resultInDB.number = NewResult.number;
				resultInDB.place = null;
				resultInDB.route1 = resultInDB.route2 = resultInDB.sum = null;
			}
			else
				DBManagerApp.m_Entities.results_speed.Add(NewResult);
		}


		/// <summary>
		/// Удаление спортсмена из БД и словаря MembersIds.
		/// </summary>
		/// <param name="MemberToDeleteKeys">
		/// удаляемый участник
		/// </param>
		/// <param name="CompSettings"></param>
		/// <param name="SaveChangesAfterDelete">
		/// Нужно ли выполнить удаление из БД (<value>true</value>) или только пометить запись, как удалённую
		/// </param>
		/// <param name="DeleteUnusedCoachesOrTeams">
		/// Нужно ли после удаления спортсмена удалить неиспользуемые команды или тренеров?
		/// Для удаления параметр <paramref name="SaveChangesAfterDelete"/> должен быть равен <value>true</value> и
		/// <paramref name="CompSettings"/> не должен быть равен <value>null</value>.
		/// </param>
		private void DeleteMember(CMemberKeys MemberToDeleteKeys,
									CCompSettings CompSettings,
									bool SaveChangesAfterDelete,
									bool DeleteUnusedCoachesOrTeams)
		{
			participations MemberPartInCurGroup = (from part in DBManagerApp.m_Entities.participations
												   where part.member == MemberToDeleteKeys.Member.id_member && part.Group == m_DBGroup.id_group
												   select part).FirstOrDefault();
			IEnumerable<participations> MemberPartsInOtherGroups = (from part in DBManagerApp.m_Entities.participations
																	where part.member == MemberToDeleteKeys.Member.id_member && part.Group != m_DBGroup.id_group
																	select part);
			if (MemberPartInCurGroup != null)
			{	// Уменьшаем номера всех идущих после него спортсменов
				// Просматриваем все раунды, в которых участвовал удаляемый спортсмен в рамках одних соревнований
				foreach (byte round in (from result in MemberPartInCurGroup.results_speed select result.round).ToList())
				{
					// Номер спортсмена в соревновании
					byte? DelMemberNumInRound = (from result in MemberPartInCurGroup.results_speed
													where result.round == round && result.number != null && result.number > 0
													select result.number).FirstOrDefault();
					if (DelMemberNumInRound != null)
					{
						// Просматриваем результаты всех остальных спортсменов, принимавших участие в раунде
						foreach (results_speed res_sp in (from result in DBManagerApp.m_Entities.results_speed
														  join part in DBManagerApp.m_Entities.participations on result.participation equals part.id_participation
														  where part.Group == m_DBGroup.id_group && result.round == round && result.number > DelMemberNumInRound.Value
														  select result).ToList())
						{
							res_sp.number--;
						}
					}
				}
			}

			if (MemberPartsInOtherGroups.Count() > 0)
			{	// Спортсмен принимал участие ещё в каких-то соревнованиях
				// Удалять спортсмена нужно именно с помощью запроса, т.к. через DeleteObject может произойти ошибка
				DBManagerApp.m_Entities.Database.ExecuteSqlCommand("DELETE FROM `participations` WHERE `id_participation`='" +
															MemberToDeleteKeys.Participation.id_participation.ToString() +
															"';");
			}
			else
			{	// Спортсмен принимал участие только в текущих соревнованиях => просто удаляем его из БД
				// Удалять спортсмена нужно именно с помощью запроса, т.к. через DeleteObject может произойти ошибка
				DBManagerApp.m_Entities.Database.ExecuteSqlCommand("DELETE FROM `members` WHERE `id_member`='" +
															MemberToDeleteKeys.Member.id_member.ToString() +
															"';");
			}
			
			MembersIds.Remove(MemberToDeleteKeys.SurnameAndName);

			if (SaveChangesAfterDelete)
			{
				DBManagerApp.m_Entities.SaveChanges(); /* Сохраняем сделанные изменения, чтобы можно было удалить тренеров и команд,
														* которые больше не задействованы в соревнованиях */

				if (DeleteUnusedCoachesOrTeams && CompSettings != null)
				{
					GlobalDefines.DeleteUnusedCoaches();
					GlobalDefines.DeleteUnusedTeams();
				}
			}
		}
	}
}
