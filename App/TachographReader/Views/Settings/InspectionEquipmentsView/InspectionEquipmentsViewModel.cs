using System.Collections.ObjectModel;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class InspectionEquipmentsViewModel : BaseSettingsViewModel
    {
        private InspectionEquipment _selectedInspectionEquipment;

        #region Public Properties

        public IRepository<InspectionEquipment> Repository { get; set; }

        public ObservableCollection<InspectionEquipment> InspectionEquipments { get; set; }

        public InspectionEquipment SelectedInspectionEquipment
        {
            get { return _selectedInspectionEquipment; }
            set
            {
                _selectedInspectionEquipment = value;
                RefreshCommands();
            }
        }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            AddInspectionEquipmentCommand = new DelegateCommand<UserControl>(OnAddInspectionEquipment);
            RemoveInspectionEquipmentCommand = new DelegateCommand<object>(OnRemoveInspectionEquipment, CanRemoveInspectionEquipment);
        }

        protected override void Load()
        {
            InspectionEquipments = new ObservableCollection<InspectionEquipment>(Repository.GetAll());
            InspectionEquipments.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<InspectionEquipment>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Commands

        #region Command : Add Inspection Equipment

        public DelegateCommand<UserControl> AddInspectionEquipmentCommand { get; set; }

        private void OnAddInspectionEquipment(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_INSPECTION_EQUIPMENT, OnAddInspectionEquipment);
        }

        private void OnAddInspectionEquipment(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                InspectionEquipment inspectionEquipment = new InspectionEquipment { Name = result };
                InspectionEquipments.Add(inspectionEquipment);
                Repository.Add(inspectionEquipment);
            }
        }

        #endregion

        #region Command : Remove Inspection Equipment

        public DelegateCommand<object> RemoveInspectionEquipmentCommand { get; set; }

        private bool CanRemoveInspectionEquipment(object obj)
        {
            return SelectedInspectionEquipment != null;
        }

        private void OnRemoveInspectionEquipment(object obj)
        {
            Repository.Remove(SelectedInspectionEquipment);
            InspectionEquipments.Remove(SelectedInspectionEquipment);
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddInspectionEquipmentCommand.RaiseCanExecuteChanged();
            RemoveInspectionEquipmentCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}