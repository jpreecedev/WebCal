namespace Webcal.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Core;
    using Shared;

    public class WorkshopSettings : BaseSettings
    {
        private bool _doNotSend;
        private bool _sendToCustomer;
        private bool _sendToOffice;
        public bool AutoBackup { get; set; }
        public string BackupFilePath { get; set; }
        public IList<CustomDayOfWeek> BackupDaysOfWeek { get; set; }
        public string Office { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string PostCode { get; set; }
        public string WorkshopName { get; set; }
        public string PhoneNumber { get; set; }
        public bool AutoPrintLabels { get; set; }
        public string MainEmailAddress { get; set; }
        public string SecondaryEmailAddress { get; set; }

        public bool SendToCustomer
        {
            get { return _sendToCustomer; }
            set
            {
                _sendToCustomer = value;
                if (_sendToCustomer)
                {
                    DoNotSend = false;
                }
            }
        }

        public bool SendToOffice
        {
            get { return _sendToOffice; }
            set
            {
                _sendToOffice = value;
                if (_sendToOffice)
                {
                    DoNotSend = false;
                }
            }
        }

        public bool DoNotSend
        {
            get { return _doNotSend; }
            set
            {
                _doNotSend = value;

                if (_doNotSend)
                {
                    SendToOffice = false;
                    SendToCustomer = false;
                }
            }
        }

        [MaxLength]
        public byte[] RawImage { get; set; }

        [NotMapped]
        public Image Image
        {
            get
            {
                if (RawImage == null)
                {
                    return null;
                }

                return ToImage(RawImage);
            }
            set
            {
                if (value == null)
                {
                    RawImage = null;
                    return;
                }

                RawImage = ToByteArray(value);
            }
        }

        private static Image ToImage(byte[] rawData)
        {
            try
            {
                using (var ms = new MemoryStream(rawData))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
            }

            return null;
        }

        private static byte[] ToByteArray(Image imageIn)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                //Generic GDI+ error ... no point in logging as the error is meaningless
            }

            return null;
        }
    }
}