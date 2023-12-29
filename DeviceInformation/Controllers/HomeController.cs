using DeviceInformation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DeviceInformation.Controllers
{
    public class HomeController : Controller
    {
       
        // GET: Home
        public ActionResult Index(int page=1)
        {
            return View();
        }
    }
}