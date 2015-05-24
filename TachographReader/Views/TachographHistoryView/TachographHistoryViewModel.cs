namespace TachographReader.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Library;
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
            Documents = new ObservableCollection<Document>(TachographDocumentsRepository.GetAll().OrderByDescending(c => c.InspectionDate));
        }

        protected override void InitialiseRepositories()
        {
            TachographDocumentsRepository = GetInstance<IRepository<TachographDocument>>();
            WorkshopSettings = GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            MailSettings = GetInstance<ISettingsRepository<MailSettings>>().Get();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        protected override void OnDocumentSelected(Document document)
        {
            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                NewTachographViewModel viewModel = tachographDocument.IsDigital
                    ? (NewTachographViewModel) MainWindow.ShowView<NewTachographView>()
                    : (NewTachographViewModel) MainWindow.ShowView<NewAnalogueTachographView>();

                viewModel.Document = tachographDocument;
                viewModel.SelectedCustomerContact = viewModel.CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, tachographDocument.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
                viewModel.SetDocumentTypes(tachographDocument.IsDigital);
                viewModel.IsReadOnly = true;
                viewModel.IsHistoryMode = true;
            }
        }

        protected override void OnEmailReportSelected(Document document)
        {
            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                tachographDocument.ToPDF().Print();
            }
        }

        protected override void OnCreateVOSADocument(DateTime start, DateTime end)
        {
            List<TachographDocument> applicableDocuments = Documents.Where(doc => doc.InspectionDate.Value >= start && doc.InspectionDate.Value <= end).Cast<TachographDocument>().ToList();
            applicableDocuments.GenerateVOSADocument(start, end);
        }

        protected override bool IncludeDeletedContacts
        {
            get { return true; }
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

            MiscellaneousSettings miscellaneousSettings = GetInstance<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
            document.ToPDF(excludeLogos: miscellaneousSettings.ExcludeLogosWhenPrinting).Print();
        }
    }
}