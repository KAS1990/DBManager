using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBManager.Global.Converters;
using System.Globalization;
using DBManager.Scanning.XMLDataClasses;
using System.Windows.Media;
using DBManager.Global;
using DBManager.SettingsWriter;
using System.Windows;
using DBManager.Scanning.DBAdditionalDataClasses;

namespace DBManager.RoundMembers.Converters
{
	/// <summary>
	/// Типо конвертор, который задаёт стиль шрифта и цвет ячейки в зависимости от состояния участника: стоит на старте, готовится, только что пробежал трассу и т.д.
	/// </summary>
	public static class MemberInteriorConverter
	{
		public class CConverterResult
		{
			private bool isEmpty = true;
			public bool IsEmpty
			{
				get { return isEmpty; }
			}

			public FontWeight FontWeight = FontWeights.Normal;
			public FontStyle FontStyle = FontStyles.Normal;
			public Brush Background = Brushes.Transparent;
			public Brush Foreground = Brushes.Black;

			public CConverterResult()
			{
			}


			public CConverterResult(CFontStyleSettings fontStyle, bool UseTransparentBackcolor)
			{
				isEmpty = false;
				FontWeight = fontStyle.FontWeight;
				FontStyle = fontStyle.FontStyle;
				Background = UseTransparentBackcolor ? Brushes.Transparent : new SolidColorBrush(fontStyle.BackgroundColor);
				Foreground = new SolidColorBrush(fontStyle.ForeColor);
			}


			public CConverterResult MixWithOther(CConverterResult rhs, bool IsRhsHasMorePriority)
			{
				if (IsEmpty)
				{
					FontWeight = rhs.FontWeight;
					FontStyle = rhs.FontStyle;
					Background = rhs.Background;
					Foreground = rhs.Foreground;
				}
				else if (!rhs.IsEmpty)
				{
					FontWeight = IsRhsHasMorePriority ? rhs.FontWeight : FontWeight;
					FontStyle = IsRhsHasMorePriority ? rhs.FontStyle : FontStyle;

					if (Background != rhs.Background)
					{
						if (Background.ToString() == Brushes.Transparent.ToString() || IsRhsHasMorePriority)
							Background = rhs.Background;
					}

					if (Foreground != rhs.Foreground)
					{
						if (IsRhsHasMorePriority)
							Foreground = rhs.Foreground;
					}
				}

				return this;
			}
		}

		public static CConverterResult Convert(CMemberAndResults Member,
												CResult result,
												enRounds? Round,
												int? MembersFromQualif,
												enCellType DestColumnType,
												out bool PlainStyleSetted)
		{
			PlainStyleSetted = false;

			CConverterResult res = new CConverterResult();

			lock (DBManagerApp.m_AppSettings.m_SettingsSyncObj)
			{
				if ((Member.HasFalsestart && DestColumnType == enCellType.SurnameAndName)
					|| (result.AdditionalEventTypes.HasValue && result.AdditionalEventTypes.Value.HasFlag(enAdditionalEventTypes.Falsestart)
							&& (DestColumnType == enCellType.Route1 || DestColumnType == enCellType.Route2)))
				{
					res = new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.FalsestartFontStyle, false);
				}

				if (Round.HasValue)
				{
					switch (Round)
					{
						#region Qualif, Qualif2
						case enRounds.Qualif:
						case enRounds.Qualif2:
							switch (DestColumnType)
							{
								#region StartNumber, SurnameAndName
								case enCellType.StartNumber:
								case enCellType.SurnameAndName:
									if (result.CondFormating.HasValue)
									{
										switch (result.CondFormating.Value)
										{
											case enCondFormating.StayOnStart: // Находится на старте
												return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle, false),
																		 false);

											case enCondFormating.JustRecievedResult: // Только что полученный результат
												if (result.ResultColumnNumber == enResultColumnNumber.Sum)
												{
													return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle, false),
																			 false);
												}
												else
													break;

											case enCondFormating.Preparing: // Участник готовится
												return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle, false),
																		  false);

											default:
												break;
										}
									}
									break;
								#endregion

								#region Route1, Route2, Sum
								case enCellType.Route1:
								case enCellType.Route2:
								case enCellType.Sum:
									if (result.CondFormating.HasValue)
									{
										switch (result.CondFormating.Value)
										{
											case enCondFormating.StayOnStart: // Находится на старте
												return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false),
																		  false);

											case enCondFormating.JustRecievedResult: // Только что полученный результат
												return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle, false),
																		  false);
												
											case enCondFormating.Preparing: // Участник готовится
												break;

											default:
												break;
										}
									}
									break;
								#endregion

								default:
									break;
							}

							if (Member.Place.HasValue && Member.Place > 0 && Member.Place <= MembersFromQualif)
							{   // Участник проходит в следуюущий тур
								return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle, false),
														false);
							}
							break;
						#endregion

						#region OneEighthFinal, QuaterFinal, SemiFinal, Final
						case enRounds.OneEighthFinal:
						case enRounds.QuaterFinal:
						case enRounds.SemiFinal:
						case enRounds.Final:
							switch (DestColumnType)
							{
								#region StartNumber, SurnameAndName
								case enCellType.StartNumber:
								case enCellType.SurnameAndName:
									break;
								#endregion

								#region Route1, Route2, Sum
								case enCellType.Route1:
								case enCellType.Route2:
								case enCellType.Sum:
									if (result.CondFormating.HasValue)
									{
										switch (result.CondFormating.Value)
										{
											case enCondFormating.StayOnStart: // Находится на старте
												return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false),
																		 false);

											case enCondFormating.JustRecievedResult: // Только что полученный результат
											case enCondFormating.Preparing: // Участник готовится
												break;

											default:
												break;
										}
									}
									break;
								#endregion

								default:
									break;
							}
							break;
						#endregion
					}
				}
			}

			if (res.IsEmpty)
				PlainStyleSetted = true;
			return res.MixWithOther(new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true),
									false);
		}
	}
}
