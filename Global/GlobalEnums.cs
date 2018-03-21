using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Global
{
	/// <summary>
	/// 
	/// </summary>
	public enum enChangeReason
	{
		crNone = 0,
		/// <summary>
		/// Изменений много, поэтому внешнее ПО должно забрать из XML всё и перезалить его в БД
		/// </summary>
		crWholeContent = 1,
		/// <summary>
		/// Удалилась строка. Её номер указан в аргументе
		/// </summary>
		crRowDeleted = 2,
		/// <summary>
		/// Добавлась строка после строки с номером, указанным в аргументе
		/// </summary>
		crRowAdded = 3,
		/// <summary>
		/// Изменилась строка. Её номер указан в аргументе
		/// </summary>
		crRowChanged = 4,
		/// <summary>
		/// Изменились только результаты. ПО должно перезалить все результаты,
		/// т.к. в Excel очень долго выяснять, какие конкретно результаты изменились
		/// </summary>
		crResultsChanged = 5,
		/// <summary>
		/// 
		/// </summary>
		crQualifSorted = 6,
		/// <summary>
		/// Раунд завершён. Название раунда понятно из узла, в котором произошли изменения. Название следующего раунда указано в аргументе
		/// </summary>
		crRoundFinished = 7,
	}


	public enum enScanningThreadState
	{
		/// <summary>
		/// поток не запущен
		/// </summary>
		Stopped,
		/// <summary>
		/// Поток работает
		/// </summary>
		Worked
	}


	public enum enSex
	{
		None = -1,
		Male = 0,
		Female = 1
	}


	public enum enGrade
	{
		None = -1,
		/// <summary> б/р </summary>
		WithoutGrade = 1,
		
		/// <summary> 3 ю </summary>
		Young3 = 2,
		
		/// <summary> 2 ю </summary>
		Young2 = 3,

		/// <summary> 1 ю </summary>
		Young1 = 4,

		/// <summary> 3 </summary>
		Adult3 = 5,

		/// <summary> 2 </summary>
		Adult2 = 6,

		/// <summary> 1 </summary>
		Adult1 = 7,

		/// <summary> КМС </summary>
		BeforeMaster = 8,

		/// <summary> МС </summary>
		Master = 9
	}


	public enum enRounds
	{
		None = -1,
		/// <summary> Квалификация_1 </summary>
		Qualif = 1,

		/// <summary> Квалификация_2 </summary>
		Qualif2 = 2,

		/// <summary> Одна_восьмая_финала </summary>
		OneEighthFinal = 3,

		/// <summary> Четвертьфинал </summary>
		QuaterFinal = 4,

		/// <summary> Полуфинал </summary>
		SemiFinal = 5,

		/// <summary> Финал </summary>
		Final = 6,

		/// <summary> Итоговый протокол </summary>
		Total = 7
	}


	public enum enSecondColNameType
	{
		None = 0,
		Team = 1,
		Coach = 2
	}


	/// <summary>
	/// Результат функции GlobalDefines.ObjectBaseEquals
	/// </summary>
	public enum enObjectBaseEqualsResult
	{
		/// <summary> Вызывающая функция должна вернуть true </summary>
		True,
		
		/// <summary> Вызывающая функция должна вернуть false </summary>
		False,

		/// <summary> Вызывающая функция должна продолжать сравнение </summary>
		ContinueComparing
	}


	public enum enCondFormating
	{
		None = 0,
		StayOnStart = 1,
		JustRecievedResult = 2,
		Preparing = 3
	}


	/// <summary>
	/// Результат функции UpdateResults.
	/// </summary>
	[Flags]
	public enum enChangedResult
	{
		None = 0,
		
		Route1Time = 1 << 0,
		Route2Time = 1 << 1,
		SumTime = 1 << 2,
		
		Route1CondFormatting = 1 << 3,
		Route2CondFormatting = 1 << 4,
		SumCondFormatting = 1 << 5,

        Route1AdditionalEvent = 1 << 6,
        Route2AdditionalEvent = 1 << 7,
        SumAdditionalEvent = 1 << 8,
    }


	/// <summary>
	/// Поле, по которому выполняется фильтрация
	/// </summary>
	public enum enFilterTarget
	{
		SecondCol,
		YearOfBirth,
		Grade,
	}


	public enum enReportTypes
	{
		/// <summary> Квалификация_1 </summary>
		Qualif = 1,

		/// <summary> Квалификация_2 </summary>
		Qualif2 = 2,

		/// <summary> Одна_восьмая_финала </summary>
		OneEighthFinal = 3,

		/// <summary> Четвертьфинал </summary>
		QuaterFinal = 4,

		/// <summary> Полуфинал </summary>
		SemiFinal = 5,

		/// <summary> Финал </summary>
		Final = 6,

		/// <summary> Итоговый протокол </summary>
		Total = 7,

		/// <summary> Командный зачёт </summary>
		Team = 8,

		/// <summary> Двоеборье </summary>
		Personal = 9,

		/// <summary> Стартовый </summary>
		StartList = 10,
	}


	public enum enPersRepPlacesAggregationMethod
	{
		/// <summary>
		/// Места, занятые участниками в скорости и трудности, суммируются для получения итогова маста 
		/// </summary>
		Sum,

		/// <summary>
		/// Места, занятые участниками в скорости и трудности, перемножаются для получения итогова маста 
		/// </summary>
		Mul
	}


	/// <summary>
	/// Метод определения победителя в личном зачёте при равенстве мест
	/// </summary>
	public enum enPersRepWinnerDetection
	{
		/// <summary>
		/// У кого есть выше место, тот и победил
		/// </summary>
		BestPlace,

		/// <summary>
		/// У кого меньшая разница, тот и победил
		/// </summary>
		LessDifference,

		/// <summary>
		/// У кого лучшее место в скорости, тот и победил
		/// </summary>
		SpeedPriority,

		/// <summary>
		/// У кого лучшее место в трудности, тот и победил
		/// </summary>
		LeadPriority,
	}


	public enum enFTPSheetGeneratorTypes
	{
		/// <summary> Квалификация_1 </summary>
		Qualif = 1,

		/// <summary> Квалификация_2 </summary>
		Qualif2 = 2,

		/// <summary> Одна_восьмая_финала </summary>
		OneEighthFinal = 3,

		/// <summary> Четвертьфинал </summary>
		QuaterFinal = 4,

		/// <summary> Полуфинал </summary>
		SemiFinal = 5,

		/// <summary> Финал </summary>
		Final = 6,

		/// <summary> Итоговый протокол </summary>
		Total = 7,

		/// <summary> Стартовый протокол </summary>
		Start = 8,
	}


	public enum enFTPLogItemType
	{
		None,
		Error,
		OK
	}


	public enum enHighlightGradesType
	{
		None,

		/// <summary>
		/// Выделить выполненные разряды
		/// </summary>
		ResultGrades,

		/// <summary>
		/// Выделить участников, выполнивших разряды
		/// </summary>
		CarryoutGrades,

		/// <summary>
		/// Выделить участников, подтвердивших разряды
		/// </summary>
		СonfirmGrades
	}


	public enum enCellType
	{
		None,
		Route1,
		Route2,
		Sum,
		StartNumber,
		SurnameAndName
	}


	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum enAdditionalEventTypes
	{
		None = 0,

		/// <summary>
		/// Фальстарт
		/// </summary>
		Falsestart = 1 << 0,
		/// <summary>
		/// Неявка 
		/// </summary>
		DontAppear = 1 << 1,
		/// <summary>
		/// вк
		/// </summary>
		BeyondQualif = 1 << 2,
		/// <summary>
		/// снят
		/// </summary>
		Disqualif = 1 << 3,
	}
}
