using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DBManager.InterfaceElements
{
    public class ButtonWithPicture : Button
    {
        #region ButtonText

        /// <summary>
        /// Текст, отображаемый на кнопке
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        /// <summary>
        /// Текст, отображаемый на кнопке
        /// </summary>
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(ButtonWithPicture), new FrameworkPropertyMetadata(null));
        #endregion

        #region ButtonImage

        /// <summary>
        /// Картика, отображаемая на кнопке
        /// </summary>
        public ImageSource ButtonImage
        {
            get { return (ImageSource)GetValue(ButtonImageProperty); }
            set { SetValue(ButtonImageProperty, value); }
        }

        /// <summary>
        /// Картика, отображаемая на кнопке
        /// </summary>
        public static readonly DependencyProperty ButtonImageProperty =
            DependencyProperty.Register("ButtonImage", typeof(ImageSource), typeof(ButtonWithPicture), new FrameworkPropertyMetadata(null));

        #endregion

        #region ImageLocation

        /// <summary>
        /// Позиция картинки
        /// </summary>
        public ButtonImageLocation ImageLocation
        {
            get { return (ButtonImageLocation)GetValue(ImageLocationProperty); }
            set { SetValue(ImageLocationProperty, value); }
        }

        /// <summary>
        /// Позиция картинки
        /// </summary>
        public static readonly DependencyProperty ImageLocationProperty =
            DependencyProperty.Register("ImageLocation", typeof(ButtonImageLocation), typeof(ButtonWithPicture), new FrameworkPropertyMetadata(ButtonImageLocation.Top));

        #endregion

        #region MaxTextWidth

        /// <summary>
        /// Максимальная ширина текста
        /// </summary>
        public double MaxTextWidth
        {
            get { return (double)GetValue(MaxTextWidthProperty); }
            set { SetValue(MaxTextWidthProperty, value); }
        }

        /// <summary>
        /// Максимальная ширина текста
        /// </summary>
        public static readonly DependencyProperty MaxTextWidthProperty =
            DependencyProperty.Register("MaxTextWidth", typeof(double), typeof(ButtonWithPicture), new FrameworkPropertyMetadata(double.PositiveInfinity));

        #endregion
    }


    public enum ButtonImageLocation
    {
        Left,
        Top,
    }
}
