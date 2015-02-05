namespace Webcal.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using Microsoft.Office.Interop.Excel;
    using Properties;
    using Shared;
    using Constants = Core.Constants;

    public static class ExcelHelper
    {
        public static void GenerateExpiringTachographDocumentsReport(ReportFileFormat format, DateTime start, DateTime end, string office)
        {
            switch (format)
            {
                case ReportFileFormat.Text:
                    GenerateExpiringTachographDocumentsTextDocument(start, end, office);
                    break;

                case ReportFileFormat.Excel:
                    GenerateExpiringTachographDocumentsExcelDocument(start, end, office);
                    break;
            }
        }

        private static void GenerateExpiringTachographDocumentsExcelDocument(DateTime start, DateTime end, string office)
        {
            Worksheet worksheet = CreateWorksheet();

            var repository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographDocument>>();
            var customerRepository = ContainerBootstrapper.Container.GetInstance<IRepository<CustomerContact>>();
            ICollection<TachographDocument> allDocuments = repository.GetAll().OrderByDescending(c => c.Created).ToList();

            SetDocumentTitle(worksheet, string.Format(Resources.TXT_TACHOGRAPH_DOCUMENTS_THAT_WILL_EXPIRE, start.ToString(Constants.DateFormat), end.ToString(Constants.DateFormat), office));

            SetBoldAndUnderlined(worksheet, 3, 0, Resources.TXT_CUSTOMER);
            SetBoldAndUnderlined(worksheet, 3, 1, Resources.TXT_EMAIL);
            SetBoldAndUnderlined(worksheet, 3, 2, Resources.TXT_PHONE_NUMBER);
            SetBoldAndUnderlined(worksheet, 3, 3, Resources.TXT_REGISTRATION_NUMBER);
            SetBoldAndUnderlined(worksheet, 3, 4, Resources.TXT_TECHNICIAN);
            SetBoldAndUnderlined(worksheet, 3, 5, Resources.TXT_OFFICE);
            SetBoldAndUnderlined(worksheet, 3, 6, Resources.TXT_DATE_OF_INSPECTION);

            ICollection<CustomerContact> customers = customerRepository.Get(c => allDocuments.Any(d => string.Equals(d.CustomerContact, c.Name)));
            int row = 4;

            foreach (TachographDocument document in allDocuments.Where(doc => doc.Created.AddYears(2).IsBetween(start, end)))
            {
                TachographDocument copy = document;
                CustomerContact customer = customers.FirstOrDefault(c => string.Equals(c.Name, copy.CustomerContact));

                SetData(worksheet, row, 0, copy.CustomerContact);
                SetData(worksheet, row, 3, copy.RegistrationNumber);
                SetData(worksheet, row, 4, copy.Technician);
                SetData(worksheet, row, 5, copy.Office);

                if (customer != null)
                {
                    SetData(worksheet, row, 1, customer.Email);
                    SetData(worksheet, row, 2, customer.PhoneNumber);
                }

                if (copy.InspectionDate != null)
                {
                    SetData(worksheet, row, 6, copy.InspectionDate.Value.ToString(Constants.DateFormat));
                }

                row++;
            }

            AutoFit(worksheet, 1, 2, 3, 4, 5, 6, 7);
        }

        private static void GenerateExpiringTachographDocumentsTextDocument(DateTime start, DateTime end, string office)
        {
            var builder = new StringBuilder();
            var repository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographDocument>>();
            var customerRepository = ContainerBootstrapper.Container.GetInstance<IRepository<CustomerContact>>();

            ICollection<TachographDocument> allDocuments = repository.GetAll();
            ICollection<CustomerContact> customers = customerRepository.Get(c => allDocuments.Any(d => string.Equals(d.CustomerContact, c.Name)));

            //Title
            builder.AppendLine(string.Format(Resources.TXT_TACHOGRAPH_DOCUMENTS_THAT_WILL_EXPIRE, start.ToString(Constants.DateFormat), end.ToString(Constants.DateFormat), office));
            builder.AppendLine();

            foreach (TachographDocument document in allDocuments.Where(doc => doc.Created.AddYears(2).IsBetween(start, end)))
            {
                TachographDocument copy = document;
                CustomerContact customer = customers.FirstOrDefault(c => string.Equals(c.Name, copy.CustomerContact));

                builder.AppendLine(string.Format("{0}: {1}", Resources.TXT_CUSTOMER, customer == null ? string.Empty : customer.Name));
                builder.AppendLine(string.Format("{0}: {1}", Resources.TXT_REGISTRATION_NUMBER, copy.RegistrationNumber));
                builder.AppendLine(string.Format("{0}: {1}", Resources.TXT_TECHNICIAN, copy.Technician));
                builder.AppendLine(string.Format("{0}: {1}", Resources.TXT_OFFICE, office));
                builder.AppendLine(string.Format("{0}: {1}", Resources.TXT_DATE_OF_INSPECTION, copy.InspectionDate == null ? string.Empty : copy.InspectionDate.Value.ToString(Constants.DateFormat)));
                builder.AppendLine();
            }

            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PlainText, string.Empty);
            if (result.Result == true)
            {
                File.WriteAllText(result.FileName, builder.ToString(), Encoding.ASCII);
            }
        }

        public static void GenerateDocumentStatistics(ReportFileFormat format, string office)
        {
            switch (format)
            {
                case ReportFileFormat.Text:
                    GenerateDocumentStatisticsTextDocument(office);
                    break;

                case ReportFileFormat.Excel:
                    GenerateDocumentStatisticsExcelDocument(office);
                    break;
            }
        }

        private static void GenerateDocumentStatisticsTextDocument(string office)
        {
            var builder = new StringBuilder();
            ICollection<TachographDocument> allDocuments = GetAllDocuments();

            //Title
            builder.AppendLine(string.Format(Resources.TXT_CREATED_REPORTS_PER_YEAR_PER_CUSTOMER, office));
            builder.AppendLine();

            builder.AppendLine(Resources.TXT_TACHOGRAPH_DOCUMENTS_PER_YEAR);

            //Year Data
            IDictionary<int, int> yearData = GetGroupedData(allDocuments, prop => prop.Created.Year);
            foreach (var item in yearData)
            {
                builder.AppendLine(string.Format("{0}: {1}", item.Key, item.Value));
            }

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(Resources.TXT_TACHOGRAPH_DOCUMENTS_BY_CUSTOMER);

            //Customer Data
            IDictionary<string, int> customerData = GetGroupedData(allDocuments, prop => prop.CustomerContact);
            foreach (var item in customerData)
            {
                builder.AppendLine(string.Format("{0}: {1}", item.Key, item.Value));
            }

            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PlainText, string.Empty);
            if (result.Result == true)
            {
                File.WriteAllText(result.FileName, builder.ToString(), Encoding.ASCII);
            }
        }

        private static void GenerateDocumentStatisticsExcelDocument(string office)
        {
            Worksheet worksheet = CreateWorksheet();
            ICollection<TachographDocument> allDocuments = GetAllDocuments();

            //Title
            SetDocumentTitle(worksheet, string.Format(Resources.TXT_CREATED_REPORTS_PER_YEAR_PER_CUSTOMER, office));

            //Documents Per Year
            int row = 5;
            SetSectionTitle(worksheet, row, 0, Resources.TXT_TACHOGRAPH_DOCUMENTS_PER_YEAR);

            //Year
            row += 2;
            SetBoldAndUnderlined(worksheet, row, 0, Resources.TXT_YEAR);

            //Number of documents
            SetBoldAndUnderlined(worksheet, row, 1, Resources.TXT_NUMBER_OF_DOCUMENTS);

            //Techograph Documents created by year
            row += 1;
            IDictionary<int, int> yearData = GetGroupedData(allDocuments, prop => prop.Created.Year);
            foreach (var item in yearData)
            {
                SetData(worksheet, row, 0, item.Key);
                SetData(worksheet, row, 1, item.Value);
                row += 1;
            }

            //Documents Created By Customer
            row += 2;
            SetSectionTitle(worksheet, row, 0, Resources.TXT_TACHOGRAPH_DOCUMENTS_BY_CUSTOMER);

            //Documents Per Year
            //Customer
            row += 2;
            SetBoldAndUnderlined(worksheet, row, 0, Resources.TXT_CUSTOMER);

            //Number of documents
            SetBoldAndUnderlined(worksheet, row, 1, Resources.TXT_NUMBER_OF_DOCUMENTS);

            //Techograph Documents created by customer
            row += 1;
            IDictionary<string, int> customerData = GetGroupedData(allDocuments, prop => prop.CustomerContact);
            foreach (var item in customerData)
            {
                SetData(worksheet, row, 0, item.Key);
                SetData(worksheet, row, 1, item.Value);
                row += 1;
            }

            AutoFit(worksheet, 1, 2);
        }

        private static Worksheet CreateWorksheet()
        {
            var application = new Application { Visible = true };

            Workbook workbook = application.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            var worksheet = (Worksheet)workbook.Worksheets.Item[1];

            worksheet.Name = Resources.TXT_GENERATED_REPORT;
            return worksheet;
        }

        private static void SetDocumentTitle(_Worksheet worksheet, string text)
        {
            worksheet.Cells[1, ToLetter(0)] = text;
            worksheet.Cells[1, ToLetter(0)].Font.Bold = true;
            worksheet.Cells[1, ToLetter(0)].Font.Size = 24;
            worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 20]].Merge();
        }

        private static void SetSectionTitle(_Worksheet worksheet, int row, int column, string text)
        {
            worksheet.Cells[row, ToLetter(column)] = text;
            worksheet.Cells[row, ToLetter(column)].Font.Bold = true;
            worksheet.Cells[row, ToLetter(column)].Font.Size = 16;
            worksheet.Range[worksheet.Cells[row, ToLetter(column)], worksheet.Cells[row, 20]].Merge();
        }

        private static void SetBoldAndUnderlined(_Worksheet worksheet, int row, int column, string text)
        {
            worksheet.Cells[row, ToLetter(column)] = text;
            worksheet.Cells[row, ToLetter(column)].Font.Bold = true;
            worksheet.Cells[row, ToLetter(column)].Font.Underline = true;
        }

        private static void SetData(_Worksheet worksheet, int row, int column, object data)
        {
            worksheet.Cells[row, ToLetter(column)] = data;
        }

        private static void AutoFit(_Worksheet worksheet, params int[] columns)
        {
            foreach (int column in columns)
            {
                worksheet.Columns[column].AutoFit();
            }
        }

        private static IDictionary<T2, int> GetGroupedData<T, T2>(IEnumerable<T> allDocuments, Func<T, T2> groupBy)
        {
            return allDocuments.GroupBy(groupBy)
                .Select(doc => new { doc.Key, Count = doc.Count() })
                .ToDictionary(t => t.Key, t => t.Count);
        }

        private static string ToLetter(int index)
        {
            string alphabet = Resources.TXT_ALPHABET;

            string value = string.Empty;

            if (index >= alphabet.Length)
            {
                value += alphabet[index / alphabet.Length - 1];
            }

            value += alphabet[index % alphabet.Length];

            return value;
        }

        private static ICollection<TachographDocument> GetAllDocuments()
        {
            var repository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographDocument>>();
            return repository.GetAll();
        }
    }
}