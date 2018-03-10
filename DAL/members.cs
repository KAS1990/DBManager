using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager
{
	public partial class members
	{
		/// <summary>
		/// Равны ли данные об участниках без учёта индексов
		/// </summary>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public bool OnlyDataFieldsEqual(members rhs)
		{
			return name == rhs.name &&
					surname == rhs.surname &&
					year_of_birth == rhs.year_of_birth &&
					sex == rhs.sex;
		}
	}
}
