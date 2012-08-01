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
using DoodleCycle.Models;

namespace DoodleCycle.Classes
{
  public class DoubleToSpeedStringConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var speed = value is double ? (double) value : 0d;

      if (null != parameter)
      {
        // speed's in m/s, need to convert:
        if (Units.Metric == App.AppSettings.MainUnits)
        {
          speed = speed*3.6;
        }
        else
        {
          speed = speed*2.23693629;
        }
      }

      return Units.Metric == App.AppSettings.MainUnits ? speed.ToString("0.00' km/h'", culture) : speed.ToString("0.00' mph'", culture);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
