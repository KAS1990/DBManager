using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using DBManager.Global.Converters;
using DBManager.Global;
using System.Windows.Interop;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DBManager.FTP
{
	/// <summary>
	/// Преобразование типа элемента лога в картику
	/// </summary>
	public class LogItemTypeToImageMarkupConverter : MarkupConverterBase
	{
		public override object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				enFTPLogItemType Type = enFTPLogItemType.None;

				if (value is enFTPLogItemType)
					Type = (enFTPLogItemType)value;
				else if (value is int)
					Type = (enFTPLogItemType)((int)value);

				switch (Type)
				{
					case enFTPLogItemType.Error:
						return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					
					case enFTPLogItemType.OK:
						return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					
					case enFTPLogItemType.None:
					default:
						return null;
				}
			}
			else
				return null;
		}

		public override object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new Exception("LogItemTypeToImageMarkupConverter.ConvertBack is not implemented");
		}


		public LogItemTypeToImageMarkupConverter() :
			base()
		{
		}
	}
}
