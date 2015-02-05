namespace Webcal.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Library.ViewModels;
    using Properties;
    using Shared;

    public class VehiclesMakesModelsViewModel : BaseSettingsViewModel
    {
        private VehicleMake _selectedMake;
        private VehicleModel _selectedModel;
        public ObservableCollection<VehicleMake> Makes { get; set; }
        public IRepository<VehicleMake> Repository { get; set; }

        public VehicleMake SelectedMake
        {
            get { return _selectedMake; }
            set
            {
                _selectedMake = value;
                RefreshCommands();
            }
        }

        public VehicleModel SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RefreshCommands();
            }
        }

        public DelegateCommand<UserControl> AddMakeCommand { get; set; }
        public DelegateCommand<object> RemoveMakeCommand { get; set; }
        public DelegateCommand<UserControl> AddModelCommand { get; set; }
        public DelegateCommand<object> RemoveModelCommand { get; set; }

        protected override void InitialiseCommands()
        {
            AddMakeCommand = new DelegateCommand<UserControl>(OnAddMake);
            RemoveMakeCommand = new DelegateCommand<object>(OnRemoveMake, CanRemoveMake);
            AddModelCommand = new DelegateCommand<UserControl>(OnAddModel, CanAddModel);
            RemoveModelCommand = new DelegateCommand<object>(OnRemoveModel, CanRemoveModel);
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<VehicleMake>>();
        }

        protected override void Load()
        {
            Makes = new ObservableCollection<VehicleMake>(Repository.GetAll("Models").RemoveAt(0));
            Makes.CollectionChanged += (sender, e) => RefreshCommands();
        }

        public override void Save()
        {
            Repository.Save();
        }

        private void OnAddMake(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MAKE_OF_VEHICLE, OnAddMake);
        }

        private void OnAddMake(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var vehicleMake = new VehicleMake {Name = result.FirstInput};
                Makes.Add(vehicleMake);
                Repository.Add(vehicleMake);
            }
        }

        private void OnRemoveMake(object obj)
        {
            Repository.Remove(SelectedMake);
            Makes.Remove(SelectedMake);
        }

        private bool CanRemoveMake(object obj)
        {
            return SelectedMake != null;
        }

        private void OnAddModel(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MODEL_OF_VEHICLE, OnAddModel);
        }

        private void OnAddModel(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                SelectedMake.Models.Add(new VehicleModel {Name = result.FirstInput});
            }
        }

        private bool CanAddModel(UserControl window)
        {
            return SelectedMake != null;
        }

        private void OnRemoveModel(object obj)
        {
            SelectedMake.Models.Remove(SelectedModel);
        }

        private bool CanRemoveModel(object obj)
        {
            return SelectedModel != null;
        }

        private void RefreshCommands()
        {
            AddMakeCommand.RaiseCanExecuteChanged();
            RemoveMakeCommand.RaiseCanExecuteChanged();
            AddModelCommand.RaiseCanExecuteChanged();
            RemoveModelCommand.RaiseCanExecuteChanged();
        }
    }
}