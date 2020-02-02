using DBManager.DAL;
using DBManager.SettingsWriter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using MSExcel = Microsoft.Office.Interop.Excel;

namespace DBManager.Global
{
    public static class GlobalDefines
    {
        #region Поля

        public static readonly string STD_APP_CONFIGS_DIR = "";
        public static readonly string STD_ERROR_LOG_FILE_PATH = "";
        public static readonly string STD_PUBLISHING_LOG_DIR = "";

        /// <summary>
        /// Аргумент для функции FrameworkElement.Measure()
        /// </summary>
        public static readonly Size STD_SIZE_FOR_MEASURE = new Size(double.PositiveInfinity, double.PositiveInfinity);

        public const string RUSSIAN_CULTURE_NAME = "ru-RU";

        public const double MIN_FLOAT_VAL = 1e-8;

        public const string STD_FLOAT_FORMAT = "F2";

        /// <summary>
        /// Если CDirScanner.CompId имеет это значение, то в БД ещё не добавлены соревы
        /// </summary>
        public const Int64 NO_OUR_COMP_IN_DB = -1;

        public const int DEFAULT_XML_INT_VAL = -1;
        public const byte DEFAULT_XML_BYTE_VAL = unchecked((byte)(-1));
        public const string DEFAULT_XML_STRING_VAL = null;
        public const double DEFAULT_XML_DOUBLE_VAL = 0;
        public static readonly DateTime DEFAULT_XML_DATE_TIME_VAL = DateTime.MinValue;

        public static readonly TimeSpan FALL_TIME_SPAN_VAL = new TimeSpan(20, 00, 0);
        public static readonly TimeSpan DISQUALIF_TIME_SPAN_VAL = new TimeSpan(30, 00, 0);
        public static readonly TimeSpan DONT_APPEAR_TIME_SPAN_VAL = new TimeSpan(40, 00, 0);
        /// <summary>
        /// Время суммы будет больше этого времени, если участник сорвался на второй трассе
        /// </summary>
        public static readonly TimeSpan FALL_ON_ROUTE_2_TIME_SPAN_VAL = new TimeSpan(01, 00, 0);

        public static readonly short MIN_YEAR_OF_BIRTH = 0;
        public static readonly int MIN_GROUP_YEAR = 0;
        public static readonly int MAX_GROUP_YEAR = 0;

        /// <summary>
        /// 1 минута в Excel
        /// </summary>
        public const double XLS_MIN = 6.94444444444444e-4;
        /// <summary>
        /// 1 секунда в Excel
        /// </summary>
        public const double XLS_SEC = 1.15740740740741e-5;
        /// <summary>
        /// 1 миллисекунда в Excel
        /// </summary>
        public const double XLS_MSEC = 1.15740740740741e-7;


        public const string QUALIF_NODE_NAME = "Квалификация_1";
        public const string QUALIF2_NODE_NAME = "Квалификация_2";
        public const string ONE_EIGHTH_FINAL_NODE_NAME = "Одна_восьмая_финала";
        public const string QUATER_FINAL_NODE_NAME = "Четвертьфинал";
        public const string SEMI_FINAL_NODE_NAME = "Полуфинал";
        public const string FINAL_NODE_NAME = "Финал";
        public const string TOTAL_NODE_NAME = "Итоговый_протокол";

        public const byte LAST_RESULT_PLACE_TO_CALC_BALLS = 30;

        public const char BULLET_CHAR_IN_MSG_BOX = (char)0x25CF;

        /// <summary>
        /// Идентификаторы раудов
        /// </summary>
        public static readonly Dictionary<string, byte> ROUND_IDS = new Dictionary<string, byte>();

        /// <summary>
        /// Названия раудов
        /// </summary>
        public static readonly Dictionary<byte, string> ROUND_NAMES = new Dictionary<byte, string>();

        /// <summary>
        /// Баллы за места
        /// </summary>
        public static readonly Dictionary<int, byte> BALLS_FOR_PLACES = new Dictionary<int, byte>();

        /// <summary>
        /// Все названия разрядов
        /// </summary>
        public static Dictionary<enGrade, string> GRADE_NAMES = new Dictionary<enGrade, string>();

        /// <summary>
        /// Все названия разрядов
        /// </summary>
        public static Dictionary<enAdditionalEventTypes, additional_events_types> ADDITIONAL_EVENT_NAMES = new Dictionary<enAdditionalEventTypes, additional_events_types>();

        /// <summary>
        /// Номера строк для перевода участников из одного раунда в другой.
        /// Ключ - целевой номер раунда.
        /// Значение - массив номеров
        /// </summary>
        public static readonly Dictionary<enRounds, byte[]> ROW_SEQUENCE = new Dictionary<enRounds, byte[]>();


        public const string LOG_EXCEPTION_TERMINAL_LINE = "...";
        public const string LOG_EXCEPTION_START_LINE = "Exception №";

        public const string XLSX_EXTENSION = ".xlsx";
        public const string XLS_EXTENSION = ".xls";
        public const string MAIN_WBK_EXTENSION = ".xlsm";
        public static string EXCEL_DEFAULT_SHEET_SIGNATURE = "ТебеНетДелаДоДругих";
        public static string EXCEL_REPORT_NAME_TEXT_TO_REPLACE = "<Название группы>";

        public const char PUBLISHING_LOG_FIELDS_SEPARATOR = ';';
        public const char PUBLISHING_LOG_LFCR_SYMBOL = '#';
        public const string PUBLISHING_LOG_FILE_EXTENSION = ".publog";
        public const int PUBLISHING_LOG_FILE_SIZE = 10 * 1024 * 1024;

        public static string[] MONTHS_IN_GENITIVE = {
                                                        "Января",
                                                        "Февраля",
                                                        "Марта",
                                                        "Апреля",
                                                        "Мая",
                                                        "Июня",
                                                        "Июля",
                                                        "Августа",
                                                        "Сентября",
                                                        "Октября",
                                                        "Ноября",
                                                        "Декабря"
                                                    };



        public static Stopwatch m_swchGlobal = new Stopwatch();
        #endregion


        static GlobalDefines()
        {
            STD_APP_CONFIGS_DIR = Environment.CurrentDirectory;
            STD_ERROR_LOG_FILE_PATH = STD_APP_CONFIGS_DIR + "\\ErrLog.txt";
            STD_PUBLISHING_LOG_DIR = STD_APP_CONFIGS_DIR + "\\PublishingLog\\";

            MIN_YEAR_OF_BIRTH = (short)(DateTime.Today.Year - 100);
            MIN_GROUP_YEAR = DateTime.Today.Year - 100;
            MAX_GROUP_YEAR = DateTime.Today.Year - 4;

            RefreshVariables();
        }


        public static void RefreshVariables()
        {
            if (DBManagerApp.m_Entities != null && DBManagerApp.m_Entities.rounds != null)
            {
                ROUND_IDS.Clear();
                ROUND_NAMES.Clear();
                GRADE_NAMES.Clear();
                BALLS_FOR_PLACES.Clear();
                ADDITIONAL_EVENT_NAMES.Clear();

                foreach (rounds round in DBManagerApp.m_Entities.rounds)
                {
                    ROUND_IDS.Add(round.name, round.id_round);
                    ROUND_NAMES.Add(round.id_round, round.name);
                }
                ROUND_IDS.Add(TOTAL_NODE_NAME, (byte)enRounds.Total);
                ROUND_NAMES.Add((byte)enRounds.Total, TOTAL_NODE_NAME);

                foreach (balls ball in DBManagerApp.m_Entities.balls)
                    BALLS_FOR_PLACES.Add(ball.place, ball.value);

                // Заполняем словарь с названиями разрядов
                foreach (grades result in DBManagerApp.m_Entities.grades)
                    GRADE_NAMES.Add((enGrade)result.id_grade, result.name);

                GRADE_NAMES.Add(enGrade.None, GRADE_NAMES[enGrade.WithoutGrade]);

                // Заполняем словарь с названиями разрядов
                foreach (additional_events_types type in DBManagerApp.m_Entities.additional_events_types)
                    ADDITIONAL_EVENT_NAMES.Add((enAdditionalEventTypes)type.flag, type);
            }

            ROW_SEQUENCE.Clear();
            ROW_SEQUENCE.Add(enRounds.OneEighthFinal, new byte[] { 1, 16, 8, 9, 4, 13, 5, 12, 2, 15, 7, 10, 3, 14, 6, 11 });
            ROW_SEQUENCE.Add(enRounds.QuaterFinal, new byte[] { 1, 8, 4, 5, 2, 7, 3, 6 });
            ROW_SEQUENCE.Add(enRounds.SemiFinal, new byte[] { 1, 4, 2, 3 });
            ROW_SEQUENCE.Add(enRounds.Final, new byte[] { 3, 4, 1, 2 });
        }


        #region Функции
        #region Сравнение чисел double и float с нулём и проверка на равенство
        /// <summary>
        /// Равно ли число float нулю с заданной точностью?
        /// </summary>
        /// <param name="val"></param>
        /// <param name="precision">
        /// точность в виде 1e-[степень], что то же самое, что 10^(-[степень])
        /// </param>
        public static bool IsFloatZero(float val, double precision = MIN_FLOAT_VAL)
        {
            return val == 0.0F || Math.Abs(val) < precision;
        }

        /// <summary>
        /// Равны ли два числа float с заданной точностью?
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="precision">
        /// точность в виде 1e-[степень], что то же самое, что 10^(-[степень])
        /// </param>
        public static bool IsFloatsEqu(float val1, float val2, double precision = MIN_FLOAT_VAL)
        {
            if (val1 == val2)
                return true;

            return Math.Abs(val1 - val2) < precision;
        }


        /// <summary>
        /// Равно ли число double нулю с заданной точностью?
        /// </summary>
        /// <param name="val"></param>
        /// <param name="precision">
        /// точность в виде 1e-[степень], что то же самое, что 10^(-[степень])
        /// </param>
        public static bool IsDoubleZero(double val, double precision = MIN_FLOAT_VAL)
        {
            return val == 0.0 || Math.Abs(val) < precision;
        }

        /// <summary>
        /// Равны ли два числа double с заданной точностью?
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="precision">
        /// точность в виде 1e-[степень], что то же самое, что 10^(-[степень])
        /// </param>
        public static bool IsDoublesEqu(double val1, double val2, double precision = MIN_FLOAT_VAL)
        {
            if (val1 == val2)
                return true;

            if (Math.Abs(val1) < precision && Math.Abs(val2) < precision)
                return Math.Abs(val1 - val2) < precision;
            else
                return Math.Abs(val1 - val2) < Math.Abs(val1) * precision;
        }
        #endregion


        /// <summary>
        /// Получение строки формата для числа float при заданной точности отображения 
        /// </summary>
        /// <param name="Precision"></param>
        /// <returns></returns>
        public static string GetFloatFmtForPrecision(int Precision)
        {
            return Precision < 0 ? "" : string.Format("F{0}", Precision);
        }


        /// <summary>
        /// Возвращает значение вида 9...9.9...9 по максимальному значению (10...0) и точности 
        /// </summary>
        /// <param name="MaxVal"></param>
        /// <param name="Precision"></param>
        /// <returns></returns>
        static public string GetMaxValForPrec(int MaxVal, int Precision)
        {
            string result = (MaxVal - 1).ToString() + Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator;
            return result.PadRight(result.Length + Precision, '9');
        }


        #region Функции для подгона ширины ComboBox по содержимому
        /// <summary>
        /// Подогнать ширину ComboBox по содержимому.
        /// Функция считает, что в ComboBox засунуты элементы типа Control
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="UseCmbWidth"></param>
        public static void TuneComboboxWidth(ComboBox cmb, bool UseCmbWidth = false)
        {
            double width = 0;
            foreach (Control item in cmb.Items)
            {
                item.Measure(STD_SIZE_FOR_MEASURE);
                if (item.DesiredSize.Width > width)
                    width = item.DesiredSize.Width;
            }
            cmb.Measure(STD_SIZE_FOR_MEASURE);

            if (width == 0)
                cmb.Width = cmb.DesiredSize.Width;
            else
                cmb.Width = UseCmbWidth ? Math.Max(cmb.Width, width + 20) : width + 20;
        }


        /// <summary>
        /// Подогнать ширину ComboBox по содержимому.
        /// Функция считает, что в ComboBox засунуты элементы типа ComboBoxItem
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="UseCmbWidth"></param>
        public static void TuneComboboxWidth2(ComboBox cmb, bool UseCmbWidth = false)
        {
            double width = 0;
            foreach (ComboBoxItem item in cmb.Items)
            {
                item.Measure(STD_SIZE_FOR_MEASURE);
                if (item.DesiredSize.Width > width)
                    width = item.DesiredSize.Width;
            }
            cmb.Measure(STD_SIZE_FOR_MEASURE);

            if (width == 0)
                width = cmb.Width = cmb.DesiredSize.Width;
            else
                width = cmb.Width = (UseCmbWidth ? Math.Max(cmb.Width, width + 20) : width + 20);

            foreach (ComboBoxItem item in cmb.Items)
                item.Width = width - 5;
        }


        /// <summary>
        /// Подогнать ширину ComboBox по содержимому.
        /// Функция считает, что в ComboBox засунуты элементы типа ComboBoxItem, у которых свойство Content имеет тип string
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="UseCmbWidth"></param>
        public static void TuneComboboxWidth3(ComboBox cmb, bool UseCmbWidth = false)
        {
            double width = 0;
            foreach (ComboBoxItem item in cmb.Items)
            {
                FormattedText ft = new FormattedText((string)item.Content,
                                                     CultureInfo.CurrentCulture,
                                                     FlowDirection.LeftToRight,
                                                     new Typeface(cmb.FontFamily, cmb.FontStyle, cmb.FontWeight, cmb.FontStretch),
                                                     cmb.FontSize,
                                                     Brushes.Black);
                if (ft.Width + 25 > width)
                    width = ft.Width + 25;
            }

            cmb.Measure(STD_SIZE_FOR_MEASURE);

            if (width == 0)
                width = cmb.Width = cmb.DesiredSize.Width;
            else
                width = cmb.Width = (UseCmbWidth ? Math.Max(cmb.Width, width) : width);
        }


        /// <summary>
        /// Подогнать ширину ComboBox по содержимому.
        /// Функция считает, что в ComboBox засунуты элементы, которые можно преобразовать к string с помощью ToString()
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="UseCmbWidth"></param>
        public static void TuneComboboxWidth4(ComboBox cmb, bool UseCmbWidth = false)
        {
            double width = 0;
            foreach (object item in cmb.Items)
            {
                FormattedText ft = new FormattedText(item.ToString(),
                                                     CultureInfo.CurrentCulture,
                                                     FlowDirection.LeftToRight,
                                                     new Typeface(cmb.FontFamily, cmb.FontStyle, cmb.FontWeight, cmb.FontStretch),
                                                     cmb.FontSize,
                                                     Brushes.Black);
                if (ft.Width + 25 > width)
                    width = ft.Width + 25;
            }

            cmb.Measure(STD_SIZE_FOR_MEASURE);

            if (width == 0)
                width = cmb.Width = cmb.DesiredSize.Width;
            else
                width = cmb.Width = (UseCmbWidth ? Math.Max(cmb.Width, width) : width);
        }

        /// <summary>
        /// Подогнать ширину ComboBox по содержимому.
        /// Функция считает, что в ComboBox засунуты какие-то элементы. Свойство, которое отображается на экране задано в cmb.DisplayMemberPath.
        /// Если значение этого свойства получить не удалось, то используется метод ToString()
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="UseCmbWidth"></param>
        public static void TuneComboboxWidth5(ComboBox cmb, bool UseCmbWidth = false)
        {
            double width = 0;
            PropertyInfo pi = null;
            foreach (object item in cmb.Items)
            {
                if (width == 0)
                    pi = item.GetType().GetProperty(cmb.DisplayMemberPath);

                FormattedText ft = new FormattedText(pi == null ? item.ToString() : pi.GetValue(item, null).ToString(),
                                                     CultureInfo.CurrentCulture,
                                                     FlowDirection.LeftToRight,
                                                     new Typeface(cmb.FontFamily, cmb.FontStyle, cmb.FontWeight, cmb.FontStretch),
                                                     cmb.FontSize,
                                                     Brushes.Black);
                if (ft.Width + 25 > width)
                    width = ft.Width + 25;
            }

            cmb.Measure(STD_SIZE_FOR_MEASURE);

            if (width == 0)
                width = cmb.Width = cmb.DesiredSize.Width;
            else
                width = cmb.Width = (UseCmbWidth ? Math.Max(cmb.Width, width) : width);
        }
        #endregion


        /// <summary>
        /// Доавляет данные в ComboBox. Добавляется ComboBoxItem, у которого Content = Value, а Tag = Key
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="Value"></param>
        /// <param name="Key"></param>
        public static ComboBoxItem AddItemToCmb<T>(ComboBox cmb, string Value, T Key)
        {
            ComboBoxItem result = new ComboBoxItem()
            {
                Content = Value,
                Tag = Key
            };
            cmb.Items.Add(result);

            return result;
        }


        #region Получение размеров компонента без учёта Margin'ов
        /// <summary>
        /// Получает ширину компонента без учёта Margin'ов
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static double GetActualControlWidth(FrameworkElement ctrl)
        {
            if (ctrl.IsLoaded && ctrl.ActualWidth != 0)
                return ctrl.ActualWidth - ctrl.Margin.Left - ctrl.Margin.Right;
            else
            {
                ctrl.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                return ctrl.DesiredSize.Width - ctrl.Margin.Left - ctrl.Margin.Right;
            }
        }


        /// <summary>
        /// Получает высоту компонента без учёта Margin'ов
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static double GetActualControlHeight(FrameworkElement ctrl)
        {
            if (ctrl.IsLoaded && ctrl.ActualHeight != 0)
                return ctrl.ActualHeight - ctrl.Margin.Bottom - ctrl.Margin.Top;
            else
            {
                ctrl.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                return ctrl.DesiredSize.Height - ctrl.Margin.Bottom - ctrl.Margin.Top;
            }
        }


        /// <summary>
        /// Получает ширину компонента без учёта Margin'ов
        /// </summary>
        /// <param name="ctrl">
        /// ContentControl, у которого в свойстве Content находится строка
        /// </param>
        /// <returns></returns>
        public static double GetActualContentControlWidth(ContentControl ctrl)
        {
            FormattedText ft = new FormattedText(ctrl.Content.ToString(),
                                                CultureInfo.CurrentCulture,
                                                FlowDirection.LeftToRight,
                                                new Typeface(ctrl.FontFamily,
                                                            ctrl.FontStyle,
                                                            ctrl.FontWeight,
                                                            ctrl.FontStretch),
                                                ctrl.FontSize,
                                                Brushes.Black);
            return ft.Width + ctrl.Padding.Left + ctrl.Padding.Right;
        }
        #endregion


        /// <summary>
        /// Получает ширину компонента c учётом Margin'ов
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="ConsiderLeftMargin">
        /// Учитывать ctrl.Margin.Left
        /// </param>
        /// <param name="ConsiderRightMargin">
        /// Учитывать ctrl.Margin.Right
        /// </param>
        /// <returns></returns>
        public static double GetWholeControlWidth(FrameworkElement ctrl, bool ConsiderLeftMargin = true, bool ConsiderRightMargin = true)
        {
            double result = double.IsNaN(ctrl.Width) ? GetActualControlWidth(ctrl) : ctrl.Width;

            if (ConsiderLeftMargin)
                result += ctrl.Margin.Left;

            if (ConsiderRightMargin)
                result += ctrl.Margin.Right;

            return result;
        }


        /// <summary>
        /// Получение оптимальной ширины TextBox'a, чтобы в него влезло число, у которого целая часть может состоять не более чем из MaxIntDigits цифр, а дробная - MaxFracDigits
        /// </summary>
        /// <param name="txt">
        /// Если null, то берём стандартные значения Padding для TextBox
        /// </param>
        /// <param name="MaxIntDigits"></param>
        /// <param name="MaxFracDigits"></param>
        /// <returns></returns>
        public static double GetTextBoxOptimalWidth(TextBox txt, int MaxIntDigits, int MaxFracDigits)
        {
            double PaddingLeft, PaddingRight;
            if (txt == null)
                PaddingLeft = PaddingRight = 1;
            else
            {
                PaddingLeft = txt.Padding.Left;
                PaddingRight = txt.Padding.Right;
            }

            // Эта формула подобрана опытным путём для стандартного шрифта, используемого в TextBox'ах
            return 7 + (MaxFracDigits + MaxIntDigits) * 7 + 1 + PaddingLeft + PaddingRight + 3;
        }


        /// <summary>
        /// Делает ширину элементов управления в столбце одинаковой и равной ширине максимального элемента
        /// Но делает он это очень хитро!!!
        /// </summary>
        /// <param name="col"></param>
        public static void FitElsWidthToGridCol(Grid grd, int col)
        {
            double MaxWidth = 0;
            FrameworkElement MaxWidthEl = null;
            IEnumerable<FrameworkElement> ColumnsElements = grd.Children.Cast<FrameworkElement>().Where(elem => Grid.GetColumn(elem) == col && Grid.GetColumnSpan(elem) == 1);
            foreach (FrameworkElement item in ColumnsElements.Where(ctrl => ctrl.Visibility != Visibility.Collapsed))
            {
                item.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                if (Math.Max(Math.Min(item.DesiredSize.Width, item.MaxWidth), item.MinWidth) > MaxWidth)
                {
                    MaxWidth = item.DesiredSize.Width;
                    MaxWidthEl = item;
                }
            }

            foreach (FrameworkElement item in ColumnsElements.Where(ctrl => ctrl.Visibility != Visibility.Collapsed))
                if (item != MaxWidthEl)
                {
                    item.Width = double.NaN;
                    item.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
        }


        #region MakeButtonsSameWidth
        /// <summary>
        /// Делает ширину кнопок, находящихся в массиве Buttons одинаковой.
        /// Эта функция должна быть вызвана не раньше, чем произойдёт событие Loaded, иначе функция сожмёт кнопки до нуля!!!
        /// Эта функция должна быть использована во всех формах, где есть кнопки "Отмена", "OK", "Применить",
        /// т.к. для разных языков названия этих кнопок могут сильно отличаться по длине.
        /// Функция учитывает только видимые кнопки
        /// </summary>
        /// <param name="Buttons"></param>
        public static void MakeButtonsSameWidth(params Button[] Buttons)
        {
            double MaxBtnWidth = 0;
            if (Buttons != null && Buttons.Length > 0)
            {
                foreach (Button btn in Buttons)
                    if (btn.ActualWidth > MaxBtnWidth)
                        MaxBtnWidth = btn.ActualWidth;

                foreach (Button btn in Buttons)
                    if (btn.Visibility == Visibility.Visible)
                        btn.Width = MaxBtnWidth;
            }
        }
        #endregion


        #region Методы для проверки введённых значений
        /// <summary>
        /// Проверка на ввод действительных чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// </param>
        public static bool CheckFloat(string Text, out float val)
        {
            val = 0;

            if (Text.Contains(' '))
                return false;
            else
                return float.TryParse(Text.Replace('.', ','), out val) || float.TryParse(Text.Replace(',', '.'), out val);
        }


        /// <summary>
        /// Проверка на ввод действительных чисел или пустая строка
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// Если введена пустая строка, то 0.
        /// </param>
        public static bool CheckFloatOrEmpty(string Text, out float val)
        {
            val = 0;

            if (string.IsNullOrEmpty(Text))
                return true;

            if (Text.Contains(' '))
                return false;
            else
                return float.TryParse(Text.Replace('.', ','), out val) || float.TryParse(Text.Replace(',', '.'), out val);
        }


        /// <summary>
        /// Проверка на ввод положительных действительных чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// </param>
        public static bool CheckPositiveFloat(string Text, out float val)
        {
            bool result = true;

            val = 0;

            if (Text.Contains(' '))
                return false;
            else
                if (!float.TryParse(Text.Replace('.', ','), out val) && !float.TryParse(Text.Replace(',', '.'), out val))
                return false;
            else
                return result && !float.IsNaN(val) && !float.IsInfinity(val) && Math.Sign(val) > 0;
        }


        /// <summary>
        /// Проверка на ввод не отрицательных действительных чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// </param>
        public static bool CheckNotNegativeFloat(string Text, out float val)
        {
            bool result = true;

            val = 0;

            if (Text.Contains(' '))
                return false;
            else
                if (!float.TryParse(Text.Replace('.', ','), out val) && !float.TryParse(Text.Replace(',', '.'), out val))
                return false;
            else
                return result && !float.IsNaN(val) && !float.IsInfinity(val) && Math.Sign(val) >= 0;
        }


        /// <summary>
        /// Проверка на ввод не отрицательных действительных чисел или пустая строка
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// Если введена пустая строка, то 0.
        /// </param>
        public static bool CheckNotNegativeFloatOrEmpty(string Text, out float val)
        {
            bool result = true;

            val = 0;

            if (string.IsNullOrEmpty(Text))
                return true;

            if (Text.Contains(' '))
                return false;
            else
                if (!float.TryParse(Text.Replace('.', ','), out val) && !float.TryParse(Text.Replace(',', '.'), out val))
                return false;
            else
                return result && !float.IsNaN(val) && !float.IsInfinity(val) && Math.Sign(val) >= 0;
        }


        /// <summary>
        /// Проверка на ввод положительных действительных чисел или пустая строка
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// Если введена пустая строка, то 0.
        /// </param>
        public static bool CheckPositiveFloatOrEmpty(string Text, out float val)
        {
            bool result = true;

            val = 0;

            if (string.IsNullOrEmpty(Text))
                return true;

            if (Text.Contains(' '))
                return false;
            else
                if (!float.TryParse(Text.Replace('.', ','), out val) && float.TryParse(Text.Replace(',', '.'), out val))
                return false;
            else
                return result && !float.IsNaN(val) && !float.IsInfinity(val) && Math.Sign(val) > 0;
        }


        /// <summary>
        /// Проверка на ввод действительных чисел типа double
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено.
        /// </param>
        public static bool CheckDouble(string Text, out double val)
        {
            val = 0;

            if (Text.Contains(' '))
                return false;
            else
                return double.TryParse(Text.Replace('.', ','), out val) || double.TryParse(Text.Replace(',', '.'), out val);
        }


        /// <summary>
        /// Проверка на ввод целых чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено 
        /// </param>
        public static bool CheckInt(string Text, out int val)
        {
            val = 0;
            return !Text.Contains(' ') && int.TryParse(Text, out val);
        }


        /// <summary>
        /// Проверка на ввод положительных целых чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено 
        /// </param>
        public static bool CheckPositiveInt(string Text, out int val)
        {
            val = 0;
            return !Text.Contains(' ') && int.TryParse(Text, out val) && val > 0;
        }


        /// <summary>
        /// Проверка на ввод не отрицательных целых чисел
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено 
        /// </param>
        public static bool CheckNotNegativeInt(string Text, out int val)
        {
            val = 0;
            return !Text.Contains(' ') && int.TryParse(Text, out val) && val >= 0;
        }


        /// <summary>
        /// Проверка на ввод не отрицательных целых чисел или пустая строка
        /// </summary>
        /// <returns></returns>
        /// <param name="Text"></param>
        /// <param name="val">
        /// Если введённое значение правильное, то этот параметр его содержит,
        /// в противном случае значение не определено 
        /// </param>
        public static bool CheckNotNegativeIntOrEmpty(string Text, out int val)
        {
            val = 0;
            return string.IsNullOrEmpty(Text) || (!Text.Contains(' ') && int.TryParse(Text, out val) && val >= 0);
        }
        #endregion


        /// <summary>
        /// Выполнить сборку мусора и при этом очистить память так, что бы это отобразилось в диспетчере устройств
        /// </summary>
        static public void ForceGCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// Замена Application.DoEvents().
        /// Код взят отсюда: http://social.msdn.microsoft.com/Forums/ru-RU/79598b96-1d41-4cbd-8c62-80b12af7a17b/-applicationdoevents-wpf?forum=fordesktopru
        /// </summary>
        public static void DoEvents(Window wnd)
        {
            if (wnd == null)
                return;

            // Create new nested message pump.
            DispatcherFrame nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than that of UI event messages.
            DispatcherOperation exitOperation = wnd.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), nestedFrame);

            // pump the nested message loop, the nested message loop will immediately
            // process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback is not finished, abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
                exitOperation.Abort();
        }

        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // Exit the nested message loop.
            frame.Continue = false;
            return null;
        }


        public static ScrollViewer GetScrollViewer(FrameworkElement ctrl)
        {
            if ((ctrl is ListBox || ctrl is TreeView) && VisualTreeHelper.GetChildrenCount(ctrl) > 0)
            {
                DependencyObject border = VisualTreeHelper.GetChild(ctrl, 0);
                if (border != null && VisualTreeHelper.GetChildrenCount(border) > 0)
                    return VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            }

            return null;
        }


        public static double GetVerticalScrollBarWidth(ScrollViewer viewer)
        {
            ScrollBar sbar = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(viewer, 0) as Grid, 2) as ScrollBar;
            return sbar.IsVisible ? sbar.Width : 0;
        }


        public static double GetHorizontalScrollBarHeight(ScrollViewer viewer)
        {
            ScrollBar sbar = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(viewer, 0) as Grid, 1) as ScrollBar;
            return sbar.IsVisible ? sbar.Height : 0;
        }


        /// <summary>
        /// Функция масштабирует <paramref name="tbctrl"/> таким  образом, чтобы он имел
        /// высоту и ширину, равные высоте и ширине одной из вкладок.
        /// Для ширины и высоты могут быть заданы разные вкладки.
        /// </summary>
        /// <param name="tbctrl">
        /// Набор вкладок. Вкладки должны содержать в свойстве Content элемент типа CSettingsTabBase.
        /// </param>
        /// <param name="WidthPatternTab">
        /// Вкладка, по которой устанавливается ширина всех остальных. Если null, то выбирается ширины максимально широкой вкладки
        /// </param>
        /// <param name="HeightPatternTab">
        /// Вкладка, по которой устанавливается высота всех остальных. Если null, то выбирается высота максимально высокой вкладки
        /// </param>
        public static void AutoscaleTabs(TabControl tbctrl, FrameworkElement WidthPatternTab, FrameworkElement HeightPatternTab)
        {
            double MaxWidth = 0, MaxHeight = 0;

            if (WidthPatternTab == null && HeightPatternTab == null)
            {
                for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
                {
                    if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
                    {
                        ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                        if (MaxWidth < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width)
                            MaxWidth = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width;
                        if (MaxHeight < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height)
                            MaxHeight = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height;
                    }
                }
            }
            else
                if (WidthPatternTab == null)
            {   /* HeightPatternTab != null => определяем ширину как максимум из всех, а высоту - высоту вкладки HeightPatternTab */
                HeightPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                MaxHeight = HeightPatternTab.DesiredSize.Height;
                for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
                {
                    if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
                    {
                        ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                        if (MaxWidth < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width)
                            MaxWidth = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Width;
                    }
                }
            }
            else
                    if (HeightPatternTab == null)
            {   /* WidthPatternTab != null => определяем высоту как максимум из всех, а ширину - ширину вкладки HeightPatternTab */
                WidthPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                MaxWidth = WidthPatternTab.DesiredSize.Height;
                for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
                {
                    if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
                    {
                        ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                        if (MaxHeight < ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height)
                            MaxHeight = ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).DesiredSize.Height;
                    }
                }
            }
            else
            {   /* (WidthPatternTab != null && HeightPatternTab != null) */
                HeightPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                MaxHeight = HeightPatternTab.DesiredSize.Height;
                if (HeightPatternTab != WidthPatternTab)
                    WidthPatternTab.Measure(GlobalDefines.STD_SIZE_FOR_MEASURE);
                MaxWidth = WidthPatternTab.DesiredSize.Width;
            }

            if (!((MaxWidth < double.MinValue) || (MaxHeight < double.MinValue)))// если измерения были произведены
                for (int tabIndex = 0; tabIndex < tbctrl.Items.Count; tabIndex++)
                {
                    if ((tbctrl.Items[tabIndex] as TabItem).Content != null)
                    {
                        ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Width = MaxWidth;
                        ((tbctrl.Items[tabIndex] as TabItem).Content as FrameworkElement).Height = MaxHeight;
                    }
                }
        }


        public static System.Windows.Media.Imaging.BitmapSource GetMsgBoxSystemIcon(MessageBoxImage icon)
        {
            IntPtr IconHandle = IntPtr.Zero;

            switch (icon)
            {
                case MessageBoxImage.Error:
                    IconHandle = System.Drawing.SystemIcons.Error.Handle;
                    break;

                case MessageBoxImage.Question:
                    IconHandle = System.Drawing.SystemIcons.Question.Handle;
                    break;

                case MessageBoxImage.Exclamation:
                    IconHandle = System.Drawing.SystemIcons.Exclamation.Handle;
                    break;

                default:
                    IconHandle = System.Drawing.SystemIcons.Information.Handle;
                    break;
            }

            return Imaging.CreateBitmapSourceFromHIcon(IconHandle, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }


        public static void SetFixedWndSizeByContent(Window wnd)
        {
            wnd.Measure(STD_SIZE_FOR_MEASURE);
            wnd.Width = wnd.DesiredSize.Width;
            wnd.Height = wnd.DesiredSize.Height;

            switch (wnd.WindowStartupLocation)
            {
                case WindowStartupLocation.CenterOwner:
                    wnd.Left = (wnd.Owner.Left + wnd.Owner.Width / 2) - wnd.Width / 2;
                    wnd.Top = (wnd.Owner.Top + wnd.Owner.Height / 2) - wnd.Height / 2;
                    break;

                case WindowStartupLocation.CenterScreen:
                    wnd.Left = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * DBManagerApp.m_DPIScale.X) / 2 - wnd.Width / 2;
                    wnd.Top = (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * DBManagerApp.m_DPIScale.Y) / 2 - wnd.Height / 2;
                    break;
            }
            wnd.SizeToContent = SizeToContent.Manual;
        }


        /// <summary>
        /// Поиск родительского элемента заданного типа 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        public static T GetNeedParentOfType<T>(FrameworkElement ctrl) where T : FrameworkElement
        {
            FrameworkElement result = ctrl;
            while (result != null && !(result is T))
            {
                result = result.Parent as FrameworkElement;
            }

            return result as T;
        }


        public static enSex SexToEnum(byte SexInInt)
        {
            switch (SexInInt)
            {
                case 0:
                    return enSex.Male;

                case 1:
                    return enSex.Female;

                case GlobalDefines.DEFAULT_XML_BYTE_VAL:
                default:
                    return enSex.None;
            }
        }


        public static TimeSpan? RouteResultToTime(string ResInStr, out enAdditionalEventTypes AdditionalEventType)
        {
            AdditionalEventType = enAdditionalEventTypes.None;
            if (string.IsNullOrWhiteSpace(ResInStr))
                return null;

            if (ResInStr == "срыв")
            {
                return FALL_TIME_SPAN_VAL;
            }
            else if (ResInStr == ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.DontAppear].name_in_xml)
            {
                AdditionalEventType = enAdditionalEventTypes.DontAppear;
                return DONT_APPEAR_TIME_SPAN_VAL;
            }
            else if (ResInStr == ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.Disqualif].name_in_xml)
            {
                AdditionalEventType = enAdditionalEventTypes.Disqualif;
                return DISQUALIF_TIME_SPAN_VAL;
            }

            double ResInDbl;
            if (!double.TryParse(ResInStr, out ResInDbl))
            {
                // попробуем заменить точки на запятые
                if (!double.TryParse(ResInStr.Replace('.', ','), out ResInDbl))
                    return null;
            }

            return new TimeSpan(0,
                                0,
                                (int)Math.Floor(ResInDbl / XLS_MIN),
                                (int)Math.Floor(Math.Round(ResInDbl / XLS_SEC, 2)) % 60,
                                ((int)Math.Round(ResInDbl / XLS_MSEC, 0) % 100));
        }


        /// <summary>
        /// Добавляет по одному пробелу после каждого вхождения символа <paramref name="ch"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string InsertSpacesAfterChar(string str, char ch)
        {
            int CharInd = str.IndexOf(ch) + 1; // Индек следующего за ch символа
            while (CharInd > 0 && CharInd < str.Length)
            {
                if (str[CharInd] != ' ')
                    str = str.Insert(CharInd, " ");
                CharInd = str.IndexOf(ch, CharInd) + 1;
            }

            return str;
        }


        public static string CreateSurnameAndName(string Surname, string Name)
        {
            return Surname + " " + Name;
        }


        /// <summary>
        /// Возвращает идентификатор существующего тренера или создаёт нового, сохраняет его в БД и возвращает его идентификатор
        /// </summary>
        /// <param name="CoachName"></param>
        /// <param name="CreateNew">
        /// true - если тренера <paramref name="CoachName"/> в БД нет, то он будет создан, добавлен в БД и функция возвратит его идентификатор
        /// false - если тренера <paramref name="CoachName"/> в БД нет, то функция возвратит null
        /// </param>
        /// <returns></returns>
        public static long? GetCoachId(string CoachName, bool CreateNew)
        {
            // Ищем тренера в таблице
            IEnumerable<coaches> CoachIdInDB = DBManagerApp.m_Entities.coaches.Where(arg => arg.name == CoachName);

            if (CoachIdInDB.Count() > 0)
                return CoachIdInDB.First().id_coach;
            else
            {	// Добавляем тренера в таблицу
                if (CreateNew)
                {
                    coaches NewCoach = new coaches()
                    {
                        name = CoachName
                    };
                    DBManagerApp.m_Entities.coaches.Add(NewCoach);
                    DBManagerApp.m_Entities.SaveChanges(); // Получаем id_coach
                    return NewCoach.id_coach;
                }
                else
                    return null;
            }
        }


        /// <summary>
        /// Возвращает идентификатор существующей команды или создаёт новую, сохраняет её в БД и возвращает её идентификатор
        /// </summary>
        /// <param name="TeamName"></param>
        /// <param name="CreateNew">
        /// true - если команды <paramref name="TeamName"/> в БД нет, то она будет создана, добавлена в БД и функция возвратит её идентификатор
        /// false - если команды <paramref name="TeamName"/> в БД нет, то функция возвратит null
        /// </param>
        /// <returns></returns>
        public static long? GetTeamId(string TeamName, bool CreateNew)
        {
            // Ищем тренера в таблице
            IEnumerable<teams> TeamIdInDB = DBManagerApp.m_Entities.teams.Where(arg => arg.name == TeamName);
            if (TeamIdInDB.Count() > 0)
                return TeamIdInDB.First().id_team;
            else
            {	// Добавляем тренера в таблицу
                if (CreateNew)
                {
                    teams NewTeam = new teams()
                    {
                        name = TeamName
                    };
                    DBManagerApp.m_Entities.teams.Add(NewTeam);
                    DBManagerApp.m_Entities.SaveChanges(); // Получаем id_team
                    return NewTeam.id_team;
                }
                else
                    return null;
            }
        }


        /// <summary>
        /// Удалить неиспользованные команды
        /// </summary>
        public static void DeleteUnusedTeams()
        {
            IEnumerable<teams> UnusedTeams = (from team in DBManagerApp.m_Entities.teams
                                              join part in DBManagerApp.m_Entities.participations on team.id_team equals part.team into AllTeams
                                              from Part in AllTeams.DefaultIfEmpty()
                                              where Part == null
                                              select team).ToList();
            foreach (teams Team in UnusedTeams)
                DBManagerApp.m_Entities.teams.Remove(Team);

            try
            {
                DBManagerApp.m_Entities.SaveChanges();
            }
            catch
            {
                // Удаление команды при удалении спортсмена почему-то происходит с ошибкой
            }
        }

        /// <summary>
        /// Удалить неиспользованных тренеров
        /// </summary>
        public static void DeleteUnusedCoaches()
        {
            IEnumerable<coaches> UnusedCoaches = (from coach in DBManagerApp.m_Entities.coaches
                                                  join part in DBManagerApp.m_Entities.participations on coach.id_coach equals part.coach into AllCoaches
                                                  from Part in AllCoaches.DefaultIfEmpty()
                                                  where Part == null
                                                  select coach).ToList();
            foreach (coaches Coach in UnusedCoaches)
                DBManagerApp.m_Entities.coaches.Remove(Coach);

            try
            {
                DBManagerApp.m_Entities.SaveChanges();
            }
            catch
            {
                // Удаление тренера при удалении спортсмена почему-то происходит с ошибкой
            }
        }


        public static bool CheckFileAccessForXMLReading(string FullFilePath, int AttemptsCount = 10)
        {
            bool result = false;
            FileStream stream = null;
            FileInfo fi = new FileInfo(FullFilePath);

            for (int i = 0; i < AttemptsCount && !result; i++)
            {
                stream = null;
                result = false;
                try
                {
                    stream = fi.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException)
                {
                    Thread.Sleep(10);
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        result = true;
                    }
                    else
                        Thread.Sleep(10);
                }
            }

            return result;
        }


        public static string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> property)
        {
            UnaryExpression convertExpression = property.Body as UnaryExpression;
            if (convertExpression != null)
                return ((MemberExpression)convertExpression.Operand).Member.Name;

            return ((MemberExpression)property.Body).Member.Name;
        }


        public static enObjectBaseEqualsResult ObjectBaseEquals(object lhs, object rhs)
        {
            if (lhs == null && rhs == null)
                return enObjectBaseEqualsResult.True;
            else
            {
                if (lhs == null || rhs == null)
                    return enObjectBaseEqualsResult.False;
            }

            return enObjectBaseEqualsResult.ContinueComparing;
        }


        public static bool IsRoundFinished(int Flags, enRounds round)
        {
            return (Flags & (1 << (int)round)) > 0;
        }


        public static bool IsRoundFinished(int? Flags, enRounds round)
        {
            return IsRoundFinished(Flags, (byte)round);
        }


        public static bool IsRoundFinished(int? Flags, byte round)
        {
            return Flags.HasValue && (Flags.Value & (1 << (int)round)) > 0;
        }


        public static void SetRoundFinished(int? Flags, enRounds round)
        {
            if (Flags.HasValue)
                Flags = (1 << (int)round);
            else
                Flags |= (1 << (int)round);
        }


        public static int SetRoundFinished(int? Flags, byte round)
        {
            if (Flags.HasValue)
                return Flags.Value | (1 << (int)round);
            else
                return (1 << (int)round);
        }


        /// <summary>
        /// Внутренняя рекурсивная функция
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="DescendantLevel"></param>
        /// <param name="MaxDescendantLevel"></param>
        /// <returns></returns>
        private static ScrollViewer GetChildScrollViewer(FrameworkElement ctrl, int DescendantLevel, int MaxDescendantLevel)
        {
            if (DescendantLevel == MaxDescendantLevel)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(ctrl); i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(ctrl, i) as FrameworkElement;
                if (child != null)
                {
                    ScrollViewer result = child as ScrollViewer;
                    if (result != null)
                        return result;
                    else
                    {
                        result = GetChildScrollViewer(child, DescendantLevel + 1, MaxDescendantLevel);
                        if (result != null)
                            return result;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Поиск ScrollViewer'а среди детей компонента
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="MaxDescendantLevel">Максимальный уровень вложенности</param>
        /// <returns></returns>
        public static ScrollViewer GetChildScrollViewer(FrameworkElement ctrl, int MaxDescendantLevel = 3)
        {
            return GetChildScrollViewer(ctrl, 0, MaxDescendantLevel);
        }


        public static string PlaceToString(int? Place)
        {
            if (Place == null)
                return "";

            switch (Place)
            {
                case 1:
                    return "I";

                case 2:
                    return "II";

                case 3:
                    return "III";

                default:
                    return Place.ToString();
            }
        }

        public static ExcelApplicationEx StartExcel()
        {
            var res = new ExcelApplicationEx();
            res.App = StartExcel(out res.NewAppCreated);
            return res;
        }


        public static MSExcel.Application StartExcel(out bool NewAppCreated)
        {
            MSExcel.Application instance = null;
            try
            {
                instance = (MSExcel.Application)Marshal.GetActiveObject("Excel.Application");
                NewAppCreated = false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                instance = new MSExcel.Application();
                /*instance.WindowState = MSExcel.XlWindowState.xlMinimized;
                instance.Visible = true;*/
                NewAppCreated = true;
            }

            return instance;
        }


        /// <summary>
        /// Меняем символы всяких "левых пробелов" на обычный пробел
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteInvalidSpaces(string str)
        {
            string result = str.Replace("\u00A0", "\u0020"); // Неразрывный пробел
            result = result.Replace("\u2007", "\u0020"); // Имеет такую же ширину, что и цифры в данном шрифте, и предназначен для набора таблиц
            result = result.Replace("\u202F", "\u0020"); // Узкий неразрывный пробел

            return result;
        }


        /// <summary>
        /// Корректируем строку с фамилией и именем: удаляем лишние пробелы корректируем двойные фамилии и имена
        /// </summary>
        /// <param name="SurnameAndNameFromXls">
        /// 
        /// </param>
        /// <param name="arrNameAndSurname"></param>
        /// <returns></returns>
        public static string CorrectSurnameAndName(string SurnameAndNameFromXls, out string[] arrNameAndSurname)
        {
            string result = DeleteInvalidSpaces(SurnameAndNameFromXls);
            arrNameAndSurname = new string[2] { "", "" };

            if (string.IsNullOrWhiteSpace(result))
                result = null;
            else
            {
                result = result.Trim();
                string[] TrimmedNameOrSurnames = result.Split(' '); // В этом массиве в элементах нет пробелов
                int index = -1;
                bool IncIndex = true;
                foreach (string TrimmedNameOrSurname in TrimmedNameOrSurnames)
                {
                    if (!string.IsNullOrWhiteSpace(TrimmedNameOrSurname))
                    {
                        if (TrimmedNameOrSurname[0] == '-' || TrimmedNameOrSurname.EndsWith("-"))
                        {   // Двойная фамилия или имя
                            if (TrimmedNameOrSurname.EndsWith("-") && TrimmedNameOrSurname.Length > 1 && IncIndex)
                                index++;

                            if (index < 0)
                                index = 0;

                            // Эта переменная нужна для поддержки корейцев, например, Сим Ин Ён
                            var adjustedIndex = Math.Min(index, arrNameAndSurname.Length - 1);
                            if (TrimmedNameOrSurname[0] == '-')
                                arrNameAndSurname[adjustedIndex] += TrimmedNameOrSurname.Length > 1 ? TrimmedNameOrSurname.ToUpper(1) : TrimmedNameOrSurname;
                            else
                                arrNameAndSurname[adjustedIndex] += TrimmedNameOrSurname.ToUpper(0);
                            IncIndex = !TrimmedNameOrSurname.EndsWith("-") && TrimmedNameOrSurname.Length > 1;
                        }
                        else
                        {
                            if (IncIndex)
                                index++;

                            // Эта переменная нужна для поддержки корейцев, например, Сим Ин Ён
                            var adjustedIndex = Math.Min(index, arrNameAndSurname.Length - 1);
                            if (!string.IsNullOrEmpty(arrNameAndSurname[adjustedIndex]))
                                arrNameAndSurname[adjustedIndex] += " ";
                            arrNameAndSurname[adjustedIndex] += TrimmedNameOrSurname.ToUpper(0);
                            IncIndex = true;
                        }
                    }
                }

                // Удаляем все лишние пробелы из result
                result = "";
                foreach (string TrimmedNameOrSurname in arrNameAndSurname)
                    result += " " + TrimmedNameOrSurname;
                result = result.TrimStart();
            }

            return result;
        }


        public static string ParseGrade(string grade, out enGrade gradeInEnum)
        {
            string result = grade;
            gradeInEnum = enGrade.None;
            if (result != null)
            {
                result = result.Trim();
                // Исправляем ошибки в названии разрядов
                if (result.Contains('б') && result.Contains('р'))
                {
                    result = "б/р";
                    gradeInEnum = enGrade.WithoutGrade;
                }
                else if (result.Contains('3'))
                {
                    if (result.Contains('ю'))
                    {
                        result = "3 ю";
                        gradeInEnum = enGrade.Young3;
                    }
                    else
                    {
                        result = "3";
                        gradeInEnum = enGrade.Adult3;
                    }
                }
                else if (result.Contains('2'))
                {
                    if (result.Contains('ю'))
                    {
                        result = "2 ю";
                        gradeInEnum = enGrade.Young2;
                    }
                    else
                    {
                        result = "2";
                        gradeInEnum = enGrade.Adult2;
                    }
                }
                else if (result.Contains('1'))
                {
                    if (result.Contains('ю'))
                    {
                        result = "1 ю";
                        gradeInEnum = enGrade.Young1;
                    }
                    else
                    {
                        result = "1";
                        gradeInEnum = enGrade.Adult1;
                    }
                }
                else if (result.Contains("к", StringComparison.OrdinalIgnoreCase))
                {
                    result = "КМС";
                    gradeInEnum = enGrade.BeforeMaster;
                }
                else if (result.Contains("м", StringComparison.OrdinalIgnoreCase))
                {
                    result = "МС";
                    gradeInEnum = enGrade.Master;
                }
                else // Неизвестный разряд
                {
                    result = GlobalDefines.DEFAULT_XML_STRING_VAL;
                }
            }

            return result;
        }

        /// <summary>
        /// Корректируем строку с командой: удаляем лишние пробелы корректируем
        /// </summary>
        /// <param name="SurnameAndNameFromXls">
        /// 
        /// </param>
        /// <param name="arrNameAndSurname"></param>
        /// <returns></returns>
        public static string CorrectTeam(string value)
        {
            string result = value;
            if (result != null)
            {
                result = GlobalDefines.DeleteInvalidSpaces(result).Trim();
                // Удаляем все неодинарные пробелы из result
                string[] arr = result.Split(' '); // В этом массиве в элементах нет пробелов
                result = "";
                foreach (string TrimmedSecondCol in arr)
                {
                    if (!string.IsNullOrWhiteSpace(TrimmedSecondCol))
                        result += " " + TrimmedSecondCol;
                }
                result = result.TrimStart(); // Удаляем начальный пробел

                // Добавляем пробелы после символов "," и "."
                result = InsertSpacesAfterChar(result, ',');
                result = InsertSpacesAfterChar(result, '.');
            }

            return result;
        }


        /// <summary>
        /// Определяет сколько баллов должен получить каждый участник паровоза
        /// </summary>
        /// <param name="Summ"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static float MakeBalls(int Summ, int Count)
        {
            if (Summ / Count < 1)
            {	// Если результат деления меньше 1, то спортсмен не должен получить баллов
                return 0;
            }
            else
                return (float)Summ / (float)Count;
        }


        public static string CreateCompDate(DateTime? StartDate, DateTime? EndDate)
        {
            string CompDate = null;

            if (StartDate == null)
                return CompDate;

            if (EndDate != null)
            {
                if (StartDate.Value.Month == EndDate.Value.Month)
                {
                    if (StartDate.Value.Day == EndDate.Value.Day)
                        CompDate = StartDate.Value.ToLongDateString();
                    else
                    {
                        CompDate = string.Format(Properties.Resources.resfmtCompDateOneMonth,
                                                    StartDate.Value.Day,
                                                    EndDate.Value.Day,
                                                    GlobalDefines.MONTHS_IN_GENITIVE[StartDate.Value.Month - 1],
                                                    EndDate.Value.Year);
                    }
                }
                else
                {
                    CompDate = string.Format(Properties.Resources.resfmtCompDateTwoMonths,
                                                StartDate.Value.Day,
                                                GlobalDefines.MONTHS_IN_GENITIVE[EndDate.Value.Month - 1],
                                                EndDate.Value.Day,
                                                GlobalDefines.MONTHS_IN_GENITIVE[StartDate.Value.Month - 1],
                                                EndDate.Value.Year);
                }
            }
            else
                CompDate = StartDate.Value.ToLongDateString();

            return CompDate;
        }


        public static string EncodePlace(byte Place)
        {
            switch (Place)
            {
                case 1:
                    return "I";

                case 2:
                    return "II";

                case 3:
                    return "III";

                default:
                    return Place.ToString();
            }
        }


        public static string EncodeSpeedResult(TimeSpan? Result, enAdditionalEventTypes? Event)
        {
            if (Result.HasValue)
            {
                if (Event.HasValue)
                {
                    if (Event.Value.HasFlag(enAdditionalEventTypes.Disqualif))
                        return ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.Disqualif].short_name;
                    else if (Event.Value.HasFlag(enAdditionalEventTypes.DontAppear))
                        return ADDITIONAL_EVENT_NAMES[enAdditionalEventTypes.DontAppear].short_name;
                }
                if (Result == FALL_TIME_SPAN_VAL)
                    return Properties.Resources.resFall;
                else if (Result > FALL_ON_ROUTE_2_TIME_SPAN_VAL)
                {	/* Участник сорвался на второй трассе =>
                     * мы конвертируем результат суммы двух трасс, т.к. время больше GlobalDefines.FALL_ON_ROUTE_2_TIME_SPAN_VAL */
                    return Result.Value.ToString(@"mm\:ss\,ff\*");
                }
                else
                    return Result.Value.ToString(@"mm\:ss\,ff");
            }

            return "";
        }


        public static void CorrectBordersInTotal(MSExcel.Worksheet wsh,
                                                    MSExcel.Range StartRange,
                                                    int RowsQ,
                                                    int PlaceColNum,
                                                    string ResultGradeRangeName,
                                                    string BallsForPlacesRangeName)
        {
            MSExcel.Range rng = StartRange;
            int r = 0;
            for (; r < RowsQ; r++)
            {
                if (rng.Offset[r].Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle == (int)MSExcel.Constants.xlNone)
                {	// Нашли первую незаполненную ячейку 
                    r--;
                    break;
                }
            }

            if (r >= 0 && r < RowsQ)
            {	// Восстанавливаем подчёркивание на всякий случай
                rng = wsh.Range[wsh.Cells[rng.Offset[r].Row, PlaceColNum], rng.Offset[r]];
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlMedium;

                rng = wsh.Range[ResultGradeRangeName].Offset[r + 1];
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
                rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlMedium;

                if (!string.IsNullOrWhiteSpace(BallsForPlacesRangeName))
                {
                    rng = wsh.Range[BallsForPlacesRangeName].Offset[r + 1];
                    rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].LineStyle = MSExcel.XlLineStyle.xlContinuous;
                    rng.Borders[MSExcel.XlBordersIndex.xlEdgeBottom].Weight = MSExcel.XlBorderWeight.xlMedium;
                }
            }
        }


        public static void CheckPublishingDirExists()
        {
            if (!Directory.Exists(STD_PUBLISHING_LOG_DIR))
                Directory.CreateDirectory(STD_PUBLISHING_LOG_DIR);
        }


        public static int CalcMinPlaceForNewGrade(enResultGradeCalcMethod ResultGradeCalcMethod, double raw)
        {
            switch (ResultGradeCalcMethod)
            {
                case SettingsWriter.enResultGradeCalcMethod.Floor:
                    return (int)Math.Floor(raw);

                case SettingsWriter.enResultGradeCalcMethod.Round:
                    return (int)Math.Round(raw, 0);
            }

            return 0;
        }


        public static FalstartsRulesRange GetFalstartsRulesRange(long GroupId, byte CurrentRound)
        {
            FalstartsRulesRange result = new FalstartsRulesRange();

            falsestarts_rules RuleForCurRound = (from rule in DBManagerApp.m_Entities.falsestarts_rules
                                                 where rule.Group == GroupId
                                                         && rule.start_round <= CurrentRound
                                                         && CurrentRound <= rule.end_round
                                                 select rule).FirstOrDefault();
            result.StartRound = RuleForCurRound == null ? CurrentRound : RuleForCurRound.start_round;
            result.EndRound = RuleForCurRound == null ? CurrentRound : RuleForCurRound.end_round;

            return result;
        }
        #endregion
    }
}
