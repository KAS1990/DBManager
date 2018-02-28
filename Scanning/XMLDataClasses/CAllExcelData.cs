using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using DBManager.Global;
using System.Xml;
using System.Reflection;
using System.Xml.Schema;
using System.Collections;

namespace DBManager.Scanning.XMLDataClasses
{
	/// <summary>
	/// Класс, хранящий все данные, прочитанные из XML-файла  
	/// </summary>
	[XmlRoot("Root", Namespace = "")]
	public class CAllExcelData : CXMLSerializerBase
	{
		#region Сериализуемые свойства
		/// <summary>
		/// Настройки соревнований
		/// </summary>
		[XmlElement(ElementName = "Desc", IsNullable = false)]
		[DefaultValue(null)]
		public CCompSettings Settings { get; set; }

		/// <summary>
		/// Участники квалификации и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.QUALIF_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults QualifResults { get; set; }

		/// <summary>
		/// Участники второй квалификации и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.QUALIF2_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults Qualif2Results { get; set; }

		/// <summary>
		/// Участники одной восьмой финала и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.ONE_EIGHTH_FINAL_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults OneEighthFinalResults { get; set; }

		/// <summary>
		/// Участники четвертьфинала и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.QUATER_FINAL_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults QuaterFinalResults { get; set; }

		/// <summary>
		/// Участники полуфинала и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.SEMI_FINAL_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults SemiFinalResults { get; set; }

		/// <summary>
		/// Участники финала и их результаты
		/// </summary>
		[XmlElement(ElementName = GlobalDefines.FINAL_NODE_NAME, IsNullable = false)]
		[DefaultValue(null)]
		public CSpeedResults FinalResults { get; set; }
		#endregion


		#region Несериализуемые свойства
		/// <summary>
		/// Итератор, который возвращает все объекты CSpeedResults, которые не равны null
		/// </summary>
		public IEnumerable<CSpeedResults> AllFilledResults
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


		public CSpeedResults RoundAfterQualif
		{
			get
			{
				if (Qualif2Results != null)
					return Qualif2Results;
				if (OneEighthFinalResults != null)
					return OneEighthFinalResults;
				if (QuaterFinalResults != null)
					return QuaterFinalResults;
				if (SemiFinalResults != null)
					return SemiFinalResults;
				if (FinalResults != null)
					return FinalResults;

				return null;
			}
		}
		#endregion


		public CAllExcelData()
		{
			CXMLSerializerBase.SetDefaultValsForAllProps(this);
		}


		public void ClearAllChangeReasons()
		{
			foreach (CSpeedResults result in AllFilledResults)
			{
				result.ChangeReason = enChangeReason.crNone;
				result.Argument = GlobalDefines.DEFAULT_XML_STRING_VAL;
			}
		}
	}
}
