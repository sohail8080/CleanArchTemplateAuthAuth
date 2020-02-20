using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.Common.Services
{
    public class LogExceptionToFileAttribute : HandleErrorAttribute
    {

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Exception ex = filterContext.Exception;

                Logger logger = new Logger();
                logger.LogError(ex);

                // Gets or sets a value that indicates whether the 
                // exception has been handled.
                //filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }


    }
}