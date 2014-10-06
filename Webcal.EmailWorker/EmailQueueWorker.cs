namespace Webcal.EmailWorker
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using Shared.Workers;

    public class EmailQueueWorker : BaseWorker
    {
        public EmailQueueWorker(Action<string> sendMessage)
            : base(sendMessage)
        {
        }

        public override void Start(IWorkerParameters parameters)
        {
            Debugger.Launch();
            Debugger.Break();

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
            if (string.IsNullOrEmpty(parameters.Recipient)) return;

            SendMessage("Sending via MAPI");
            var mapiMailMessage = new MapiMailMessage
            {
                Subject = parameters.Subject,
                Body = parameters.Body
            };

            mapiMailMessage.Recipients.Add(parameters.Recipient);

            if (!string.IsNullOrEmpty(parameters.AttachmentPath))
                mapiMailMessage.Files.Add(parameters.AttachmentPath);

            mapiMailMessage.ShowDialog();
        }

        private void SendViaSMTPClient(EmailParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.Recipient)) return;

            SendMessage("Sending via SMTP");
            using (var mailMessage = new MailMessage())
            {
                mailMessage.To.Add(parameters.Recipient);
                mailMessage.Subject = parameters.Subject;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = parameters.Body;
                mailMessage.From = new MailAddress("webcal@tachoworkshop.co.uk");
                if (!string.IsNullOrEmpty(parameters.AttachmentPath) && File.Exists(parameters.AttachmentPath))
                    mailMessage.Attachments.Add(new Attachment(parameters.AttachmentPath));

                using (var smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential("webcal@tachoworkshop.co.uk", "skillraywebcal");
                    smtp.Host = "mail.yellowbus.co.uk";
                    smtp.Send(mailMessage);
                }
            }
        }
    }
}