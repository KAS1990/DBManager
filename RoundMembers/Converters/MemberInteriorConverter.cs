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

namespace DBManager.RoundMembers.Converters
{
	/// <summary>
	/// Типо конвертор, который задаёт стиль шрифта и цвет ячейки в зависимости от состояния участника: стоит на старте, готовится, только что пробежал трассу и т.д.
	/// </summary>
	public static class MemberInteriorConverter
	{
		public class CConverterResult
		{
			public FontWeight FontWeight;
			public FontStyle FontStyle;
			public Brush Background;
			public Brush Foreground;

			public CConverterResult(CFontStyleSettings fontStyle, bool UseTransparentBackcolor)
			{
				FontWeight = fontStyle.FontWeight;
				FontStyle = fontStyle.FontStyle;
				Background = UseTransparentBackcolor ? Brushes.Transparent : new SolidColorBrush(fontStyle.BackgroundColor);
				Foreground = new SolidColorBrush(fontStyle.ForeColor);
			}
		}

		public static CConverterResult Convert(CResult result, enRounds? Round, int? RoundPlace, int? MembersFromQualif, enCellType DestColumnType)
		{
			if ((RoundPlace != null) || (MembersFromQualif != null))
			{
				if ((result == null) || (Round == null) || (RoundPlace == null) || (MembersFromQualif == null))
				{
					if (result != null && result.CondFormating.HasValue && Round.HasValue && MembersFromQualif.HasValue)
					{	// Возможно участник стоит на старте
						lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
						{
							if (result.CondFormating.Value == enCondFormating.StayOnStart &&
								DestColumnType != enCellType.StartNumber &&
								DestColumnType != enCellType.SurnameAndName)
							{
								return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false);
							}
						}
					}
					else
						return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
				}
			}
			else if ((RoundPlace == null) && (MembersFromQualif == null))
			{
				if ((result == null) || (Round == null))
					return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
			}
			else
				return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);


			if (!(result == null || result.CondFormating == null || DestColumnType == enCellType.None))
			{
				lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
				{
					switch (result.CondFormating.Value)
					{
						case enCondFormating.StayOnStart: // Находится на старте
							switch (DestColumnType)
							{
								case enCellType.StartNumber:
								case enCellType.SurnameAndName:
									if (Round == enRounds.Qualif || Round == enRounds.Qualif2)
										return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle, false);
									break;

								case enCellType.Route1:
								case enCellType.Route2:
								case enCellType.Sum:
									return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false);

								default:
									break;
							}
							break;

						case enCondFormating.JustRecievedResult: // Только что полученный результат
							if (Round == enRounds.Qualif ||
								Round == enRounds.Qualif2 ||
								(Round > enRounds.Qualif2 && Round <= enRounds.Final && result.ResultColumnNumber == enResultColumnNumber.Sum))
							{	// Тут подсветка не нужна
								break;
							}
							else
								return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle, false);

						case enCondFormating.Preparing: // Участник готовится
							if (Round == enRounds.Qualif || Round == enRounds.Qualif2)
							{
								switch (DestColumnType)
								{
									case enCellType.StartNumber:
									case enCellType.SurnameAndName:
										return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle, false);

									default:
										break;
								}
							}
							break;

						default:
							break;
					}
				}
			}

			if (RoundPlace.HasValue && MembersFromQualif.HasValue)
			{
				if (RoundPlace > 0 && RoundPlace <= MembersFromQualif)
				{	// Участник проходит в следуюущий тур
					return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle, false);
				}
				else
					return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
			}
			else
				return new CConverterResult(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle,true);
		}
	}
}
