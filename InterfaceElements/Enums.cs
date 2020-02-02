namespace DBManager.InterfaceElements
{
    /// <summary>
    /// Какую проверку делает элемент управления при потере им фокуса, если в него можно что-то вводить
    /// </summary>
    public enum enInputCheckType
    {
        None,

        Float,
        FloatOrEmpty,
        PositiveFloat,
        PositiveFloatOrEmpty,
        NotNegativeFloat,
        NotNegativeFloatOrEmpty,

        Double,

        Int,
        PositiveInt,
        NotNegativeInt,
        NotNegativeIntOrEmpty,

        /// <summary>
        /// Проверка на ввод времени, при этом нулевое время считатется ошибкой
        /// </summary>
        Time,
        /// <summary>
        /// Проверка на ввод времени, при этом нулевое время ошибкой не считатется
        /// </summary>
        TimeZeroTimeAllowed,
        /// <summary>
        /// Проверка на ввод времени, при этом нулевое время считатется ошибкой или пустой строки
        /// </summary>
        TimeOrEmpty,

        /// <summary>
        /// Нельзя оставлять поле пустым или заполнять одними пробелами
        /// </summary>
        NotEmpty
    }


    /// <summary>
    /// Результат работы функции CaclValue для элементов управления, куда можно что-то вводить
    /// </summary>
    public enum enCaclValueResult
    {
        /// <summary>
        /// Ошибок нет
        /// </summary>
        NoError,
        /// <summary>
        /// Ошибка связана с несоотвествием введённого значения и InputCheckType
        /// </summary>
        InputCheckError,
        /// <summary>
        /// AdditionalRightInputCond == false
        /// </summary>
        AdditionalCondError
    }
}