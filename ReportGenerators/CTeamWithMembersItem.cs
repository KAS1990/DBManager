using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBManager.ReportGenerators
{
	public class CTeamWithMembersItem
	{
		/// <summary>
		/// Сведения о команде
		/// </summary>
		public CTeamForReportItem m_TeamInfo = null;

		/// <summary>
		/// Участники команды в трудности.
		/// Ключ - идентификатор группы
		/// </summary>
		public Dictionary<long, List<CPersonalItem>> m_dictSpeedMembers = new Dictionary<long, List<CPersonalItem>>();
		/// <summary>
		/// Участники команды в скорости.
		/// Ключ - идентификатор группы
		/// </summary>
		public Dictionary<long, List<CPersonalItem>> m_dictLeadMembers = new Dictionary<long, List<CPersonalItem>>();

		/// <summary>
		/// Баллы, заработанные командой в трудности
		/// </summary>
		float m_LeadBalls = 0;
		public float LeadBalls
		{
			get
			{
				if (m_LeadBalls == 0)
					m_LeadBalls = m_dictLeadMembers.Sum(gr => gr.Value.Sum(member => member.m_Balls));
				
				return m_LeadBalls;
			}
		}

		/// <summary>
		/// Баллы, заработанные командой в скорости
		/// </summary>
		float m_SpeedBalls = 0;
		public float SpeedBalls
		{
			get
			{
				if (m_SpeedBalls == 0)
					m_SpeedBalls = m_dictSpeedMembers.Sum(gr => gr.Value.Sum(member => member.m_Balls));

				return m_SpeedBalls;
			}
		}

		/// <summary>
		/// Результат команды в баллах
		/// </summary>
		public float TotalBalls
		{
			get { return LeadBalls + SpeedBalls; }
		}


		public void ResetBalls()
		{
			m_LeadBalls = m_SpeedBalls = 0;
		}
	}
}
