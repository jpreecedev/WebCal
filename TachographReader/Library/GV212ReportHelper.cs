namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Windows.DateRangePickerWindow;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using PDF;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class GV212ReportHelper
    {
        public static void Create(bool promptForDate)
        {
            var tachographDocuments = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>().GetAll(false);
            var documentHistoryItems = new ObservableCollection<IDocumentHistoryItem>(tachographDocuments.Select(c => new DocumentHistoryItem(c)));
            Create(documentHistoryItems, promptForDate);
        }

        public static void Create(ICollection<IDocumentHistoryItem> documents, bool promptForDate)
        {
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            if (promptForDate)
            {
                var window = new DateRangePickerWindow();
                if (window.ShowDialog() != true)
                {
                    return;
                }

                var viewModel = window.DataContext as DateRangePickerWindowViewModel;
                if (viewModel == null)
                {
                    return;
                }

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

                var settingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
                var settings = settingsRepository.GetWorkshopSettings();
                settings.MonthlyGV212Date = DateTime.Now.Date;
                settingsRepository.Save(settings);

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