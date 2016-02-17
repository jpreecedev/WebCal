namespace TachographReader.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using Library;
    using Library.PDF;
    using Library.ViewModels;
    using Properties;
    using Shared;

    public class QCCheckHistoryViewModel : BaseHistoryViewModel
    {
        public IRepository<QCReport> Repository { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        protected override void Load()
        {
            SearchFilters = new List<string>
            {
                Resources.TXT_REGISTRATION_NUMBER,
                Resources.TXT_TECHNICIAN
            };

            SelectedSearchFilter = SearchFilters.First();

            Reports = new ObservableCollection<BaseReport>(Repository.GetAll());
        }
        
        protected override void OnReportSelected(BaseReport report)
        {
            if (report == null) return;

            var qcCheckViewModel = (QCCheckViewModel)MainWindow.ShowView<QCCheckView>();
            qcCheckViewModel.Document = new QCReportViewModel((QCReport)report);
            qcCheckViewModel.IsHistoryMode = true;
            qcCheckViewModel.IsReadOnly = true;
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<QCReport>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        protected override void SearchReports()
        {
            switch (SearchFilters.IndexOf(SelectedSearchFilter))
            {
                case 0:
                    Reports = new ObservableCollection<BaseReport>(Reports.OfType<QCReport>().ToList().Remove(item => item.VehicleRegistrationNumber == null || !item.VehicleRegistrationNumber.ToLower().Contains(SearchTerm.ToLower())));
                    break;
                    
                case 1:
                    Reports = new ObservableCollection<BaseReport>(Reports.OfType<QCReport>().ToList().Remove(item => item.TechnicianName == null || !item.TechnicianName.ToLower().Contains(SearchTerm.ToLower())));
                    break;
            }
        }

        private void OnReprintCertificate(object obj)
        {
            var document = new QCReportViewModel((QCReport)SelectedReport);
            document.ToReportPDF().Print();
        }
    }
}