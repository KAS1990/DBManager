using DBManager.Global;

namespace DBManager.DAL
{
    public class FalstartsRulesRange
    {
        private byte m_StartRound = 0;
        private byte m_EndRound = 0;

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
