namespace Webcal.DataModel
{
    using System.ComponentModel.DataAnnotations;
    using Shared;
    using Shared.Core;

    public class MailSettings : BaseSettings
    {
        public bool AutoEmailCertificates { get; set; }
        public bool PersonaliseMyEmails { get; set; }

        public bool AllowEditingOfEmail
        {
            get { return (AutoEmailCertificates || PersonaliseMyEmails); }
        }

        public bool DontSendEmails
        {
            get
            {
                return !AllowEditingOfEmail &&
                       !AutoEmailCertificates &&
                       !PersonaliseMyEmails;
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