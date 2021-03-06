﻿using System.Windows;
using System.Windows.Navigation;
using DoodleCycle.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using NorthernLights;

namespace DoodleCycle
{
  public partial class App : Application
  {
    private static readonly string _connectionString = "Data Source=isostore:/DoodleCycle.sdf";
    private static AppSettings _appSettings;
    // The current version of the application.
    internal static int AppVersion = 1;

    private static LicenseInformation _licenseInfo = new LicenseInformation();

    public static bool IsTrial { get; private set; }

    public static AppSettings AppSettings
    {
      get { return _appSettings; }
    }

    /// <summary>
    /// Provides easy access to the root frame of the Phone Application.
    /// </summary>
    /// <returns>The root frame of the Phone Application.</returns>
    public PhoneApplicationFrame RootFrame { get; private set; }

    internal static string ConnectionString
    {
      get { return _connectionString; }
    }

    /// <summary>
    /// Constructor for the Application object.
    /// </summary>
    public App()
    {
      // Global handler for uncaught exceptions. 
      UnhandledException += Application_UnhandledException;

      // Standard Silverlight initialization
      InitializeComponent();

      // Phone-specific initialization
      InitializePhoneApplication();

      var pas = PhoneApplicationService.Current;

      // Show graphics profiling information while debugging.
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // Display the current frame rate counters.
        Current.Host.Settings.EnableFrameRateCounter = true;

        // Display the metro grid helper.
        MetroGridHelper.IsVisible = true;

        // Show the areas of the app that are being redrawn in each frame.
        //Application.Current.Host.Settings.EnableRedrawRegions = true;

        // Enable non-production analysis visualization mode, 
        // which shows areas of a page that are handed off to GPU with a coloured overlay.
        //Application.Current.Host.Settings.EnableCacheVisualization = true;

        // Disable the application idle detection by setting the UserIdleDetectionMode property of the
        // application's PhoneApplicationService object to Disabled.
        // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
        // and consume battery power when the user is not using the phone.
        pas.UserIdleDetectionMode = IdleDetectionMode.Disabled;
      }

      using (var db = new RideDataContext(ConnectionString))
      {
        if (!db.DatabaseExists())
        {
          // Create the local database
          db.CreateDatabase();

          // Set the new database version.
          DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();
          dbUpdater.DatabaseSchemaVersion = AppVersion;
          dbUpdater.Execute();
        }
        else
        {
          // Check whether a database update is needed.
          DatabaseSchemaUpdater dbUpdater = db.CreateDatabaseSchemaUpdater();

          if (dbUpdater.DatabaseSchemaVersion < AppVersion)
          {
            // Add the Priority column (added in version 2).
            //dbUpdater.AddColumn<ToDoItem>("Priority");

            // Add the new database version.
            //dbUpdater.DatabaseSchemaVersion = AppVersion;

            // Perform the database update in a single transaction.
            //dbUpdater.Execute();
          }
        }
      }

      _appSettings = new AppSettings();

      // Check that Idle Detection Mode matches user settings...
      if (_appSettings.RunUnderLockScreen && pas.ApplicationIdleDetectionMode == IdleDetectionMode.Enabled)
      {
        // User has allowed us to run under lock screen, but App Idle Detection is still enabled:
        pas.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
      }
      else if (!_appSettings.RunUnderLockScreen && pas.ApplicationIdleDetectionMode == IdleDetectionMode.Disabled)
      {
        // User hasn't allowed us to run under lock screen, but App Idle Detection has been disabled:
        pas.ApplicationIdleDetectionMode = IdleDetectionMode.Enabled;
      }

      if (!_appSettings.AllowAnonymousReporting)
      {
        LittleWatson.Instance.AllowAnonymousHttpReporting = false;
      }
    }

    // Code to execute when the application is launching (eg, from Start)
    // This code will not execute when the application is reactivated
    private void Application_Launching(object sender, LaunchingEventArgs e)
    {
      checkLicense();
    }

    // Code to execute when the application is activated (brought to foreground)
    // This code will not execute when the application is first launched
    private void Application_Activated(object sender, ActivatedEventArgs e)
    {
      // Ensure that application state is restored appropriately
      checkLicense();
    }

    // Code to execute when the application is deactivated (sent to background)
    // This code will not execute when the application is closing
    private void Application_Deactivated(object sender, DeactivatedEventArgs e)
    {
      // Ensure that required application state is persisted here.
    }

    // Code to execute when the application is closing (eg, user hit Back)
    // This code will not execute when the application is deactivated
    private void Application_Closing(object sender, ClosingEventArgs e)
    {
    }

    // Code to execute if a navigation fails
    private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // A navigation has failed; break into the debugger
        System.Diagnostics.Debugger.Break();
      }

      // Log it for later
      LittleWatson.SaveExceptionForReporting(e.Exception);
    }

    // Code to execute on Unhandled Exceptions
    private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
      if (System.Diagnostics.Debugger.IsAttached)
      {
        // An unhandled exception has occurred; break into the debugger
        System.Diagnostics.Debugger.Break();
      }

      // Log it for later
      LittleWatson.SaveExceptionForReporting(e.ExceptionObject);
    }

    /// <summary>
    /// Check the current license information for this application
    /// </summary>
    private void checkLicense()
    {
      // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
      // property to simulate trial mode being on or off. 
#if DEBUG
      string message = "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
      IsTrial = MessageBox.Show(message, "Debug Trial",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK;
#else
            IsTrial = _licenseInfo.IsTrial();
#endif
    }

    #region Phone application initialization

    // Avoid double-initialization
    private bool phoneApplicationInitialized = false;

    // Do not add any additional code to this method
    private void InitializePhoneApplication()
    {
      if (phoneApplicationInitialized)
        return;

      // Create the frame but don't set it as RootVisual yet; this allows the splash
      // screen to remain active until the application is ready to render.
      //RootFrame = new PhoneApplicationFrame();
      RootFrame = new TransitionFrame();
      RootFrame.Navigated += CompleteInitializePhoneApplication;

      // Handle navigation failures
      RootFrame.NavigationFailed += RootFrame_NavigationFailed;

      // Ensure we don't initialize again
      phoneApplicationInitialized = true;
    }

    // Do not add any additional code to this method
    private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
    {
      // Set the root visual to allow the application to render
      if (RootVisual != RootFrame)
        RootVisual = RootFrame;

      // Remove this handler since it is no longer needed
      RootFrame.Navigated -= CompleteInitializePhoneApplication;
    }

    #endregion
  }
}