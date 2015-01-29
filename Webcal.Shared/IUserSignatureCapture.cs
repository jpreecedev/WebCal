namespace Webcal.Shared
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;

    public interface IUserSignatureCapture
    {
        [MaxLength]
        byte[] RawImage { get; set; }

        [NotMapped]
        Image Image { get; set; }
    }
}