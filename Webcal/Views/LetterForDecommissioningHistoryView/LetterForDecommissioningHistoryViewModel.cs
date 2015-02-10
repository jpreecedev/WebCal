namespace Webcal.Views
{
    using System.Collections.ObjectModel;
    using System.IO;
    using Connect.Shared.Models;
    using Core;
    using Library.PDF;
    using Shared;
    using Shared.Helpers;

    public class LetterForDecommissioningHistoryViewModel : BaseHistoryViewModel
    {
        public IRepository<LetterForDecommissioningDocument> LetterForDecommissioningRepository { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        protected override void Load()
        {
            SearchFilters.RemoveAt(SearchFilters.Count - 1);
            Documents = new ObservableCollection<Document>(LetterForDecommissioningRepository.GetAll());
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null) return;

            var letterForDecommissioningViewModel = (LetterForDecommissioningViewModel) MainWindow.ShowView<LetterForDecommissioningView>();
            letterForDecommissioningViewModel.Document = (LetterForDecommissioningDocument) document;
            letterForDecommissioningViewModel.IsHistoryMode = true;
        }

        protected override void InitialiseRepositories()
        {
            LetterForDecommissioningRepository = GetInstance<IRepository<LetterForDecommissioningDocument>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        private void OnReprintCertificate(object obj)
        {
            var document = SelectedDocument as LetterForDecommissioningDocument;
            if (document == null) return;

            document.ToPDF().Print();
        }
    }
}