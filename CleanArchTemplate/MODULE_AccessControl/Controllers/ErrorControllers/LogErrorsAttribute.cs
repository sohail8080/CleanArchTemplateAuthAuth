using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.MODULE_AccessControl.Controllers.ErrorControllers
{

    //[LogErrors(Order = 0)]
    //[HandleError(Order = 99)]
    //public class ContactController : Controller
    //{


    //        catch (Exception Ex)
    //{
    //mailobj.SendMail(
    //this.ControllerContext.RouteData.Values["controller"].ToString(),
    //this.ControllerContext.RouteData.Values["action"].ToString(),
    //Convert.ToString(Ex.Message.Replace("<", "").Replace(">", "").ToString()),
    //Convert.ToString(Ex.HelpLink),
    //Convert.ToString(Ex.Source),
    //Convert.ToString(Ex.StackTrace),
    //Convert.ToString(Ex.TargetSite));
    //return Json("error in the system.!", JsonRequestBehavior.AllowGet);
    //    }


    public class LogErrorsAttribute : FilterAttribute, IExceptionFilter
    {
        #region IExceptionFilter Members

        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                string controller = filterContext.RouteData.Values["controller"].ToString();
                string action = filterContext.RouteData.Values["action"].ToString();
                string loggerName = string.Format("{0}Controller.{1}", controller, action);

                //log4net.LogManager.GetLogger(loggerName).Error(string.Empty, filterContext.Exception);
            }

        }

        #endregion
    }
}