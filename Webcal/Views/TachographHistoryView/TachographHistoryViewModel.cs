namespace Webcal.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Library.PDF;
    using Shared;

    public class TachographHistoryViewModel : BaseHistoryViewModel
    {
        public IRepository<TachographDocument> TachographDocumentsRepository { get; set; }

        public WorkshopSettings WorkshopSettings { get; set; }
        public MailSettings MailSettings { get; set; }

        public DelegateCommand<object> ReprintLabelCommand { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        protected override void Load()
        {
            Documents = new ObservableCollection<Document>(TachographDocumentsRepository.GetAll());
        }

        protected override void InitialiseRepositories()
        {
            TachographDocumentsRepository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographDocument>>();
            WorkshopSettings = ContainerBootstrapper.Container.GetInstance<IGeneralSettingsRepository>().GetSettings();
            MailSettings = ContainerBootstrapper.Container.GetInstance<IMailSettingsRepository>().GetSettings();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null)
            {
                return;
            }

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                NewTachographViewModel viewModel = tachographDocument.IsDigital
                    ? (NewTachographViewModel)MainWindow.ShowView<NewTachographView>()
                    : (NewTachographViewModel)MainWindow.ShowView<NewAnalogueTachographView>();

                viewModel.Document = tachographDocument;
                viewModel.SetDocumentTypes(tachographDocument.IsDigital);
                viewModel.IsReadOnly = true;
            }
        }

        protected override void OnEmailReportSelected(Document document)
        {
            if (document == null)
            {
                return;
            }

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                if (PDFHelper.GenerateTachographPlaque(document, true))
                {
                    EmailHelper.SendEmail(WorkshopSettings, MailSettings, document, Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
                }
            }
        }

        protected override void OnCreateVOSADocument(DateTime start, DateTime end)
        {
            List<TachographDocument> applicableDocuments = Documents.Where(doc => doc.InspectionDate.Value >= start && doc.InspectionDate.Value <= end).Cast<TachographDocument>().ToList();
            PDFHelper.GenerateVOSADocument(applicableDocuments, start, end);
        }

        private void OnReprintLabel(object obj)
        {
            var document = SelectedDocument as TachographDocument;
            if (document != null)
            {
                LabelHelper.Print(document);
            }
        }

        private void OnReprintCertificate(object obj)
        {
            var document = SelectedDocument as TachographDocument;
            if (document == null)
            {
                return;
            }

            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
            }
        }
    }
}