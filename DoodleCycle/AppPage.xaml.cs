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
using NorthernLights;
using Microsoft.Phone.Tasks;

namespace DoodleCycle
{
  public partial class AppPage : PhoneApplicationPage
  {
    public AppPage()
    {
      InitializeComponent();

      ExceptionContainer exception = LittleWatson.GetPreviousException();

      if (null != exception)
      {
        if (LittleWatson.Instance.AllowAnonymousHttpReporting)
        {
          // Need to set up endpoint online to capture this...
        }
        else
        {
          // Show a popup.
          Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
              var email = new EmailComposeTask
              {
                To = "support@doodle.co.uk",
                Subject = "Doodle Cycle: auto-generated problem report",
                Body = string.Format("{0}{2}{1}", exception.Message, exception.StackTrace, Environment.NewLine)
              };
              email.Show();
            });
        }
      }
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