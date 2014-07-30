using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Windows.DateRangePickerWindow;
using Webcal.Properties;

namespace Webcal.Core
{
    public class BaseHistoryViewModel : BaseMainViewModel
    {
        #region Private Members

        private ObservableCollection<Document> _originalDocuments;

        #endregion

        #region Constructor

        public BaseHistoryViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

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

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            OpenInReportFormCommand = new DelegateCommand<object>(OnOpenInReportForm);
            EmailReportFormCommand = new DelegateCommand<object>(OnEmailReportFormCommand);
            PerformSearchCommand = new DelegateCommand<object>(OnPerformSearch);
            CreateVOSADocumentCommand = new DelegateCommand<object>(OnCreateVOSADocument);
        }

        #endregion

        #region Public Properties

        public List<string> SearchFilters { get; set; }

        public string SelectedSearchFilter { get; set; }

        public string SearchTerm { get; set; }

        public ObservableCollection<Document> Documents { get; set; }

        public Document SelectedDocument { get; set; }

        #endregion

        #region Protected Methods

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

        #endregion

        #region Commands

        #region Command : Open in report form

        public DelegateCommand<object> OpenInReportFormCommand { get; set; }

        private void OnOpenInReportForm(object obj)
        {
            OnDocumentSelected(SelectedDocument);
        }

        #endregion

        public DelegateCommand<object> EmailReportFormCommand { get; set; }

        private void OnEmailReportFormCommand(object obj)
        {
            OnEmailReportSelected(SelectedDocument);
        }

        #region Command : Perform Search

        public DelegateCommand<object> PerformSearchCommand { get; set; }

        private void OnPerformSearch(object obj)
        {
            if (SearchTerm == null) return;

            Documents.Clear();
            Documents.AddRange(_originalDocuments);

            switch (SearchFilters.IndexOf(SelectedSearchFilter))
            {
                case 0:
                    Documents.Remove(item => !item.RegistrationNumber.Contains(SearchTerm));
                    break;

                case 1:
                    Documents.Remove(item => !item.CustomerContact.Contains(SearchTerm));
                    break;

                case 2:
                    Documents.Remove(item => !item.Technician.Contains(SearchTerm));
                    break;

                case 3:
                    Documents.Remove(item => !item.Office.Contains(SearchTerm));
                    break;

                case 4:
                    Documents.Remove(item => !item.DocumentType.Contains(SearchTerm));
                    break;
            }
        }

        #endregion

        #region Command : Create VOSA Document

        public DelegateCommand<object> CreateVOSADocumentCommand { get; set; }

        private void OnCreateVOSADocument(object obj)
        {
            DateRangePickerWindow window = new DateRangePickerWindow();

            if (window.ShowDialog() == true)
            {
                DateRangePickerWindowViewModel viewModel = window.DataContext as DateRangePickerWindowViewModel;
                if (viewModel == null)
                    return;

                DateTime end = DateTime.Parse(string.Format("{0} 23:59:59", viewModel.EndDateTime.ToString(Core.Constants.DateFormat)));
                OnCreateVOSADocument(viewModel.StartDateTime, end);
            }
        }

        #endregion

        #endregion
    }
}
