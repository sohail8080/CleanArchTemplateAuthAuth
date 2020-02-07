using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers.ErrorHandling
{
    public class HandleErrorsAttribute : HandleErrorAttribute
    {

        //private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                //log.Error("Error in Controller", filterContext.Exception);
            }

            base.OnException(filterContext);
        }


        //[HandleError(ExceptionType = typeof(System.Data.DataException), View = "DatabaseError")]
        //public ActionResult Index(int id)
        //{
        //    var db = new MyDataContext();
        //    return View("Index", db.Categories.Single(x => x.Id == id));
        //}


        //[HandleError(ExceptionType = typeof(System.Data.DataException), View = "DatabaseError")]
        //public class HomeController : Controller
        //{
        //    /* Controller Actions with HandleError applied to them */
        //}


        //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //{
        //    filters.Add(new HandleErrorAttribute
        //    {
        //        ExceptionType = typeof(System.Data.DataException),
        //        View = "DatabaseError"
        //    });

        //    filters.Add(new HandleErrorAttribute()); //by default added
        //}


    }
}