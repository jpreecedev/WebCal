namespace Webcal.Views.Settings
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Library.ViewModels;
    using Properties;
    using Shared;

    public class TechniciansViewModel : BaseSettingsViewModel
    {
        private Technician _selectedTechnician;
        public IRepository<Technician> Repository { get; set; }
        public ObservableCollection<Technician> Technicians { get; set; }

        public Technician SelectedTechnician
        {
            get { return _selectedTechnician; }
            set
            {
                _selectedTechnician = value;
                RefreshCommands();
            }
        }

        public DelegateCommand<UserControl> AddTechnicianCommand { get; set; }
        public DelegateCommand<object> RemoveTechnicianCommand { get; set; }
        public DelegateCommand<object> SetDefaultCommand { get; set; }

        protected override void Load()
        {
            ICollection<Technician> technicians = Repository.GetAll().RemoveAt(0);

            Technicians = new ObservableCollection<Technician>(technicians);
            Technicians.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseCommands()
        {
            AddTechnicianCommand = new DelegateCommand<UserControl>(OnAddTechnician);
            RemoveTechnicianCommand = new DelegateCommand<object>(OnRemoveTechnician, CanRemoveTechnician);
            SetDefaultCommand = new DelegateCommand<object>(OnSetDefault, CanSetDefault);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<Technician>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        private void OnAddTechnician(UserControl window)
        {
            var userPromptViewModel = new UserPromptViewModel()
            {
                FirstPrompt = Resources.TXT_GIVE_NAME_OF_TECHNICIAN,
                SecondPrompt = Resources.TXT_ENTER_TECHNICIAN_NUMBER
            };

            GetInputFromUser(window, userPromptViewModel , OnAddTechnician);
        }

        private void OnAddTechnician(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var technician = new Technician {Name = result.FirstInput, Number = result.SecondInput};
                Technicians.Add(technician);
                Repository.Add(technician);
            }
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

            var copy = new List<Technician>(Technicians.ToArray());
            int selectedIndex = Technicians.IndexOf(SelectedTechnician);

            foreach (Technician technician in copy)
            {
                technician.IsDefault = false;
            }

            Technicians = new ObservableCollection<Technician>(copy);
            SelectedTechnician = Technicians[selectedIndex];
            SelectedTechnician.IsDefault = true;
        }

        private void RefreshCommands()
        {
            AddTechnicianCommand.RaiseCanExecuteChanged();
            RemoveTechnicianCommand.RaiseCanExecuteChanged();
            SetDefaultCommand.RaiseCanExecuteChanged();
        }
    }
}