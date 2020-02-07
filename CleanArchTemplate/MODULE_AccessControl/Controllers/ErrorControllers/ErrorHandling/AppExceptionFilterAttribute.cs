using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers.ErrorHandling
{

    //public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    //{
    //    public override void OnException(ExceptionContext context)
    //    {
    //        //Notice pulling from HttpContext Application Svcs -- don't like that
    //        var loggerFactory = (ILoggerFactory)context.HttpContext.ApplicationServices.GetService(typeof(ILoggerFactory));

    //        var logger = loggerFactory.Create("MyWeb.Web.Api");
    //        logger.WriteError(2, "Error Occurred", context.Exception);

    //        context.Result = new JsonResult(
    //            new
    //            {
    //                context.Exception.Message,
    //                context.Exception.StackTrace
    //            });
    //    }
    //}

}