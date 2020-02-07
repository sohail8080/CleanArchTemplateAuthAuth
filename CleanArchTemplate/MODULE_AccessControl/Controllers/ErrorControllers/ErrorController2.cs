using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{

    [HandleError]
    public class ErrorController2 : Controller
    {
        // GET: Error
        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Forbidden()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult NotImplemented()
        {
            return View();
        }

        public ActionResult ServerBusyOrDown()
        {
            return View();
        }

        public ActionResult ServerUnavailable()
        {
            return View();
        }

        public ActionResult Timeout()
        {
            return View();
        }

        public ActionResult Unauthorized()
        {
            return View();
        }
    }

}