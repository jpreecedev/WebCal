using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TachoReaderLicensingWebsite.DataModel.Models;

namespace TachoReaderLicensingWebsite.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        //
        // GET: /Client/

        public ActionResult Details()
        {
            var model = new MainViewModel();

            var svcCust = new DataModel.Services.ClientService();

            model.Customers = svcCust.LoadClients(string.Empty);


            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Guid ID, string Name)
        {
            var result = Guid.Empty;

            if (!String.IsNullOrEmpty(Name))
            {
                var svcCust = new DataModel.Services.ClientService();
                if (ID == Guid.Empty)
                {
                    result = svcCust.AddClient(Name);
                }
                else
                {
                    result = svcCust.UpdateClientName(ID, Name);
                }
            }

            return Content(result.ToString());
        }

        [HttpPost]
        public ActionResult Delete(Guid ID)
        {
            if (ID != Guid.Empty)
            {
                var svcCust = new DataModel.Services.ClientService();
                svcCust.DeleteClient(ID);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }


            return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [OutputCache(Duration=0)]
        public ActionResult RefreshClientList(string SearchFilter)
        {
            var svcCust = new DataModel.Services.ClientService();
            var model = svcCust.LoadClients(SearchFilter);

            return PartialView("_ClientList", model);
        }

        [HttpGet]
        public ActionResult Load(Guid ClientAccessID)
        {
            var svcCust = new DataModel.Services.ClientService();
            var model = svcCust.LoadDetails(ClientAccessID);

            //return Content(new JavaScriptSerializer().Serialize(model));

            return Content(model.Name);
        }

    }
}
