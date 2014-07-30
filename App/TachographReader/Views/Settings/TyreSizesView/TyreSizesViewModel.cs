using System.Collections.ObjectModel;
using System.Windows.Controls;
using StructureMap;
using Webcal.Shared;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class TyreSizesViewModel : BaseSettingsViewModel
    {
        private TyreSize _selectedTyreSize;

        #region Public Properties

        public ObservableCollection<TyreSize> TyreSizes { get; set; }

        public IRepository<TyreSize> Repository { get; set; }

        public TyreSize SelectedTyreSize
        {
            get { return _selectedTyreSize; }
            set
            {
                _selectedTyreSize = value;
                RefreshCommands();
            }
        }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            AddTyreSizeCommand = new DelegateCommand<UserControl>(OnAddTyreSize);
            RemoveTyreSizeCommand = new DelegateCommand<object>(OnRemoveTyreSize, CanRemoveTyreSize);
        }

        protected override void Load()
        {
            TyreSizes = new ObservableCollection<TyreSize>(Repository.GetAll());
            TyreSizes.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<TyreSize>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Commands

        #region Command : Add Tyre Size

        public DelegateCommand<UserControl> AddTyreSizeCommand { get; set; }

        private void OnAddTyreSize(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_TYRE_SIZE, OnAddTyreSize);
        }

        private void OnAddTyreSize(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                TyreSize tyreSize = new TyreSize {Size = result};
                TyreSizes.Add(tyreSize);
                Repository.Add(tyreSize);
            }
        }

        #endregion

        #region Command : Remove Tyre Size

        public DelegateCommand<object> RemoveTyreSizeCommand { get; set; }

        private bool CanRemoveTyreSize(object obj)
        {
            return SelectedTyreSize != null;
        }

        private void OnRemoveTyreSize(object obj)
        {
            Repository.Remove(SelectedTyreSize);
            TyreSizes.Remove(SelectedTyreSize);
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddTyreSizeCommand.RaiseCanExecuteChanged();
            RemoveTyreSizeCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}