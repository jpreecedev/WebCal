namespace TachographReader.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using Library;
    using Shared;
    using DocumentType = Connect.Shared.DocumentType;

    public class LetterForDecommissioningViewModel : BaseNewDocumentViewModel
    {
        #region Constructors

        public LetterForDecommissioningViewModel()
        {
            Document = new LetterForDecommissioningDocument();
        }

        public LetterForDecommissioningViewModel(LetterForDecommissioningDocument document)
        {
            Document = document;
        }

        #endregion

        #region Public Properties

        public LetterForDecommissioningDocument Document { get; set; }

        public IRepository<LetterForDecommissioningDocument> LetterForDecommissioningRepository { get; set; }

        public IRepository<TachographMake> TachographMakesRepository { get; set; }

        public ObservableCollection<TachographMake> TachographMakes { get; set; }

        public IRepository<Technician> TechnicianRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            Populate();
        }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            LetterForDecommissioningRepository = GetInstance<IRepository<LetterForDecommissioningDocument>>();
            TachographMakesRepository = GetInstance<IRepository<TachographMake>>();
            TechnicianRepository = GetInstance<IRepository<Technician>>();
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        protected override void Add()
        {
            if (IsHistoryMode) return;

            Document.Created = DateTime.Now;
            LetterForDecommissioningRepository.Add(Document);
            ConnectHelper.Upload(Document);
        }

        protected override void OnFoundDocumentOnConnect(Document document)
        {
            var letterForDecommissioningDocument = document as LetterForDecommissioningDocument;
            if (letterForDecommissioningDocument != null)
            {
                Document = letterForDecommissioningDocument;
            }
        }

        protected override DocumentType GetDocumentType()
        {
            return DocumentType.LetterForDecommissioning;
        }

        protected override bool RegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber))
            {
                return false;
            }

            if (!LetterForDecommissioningRepository.Any())
            {
                return false;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            var match = LetterForDecommissioningRepository.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(doc => doc.Created)
                .FirstOrDefault();

            if (match != null)
            {
                Document = match;
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void Populate()
        {
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll());
            Technicians = new ObservableCollection<Technician>(TechnicianRepository.GetAll());

            Technician defaultTechnician = Technicians.FirstOrDefault(technician => technician != null && technician.IsDefault);
            if (defaultTechnician != null)
            {
                Document.Technician = defaultTechnician.Name;
            }
        }

        #endregion
    }
}