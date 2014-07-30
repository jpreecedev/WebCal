using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Library.PDF;
using Webcal.Shared;

namespace Webcal.Views
{
    public class TachographHistoryViewModel : BaseHistoryViewModel
    {
        #region Public Properties

        public IRepository<TachographDocument> TachographDocumentsRepository { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            Documents = new ObservableCollection<Document>(TachographDocumentsRepository.GetAll());
        }

        protected override void InitialiseRepositories()
        {
            TachographDocumentsRepository = ObjectFactory.GetInstance<IRepository<TachographDocument>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null) return;

            TachographDocument tachographDocument = document as TachographDocument;
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
            if (document == null) return;

            TachographDocument tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                //email report

                if (PDFHelper.GenerateTachographPlaque(document, true))
                {
                    EmailHelper.SendEmail(document, Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
                }
            }
        }

        protected override void OnCreateVOSADocument(DateTime start, DateTime end)
        {
            List<TachographDocument> applicableDocuments = Documents.Where(doc => doc.InspectionDate.Value >= start && doc.InspectionDate.Value <= end).Cast<TachographDocument>().ToList();
            PDFHelper.GenerateVOSADocument(applicableDocuments, start, end);
        }

        #endregion

        #region Commands

        #region Command : Re-Print Label

        public DelegateCommand<object> ReprintLabelCommand { get; set; }

        private void OnReprintLabel(object obj)
        {
            TachographDocument document = SelectedDocument as TachographDocument;
            if (document != null)
            {
                using (LabelHelper labelHelper = new LabelHelper())
                    labelHelper.Print(document);
            }
        }

        #endregion

        #region Command : Re-Print Certificate

        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        private void OnReprintCertificate(object obj)
        {
            TachographDocument document = SelectedDocument as TachographDocument;
            if (document == null) return;

            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
            }
        }

        #endregion

        #endregion
    }
}
