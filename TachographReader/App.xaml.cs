namespace TachographReader
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Windows;
    using Windows.LogInWindow;
    using Windows.ReprintWindow;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using DataModel.Migrations;
    using Library;
    using Microsoft.Shell;
    using Shared;
    using Shared.Helpers;

    public partial class App : ISingleInstanceApp
    {
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (args == null || args.Count < 2)
            {
                return true;
            }

            try
            {
                var arg = args[1];
                var protocolSplit = arg.Split(':');
                var parameters = protocolSplit[1];
                var parametersSplit = parameters.Split(',');
                var command = parametersSplit[0];
                var parameter = parametersSplit[1];

                switch (command)
                {
                    case "reprintlabel":

                        var window = new ReprintWindow
                        {
                            DataContext = new ReprintWindowViewModel
                            {
                                ReprintMode = ReprintMode.Label,
                                RegistrationNumber = parameter
                            }
                        };

                        var dataContext = (ReprintWindowViewModel)window.DataContext;
                        dataContext.ReprintCommand.Execute(window);

                        break;
                }

            }
            catch (Exception)
            {

            }

            return true;
        }

        public void Boot()
        {
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Current.Startup += Current_Startup;
        }

        private async void Current_Startup(object sender, StartupEventArgs e)
        {
            var startupArguments = new StartupArguments();
            startupArguments.Parse(e.Args);

            if (!string.IsNullOrEmpty(startupArguments.Culture))
            {
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(startupArguments.Culture);
            }

            var splashScreen = new SplashScreen(LocalizationHelper.GetResourceManager().GetString("TXT_SPLASH_SCREEN_PATH"));
            splashScreen.Show(false);

            CheckForUpdates();

            await AsyncHelper.CallSync(InitialiseApplication);

            splashScreen.Close(new TimeSpan(0, 0, 0));

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
            
            MigrationHelper.MoveDatabaseIfRequired();
            MigrationHelper.HackMigrationHistoryTable();

            //Prepare database
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TachographContext, Configuration>());

            MigrationHelper.MigrateWorkshopImages();

            TachographContext context = new TachographContext();
            context.Database.CreateIfNotExists();

            //Make database accessible to all users
            MigrationHelper.SetDatabasePermissions();

            //Seed database
            MigrationHelper.ApplyDataHacks();
            
            //Apply theme
            ThemeSettings themeSettings = ContainerBootstrapper.Resolve<ISettingsRepository<ThemeSettings>>().GetThemeSettings();
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = themeSettings.Source });

            //Launch Webcal Protocol
            RegistryHelper.RegisterProtocol();

            //Initialise worker queue, which will start checking for unprocessed tasks
            WorkerHelper.Initialize();

            return true;
        }

        private static void CheckForUpdates()
        {
            try
            {
                Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "updater.exe"));
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