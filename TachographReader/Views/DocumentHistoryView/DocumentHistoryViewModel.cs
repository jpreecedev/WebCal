namespace TachographReader.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using Library;
    using Library.ViewModels;
    using Properties;

    public class DocumentHistoryViewModel : BaseMainViewModel
    {
        private ICollection<IDocumentHistoryItem> _originalDocumentHistoryItems;

        public List<string> SearchFilters { get; set; }
        public string SelectedSearchFilter { get; set; }
        public string SearchTerm { get; set; }

        public List<string> DocumentTypes { get; set; }
        public string SelectedDocumentType { get; set; }

        public ObservableCollection<IDocumentHistoryItem> Documents { get; set; }
        public IDocumentHistoryItem SelectedDocument { get; set; }

        public DelegateCommand<object> ReprintLabelCommand { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }
        public DelegateCommand<object> OpenInReportFormCommand { get; set; }
        public DelegateCommand<object> EmailReportFormCommand { get; set; }
        public DelegateCommand<object> PerformSearchCommand { get; set; }
        public DelegateCommand<object> CreateGV212DocumentCommand { get; set; }

        protected override void Load()
        {
            using (var context = new TachographContext())
            {
                var documents = context.GetAllDocuments().Select(c => new DocumentHistoryItem(c));
                Documents = new ObservableCollection<IDocumentHistoryItem>(
                    documents.Concat(context.GetQCReports().Select(c => new DocumentHistoryItem(c)))
                             .Concat(context.GetReports<GV212Report>().Select(c => new DocumentHistoryItem(c)))
                             .OrderByDescending(c => c.Created));

                _originalDocumentHistoryItems = new ObservableCollection<IDocumentHistoryItem>(Documents);
            }

            DocumentTypes = new List<string>
            {
                Resources.TXT_SELECT_ALL,
                typeof (TachographDocument).Name.SplitByCapitals(),
                typeof (UndownloadabilityDocument).Name.SplitByCapitals(),
                typeof (LetterForDecommissioningDocument).Name.SplitByCapitals(),
                typeof (QCReport).Name.SplitByCapitals(),
                "QC 3 Month Walkaround",
                "GV 212"
            };

            SearchFilters = new List<string>
            {
                Resources.TXT_REGISTRATION_NUMBER,
                Resources.TXT_CUSTOMER,
                Resources.TXT_TECHNICIAN,
                Resources.TXT_OFFICE,
                Resources.TXT_DOCUMENT_TYPE
            };

            SelectedSearchFilter = SearchFilters.First();
            SelectedDocumentType = Resources.TXT_SELECT_ALL;
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
            OpenInReportFormCommand = new DelegateCommand<object>(OnOpenInReportForm, CanOpenInReportForm);
            EmailReportFormCommand = new DelegateCommand<object>(OnEmailReportSelected);
            PerformSearchCommand = new DelegateCommand<object>(OnPerformSearch);
            CreateGV212DocumentCommand = new DelegateCommand<object>(OnCreateGV212Document);
        }

        private bool CanOpenInReportForm(object obj)
        {
            return SelectedDocument != null && SelectedDocument.GV212Report == null;
        }

        private void OnEmailReportSelected(object obj)
        {
            if (SelectedDocument == null)
            {
                return;
            }

            SelectedDocument.Email();
        }

        private void OnCreateGV212Document(object obj)
        {
            GV212ReportHelper.Create(Documents, true);
        }

        private void OnReprintLabel(object obj)
        {
            if (SelectedDocument == null)
            {
                return;
            }

            SelectedDocument.PrintLabel();
        }

        private void OnReprintCertificate(object obj)
        {
            if (SelectedDocument == null)
            {
                return;
            }

            SelectedDocument.Print();
        }

        private void OnOpenInReportForm(object obj)
        {
            if (SelectedDocument == null)
            {
                return;
            }

            BaseNewDocumentViewModel viewModel = null;
            if (SelectedDocument.IsReport())
            {
                var report = SelectedDocument.Report as QCReport;
                if (report != null)
                {
                    viewModel = (QCCheckViewModel)MainWindow.ShowView<QCCheckView>();
                    var qcReportViewModel = (BaseNewDocumentViewModel<QCReportViewModel>)viewModel;
                    qcReportViewModel.Document = new QCReportViewModel(report);
                }

                var monthReport = SelectedDocument.Report as QCReport6Month;
                if (monthReport != null)
                {
                    viewModel = (QC6MonthCheckViewModel) MainWindow.ShowView<QC6MonthCheckView>();
                    var qc6MonthViewModel = (BaseNewDocumentViewModel<QCReport6Month>) viewModel;
                    qc6MonthViewModel.Document = monthReport;

                }
            }
            else
            {
                var tachographDocument = SelectedDocument.Document as TachographDocument;
                if (tachographDocument != null)
                {
                    viewModel = tachographDocument.IsDigital
                        ? (NewTachographViewModel)MainWindow.ShowView<NewTachographView>()
                        : (NewTachographViewModel)MainWindow.ShowView<NewAnalogueTachographView>();

                    var tachographHistoryViewModel = (NewTachographViewModel)viewModel;
                    tachographHistoryViewModel.Document = tachographDocument;
                    tachographHistoryViewModel.SetDocumentTypes(tachographDocument.IsDigital);
                    tachographHistoryViewModel.SelectedCustomerContact = viewModel.CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, tachographDocument.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
                }

                var undownloadabilityDocument = SelectedDocument.Document as UndownloadabilityDocument;
                if (undownloadabilityDocument != null)
                {
                    viewModel = (NewUndownloadabilityViewModel)MainWindow.ShowView<NewUndownloadabilityView>();
                    var undownloadabilityViewModel = (BaseNewDocumentViewModel<UndownloadabilityDocument>)viewModel;
                    undownloadabilityViewModel.Document = undownloadabilityDocument;
                }

                var letterForDecommissioningDocument = SelectedDocument.Document as LetterForDecommissioningDocument;
                if (letterForDecommissioningDocument != null)
                {
                    viewModel = (BaseNewDocumentViewModel<LetterForDecommissioningDocument>)MainWindow.ShowView<LetterForDecommissioningView>();
                    var letterForDecommissioningViewModel = (BaseNewDocumentViewModel<LetterForDecommissioningDocument>)viewModel;
                    letterForDecommissioningViewModel.Document = letterForDecommissioningDocument;
                }
            }

            if (viewModel != null)
            {
                viewModel.IsReadOnly = true;
                viewModel.IsHistoryMode = true;
            }
        }

        private void OnPerformSearch(object obj)
        {
            if (Documents == null)
            {
                Documents = new ObservableCollection<IDocumentHistoryItem>();
            }

            Documents.Clear();
            Documents.AddRange(_originalDocumentHistoryItems);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                switch (SearchFilters.IndexOf(SelectedSearchFilter))
                {
                    case 0:
                        Documents.Remove(item => item.RegistrationNumber == null || !item.RegistrationNumber.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 1:
                        Documents.Remove(item => item.CustomerContact == null || !item.CustomerContact.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 2:
                        Documents.Remove(item => item.TechnicianName == null || !item.TechnicianName.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 3:
                        Documents.Remove(item => item.Office == null || !item.Office.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 4:
                        Documents.Remove(item => item.DocumentType == null || !item.DocumentType.ToLower().Contains(SearchTerm.ToLower()));
                        break;
                }
            }

            if (SelectedDocumentType != Resources.TXT_SELECT_ALL)
            {
                Documents.Remove(item => item.Type != SelectedDocumentType);
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "SelectedDocument")
            {
                OpenInReportFormCommand.RaiseCanExecuteChanged();
            }
        }
    }
}