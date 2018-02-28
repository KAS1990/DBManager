using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media;
using DBManager.Scanning.XMLDataClasses;
using System.Windows.Data;
using DBManager.Global;
using DBManager.Global.Converters;
using System.Windows;
using DBManager.SettingsWriter;

namespace DBManager.RoundMembers.Converters
{
	/// <summary>
	/// Конвертор, который подсвечивает участников разными цветами: стоит на старте, готовится, только что пробежал трассу и т.д.
	/// </summary>
	public class HighlightMemberBrushMarkupConverter : MarkupMultiConverterBase
	{
		bool m_IsBackgroudColor = true;
		public bool IsBackgroudColor
		{
			get { return m_IsBackgroudColor; }
			set { m_IsBackgroudColor = value; }
		}


		object ConvResultVal(CFontStyleSettings FontStyleSettings, bool UseTransparentBackcolor)
		{
			if (IsBackgroudColor)
				return UseTransparentBackcolor ? Brushes.Transparent : new SolidColorBrush(FontStyleSettings.BackgroundColor);
			else
				return new SolidColorBrush(FontStyleSettings.ForeColor);
		}
				

		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			enCellType DestColumnType = parameter == null ? enCellType.None : (enCellType)parameter;

			CResult result = values[0] as CResult;
			if (values.Length == 4)
			{
				if (result == null || !(values[1] is byte) || !(values[2] is int) || !(values[3] is int))
				{
					if (result != null && result.CondFormating.HasValue && (values[1] is byte) && values[3] is int)
					{	// Возможно участник стоит на старте
						lock (DBManagerApp.m_AppSettings.m_SettigsSyncObj)
						{
							if (result.CondFormating.Value == enCondFormating.StayOnStart &&
								DestColumnType != enCellType.StartNumber &&
								DestColumnType != enCellType.SurnameAndName)
							{
								return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false);
							}
						}
					}
					else
						return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
				}
			}
			else if (values.Length == 2)
			{
				if (result == null || !(values[1] is byte))
					return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
			}
			else
				return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
						
			enRounds Round = (enRounds)((byte)values[1]);
			
			if (!(result == null || result.CondFormating == null))
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
										return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.InvitedToStartFontStyle, false);
									break;

								case enCellType.None:
									return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.StayOnStartFontStyle, false);

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
								return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.JustRecievedResultFontStyle, false);
						
						case enCondFormating.Preparing: // Участник готовится
							if (Round == enRounds.Qualif || Round == enRounds.Qualif2)
							{
								switch (DestColumnType)
								{
									case enCellType.StartNumber:
									case enCellType.SurnameAndName:
										return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PreparingFontStyle, false);

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

			if (values.Length == 4)
			{
				int RoundPlace = values[2] is int ? (int)values[2] : 0;
				int MembersFromQualif = values[3] is int ? (int)values[3] : 0;
				if (RoundPlace > 0 && RoundPlace <= MembersFromQualif)
				{	// Участник проходит в следуюущий тур
					return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.NextRoundMembersCountFontStyle, false);
				}
				else
					return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
			}
			else
				return ConvResultVal(DBManagerApp.m_AppSettings.m_Settings.PlainResultsFontStyle, true);
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new Exception("HighlightMemberBrushMarkupConverter.ConvertBack is not implemented");
		}


		public HighlightMemberBrushMarkupConverter() :
			base()
		{
		}
	}
}
