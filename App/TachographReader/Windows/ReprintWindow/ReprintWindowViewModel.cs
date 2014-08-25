﻿namespace Webcal.Windows.ReprintWindow
{
    using System;
    using System.IO;
    using System.Windows;
    using Core;
    using DataModel;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using StructureMap;
    using BaseNotification = Core.BaseNotification;

    public class ReprintWindowViewModel : BaseNotification
    {
        public ReprintWindowViewModel()
        {
            TachographDocumentRepository = ObjectFactory.GetInstance<IRepository<TachographDocument>>();
            UndownloadabilityDocumentRepository = ObjectFactory.GetInstance<IRepository<UndownloadabilityDocument>>();

            ReprintCommand = new DelegateCommand<Window>(OnReprint);
            CancelCommand = new DelegateCommand<Window>(OnCancel);
        }

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
        
        public DelegateCommand<Window> ReprintCommand { get; set; }

        public DelegateCommand<Window> CancelCommand { get; set; }

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

        private static void OnCancel(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }
        
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
            string registrationNumber = RegistrationNumber.ToUpper().Replace(" ", "");
            return repository.FirstOrDefault(item => string.Equals(item.RegistrationNumber, registrationNumber, StringComparison.CurrentCultureIgnoreCase));
        }

        private void Print(Document document)
        {
            if (document == null)
                return;

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
                PrintLabel(tachographDocument);

            PrintCertificate(document);
            MessageBoxHelper.ShowMessage(Resources.TXT_REPRINT_COMPLETED);
        }

        private void PrintLabel(TachographDocument tachographDocument)
        {
            if (ReprintMode != ReprintMode.Label)
                return;

            using (var labelHelper = new LabelHelper())
            {
                labelHelper.Print(tachographDocument);
            }
        }

        private void PrintCertificate(Document document)
        {
            if (ReprintMode != ReprintMode.Certificate)
                return;

            if (PDFHelper.GenerateTachographPlaque(document, true))
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
        }
    }
}