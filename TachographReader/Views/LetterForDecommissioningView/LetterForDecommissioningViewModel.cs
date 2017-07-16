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
    using System.Windows.Controls;

    public class LetterForDecommissioningViewModel : BaseNewDocumentViewModel<LetterForDecommissioningDocument>
    {
        public LetterForDecommissioningViewModel()
        {
            Document = new LetterForDecommissioningDocument();
        }

        public LetterForDecommissioningViewModel(LetterForDecommissioningDocument document)
        {
            Document = document;
        }
        
        public IRepository<LetterForDecommissioningDocument> LetterForDecommissioningRepository { get; set; }

        public IRepository<TachographMake> TachographMakesRepository { get; set; }

        public ObservableCollection<TachographMake> TachographMakes { get; set; }

        public IRepository<Technician> TechnicianRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }
        
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

            if (string.IsNullOrEmpty(Document.DepotName))
            {
                Document.DepotName = RegistrationData.DepotName;
            }
        }

        protected override Document GetDocument(Grid root)
        {
            CustomerContactHelper.CreateCustomerContactIfRequired(Document, root);
            return Document;
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

        protected override void Update()
        {
            LetterForDecommissioningRepository.AddOrUpdate(Document);
            ConnectHelper.Upload(Document, true);
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

            if (!LetterForDecommissioningRepository.Any())
            {
                return;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            var match = LetterForDecommissioningRepository.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(doc => doc.Created)
                .FirstOrDefault();

            if (match != null)
            {
                Document = match;
                SelectedCustomerContact = CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, match.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
            }
        }
        
        private void Populate()
        {
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll());
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