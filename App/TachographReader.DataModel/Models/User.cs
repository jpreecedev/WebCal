using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Webcal.Shared;

namespace Webcal.DataModel
{
    public class User : BaseModel
    {
        #region Public Properties

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

        #endregion

        #region Private Methods

        private static byte[] ToByteArray(Image imageIn)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
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
                using (MemoryStream ms = new MemoryStream(rawData))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex);
            }

            return null;
        }

        #endregion
    }
}
