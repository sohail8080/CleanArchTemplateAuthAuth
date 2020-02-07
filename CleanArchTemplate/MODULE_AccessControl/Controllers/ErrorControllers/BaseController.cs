using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers.ErrorControllers
{
    public class BaseController : Controller
    {
        public BaseController() { }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                if (filterContext.Controller.TempData.ContainsKey("Error"))
                {
                    var modelState = filterContext.Controller.TempData["Error"] as ModelState;
                    filterContext.Controller.ViewData.ModelState.Merge(new ModelStateDictionary() { new KeyValuePair<string, ModelState>("Error", modelState) });
                    filterContext.Controller.TempData.Remove("Error");
                }
            }
            if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
            {
                if (filterContext.Controller.ViewData.ModelState.ContainsKey("Error"))
                {
                    filterContext.Controller.TempData["Error"] = filterContext.Controller.ViewData.ModelState["Error"];
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }

}