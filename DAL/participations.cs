using DBManager.Global;
using DBManager.Scanning.XMLDataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager
{
	public partial class participations
	{
		/// <summary>
		/// Равны ли данные, которые заносятся в таблицу participations из xml-файла
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns></returns>
		public bool OnlyFillFromXMLFieldsEqual(CMember rhs, CCompSettings CompSettings)
		{
			bool result = init_grade == (byte)rhs.GradeInEnum;

			if (result)
			{
				if (CompSettings.SecondColNameType == enSecondColNameType.Coach)
				{
					if (rhs.SecondCol == null)
					{
						if (coach != null)
							result = false;
					}
					else if (coach == null)
						result = false;
					else
					{   // Проверяем, не изменилось ли название тренера
						coaches CurCoachInDB = DBManagerApp.m_Entities.coaches.First(arg => arg.id_coach == coach);
						result = CurCoachInDB.name == rhs.SecondCol;
					}
				}
				else
				{
					if (rhs.SecondCol == null)
					{
						if (team != null)
							result = false;
					}
					else if (team == null)
						result = false;
					else
					{   // Проверяем, не изменилось ли название тренера
						teams CurTeamInDB = DBManagerApp.m_Entities.teams.First(arg => arg.id_team == team);
						result = CurTeamInDB.name == rhs.SecondCol;
					}
				}
			}

			return result;
		}
	}
}
