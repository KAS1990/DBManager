using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using DBManager.Global;
using System.Windows.Controls;

namespace DBManager.AttachedProperties
{
	/// <summary>
	/// Свойство для задания размеров изображений для кнопок Ribbon'a
	/// </summary>
	public class RibbonImageResizeAttachedProps : DependencyObject
	{
		#region ImageWidth
		public static double GetImageWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(ImageWidthProperty);
		}
		
		
		public static void SetImageWidth(DependencyObject obj, double value)
		{
			obj.SetValue(ImageWidthProperty, value);
		}
		
		
		public static readonly DependencyProperty ImageWidthProperty =
			DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(RibbonImageResizeAttachedProps), new UIPropertyMetadata(0.0));
		#endregion


		#region ImageHeight
		public static double GetImageHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(ImageHeightProperty);
		}
		
		
		public static void SetImageHeight(DependencyObject obj, double value)
		{
			obj.SetValue(ImageHeightProperty, value);
		}
		
		
		public static readonly DependencyProperty ImageHeightProperty =
			DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(RibbonImageResizeAttachedProps), new UIPropertyMetadata(0.0));
		#endregion
	}


	public class BtnWithImagesAttachedProps : DependencyObject
	{
		#region Image
		public static ImageSource GetImage(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(ImageProperty);
		}


		public static void SetImage(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(ImageProperty, value);
		}


		/// <summary>
		/// Картика, отображаемая на кнопке
		/// </summary>
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(BtnWithImagesAttachedProps), new UIPropertyMetadata(null));
		#endregion


		#region ImageOnFocus
		public static ImageSource GetImageOnFocus(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(ImageOnCheckedProperty);
		}


		public static void SetImageOnFocus(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(ImageOnCheckedProperty, value);
		}


		/// <summary>
		/// Картинка, отображаемая на кнопке, когда на неё наведена мышь
		/// </summary>
		public static readonly DependencyProperty ImageOnFocusProperty =
			DependencyProperty.RegisterAttached("ImageOnFocus", typeof(ImageSource), typeof(BtnWithImagesAttachedProps), new UIPropertyMetadata(null));
		#endregion


		#region ImageOnChecked
		public static ImageSource GetImageOnChecked(DependencyObject obj)
		{
			return (ImageSource)obj.GetValue(ImageOnCheckedProperty);
		}


		public static void SetImageOnChecked(DependencyObject obj, ImageSource value)
		{
			obj.SetValue(ImageOnCheckedProperty, value);
		}


		/// <summary>
		/// Картинка, отображаемая на кнопке, когда она вдавлена
		/// </summary>
		public static readonly DependencyProperty ImageOnCheckedProperty =
			DependencyProperty.RegisterAttached("ImageOnChecked", typeof(ImageSource), typeof(BtnWithImagesAttachedProps), new UIPropertyMetadata(null));
		#endregion
	}
	

	public class RoundResultsAttachedProps : DependencyObject
	{
		#region ExtraBorderBrush
		public static Brush GetExtraBorderBrush(DependencyObject obj)
		{
			return (Brush)obj.GetValue(ExtraBorderBrushProperty);
		}


		public static void SetExtraBorderBrush(DependencyObject obj, Brush value)
		{
			obj.SetValue(ExtraBorderBrushProperty, value);
		}


		/// <summary>
		/// Картика, отображаемая на кнопке
		/// </summary>
		public static readonly DependencyProperty ExtraBorderBrushProperty =
			DependencyProperty.RegisterAttached("ExtraBorderBrush",
												typeof(Brush),
												typeof(RoundResultsAttachedProps),
												new UIPropertyMetadata(Brushes.Transparent));
		#endregion


		#region FilterTarget
		public static enFilterTarget GetFilterTarget(DependencyObject obj)
		{
			return (enFilterTarget)obj.GetValue(FilterTargetProperty);
		}


		public static void SetFilterTarget(DependencyObject obj, enFilterTarget value)
		{
			obj.SetValue(FilterTargetProperty, value);
		}


		/// <summary>
		/// Картика, отображаемая на кнопке
		/// </summary>
		public static readonly DependencyProperty FilterTargetProperty =
			DependencyProperty.RegisterAttached("FilterTarget",
												typeof(enFilterTarget),
												typeof(RoundResultsAttachedProps),
												new UIPropertyMetadata(enFilterTarget.SecondCol));
		#endregion
	}


	public class RightPanelAttachedProps : DependencyObject
	{
		#region ColoredLabelControlOrientation
		public static Orientation GetRightPanelAttachedProps(DependencyObject obj)
		{
			return (Orientation)obj.GetValue(ColoredLabelControlOrientationProperty);
		}


		public static void SetColoredLabelControlOrientation(DependencyObject obj, Orientation value)
		{
			obj.SetValue(ColoredLabelControlOrientationProperty, value);
		}


		/// <summary>
		/// Картика, отображаемая на кнопке
		/// </summary>
		public static readonly DependencyProperty ColoredLabelControlOrientationProperty =
			DependencyProperty.RegisterAttached("ColoredLabelControlOrientation",
												typeof(Orientation),
												typeof(RightPanelAttachedProps),
												new UIPropertyMetadata(Orientation.Vertical));
		#endregion
	}
}
