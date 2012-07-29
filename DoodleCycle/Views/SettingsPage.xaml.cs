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
      MessageBox.Show("Boo!");
      switch (e.PropertyName)
      {
        case "RunUnderLockScreen":
          if (App.AppSettings.RunUnderLockScreen)
          {
              // User has enabled running under the lock screen :)
              PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
          }
          else
          {
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Enabled;
          }
          break;
        default:
          break;
      }
    }
  }
}