namespace TachographReader.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library.ViewModels;
    using Properties;
    using Shared;

    public class InspectionMethodsViewModel : BaseSettingsViewModel
    {
        private InspectionMethod _selectedInspectionMethod;
        public ObservableCollection<InspectionMethod> InspectionMethods { get; set; }
        public IRepository<InspectionMethod> Repository { get; set; }

        public InspectionMethod SelectedInspectionMethod
        {
            get { return _selectedInspectionMethod; }
            set
            {
                _selectedInspectionMethod = value;
                RefreshCommands();
            }
        }

        public DelegateCommand<UserControl> AddInspectionMethodCommand { get; set; }
        public DelegateCommand<object> RemoveInspectionMethodCommand { get; set; }

        protected override void InitialiseCommands()
        {
            AddInspectionMethodCommand = new DelegateCommand<UserControl>(OnAddInspectionMethod);
            RemoveInspectionMethodCommand = new DelegateCommand<object>(OnRemoveInspectionMethod, CanRemoveInspectionMethod);
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<InspectionMethod>>();
        }

        protected override void Load()
        {
            InspectionMethods = new ObservableCollection<InspectionMethod>(Repository.GetAll().OrderBy(c => c.Name));
            InspectionMethods.CollectionChanged += (sender, e) => RefreshCommands();
        }

        private void OnAddInspectionMethod(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_INSPECTION_METHOD, OnAddInspectionMethod);
        }

        private void OnAddInspectionMethod(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var inspectionMethod = new InspectionMethod {Name = result.FirstInput};
                InspectionMethods.Add(inspectionMethod);
                Repository.Add(inspectionMethod);
            }
        }

        private bool CanRemoveInspectionMethod(object obj)
        {
            return SelectedInspectionMethod != null;
        }

        private void OnRemoveInspectionMethod(object obj)
        {
            Repository.Remove(SelectedInspectionMethod);
            InspectionMethods.Remove(SelectedInspectionMethod);
        }

        private void RefreshCommands()
        {
            AddInspectionMethodCommand.RaiseCanExecuteChanged();
            RemoveInspectionMethodCommand.RaiseCanExecuteChanged();
        }
    }
}