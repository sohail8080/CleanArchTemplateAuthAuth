using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.ErrorHandlingggg
{
    public class CustomExceptionFilter : HandleErrorAttribute
    {

        public override void OnException(ExceptionContext filterContext)
        {

            //_logger.Error("Uncaught exception", filterContext.Exception);

            ViewResult view = new ViewResult();
            view.ViewName = "Error";
            filterContext.Result = view;

            // Prepare the response code.
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            //filterContext.HttpContext.Items["Exception"] = Exception.Message;

        }
    }
}