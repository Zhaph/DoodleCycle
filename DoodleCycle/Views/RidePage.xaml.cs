using System;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using DoodleCycle.Models;
using DoodleCycle.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DoodleCycle.Views
{
  public partial class RidePage : PhoneApplicationPage
  {
    private bool _resumeLastRide;
    private bool _rideStarted;
    private bool _rideInProgress;
    private bool _stopTimer;
    private readonly ApplicationBarIconButton _startStopButton;
    private readonly IApplicationBarIconButton _pauseButton;
    private readonly int _second = 1000; // 1000 milliseconds.
    private readonly Timer _timer;
    private Ride _currentRide;
    private readonly RideDataContext _rideDc;


    private GeoCoordinateWatcher _location;

    public RidePage()
    {
      InitializeComponent();

      Loaded += RidePage_Loaded;

      _timer = new Timer(secondTick, null, Timeout.Infinite, _second);

      _startStopButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      _pauseButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

      _rideDc = new RideDataContext(App.ConnectionString);
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
        if (null == _location)
        {
          _location = new GeoCoordinateWatcher(GeoPositionAccuracy.High) {MovementThreshold = 10}; // Use High Accuracy, ignore low-level inaccuracy...
          _location.StatusChanged += locationStatusChanged;
          _location.PositionChanged += locationPositionChanged;
        }

        _location.Start();
      }
      else
      {
        if (MessageBoxResult.OK == MessageBox.Show("You need to enable Location Services to use this app. Go there now?", "location services not enabled", MessageBoxButton.OKCancel))
        {
          NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/SettingsPage.xaml", UriKind.Relative));
        }
      }

      preparePage();
    }

    private void locationPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
    {
      //throw new NotImplementedException();
      if (_rideInProgress)
      {
        _currentRide.LastPosition = e.Position.Location;
      }
    }

    private void locationStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
    {
      switch (e.Status)
      {
        case GeoPositionStatus.Disabled:
          // The Location Service is Disabled or unsupported.
          // Check to see whether the user has disabled the location service:
          if (GeoPositionPermission.Denied == _location.Permission)
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
          ProgressBar.Visibility = Visibility.Collapsed;
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
        _rideStarted = true;
        _pauseButton.IsEnabled = true;
      }

      if (_resumeLastRide)
      {
        // Get last ride.
        _currentRide = (from r in _rideDc.Rides orderby r.RideStartTime descending select r).FirstOrDefault();
      }
      else
      {
        _currentRide = new Ride { RideDistance = 0.0, RideDurationRaw = 0 };
        // Attach it to the database...
        _rideDc.Rides.InsertOnSubmit(_currentRide);
      }

      DataContext = _currentRide;
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
      _location.Stop();
      NavigationService.Navigate(new Uri("/DoodleCycle;component/AppPage.xaml", UriKind.Relative));
    }

    private void startTimer()
    {
      _timer.Change(_second, _second);
    }

    private void stopTimer()
    {
      _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    private void secondTick(object state)
    {
      Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                  {
                                                    _currentRide.RideDurationRaw++;
                                                  }
        );
    }



    private void startStopRideButtonClick(object sender, EventArgs e)
    {
      if (!_rideInProgress)
      {
        // Start ride, enable Pause button...
        _pauseButton.IsEnabled = true;
        _startStopButton.IconUri = new Uri("/Content/Images/appbar.control.stop.png", UriKind.Relative);
        _rideStarted = true;
        _currentRide.RideStartTime = DateTime.Now;
        startTimer();
        _rideInProgress = true;
      }
      else
      {
        // Stop ride, disable Pause button...
        _rideInProgress = false;
        stopTimer();
        _pauseButton.IsEnabled = false;
        _startStopButton.IsEnabled = false;
        CloseRide.Visibility = Visibility.Visible;

        // Save Ride to Database...
        _rideDc.SubmitChanges();
      }
    }

    private void pauseRideButtonClick(object sender, EventArgs e)
    {
      // Invert Ride in progress...
      if (_rideInProgress)
      {
        _rideInProgress = false;
        stopTimer();
      }
      else
      {
        _rideInProgress = true;
        startTimer();
      }

    }
  }
}