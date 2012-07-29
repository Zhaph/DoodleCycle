using System;
using System.ComponentModel;
using System.Windows;
using DoodleCycle.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DoodleCycle.Views
{
  public partial class SettingsPage : PhoneApplicationPage
  {
    public SettingsPage()
    {
      InitializeComponent();

      DataContext = App.AppSettings;
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      App.AppSettings.PropertyChanged += AppSettings_PropertyChanged;
    }

    protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);
      App.AppSettings.PropertyChanged -= AppSettings_PropertyChanged;
    }

    private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "RunUnderLockScreen":
          var pas = PhoneApplicationService.Current;

          if (App.AppSettings.RunUnderLockScreen && pas.ApplicationIdleDetectionMode == IdleDetectionMode.Enabled)
          {
            // User has enabled running under the lock screen :)
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
          }
          else
          {
            try
            {
              PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Enabled;
            }
            catch (InvalidOperationException)
            {
              // Requires restart.
              MessageBox.Show("You need to restart the app to change this setting.", "sorry about this...", MessageBoxButton.OK);
            }
          }

          break;
        default:
          break;
      }
    }
  }
}