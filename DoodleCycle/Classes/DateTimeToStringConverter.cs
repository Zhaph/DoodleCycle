using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DoodleCycle.Classes
{
  /// <summary>
  /// Required to work around bug in Windows Phone Silverlight implementation with DateTime.ToString.
  /// </summary>
  public class DateTimeToStringConverter: IValueConverter
  {
    private readonly DateTimeFormatInfo _format = Thread.CurrentThread.CurrentCulture.DateTimeFormat;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value != null && value is DateTime)
      {
        var dateTime = (DateTime)value;

        string format = string.Empty;

        if (parameter != null)
        {
          switch ((string)parameter)
          {
            case "g":
              format = string.Format("{0} {1}", _format.ShortDatePattern, _format.ShortTimePattern);
              break;
            case "G":
              format = string.Format("{0} {1}", _format.ShortDatePattern, _format.LongTimePattern);
              break;
            case "d":
              format = _format.ShortDatePattern;
              break;
            case "D":
              format = _format.LongDatePattern;
              break;
            case "t":
              format = _format.ShortTimePattern;
              break;
            case "T":
              format = _format.LongTimePattern;
              break;
          }
        }

        return dateTime.ToString(format, culture);
      }

      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
