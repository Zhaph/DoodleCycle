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
  public class AppViewModel: INotifyPropertyChanged
  {
    private readonly RideDataContext _rideDc;

    public bool IsDataLoaded { get; private set; }

    public AppViewModel(string rideDBConnectionString)
    {
      _rideDc = new RideDataContext(rideDBConnectionString);
    }

    public void SaveChangesToDB()
    {
      _rideDc.SubmitChanges();
    }

    public Ride LastRide { get; private set; }

    public Ride SummaryRide { get; private set; }

    public void LoadData()
    {
      // Get last ride.
      LastRide = (from r in _rideDc.Rides orderby r.RideTime descending select r).FirstOrDefault() ??
                 new Ride {RideDistance = 23.4, RideDurationRaw = 12445};

      // Summary Details
      if ((from r in _rideDc.Rides select r).Any())
      {
        double? totalDistance = (from r in _rideDc.Rides select r.RideDistance).Sum();

        int? totalTime = (from r in _rideDc.Rides select r.RideDurationRaw).Sum();

        SummaryRide = new Ride
                        {
                          RideDistance = (totalDistance.HasValue ? totalDistance.Value : 0.0),
                          RideDurationRaw = (totalTime.HasValue ? totalTime.Value : 0)
                        };
      }
      else
      {
        SummaryRide = new Ride { RideDistance = 140.5, RideDurationRaw = 12224445 };
      }
      IsDataLoaded = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
