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
        public bool IsReadOnly { get; set; }

        protected override void Load()
        {
            base.Load();
            Populate();
        }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            UndownloadabilityRepository = GetInstance<IRepository<UndownloadabilityDocument>>();
            TachographMakesRepository = GetInstance<IRepository<TachographMake>>();
            TechnicianRepository = GetInstance<IRepository<Technician>>();

            if (string.IsNullOrEmpty(Document.DepotName))
            {
                Document.DepotName = RegistrationData.DepotName;
            }
        }

        protected override Document GetDocument()
        {
            return Document;
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
            ConnectHelper.Upload(Document, false);
        }

        protected override void OnCustomerContactChanged(CustomerContact customerContact)
        {
            Document.CustomerContact = customerContact == null ? null : customerContact.Name;
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber))
            {
                return;
            }
            
            if (!UndownloadabilityRepository.Any())
            {
                return;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            var match = UndownloadabilityRepository.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(doc => doc.Created)
                .FirstOrDefault();

            if (match != null)
            {
                Document = match;
                SelectedCustomerContact = CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, Document.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
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
            SelectedCustomerContact = CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, Document.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}