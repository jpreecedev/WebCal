namespace Webcal
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;
    using Windows;
    using Windows.LogInWindow;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using DataModel.Migrations;
    using Library;
    using Shared;

    public partial class App
    {
        private bool _isBuild;

        public App()
        {
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Current.Startup += Current_Startup;
        }

        private void Current_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Any(a => a.Contains("build")))
            {
                _isBuild = true;
            }

            var splashScreen = new SplashScreen("Images/splash.png");
            splashScreen.Show(false);

            if (!InitialiseApplication())
            {
                return;
            }

            splashScreen.Close(new TimeSpan(0, 0, 0));

            if (!_isBuild)
            {
                CheckForUpdates();

                var window = new LogInWindow();
                if (window.ShowDialog() == true)
                {
                    ShutdownMode = ShutdownMode.OnMainWindowClose;
                    MainWindow = new MainWindow();

                    MainWindow.ShowDialog();
                }
                else
                {
                    Current.Shutdown();
                }
            }
            else
            {
                Current.Shutdown();
            }
        }

        private static bool InitialiseApplication()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

            //Resource dictionaries must be set in code to avoid issues with older operating systems
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri("Resources/MainResourceDictionary.xaml", UriKind.Relative)});
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Generic.xaml") });

            //Prepare database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TachographContext, Configuration>());

            TachographContext context = new TachographContext();
            context.Database.CreateIfNotExists();

            //Make database accessible to all users
            MigrationHelper.SetDatabasePermissions();

            //Seed database
            ISettingsRepository<WorkshopSettings> generalSettings = SeedDataHelper.SeedDatabase();
            MigrationHelper.MigrateIfRequired();

            //Back up the database, if needed
            BackupRestoreManager.BackupIfRequired(generalSettings.GetWorkshopSettings());

            return true;
        }

        private static void CheckForUpdates()
        {
            try
            {
                Process.Start("AutoUpdater.exe", "/silentall");
            }
            catch
            {
            }
        }

        private static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Webcal.Properties.Resources.EXC_UNHANDLED_EXCEPTION, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, e.Exception)));
            e.Handled = true;
        }
    }
}