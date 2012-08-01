using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Device.Location;
using System.Globalization;
using PropertyChanged;

namespace DoodleCycle.Models
{
  [Table]
  public class Ride : INotifyPropertyChanged, INotifyPropertyChanging
  {
    // Version column aids update performance.
    [Column(IsVersion = true)]
    private Binary _version;

    private GeoCoordinate _lastPosition;

    /// <summary>
    /// Gets or sets the ride id.
    /// </summary>
    /// <value>
    /// The ride id.
    /// </value>
    [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false,
      AutoSync = AutoSync.OnInsert)]
    public int RideId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the ride.
    /// </summary>
    /// <value>
    /// The ride date.
    /// </value>
    [Column]
    public DateTime RideStartTime { get; set; }

    [Column]
    public int RideDurationRaw { get; set; }

    [Column(DbType = "FLOAT")]
    public double RideDistance { get; set; }

    [Column(DbType = "FLOAT")]
    public double AltitudeChange { get; set; }

    [DependsOn("LastPosition")]
    [Column(DbType = "FLOAT")]
    public double LastLatitude { get; set; }

    [DependsOn("LastPosition")]
    [Column(DbType = "FLOAT")]
    public double LastLongitude { get; set; }

    [DependsOn("LastPosition")]
    [Column(DbType = "FLOAT")]
    public double LastAltitude { get; set; }

    public GeoCoordinate LastPosition
    {
      get
      {
        if (null == _lastPosition && LastLatitude > 0.0)
        {
          _lastPosition = new GeoCoordinate(LastLatitude, LastLongitude, LastAltitude);
        }

        return _lastPosition;
      }
      set { _lastPosition = value; }
    }

    [DependsOn("RideDurationRaw")]
    public TimeSpan RideDuration
    {
      get { return new TimeSpan(0, 0, 0, RideDurationRaw); }
    }

    [DependsOn("RideDurationRaw", "RideDistance")]
    public double AverageSpeed
    {
      get
      {
        double distance = Classes.DoubleToDistanceStringConverter.LongDistance(RideDistance, App.AppSettings.MainUnits);

        return RideDurationRaw > 0.0 ? distance / RideDuration.TotalHours : 0.0;
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region INotifyPropertyChanging Members

    public event PropertyChangingEventHandler PropertyChanging;

    #endregion
  }

  public class RideDataContext : DataContext
  {
    public Table<Ride> Rides;

    public RideDataContext(string connectionString) : base(connectionString)
    {
    }
  }
}