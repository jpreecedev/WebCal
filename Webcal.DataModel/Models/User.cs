namespace Webcal.DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using Connect.Shared.Models;
    using Shared;
    using Shared.Core;
    using Shared.Helpers;

    public class User : BaseModel, IUserSignatureCapture
    {
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
                {
                    return null;
                }

                return RawImage.ToImage();
            }
            set
            {
                if (value == null)
                {
                    RawImage = null;
                    return;
                }

                RawImage = value.ToByteArray();
            }
        }
    }
}