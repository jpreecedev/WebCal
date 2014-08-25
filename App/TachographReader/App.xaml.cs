namespace TachographReader
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;
    using StructureMap;
    using Webcal.DataModel.Core;
    using Webcal.DataModel.Library;
    using Webcal.Library;
    using Webcal.Shared;
    using Webcal.Windows;
    using Webcal.Windows.LogInWindow;

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
                _isBuild = true;

            var splashScreen = new SplashScreen("Images/splash.png");
            splashScreen.Show(false);

            if (!InitialiseApplication())
                return;

            splashScreen.Close(new TimeSpan(0, 0, 0));

            if (!_isBuild)
            {
                CheckForUpdates();

                var window = new LogInWindow();
                if (window.ShowDialog() == true)
                {
                    ShutdownMode = ShutdownMode.OnMainWindowClose;
                    MainWindow = new MainWindow();
                    SmartCardMonitor.Instance.MainWindowViewModel = (MainWindowViewModel) MainWindow.DataContext;

                    MainWindow.ShowDialog();
                }
                else
                    Current.Shutdown();
            }
            else
                Current.Shutdown();
        }

        private static bool InitialiseApplication()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

            if (!InitialiseObjectFactory())
                return false;

            //Resource dictionaries must be set in code to avoid issues with older operating systems
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri("Resources/MainResourceDictionary.xaml", UriKind.Relative)});
            Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri(string.Format("pack://application:,,,/Fluent;Component/Themes/Office2010/{0}.xaml", "Silver"))});

            //Seed database
            IGeneralSettingsRepository generalSettings = SeedDataHelper.SeedDatabase();

            //Back up the database, if needed
            BackupRestoreManager.BackupIfRequired(generalSettings.GetSettings());

            return true;
        }

        private static bool InitialiseObjectFactory()
        {
            bool canContinue = false;

            ObjectFactory.Configure(x => canContinue = ContainerBootstrapper.Configure(x, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\webcal.sdf"));

            return canContinue;
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
            MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Webcal.Properties.Resources.EXC_UNHANDLED_EXCEPTION, ExceptionPolicy.HandleException(e.Exception)));
            e.Handled = true;
        }
    }
}