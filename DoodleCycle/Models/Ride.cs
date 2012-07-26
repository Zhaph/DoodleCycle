using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace DoodleCycle.Models
{
  [Table]
  public class Ride : INotifyPropertyChanged, INotifyPropertyChanging
  {
    // Version column aids update performance.
    [Column(IsVersion = true)]
    private Binary _version;

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
    public DateTime RideTime { get; set; }

    [Column]
    public int RideDurationRaw { get; set; }

    [Column(DbType = "FLOAT")]
    public double RideDistance { get; set; }

    public TimeSpan RideDuration
    {
      get { return new TimeSpan(0, 0, 0, RideDurationRaw); }
    }

    public double AverageSpeed
    {
      get
      {
        return RideDurationRaw > 0.0 ? RideDistance/RideDuration.TotalHours : 0.0;
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