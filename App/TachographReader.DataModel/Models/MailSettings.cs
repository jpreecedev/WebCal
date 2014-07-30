using System.ComponentModel.DataAnnotations;

namespace Webcal.DataModel
{
    public class MailSettings : BaseModel
    {
        public int Id { get; set; }

        public bool AutoEmailCertificates { get; set; }

        public bool PersonaliseMyEmails { get; set; }

        public bool AllowEditingOfEmail
        {
            get
            {
                return (AutoEmailCertificates || PersonaliseMyEmails);
            }
        }

        [MaxLength]
        public string Subject { get; set; }

        [MaxLength]
        public string Body { get; set; }

        [MaxLength]
        public string Content { get; set; }
    }
}
