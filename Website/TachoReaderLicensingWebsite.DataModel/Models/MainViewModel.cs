using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TachoReaderLicensingWebsite.DataModel.Models
{
    public class MainViewModel : Client
    {
        public IEnumerable<Client> Customers { get; set; }

        public MainViewModel()
        {
            Customers = new List<Client>();
        }


    }
}
