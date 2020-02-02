namespace DBManager.OnlineDB
{
    public enum enOnlineDBKind
    {
        Hard = 1,
        Boulder = 2,
        Speed = 3,
    }

    public enum enOnlineSex
    {
        None = -1,
        Female = 0,
        Male = 1,
    }

    public enum enOnlineGrade
    {
        None = -1,

        /// <summary> б/р </summary>
        WithoutGrade = 1,

        /// <summary> 3 ю </summary>
        Young3 = 4,

        /// <summary> 2 ю </summary>
        Young2 = 3,

        /// <summary> 1 ю </summary>
        Young1 = 2,

        /// <summary> 3 </summary>
        Adult3 = 7,

        /// <summary> 2 </summary>
        Adult2 = 6,

        /// <summary> 1 </summary>
        Adult1 = 5,

        /// <summary> КМС </summary>
        BeforeMaster = 8,

        /// <summary> МС </summary>
        Master = 9,

        /// <summary> МСМК </summary>
        InternationalMaster = 11,
    }
}
