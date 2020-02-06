using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.MODULE_AccessControl.ErrorHandling
{


    [AttributeUsage(
            AttributeTargets.Class | AttributeTargets.Method,
            Inherited = true,
            AllowMultiple = true)]
    public class HandleErrorAttribute : FilterAttribute, IExceptionFilter
    {
        // ...
        public void OnException(ExceptionContext filterContext)
        {
            throw new NotImplementedException();
        }
    }
}