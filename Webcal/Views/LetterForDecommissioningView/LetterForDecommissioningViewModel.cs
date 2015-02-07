namespace Webcal.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Shared;

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

        public ICollection<LetterForDecommissioningDocument> AlLetterForDecommissioningDocuments { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            Populate();
        }

        protected override void InitialiseRepositories()
        {
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
            LetterForDecommissioningRepository.Save();
            ConnectHelper.Upload(Document);
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            ICollection<LetterForDecommissioningDocument> allDocuments = AlLetterForDecommissioningDocuments ?? (AlLetterForDecommissioningDocuments = LetterForDecommissioningRepository.GetAll());
            if (!allDocuments.IsNullOrEmpty())
            {
                LetterForDecommissioningDocument match = allDocuments.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(doc => doc.Created)
                    .FirstOrDefault();

                if (match != null)
                    Document = match;
            }
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