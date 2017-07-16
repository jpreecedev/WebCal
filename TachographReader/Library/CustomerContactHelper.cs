namespace TachographReader.Library
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using Connect.Shared.Models;
    using Controls;
    using TachographReader.DataModel.Core;
    using TachographReader.Shared;

    public static class CustomerContactHelper
    {
        public static void CreateCustomerContactIfRequired(Document document, Grid root)
        {
            if (document.CustomerContact != null) return;

            var customerContactRepository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();
            var customerContact = root.FindVisualChildren<InputComboField>().FirstOrDefault(c => c.Name == "CustomerContact");

            if (customerContact != null)
            {
                document.CustomerContact = customerContact.SelectedText;

                if (!customerContactRepository.Any(c => customerContact.SelectedText.Trim().ToUpper() == c.Name.Trim().ToUpper()))
                {
                    customerContactRepository.Add(new CustomerContact
                    {
                        Name = customerContact.SelectedText
                    });
                }
            }
        }
    }
}