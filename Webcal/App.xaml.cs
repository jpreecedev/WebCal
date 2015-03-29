namespace TachographReader
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
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
    using Shared.Helpers;

    public partial class App
    {
        public App()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Current.Startup += Current_Startup;
        }

        private void Current_Startup(object sender, StartupEventArgs e)
        {
            var splashScreen = new SplashScreen(LocalizationHelper.GetResourceManager().GetString("TXT_SPLASH_SCREEN_PATH"));
            splashScreen.Show(false);

            if (!InitialiseApplication())
            {
                return;
            }

            splashScreen.Close(new TimeSpan(0, 0, 0));

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

        private static bool InitialiseApplication()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments));

            //Resource dictionaries must be set in code to avoid issues with older operating systems
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/MainResourceDictionary.xaml", UriKind.Relative) });
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
            MigrationHelper.ApplyDataHacks();

            //Back up the database, if needed
            BackupRestoreManager.BackupIfRequired(generalSettings.GetWorkshopSettings());

            //Apply theme
            ThemeSettings themeSettings = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<ThemeSettings>>().GetThemeSettings();
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = themeSettings.Source });

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
            MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", TachographReader.Properties.Resources.EXC_UNHANDLED_EXCEPTION, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, e.Exception)));
            e.Handled = true;
        }
    }
}