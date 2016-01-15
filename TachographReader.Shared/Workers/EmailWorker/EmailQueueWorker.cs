namespace TachographReader.Shared.Workers.EmailWorker
{
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using Properties;

    public class EmailQueueWorker : BaseWorker
    {
        public override void Start(IWorkerParameters parameters)
        {
            var emailParameters = new EmailParameters(parameters);

            if (emailParameters.PersonaliseMyEmails)
            {
                SendViaMAPIMessage(emailParameters);
            }
            else
            {
                SendViaSMTPClient(emailParameters);
            }
        }

        private void SendViaMAPIMessage(EmailParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Recipient))
            {
                return;
            }

            var mapiMailMessage = new MapiMailMessage
            {
                Subject = parameters.Subject,
                Body = parameters.Body
            };

            mapiMailMessage.Recipients.Add(parameters.Recipient);

            if (!string.IsNullOrEmpty(parameters.AttachmentPath))
            {
                mapiMailMessage.Files.Add(parameters.AttachmentPath);
            }

            mapiMailMessage.ShowDialog();
        }

        private void SendViaSMTPClient(EmailParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Recipient))
            {
                return;
            }

            using (var mailMessage = new MailMessage())
            {
                mailMessage.To.Add(parameters.Recipient);
                mailMessage.Subject = parameters.Subject;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = parameters.Body;
                mailMessage.From = new MailAddress("webcal@tachoworkshop.co.uk");
                if (!string.IsNullOrEmpty(parameters.AttachmentPath) && File.Exists(parameters.AttachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(parameters.AttachmentPath));
                }

                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential("admin@webcalconnect.com", "No05K8lGgB");
                    smtp.Host = "mail.webcalconnect.com";
                    smtp.Send(mailMessage);
                }
            }
        }
    }
}