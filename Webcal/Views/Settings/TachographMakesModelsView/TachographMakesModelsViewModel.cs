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

    public class TachographMakesModelsViewModel : BaseSettingsViewModel
    {
        private TachographMake _selectedMake;
        private TachographModel _selectedModel;
        public ObservableCollection<TachographMake> Makes { get; set; }

        public TachographMake SelectedMake
        {
            get { return _selectedMake; }
            set
            {
                _selectedMake = value;
                RefreshCommands();
            }
        }

        public TachographModel SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                RefreshCommands();
            }
        }

        public IRepository<TachographMake> Repository { get; set; }
        public DelegateCommand<UserControl> AddMakeCommand { get; set; }
        public DelegateCommand<object> RemoveMakeCommand { get; set; }
        public DelegateCommand<UserControl> AddModelCommand { get; set; }
        public DelegateCommand<object> RemoveModelCommand { get; set; }

        protected override void Load()
        {
            Makes = new ObservableCollection<TachographMake>(Repository.GetAll("Models").RemoveAt(0));
            Makes.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseCommands()
        {
            AddMakeCommand = new DelegateCommand<UserControl>(OnAddMake);
            RemoveMakeCommand = new DelegateCommand<object>(OnRemoveMake, CanRemoveMake);
            AddModelCommand = new DelegateCommand<UserControl>(OnAddModel, CanAddModel);
            RemoveModelCommand = new DelegateCommand<object>(OnRemoveModel, CanRemoveModel);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<TachographMake>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        private void OnAddMake(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MAKE_OF_TACHOGRAPH, OnAddMake);
        }

        private void OnAddMake(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var make = new TachographMake {Name = result.FirstInput};
                Makes.Add(make);
                Repository.Add(make);
            }
        }

        private bool CanRemoveMake(object obj)
        {
            return SelectedMake != null;
        }

        private void OnRemoveMake(object obj)
        {
            if (SelectedMake == null)
            {
                return;
            }

            Repository.Remove(SelectedMake);
            Makes.Remove(SelectedMake);
        }

        private void OnAddModel(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MODEL_OF_TACHOGRAPH, OnAddModel);
        }

        private void OnAddModel(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var model = new TachographModel {Name = result.FirstInput};
                SelectedMake.Models.Add(model);
            }
        }

        private bool CanAddModel(UserControl obj)
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