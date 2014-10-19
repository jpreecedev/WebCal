namespace Webcal.Core
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Controls;
    using DataModel;
    using DataModel.Core;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;
    using Views;
    using Views.Settings;

    public class BaseMainViewModel : BaseViewModel
    {
        public BaseMainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            CustomerContactRepository = ContainerBootstrapper.Container.GetInstance<IRepository<CustomerContact>>();
            CustomerContacts = new ObservableCollection<CustomerContact>(CustomerContactRepository.GetAll(true).OrderBy(c => c.Name));
        }

        public ObservableCollection<CustomerContact> CustomerContacts { get; set; }
        public CustomerContact SelectedCustomerContact { get; set; }
        public IRepository<CustomerContact> CustomerContactRepository { get; set; }
        public DelegateCommand<object> NewCustomerCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }
        public DelegateCommand<InputTextField> BrowseCommand { get; set; }

        protected override void InitialiseCommands()
        {
            NewCustomerCommand = new DelegateCommand<object>(OnNewCustomer);
            CancelCommand = new DelegateCommand<object>(OnCancel);
            BrowseCommand = new DelegateCommand<InputTextField>(OnBrowse);
        }

        protected override void BeforeLoad()
        {
            if (MainWindow != null)
            {
                MainWindow.ModalClosedEvent += OnSmallModalClosed;
            }

            if (!IncludeDeletedContacts)
            {
                CustomerContacts.Remove(c => c.IsDeleted);
            }
        }

        public override void Dispose()
        {
            if (MainWindow.ModalClosedEvent != null)
            {
                MainWindow.ModalClosedEvent -= OnSmallModalClosed;
            }
        }

        protected virtual bool IncludeDeletedContacts
        {
            get { return false; }
        }

        private void OnNewCustomer(object obj)
        {
            MainWindow.ShowSmallModal<CustomerSettingsView>();
        }

        private void OnSmallModalClosed(object sender, ModalClosedEventArgs e)
        {
            OnNewCustomerSelected(e);
        }

        private void OnNewCustomerSelected(ModalClosedEventArgs e)
        {
            if (e.Saved)
            {
                CustomerContacts.Add((CustomerContact)e.Parameter);
                CustomerContact[] sorted = CustomerContacts.OrderBy(c => c.Name).ToArray();
                CustomerContacts.Clear();
                foreach (CustomerContact item in sorted)
                {
                    CustomerContacts.Add(item);
                    if (item == e.Parameter)
                    {
                        SelectedCustomerContact = item;
                    }
                }
            }
        }

        private void OnCancel(object obj)
        {
            if (HasChanged)
            {
                if (AskQuestion(Resources.TXT_UNSAVED_CHANGES_WILL_BE_LOST))
                {
                    MainWindow.ShowView<HomeScreenView>();
                    return;
                }
            }

            MainWindow.IsNavigationLocked = false;
            MainWindow.ShowView<HomeScreenView>();
        }

        private static void OnBrowse(InputTextField textField)
        {
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.All, "");
            if (result.Result == true)
            {
                textField.Text = result.FileName;
            }
        }
    }
}