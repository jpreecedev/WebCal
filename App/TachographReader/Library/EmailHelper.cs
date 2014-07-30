using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.AccessControl;
using StructureMap;
using Webcal.DataModel;
using Webcal.Library.MAPI;
using Webcal.Shared;

namespace Webcal.Library
{
    public static class EmailHelper
    {
        public static void SendEmail(Document document, string attachmentPath)
        {
            if (document == null) return;
            if (document.CustomerContact == null || string.IsNullOrEmpty(document.CustomerContact)) return;

            IMailSettingsRepository mailRepository = ObjectFactory.GetInstance<IMailSettingsRepository>();
            MailSettings settings = mailRepository.GetSettings();

            if (!settings.AutoEmailCertificates && !settings.PersonaliseMyEmails)
                return;

            string recipient = GetRecipientEmailAddress(document.CustomerContact);
            if (string.IsNullOrEmpty(recipient))
                return;

            string subject = MacroHelper.FindAndReplace(settings.Subject, document);
            string body = MacroHelper.FindAndReplace(settings.Body, document);

            if (settings.PersonaliseMyEmails)
            {
                SendViaMAPIMessage(attachmentPath, subject, body, recipient);
            }
            else
            {
                SendViaSMTPClient(document, attachmentPath, recipient, subject, body);
            }
        }

        private static void SendViaMAPIMessage(string attachmentPath, string subject, string body, string recipient)
        {
            if (string.IsNullOrEmpty(recipient)) return;

            MapiMailMessage mapiMailMessage = new MapiMailMessage
                                                  {
                                                      Subject = subject,
                                                      Body = body
                                                  };

            mapiMailMessage.Recipients.Add(recipient);

            if (!string.IsNullOrEmpty(attachmentPath))
                mapiMailMessage.Files.Add(attachmentPath);

            mapiMailMessage.ShowDialog();
        }

        private static void SendViaSMTPClient(Document document, string attachmentPath, string recipient, string subject, string body)
        {
            if (document == null) return;
            if (string.IsNullOrEmpty(recipient)) return;

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.To.Add(recipient);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = body;
                mailMessage.From = new MailAddress("webcal@tachoworkshop.co.uk");
                if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Credentials = new NetworkCredential("webcal@tachoworkshop.co.uk", "skillraywebcal");
                    smtp.Host = "mail.yellowbus.co.uk";
                    smtp.Send(mailMessage);
                }
            }
        }

        private static string GetRecipientEmailAddress(string customerContact)
        {
            IRepository<CustomerContact> repository = ObjectFactory.GetInstance<IRepository<CustomerContact>>();
            CustomerContact customer = repository.FirstOrDefault(contact => string.Equals(customerContact, contact.Name, StringComparison.CurrentCultureIgnoreCase));

            if (customer == null || string.IsNullOrEmpty(customer.Email))
                return string.Empty;

            if (!IsValidEmail(customer.Email))
                return string.Empty;

            return customer.Email;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            //Regular expressions are SO over-rated

            try
            {
                new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
