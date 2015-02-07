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
    using DataModel.Library;
    using Library;
    using Shared;

    public class NewUndownloadabilityViewModel : BaseNewDocumentViewModel
    {
        public NewUndownloadabilityViewModel()
        {
            Document = new UndownloadabilityDocument();
        }

        public NewUndownloadabilityViewModel(UndownloadabilityDocument document)
        {
            Document = document;
        }

        public UndownloadabilityDocument Document { get; set; }
        public IRepository<UndownloadabilityDocument> UndownloadabilityRepository { get; set; }
        public IRepository<TachographMake> TachographMakesRepository { get; set; }
        public ObservableCollection<TachographMake> TachographMakes { get; set; }
        public IRepository<Technician> TechnicianRepository { get; set; }
        public ObservableCollection<Technician> Technicians { get; set; }
        public ICollection<UndownloadabilityDocument> AllUndownloadabilityDocuments { get; set; }
        public bool IsReadOnly { get; set; }

        protected override void Load()
        {
            base.Load();
            Populate();
        }

        protected override void InitialiseRepositories()
        {
            UndownloadabilityRepository = GetInstance<IRepository<UndownloadabilityDocument>>();
            TachographMakesRepository = GetInstance<IRepository<TachographMake>>();
            TechnicianRepository = GetInstance<IRepository<Technician>>();
            WorkshopSettings = GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            MailSettings = GetInstance<ISettingsRepository<MailSettings>>().Get();
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        protected override void Add()
        {
            if (IsReadOnly)
            {
                return;
            }

            Document.Created = DateTime.Now;
            UndownloadabilityRepository.Add(Document);
            UndownloadabilityRepository.Save();
            ConnectHelper.Upload(Document);
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", string.Empty).ToUpper();

            ICollection<UndownloadabilityDocument> allDocuments = AllUndownloadabilityDocuments ?? (AllUndownloadabilityDocuments = UndownloadabilityRepository.GetAll().OrderBy(c => c.Created).ToList());
            if (!allDocuments.IsNullOrEmpty())
            {
                UndownloadabilityDocument match = allDocuments.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(doc => doc.Created)
                    .FirstOrDefault();

                if (match != null)
                {
                    Document = match;
                }
            }
        }

        private void Populate()
        {
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll("Models"));
            Technicians = new ObservableCollection<Technician>(TechnicianRepository.GetAll());

            Technician defaultTechnician = Technicians.FirstOrDefault(technician => technician != null && technician.IsDefault);
            if (defaultTechnician != null)
            {
                Document.Technician = defaultTechnician.Name;
            }
        }
    }
}