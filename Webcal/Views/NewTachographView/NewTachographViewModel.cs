namespace Webcal.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.Linq;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;

    public class NewTachographViewModel : BaseNewDocumentViewModel
    {
        public bool CardBeingRead = false;

        public NewTachographViewModel()
        {
            Document = new TachographDocument();

            VehicleTypes = VehicleType.GetVehicleTypes();
            Document.VehicleType = VehicleTypes.First();
        }

        public TachographDocument Document { get; set; }
        public IRepository<VehicleMake> VehicleRepository { get; set; }
        public IRepository<TyreSize> TyreSizesRepository { get; set; }
        public IRepository<TachographMake> TachographMakesRepository { get; set; }
        public IRepository<Technician> TechniciansRepository { get; set; }
        public IRepository<TachographDocument> TachographDocumentRepository { get; set; }
        public IRepository<WorkshopCardFile> WorkshopCardFilesRepository { get; set; }
        public ObservableCollection<string> DocumentTypes { get; set; }
        public ObservableCollection<string> VehicleTypes { get; set; }
        public ObservableCollection<VehicleMake> VehicleMakes { get; set; }
        public ObservableCollection<TyreSize> TyreSizes { get; set; }
        public ObservableCollection<TachographMake> TachographMakes { get; set; }
        public ObservableCollection<Technician> Technicians { get; set; }
        public ICollection<TachographDocument> AllTachographDocuments { get; set; }
        public bool IsReadFromCardEnabled { get; set; }
        public string ReadFromCardContent { get; set; }
        public string StatusText { get; set; }
        public bool IsCardReadUserInitiated { get; set; }
        public bool IsReadOnly { get; set; }
        public string LastPlateRead { get; set; }
        public DelegateCommand<object> ReadFromCardCommand { get; set; }
        public DelegateCommand<Grid> PrintLabelCommand { get; set; }

        public void SetDocumentTypes(bool isDigital)
        {
            Document.IsDigital = isDigital;

            DocumentTypes = DocumentType.GetDocumentTypes(isDigital);
            Document.DocumentType = Document.DocumentType ?? DocumentTypes.First();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReadFromCardCommand = new DelegateCommand<object>(OnReadFromCard);
            PrintLabelCommand = new DelegateCommand<Grid>(OnPrintLabel);
        }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            VehicleRepository = ContainerBootstrapper.Container.GetInstance<IRepository<VehicleMake>>();
            TyreSizesRepository = ContainerBootstrapper.Container.GetInstance<IRepository<TyreSize>>();
            TachographMakesRepository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographMake>>();
            TechniciansRepository = ContainerBootstrapper.Container.GetInstance<IRepository<Technician>>();
            TachographDocumentRepository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographDocument>>();
            WorkshopCardFilesRepository = ContainerBootstrapper.Container.GetInstance<IRepository<WorkshopCardFile>>();
        }

        protected override void Load()
        {
            base.Load();

            SwitchReadButtonState(true);
            Populate();
        }

        public override void OnModalClosed()
        {
            Populate();
        }

        protected override void Add()
        {
            Document.Created = DateTime.Now;

            if (Document.CalibrationTime == null)
            {
                Document.CalibrationTime = DateTime.Now;
            }

            TachographDocumentRepository.AddOrUpdate(Document);
            TachographDocumentRepository.Save();
            WorkshopCardFilesRepository.Save();
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber))
            {
                return;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", "").ToUpper();

            ICollection<TachographDocument> allDocuments = AllTachographDocuments ?? (AllTachographDocuments = TachographDocumentRepository.GetAll().OrderByDescending(c => c.Created).ToList());
            if (!allDocuments.IsNullOrEmpty())
            {
                TachographDocument match = allDocuments.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(doc => doc.Created)
                    .FirstOrDefault();

                if (match == null)
                {
                    return;
                }
                Document.CalibrationTime = DateTime.Now;
                Document.CardSerialNumber = match.CardSerialNumber;
                Document.Created = DateTime.Now;
                Document.CustomerContact = match.CustomerContact;
                Document.InspectionDate = DateTime.Now;
                Document.InspectionInfo = match.InspectionInfo;
                Document.IsDigital = match.IsDigital;
                Document.MinorWorkDetails = match.MinorWorkDetails;
                Document.Office = match.Office;
                Document.RegistrationNumber = match.RegistrationNumber;
                Document.SerialNumber = match.SerialNumber;
                Document.TachographAdapterLocation = match.TachographAdapterLocation;
                Document.TachographAdapterSerialNumber = match.TachographAdapterSerialNumber;
                Document.TachographCableColor = match.TachographCableColor;
                Document.TachographHasAdapter = match.TachographHasAdapter;
                Document.TachographMake = match.TachographMake;
                Document.TachographModel = match.TachographModel;
                Document.TachographType = match.TachographType;
                Document.Tampered = match.Tampered;
                Document.TyreSize = match.TyreSize;
                Document.VehicleMake = match.VehicleMake;
                Document.VehicleModel = match.VehicleModel;
                Document.VehicleType = match.VehicleType;
                Document.VIN = match.VIN;
            }
        }

        protected override void OnFastReadCompleted(object sender, DriverCardCompletedEventArgs e)
        {
            CardBeingRead = false;

            if (!e.IsSuccess)
            {
                StatusText = Resources.TXT_UNABLE_READ_SMART_CARD;
                SwitchReadButtonState(true);
                return;
            }

            if (e.CalibrationRecord == null)
            {
                if (IsCardReadUserInitiated)
                {
                    SwitchReadButtonState(true);
                    ShowError(Resources.EXC_NO_SMART_CARD_READERS_FOUND);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(LastPlateRead))
                {
                    LastPlateRead = "";
                }

                if (LastPlateRead != e.CalibrationRecord.VehicleRegistrationNumber || !e.AutoRead)
                {
                    LastPlateRead = e.CalibrationRecord.VehicleRegistrationNumber;

                    if (RegistrationChangedCommand != null)
                    {
                        RegistrationChangedCommand.Execute(e.CalibrationRecord.VehicleRegistrationNumber);
                    }

                    MainWindow.IsNavigationLocked = true;

                    Document.Convert(e.CalibrationRecord);

                    PrintLabel(Document);

                    StatusText = Resources.TXT_GENERATING_WORKSHOP_CARD_FILE;
                    DriverCardReader.GenerateDump();
                }
            }
        }

        protected override void OnDumpCompleted(object sender, DriverCardCompletedEventArgs e)
        {
            CardBeingRead = false;

            string[] cardDetails = DisplayWorkshopCardDetails(e.DumpFilePath);
            if (cardDetails != null)
            {
                WorkshopCardFile workshopCardFile = WorkshopCardFile.GetWorkshopCardFile(DateTime.Now, cardDetails[1], cardDetails[0]);
                WorkshopCardFilesRepository.Add(workshopCardFile.Clone<WorkshopCardFile>());
                StatusText = Resources.TXT_WORKSHOP_CARD_FILE_GENERATED;
            }
            else
            {
                StatusText = Resources.TXT_UNABLE_GENERATE_WORKSHOP_CARD;
                MessageBoxHelper.ShowMessage(Resources.ERR_UNABLE_READ_SMART_CARD);
                MainWindow.IsNavigationLocked = false;
            }

            SwitchReadButtonState(true);
        }

        protected override void OnProgress(object sender, DriverCardProgressEventArgs e)
        {
            StatusText = e.Message;
        }

        protected override bool IncludeDeletedContacts
        {
            get { return IsHistoryMode; }
        }

        private void OnReadFromCard(object obj)
        {
            if (!CardBeingRead)
            {
                CardBeingRead = true;

                try
                {
                    IsCardReadUserInitiated = obj == null;
                    SwitchReadButtonState(false);

                    DriverCardReader.FastRead(obj != null);
                }
                catch (AggregateException aggregateEx)
                {
                    SwitchReadButtonState(true);
                    ShowError("{0}\n\n{1}", Resources.EXC_ERROR_WHILST_READING_SMART_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, aggregateEx));
                }
                catch (Exception ex)
                {
                    SwitchReadButtonState(true);
                    ShowError("{0}\n\n{1}", Resources.EXC_ERROR_WHILST_READING_SMART_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
                }
            }
        }

        private void OnPrintLabel(DependencyObject root)
        {
            if (root == null)
            {
                return;
            }

            if (!IsValid(root))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            PrintLabel(Document);
        }

        private void SwitchReadButtonState(bool isEnabled)
        {
            if (isEnabled)
            {
                ReadFromCardContent = Resources.TXT_READ_FROM_CARD;
                IsReadFromCardEnabled = true;
            }
            else
            {
                IsReadFromCardEnabled = false;
                ReadFromCardContent = Resources.TXT_READING;
            }
        }

        private void Populate()
        {
            TyreSizes = new ObservableCollection<TyreSize>(TyreSizesRepository.GetAll());
            VehicleMakes = new ObservableCollection<VehicleMake>(VehicleRepository.GetAll("Models"));
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll());
            Technicians = new ObservableCollection<Technician>(TechniciansRepository.GetAll());

            Technician defaultTechnician = Technicians.FirstOrDefault(technician => technician != null && technician.IsDefault);
            if (defaultTechnician != null)
            {
                Document.Technician = defaultTechnician.Name;
            }
        }

        private string[] DisplayWorkshopCardDetails(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                XDocument document = XDocument.Parse(xml);
                XElement first = document.Descendants("CardDump").FirstOrDefault();

                var result = new List<string>();
                if (first != null)
                {
                    result.Add(first.Element("TempPath").SafelyGetValue());
                    result.Add(first.Element("WorkshopName").SafelyGetValue());
                }

                if (!File.Exists(result[0]))
                {
                    MessageBoxHelper.ShowError(Resources.EXC_UNABLE_GENERATE_CARD_DUMP);
                    return null;
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_READ_SMART_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
            }

            return null;
        }

        private static void PrintLabel(TachographDocument document)
        {
            if (document.CalibrationTime == null)
            {
                document.CalibrationTime = DateTime.Now;
            }

            LabelHelper.Print(document);
        }
    }
}