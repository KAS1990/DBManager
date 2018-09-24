using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;

namespace DBManager.Scanning
{
	/// <summary>
	/// Тип произведённых в таблице изменений
	/// </summary>
	public enum enDataChangesTypes
	{
		/// <summary>
		/// Добавление
		/// </summary>
		Add,
		/// <summary>
		/// Добавилось несколько элементов
		/// </summary>
		AddManyPcs,
		/// <summary>
		/// Удаление
		/// </summary>
		Delete,
		/// <summary>
		/// Изменение
		/// </summary>
		Changing,
		/// <summary>
		/// Синхронизация с файлом произошла неудачна
		/// </summary>
		SyncFailed,
		/// <summary>
		/// Квалификация отсртирована
		/// </summary>
		QualifSorted,
		/// <summary>
		/// Раунд завершился
		/// </summary>
		RoundFinished,
		/// <summary>
		/// При сканировании случилось исключение. Само исключение находится в поле Argument
		/// </summary>
		ExceptionInScanner,
		/// <summary>
		/// Изменилось всего несколько строк, поэтому можно быстрее отобразить изменения на экране
		/// </summary>
		OnlySomeRowsChanged
	}


	/// <summary>
	/// Объекты, которые были изменены
	/// </summary>
	[Flags]
	public enum enDataChangedObjects
	{
		None = 0,
		/// <summary>
		/// Изменились настройки соревнований: таблицы descriptions, groups и round_dates. Что конкретно поменялось останется неизвестным.
		/// </summary>
		CompSettings = 1,
		/// <summary>
		/// Изменились данные в таблицах members и participations, а также, возможно, coaches и teams
		/// </summary>
		Members = 2,
		/// <summary>
		/// Изменились результаты соревнований: таблицы results_speed и participations
		/// </summary>
		Results = 4,
		/// <summary>
		/// Изменились путь к файлам
		/// </summary>
		Paths = 8,
		/// <summary>
		/// Добавлена или изменена группа
		/// </summary>
		Group = 16,
		/// <summary>
		/// Произошло исключение
		/// </summary>
		Exception = 32
	}

	
	public class CDataChangedInfo
	{
		public readonly CScannerBase OriginalSource;

		/// <summary>
		/// Тип изменений
		/// </summary>
		public enDataChangesTypes ChangingType;
		/// <summary>
		/// Какой объект БД был изменён
		/// </summary>
		public enDataChangedObjects ChangedObjects;

		public object PrevVal = null;
		public object CurVal = null;
		public enChangeReason ChangeReason = enChangeReason.crNone;
		public object Argument = null;
		public List<object> ListArguments = null;

		/// <summary>
		/// Идентификатор записи в БД
		/// </summary>
		public long ID = GlobalDefines.DEFAULT_XML_INT_VAL;

		public long GroupID = GlobalDefines.DEFAULT_XML_INT_VAL;

		
		public CDataChangedInfo(CScannerBase originalSource)
		{
			OriginalSource = originalSource;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	public delegate void DataChangedEventHandler(CScannerBase sender, DataChangedEventArgs e);


	public class DataChangedEventArgs : EventArgs
	{
		readonly List<CDataChangedInfo> m_Changes = null;
		/// <summary>
		/// Список изменений
		/// </summary>
		public ReadOnlyCollection<CDataChangedInfo> Changes
		{
			get { return new ReadOnlyCollection<CDataChangedInfo>(m_Changes); }
		}
		
		public DataChangedEventArgs(List<CDataChangedInfo> changes)
		{
			m_Changes = changes;
		}
	}
}
