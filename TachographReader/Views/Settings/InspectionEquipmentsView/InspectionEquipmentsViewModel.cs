﻿namespace TachographReader.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using Library.ViewModels;
    using Properties;
    using Shared;

    public class InspectionEquipmentsViewModel : BaseSettingsViewModel
    {
        private InspectionEquipment _selectedInspectionEquipment;
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

        public DelegateCommand<UserControl> AddInspectionEquipmentCommand { get; set; }
        public DelegateCommand<object> RemoveInspectionEquipmentCommand { get; set; }

        protected override void InitialiseCommands()
        {
            AddInspectionEquipmentCommand = new DelegateCommand<UserControl>(OnAddInspectionEquipment);
            RemoveInspectionEquipmentCommand = new DelegateCommand<object>(OnRemoveInspectionEquipment, CanRemoveInspectionEquipment);
        }

        protected override void Load()
        {
            InspectionEquipments = new ObservableCollection<InspectionEquipment>(Repository.GetAll().OrderBy(c => c.Name));
            InspectionEquipments.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<InspectionEquipment>>();
        }

        private void OnAddInspectionEquipment(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_INSPECTION_EQUIPMENT, OnAddInspectionEquipment);
        }

        private void OnAddInspectionEquipment(UserPromptViewModel result)
        {
            if (result == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(result.FirstInput))
            {
                var inspectionEquipment = new InspectionEquipment {Name = result.FirstInput};
                InspectionEquipments.Add(inspectionEquipment);
                Repository.Add(inspectionEquipment);
            }
        }

        private bool CanRemoveInspectionEquipment(object obj)
        {
            return SelectedInspectionEquipment != null;
        }

        private void OnRemoveInspectionEquipment(object obj)
        {
            Repository.Remove(SelectedInspectionEquipment);
            InspectionEquipments.Remove(SelectedInspectionEquipment);
        }

        private void RefreshCommands()
        {
            AddInspectionEquipmentCommand.RaiseCanExecuteChanged();
            RemoveInspectionEquipmentCommand.RaiseCanExecuteChanged();
        }
    }
}