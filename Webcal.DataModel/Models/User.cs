namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Core;
    using Shared;

    public class User : BaseModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [MaxLength]
        public byte[] RawImage { get; set; }

        [NotMapped]
        public Image Image
        {
            get
            {
                if (RawImage == null)
                    return null;

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