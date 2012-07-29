using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.Phone.Shell;
using NorthernLights;

namespace DoodleCycle.Models
{
  public class AppSettings : INotifyPropertyChanged
  {
    public bool EnableLocationServices
    {
      get { return PersistentVariables.Get<bool>("EnableLocationServices"); }
      set { PersistentVariables.Set("EnableLocationServices", value); }
    }

    public bool RunUnderLockScreen
    {
      get { return PersistentVariables.Get<bool>("RunUnderLockScreen"); }
      set { PersistentVariables.Set("RunUnderLockScreen", value); }
    }

    public Units MainUnits
    {
      get { return PersistentVariables.Get<Units>("MainUnits"); }
      set { PersistentVariables.Set("MainUnits", value); }
    }

    public Units ElevationUnits
    {
      get { return PersistentVariables.Get<Units>("ElevationUnits"); }
      set { PersistentVariables.Set("ElevationUnits", value); }
    }

    public bool AllowAnonymousReporting
    {
      get { return PersistentVariables.Get<bool>("AllowAnonymousReporting"); }
      set { PersistentVariables.Set("AllowAnonymousReporting", value); }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged != null)
      {
        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }

      switch (propertyName)
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

  public enum Units
  {
    Imperial,
    Metric,
  }
}
