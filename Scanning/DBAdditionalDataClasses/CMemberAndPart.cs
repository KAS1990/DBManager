using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Scanning.DBAdditionalDataClasses
{
	public class CMemberAndPart : CDBAdditionalClassBase, ICanRefreshFrom
	{
		public members Member = null;
		public participations Participation = null;


		#region Конструкторы
		public CMemberAndPart()
		{
		}

		
		public CMemberAndPart(members member, participations participation)
		{
			Member = member;
			Participation = participation;
		}
		#endregion


		public override void RefreshFrom(ICanRefreshFrom rhs,
										bool SkipNullsForObjects,
										bool SkipNullsForNullables)
		{
			base.RefreshFrom(rhs, SkipNullsForObjects, SkipNullsForNullables);

			CMemberAndPart rhsMemberAndPart = rhs as CMemberAndPart;

			if (rhsMemberAndPart == null)
				return;

			if (!SkipNullsForObjects || rhsMemberAndPart.Member != null)
				Member = rhsMemberAndPart.Member;

			if (!SkipNullsForObjects || rhsMemberAndPart.Participation != null)
				Participation = rhsMemberAndPart.Participation;
		}
	}
}
