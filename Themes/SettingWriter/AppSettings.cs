﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Schema;
using System.Xml;
using System.ComponentModel;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.SettingsWriter
{
	public enum enResultGradeCalcMethod
	{
		/// <summary>
		/// Это место и выше (округление "вниз")
		/// </summary>
		Floor,
		/// <summary>
		/// Это место и выше ("математическое" округление до целых)
		/// </summary>
		Round
	}

	[Serializable]
	public class CFileScannerSettings
	{
		/// <summary>
		/// Полный путь к файлу
		/// </summary>
		[XmlAttribute]
		public string FullFilePath { get; set; }
		
		/// <summary>
		/// Идентификатор группы
		/// </summary>
		[XmlAttribute]
		public Int64 GroupId { get; set; }
	}


	[Serializable]
	public class CFontStyleSettings : IXmlSerializable
	{
		/// <summary>
		/// 
		/// </summary>
		[XmlElement]
		public FontWeight FontWeight { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement]
		public FontStyle FontStyle { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement]
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[XmlElement]
		public Color ForeColor { get; set; }

		public CFontStyleSettings()
		{
			FontWeight = FontWeights.Normal;
			FontStyle = FontStyles.Normal;
			ForeColor = Colors.Black;
			BackgroundColor = Colors.Transparent;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			bool isEmpty = reader.IsEmptyElement;

			if (isEmpty)
				return;

			XmlSerializer ColorSerializer = new XmlSerializer(typeof(Color));

			if (reader.MoveToAttribute("FontWeight"))
				FontWeight = FontWeight.FromOpenTypeWeight(int.Parse(reader.Value));
			else
				FontWeight = FontWeights.Normal;

			if (reader.MoveToAttribute("FontStyle"))
			{
				if (reader.Value == FontStyles.Italic.ToString())
					FontStyle = FontStyles.Italic;
				else if (reader.Value == FontStyles.Oblique.ToString())
					FontStyle = FontStyles.Oblique;
				else
					FontStyle = FontStyles.Normal;
			}
			else
				FontStyle = FontStyles.Normal;

			reader.ReadStartElement();

			reader.ReadStartElement("BackgroundColor");
			BackgroundColor = (Color)ColorSerializer.Deserialize(reader);
			reader.ReadEndElement();

			reader.ReadStartElement("ForeColor");
			ForeColor = (Color)ColorSerializer.Deserialize(reader);
			reader.ReadEndElement();

			reader.ReadEndElement();
		}

		public void WriteXml(XmlWriter writer)
		{
			XmlSerializer ColorSerializer = new XmlSerializer(typeof(Color));

			writer.WriteAttributeString("FontWeight", FontWeight.ToOpenTypeWeight().ToString());
			writer.WriteAttributeString("FontStyle", FontStyle.ToString());

			writer.WriteStartElement("BackgroundColor");
			ColorSerializer.Serialize(writer, BackgroundColor);
			writer.WriteEndElement();

			writer.WriteStartElement("ForeColor");
			ColorSerializer.Serialize(writer, ForeColor);
			writer.WriteEndElement();
		}
	}


	[Serializable]
	public class CTeamForTeamReport
	{
		/// <summary>
		/// Название команды
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string Name = GlobalDefines.DEFAULT_XML_STRING_VAL;

		/// <summary>
		/// Идентификаторы команд, которые объединены в группу
		/// </summary>
		[DefaultValue(null)]
		public List<long> SubteamsIds = null;
	}


	[Serializable]
	public class CLeadSheetInfo
	{
		/// <summary>
		/// Индекс листа, начиная с 0
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int SheetIndex = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Дата начала соревнований
		/// </summary>
		[DefaultValue(null)]
		public CCompDate StartDate = null;

		/// <summary>
		/// Дата окончания соревнований
		/// </summary>
		[DefaultValue(null)]
		public CCompDate EndDate = null;
	}


	[Serializable]
	public class CFTPGroupItemInSets
	{
		/// <summary>
		/// Идентификтор группы
		/// </summary>
		[DefaultValue(-1)]
		public long GroupId = -1;

		[DefaultValue(false)]
		public bool IsSelected = false;

		/// <summary>
		/// Путь к книге на FTP
		/// </summary>
		[DefaultValue(null)]
		public string FTPWbkPath = null;


		public bool CheckFTPWbkFullPath()
		{
			return !string.IsNullOrWhiteSpace(FTPWbkPath) && System.IO.Path.GetExtension(FTPWbkPath) == GlobalDefines.XLS_EXTENSION;
		}
	}


	[Serializable]
	public class CCompSpecificSets
	{
		/// <summary>
		/// Идентификатор соревнования в БД
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public long CompId = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Число мужчин в группе для командного зачёта
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int MenInGroup = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Число женщин в группе для командного зачёта
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int WomenInGroup = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Группы, которые используются для подсчёта командного зачёта
		/// </summary>
		[XmlArray("TeamsForTeamReport")]
		[DefaultValue(null)]
		public List<CTeamForTeamReport> lstTeamsForTeamReport = null;

		/// <summary>
		/// Путь к файлу с протоколом трудности
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string LeadReportXlsPath = GlobalDefines.DEFAULT_XML_STRING_VAL;

		/// <summary>
		/// Номер строки, с которой начинается список участников
		/// Начинается с 1!!!
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int FirstMemberRow = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Номер столбца, в котором находится место участника
		/// Начинается с 1!!!
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int PlaceColumnIndex = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Номер столбца, в котором содержится ФИ участника.
		/// Начинается с 1!!!
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int PersonalDataColumnIndex = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Номер столбца, в котором содержится г.р. участника.
		/// Начинается с 1!!!
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int YearOfBirthColumnIndex = GlobalDefines.DEFAULT_XML_INT_VAL;

		/// <summary>
		/// Номер столбца, в котором содержится команда участника.
		/// Начинается с 1!!!
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_INT_VAL)]
		public int TeamColumnIndex = GlobalDefines.DEFAULT_XML_INT_VAL;
					
		/// <summary>
		/// Номера листов в протоколе трудности для каждой группы
		/// Ключ - идентификатор группы
		/// Значение - информация о листе
		/// </summary>
		[XmlElement("GroupsLeadSheetsInfos")]
		[DefaultValue(null)]
		public SerializableDictionary<long, CLeadSheetInfo> dictGroupsLeadSheetsInfos = null;

		[DefaultValue(enPersRepPlacesAggregationMethod.Sum)]
		public enPersRepPlacesAggregationMethod PersRepPlaceAggregationMethod = enPersRepPlacesAggregationMethod.Sum;

		[DefaultValue(enPersRepWinnerDetection.LeadPriority)]
		public enPersRepWinnerDetection PersRepWinnerDetection = enPersRepWinnerDetection.LeadPriority;

		/// <summary>
		/// Настройки групп, связанные с посылкой данных на FTP-сервер
		/// </summary>
		[XmlElement("GroupsForAutosendToFTP")]
		[DefaultValue(null)]
		public SerializableDictionary<long, CFTPGroupItemInSets> dictGroupsForAutosendToFTP = null;


		public void ToDefault()
		{
			CompId =
				WomenInGroup =
				MenInGroup =
				FirstMemberRow =
				PlaceColumnIndex =
				PersonalDataColumnIndex =
				YearOfBirthColumnIndex =
				TeamColumnIndex = GlobalDefines.DEFAULT_XML_INT_VAL;
			LeadReportXlsPath = GlobalDefines.DEFAULT_XML_STRING_VAL;
			lstTeamsForTeamReport = new List<CTeamForTeamReport>();
			dictGroupsLeadSheetsInfos = new SerializableDictionary<long, CLeadSheetInfo>();

			PersRepPlaceAggregationMethod = enPersRepPlacesAggregationMethod.Sum;
			PersRepWinnerDetection = enPersRepWinnerDetection.LeadPriority;

			dictGroupsForAutosendToFTP = new SerializableDictionary<long, CFTPGroupItemInSets>();
		}


		/// <summary>
		/// Проверяем значение полей структуры и приводим их в значение по умолчнию, если они не инициализированы
		/// </summary>
		public void CheckAndToDefault()
		{
			if (CompId == GlobalDefines.DEFAULT_XML_INT_VAL)
			{
				lstTeamsForTeamReport = new List<CTeamForTeamReport>();
				dictGroupsLeadSheetsInfos = new SerializableDictionary<long, CLeadSheetInfo>();
				dictGroupsForAutosendToFTP = new SerializableDictionary<long, CFTPGroupItemInSets>();
			}
			else
			{
				if (lstTeamsForTeamReport == null)
					lstTeamsForTeamReport = new List<CTeamForTeamReport>();
				if (dictGroupsLeadSheetsInfos == null)
					dictGroupsLeadSheetsInfos = new SerializableDictionary<long, CLeadSheetInfo>();
				if (dictGroupsForAutosendToFTP == null)
					dictGroupsForAutosendToFTP = new SerializableDictionary<long, CFTPGroupItemInSets>();
			}

			if (PlaceColumnIndex < 1)
				PlaceColumnIndex = 1;
			if (PersonalDataColumnIndex < 1)
				PersonalDataColumnIndex = 2;
			if (YearOfBirthColumnIndex < 4)
				YearOfBirthColumnIndex = 4;
			if (TeamColumnIndex < 3)
				TeamColumnIndex = 3;
		}
	}


	[Serializable]
	public class CExcelSettings
	{
		/// <summary>
		/// Максимальная длина названия листа в книге Excel 
		/// </summary>
		[DefaultValue(0)]
		public int MaxSheetNameLen = 31;
		
		/// <summary>
		/// Название файла с шаблонами отчётов 
		/// </summary>
		[DefaultValue(null)]
		public string ReportTemplatesWbkName = "ReportTemplates.xlsx";

		/// <summary>
		/// Номера листов, соотвестующие каждому типу отчёта
		/// Ключ - тип отчёта.
		/// Значение - номер листа в ReportTemplatesWbkName. Начинается с 1!!!
		/// </summary>
		[XmlElement("ReportTemplates")]
		[DefaultValue(null)]
		public SerializableDictionary<enReportTypes, int> dictReportTemplates = null;
		
		/// <summary>
		/// Название файла с шаблонами листов для выгрузки на FTP 
		/// </summary>
		[DefaultValue(null)]
		public string FTPTemplatesWbkName = "FTPTemplates.xls";


		/// <summary>
		/// Номера листов, соотвестующие каждому типу листа при выгрузке на FTP
		/// Ключ - тип отчёта.
		/// Значение - номер листа в ReportTemplatesWbkName. Начинается с 1!!!
		/// </summary>
		[XmlElement("FTPSheetTemplates")]
		[DefaultValue(null)]
		public SerializableDictionary<enFTPSheetGeneratorTypes, int> dictFTPSheetTemplates = null;
		

		public void ToDefault()
		{
			MaxSheetNameLen = 31;

			ReportTemplatesWbkName = "ReportTemplates.xlsx";

			dictReportTemplates = new SerializableDictionary<enReportTypes, int>();
			dictReportTemplates.Add(enReportTypes.Qualif, 1);
			dictReportTemplates.Add(enReportTypes.Qualif2, 1);
			dictReportTemplates.Add(enReportTypes.OneEighthFinal, 2);
			dictReportTemplates.Add(enReportTypes.QuaterFinal, 3);
			dictReportTemplates.Add(enReportTypes.SemiFinal, 4);
			dictReportTemplates.Add(enReportTypes.Final, 5);
			dictReportTemplates.Add(enReportTypes.Total, 6);
			dictReportTemplates.Add(enReportTypes.Team, 7);
			dictReportTemplates.Add(enReportTypes.Personal, 8);
			dictReportTemplates.Add(enReportTypes.StartList, 9);
			

			FTPTemplatesWbkName = "FTPTemplates.xls";

			dictFTPSheetTemplates = new SerializableDictionary<enFTPSheetGeneratorTypes, int>();
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Start, 1);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Qualif, 1);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Qualif2, 1);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.OneEighthFinal, 2);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.QuaterFinal, 3);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.SemiFinal, 4);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Final, 5);
			dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Total, 6);
		}


		public void CheckAndToDefault()
		{
			if (string.IsNullOrWhiteSpace(ReportTemplatesWbkName) ||
				System.IO.Path.GetExtension(ReportTemplatesWbkName) != GlobalDefines.XLSX_EXTENSION)
			{
				ReportTemplatesWbkName = "ReportTemplates.xls";
			}

			if (dictReportTemplates == null || dictReportTemplates.Count < (int)enReportTypes.Personal)
			{
				dictReportTemplates = new SerializableDictionary<enReportTypes, int>();
				dictReportTemplates.Add(enReportTypes.Qualif, 1);
				dictReportTemplates.Add(enReportTypes.Qualif2, 1);
				dictReportTemplates.Add(enReportTypes.OneEighthFinal, 2);
				dictReportTemplates.Add(enReportTypes.QuaterFinal, 3);
				dictReportTemplates.Add(enReportTypes.SemiFinal, 4);
				dictReportTemplates.Add(enReportTypes.Final, 5);
				dictReportTemplates.Add(enReportTypes.Total, 6);
				dictReportTemplates.Add(enReportTypes.Team, 7);
				dictReportTemplates.Add(enReportTypes.Personal, 8);
				dictReportTemplates.Add(enReportTypes.StartList, 9);
			}


			if (string.IsNullOrWhiteSpace(FTPTemplatesWbkName) ||
				System.IO.Path.GetExtension(FTPTemplatesWbkName) != GlobalDefines.XLSX_EXTENSION)
			{
				FTPTemplatesWbkName = "FTPTemplates.xls";
			}

			if (dictFTPSheetTemplates == null || dictFTPSheetTemplates.Count < (int)enFTPSheetGeneratorTypes.Start)
			{
				dictFTPSheetTemplates = new SerializableDictionary<enFTPSheetGeneratorTypes, int>();
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Start, 1);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Qualif, 1);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Qualif2, 1);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.OneEighthFinal, 2);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.QuaterFinal, 3);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.SemiFinal, 4);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Final, 5);
				dictFTPSheetTemplates.Add(enFTPSheetGeneratorTypes.Total, 6);
			}
		}
	}

	/// <summary>
	/// Класс, содержащий все настройки, которые есть в программе
	/// </summary>
	/// 
	[XmlRoot("Settings", IsNullable = false)]
	public class AppSettings
	{
		/* Список всех настроек приложения */
		/// <summary>
		/// Запустили программу после автоматической перезагрузки.
		/// </summary>
		public bool IsRestarting = false;
		
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(null)]
		public string CompDir = null;

		/// <summary>
		/// false - при удалении xml-файла данные соотвествующей группы не удаляются из БД
		/// </summary>
		[DefaultValue(true)]
		public bool HandleFileDeletion = true;

		/// <summary>
		/// false - при старте ПО не выполняется обработка xls-файла,
		/// просто происходит вычитывание данных из БД.
		/// При этом выбирается то соревнование, которое указано в <see cref="CompDir"/>
		/// </summary>
		[DefaultValue(true)]
		public bool AutodetectOnStart = true;

		[DefaultValue(null)]
		public CCompSpecificSets DefaultCompSettings = null;
				
		/// <summary>
		/// Метод, с помощью которого вычисляются места для присвоения разрядов
		/// </summary>
		[DefaultValue(enResultGradeCalcMethod.Floor)]
		public enResultGradeCalcMethod ResultGradeCalcMethod = enResultGradeCalcMethod.Floor;

		/// <summary>
		/// true - при вычислении разрядов учитывается только 75% участников
		/// </summary>
		[DefaultValue(false)]
		public bool Only75PercentForCalcGrades = false;

		/// <summary>
		/// Возраст, с которого можно присваивать разряды 
		/// </summary>
		[DefaultValue(10)]
		public int MinAgeToCalcResultGrade = 10;

		/// <summary>
		/// Обычный текст
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings PlainResultsFontStyle = null;

		/// <summary>
		/// В следующий раунд выходят
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings NextRoundMembersCountFontStyle = null;

		/// <summary>
		/// Участник приглашается на старт
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings InvitedToStartFontStyle = null;

		/// <summary>
		/// Участник готовится
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings PreparingFontStyle = null;

		/// <summary>
		/// Участник находится на старте
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings StayOnStartFontStyle = null;

		/// <summary>
		/// Только что полученный результат
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings JustRecievedResultFontStyle = null;

		/// <summary>
		/// Участник сделал фальстарт
		/// </summary>
		[XmlElement]
		[DefaultValue(null)]
		public CFontStyleSettings FalsestartFontStyle = null;

		/// <summary>
		/// Шрифт
		/// </summary>
		[XmlElement]
		[DefaultValue("Arial")]
		public string FontFamilyName = "Arial";

		/// <summary>
		/// Размер шрифта
		/// </summary>
		[XmlElement]
		[DefaultValue(14.0)]
		public double FontSize = 14.0;

		/// <summary>
		/// Файлы, которые сканирует программа.
		/// Ключ - имя файла с расширением
		/// </summary>
		[XmlElement("FileScannerSettings")]
		[DefaultValue(null)]
		public SerializableDictionary<string, CFileScannerSettings> dictFileScannerSettings = null;

		/// <summary>
		/// Настройки всех соревнований
		/// Ключ - идентификатор соревнования
		/// </summary>
		[XmlElement("CompSettings")]
		[DefaultValue(null)]
		public SerializableDictionary<long, CCompSpecificSets> dictCompSettings = null;
		
		/// <summary>
		/// Настройки для экспорта в Excel
		/// </summary>
		[DefaultValue(null)]
		public CExcelSettings ExcelSettings = null;

		#region FTP
		/// <summary>
		/// FTP-сервер
		/// </summary>
		[DefaultValue(null)]
		public string FTPHost = "smolclimb.ru";
		
		/// <summary>
		/// Порт для обмена с FTP-сервером
		/// </summary>
		[DefaultValue(0)]
		public int FTPPort = 21;

		/// <summary>
		/// Пользователь на FTP-сервере
		/// </summary>
		[DefaultValue(null)]
		public string FTPUsername = "extremist_comp";

		/// <summary>
		/// Пароль от FTP-сервера
		/// </summary>
		[DefaultValue(null)]
		public string FTPPassword = "OQFvPx3G";
		#endregion


		/// <summary>
		/// Полный путь к bat-нику, запускаеющему MySQL.
		/// </summary>
		[DefaultValue(GlobalDefines.DEFAULT_XML_STRING_VAL)]
		public string MySQLBatFullPath = GlobalDefines.DEFAULT_XML_STRING_VAL;


		[XmlIgnore]
		public ResourceDictionary m_GlobalResources = new ResourceDictionary()
		{
			Source = new Uri("\\Global\\GlobalResources.xaml", UriKind.RelativeOrAbsolute)
		};


		public void ToDefault()
		{
			CompDir = "";
			HandleFileDeletion = AutodetectOnStart = true;
			ResultGradeCalcMethod = enResultGradeCalcMethod.Floor;

			DefaultCompSettings = new CCompSpecificSets()
			{
				WomenInGroup = 3,
				MenInGroup = 3,
				LeadReportXlsPath = null,
				FirstMemberRow = 8,
				PlaceColumnIndex = 1,
				PersonalDataColumnIndex = 2,
				YearOfBirthColumnIndex = 4,
				TeamColumnIndex = 3,
				PersRepPlaceAggregationMethod = enPersRepPlacesAggregationMethod.Sum,
				PersRepWinnerDetection = enPersRepWinnerDetection.LeadPriority
			};

			PlainResultsFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["PlainResultsBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["PlainResultsForeBrush"] as SolidColorBrush).Color
			};
			NextRoundMembersCountFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["NextRoundMembersCountBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["NextRoundMembersCountForeBrush"] as SolidColorBrush).Color
			};
			InvitedToStartFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["InvitedToStartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["InvitedToStartForeBrush"] as SolidColorBrush).Color
			};
			PreparingFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["PreparingBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["PreparingForeBrush"] as SolidColorBrush).Color
			};
			StayOnStartFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["StayOnStartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["StayOnStartForeBrush"] as SolidColorBrush).Color
			};
			JustRecievedResultFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["JustRecievedResultBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["JustRecievedResultForeBrush"] as SolidColorBrush).Color
			};
			FalsestartFontStyle = new CFontStyleSettings()
			{
				BackgroundColor = (m_GlobalResources["FalsestartBrush"] as SolidColorBrush).Color,
				ForeColor = (m_GlobalResources["FalsestartForeBrush"] as SolidColorBrush).Color
			};
						
			FontSize = 14.0;
			FontFamilyName = "Arial";
			
			dictFileScannerSettings = new SerializableDictionary<string, CFileScannerSettings>();

			dictCompSettings = new SerializableDictionary<long, CCompSpecificSets>();

			ExcelSettings = new CExcelSettings();
			ExcelSettings.ToDefault();

			FTPHost = "smolclimb.ru";
			FTPPort = 21;
			FTPUsername = "extremist_comp";
			FTPPassword = "OQFvPx3G";

			MySQLBatFullPath = null;

			Only75PercentForCalcGrades = false;
			MinAgeToCalcResultGrade = 10;
		}


		/// <summary>
		/// Проверяем значение полей структуры и приводим их в значение по умолчнию, если они не инициализированы
		/// </summary>
		public void CheckAndToDefault()
		{
			if (string.IsNullOrEmpty(CompDir))
				CompDir = "";

			if (PlainResultsFontStyle == null)
			{
				PlainResultsFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["PlainResultsBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["PlainResultsForeBrush"] as SolidColorBrush).Color
				};
			}
						
			if (NextRoundMembersCountFontStyle == null)
			{
				NextRoundMembersCountFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["NextRoundMembersCountBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["NextRoundMembersCountForeBrush"] as SolidColorBrush).Color
				};
			}

			if (InvitedToStartFontStyle == null)
			{
				InvitedToStartFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["InvitedToStartBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["InvitedToStartForeBrush"] as SolidColorBrush).Color
				};
			}

			if (PreparingFontStyle == null)
			{
				PreparingFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["PreparingBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["PreparingForeBrush"] as SolidColorBrush).Color
				};
			}

			if (StayOnStartFontStyle == null)
			{
				StayOnStartFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["StayOnStartBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["StayOnStartForeBrush"] as SolidColorBrush).Color
				};
			}

			if (JustRecievedResultFontStyle == null)
			{
				JustRecievedResultFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["JustRecievedResultBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["JustRecievedResultForeBrush"] as SolidColorBrush).Color
				};
			}

			if (FalsestartFontStyle == null)
			{
				FalsestartFontStyle = new CFontStyleSettings()
				{
					BackgroundColor = (m_GlobalResources["FalsestartBrush"] as SolidColorBrush).Color,
					ForeColor = (m_GlobalResources["FalsestartForeBrush"] as SolidColorBrush).Color
				};
			}

			if (string.IsNullOrEmpty(FontFamilyName))
				FontFamilyName = "Arial";

			if (dictFileScannerSettings == null)
				dictFileScannerSettings = new SerializableDictionary<string, CFileScannerSettings>();

			if (dictCompSettings == null)
				dictCompSettings = new SerializableDictionary<long, CCompSpecificSets>();
			else
			{
				foreach (KeyValuePair<long, CCompSpecificSets> Item in dictCompSettings)
					Item.Value.CheckAndToDefault();
				foreach (KeyValuePair<long, CCompSpecificSets> ItemToDel in
						new List<KeyValuePair<long, CCompSpecificSets>>(dictCompSettings.Where(arg => arg.Value.CompId == GlobalDefines.DEFAULT_XML_INT_VAL)))
				{
					dictCompSettings.Remove(ItemToDel.Key);
				}
			}

			if (DefaultCompSettings == null)
			{
				DefaultCompSettings = new CCompSpecificSets()
				{
					WomenInGroup = 3,
					MenInGroup = 3,
					LeadReportXlsPath = null,
					FirstMemberRow = 8,
					PlaceColumnIndex = 1,
					PersonalDataColumnIndex = 2,
					PersRepPlaceAggregationMethod = enPersRepPlacesAggregationMethod.Sum,
					PersRepWinnerDetection = enPersRepWinnerDetection.LeadPriority
				};
			}

			if (ExcelSettings == null)
			{
				ExcelSettings = new CExcelSettings();
				ExcelSettings.ToDefault();
			}
			else
				ExcelSettings.CheckAndToDefault();

			if (string.IsNullOrEmpty(FTPHost))
				FTPHost = "smolclimb.ru";
			if (FTPPort <= 0)
				FTPPort = 21;
			if (string.IsNullOrEmpty(FTPUsername))
				FTPUsername = "extremist_comp";
			if (string.IsNullOrEmpty(FTPPassword))
				FTPPassword = "OQFvPx3G";

			if (string.IsNullOrEmpty(MySQLBatFullPath))
				MySQLBatFullPath = "D:\\Саша\\Документы\\Эксель\\Для соревнований\\Скалолазание\\Скорость Last Edition\\БД\\RunMySQLServer.lnk";

			if (MinAgeToCalcResultGrade <= 0)
				MinAgeToCalcResultGrade = 10;
		}
	}
}