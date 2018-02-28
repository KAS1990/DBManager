using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CMemberAndPart : CDBAdditionalClassBase
	{
		public members Member = null;
		public participations Participation = null;

		public CMemberAndPart()
		{
		}

		
		public CMemberAndPart(members member, participations participation)
		{
			Member = member;
			Participation = participation;
		}
	}
}
