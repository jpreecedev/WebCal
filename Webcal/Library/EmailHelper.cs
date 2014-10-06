namespace Webcal.Library
{
    using System;
    using System.Net.Mail;
    using DataModel;
    using DataModel.Core;
    using Shared;
    using Shared.Workers;

    public static class EmailHelper
    {
        public static void SendEmail(Document document, string attachmentPath)
        {
            if (document == null || document.CustomerContact == null || string.IsNullOrEmpty(document.CustomerContact))
            {
                return;
            }

            var mailRepository = ContainerBootstrapper.Container.GetInstance<IMailSettingsRepository>();
            MailSettings settings = mailRepository.GetSettings();

            if (!settings.AutoEmailCertificates && !settings.PersonaliseMyEmails)
            {
                return;
            }

            string recipient = GetRecipientEmailAddress(document.CustomerContact);
            if (string.IsNullOrEmpty(recipient))
            {
                return;
            }

            CreateEmailTask(settings, attachmentPath, recipient);
        }

        private static string GetRecipientEmailAddress(string customerContact)
        {
            var repository = ContainerBootstrapper.Container.GetInstance<IRepository<CustomerContact>>();
            CustomerContact customer = repository.FirstOrDefault(contact => string.Equals(customerContact, contact.Name, StringComparison.CurrentCultureIgnoreCase));

            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                return string.Empty;
            }

            if (!IsValidEmail(customer.Email))
            {
                return string.Empty;
            }

            return customer.Email;
        }

        private static void CreateEmailTask(MailSettings settings, string attachmentPath, string recipient)
        {
            var workerTask = new WorkerTask { TaskName = WorkerTaskName.Email };

            workerTask.Parameters = new WorkerParameters();
            workerTask.Parameters.SetParameter("PersonaliseMyEmails", settings.PersonaliseMyEmails);
            workerTask.Parameters.SetParameter("AttachmentPath", attachmentPath);
            workerTask.Parameters.SetParameter("Recipient", recipient);
            workerTask.Parameters.SetParameter("Subject", settings.Subject);
            workerTask.Parameters.SetParameter("Body", settings.Body);

            WorkerHelper.RunTask(workerTask);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

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