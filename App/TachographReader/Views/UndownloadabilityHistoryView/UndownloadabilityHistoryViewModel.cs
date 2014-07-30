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
    public class UndownloadabilityHistoryViewModel : BaseHistoryViewModel
    {
        #region Public Properties

        public IRepository<UndownloadabilityDocument> UndownloadabilityDocumentsRepository { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            SearchFilters.RemoveAt(SearchFilters.Count - 1);
            Documents = new ObservableCollection<Document>(UndownloadabilityDocumentsRepository.GetAll());
        }

        protected override void OnDocumentSelected(Document document)
        {
            if (document == null) return;

            NewUndownloadabilityViewModel undownloadabilityViewModel = (NewUndownloadabilityViewModel)MainWindow.ShowView<NewUndownloadabilityView>();
            undownloadabilityViewModel.Document = (UndownloadabilityDocument)document;
            undownloadabilityViewModel.IsReadOnly = true;
        }

        protected override void InitialiseRepositories()
        {
            UndownloadabilityDocumentsRepository = ObjectFactory.GetInstance<IRepository<UndownloadabilityDocument>>();
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
            UndownloadabilityDocument document = SelectedDocument as UndownloadabilityDocument;
            if (document == null) return;

            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
            }
        }

        #endregion

        #endregion
    }
}
