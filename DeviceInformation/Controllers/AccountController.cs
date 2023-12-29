using DeviceInformation.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DeviceInformation.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Register()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
           if(ModelState.IsValid)
            {
                var userStore = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(userStore);
                var user = new IdentityUser() { UserName = model.UserName };

                IdentityResult result = manager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed");
                }
            }
            return View(model);
        }
        
        public ActionResult Login(string returnUrl)
        {
            var retUrl = string.IsNullOrEmpty(returnUrl) ? "/Home/Index" : returnUrl;
            ViewBag.ReturnUrl = retUrl;  
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var retUrl = string.IsNullOrEmpty(returnUrl) ? "/Home/Index": returnUrl;
            if (ModelState.IsValid)
            {
                var userStore = new UserStore<IdentityUser>();
                var userManager = new UserManager<IdentityUser>(userStore);
                var user = userManager.Find(model.UserName,model.Password);

                if (user != null)
                {
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;
                    var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, userIdentity);
                    return Redirect(retUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Login failed");
                    return View(model);
                }
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}