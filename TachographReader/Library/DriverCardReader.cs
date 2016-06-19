namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Xml.Linq;
    using DataModel.Core;
    using EventArguments;
    using PCSC;
    using Properties;
    using Shared;
    using Shared.Core;

    public class DriverCardReader : IDriverCardReader
    {
        private readonly SCardMonitor _monitor;
        public EventHandler<DriverCardCompletedEventArgs> Completed { get; set; }
        public EventHandler<DriverCardProgressEventArgs> Progress { get; set; }
        public EventHandler<EventArgs> CardInserted { get; set; }
        public EventHandler<EventArgs> CardRemoved { get; set; }

        public DriverCardReader()
        {
            try
            {
                _monitor = new SCardMonitor(ContextFactory.Instance, SCardScope.System);
                _monitor.CardInserted += Monitor_CardInserted;
                _monitor.CardRemoved += Monitor_CardRemoved;

                var cardReaders = DetectSmartCardReaders();
                if (cardReaders.IsNullOrEmpty())
                {
                    return;
                }

                _monitor.Start(cardReaders);
            }
            catch (Exception)
            {

            }
        }

        public void FastRead(bool autoRead)
        {
            if (_monitor == null || !_monitor.Monitoring)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Exception = new Exception(Resources.EXC_NO_SMART_CARD_READERS_FOUND),
                    Operation = SmartCardReadOperation.Fast
                });
                return;
            }

            SafelyWithRetry(SmartCardReadOperation.Fast, () =>
            {
                string content = ReadSmartCard(string.Empty);
                if (string.IsNullOrEmpty(content))
                {
                    OnCompleted(new DriverCardCompletedEventArgs
                    {
                        AutoRead = autoRead,
                        Operation = SmartCardReadOperation.Fast,
                        Exception = new Exception(Resources.EXC_UNABLE_READ_SMART_CARD)
                    });
                    return;
                }

                XDocument document = XDocument.Parse(content);
                var calibrationRecord = CalibrationRecordParser.Parse(document);

                OnCompleted(new DriverCardCompletedEventArgs
                {
                    AutoRead = autoRead,
                    Operation = SmartCardReadOperation.Fast,
                    CalibrationRecord = calibrationRecord
                });
            });
        }

        public void GetFullHistory()
        {
            if (_monitor == null || !_monitor.Monitoring)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.History,
                    Exception = new Exception(Resources.EXC_NO_SMART_CARD_READERS_FOUND)
                });
                return;
            }

            SafelyWithRetry(SmartCardReadOperation.History, () =>
            {
                string xml = ReadSmartCard("/calibrations");
                if (string.IsNullOrEmpty(xml))
                {
                    OnCompleted(new DriverCardCompletedEventArgs
                    {
                        Operation = SmartCardReadOperation.History
                    });
                    return;
                }

                XDocument document = XDocument.Parse(xml);
                var history = CalibrationRecordParser.ParseMany(document);

                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.History,
                    CalibrationHistory = history
                });
            });
        }

        public void GenerateDump()
        {
            if (_monitor == null || !_monitor.Monitoring)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.Dump,
                    Exception = new Exception(Resources.EXC_NO_SMART_CARD_READERS_FOUND)
                });
                return;
            }

            SafelyWithRetry(SmartCardReadOperation.Dump, () =>
            {
                string xml = ReadSmartCard("/dump");
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.Dump,
                    DumpFilePath = xml
                });
            });
        }

        public void Dispose()
        {
            if (_monitor != null)
            {
                _monitor.CardInserted -= Monitor_CardInserted;
                _monitor.CardRemoved -= Monitor_CardRemoved;
                _monitor.Cancel();
                _monitor.Dispose();
            }
        }

        private static string ReadSmartCard(string arguments)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\SmartCardReader.jar";

            var processInfo = new ProcessStartInfo("java.exe", string.Format("-jar \"{1}\" {0}", arguments, path))
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var proc = Process.Start(processInfo))
            {
                //Must be done before WaitForExit to avoid deadlocks
                var content = proc.StandardOutput.ReadToEnd();

                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    return content;
                }
            }

            return string.Empty;
        }

        private static string[] DetectSmartCardReaders()
        {
            List<string> readers = null;

            try
            {
                using (var context = new SCardContext())
                {
                    context.Establish(SCardScope.System);
                    readers = context.GetReaders().ToList();
                    context.Release();
                }
            }
            catch
            {
                //Simply no smart card readers detected
            }

            return readers == null ? null : readers.ToArray();
        }

        private void OnProgress(string message)
        {
            var progress = Progress;
            if (progress != null)
            {
                Application.Current.Dispatcher.Invoke(() => { progress(this, new DriverCardProgressEventArgs { Message = message }); });
            }
        }

        private void SafelyWithRetry(SmartCardReadOperation operation, Action action)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var resilient = new Resilient(OnProgress);
                    resilient.ExecuteWithRetry(action);
                })
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            HandleException(operation, t.Exception);
                        }
                    });
            }
            catch (Exception ex)
            {
                HandleException(operation, ex);
            }
        }

        private void OnCompleted(DriverCardCompletedEventArgs args)
        {
            var completed = Completed;
            if (completed != null)
            {
                Application.Current.Dispatcher.Invoke(() => { completed(this, args); });
            }
        }

        private void HandleException(SmartCardReadOperation operation, Exception exception)
        {
            ExceptionPolicy.HandleException(ContainerBootstrapper.Container, exception);
            OnCompleted(new DriverCardCompletedEventArgs
            {
                Exception = exception,
                Operation = operation
            });
        }

        private void Monitor_CardRemoved(object sender, CardStatusEventArgs e)
        {
            if (CardRemoved != null)
            {
                CardRemoved(this, new EventArgs());
            }
        }

        private void Monitor_CardInserted(object sender, CardStatusEventArgs e)
        {
            if (CardInserted != null)
            {
                CardInserted(this, new EventArgs());
            }
        }
    }
}