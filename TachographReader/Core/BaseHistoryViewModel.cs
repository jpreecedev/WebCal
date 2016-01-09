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
    using DataModel;
    using Library;
    using Properties;

    public class BaseHistoryViewModel : BaseMainViewModel
    {
        private ObservableCollection<Document> _originalDocuments;

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
        public Document SelectedDocument { get; set; }
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

        protected virtual void OnEmailReportSelected(Document document)
        {
        }

        protected virtual void OnCreateVOSADocument(DateTime start, DateTime end)
        {
        }

        protected override void AfterLoad()
        {
            _originalDocuments = new ObservableCollection<Document>(Documents);
        }

        private void OnOpenInReportForm(object obj)
        {
            OnDocumentSelected(SelectedDocument);
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

                DateTime end = DateTime.Parse(string.Format("{0} 23:59:59", viewModel.EndDateTime.ToString(Constants.DateFormat)));
                OnCreateVOSADocument(viewModel.StartDateTime, end);
            }
        }
    }
}