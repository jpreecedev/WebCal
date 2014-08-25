namespace Webcal.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Linq;
    using Windows;
    using DataModel.Core;
    using DataModel.Library;
    using PCSC;
    using Properties;
    using Shared;

    public sealed class SmartCardMonitor : IDisposable
    {
        private static SmartCardMonitor _instance;
        private SCardMonitor _monitor;
        private List<string> _readers;

        public MainWindowViewModel MainWindowViewModel { get; set; }

        public static SmartCardMonitor Instance
        {
            get { return _instance ?? (_instance = new SmartCardMonitor()); }
        }

        public void Dispose()
        {
            if (_monitor != null)
            {
                _monitor.Cancel();
                _monitor.Dispose();
            }
        }

        public CalibrationRecord Refresh()
        {
            _readers = GetReaders();
            if (_readers.IsNullOrEmpty())
                return null;

            if (_monitor == null)
            {
                _monitor = new SCardMonitor(new SCardContext(), SCardScope.System);
                _monitor.Initialized += OnInitialised;
                _monitor.Start(_readers.ToArray());
            }

            string content = ReadSmartCard(string.Empty);
            if (string.IsNullOrEmpty(content))
                return null;

            XDocument document = XDocument.Parse(content);

            return ReadCalibrationRecord(document.Descendants("CalibrationRecord").FirstOrDefault());
        }

        public List<CalibrationRecord> GetCalibrationHistory()
        {
            try
            {
                string xml = ReadSmartCard("/calibrations");
                if (string.IsNullOrEmpty(xml))
                    return null;

                var result = new List<CalibrationRecord>();
                XDocument document = XDocument.Parse(xml);

                foreach (XElement element in document.Descendants("CalibrationRecord"))
                {
                    try
                    {
                        result.Add(ReadCalibrationRecord(element));
                    }
                    catch (Exception ex)
                    {
                        //Something very wrong, keep it quiet tho
                        ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
            }

            return null;
        }

        public string GetCardDump()
        {
            try
            {
                return ReadSmartCard("/dump");
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
            }

            return null;
        }
        
        private void OnInitialised(object sender, CardStatusEventArgs e)
        {
            if (e.State == (SCRState.Present | SCRState.Unpowered))
                ReadSmartCard(string.Empty);
        }

        private string ReadSmartCard(string arguments)
        {
            try
            {
                var processInfo = new ProcessStartInfo("java.exe", string.Format("-jar Resources\\SmartCardReader.jar {0}", arguments))
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };

                using (Process proc = Process.Start(processInfo))
                {
                    //Must be done before WaitForExit to avoid deadlocks
                    string content = proc.StandardOutput.ReadToEnd();

                    proc.WaitForExit();

                    if (proc.ExitCode == 0)
                        return content;
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_READ_SMART_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
                UnlockMainWindow();
            }

            return string.Empty;
        }

        private static List<string> GetReaders()
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

            return readers;
        }

        private static CalibrationRecord ReadCalibrationRecord(XContainer element)
        {
            var cr = new CalibrationRecord
            {
                CalibrationTime = element.Element("CalibrationTime").SafelyGetValueAsDateTime(),
                MaxSpeed = element.Element("MaxSpeed").SafelyGetValueAsDouble(),
                NextCalibrationDate = element.Element("NextCalibrationDate").SafelyGetValueAsDateTime(),
                OdometerValue = element.Element("OdometerValue").SafelyGetValue(),
                Purpose = element.Element("Purpose").SafelyGetValue(),
                SensorSerialNumber = element.Element("SensorSerialNumber").SafelyGetValue(),
                TyreSize = element.Element("TyreSize").SafelyGetValue(),
                TyreCircumference = element.Element("TyreCircumference").SafelyGetValue(),
                VehicleIdentificationNumber = element.Element("VehicleIdentificationNumber").SafelyGetValue(),
                VehicleRegistrationNation = element.Element("VehicleRegistrationNation").SafelyGetValue(),
                VehicleRegistrationNumber = element.Element("VehicleRegistrationNumber").SafelyGetValue(),
                VuPartNumber = element.Element("VuPartNumber").SafelyGetValue(),
                VuSerialNumber = element.Element("VuSerialNumber").SafelyGetValue(),
                WFactor = element.Element("WFactor").SafelyGetValue(),
                KFactor = element.Element("KFactor").SafelyGetValue(),
                TachographManufacturer = element.Element("VuManufacturer").SafelyGetValue(),
                CardSerialNumber = element.Element("CardSerialNumber").SafelyGetValue()
            };
            if (cr.OdometerValue == "16777215")
                cr.OdometerValue = "";
            return cr;
        }

        private void UnlockMainWindow()
        {
            MainWindowViewModel.IsNavigationLocked = false;
        }
    }
}