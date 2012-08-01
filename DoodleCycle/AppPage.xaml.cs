using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DoodleCycle.ViewModels;

namespace DoodleCycle
{
  public partial class AppPage : PhoneApplicationPage
  {
    private AppViewModel _viewModel;


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
          if (MessageBoxResult.OK == MessageBox.Show("There was an error last time I ran, would you mind sending my author a message? You can enable anonymous reporting in settings.", "sorry about this...", MessageBoxButton.OKCancel))
          {
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

      if (null == _viewModel)
      {
        _viewModel = new AppViewModel(App.ConnectionString);

        _viewModel.LoadData();
      }

      DataContext = _viewModel;
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      App.AppSettings.PropertyChanged += AppSettings_Changed;

      if (System.Windows.Navigation.NavigationMode.Back == e.NavigationMode)
      {
        DataContext = null;
        DataContext = _viewModel;
      }
    }

    protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);

      App.AppSettings.PropertyChanged -= AppSettings_Changed;
    }

    private void AppSettings_Changed(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "MainUnits":
          // Update Ride Values
          break;
        default:
          break;
      }
    }

    private void newRideButtonClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/RidePage.xaml", UriKind.Relative));
    }

    private void resumeRideButtonClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/RidePage.xaml?resumeRide=" + bool.TrueString, UriKind.Relative));
    }

    private void settingsMenuItemClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/SettingsPage.xaml", UriKind.Relative));
    }

    private void aboutMenuItemClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
    }
  }
}