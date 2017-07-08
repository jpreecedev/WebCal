namespace TachographReader.DataModel.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Xml.Serialization;
    using Connect.Shared;

    [Table("AdvertisingSettings")]
    public class AdvertisingSettings : BaseSettings
    {
        [XmlIgnore, MaxLength]
        public byte[] RawImage { get; set; }

        [XmlIgnore, NotMapped]
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

        public string Message { get; set; }

        [DefaultValue("Arial")]
        public string Font { get; set; }

        [DefaultValue(8)]
        public int FontSize { get; set; }

        public bool IsEnabled { get; set; }
    }
}