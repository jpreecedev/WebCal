using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class TechniciansViewModel : BaseSettingsViewModel
    {
        private Technician _selectedTechnician;

        #region Public Properties

        public IRepository<Technician> Repository { get; set; }

        public ObservableCollection<Technician> Technicians { get; set; }

        public Technician SelectedTechnician
        {
            get { return _selectedTechnician; }
            set
            {
                _selectedTechnician = value;
                RefreshCommands();
            }
        }

        #endregion

        #region Overrides

        protected override void Load()
        {
            ICollection<Technician> technicians = Repository.GetAll().RemoveAt(0);

            Technicians = new ObservableCollection<Technician>(technicians);
            Technicians.CollectionChanged += (sender, e) => RefreshCommands();
        }

        protected override void InitialiseCommands()
        {
            AddTechnicianCommand = new DelegateCommand<UserControl>(OnAddTechnician);
            RemoveTechnicianCommand = new DelegateCommand<object>(OnRemoveTechnician, CanRemoveTechnician);
            SetDefaultCommand = new DelegateCommand<object>(OnSetDefault, CanSetDefault);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<Technician>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Commands

        #region Command : Add Technician

        public DelegateCommand<UserControl> AddTechnicianCommand { get; set; }

        private void OnAddTechnician(UserControl window)
        {
            GetInputFromUser(window, Resources.TXT_GIVE_NAME_OF_TECHNICIAN, OnAddTechnician);
        }

        private void OnAddTechnician(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                Technician technician = new Technician {Name = result};
                Technicians.Add(technician);
                Repository.Add(technician);
            }
        }

        #endregion

        #region Command : Remove Technician

        public DelegateCommand<object> RemoveTechnicianCommand { get; set; }

        private bool CanRemoveTechnician(object obj)
        {
            return SelectedTechnician != null;
        }

        private void OnRemoveTechnician(object obj)
        {
            Repository.Remove(SelectedTechnician);
            Technicians.Remove(SelectedTechnician);
        }

        #endregion

        #region Command : Set Default

        public DelegateCommand<object> SetDefaultCommand { get; set; }

        private bool CanSetDefault(object obj)
        {
            return SelectedTechnician != null;
        }

        private void OnSetDefault(object obj)
        {
            if (Technicians.IsNullOrEmpty() || SelectedTechnician == null)
                return;

            List<Technician> copy = new List<Technician>(Technicians.ToArray());
            int selectedIndex = Technicians.IndexOf(SelectedTechnician);

            foreach (Technician technician in copy)
                technician.IsDefault = false;

            Technicians = new ObservableCollection<Technician>(copy);
            SelectedTechnician = Technicians[selectedIndex];
            SelectedTechnician.IsDefault = true;
        }

        #endregion

        #endregion

        #region Private Methods

        private void RefreshCommands()
        {
            AddTechnicianCommand.RaiseCanExecuteChanged();
            RemoveTechnicianCommand.RaiseCanExecuteChanged();
            SetDefaultCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}