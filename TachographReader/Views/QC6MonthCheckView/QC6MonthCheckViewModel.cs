namespace TachographReader.Views
{
    using System;
    using System.Collections.ObjectModel;
    using Windows;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Library;
    using Shared;

    public class QC6MonthCheckViewModel : BaseNewDocumentViewModel<QCReport6Month>
    {
        public QC6MonthCheckViewModel()
        {
            Document = new QCReport6Month {Date = DateTime.Now};
        }

        public IRepository<QCReport6Month> Repository { get; set; }

        public IRepository<Technician> TechniciansRepository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            Repository = GetInstance<IRepository<QCReport6Month>>();
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

            var repository = GetInstance<ISettingsRepository<WorkshopSettings>>();
            var workshopSettings = repository.GetWorkshopSettings();
            if (Document.Created.Date > workshopSettings.CentreQuarterlyCheckDate)
            {
                workshopSettings.CentreQuarterlyCheckDate = Document.Created.Date;
                repository.Save(workshopSettings);
            }
        }

        protected override void Update()
        {
            Repository.AddOrUpdate(Document);
            ConnectHelper.Upload(Document, true);
        }

        protected override BaseReport GetReport()
        {
            return Document;
        }

        protected override void Load()
        {
            base.Load();

            Populate();

            if (string.IsNullOrEmpty(Document.CentreName) || string.IsNullOrEmpty(Document.CentreSealNumber) || Document.CentreName.Length < 3 ||  Document.CentreSealNumber.Length < 3)
            {
                ShowError("You must set the 'Workshop Name' and 'Seal Number' before generating this report.  Please set them first using the Settings dialog.\n\nPlease note, both must be at least 3 characters long.");
                MainWindow.ShowView<HomeScreenView>();
            }
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        private void Populate()
        {
            Technicians = new ObservableCollection<Technician>(TechniciansRepository.GetAll());
            Document.CentreName = WorkshopSettings.WorkshopName;
            Document.CentreSealNumber = RegistrationData.SealNumber;
        }
    }
}