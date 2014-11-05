namespace Webcal.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public static class CalibrationRecordParser
    {
        public static ICollection<CalibrationRecord> ParseMany(XContainer container)
        {
            ICollection<CalibrationRecord> result = new List<CalibrationRecord>();

            foreach (XElement element in container.Descendants("CalibrationRecord"))
            {
                try
                {
                    result.Add(Parse(element));
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
                }
            }

            return result;
        }

        public static CalibrationRecord Parse(XContainer xContainer)
        {
            var element = xContainer.Descendants("CalibrationRecord").FirstOrDefault();

            if (element == null)
            {
                return null;
            }

            return Parse(element);
        }

        public static CalibrationRecord Parse(XElement element)
        {
            if (element == null)
            {
                return null;
            }

            var calibrationRecord = new CalibrationRecord
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

            if (calibrationRecord.OdometerValue == "16777215")
            {
                calibrationRecord.OdometerValue = string.Empty;
            }

            return calibrationRecord;
        }
    }
}