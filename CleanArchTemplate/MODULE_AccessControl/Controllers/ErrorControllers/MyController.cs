using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{
    public class MyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            if (id != 5)
            {
                ModelState.AddModelError("Error", "Specified row does not exist.");
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return View("Specified row exists.");
            }
        }
    }
}