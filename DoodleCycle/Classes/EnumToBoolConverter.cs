using System;
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
  public class EnumToBoolConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value,
        Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return value.ToString() == parameter.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return Enum.Parse(targetType, (string)parameter, true);
    }
    #endregion
  }  
}
