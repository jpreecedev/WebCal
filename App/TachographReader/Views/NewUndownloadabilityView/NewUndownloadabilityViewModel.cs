using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StructureMap;
using Webcal.Shared;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;

namespace Webcal.Views
{
    public class NewUndownloadabilityViewModel : BaseNewDocumentViewModel
    {
        #region Constructors

        public NewUndownloadabilityViewModel()
        {
            Document = new UndownloadabilityDocument();
        }

        public NewUndownloadabilityViewModel(UndownloadabilityDocument document)
        {
            Document = document;
        }

        #endregion

        #region Public Properties

        public UndownloadabilityDocument Document { get; set; }

        public IRepository<UndownloadabilityDocument> UndownloadabilityRepository { get; set; }

        public IRepository<TachographMake> TachographMakesRepository { get; set; }

        public ObservableCollection<TachographMake> TachographMakes { get; set; }

        public IRepository<Technician> TechnicianRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }

        public ICollection<UndownloadabilityDocument> AllUndownloadabilityDocuments { get; set; }

        public bool IsReadOnly { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            Populate();
        }

        protected override void InitialiseRepositories()
        {
            UndownloadabilityRepository = ObjectFactory.GetInstance<IRepository<UndownloadabilityDocument>>();
            TachographMakesRepository = ObjectFactory.GetInstance<IRepository<TachographMake>>();
            TechnicianRepository = ObjectFactory.GetInstance<IRepository<Technician>>();
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        protected override void Add()
        {
            if (IsReadOnly) return;

            Document.Created = DateTime.Now;
            UndownloadabilityRepository.Add(Document);
            UndownloadabilityRepository.Save();
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            ICollection<UndownloadabilityDocument> allDocuments = AllUndownloadabilityDocuments ?? (AllUndownloadabilityDocuments = UndownloadabilityRepository.GetAll());
            if (!allDocuments.IsNullOrEmpty())
            {
                UndownloadabilityDocument match = allDocuments.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
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
