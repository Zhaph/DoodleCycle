using System;
using System.Device.Location;
using System.Windows;
using DoodleCycle.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DoodleCycle.Views
{
  public partial class RidePage : PhoneApplicationPage
  {
    private bool _resumeLastRide;
    private bool _rideInProgress;
    private RideViewModel _rideVM;
    private readonly ApplicationBarIconButton _startStopButton;
    private readonly IApplicationBarIconButton _pauseButton;

    private GeoCoordinateWatcher location;

    public RidePage()
    {
      InitializeComponent();

      Loaded += RidePage_Loaded;

      _startStopButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      _pauseButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      string resumeValue;
      NavigationContext.QueryString.TryGetValue("resumeRide", out resumeValue);

      bool.TryParse(resumeValue, out _resumeLastRide);
    }

    private void RidePage_Loaded(object sender, RoutedEventArgs e)
    {
      // Initialise Location Services...

      if (App.AppSettings.EnableLocationServices)
      {
        // Start Location Watcher:
        if (null == location)
        {
          location = new GeoCoordinateWatcher(GeoPositionAccuracy.High) {MovementThreshold = 20}; // Use High Accuracy.
          location.StatusChanged += locationStatusChanged;
          location.PositionChanged += locationPositionChanged;
        }

        location.Start();
      }
      else
      {
        if (MessageBoxResult.OK == MessageBox.Show("You need to enable Location Services to use this app. Go there now?", "location services not enabled", MessageBoxButton.OKCancel))
        {
          NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/SettingsPage.xaml", UriKind.Relative));
        }
      }
    }

    private void locationPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
    {
      //throw new NotImplementedException();
    }

    private void locationStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
    {
      switch (e.Status)
      {
        case GeoPositionStatus.Disabled:
          // The Location Service is Disabled or unsupported.
          // Check to see whether the user has disabled the location service:
          if (GeoPositionPermission.Denied == location.Permission)
          {
            StatusText.Text = "You need to allow this application access to location services...";
            ProgressBar.Visibility = Visibility.Collapsed;
          }
          else
          {
            // Location is not functioning on this device...
            StatusText.Text = "Your device doesn't appear to support location services...";
            ProgressBar.Visibility = Visibility.Collapsed;
          }
          break;
        case GeoPositionStatus.Initializing:
          // Nothing to do, we should already be in this state...
          break;
        case GeoPositionStatus.NoData:
          // Location service is working, but cannot get location data.
          StatusText.Text = "Unable to retrieve location data.";
          ProgressBar.Visibility = System.Windows.Visibility.Collapsed;
          break;
        case GeoPositionStatus.Ready:
          InitialisingPanel.Visibility = Visibility.Collapsed;
          ContentPanel.Visibility = Visibility.Visible;

          _startStopButton.IsEnabled = true;
          _pauseButton.IsEnabled = false;
          break;
      }
    }

    private void preparePage()
    {
      if (_rideInProgress || _resumeLastRide)
      {
        _rideInProgress = true;
      }

      _rideVM = new RideViewModel(App.ConnectionString, _rideInProgress);

      DataContext = _rideVM;
    }

    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
      // Need to check whether we're running or not...
    }

    private void settingsMenuItemClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/SettingsPage.xaml", UriKind.Relative));
    }

    private void CloseRideClicked(object sender, RoutedEventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/AppPage.xaml", UriKind.Relative));
    }
  }
}