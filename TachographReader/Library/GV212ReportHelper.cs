namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Windows.DateRangePickerWindow;
    using Connect.Shared.Models;
    using Core;
    using DataModel.Core;
    using PDF;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class GV212ReportHelper
    {
        public static void Create(DateTime start, DateTime end)
        {
            var tachographDocuments = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>().GetAll(false);
            var documentHistoryItems = new ObservableCollection<IDocumentHistoryItem>(tachographDocuments.Select(c => new DocumentHistoryItem(c)));

            Create(documentHistoryItems, false, start, end);
        }

        public static void Create(bool promptForDate)
        {
            var tachographDocuments = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>().GetAll(false);
            var documentHistoryItems = new ObservableCollection<IDocumentHistoryItem>(tachographDocuments.Select(c => new DocumentHistoryItem(c)));
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            Create(documentHistoryItems, promptForDate, start, end);
        }

        public static void Create(ICollection<IDocumentHistoryItem> documents, bool promptForDate)
        {
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var end = start.AddMonths(1).AddDays(-1);

            Create(documents, promptForDate, start, end);
        }

        public static void Create(ICollection<IDocumentHistoryItem> documents, bool promptForDate, DateTime start, DateTime end)
        {
            if (promptForDate)
            {
                var window = new DateRangePickerWindow();
                var viewModel = window.DataContext as DateRangePickerWindowViewModel;
                if (viewModel == null)
                {
                    return;
                }

                viewModel.StartDateTime = start;
                viewModel.EndDateTime = end;

                if (window.ShowDialog() != true)
                {
                    return;
                }

                start = DateTime.Parse($"{viewModel.StartDateTime.ToString(Constants.DateFormat)} 00:00:00");
                end = DateTime.Parse($"{viewModel.EndDateTime.ToString(Constants.DateFormat)} 23:59:59");
            }

            var applicableDocuments = documents.Where(doc => doc.Document != null && doc.Document is TachographDocument)
                .Select(c => c.Document)
                .Where(doc => doc.InspectionDate != null && doc.InspectionDate.Value >= start && doc.InspectionDate.Value <= end)
                .Cast<TachographDocument>()
                .ToList();

            var result = applicableDocuments.GenerateGV212Document(start, end);
            if (result.Success)
            {
                var gv212Repository = ContainerBootstrapper.Resolve<IRepository<GV212Report>>();
                gv212Repository.Add(new GV212Report
                {
                    Created = DateTime.Now.Date,
                    SerializedData = result.SerializedData
                });

                if (MessageBoxHelper.AskQuestion(Resources.TXT_DO_YOU_WANT_TO_PRINT))
                {
                    result.Print();
                }
            }
        }
        
        public static bool HasDataForThisMonth()
        {
            var tachographDocuments = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>().GetAll(false);

            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var applicableDocuments = tachographDocuments
                .Where(doc => doc.InspectionDate != null && doc.InspectionDate.Value >= start && doc.InspectionDate.Value <= end)
                .ToList();

            return applicableDocuments.Any();
        }
    }
}