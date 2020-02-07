using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{
    public sealed class ErrorsController3 : Controller
    {
        public ActionResult NotFound()
        {
            ActionResult result;

            object model = Request.Url.PathAndQuery;

            if (!Request.IsAjaxRequest())
                result = View(model);
            else
                result = PartialView("_NotFound", model);

            return result;
        }


        public void Fire404Error(HttpException exception)
        {
            //you can place any other error handling code here
            //throw new PageNotFoundException("page or resource");
        }

    }
}