namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using DataModel.Core;
    using EventArguments;
    using PCSC;
    using Properties;
    using Shared;
    using Shared.Core;
    using Shared.Helpers;

    public class DriverCardReader : IDriverCardReader
    {
        private bool _isInitialised;
        private SCardMonitor _monitor;
        public EventHandler<DriverCardCompletedEventArgs> Completed { get; set; }
        public EventHandler<DriverCardProgressEventArgs> Progress { get; set; }

        public void FastRead(bool autoRead)
        {
            var error = Initialise();
            if (error != null)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Exception = error,
                    Operation = SmartCardReadOperation.Fast
                });
                return;
            }

            SafelyWithRetry(SmartCardReadOperation.Fast, () =>
            {
                string content = ReadSmartCard("/calibrations");
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
                var calibrationRecord = CalibrationRecordParser.ParseMany(document).OrderByDescending(c => c.CalibrationTime).FirstOrDefault();

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
            var error = Initialise();
            if (error != null)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.History,
                    Exception = error
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
            var error = Initialise();
            if (error != null)
            {
                OnCompleted(new DriverCardCompletedEventArgs
                {
                    Operation = SmartCardReadOperation.Dump,
                    Exception = error
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
                _monitor.Cancel();
                _monitor.Dispose();
            }
        }

        private Exception Initialise()
        {
            if (_isInitialised)
            {
                return null;
            }

            try
            {
                _monitor = new SCardMonitor(new SCardContext(), SCardScope.System);

                var cardReaders = DetectSmartCardReaders();
                if (cardReaders.IsNullOrEmpty())
                {
                    _isInitialised = false;
                    throw new Exception(Resources.EXC_NO_SMART_CARD_READERS_FOUND);
                }

                _monitor.Start(cardReaders);
                _isInitialised = true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_INITIALISE_SMART_CARD_READER, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
                return ex;
            }

            return null;
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

            using (Process proc = Process.Start(processInfo))
            {
                //Must be done before WaitForExit to avoid deadlocks
                string content = proc.StandardOutput.ReadToEnd();

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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    progress(this, new DriverCardProgressEventArgs { Message = message });
                });
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
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    completed(this, args);
                });
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
    }
}