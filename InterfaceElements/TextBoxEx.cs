using DBManager.Global;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using WPFLocalization;

namespace DBManager.InterfaceElements
{
    /// <summary>
    /// TextBox, в который добавлена дополнительная функциональность, чтобы сделать наш интерфейс лучшим в мире
    /// </summary>
    [RuntimeNameProperty("Name")]
    [DebuggerDisplay("Text = {Text}")]
    public class TextBoxEx : TextBox, IVisBaseElement
    {
        private const string STD_ERROR_MSG_RES_STRING = "resfmtInvalidNumber";
        private const string STD_ERROR_MSG_RES_STRING_WITH_ROW_NUM = "resfmtInvalidNumberWithRowNum";

        private const int MIN_TIME_BETWEEN_CHECK_ERROR_INPUT_MS = 1000;

        /// <summary>
        /// Текст, который был в поле, когда на него перевели фокус. Поле не очищается после LostFocus
        /// </summary>
        protected string m_TextOnGotFocus = "";

        /// <summary>
        /// Стандартный цвет элемента, т.е. тот, что был при его создании или смене свойства Background
        /// </summary>
        private Brush m_StdElementColor = Brushes.Black;

        /// <summary>
        /// Стандартная ширина границы елемента, т.е. та, что была при его создании или смене свойства BorderThickness
        /// </summary>
        private Thickness m_StdBorderThickness = new Thickness(5);

        /// <summary>
        /// Имеет ли сейчас элемент цвет m_StdElementColor или ErrorBackground
        /// </summary>
        private bool m_IsErrorColor = false;

        /// <summary>
        /// Время в милисекундах, когда было показано сообщение об ошибке ввода.
        /// Это поле совместно с константой MIN_TIME_BETWEEN_CHECK_ERROR_INPUT_MS позволяет исключить появление подряд нескольких сообщений об ошибках
        /// </summary>
        private int m_ShowErrorMsgMs = 0;


        #region Свойство Value
        private object m_Value = null;
        /// <summary>
        /// Значение, введённое в поле.
        /// Если InputCheckType = enInputCheckType.None, то это строка,
        /// если InputCheckType = время, то CVisTime
        /// если InputCheckType = числовой тип - float или int,
        /// иначе значение свойства Text - string
        /// Это значение заполняется только при покидании полем фокуса или с помощью метода CalcValue
        /// </summary>
        public object Value
        {
            get { return m_Value; }
            protected set { m_Value = value; }
        }
        #endregion


        #region Свойства типа bool с "автосбросом" или "автоустановкой"
        #region Свойство HandleGotFocus
        private bool m_HandleGotFocus = true;

        /// <summary>
        /// Нужно ли выполнять код в OnGotFocus.
        /// Это не нужно делать, если фокус перешёл на ComboBox, но пользователь ввёл неверные данные,
        /// и после показа сообщения об ошибке почему-то срабатывает OnGotFocus, который не нужно обрабатывать.
        /// Может быть и другие компоненты ведут себя так же, но защита в виде этого свойства нас спасёт и от этого.
        /// После чтения это свойство автоматически устанавливается в true
        /// </summary>
        private bool HandleGotFocus
        {
            get
            {
                bool result = m_HandleGotFocus;
                m_HandleGotFocus = true;
                return result;
            }
            set { m_HandleGotFocus = value; }
        }
        #endregion


        #region Свойство PressedCancelButton
        private bool m_PressedCancelButton = false;

        /// <summary>
        /// Устанавливается в методе OnPreviewLostKeyboardFocus, т.к. он срабатывает раньше всех, и проверяется в OnLostFocus.
        /// Это поле нужно чтобы не срабатывало стандартное OnLostFocus, если нажали CancelButton,
        /// т.к. во всех местах программы проверки производятся на событии LostFocus и при нажатии на CancelButton они срабатывать не должны.
        /// После чтения это свойство автоматически устанавливается в false
        /// </summary>
        private bool PressedCancelButton
        {
            get
            {
                bool result = m_PressedCancelButton;
                m_PressedCancelButton = false;
                return result;
            }
            set { m_PressedCancelButton = value; }
        }
        #endregion


        #region Свойство HandleTextChanged
        private bool m_HandleTextChanged = true;

        /// <summary>
        /// Нужно ли выполнять код в OnTextChanged.
        /// Это не нужно делать, если возвращаем отменяем ввод клавишей ESC или
        /// восстанавливаем данные после ошибки, если ReturnPrevValOnErrInput == true
        /// После чтения это свойство автоматически устанавливается в true
        /// </summary>
        private bool HandleTextChanged
        {
            get
            {
                bool result = m_HandleTextChanged;
                m_HandleTextChanged = true;
                return result;
            }
            set { m_HandleTextChanged = value; }
        }
        #endregion


        #region Свойство OnPreviewLostKeyboardFocusFired
        private bool m_OnPreviewLostKeyboardFocusFired = false;

        /// <summary>
        /// Было ли вызвано OnPreviewLostKeyboardFocus до OnLostFocus. Если этого не было, то не нужно ничего делать в OnLostFocus.
        /// После чтения это свойство автоматически устанавливается в false
        /// </summary>
        private bool OnPreviewLostKeyboardFocusFired
        {
            get
            {
                bool result = m_OnPreviewLostKeyboardFocusFired;
                m_OnPreviewLostKeyboardFocusFired = false;
                return result;
            }
            set { m_OnPreviewLostKeyboardFocusFired = value; }
        }
        #endregion
        #endregion


        #region Свойство ErrorColor
        private Brush m_ErrorColor = Brushes.Red;
        /// <summary>
        /// Цвет фона елемента, если в него ввели неверные данные
        /// </summary>
        public Brush ErrorColor
        {
            get { return m_ErrorColor; }
            set { m_ErrorColor = value; }
        }
        #endregion


        #region Свойство ErrorBorderThickness
        private Thickness m_ErrorBorderThickness = new Thickness(2);
        /// <summary>
        /// Цвет фона елемента, если в него ввели неверные данные
        /// </summary>
        public Thickness ErrorBorderThickness
        {
            get { return m_ErrorBorderThickness; }
            set { m_ErrorBorderThickness = value; }
        }
        #endregion


        #region Свойство FieldName
        private string m_FieldName = "";
        /// <summary>
        /// Название поля, к которому привязан данный элемент,
        /// чтобы можно было его вывести при ошибочном вводе данных в поле
        /// </summary>
        public string FieldName
        {
            get { return m_FieldName; }
            set { m_FieldName = value; }
        }
        #endregion


        #region Свойство ElementCoords
        private PointI m_ElementCoords = new PointI(-1, -1);

        /// <summary>
        /// Координаты елемента, если он используется в ListBox или Grid
        /// </summary>
        public PointI ElementCoords
        {
            get { return m_ElementCoords; }
            set { m_ElementCoords = value; }
        }
        #endregion


        #region Свойство ErrorMsgResString
        private string m_ErrorMsgResString = STD_ERROR_MSG_RES_STRING;
        /// <summary>
        /// Идентификатор строки из ресурсов,
        /// которая используется для показа сообщения, если ElementCoords.IsEmpty()
        /// При задании для этой строки нестандартного значения, нужно учитывать, что в ней должно быть место для стандартной концовки, если UseStdErrMsgEnd == true,
        /// и FieldName, если string.IsNullOrEmpty(FieldName) == false
        /// </summary>
        public string ErrorMsgResString
        {
            get { return m_ErrorMsgResString; }
            set { m_ErrorMsgResString = value; }
        }
        #endregion


        #region Свойство ErrorMsgString
        private string m_ErrorMsgString = null;
        /// <summary>
        /// Строка, содержащая выводимое при ошибке значение, если ElementCoords.IsEmpty().
        /// Это свойство используется, если ErrorMsgResString == null.
        /// При задании для этой строки нестандартного значения, нужно учитывать, что в ней должно быть место для стандартной концовки, если UseStdErrMsgEnd == true,
        /// и FieldName, если string.IsNullOrEmpty(FieldName) == false
        /// </summary>
        public string ErrorMsgString
        {
            get { return m_ErrorMsgString; }
            set { m_ErrorMsgString = value; }
        }
        #endregion


        #region Свойство ErrorMsgResStringWithRowNum
        private string m_ErrorMsgResStringWithRowNum = STD_ERROR_MSG_RES_STRING_WITH_ROW_NUM;
        /// <summary>
        /// Идентификатор строки из ресурсов,
        /// которая используется для показа сообщения, если !ElementCoords.IsEmpty()
        /// При задании для этой строки нестандартного значения, нужно учитывать, что в ней должно быть место для вывода номера строки,
        /// стандартной концовки, если UseStdErrMsgEnd == true и FieldName, если string.IsNullOrEmpty(FieldName) == false
        /// </summary>
        public string ErrorMsgResStringWithRowNum
        {
            get { return m_ErrorMsgResStringWithRowNum; }
            set { m_ErrorMsgResStringWithRowNum = value; }
        }
        #endregion


        #region Свойство UseStdErrMsgEnd
        private bool m_UseStdErrMsgEnd = true;
        /// <summary>
        /// Нужно ли добавлять к сообщениям ErrorMsgResString и ErrorMsgResStringWithRowNum стандартную фразу:
        /// "Повторите ввод или нажмите клавишу ESC для возврата последних корректных данных." (строка из ресурсов - resStdInputErrMsgEnd)
        /// </summary>
        public bool UseStdErrMsgEnd
        {
            get { return m_UseStdErrMsgEnd; }
            set { m_UseStdErrMsgEnd = value; }
        }
        #endregion


        #region Свойство AddWhatWrongStr
        private bool m_AddWhatWrongStr = true;
        /// <summary>
        /// Нужно ли добавлять выводить в сообщении об ошибке "что должно быть введено",
        /// например "должно быть введено время в формате "сс", "мм:сс" или "чч:мм:сс"".
        /// Это строка берётся из ресурсов имеет название: "resfmt" + InputCheckType.ToString() + "InvalidNumber"
        /// </summary>
        public bool AddWhatWrongStr
        {
            get { return m_AddWhatWrongStr; }
            set { m_AddWhatWrongStr = value; }
        }
        #endregion


        #region Свойство ReturnPrevValOnErrInput
        private bool m_ReturnPrevValOnErrInput = false;
        /// <summary>
        /// Нужно ли вернуть старое значение, если при потере фокуса оказалось, что пользователь ввёл неправильное значение
        /// </summary>
        public bool ReturnPrevValOnErrInput
        {
            get { return m_ReturnPrevValOnErrInput; }
            set { m_ReturnPrevValOnErrInput = value; }
        }
        #endregion


        #region Свойство InputCheckResult
        private enCaclValueResult m_InputCheckResult = enCaclValueResult.NoError;
        /// <summary>
        /// Результат проверки ввода данных
        /// </summary>
        public enCaclValueResult InputCheckResult
        {
            get { return m_InputCheckResult; }
            protected set
            {
                if (m_InputCheckResult != value)
                {
                    m_InputCheckResult = value;
                    SwitchErrorColor(m_InputCheckResult != enCaclValueResult.NoError);
                }
            }
        }
        #endregion


        #region Свойство IsRightInput
        /// <summary>
        /// Пользователь ввёл правильные данные.
        /// </summary>
        public bool IsRightInput
        {
            get
            {
                return InputCheckResult == enCaclValueResult.NoError;
            }
        }
        #endregion


        #region Свойство InputCheckType
        protected enInputCheckType m_InputCheckType = enInputCheckType.None;
        /// <summary>
        /// Какую проверку производит TextBoxEx при потере им фокуса
        /// </summary>
        public virtual enInputCheckType InputCheckType
        {
            get { return m_InputCheckType; }
            set
            {
                if (m_InputCheckType != value)
                {
                    m_InputCheckType = value;
                    CalcValue(false);
                }
            }
        }
        #endregion


        #region Свойство Modified. только для чтения
        /// <summary>
        /// Изменились ли данные в поле с момента последнего получения фокуса
        /// </summary>
        public bool Modified
        {
            get
            {
                return m_TextOnGotFocus != Text;
            }
        }
        #endregion


        #region Свойство AdditionalRightInputCond
        private Predicate<TextBoxEx> m_AdditionalRightInputCond = null;
        /// <summary>
        /// Дополнительное условие, которое проверяется в CalcValue.
        /// Это условие проверяется только при потере фокуса полем ввода, т.к. пока нужно именно так, потом, возможно, придётся добавить дополнительный функционал.
        /// Если это условие false, то CalcValue возвращает AdditionalCondError и стандартные ссобщения об ошибках не показываются,
        /// т.е. все сообщения должны выдаваться в AdditionalRightInputCond
        /// </summary>
        public Predicate<TextBoxEx> AdditionalRightInputCond
        {
            get { return m_AdditionalRightInputCond; }
            set { m_AdditionalRightInputCond = value; }
        }
        #endregion


        #region Свойство ShowErrMsgOnAddCondErr
        private bool m_ShowErrMsgOnAddCondErr = false;
        /// <summary>
        /// Нужно ли покащывать сообщение об ошибке, если нарушено условие AdditionalRightInputCond
        /// </summary>
        public bool ShowErrMsgOnAddCondErr
        {
            get { return m_ShowErrMsgOnAddCondErr; }
            set { m_ShowErrMsgOnAddCondErr = value; }
        }
        #endregion


        #region Свойство PrevValReturned
        private bool m_PrevValReturned = false;
        /// <summary>
        /// Было возвращено предыдущее значение (т.е. то, которое было при получении фокуса). Поле сбрасывается при получении фокуса
        /// </summary>
        public bool PrevValReturned
        {
            get { return m_PrevValReturned; }
            protected set { m_PrevValReturned = value; }
        }
        #endregion


        public TextBoxEx()
        {
            m_StdElementColor = Background;
            m_StdBorderThickness = BorderThickness;

            SetResourceReference(StyleProperty, typeof(TextBox));
        }


        /// <summary>
        /// Эта процедура вызывается до срабатывания события GotFocus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (HandleGotFocus)
            {
                InputCheckResult = enCaclValueResult.NoError;
                m_TextOnGotFocus = Text;
            }
            PrevValReturned = false;
            base.OnGotFocus(e);
        }


        #region Обработка потери фокуса
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            OnPreviewLostKeyboardFocusFired = true;

            if (!IsVisible)
                return;

            /* Проверяем, не должен ли фокус перейти на кнопку типа CancelButton */
            if (e.NewFocus is CancelButton || e.NewFocus is Window)
            {   /* Да => пользователь хочет закрыть форму без проверок ввода */
                e.Handled = e.NewFocus is Window;
                PressedCancelButton = true;
                return;
            }
            PressedCancelButton = false;

            if (Modified)
                InputCheckResult = CalcValue();

            if (IsRightInput)
            {
                if ((InputCheckType == enInputCheckType.Time ||
                    InputCheckType == enInputCheckType.TimeOrEmpty ||
                    InputCheckType == enInputCheckType.TimeZeroTimeAllowed) &&
                    Value != null &&
                    !string.IsNullOrEmpty(Text))
                {
                    Text = Value.ToString();
                }
                base.OnPreviewLostKeyboardFocus(e);
            }
            else
            {
                e.Handled = true;
                OnErrorInput(ShowErrMsgOnAddCondErr || InputCheckResult != enCaclValueResult.AdditionalCondError);
            }
        }


        /// <summary>
        /// Эта процедура вызывается до срабатывания события LostFocus, но после OnPreviewLostKeyboardFocus (если OnPreviewLostKeyboardFocus вызвана и проверка прошла успешно),
        /// поэтому в обработчике LostFocus не нужно проверять корректность ввода,
        /// т.к. эта функция написана так, что в обработчик LostFocus попадём только в случае правильного ввода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (PressedCancelButton || !OnPreviewLostKeyboardFocusFired || !IsRightInput)
                return;

            base.OnLostFocus(e);
        }
        #endregion


        /// <summary>
        /// Убирает "ошибочную подсветку", если меняем текст
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (HandleTextChanged)
                InputCheckResult = enCaclValueResult.NoError;
            CalcValue(false);
            base.OnTextChanged(e);
        }


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (!IsRightInput)
                    {
                        PrevValReturned = true;
                        Text = m_TextOnGotFocus;
                        e.Handled = true;
                    }
                    break;

                case Key.Enter:
                    if (Modified)
                    {   /* Нужно имитировать изменение текста в TextBoxEx'у */
                        InputCheckResult = CalcValue();
                        if (IsRightInput)
                        {
                            RaiseEvent(new RoutedEventArgs(TextBoxEx.LostFocusEvent, this));
                            m_TextOnGotFocus = Text;
                        }
                        else
                        {
                            OnErrorInput(ShowErrMsgOnAddCondErr || InputCheckResult != enCaclValueResult.AdditionalCondError);
                            e.Handled = true;
                        }
                        return;
                    }
                    break;
            }

            base.OnPreviewKeyDown(e);
        }


        /// <summary>
        /// Действия, которые нужно совершить, если пользователь ввёл неверные данные
        /// </summary>
        /// <param name="ShowErrMsg">
        /// Нужно ли показывать сообщения об ошибке. Это не нужно делать, например, если AdditionalRightInputCond == false
        /// </param>
        private void OnErrorInput(bool ShowErrMsg)
        {
            if (InputCheckType == enInputCheckType.None && AdditionalRightInputCond == null)
                return;

            if (ReturnPrevValOnErrInput)
            {
                HandleTextChanged = false;
                PrevValReturned = true;
                Text = m_TextOnGotFocus;
            }

            if (ShowErrMsg && Math.Abs(Environment.TickCount - m_ShowErrorMsgMs) > MIN_TIME_BETWEEN_CHECK_ERROR_INPUT_MS)
            {
                string ResMsgStr = null;
                string Msg = null;

                string strErrDesc = LocalizationManager.ResourceManager.GetString("resfmt" + InputCheckType.ToString() + "InvalidNumber");
                if (InputCheckType == enInputCheckType.Float ||
                    InputCheckType == enInputCheckType.FloatOrEmpty ||
                    InputCheckType == enInputCheckType.NotNegativeFloat ||
                    InputCheckType == enInputCheckType.NotNegativeFloatOrEmpty ||
                    InputCheckType == enInputCheckType.PositiveFloat ||
                    InputCheckType == enInputCheckType.PositiveFloatOrEmpty)
                {   /* Добавляем в строку десятичный разделитель, свойственный для выбранного языка */
                    strErrDesc = string.Format(strErrDesc, Properties.Resources.resDecimalPointOrComma);
                }

                if (ElementCoords.IsEmpty())
                {
                    ResMsgStr = ErrorMsgResString == null ?
                                    ErrorMsgString :
                                    LocalizationManager.ResourceManager.GetString(ErrorMsgResString);
                    if (ErrorMsgResString == STD_ERROR_MSG_RES_STRING || ErrorMsgResString == null)
                        if (AddWhatWrongStr)
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                                    string.Format(ResMsgStr, FieldName, strErrDesc);
                        else
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, Properties.Resources.resStdInputErrMsgEnd) :
                                                    string.Format(ResMsgStr, FieldName);
                    else
                        if (string.IsNullOrEmpty(FieldName))
                        if (AddWhatWrongStr)
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                                    string.Format(ResMsgStr, strErrDesc);
                        else
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, Properties.Resources.resStdInputErrMsgEnd) :
                                                    ResMsgStr;
                    else
                            if (AddWhatWrongStr)
                        Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                                string.Format(ResMsgStr, FieldName, strErrDesc);
                    else
                        Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, Properties.Resources.resStdInputErrMsgEnd) :
                                                string.Format(ResMsgStr, FieldName, ResMsgStr);
                }
                else
                {
                    ResMsgStr = LocalizationManager.ResourceManager.GetString(ErrorMsgResStringWithRowNum);
                    if (ErrorMsgResStringWithRowNum == STD_ERROR_MSG_RES_STRING_WITH_ROW_NUM)
                    {
                        if (AddWhatWrongStr)
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                                    string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, strErrDesc);
                        else
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, Properties.Resources.resStdInputErrMsgEnd) :
                                                    string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1);
                    }
                    else
                        if (string.IsNullOrEmpty(FieldName))
                        if (AddWhatWrongStr)
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, ElementCoords.Y + 1, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                                string.Format(ResMsgStr, ElementCoords.Y + 1, strErrDesc);
                        else
                            Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, ElementCoords.Y + 1, Properties.Resources.resStdInputErrMsgEnd) :
                                                string.Format(ResMsgStr, ElementCoords.Y + 1);
                    else
                            if (AddWhatWrongStr)
                        Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, strErrDesc, Properties.Resources.resStdInputErrMsgEnd) :
                                            string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, strErrDesc);
                    else
                        Msg = UseStdErrMsgEnd ? string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1, Properties.Resources.resStdInputErrMsgEnd) :
                                            string.Format(ResMsgStr, FieldName, ElementCoords.Y + 1);
                }
                HandleGotFocus = false;
                /* Здесь Owner не задаём, т.к. это будет геморрой, а без Owner должно работать нормально */
                MessageBox.Show(Msg, AppAttributes.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                m_ShowErrorMsgMs = Environment.TickCount;
            }
        }


        /// <summary>
        /// Пересчёт поля Value по Text.
        /// </summary>
        /// <param name="IsOnLostFocus">
        /// Происходит ли вызов функции при потере фокуса или нет
        /// </param>
        /// <returns></returns>
        public enCaclValueResult CalcValue(bool IsOnLostFocus = true)
        {
            enCaclValueResult result = enCaclValueResult.NoError;
            float FloatVal = 0;
            double DoubleVal = 0;
            int IntVal = 0;

            switch (InputCheckType)
            {
                case enInputCheckType.Float:
                    if (!GlobalDefines.CheckFloat(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.FloatOrEmpty:
                    if (!GlobalDefines.CheckFloatOrEmpty(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.NotNegativeFloat:
                    if (!GlobalDefines.CheckNotNegativeFloat(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.NotNegativeFloatOrEmpty:
                    if (!GlobalDefines.CheckNotNegativeFloatOrEmpty(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.PositiveFloat:
                    if (!GlobalDefines.CheckPositiveFloat(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.PositiveFloatOrEmpty:
                    if (!GlobalDefines.CheckPositiveFloatOrEmpty(Text, out FloatVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = FloatVal;
                    break;

                case enInputCheckType.Double:
                    if (!GlobalDefines.CheckDouble(Text, out DoubleVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = DoubleVal;
                    break;

                case enInputCheckType.Int:
                    if (!GlobalDefines.CheckInt(Text, out IntVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = IntVal;
                    break;

                case enInputCheckType.PositiveInt:
                    if (!GlobalDefines.CheckPositiveInt(Text, out IntVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = IntVal;
                    break;

                case enInputCheckType.NotNegativeInt:
                    if (!GlobalDefines.CheckNotNegativeInt(Text, out IntVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = IntVal;
                    break;

                case enInputCheckType.NotNegativeIntOrEmpty:
                    if (!GlobalDefines.CheckNotNegativeIntOrEmpty(Text, out IntVal))
                        result = enCaclValueResult.InputCheckError;
                    Value = IntVal;
                    break;

                case enInputCheckType.Time:
                    Value = Text;
                    break;

                case enInputCheckType.TimeZeroTimeAllowed:
                    Value = Text;
                    break;

                case enInputCheckType.TimeOrEmpty:
                    Value = Text;
                    break;

                case enInputCheckType.NotEmpty:
                    if (string.IsNullOrWhiteSpace(Text))
                        result = enCaclValueResult.InputCheckError;
                    Value = Text;
                    break;

                default:
                    Value = Text;
                    break;
            }

            if (IsOnLostFocus && AdditionalRightInputCond != null && result == enCaclValueResult.NoError && !AdditionalRightInputCond(this))
                result = enCaclValueResult.AdditionalCondError;

            return result;
        }


        /// <summary>
        /// Выполняет последовательную смену фона елемента с обычного в ErrorColor и обратно
        /// </summary>
        public void SwitchErrorColor(bool ToErrorColor)
        {
            if (ToErrorColor == m_IsErrorColor)
                return;

            if (ToErrorColor)
            {
                if (BorderThickness != m_StdBorderThickness && BorderThickness != ErrorBorderThickness)
                    m_StdBorderThickness = BorderThickness;
                if (m_StdElementColor != BorderBrush && BorderBrush != ErrorColor)
                    m_StdElementColor = BorderBrush;
                BorderThickness = ErrorBorderThickness;
                BorderBrush = ErrorColor;
            }
            else
            {
                BorderThickness = m_StdBorderThickness;
                BorderBrush = m_StdElementColor;
            }

            m_IsErrorColor = ToErrorColor;
        }


        /// <summary>
        /// Установить текущее значение свойства Text как значение, которое было, когда элементу был передан фокус
        /// </summary>
        public void SetCurTextAsOnGotFocus()
        {
            m_TextOnGotFocus = Text;
        }


        /// <summary>
        /// Проверить введённые данные на "правильность"
        /// </summary>
        public void RecalcRightInput()
        {
            InputCheckResult = CalcValue(false);
            if (!IsRightInput)
                OnErrorInput(ShowErrMsgOnAddCondErr || InputCheckResult != enCaclValueResult.AdditionalCondError);
        }
    }
}
