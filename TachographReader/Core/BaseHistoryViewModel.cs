namespace TachographReader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Windows.DateRangePickerWindow;
    using Connect.Shared.Models;
    using Library;
    using Properties;

    public class BaseHistoryViewModel : BaseMainViewModel
    {
        private ObservableCollection<Document> _originalDocuments;
        private ObservableCollection<BaseReport> _originalReports; 

        public BaseHistoryViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            SearchFilters = new List<string>
            {
                Resources.TXT_REGISTRATION_NUMBER,
                Resources.TXT_CUSTOMER,
                Resources.TXT_TECHNICIAN,
                Resources.TXT_OFFICE,
                Resources.TXT_DOCUMENT_TYPE
            };

            SelectedSearchFilter = SearchFilters.First();
        }

        public List<string> SearchFilters { get; set; }
        public string SelectedSearchFilter { get; set; }
        public string SearchTerm { get; set; }
        public ObservableCollection<Document> Documents { get; set; }
        public ObservableCollection<BaseReport> Reports { get; set; }
        public Document SelectedDocument { get; set; }
        public BaseReport SelectedReport { get; set; }
        public DelegateCommand<object> OpenInReportFormCommand { get; set; }
        public DelegateCommand<object> EmailReportFormCommand { get; set; }
        public DelegateCommand<object> PerformSearchCommand { get; set; }
        public DelegateCommand<object> CreateVOSADocumentCommand { get; set; }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            OpenInReportFormCommand = new DelegateCommand<object>(OnOpenInReportForm);
            EmailReportFormCommand = new DelegateCommand<object>(OnEmailReportFormCommand);
            PerformSearchCommand = new DelegateCommand<object>(OnPerformSearch);
            CreateVOSADocumentCommand = new DelegateCommand<object>(OnCreateVOSADocument);
        }

        protected virtual void OnDocumentSelected(Document document)
        {
        }

        protected virtual void OnReportSelected(BaseReport report)
        {
        }

        protected virtual void OnEmailReportSelected(Document document)
        {
        }

        protected virtual void OnCreateVOSADocument(DateTime start, DateTime end)
        {
        }

        protected virtual void SearchReports()
        {
        }

        protected override void AfterLoad()
        {
            if (Documents != null)
            {
                _originalDocuments = new ObservableCollection<Document>(Documents);
            }

            if (Reports != null)
            {
                _originalReports = new ObservableCollection<BaseReport>(Reports);
            }
        }

        private void OnOpenInReportForm(object obj)
        {
            if (SelectedDocument != null)
            {
                OnDocumentSelected(SelectedDocument);
            }
            if (SelectedReport != null)
            {
                OnReportSelected(SelectedReport);
            }
        }

        private void OnEmailReportFormCommand(object obj)
        {
            OnEmailReportSelected(SelectedDocument);
        }

        private void OnPerformSearch(object obj)
        {
            if (SearchTerm == null)
            {
                return;
            }

            if (Documents != null)
            {
                Documents.Clear();
                Documents.AddRange(_originalDocuments);

                switch (SearchFilters.IndexOf(SelectedSearchFilter))
                {
                    case 0:
                        Documents.Remove(item => item.RegistrationNumber == null || !item.RegistrationNumber.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 1:
                        Documents.Remove(item => item.CustomerContact == null || !item.CustomerContact.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 2:
                        Documents.Remove(item => item.Technician == null || !item.Technician.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 3:
                        Documents.Remove(item => item.Office == null || !item.Office.ToLower().Contains(SearchTerm.ToLower()));
                        break;

                    case 4:
                        Documents.Remove(item => item.DocumentType == null || !item.DocumentType.ToLower().Contains(SearchTerm.ToLower()));
                        break;
                }
            }
            if (Reports != null)
            {
                Reports.Clear();
                Reports.AddRange(_originalReports);

                SearchReports();
            }
        }

        private void OnCreateVOSADocument(object obj)
        {
            var window = new DateRangePickerWindow();

            if (window.ShowDialog() == true)
            {
                var viewModel = window.DataContext as DateRangePickerWindowViewModel;
                if (viewModel == null)
                {
                    return;
                }

                var end = DateTime.Parse($"{viewModel.EndDateTime.ToString(Constants.DateFormat)} 23:59:59");
                OnCreateVOSADocument(viewModel.StartDateTime, end);
            }
        }
    }
}