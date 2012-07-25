using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace DoodleCycle
{
  public partial class AppPage : PhoneApplicationPage
  {
    public AppPage()
    {
      InitializeComponent();
    }

    private void aboutMenuItemClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
    }

    private void newRideButtonClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/RidePage.xaml", UriKind.Relative));
    }
  }
}