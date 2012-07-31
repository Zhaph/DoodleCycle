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
  public class DoubleToDistanceStringConverter: IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var metres = value is double ? (double) value : 0d;
      bool main = parameter.ToString() == "Main";

      string distance;

      if (main)
      {
        distance = Units.Metric == App.AppSettings.MainUnits ? LongDistance(metres, Units.Metric).ToString("0.0' km'", culture) : LongDistance(metres, Units.Imperial).ToString("0.0' miles'", culture);
      }
      else
      {
        distance = Units.Metric == App.AppSettings.ElevationUnits ? ShortDistance(metres, Units.Metric).ToString("0.0' m'", culture) : ShortDistance(metres, Units.Imperial).ToString("0.0'\''", culture);
      }

      return distance;
    }

    internal static double LongDistance(double metres, Units units)
    {
      return Units.Metric == units ? metres*0.001 : metres*0.000621371192;
    }

    internal static double ShortDistance(double metres, Units units)
    {
      return Units.Metric == units ? metres : metres*3.2808399;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
