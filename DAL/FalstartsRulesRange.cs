using DBManager.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.DAL
{
	public class FalstartsRulesRange
	{
		byte m_StartRound = 0;
		byte m_EndRound = 0;

		public byte StartRound
		{
			get { return m_StartRound; }
			set { m_StartRound = value; }
		}

		public enRounds StartRoundInEnum
		{
			get { return (enRounds)m_StartRound; }
			set { m_StartRound = (byte)value; }
		}


		public byte EndRound
		{
			get { return m_EndRound; }
			set { m_EndRound = value; }
		}

		public enRounds EndRoundInEnum
		{
			get { return (enRounds)m_EndRound; }
			set { m_EndRound = (byte)value; }
		}
	}
}
