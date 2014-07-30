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
    public class TachographMakesModelsViewModel : BaseSettingsViewModel
    {
        private TachographMake _selectedMake;
        private TachographModel _selectedModel;

        #region Public Properties

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

        #endregion

        #region Overrides

        protected override void Load()
        {
            Makes = new ObservableCollection<TachographMake>(Repository.GetAll().RemoveAt(0));
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
            Repository = ObjectFactory.GetInstance<IRepository<TachographMake>>();
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
            GetInputFromUser(window, Resources.TXT_GIVE_MAKE_OF_TACHOGRAPH, OnAddMake);
        }

        private void OnAddMake(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                TachographMake make = new TachographMake {Name = result};
                Makes.Add(make);
                Repository.Add(make);
            }
        }

        #endregion

        #region Command : Remove Make

        public DelegateCommand<object> RemoveMakeCommand { get; set; }

        private bool CanRemoveMake(object obj)
        {
            return SelectedMake != null;
        }

        private void OnRemoveMake(object obj)
        {
            if (SelectedMake == null)
                return;

            Repository.Remove(SelectedMake);
            Makes.Remove(SelectedMake);
        }

        #endregion

        #region Command : Add Model

        public DelegateCommand<UserControl> AddModelCommand { get; set; }

        private void OnAddModel(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_MODEL_OF_TACHOGRAPH, OnAddModel);
        }

        private void OnAddModel(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                TachographModel model = new TachographModel {Name = result};
                SelectedMake.Models.Add(model);
            }
        }

        private bool CanAddModel(UserControl obj)
        {
            return SelectedMake != null;
        }

        #endregion

        #region Command : Remove Model

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