using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{
    public class ErrorController6 : Controller
    {
        //
        // GET: /Error/
        //[HttpGet("/Error/NotFound")]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;

            return View();
        }

        //[HttpGet("/Error/ErrorPage")]
        public ActionResult ErrorPage()
        {
            Response.StatusCode = 500;

            return View();
        }
    }
}