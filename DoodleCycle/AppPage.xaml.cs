using System;
using System.Windows;
using System.Windows.Navigation;
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      if (NavigationMode.Back == e.NavigationMode)
      {
        DataContext = null;
        // Need to reload the data as we're using "GoBack", and it doesn't refresh the data context.
        _viewModel.LoadData();
        DataContext = _viewModel;
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