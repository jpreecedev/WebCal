namespace TachographReader.Views.Settings
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using Library;
    using Properties;
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
        public bool IsSearchingConnect { get; set; }
        public ObservableCollection<CustomerContact> AutoCompleteCustomerContacts { get; set; }

        public DelegateCommand<object> RemoveCommand { get; set; }
        public DelegateCommand<object> SaveCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }
        public DelegateCommand<CustomerContact> AutoCompleteSelectionChangedCommand { get; set; }
 
        protected override void InitialiseCommands()
        {
            RemoveCommand = new DelegateCommand<object>(OnRemove, CanRemove);
            SaveCommand = new DelegateCommand<object>(OnSave);
            CancelCommand = new DelegateCommand<object>(OnCancel);
            AutoCompleteSelectionChangedCommand = new DelegateCommand<CustomerContact>(OnAutoCompleteSelectionChanged);
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<CustomerContact>>();
        }

        protected override void Load()
        {
            LoadCustomerContacts();
            AutoCompleteCustomerContacts = new ObservableCollection<CustomerContact>();
            InitialiseNewCustomerContact();
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

        private void OnSave(object obj)
        {
            if (string.IsNullOrEmpty(NewCustomerContact.Name) || NewCustomerContact.Name.Length < 2)
            {
                ShowError(Resources.TXT_CUSTOMER_SETTINGS_ENTER_CUSTOMER_NAME);
                return;
            }

            var customerContact = NewCustomerContact.Clone<CustomerContact>();

            if (IsEditing && SelectedIndex > -1)
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

            CustomerContacts.Clear();
            CustomerContacts.AddRange(Repository.GetAll().OrderBy(c => c.Name));
        }

        private void OnAutoCompleteSelectionChanged(CustomerContact customerContact)
        {
            if (customerContact == null)
            {
                return;
            }

            SelectedCustomerContact = customerContact;
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

        private void LoadCustomerContacts()
        {
            if (DoneCallback == null)
            {
                CustomerContacts = new ObservableCollection<CustomerContact>(Repository.GetAll().OrderBy(c => c.Name));
            }
            else
            {
                CustomerContacts = new ObservableCollection<CustomerContact>();
            }
        }
    }
}