using System.Collections.ObjectModel;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class VehiclesMakesModelsViewModel : BaseSettingsViewModel
    {
        private VehicleMake _selectedMake;
        private VehicleModel _selectedModel;

        #region Public Properties

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

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            AddMakeCommand = new DelegateCommand<UserControl>(OnAddMake);
            RemoveMakeCommand = new DelegateCommand<object>(OnRemoveMake, CanRemoveMake);
            AddModelCommand = new DelegateCommand<UserControl>(OnAddModel, CanAddModel);
            RemoveModelCommand = new DelegateCommand<object>(OnRemoveModel, CanRemoveModel);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<VehicleMake>>();
        }

        protected override void Load()
        {
            Makes = new ObservableCollection<VehicleMake>(Repository.GetAll().RemoveAt(0));
            Makes.CollectionChanged += (sender, e) => RefreshCommands();
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Commands

        #region Command : Add Make

        public DelegateCommand<UserControl> AddMakeCommand { get; set; }

        private void OnAddMake(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MAKE_OF_VEHICLE, OnAddMake);
        }

        private void OnAddMake(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                VehicleMake vehicleMake = new VehicleMake {Name = result};
                Makes.Add(vehicleMake);
                Repository.Add(vehicleMake);
            }
        }

        #endregion

        #region Command : Remove Make Command

        public DelegateCommand<object> RemoveMakeCommand { get; set; }

        private void OnRemoveMake(object obj)
        {
            Repository.Remove(SelectedMake);
            Makes.Remove(SelectedMake);
        }

        private bool CanRemoveMake(object obj)
        {
            return SelectedMake != null;
        }

        #endregion

        #region Command : Add Model Command

        public DelegateCommand<UserControl> AddModelCommand { get; set; }

        private void OnAddModel(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MODEL_OF_VEHICLE, OnAddModel);
        }

        private void OnAddModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
                SelectedMake.Models.Add(new VehicleModel {Name = result});
        }

        private bool CanAddModel(UserControl window)
        {
            return SelectedMake != null;
        }

        #endregion

        #region Command : Remove Model Command

        public DelegateCommand<object> RemoveModelCommand { get; set; }

        private void OnRemoveModel(object obj)
        {
            SelectedMake.Models.Remove(SelectedModel);
        }

        private bool CanRemoveModel(object obj)
        {
            return SelectedModel != null;
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddMakeCommand.RaiseCanExecuteChanged();
            RemoveMakeCommand.RaiseCanExecuteChanged();
            AddModelCommand.RaiseCanExecuteChanged();
            RemoveModelCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}