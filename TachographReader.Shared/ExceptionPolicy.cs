namespace TachographReader.Shared
{
    using System;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using global::Connect.Shared.Models;
    using Helpers;
    using Microsoft.Practices.Unity;
    using Models;
    using Properties;
    using Application = System.Windows.Forms.Application;

    public static class ExceptionPolicy
    {
        public static string HandleException(IUnityContainer container, Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            try
            {
                container.Resolve<IRepository<DetailedException>>().Add(new DetailedException
                {
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                    ExceptionDetails = string.Format("{0}\n{1}", exception.Message, exception.StackTrace),
                    Occurred = DateTime.Now,
                    RawImage = ScreenshotHelper.TakeScreenshot()
                });
            }
            catch (Exception ex)
            {
                if (string.Equals(Resources.ERR_UNABLE_FIND_DATA_PROVIDER, ex.Message))
                {
                    MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.ERR_UNABLE_FIND_SQL_CE, ex.Message));
                    Application.Exit();
                }
                else
                {
                    MessageBoxHelper.ShowError(ex.Message);
                }
            }

            return GetExceptionMessage(exception);
        }

        public static void Serialize(object obj, string outputPath)
        {
            var serializerObj = new XmlSerializer(obj.GetType());

            using (TextWriter writeFileStream = new StreamWriter(outputPath))
            {
                serializerObj.Serialize(writeFileStream, obj);
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            var dbEntityValidationException = exception as DbEntityValidationException;
            if (dbEntityValidationException != null)
            {
                return ExtractEntityValidationExceptions(dbEntityValidationException);
            }

            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                string message = string.Empty;
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                   message += (GetExceptionMessage(message, innerException));
                }
                return message;
            }

            return GetExceptionMessage(string.Empty, exception);
        }

        private static string ExtractEntityValidationExceptions(DbEntityValidationException exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            foreach (DbEntityValidationResult entityValidationError in exception.EntityValidationErrors)
            {
                builder.AppendFormat(Resources.EXC_ENTITY_VALIDATION_ERRORS, entityValidationError.Entry.Entity.GetType().Name, entityValidationError.Entry.State);
                foreach (DbValidationError error in entityValidationError.ValidationErrors)
                {
                    builder.AppendFormat(Resources.EXC_VALIDATION_ERROR_PROPERTY, error.PropertyName, error.ErrorMessage);
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string GetExceptionMessage(string message, Exception exception)
        {
            message += string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

            if (exception.InnerException != null)
            {
                return GetExceptionMessage(message, exception.InnerException);
            }

            return message;
        }
    }
}