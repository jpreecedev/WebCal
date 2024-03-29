﻿using Microsoft.Practices.ObjectBuilder2;

namespace TachographReader.Library
{
    using System;
    using System.Net.Mail;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using Shared;
    using Shared.Workers;

    public static class EmailHelper
    {
        public static void SendEmail(WorkshopSettings workshopSettings, MailSettings mailSettings, Document document, string attachmentPath)
        {
            if (workshopSettings == null)
            {
                throw new ArgumentNullException(nameof(workshopSettings));
            }
            if (mailSettings == null)
            {
                throw new ArgumentNullException(nameof(mailSettings));
            }
            if (document == null)
            {
                return;
            }

            string recipient = GetRecipientEmailAddresses(workshopSettings, mailSettings, document.CustomerContact);
            if (string.IsNullOrEmpty(recipient))
            {
                return;
            }

            recipient.Split(';').ForEach(c =>
            {
                CreateEmailTask(mailSettings, attachmentPath, c);
            });

        }

        private static string GetRecipientEmailAddresses(WorkshopSettings workshopSettings, MailSettings mailSettings, string customerContact)
        {
            string recipients = string.Empty;

            if (mailSettings == null || mailSettings.DontSendEmails || workshopSettings.DoNotSend)
            {
                return recipients;
            }

            if (workshopSettings.SendToOffice)
            {
                recipients = AppendRecipient(recipients, workshopSettings.MainEmailAddress);
                recipients = AppendRecipient(recipients, workshopSettings.SecondaryEmailAddress);
            }
            if (workshopSettings.SendToCustomer && !string.IsNullOrEmpty(customerContact))
            {
                var repository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();
                CustomerContact customer = repository.FirstOrDefault(contact => string.Equals(customerContact, contact.Name, StringComparison.CurrentCultureIgnoreCase));

                if (customer != null)
                {
                    if (IsValidEmail(customer.Email))
                    {
                        recipients = AppendRecipient(recipients, customer.Email);
                    }
                    if (IsValidEmail(customer.SecondaryEmail))
                    {
                        recipients = AppendRecipient(recipients, customer.SecondaryEmail);
                    }
                }
            }

            return recipients;
        }

        private static string AppendRecipient(string existingRecipient, string newRecipient)
        {
            if (string.IsNullOrEmpty(newRecipient))
            {
                return existingRecipient;
            }

            if (string.IsNullOrEmpty(existingRecipient))
            {
                return newRecipient;
            }
            return $"{existingRecipient}; {newRecipient}";
        }

        private static void CreateEmailTask(MailSettings settings, string attachmentPath, string recipient)
        {
            var parameters = new WorkerParameters();
            parameters.SetParameter("PersonaliseMyEmails", settings.PersonaliseMyEmails);
            parameters.SetParameter("AttachmentPath", attachmentPath);
            parameters.SetParameter("Recipient", recipient);
            parameters.SetParameter("Subject", settings.Subject);
            parameters.SetParameter("Body", settings.Body);

            var workerTask = new WorkerTask {TaskName = WorkerTaskName.Email};
            workerTask.SetWorkerParameters(parameters);

            WorkerHelper.QueueTask(workerTask);
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