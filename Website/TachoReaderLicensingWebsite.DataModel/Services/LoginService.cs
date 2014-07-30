using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TachoReaderLicensingWebsite.DataModel.Services
{
    public class LoginService
    {

        public bool AuthenticateClient(string username, string password) {
            if (username == "admin" && password == "password")
            {
                return true;
            }

            return false;
        }

    }
}
