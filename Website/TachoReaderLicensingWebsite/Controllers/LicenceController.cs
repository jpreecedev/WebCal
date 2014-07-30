using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TachoReaderLicensingWebsite.Controllers
{
    public class LicenceController : Controller
    {
        //
        // GET: /Licence/

        [OutputCache(Duration = 0)]
        public ActionResult RefreshLicences(Guid id)
        {
            var svcCust = new DataModel.Services.LicenceService();
            var model = svcCust.LoadLicences(id);

            return PartialView("_LicenceList", model);
        }

        [HttpPost]
        public ActionResult Add(Guid ClientID, string ExpiryDate)
        {
            if (ClientID != Guid.Empty && !string.IsNullOrEmpty(ExpiryDate))
            {
                var svcCust = new DataModel.Services.LicenceService();
                svcCust.AddLicence(ClientID, ExpiryDate);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);

        }

        [HttpPost]
        public ActionResult Delete(Guid AccessID)
        {
            if (AccessID != Guid.Empty)
            {
                var svcCust = new DataModel.Services.LicenceService();
                svcCust.DeleteLicence(AccessID);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }


            return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);

        }
        


    }
}
