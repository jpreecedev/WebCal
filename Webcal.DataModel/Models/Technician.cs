namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Core;
    using Properties;
    using Shared;
    using Shared.Core;

    public class Technician : BaseModel, IUserSignatureCapture
    {
        public string Name { get; set; }
        public string Number { get; set; }

        public bool IsDefault { get; set; }

        public bool HasSignature
        {
            get { return Image != null; }
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

        public override string ToString()
        {
            if (IsDefault)
            {
                return string.Format(Resources.TXT_TECHNICIAN_DEFAULT, Name);
            }

            return Name;
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
    }
}