using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.Scanning
{
	public interface ICanRefreshFrom
	{
		void RefreshFrom(ICanRefreshFrom rhs,
						bool SkipNullsForObjects,
						bool SkipNullsForNullables);
	}
}
