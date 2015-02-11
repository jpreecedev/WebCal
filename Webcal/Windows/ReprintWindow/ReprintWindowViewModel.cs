namespace Webcal.Windows.ReprintWindow
{
    using System;
    using System.Linq;
    using System.Windows;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using Shared.Connect;
    using Shared.Helpers;

    public class ReprintWindowViewModel : BaseModalWindowViewModel
    {
        public ReprintWindowViewModel()
        {
            TachographDocumentRepository = GetInstance<IRepository<TachographDocument>>();
            UndownloadabilityDocumentRepository = GetInstance<IRepository<UndownloadabilityDocument>>();
            LetterForDecommissioningRepository = GetInstance<IRepository<LetterForDecommissioningDocument>>();
            RegistrationData = GetInstance<IRepository<RegistrationData>>().First();

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
        public IRepository<LetterForDecommissioningDocument> LetterForDecommissioningRepository { get; set; }
        public ReprintMode ReprintMode { get; set; }
        public DelegateCommand<Window> ReprintCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }
        public RegistrationData RegistrationData { get; set; }

        public bool IsLoading { get; set; }

        private void OnReprint(Window window)
        {
            if (string.IsNullOrEmpty(RegistrationNumber))
            {
                return;
            }

            Document document = FindDocument();
            if (document == null)
            {
                MessageBoxHelper.ShowError(Resources.ERR_UNABLE_FIND_ANY_MATCHES, Window);
                return;
            }

            Print(document);
            window.Close();
        }

        private static void OnCancel(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.Close();
        }

        private Document FindDocument()
        {
            Document result = null;

            try
            {
                IsLoading = true;

                TachographDocument tachographDocument = Find(TachographDocumentRepository);
                if (tachographDocument != null)
                {
                    result = tachographDocument;
                }

                UndownloadabilityDocument undownloadabilityDocument = Find(UndownloadabilityDocumentRepository);
                if (undownloadabilityDocument != null)
                {
                    result = undownloadabilityDocument;
                }

                LetterForDecommissioningDocument letterForDecommissioningDocument = Find(LetterForDecommissioningRepository);
                if (letterForDecommissioningDocument != null)
                {
                    result = letterForDecommissioningDocument;
                }

                if (result == null && RegistrationData.IsConnectEnabled)
                {
                    GetInstance<IConnectClient>().CallSync(ConnectHelper.GetConnectKeys(), client =>
                    {
                        return client.Service.Find(RegistrationNumber.ToUpper().Replace(" ", string.Empty));
                    },
                    connectResult =>
                    {
                        result = (Document) connectResult.Data;
                    });
                }
            }
            finally
            {
                IsLoading = false;
            }

            return result;
        }

        private T Find<T>(IRepository<T> repository) where T : Document
        {
            string registrationNumber = RegistrationNumber.ToUpper().Replace(" ", string.Empty);
            return repository.Where(item => string.Equals(item.RegistrationNumber, registrationNumber, StringComparison.CurrentCultureIgnoreCase))
                             .OrderByDescending(item => item.InspectionDate)
                             .FirstOrDefault();
        }

        private void Print(Document document)
        {
            if (document == null)
            {
                return;
            }

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                PrintLabel(tachographDocument);
            }

            PrintCertificate(document);
        }

        private void PrintLabel(TachographDocument tachographDocument)
        {
            if (ReprintMode != ReprintMode.Label)
            {
                return;
            }

            LabelHelper.Print(tachographDocument);
        }

        private void PrintCertificate(Document document)
        {
            if (ReprintMode != ReprintMode.Certificate)
            {
                return;
            }

            MiscellaneousSettings miscellaneousSettings = GetInstance<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
            document.ToPDF(excludeLogos: miscellaneousSettings.ExcludeLogosWhenPrinting).Print();
        }
    }
}