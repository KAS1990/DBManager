using DBManager.Global;
using DBManager.Global.Converters;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DBManager.OnlineResults
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
                enOnlineResultsLogItemType Type = enOnlineResultsLogItemType.None;

                if (value is enOnlineResultsLogItemType)
                    Type = (enOnlineResultsLogItemType)value;
                else if (value is int)
                    Type = (enOnlineResultsLogItemType)((int)value);

                switch (Type)
                {
                    case enOnlineResultsLogItemType.Error:
                        return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                    case enOnlineResultsLogItemType.OK:
                        return Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                    case enOnlineResultsLogItemType.None:
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
