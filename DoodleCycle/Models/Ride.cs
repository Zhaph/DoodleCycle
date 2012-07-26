using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DoodleCycle.Models
{
  [Table]
  public class Ride: INotifyPropertyChanged, INotifyPropertyChanging
  {
    private int _rideId;

    /// <summary>
    /// Gets or sets the ride id.
    /// </summary>
    /// <value>
    /// The ride id.
    /// </value>
    [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType= "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
    public int RideId
    { 
      get { return _rideId; }
      set
      {
        if (_rideId != value)
        {
          OnPropertyChanging("RideId");
          _rideId = value;
        }
      } 
    }

    
    /// <summary>
    /// Gets or sets the date and time of the ride.
    /// </summary>
    /// <value>
    /// The ride date.
    /// </value>
    [Column()]
    public DateTime RideTime {get; set;}

    [Column]
    public int RideDurationRaw { get; set; }

    [Column (DbType = "FLOAT")]
    public double RideLength { get; set; }

    // Version column aids update performance.
    [Column(IsVersion = true)]
    private Binary _version;

    public event PropertyChangedEventHandler PropertyChanged;

    public event PropertyChangingEventHandler PropertyChanging;

    // Used to notify that a property is about to change
    private void OnPropertyChanging(string propertyName)
    {
      if (PropertyChanging != null)
      {
        PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
      }
    }
  }


  public class RideDataContext: DataContext
  {
    public RideDataContext(string connectionString): base(connectionString)
    { }

    public Table<Ride> Rides;
  }
}
