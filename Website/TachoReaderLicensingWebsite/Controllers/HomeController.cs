using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TachoReaderLicensingWebsite.DataModel.Models;

namespace TachoReaderLicensingWebsite.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [AllowAnonymous]
        public ActionResult Login()
        {
            var model = new loginViewModel();
            return View(model);
        }

        //POST: /Home/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(loginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var svc = new DataModel.Services.LoginService();

                if (svc.AuthenticateClient(model.username, model.password))
                {
                    FormsAuthentication.SetAuthCookie(model.username, false);
                    return RedirectToAction("Details", "Client", null);    
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
                
            return View(model);
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
      
        
    }
}
