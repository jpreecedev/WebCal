﻿namespace TachographReader.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Connect.Shared.Models;
    using Controls;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;
    using Views;
    using Views.Settings;

    public class BaseMainViewModel : BaseViewModel
    {
        private CustomerContact _selectedCustomerContact;

        public BaseMainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            CustomerContactRepository = GetInstance<IRepository<CustomerContact>>();
            CustomerContacts = new ObservableCollection<CustomerContact>(CustomerContactRepository.GetAll(true).OrderBy(c => c.Name));
        }

        public CustomerContact SelectedCustomerContact
        {
            get { return _selectedCustomerContact; }
            set
            {
                _selectedCustomerContact = value;
                OnCustomerContactChanged(value);
            }
        }

        public ObservableCollection<CustomerContact> CustomerContacts { get; set; }
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

        protected virtual void OnCustomerContactChanged(CustomerContact customerContact)
        {            
        }

        protected void CallAsync<T>(Func<T> beginCall, Action<T> endCall, Action<Exception> exceptionHandler, Action alwaysCall = null)
        {
            AsyncHelper.CallAsync(beginCall, endCall, exceptionHandler, alwaysCall);
        }

        private void OnNewCustomer(object obj)
        {
            MainWindow.ShowSmallModal<CustomerSettingsLightView>();
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
            
            MainWindow.ShowView<HomeScreenView>();
        }

        private static void OnBrowse(InputTextField textField)
        {
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.All, string.Empty);
            if (result.Result == true)
            {
                textField.Text = result.FileName;
            }
        }
    }
}