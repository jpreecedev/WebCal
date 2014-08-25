namespace Webcal.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using Properties;
    using Shared;
    using StructureMap;

    public class TyreSizesViewModel : BaseSettingsViewModel
    {
        private TyreSize _selectedTyreSize;
        
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

        public DelegateCommand<UserControl> AddTyreSizeCommand { get; set; }
        public DelegateCommand<object> RemoveTyreSizeCommand { get; set; }

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
        
        private void OnAddTyreSize(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_TYRE_SIZE, OnAddTyreSize);
        }

        private void OnAddTyreSize(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                var tyreSize = new TyreSize {Size = result};
                TyreSizes.Add(tyreSize);
                Repository.Add(tyreSize);
            }
        }

        private bool CanRemoveTyreSize(object obj)
        {
            return SelectedTyreSize != null;
        }

        private void OnRemoveTyreSize(object obj)
        {
            Repository.Remove(SelectedTyreSize);
            TyreSizes.Remove(SelectedTyreSize);
        }

        private void RefreshCommands()
        {
            AddTyreSizeCommand.RaiseCanExecuteChanged();
            RemoveTyreSizeCommand.RaiseCanExecuteChanged();
        }
    }
}