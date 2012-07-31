using System;
using System.Globalization;
using System.Net;
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
  public class TimespanToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Doesn't actually need to be generic, so I'm going to hard-code my requirements:
      var timespan = value is TimeSpan ? (TimeSpan) value : new TimeSpan();

      return string.Format("{0}:{1}:{2}", Math.Floor(timespan.TotalHours), timespan.Minutes, timespan.Seconds);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
