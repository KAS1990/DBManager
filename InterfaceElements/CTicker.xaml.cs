using DBManager.Global;
using DBManager.Global.Converters;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DBManager.InterfaceElements
{
    /// <summary>
    /// Interaction logic for CTicker.xaml
    /// </summary>
    [RuntimeNameProperty("Name")]
    [DebuggerDisplay("Text = {TickerText}")]
    public partial class CTicker : CNotifyPropertyChangedUserCtrl
    {
        public const double MIN_SPEED_DPS = 50.0;


        #region TickerSpeed
        /// <summary>
        /// Скорость бегущей строки в единицах в секунду
        /// </summary>
        public double TickerSpeed
        {
            get { return (double)GetValue(TickerSpeedProperty); }
            set { SetValue(TickerSpeedProperty, value); }
        }

        public static readonly DependencyProperty TickerSpeedProperty =
            DependencyProperty.Register("TickerSpeed",
                                        typeof(double),
                                        typeof(CTicker),
                                        new FrameworkPropertyMetadata(MIN_SPEED_DPS,
                                                                        new PropertyChangedCallback(OnTickerSpeedChanged),
                                                                        new CoerceValueCallback(CoerceTickerSpeed)));

        private static object CoerceTickerSpeed(DependencyObject d, object value)
        {
            // Запрещаем устанавливать скорость меньше минимальной
            if ((double)value < MIN_SPEED_DPS)
                value = MIN_SPEED_DPS;

            return value;
        }

        private static void OnTickerSpeedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CTicker ticker = (CTicker)sender;

            if (ticker != null && e.NewValue != null && !string.IsNullOrWhiteSpace(e.NewValue.ToString()))
            {
                if (ticker.RunAnimation)
                    ticker.RestartAnimation();
                else
                    ticker.RunAnimation = true;
            }
            else
                ticker.RunAnimation = false;
        }
        #endregion


        #region TickerText
        /// <summary>
        /// Текст бегущей сроки
        /// </summary>
        public string TickerText
        {
            get { return (string)GetValue(TickerTextProperty); }
            set
            {
                StopAnimation();
                bool ValueChanged = TickerText != value;
                SetValue(TickerTextProperty, value);
                if (!ValueChanged && RunAnimation)
                    RestartAnimation();
            }
        }

        public static readonly DependencyProperty TickerTextProperty =
            DependencyProperty.Register("TickerText",
                                        typeof(string),
                                        typeof(CTicker),
                                        new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnTickerTextChanged)));

        private static void OnTickerTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CTicker ticker = (CTicker)sender;

            if (ticker != null && e.NewValue != null && !string.IsNullOrWhiteSpace(e.NewValue.ToString()))
            {
                if (ticker.RunAnimation)
                    ticker.RestartAnimation();
                else
                    ticker.RunAnimation = true;
            }
            else
                ticker.RunAnimation = false;
        }
        #endregion


        #region RunAnimation
        private static readonly string RunAnimationPropertyName = GlobalDefines.GetPropertyName<CTicker>(m => m.RunAnimation);

        private bool m_RunAnimation = false;

        public bool RunAnimation
        {
            get { return m_RunAnimation; }
            set
            {
                if (TickerText == null)
                    value = false;

                if (m_RunAnimation != value)
                {
                    m_RunAnimation = value;

                    if (m_RunAnimation)
                        RestartAnimation();
                    else
                        StopAnimation();

                    OnPropertyChanged(RunAnimationPropertyName);
                }
            }
        }
        #endregion


        private readonly Storyboard strbrdTicker1OneTime = null;
        private readonly Storyboard strbrdTicker1 = null;

        private readonly Storyboard strbrdTicker2Hide = null;
        private readonly Storyboard strbrdTicker2 = null;


        public CTicker()
        {
            InitializeComponent();

            strbrdTicker1OneTime = Resources["Ticker1OneTimeStoryboard"] as Storyboard;
            strbrdTicker1 = Resources["Ticker1Storyboard"] as Storyboard;

            strbrdTicker2Hide = Resources["Ticker2HideStoryboard"] as Storyboard;
            strbrdTicker2 = Resources["Ticker2Storyboard"] as Storyboard;
        }


        private void cnvsTicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize != new Size(0, 0) && e.WidthChanged && RunAnimation)
                RestartAnimation();
        }


        private void cnvsTicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (RunAnimation)
                RestartAnimation();
        }


        private void StopAnimation()
        {
            if (strbrdTicker2 != null)
                strbrdTicker2.Stop(cnvsTicker);

            if (strbrdTicker2Hide != null)
                strbrdTicker2Hide.Stop(cnvsTicker);

            if (strbrdTicker1 != null)
                strbrdTicker1.Stop(cnvsTicker);

            if (strbrdTicker1OneTime != null)
                strbrdTicker1OneTime.Stop(cnvsTicker);
        }


        public void RestartAnimation()
        {
            if (IsLoaded)
            {
                StopAnimation();

                DispatcherTimer tmrHack = new DispatcherTimer()
                {
                    Interval = new TimeSpan(0, 0, 1)
                };
                tmrHack.Tick += (s, e) =>
                {
                    tmrHack.Stop();
                    if (strbrdTicker1 != null)
                        strbrdTicker1.Begin(cnvsTicker);

                    if (strbrdTicker1OneTime != null)
                        strbrdTicker1OneTime.Begin(cnvsTicker, true);

                    if (strbrdTicker2Hide != null)
                        strbrdTicker2Hide.Begin(cnvsTicker, true);

                    if (strbrdTicker2 != null)
                        strbrdTicker2.Begin(cnvsTicker, true);
                };
                tmrHack.Start();
            }
        }
    }


    /// <summary>
    /// Вычисляет левую границу, до которой должен двигаться текст
    /// </summary>
    public class LastLeftMultiTickerMarkupConverter : MarkupMultiConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                double CanvasWidth = values[0] is double ? (double)values[0] : 0;
                double TextWidth = values[1] is double ? (double)values[1] : 0;

                if (TextWidth > 0)
                {
                    if (TextWidth > CanvasWidth / 2.0)
                        return -(CanvasWidth / 2.0 + TextWidth + (TextWidth - CanvasWidth / 2.0));
                    else
                        return -(CanvasWidth / 2.0 + TextWidth);
                }
                else
                    return 100500.0; // Чтобы не было ошибок в bindings
            }
            else
                return 100500.0; // Чтобы не было ошибок в bindings
        }


        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("LastLeftMultiTickerMarkupConverter.ConvertBack is not implemented");
        }


        public LastLeftMultiTickerMarkupConverter() :
            base()
        {
        }
    }


    /// <summary>
    /// Вычисляет левую границу, до которой должен двигаться текст
    /// </summary>
    public class LastLeftTickerMarkupConverter : MarkupConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double TextWidth = value is double ? (double)value : 0;
            return -TextWidth;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("LastLeftTickerMarkupConverter.ConvertBack is not implemented");
        }


        public LastLeftTickerMarkupConverter() :
            base()
        {
        }
    }


    /// <summary>
    /// Вычисляет длительность анимации в зависимости от длины бегущей строки, длины текста и скорости
    /// </summary>
    public class DurationTickerConverter : IMultiValueConverter
    {
        private enum enConverterVariant
        {
            StoryboardDuration,
            AnimationDuration
        }


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && parameter != null && parameter is string)
            {
                enConverterVariant Variant = (enConverterVariant)int.Parse(parameter.ToString());
                double CanvasWidth = values[0] is double ? (double)values[0] : 0;
                double TextWidth = values[1] is double ? (double)values[1] : 0;
                double SpeedDpS = values[2] is double ? (double)values[2] : CTicker.MIN_SPEED_DPS; // Скорость в пикселях в секунду

                int result = 0;
                if (TextWidth > 0 && SpeedDpS > 0)
                {
                    switch (Variant)
                    {
                        case enConverterVariant.StoryboardDuration:
                            result = (int)Math.Round((CanvasWidth / 2.0 + TextWidth) / (SpeedDpS / 1000.0)) * 2;
                            break;

                        case enConverterVariant.AnimationDuration:
                            result = (int)Math.Round((CanvasWidth + TextWidth) / (SpeedDpS / 1000.0));
                            break;
                    }
                }
                return new Duration(TimeSpan.FromMilliseconds(result));
            }
            else
                return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("BeginTimeTickerConverter.ConvertBack is not implemented");
        }
    }


    /// <summary>
    /// Вычисляет длительность анимации в зависимости от длины бегущей строки, длины текста и скорости
    /// </summary>
    public class DurationTickerOneTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                double CanvasWidth = values[0] is double ? (double)values[0] : 0;
                double TextWidth = values[1] is double ? (double)values[1] : 0;
                double SpeedDpS = values[2] is double ? (double)values[2] : CTicker.MIN_SPEED_DPS;

                int BegTimeInMS = 0;
                if (TextWidth > 0 && SpeedDpS > 0)
                {
                    if (TextWidth > CanvasWidth / 2.0)
                        BegTimeInMS = (int)Math.Round((CanvasWidth / 2.0 + TextWidth + (TextWidth - CanvasWidth / 2.0)) / (SpeedDpS / 1000.0));
                    else
                        BegTimeInMS = (int)Math.Round((CanvasWidth / 2.0 + TextWidth) / (SpeedDpS / 1000.0));
                }
                TimeSpan result = TimeSpan.FromMilliseconds(BegTimeInMS);
                if (targetType == typeof(TimeSpan?))
                    return (TimeSpan?)result;
                else
                    return new Duration(result);
            }
            else
                return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("BeginTimeTickerOneTimeConverter.ConvertBack is not implemented");
        }
    }


    /// <summary>
    /// Вычисляет длительность анимации в зависимости от длины бегущей строки, длины текста и скорости
    /// </summary>
    public class FirstBeginTimeTickerConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3)
            {
                double CanvasWidth = values[0] is double ? (double)values[0] : 0;
                double TextWidth = values[1] is double ? (double)values[1] : 0;
                double SpeedDpS = values[2] is double ? (double)values[2] : CTicker.MIN_SPEED_DPS;

                int BegTimeInMS = 0;
                if (TextWidth > 0 && SpeedDpS > 0)
                {
                    if (TextWidth > CanvasWidth / 2.0)
                        BegTimeInMS = (int)Math.Round((TextWidth - CanvasWidth / 2.0) / (SpeedDpS / 1000.0));
                }
                TimeSpan result = TimeSpan.FromMilliseconds(BegTimeInMS);
                if (targetType == typeof(TimeSpan?))
                    return (TimeSpan?)result;
                else
                    return new Duration(result);
            }
            else
                return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new Exception("BeginTimeTickerConverter.ConvertBack is not implemented");
        }
    }


    /// <summary>
    /// Отладочный конвертер
    /// </summary>
    public class HalfWidthConverter : MarkupConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double CanvasWidth = value is double ? (double)value : 0;
            return CanvasWidth / 2.0;
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("HalfWidthConverter.ConvertBack is not implemented");
        }


        public HalfWidthConverter() :
            base()
        {

        }
    }
}
