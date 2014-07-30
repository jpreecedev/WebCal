using System.Collections.ObjectModel;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class InspectionMethodsViewModel : BaseSettingsViewModel
    {
        private InspectionMethod _selectedInspectionMethod;

        #region Public Properties

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

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            AddInspectionMethodCommand = new DelegateCommand<UserControl>(OnAddInspectionMethod);
            RemoveInspectionMethodCommand = new DelegateCommand<object>(OnRemoveInspectionMethod, CanRemoveInspectionMethod);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<InspectionMethod>>();
        }

        protected override void Load()
        {
            InspectionMethods = new ObservableCollection<InspectionMethod>(Repository.GetAll());
            InspectionMethods.CollectionChanged += (sender, e) => RefreshCommands();
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Commands

        #region Command : Add Inspection Method

        public DelegateCommand<UserControl> AddInspectionMethodCommand { get; set; }

        private void OnAddInspectionMethod(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_INSPECTION_METHOD, OnAddInspectionMethod);
        }

        private void OnAddInspectionMethod(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                InspectionMethod inspectionMethod = new InspectionMethod { Name = result };
                InspectionMethods.Add(inspectionMethod);
                Repository.Add(inspectionMethod);
            }
        }

        #endregion

        #region Command : Remove Inspection Method

        public DelegateCommand<object> RemoveInspectionMethodCommand { get; set; }

        private bool CanRemoveInspectionMethod(object obj)
        {
            return SelectedInspectionMethod != null;
        }

        private void OnRemoveInspectionMethod(object obj)
        {
            Repository.Remove(SelectedInspectionMethod);
            InspectionMethods.Remove(SelectedInspectionMethod);
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddInspectionMethodCommand.RaiseCanExecuteChanged();
            RemoveInspectionMethodCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}