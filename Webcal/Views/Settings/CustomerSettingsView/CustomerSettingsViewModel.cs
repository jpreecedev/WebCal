namespace Webcal.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Shared;

    public class CustomerSettingsViewModel : BaseSettingsViewModel
    {
        private CustomerContact _selectedCustomerContact;
        public IRepository<CustomerContact> Repository { get; set; }
        public ObservableCollection<CustomerContact> CustomerContacts { get; set; }
        public CustomerContact NewCustomerContact { get; set; }
        public bool IsEditing { get; set; }

        public CustomerSettingsViewModel()
        {
            SelectedIndex = -1;
        }

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
        public DelegateCommand<object> RemoveCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }

        protected override void InitialiseCommands()
        {
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            SaveCommand = new DelegateCommand<object>(OnSave, CanSave);
            CancelCommand = new DelegateCommand<object>(OnCancel);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<CustomerContact>>();
        }

        protected override void Load()
        {
            if (DoneCallback == null)
            {
                CustomerContacts = new ObservableCollection<CustomerContact>(Repository.GetAll().OrderBy(c => c.Name));
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
            {
                NewCustomerContact.PropertyChanged -= NewCustomerContact_PropertyChanged;
            }

            NewCustomerContact = null;
        }

        private bool CanRemove(object obj)
        {
            return DoneCallback == null && SelectedCustomerContact != null;
        }

        private void OnRemove(object obj)
        {
            Repository.Remove(SelectedCustomerContact);
            CustomerContacts.Remove(SelectedCustomerContact);
        }

        private bool CanSave(object obj)
        {
            return NewCustomerContact != null && !string.IsNullOrEmpty(NewCustomerContact.Name) && NewCustomerContact.Name.Length > 1;
        }

        private void OnSave(object obj)
        {
            var customerContact = NewCustomerContact.Clone<CustomerContact>();

            if (IsEditing)
            {
                CustomerContacts[SelectedIndex] = customerContact;
            }
            else
            {
                CustomerContacts.Add(customerContact);
            }

            Reset();
            Repository.AddOrUpdate(customerContact);
            Saved(customerContact);

            Repository.Save();
        }

        private void OnCancel(object obj)
        {
            Reset();
            Cancelled(null);
        }

        private void InitialiseNewCustomerContact()
        {
            if (NewCustomerContact != null)
            {
                NewCustomerContact.PropertyChanged -= NewCustomerContact_PropertyChanged;
            }

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
    }
}