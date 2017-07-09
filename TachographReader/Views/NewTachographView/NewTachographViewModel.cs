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

    public class NewTachographViewModel : BaseNewDocumentViewModel<TachographDocument>
    {
        public NewTachographViewModel()
        {
            Document = new TachographDocument();

            VehicleTypes = VehicleType.GetVehicleTypes();
            Document.VehicleType = VehicleTypes.First();
        }

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
        public DelegateCommand<object> ReadFromCardCommand { get; set; }
        public DelegateCommand<Grid> PrintLabelCommand { get; set; }
        public DelegateCommand<object> AddInspectionInfoCommand { get; set; }
        public DelegateCommand<string> UpdateCardSerialNumber { get; set; }

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
            AddInspectionInfoCommand = new DelegateCommand<object>(OnAddInspectionInfo);
            UpdateCardSerialNumber = new DelegateCommand<string>(OnUpdateCardSerialNumber);
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

            if (string.IsNullOrEmpty(Document.DepotName))
            {
                Document.DepotName = RegistrationData.DepotName;
            }
        }

        protected override Document GetDocument()
        {
            return Document;
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
            if (Document.Created == default(DateTime))
            {
                Document.Created = DateTime.Now;
            }

            if (Document.CalibrationTime == null)
            {
                Document.CalibrationTime = DateTime.Now;
            }

            AddInspectionInfoCommand.Execute(null);

            TachographDocumentRepository.AddOrUpdate(Document);
            ConnectHelper.Upload(Document);
        }

        protected override void Update()
        {
            TachographDocumentRepository.AddOrUpdate(Document);
            ConnectHelper.Upload(Document, true);
        }

        protected override void RegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber) || !Document.IsDigital)
            {
                return;
            }

            //Remove all spaces from registration number
            Document.RegistrationNumber = registrationNumber.Replace(" ", string.Empty).ToUpper();

            if (!TachographDocumentRepository.Any())
            {
                return;
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
                return;
            }

            Document.CalibrationTime = DateTime.Now;
            Document.CardSerialNumber = match.CardSerialNumber;

            if (Document.Created == default(DateTime))
            {
                Document.Created = DateTime.Now;
            }
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

            SelectedCustomerContact = CustomerContacts.FirstOrDefault(c => string.Equals(c.Name, match.CustomerContact, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override void OnFastReadCompleted(object sender, DriverCardCompletedEventArgs e)
        {
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
                if (RegistrationChangedCommand != null)
                {
                    RegistrationChangedCommand.Execute(GetRegistrationToQuery(e.CalibrationRecord));
                }

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

                if (!string.IsNullOrEmpty(Document.TachographMake) && !string.IsNullOrEmpty(e.CalibrationRecord.VuPartNumber))
                {
                    if (string.Equals(Document.TachographMake, Resources.TXT_SIEMENS_VDO, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (e.CalibrationRecord.VuPartNumber.StartsWith(DataModel.Properties.Resources.TXT_SEED_TACHO_MODEL_NAME))
                        {
                            Document.TachographModel = e.CalibrationRecord.VuPartNumber;
                        }
                    }
                    if (string.Equals(Document.TachographMake, Resources.TXT_STONERIDGE, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (e.CalibrationRecord.VuPartNumber.StartsWith(Resources.TXT_STONERIDGE_CARD))
                        {
                            Document.TachographModel = e.CalibrationRecord.VuPartNumber;
                        }
                    }
                }

                PrintLabel(Document, false);

                StatusText = Resources.TXT_GENERATING_WORKSHOP_CARD_FILE;
                DriverCardReader.GenerateDump();
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
                StatusText = Resources.TXT_WORKSHOP_CARD_FILE_GENERATED;
                ReadFromCardContent = Resources.TXT_READ_FROM_CARD;
            }
            else
            {
                StatusText = Resources.TXT_UNABLE_GENERATE_WORKSHOP_CARD;
                MessageBoxHelper.ShowMessage(Resources.ERR_UNABLE_READ_SMART_CARD);
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

        protected override void OnCustomerContactChanged(CustomerContact customerContact)
        {
            Document.CustomerContact = customerContact == null ? null : customerContact.Name;
        }

        protected override void Close()
        {
            if (Document.IsQCCheck)
            {
                var viewModel = (QCCheckViewModel)MainWindow.ShowView<QCCheckView>();
                viewModel.PopulateFromCalibration(Document);
                return;
            }

            base.Close();
        }

        private static string GetRegistrationToQuery(CalibrationRecord calibrationRecord)
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

        private void OnAddInspectionInfo(object param)
        {
            if (string.IsNullOrEmpty(Document.NewInspectionInfo))
            {
                return;
            }

            if (string.IsNullOrEmpty(Document.InspectionInfo))
            {
                Document.InspectionInfo = Document.NewInspectionInfo;
            }
            else
            {
                Document.InspectionInfo = $"{Document.NewInspectionInfo}\n{Document.InspectionInfo}";
            }

            Document.NewInspectionInfo = string.Empty;
        }

        private void OnUpdateCardSerialNumber(string technicianName)
        {
            Document.CardSerialNumber = null;
            
            if (!Technicians.IsNullOrEmpty())
            {
                foreach (var technician in Technicians)
                {
                    if (technician != null && !string.IsNullOrEmpty(technician.Number))
                    {
                        if (string.Equals(technician.Name, technicianName))
                        {
                            Document.CardSerialNumber = technician.Number;
                        }
                    }
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
            TachographMakes = new ObservableCollection<TachographMake>(TachographMakesRepository.GetAll("Models"));
            Technicians = new ObservableCollection<Technician>(TechniciansRepository.GetAll());

            if (!IsHistoryMode)
            {
                var defaultTechnician = Technicians.FirstOrDefault(technician => technician != null && technician.IsDefault);
                if (defaultTechnician != null)
                {
                    Document.Technician = defaultTechnician.Name;
                }
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

                XDocument document = XDocument.Parse(xml.Replace("&", "&amp;"));
                XElement first = document.Descendants("CardDump").FirstOrDefault();

                var result = new List<string>();
                if (first != null)
                {
                    result.Add(first.Element("TempPath").SafelyGetValue().Replace("/", ""));
                    result.Add(first.Element("WorkshopName").SafelyGetValue());
                }

                if (!File.Exists(result[0]))
                {
                    var diagnosticMessage = $"Basic response:\n{xml}\n\nExpected path:\n{result[0]}\n\nFull path:\n{Path.GetFullPath(result[0])}\n\nWorkshop name:\n{result[1]}";
                    MessageBoxHelper.ShowError(string.Format(Resources.EXC_UNABLE_GENERATE_CARD_DUMP, diagnosticMessage));
                    return null;
                }

                return result.ToArray();
            }
            catch (Exception ex)
            {
                ShowError($"{Resources.TXT_UNABLE_READ_SMART_CARD}\n\n{ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)}");
            }

            return null;
        }

        private void PrintLabel(TachographDocument document, bool userInitiated)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (string.IsNullOrEmpty(document.DocumentType))
            {
                if (userInitiated)
                {
                    ShowError(Resources.TXT_ERR_MUST_SELECT_DOCUMENT_TYPE);
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