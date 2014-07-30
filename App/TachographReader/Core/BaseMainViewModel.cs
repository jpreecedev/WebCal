using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using StructureMap;
using Webcal.Controls;
using Webcal.DataModel;
using Webcal.EventArguments;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Views;
using Webcal.Views.Settings;
using Webcal.Properties;

namespace Webcal.Core
{
    public class BaseMainViewModel : BaseViewModel
    {
        #region Constructor

        public BaseMainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            CustomerContactRepository = ObjectFactory.GetInstance<IRepository<CustomerContact>>();
            CustomerContacts = new ObservableCollection<CustomerContact>(CustomerContactRepository.GetAll());
        }

        #endregion

        #region Public Properties

        public ObservableCollection<CustomerContact> CustomerContacts { get; set; }

        public CustomerContact SelectedCustomerContact { get; set; }

        public IRepository<CustomerContact> CustomerContactRepository { get; set; }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            NewCustomerCommand = new DelegateCommand<object>(OnNewCustomer);
            CancelCommand = new DelegateCommand<object>(OnCancel);
            BrowseCommand = new DelegateCommand<InputTextField>(OnBrowse);
        }

        protected override void BeforeLoad()
        {
            if (MainWindow != null)
                MainWindow.ModalClosedEvent += OnSmallModalClosed;
        }

        public override void Dispose()
        {
            if (MainWindow.ModalClosedEvent != null)
                MainWindow.ModalClosedEvent -= OnSmallModalClosed;
        }

        #endregion

        #region Protected Methods

        protected bool IsValid(DependencyObject root)
        {
            bool isValid = true;
            
            foreach (IValidate child in root.FindValidatableChildren().Where(child => !child.IsValid()))
            {
                isValid = false;
            }

            return isValid;
        }

        #endregion

        #region Commands

        #region Command : New Customer

        public DelegateCommand<object> NewCustomerCommand { get; set; }

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
                //SelectedCustomerContact = CustomerContacts.Last();
                var sorted = CustomerContacts.OrderBy(c => c.Name).ToArray();
                CustomerContacts.Clear();
                foreach (var item in sorted)
                {
                    CustomerContacts.Add(item);
                    if (item == (CustomerContact)e.Parameter)
                    {
                        SelectedCustomerContact = item;
                    }
                }
                //var sortedContacts = CollectionViewSource.GetDefaultView(CustomerContacts);
                //sortedContacts.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
                //CustomerContacts = sortedContacts;
                //SelectedCustomerContact = CustomerContacts.Last();
            }
        }

        #endregion

        #region Command : Cancel

        public DelegateCommand<object> CancelCommand { get; set; }

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

        #endregion

        #region Command : Browse

        public DelegateCommand<InputTextField> BrowseCommand { get; set; }

        private static void OnBrowse(InputTextField textField)
        {
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.All, "");
            if (result.Result == true)
            {
                textField.Text = result.FileName;
            }
        }

        #endregion

        #endregion
    }
}