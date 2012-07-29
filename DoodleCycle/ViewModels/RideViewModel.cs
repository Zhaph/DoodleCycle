using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DoodleCycle.Models;

namespace DoodleCycle.ViewModels
{
  public class RideViewModel : INotifyPropertyChanged
  {
    private readonly RideDataContext _rideDc;
    private readonly bool _resumeLastRide;
    
    public bool IsDataLoaded { get; private set; }

    public Ride CurrentRide { get; private set; }

    public RideViewModel(string rideDbConnectionString): this(rideDbConnectionString, false)
    {
    }

    public RideViewModel(string rideDbConnectionString, bool resumeLastRide)
    {
      _rideDc = new RideDataContext(rideDbConnectionString);
      _resumeLastRide = resumeLastRide;
    }

    public void SaveChanges()
    {
      _rideDc.SubmitChanges();
    }

    public void LoadData()
    {
      if (_resumeLastRide)
      {
        // Get last ride.
        CurrentRide = (from r in _rideDc.Rides orderby r.RideTime descending select r).FirstOrDefault();
      }
      else
      {
        CurrentRide = new Ride { RideDistance = 23.4, RideDurationRaw = 12445 };
        // Attach it to the database...
        _rideDc.Rides.InsertOnSubmit(CurrentRide);
      }

      IsDataLoaded = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
