using System;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using DoodleCycle.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DoodleCycle.Views
{
  public partial class RidePage : PhoneApplicationPage
  {
    private bool _resumeLastRide;
    private RideState _rideState;
    private readonly ApplicationBarIconButton _startStopButton;
    private readonly IApplicationBarIconButton _pauseButton;
    private readonly int _second = 1000; // 1000 milliseconds.
    private readonly Timer _timer;
    private Ride _currentRide;
    private readonly RideDataContext _rideDc;
    private DateTimeOffset _lastPositionTimestamp;

    private GeoCoordinateWatcher _location;

    public RidePage()
    {
      InitializeComponent();

      Loaded += ridePageLoaded;

      _timer = new Timer(secondTick, null, Timeout.Infinite, _second);

      _startStopButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
      _pauseButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;

      _rideDc = new RideDataContext(App.ConnectionString);
    }

    private void ridePageLoaded(object sender, RoutedEventArgs e)
    {
      // Initialise Location Services...

      if (App.AppSettings.EnableLocationServices)
      {
        StatusText.Text = "initialising...";
        ProgressBar.Visibility = Visibility.Visible;
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
        else
        {
          StatusText.Text = "Location Services are disabled in Settings. Please ensure you turn them on.";
          ProgressBar.Visibility = Visibility.Collapsed;
        }
      }

      preparePage();
    }

    private void locationPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
    {
      if (RideState.InProgressRunning == _rideState)
      {
        double distanceFromLast = _currentRide.LastPosition.GetDistanceTo(e.Position.Location);
        double currentSpeed = distanceFromLast/e.Position.Timestamp.Subtract(_lastPositionTimestamp).TotalSeconds;
        double elevationGain = (e.Position.Location.Altitude > _currentRide.LastPosition.Altitude)
                                 ? e.Position.Location.Altitude - _currentRide.LastPosition.Altitude
                                 : 0.0;

        _currentRide.RideDistance += distanceFromLast;
        _currentRide.CurrentSpeed = currentSpeed;
        if (15 > e.Position.Location.VerticalAccuracy)
        {
          _currentRide.AltitudeChange += elevationGain;
        }
        if (_currentRide.TopSpeed < currentSpeed)
        {
          _currentRide.TopSpeed = currentSpeed;
        }
        _currentRide.LastPosition = e.Position.Location;
        _lastPositionTimestamp = e.Position.Timestamp;
      }
    }

    private void locationStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
    {
      switch (e.Status)
      {
        case GeoPositionStatus.Disabled:
          // Make sure InitialisingPanel is shown.
          InitialisingPanel.Visibility = Visibility.Visible;
          ContentPanel.Visibility = Visibility.Collapsed;
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
          // Make sure InitialisingPanel is shown.
          InitialisingPanel.Visibility = Visibility.Visible;
          ContentPanel.Visibility = Visibility.Collapsed;
          break;
        case GeoPositionStatus.NoData:
          // Make sure InitialisingPanel is shown.
          InitialisingPanel.Visibility = Visibility.Visible;
          ContentPanel.Visibility = Visibility.Collapsed;
          // Location service is working, but cannot get location data.
          StatusText.Text = "Unable to retrieve location data.";
          ProgressBar.Visibility = Visibility.Collapsed;
          break;
        case GeoPositionStatus.Ready:
          // Swap panels over so we can show the ride details.
          InitialisingPanel.Visibility = Visibility.Collapsed;
          ContentPanel.Visibility = Visibility.Visible;

          if (!_resumeLastRide)
          {
            _startStopButton.IsEnabled = true;
            _pauseButton.IsEnabled = false;
          }
          else
          {
            _startStopButton.IsEnabled = true;
            _pauseButton.IsEnabled = true;
          }
          break;
      }
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
      if (RideState.Unstarted == _rideState)
      {
        // Get the current position from the Location Service
        _currentRide.RideStartTime = DateTime.Now;
        _lastPositionTimestamp = _location.Position.Timestamp;
        _currentRide.LastPosition = _location.Position.Location;
        _currentRide.CurrentSpeed = 0.0;

        // Start ride, enable Pause button...
        _pauseButton.IsEnabled = true;
        
        setStopButton();
        startTimer();
        _rideState = RideState.InProgressRunning;
      }
      else
      {
        // Stop ride, disable Pause button...
        _rideState = RideState.Stopped;
        stopTimer();
        _pauseButton.IsEnabled = false;
        _startStopButton.IsEnabled = false;

//        IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
//        using (_rideDc.Log = new StreamWriter(iso.CreateFile("log.txt")))
        {
          // Save Ride to Database...
          _rideDc.SubmitChanges();
        }

        CloseRide.Visibility = Visibility.Visible;
      }
    }

    private void setPauseButton()
    {
      _pauseButton.IconUri = new Uri("/Content/Images/appbar.control.pause.png", UriKind.Relative);
      _pauseButton.Text = "Pause";
    }

    private void setPauseButtonAsResume()
    {
      _pauseButton.IconUri = new Uri("/Content/Images/appbar.control.play.png", UriKind.Relative);
      _pauseButton.Text = "Resume";
    }

    private void setStopButton()
    {
      _startStopButton.IconUri = new Uri("/Content/Images/appbar.control.stop.png", UriKind.Relative);
      _startStopButton.Text = "Stop";
    }

    private void pauseRideButtonClick(object sender, EventArgs e)
    {
      // Invert Ride in progress...
      pauseRide(false);
    }

    private void pauseRide(bool saveRide)
    {
      if (RideState.InProgressRunning == _rideState)
      {
        setPauseButtonAsResume();
        _rideState = RideState.InProgressPaused;
        stopTimer();
      }
      else
      {
        setPauseButton();
        _rideState = RideState.InProgressRunning;
        startTimer();
      }

      if (saveRide)
      {
        // Save Ride to Database...
        _rideDc.SubmitChanges();
      }
    }

    private void preparePage()
    {
      if (RideState.Unstarted != _rideState || _resumeLastRide)
      {
        _rideState = RideState.InProgressPaused;
        setStopButton();
        _startStopButton.IsEnabled = true;
        _pauseButton.IsEnabled = true;
      }

      if (_resumeLastRide)
      {
        // Get last ride.
        _currentRide = (from r in _rideDc.Rides orderby r.RideStartTime descending select r).FirstOrDefault();
        setPauseButtonAsResume();
      }
      else
      {
        _currentRide = new Ride { RideDistance = 0.0, RideDurationRaw = 0, RideStartTime = DateTime.Now};
        // Attach it to the database...
        _rideDc.Rides.InsertOnSubmit(_currentRide);
      }

      DataContext = _currentRide;
    }

    private void startTimer()
    {
      _timer.Change(_second, _second);
    }

    private void stopTimer()
    {
      _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      string resumeValue;
      NavigationContext.QueryString.TryGetValue("resumeRide", out resumeValue);

      bool.TryParse(resumeValue, out _resumeLastRide);

      if (NavigationMode.Back == e.NavigationMode && RideState.Unstarted != _rideState)
      {
        // Probably returning from settings...
        _resumeLastRide = true;
        DataContext = null;
        DataContext = _currentRide;
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
      // Need to check whether we're running or not...
      if (RideState.InProgressRunning == _rideState)
      {
        pauseRide(true);
      }
      if (null != _location && _location.Status == GeoPositionStatus.Ready)
      {
        _location.Stop();
      }
    }

    private void settingsMenuItemClicked(object sender, EventArgs e)
    {
      NavigationService.Navigate(new Uri("/DoodleCycle;component/Views/SettingsPage.xaml", UriKind.Relative));
    }

    private void closeRideClicked(object sender, RoutedEventArgs e)
    {
      _location.Stop();
      NavigationService.GoBack();
    }
  }

  internal enum RideState
  {
    Unstarted,
    InProgressRunning,
    InProgressPaused,
    Stopped,
  }
}