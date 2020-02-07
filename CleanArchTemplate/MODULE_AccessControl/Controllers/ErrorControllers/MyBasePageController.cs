using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{
    public class MyBasePageController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            //The "ErrorManager" in the above code is just a view that is using a
            //Model based on ExceptionContext

            filterContext.GetType();
            filterContext.ExceptionHandled = true;
            this.View("ErrorManager", filterContext).ExecuteResult(this.ControllerContext);
            base.OnException(filterContext);
        }
    }
}