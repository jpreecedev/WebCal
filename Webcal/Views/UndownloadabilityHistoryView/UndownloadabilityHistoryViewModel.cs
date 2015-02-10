namespace Webcal.Views
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Library.PDF;
    using Shared;
    using Shared.Helpers;

    public class UndownloadabilityHistoryViewModel : BaseHistoryViewModel
    {
        public IRepository<UndownloadabilityDocument> UndownloadabilityDocumentsRepository { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        protected override void Load()
        {
            SearchFilters.RemoveAt(SearchFilters.Count - 1);
            Documents = new ObservableCollection<Document>(UndownloadabilityDocumentsRepository.GetAll().OrderByDescending(c => c.Created));
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null)
            {
                return;
            }

            var undownloadabilityViewModel = (NewUndownloadabilityViewModel) MainWindow.ShowView<NewUndownloadabilityView>();
            undownloadabilityViewModel.Document = (UndownloadabilityDocument) document;
            undownloadabilityViewModel.IsReadOnly = true;
            undownloadabilityViewModel.IsHistoryMode = true;
        }

        protected override void InitialiseRepositories()
        {
            UndownloadabilityDocumentsRepository = GetInstance<IRepository<UndownloadabilityDocument>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        private void OnReprintCertificate(object obj)
        {
            var document = SelectedDocument as UndownloadabilityDocument;
            if (document == null)
            {
                return;
            }

            document.ToPDF().Print();
        }
    }
}