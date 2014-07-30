using System;
using System.IO;
using System.Windows;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Library.PDF;
using Webcal.Properties;
using Webcal.Shared;
using BaseNotification = Webcal.Core.BaseNotification;

namespace Webcal.Windows.ReprintWindow
{
    public class ReprintWindowViewModel : BaseNotification
    {
        #region Constructor

        public ReprintWindowViewModel()
        {
            TachographDocumentRepository = ObjectFactory.GetInstance<IRepository<TachographDocument>>();
            UndownloadabilityDocumentRepository = ObjectFactory.GetInstance<IRepository<UndownloadabilityDocument>>();

            ReprintCommand = new DelegateCommand<Window>(OnReprint);
            CancelCommand = new DelegateCommand<Window>(OnCancel);
        }

        #endregion

        #region Public Properties

        public string TitleText
        {
            get
            {
                switch (ReprintMode)
                {
                    case ReprintMode.Certificate:
                        return Resources.TXT_REPRINT_CERTIFICATE;

                    case ReprintMode.Label:
                        return Resources.TXT_REPRINT_LABEL;
                }

                return string.Empty;
            }
        }

        public string RegistrationNumber { get; set; }

        public IRepository<TachographDocument> TachographDocumentRepository { get; set; }

        public IRepository<UndownloadabilityDocument> UndownloadabilityDocumentRepository { get; set; }

        public ReprintMode ReprintMode { get; set; }

        #endregion

        #region Commands

        #region Command : Reprint

        public DelegateCommand<Window> ReprintCommand { get; set; }

        private void OnReprint(Window window)
        {
            if (string.IsNullOrEmpty(RegistrationNumber))
                return;

            Document document = FindDocument();
            if (document == null)
            {
                MessageBoxHelper.ShowError(Resources.ERR_UNABLE_FIND_ANY_MATCHES);
                return;
            }

            Print(document);
            window.Close();
        }

        #endregion

        #region Command : Cancel

        public DelegateCommand<Window> CancelCommand { get; set; }

        private static void OnCancel(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }

        #endregion

        #endregion

        #region Private Methods

        private Document FindDocument()
        {
            TachographDocument tachographDocument = Find(TachographDocumentRepository);
            if (tachographDocument != null)
                return tachographDocument;

            UndownloadabilityDocument undownloadabilityDocument = Find(UndownloadabilityDocumentRepository);
            if (undownloadabilityDocument != null)
                return undownloadabilityDocument;

            return null;
        }

        private T Find<T>(IRepository<T> repository) where T : Document
        {
            var registrationNumber = RegistrationNumber.ToUpper().Replace(" ", "");
            return repository.FirstOrDefault(item => string.Equals(item.RegistrationNumber, registrationNumber, StringComparison.CurrentCultureIgnoreCase));
        }

        private void Print(Document document)
        {
            if (document == null)
                return;

            TachographDocument tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                PrintLabel(tachographDocument);
            }

            PrintCertificate(document);
            MessageBoxHelper.ShowMessage(Resources.TXT_REPRINT_COMPLETED);
        }

        private void PrintLabel(TachographDocument tachographDocument)
        {
            if (ReprintMode != ReprintMode.Label)
                return;

            using (LabelHelper labelHelper = new LabelHelper())
                labelHelper.Print(tachographDocument);
        }

        private void PrintCertificate(Document document)
        {
            if (ReprintMode != ReprintMode.Certificate)
                return;

            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
            }
        }

        #endregion
    }
}
