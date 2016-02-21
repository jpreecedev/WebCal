namespace TachographReader.Views
{
    using System;
    using System.Collections.ObjectModel;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using Library;
    using Shared;

    public class QC3MonthCheckViewModel : BaseNewDocumentViewModel
    {
        public QC3MonthCheckViewModel()
        {
            Document = new QCReport3Month();
        }

        public QCReport3Month Document { get; set; }

        public bool IsReadOnly { get; set; }

        public IRepository<QCReport3Month> Repository { get; set; }

        public IRepository<Technician> TechniciansRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            Repository = GetInstance<IRepository<QCReport3Month>>();
            TechniciansRepository = GetInstance<IRepository<Technician>>();
        }

        protected override void Add()
        {
            if (IsReadOnly || IsHistoryMode)
            {
                return;
            }

            Document.Created = DateTime.Now;
            Repository.AddOrUpdate(Document);
            ConnectHelper.Upload(Document);
        }

        protected override BaseReport GetReport()
        {
            return Document;
        }

        protected override void Load()
        {
            base.Load();
            Populate();
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        private void Populate()
        {
            Technicians = new ObservableCollection<Technician>(TechniciansRepository.GetAll());
            Document.TachoCentreName = WorkshopSettings.WorkshopName;
            Document.CentreSealNumber = RegistrationData.SealNumber;
        }
    }
}