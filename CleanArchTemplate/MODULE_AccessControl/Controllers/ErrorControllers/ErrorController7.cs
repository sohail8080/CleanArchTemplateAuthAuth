using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers
{
    public class ErrorController7 : Controller
    {
        public ActionResult HttpError403(string error)
        {
            ViewBag.Description = error;
            return this.View();
        }


    }
}