using DBManager.Global;
using DBManager.Global.Converters;
using DBManager.SettingsWriter;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace DBManager.InterfaceElements
{
    /// <summary>
    /// Interaction logic for CFontStylePicker.xaml
    /// </summary>
    public partial class CFontStylePicker : CNotifyPropertyChangedUserCtrl
    {
        #region Modified
        public static readonly string ModifiedPropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.Modified);

        private bool m_Modified = false;

        public bool Modified
        {
            get { return m_Modified; }
            set
            {
                if (m_Modified != value)
                {
                    m_Modified = value;
                    OnPropertyChanged(ModifiedPropertyName);
                }
            }
        }
        #endregion


        #region MyFontWeight
        private static readonly string MyFontWeightPropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.MyFontWeight);

        private FontWeight m_MyFontWeight = FontWeights.Normal;

        public FontWeight MyFontWeight
        {
            get { return m_MyFontWeight; }
            set
            {
                if (m_MyFontWeight != value)
                {
                    m_MyFontWeight = value;
                    Modified = true;
                    OnPropertyChanged(MyFontWeightPropertyName);
                }
            }
        }
        #endregion


        #region MyFontStyle
        private static readonly string MyFontStylePropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.MyFontStyle);

        private FontStyle m_MyFontStyle = FontStyles.Normal;

        public FontStyle MyFontStyle
        {
            get { return m_MyFontStyle; }
            set
            {
                if (m_MyFontStyle != value)
                {
                    m_MyFontStyle = value;
                    Modified = true;
                    OnPropertyChanged(MyFontStylePropertyName);
                }
            }
        }
        #endregion


        #region BackgroundColor
        public static readonly string BackgroundColorPropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.BackgroundColor);

        private Color m_BackgroundColor = Colors.White;

        public Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set
            {
                if (m_BackgroundColor != value)
                {
                    m_BackgroundColor = value;
                    Modified = true;
                    OnPropertyChanged(BackgroundColorPropertyName);
                }
            }
        }
        #endregion


        #region ForeColor
        private static readonly string ForeColorPropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.ForeColor);

        private Color m_ForeColor = Colors.Black;

        public Color ForeColor
        {
            get { return m_ForeColor; }
            set
            {
                if (m_ForeColor != value)
                {
                    m_ForeColor = value;
                    Modified = true;
                    OnPropertyChanged(ForeColorPropertyName);
                }
            }
        }
        #endregion


        #region TextSample
        private static readonly string TextSamplePropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.TextSample);

        private string m_TextSample = Properties.Resources.resTextSample;

        public string TextSample
        {
            get { return m_TextSample; }
            set
            {
                if (m_TextSample != value)
                {
                    m_TextSample = value;
                    Modified = true;
                    OnPropertyChanged(TextSamplePropertyName);
                }
            }
        }
        #endregion


        #region FieldsToShow
        private static readonly string FieldsToShowPropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.FieldsToShow);

        private enFieldsToShow m_FieldsToShow = enFieldsToShow.All;

        public enFieldsToShow FieldsToShow
        {
            get { return m_FieldsToShow; }
            set
            {
                if (m_FieldsToShow != value)
                {
                    m_FieldsToShow = value;
                    OnPropertyChanged(FieldsToShowPropertyName);
                }
            }
        }
        #endregion


        #region FontFamilyName
        private static readonly string FontFamilyNamePropertyName = GlobalDefines.GetPropertyName<CFontStylePicker>(m => m.FontFamilyName);

        private string m_FontFamilyName = "Arial";

        public string FontFamilyName
        {
            get { return m_FontFamilyName; }
            set
            {
                if (m_FontFamilyName != value)
                {
                    m_FontFamilyName = value;
                    OnPropertyChanged(FontFamilyNamePropertyName);
                }
            }
        }
        #endregion


        public CFontStyleSettings FontStyleSettings
        {
            get
            {
                return new CFontStyleSettings()
                {
                    FontWeight = this.MyFontWeight,
                    FontStyle = this.MyFontStyle,
                    ForeColor = this.ForeColor,
                    BackgroundColor = this.BackgroundColor
                };
            }
            set
            {
                this.MyFontWeight = value.FontWeight;
                this.MyFontStyle = value.FontStyle;
                this.ForeColor = value.ForeColor;
                this.BackgroundColor = value.BackgroundColor;
            }
        }


        public CFontStylePicker()
        {
            InitializeComponent();

            Modified = false;
        }
    }


    [Flags]
    public enum enFieldsToShow : uint
    {
        Bold = 1 << 1,
        Italic = 1 << 2,
        ForeColor = 1 << 3,
        BackgroundColor = 1 << 4,
        All = 0xFFFFFFFF
    }


    public class FieldsToShowToVisibilityConverter : MarkupConverterBase
    {
        private Visibility m_UnvisibleValue = Visibility.Collapsed;
        /// <summary>
        /// Значение, которое используется в качестве Visible = false
        /// </summary>
        public Visibility UnvisibleValue
        {
            get { return m_UnvisibleValue; }
            set { m_UnvisibleValue = value; }
        }

        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is enFieldsToShow && parameter is enFieldsToShow)
            {
                enFieldsToShow FieldsToShow = (enFieldsToShow)value;
                enFieldsToShow CheckingFileld = (enFieldsToShow)parameter;
                return FieldsToShow.HasFlag(CheckingFileld) ? Visibility.Visible : UnvisibleValue;
            }
            else
                return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotFiniteNumberException("ConvertBack is not implemented in FieldsToShowToVisibilityConverter");
        }


        public FieldsToShowToVisibilityConverter() :
            base()
        {
        }
    }


    public class FontWeightToBoolConverter : MarkupConverterBase
    {
        private FontWeight m_TrueValue = FontWeights.Bold;
        public FontWeight TrueValue
        {
            get { return m_TrueValue; }
            set { m_TrueValue = value; }
        }

        private FontWeight m_FalseValue = FontWeights.Normal;
        public FontWeight FalseValue
        {
            get { return m_FalseValue; }
            set { m_FalseValue = value; }
        }

        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is FontWeight)
                return (FontWeight)value == TrueValue;
            else
                return false;
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? TrueValue : FalseValue;
            else if (value is bool?)
                return (((bool?)value).HasValue && ((bool?)value).Value) ? TrueValue : FalseValue;
            else
                return FalseValue;
        }


        public FontWeightToBoolConverter() :
            base()
        {
        }
    }


    public class FontStyleToBoolConverter : MarkupConverterBase
    {
        private FontStyle m_TrueValue = FontStyles.Italic;
        public FontStyle TrueValue
        {
            get { return m_TrueValue; }
            set { m_TrueValue = value; }
        }

        private FontStyle m_FalseValue = FontStyles.Normal;
        public FontStyle FalseValue
        {
            get { return m_FalseValue; }
            set { m_FalseValue = value; }
        }

        public override object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is FontStyle)
                return (FontStyle)value == TrueValue;
            else
                return false;
        }

        public override object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? TrueValue : FalseValue;
            else if (value is bool?)
                return (((bool?)value).HasValue && ((bool?)value).Value) ? TrueValue : FalseValue;
            else
                return FalseValue;
        }


        public FontStyleToBoolConverter() :
            base()
        {
        }
    }
}
