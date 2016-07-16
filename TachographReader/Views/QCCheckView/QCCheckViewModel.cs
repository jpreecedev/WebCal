namespace TachographReader.Views
{
    using System;
    using System.Collections.ObjectModel;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using Library;
    using Library.ViewModels;
    using Shared;

    public class QCCheckViewModel : BaseNewDocumentViewModel<QCReportViewModel>
    {
        public QCCheckViewModel()
        {
            Document = new QCReportViewModel();
        }
        
        public IRepository<QCReport> Repository { get; set; }

        public IRepository<TachographMake> TachographMakesRepository { get; set; }

        public ObservableCollection<TachographMake> TachographMakes { get; set; }

        public ObservableCollection<VehicleMake> VehicleMakes { get; set; }
        
        public IRepository<VehicleMake> VehicleRepository { get; set; }

        public IRepository<Technician> TechniciansRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }
        
        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            Repository = GetInstance<IRepository<QCReport>>();
            TachographMakesRepository = GetInstance<IRepository<TachographMake>>();
            VehicleRepository = GetInstance<IRepository<VehicleMake>>();
            TechniciansRepository = GetInstance<IRepository<Technician>>();
        }

        protected override BaseReport GetReport()
        {
            return Document;
        }

        protected override void Add()
        {
            if (IsReadOnly || IsHistoryMode)
            {
                return;
            }

            Document.Created = DateTime.Now;
            Repository.AddOrUpdate(Document.Downcast<QCReport>());
            ConnectHelper.Upload(Document.Downcast<QCReport>());
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

        public void PopulateFromCalibration(TachographDocument document)
        {
            if (document == null || !document.IsQCCheck)
            {
                return;
            }

            Document = new QCReportViewModel(document);
        }

        private void Populate()
        {
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll("Models"));
            VehicleMakes = new ObservableCollection<VehicleMake>(VehicleRepository.GetAll("Models"));
            Technicians = new ObservableCollection<Technician>(TechniciansRepository.GetAll());

            Document.TachoCentreName = WorkshopSettings.WorkshopName;
            Document.TachoCentreLine1 = WorkshopSettings.Address1;
            Document.TachoCentreLine2 = WorkshopSettings.Address2;
            Document.TachoCentreLine3 = WorkshopSettings.Address3;
            Document.TachoCentreCity = WorkshopSettings.Town;
            Document.TachoCentrePostCode = WorkshopSettings.PostCode;

            Document.IsUILocked = IsHistoryMode;
        }
    }
}