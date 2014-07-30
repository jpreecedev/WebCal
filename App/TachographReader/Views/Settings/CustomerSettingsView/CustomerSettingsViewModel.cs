using System.Collections.ObjectModel;
using System.ComponentModel;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Shared;

namespace Webcal.Views.Settings
{
    public class CustomerSettingsViewModel : BaseSettingsViewModel
    {
        private CustomerContact _selectedCustomerContact;

        #region Public Properties

        public IRepository<CustomerContact> Repository { get; set; }

        public ObservableCollection<CustomerContact> CustomerContacts { get; set; }

        public CustomerContact NewCustomerContact { get; set; }

        public bool IsEditing { get; set; }

        public CustomerContact SelectedCustomerContact
        {
            get { return _selectedCustomerContact; }
            set
            {
                _selectedCustomerContact = value;

                if (value != null)
                {
                    NewCustomerContact = value.Clone<CustomerContact>();
                    IsEditing = true;
                }

                InitialiseNewCustomerContact();
            }
        }

        public int SelectedIndex { get; set; }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            SaveCommand = new DelegateCommand<object>(OnSave, CanSave);
            CancelCommand = new DelegateCommand<object>(OnCancel);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<CustomerContact>>();
        }

        protected override void Load()
        {
            if (DoneCallback == null)
            {
                CustomerContacts = new ObservableCollection<CustomerContact>(Repository.GetAll());
            }
            else
            {
                CustomerContacts = new ObservableCollection<CustomerContact>();
            }

            InitialiseNewCustomerContact();
        }

        public override void Save()
        {
            Repository.Save();
        }

        public override void Dispose()
        {
            if (NewCustomerContact != null)
                NewCustomerContact.PropertyChanged -= NewCustomerContact_PropertyChanged;

            NewCustomerContact = null;
        }

        #endregion

        #region Commands

        #region Command : Remove

        public DelegateCommand<object> RemoveCommand { get; set; }

        private bool CanRemove(object obj)
        {
            return DoneCallback == null && SelectedCustomerContact != null;
        }

        private void OnRemove(object obj)
        {
            Repository.Remove(SelectedCustomerContact);
            CustomerContacts.Remove(SelectedCustomerContact);
        }

        #endregion

        #region Command : Save

        public DelegateCommand<object> SaveCommand { get; set; }

        private bool CanSave(object obj)
        {
            return NewCustomerContact != null && !string.IsNullOrEmpty(NewCustomerContact.Name) && NewCustomerContact.Name.Length > 1;
        }

        private void OnSave(object obj)
        {
            CustomerContact customerContact = NewCustomerContact.Clone<CustomerContact>();

            if (IsEditing)
            {
                CustomerContacts[SelectedIndex] = customerContact;
            }
            else
            {
                CustomerContacts.Add(customerContact);
            }

            Reset();
            Saved(customerContact);

            Repository.Add(customerContact);
            Repository.Save();
        }

        #endregion

        #region Command : Cancel

        public DelegateCommand<object> CancelCommand { get; set; }

        private void OnCancel(object obj)
        {
            Reset();
            Cancelled(null);
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitialiseNewCustomerContact()
        {
            if (NewCustomerContact != null)
                NewCustomerContact.PropertyChanged -= NewCustomerContact_PropertyChanged;

            NewCustomerContact = SelectedCustomerContact ?? new CustomerContact();
            NewCustomerContact.PropertyChanged += NewCustomerContact_PropertyChanged;

            SaveCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }

        private void NewCustomerContact_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void Reset()
        {
            SelectedCustomerContact = null;
            SelectedIndex = -1;
            IsEditing = false;
            InitialiseNewCustomerContact();
        }

        #endregion
    }
}