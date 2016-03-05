namespace TachographReader.Views
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Windows.SignatureCaptureWindow;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Library;
    using Library.PDF;
    using Library.ViewModels;
    using Properties;
    using Shared;
    using Image = System.Drawing.Image;

    public class TechniciansViewModel : BaseNavigationViewModel
    {
        private Technician _selectedTechnician;
        private Image _signatureImage;

        public IRepository<Technician> Repository { get; set; }
        public ObservableCollection<Technician> Technicians { get; set; }

        public bool IsPromptVisible { get; set; }
        public UserPromptViewModel Prompt { get; set; }
        public Action<UserPromptViewModel> Callback { get; set; }

        public Technician SelectedTechnician
        {
            get { return _selectedTechnician; }
            set
            {
                _selectedTechnician = value;
                RefreshCommands();
            }
        }

        public Visibility LoggedInAsSuperUser
        {
            get { return UserManagement.LoggedInAsSuperUser ? Visibility.Visible : Visibility.Collapsed; }
        }

        public DelegateCommand<UserControl> AddTechnicianCommand { get; set; }
        public DelegateCommand<UserControl> EditTechnicianCommand { get; set; }
        public DelegateCommand<object> RemoveTechnicianCommand { get; set; }
        public DelegateCommand<object> SetDefaultCommand { get; set; }
        public DelegateCommand<object> AddSignatureCommand { get; set; }
        public DelegateCommand<object> GenerateStatusReportCommand { get; set; }

        protected override void Load()
        {
            ICollection<Technician> technicians = Repository.GetAll().Where(c => !string.IsNullOrEmpty(c.Name)).ToList();

            Technicians = new ObservableCollection<Technician>(technicians);
            Technicians.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseCommands()
        {
            AddTechnicianCommand = new DelegateCommand<UserControl>(OnAddTechnician);
            EditTechnicianCommand = new DelegateCommand<UserControl>(OnEditTechnician);
            RemoveTechnicianCommand = new DelegateCommand<object>(OnRemoveTechnician, CanRemoveTechnician);
            SetDefaultCommand = new DelegateCommand<object>(OnSetDefault, CanSetDefault);
            AddSignatureCommand = new DelegateCommand<object>(OnAddSignature);
            GenerateStatusReportCommand = new DelegateCommand<object>(OnGenerateStatusReport);
        }
        
        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<Technician>>();
        }

        private void OnAddTechnician(UserControl window)
        {
            var userPromptViewModel = new UserPromptViewModel
            {
                FirstPrompt = Resources.TXT_GIVE_NAME_OF_TECHNICIAN,
                SecondPrompt = Resources.TXT_ENTER_TECHNICIAN_NUMBER,
                DatePrompt = Resources.TXT_DATE_OF_LAST_CHECK,
                SecondDatePrompt = Resources.TXT_TECHNICIANS_DATE_OF_LAST_3_YEAR_CHECK,
                AddSignatureCommand = AddSignatureCommand
            };

            GetInputFromUser(window, userPromptViewModel, OnAddTechnician);
        }

        private void OnAddTechnician(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var technician = new Technician
                {
                    Name = result.FirstInput,
                    Number = result.SecondInput,
                    Image = _signatureImage,
                    DateOfLastCheck = result.DateInput,
                    DateOfLast3YearCheck = result.SecondDateInput
                };
                Technicians.Add(technician);
                Repository.Add(technician);
            }
        }

        private void OnEditTechnician(UserControl window)
        {
            if (SelectedTechnician == null)
            {
                return;
            }

            var userPromptViewModel = new UserPromptViewModel
            {
                FirstPrompt = Resources.TXT_GIVE_NAME_OF_TECHNICIAN,
                FirstInput = SelectedTechnician.Name,
                SecondPrompt = Resources.TXT_ENTER_TECHNICIAN_NUMBER,
                SecondInput = SelectedTechnician.Number,
                DatePrompt = Resources.TXT_DATE_OF_LAST_CHECK,
                DateInput = SelectedTechnician.DateOfLastCheck,
                SecondDatePrompt = Resources.TXT_TECHNICIANS_DATE_OF_LAST_3_YEAR_CHECK,
                SecondDateInput = SelectedTechnician.DateOfLast3YearCheck,
                AddSignatureCommand = AddSignatureCommand
            };

            GetInputFromUser(window, userPromptViewModel, OnEditTechnician);
        }

        private void OnEditTechnician(UserPromptViewModel result)
        {
            if (result == null || SelectedTechnician == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var selectedTechnician = SelectedTechnician.Clone<Technician>();
                selectedTechnician.Name = result.FirstInput;
                selectedTechnician.Number = result.SecondInput;
                selectedTechnician.DateOfLastCheck = result.DateInput;
                selectedTechnician.DateOfLast3YearCheck = result.SecondDateInput;

                var index = Technicians.IndexOf(SelectedTechnician);
                Technicians.Remove(SelectedTechnician);

                if (index > -1)
                {
                    Technicians.Insert(index, selectedTechnician);
                }
                else
                {
                    Technicians.Add(selectedTechnician);
                }
                Repository.AddOrUpdate(selectedTechnician);
                SelectedTechnician = null;
            }
        }

        private void OnAddSignature(object arg)
        {
            var window = new SignatureCaptureWindow();
            var dataContext = (SignatureCaptureWindowViewModel)window.DataContext;

            dataContext.OnSignatureCaptured = signature =>
            {
                _signatureImage = signature;
            };

            window.ShowDialog();
        }

        private bool CanRemoveTechnician(object obj)
        {
            return SelectedTechnician != null;
        }

        private void OnRemoveTechnician(object obj)
        {
            Repository.Remove(SelectedTechnician);
            Technicians.Remove(SelectedTechnician);
        }

        private bool CanSetDefault(object obj)
        {
            return SelectedTechnician != null;
        }

        private void OnSetDefault(object obj)
        {
            if (Technicians.IsNullOrEmpty() || SelectedTechnician == null)
            {
                return;
            }

            var currentDefault = Technicians.FirstOrDefault(c => c.IsDefault);
            if (currentDefault != null)
            {
                currentDefault.IsDefault = false;
                Repository.AddOrUpdate(currentDefault);
            }

            SelectedTechnician.IsDefault = true;
            Repository.AddOrUpdate(SelectedTechnician);
        }

        private void RefreshCommands()
        {
            AddTechnicianCommand.RaiseCanExecuteChanged();
            RemoveTechnicianCommand.RaiseCanExecuteChanged();
            SetDefaultCommand.RaiseCanExecuteChanged();
        }
        
        private void OnGenerateStatusReport(object obj)
        {
            var generalSettingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            var statusReport = new StatusReportViewModel
            {
                Technicians = Technicians,
                TachoCentreLastCheck = generalSettingsRepository.CentreQuarterlyCheckDate,
                GV212LastCheck = generalSettingsRepository.MonthlyGV212Date
            };

            var result = statusReport.GenerateStatusReport();
            if (result.Success)
            {
                if (AskQuestion(Resources.TXT_DO_YOU_WANT_TO_PRINT))
                {
                    result.Print();
                }
            }
        }
    }
}