using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}
