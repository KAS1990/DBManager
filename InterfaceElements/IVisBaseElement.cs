using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.InterfaceElements
{
	public interface IVisBaseElement
	{
		/// <summary>
		/// Название поля, к которому привязан данный элемент, чтобы можно было его вывести при ошибочном вводе данных в поле
		/// </summary>
		string FieldName
		{
			get;
			set;
		}
		
		/// <summary>
		/// Координаты елемента, если он используется в ListBox
		/// </summary>
		PointI ElementCoords
		{
			get; 
			set;
		}
	}
}
