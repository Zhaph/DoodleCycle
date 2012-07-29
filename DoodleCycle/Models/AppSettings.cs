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
  }

  public enum Units
  {
    Imperial,
    Metric,
  }
}
