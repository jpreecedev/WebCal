using System.Collections.ObjectModel;
using System.IO;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Library.PDF;
using Webcal.Shared;

namespace Webcal.Views
{
    using DataModel.Core;

    public class LetterForDecommissioningHistoryViewModel : BaseHistoryViewModel
    {
        #region Public Properties

        public IRepository<LetterForDecommissioningDocument> LetterForDecommissioningRepository { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            SearchFilters.RemoveAt(SearchFilters.Count - 1);
            Documents = new ObservableCollection<Document>(LetterForDecommissioningRepository.GetAll());
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null) return;

            LetterForDecommissioningViewModel letterForDecommissioningViewModel = (LetterForDecommissioningViewModel)MainWindow.ShowView<LetterForDecommissioningView>();
            letterForDecommissioningViewModel.Document = (LetterForDecommissioningDocument)document;
            letterForDecommissioningViewModel.IsHistoryMode = true;
        }

        protected override void InitialiseRepositories()
        {
            LetterForDecommissioningRepository = ContainerBootstrapper.Container.GetInstance<IRepository<LetterForDecommissioningDocument>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        #endregion

        #region Commands

        #region Command : Reprint Certificate

        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        private void OnReprintCertificate(object obj)
        {
            LetterForDecommissioningDocument document = SelectedDocument as LetterForDecommissioningDocument;
            if (document == null) return;

            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(ImageHelper.GetTemporaryDirectory(), "document.pdf"));
            }
        }

        #endregion

        #endregion
    }
}
