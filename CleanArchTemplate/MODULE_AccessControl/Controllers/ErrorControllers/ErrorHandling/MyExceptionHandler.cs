using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers.ErrorHandling
{
    public class MyExceptionHandler : ActionFilterAttribute, IExceptionFilter

    {

        public void OnException(ExceptionContext filterContext)

        {

            Exception e = filterContext.Exception;

            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult()

            {

                ViewName = "SomeException"

            };

        }
    }
}