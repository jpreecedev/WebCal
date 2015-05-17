namespace TachographReader.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class NewTachographViewModel : BaseNewDocumentViewModel
    {
        public bool CardBeingRead;

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

            var settings = GetInstance<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();

            DocumentTypes = DocumentType.GetDocumentTypes(isDigital);
            Document.DocumentType = Document.DocumentType ?? DocumentTypes.FirstOrDefault(c => string.Equals(c, isDigital ? settings.DefaultDigitalDocumentType : settings.DefaultAnalogueDocumentType));
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

            VehicleRepository = GetInstance<IRepository<VehicleMake>>();
            TyreSizesRepository = GetInstance<IRepository<TyreSize>>();
            TachographMakesRepository = GetInstance<IRepository<TachographMake>>();
            TechniciansRepository = GetInstance<IRepository<Technician>>();
            TachographDocumentRepository = GetInstance<IRepository<TachographDocument>>();
            WorkshopCardFilesRepository = GetInstance<IRepository<WorkshopCardFile>>();
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
            ConnectHelper.Upload(Document);
        }

        protected override void OnFoundDocumentOnConnect(Document document)
        {
            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                Document = tachographDocument;
            }
        }

        protected override Connect.Shared.DocumentType GetDocumentType()
        {
            return Connect.Shared.DocumentType.Tachograph;
        }

        protected override bool RegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber))
            {
                return false;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", string.Empty).ToUpper();

            if (!TachographDocumentRepository.Any())
            {
                return false;
            }

            TachographDocument match = TachographDocumentRepository.Where(doc => string.Equals(doc.RegistrationNumber, Document.RegistrationNumber, StringComparison.CurrentCultureIgnoreCase))
                .OrderByDescending(doc => doc.Created)
                .FirstOrDefault();

            if (match == null && registrationNumber.Length > 8)
            {
                match = TachographDocumentRepository.Where(doc => string.Equals(doc.VIN, registrationNumber, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(doc => doc.Created)
                    .FirstOrDefault();
            }

            if (match == null)
            {
                return false;
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
            Document.NewBattery = match.NewBattery;

            return true;
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
                    LastPlateRead = string.Empty;
                }

                if (LastPlateRead != e.CalibrationRecord.VehicleRegistrationNumber || !e.AutoRead)
                {
                    LastPlateRead = e.CalibrationRecord.VehicleRegistrationNumber;

                    if (RegistrationChangedCommand != null)
                    {
                        RegistrationChangedCommand.Execute(GetRegistrationToQuery(e.CalibrationRecord));
                    }

                    MainWindow.IsNavigationLocked = true;

                    Document.Convert(e.CalibrationRecord);

                    if (!Technicians.IsNullOrEmpty() && !string.IsNullOrEmpty(e.CalibrationRecord.CardSerialNumber))
                    {
                        foreach (var technician in Technicians)
                        {
                            if (technician != null && !string.IsNullOrEmpty(technician.Number))
                            {
                                if (string.Equals(technician.Number, e.CalibrationRecord.CardSerialNumber))
                                {
                                    Document.Technician = technician.Name;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(Document.TachographMake) && string.Equals(Document.TachographMake, Resources.TXT_SIEMENS_VDO))
                    {
                        if (!string.IsNullOrEmpty(e.CalibrationRecord.VuPartNumber) && e.CalibrationRecord.VuPartNumber.StartsWith(DataModel.Properties.Resources.TXT_SEED_TACHO_MODEL_NAME))
                        {
                            Document.TachographModel = DataModel.Properties.Resources.TXT_SEED_TACHO_MODEL_NAME;
                        }
                    }

                    PrintLabel(Document, false);

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

                SwitchReadButtonState(false);
                MainWindow.IsNavigationLocked = true;
                StatusText = Resources.TXT_WORKSHOP_CARD_FILE_GENERATED;
                ReadFromCardContent = Resources.TXT_READ_FROM_CARD;
            }
            else
            {
                StatusText = Resources.TXT_UNABLE_GENERATE_WORKSHOP_CARD;
                MessageBoxHelper.ShowMessage(Resources.ERR_UNABLE_READ_SMART_CARD);
                MainWindow.IsNavigationLocked = false;
                SwitchReadButtonState(true);
            }
        }

        protected override void OnProgress(object sender, DriverCardProgressEventArgs e)
        {
            StatusText = e.Message;
        }

        protected override void OnCardInserted(object sender, EventArgs e)
        {
            if (ReadFromCardCommand != null)
            {
                ReadFromCardCommand.Execute(new object());
            }
        }

        protected override void OnCardRemoved(object sender, EventArgs e)
        {
            CardBeingRead = false;
            StatusText = Resources.TXT_UNABLE_READ_SMART_CARD;
            SwitchReadButtonState(true);
            MainWindow.IsNavigationLocked = false;
        }

        protected override bool IncludeDeletedContacts
        {
            get { return IsHistoryMode; }
        }

        protected override void TachographMakeChanged(string make)
        {
            if (IsRegistrationChanging)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Document.DocumentType) && string.Equals(Document.DocumentType, Resources.TXT_DIGITAL_TWO_YEAR_CALIBRATION))
            {
                Document.NewBattery = !string.IsNullOrEmpty(make) && string.Equals(make.ToUpper(), Resources.TXT_SIEMENS_VDO);
            }
        }

        protected override void OnCustomerContactAdded(CustomerContact customerContact)
        {
            Document.CustomerContact = customerContact.Name;
        }

        private string GetRegistrationToQuery(CalibrationRecord calibrationRecord)
        {
            if (calibrationRecord == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(calibrationRecord.VehicleRegistrationNumber)) //Fall back to VIN
            {
                return calibrationRecord.VehicleIdentificationNumber;
            }

            return calibrationRecord.VehicleRegistrationNumber;
        }

        private void OnReadFromCard(object param)
        {
            if (!CardBeingRead)
            {
                CardBeingRead = true;

                try
                {
                    IsCardReadUserInitiated = param == null;
                    SwitchReadButtonState(false);
                    DriverCardReader.FastRead(param != null);
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

            PrintLabel(Document, true);
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

        private static void PrintLabel(TachographDocument document, bool userInitiated)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (string.IsNullOrEmpty(document.DocumentType))
            {
                if (userInitiated)
                {
                    MessageBoxHelper.ShowError(Resources.TXT_ERR_MUST_SELECT_DOCUMENT_TYPE);
                }
                return;
            }

            if (document.CalibrationTime == null)
            {
                document.CalibrationTime = DateTime.Now;
            }

            LabelHelper.Print(document);
        }
    }
}