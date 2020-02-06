using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CleanArchTemplate.MODULE_AccessControl.ErrorHandling
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        {
            if (context.HttpContext.Request.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "NoPermissions",
                    controller = "Error",
                    area = ""
                }));
            }
            else
            {
                base.HandleUnauthorizedRequest(context);
            }
        }
    }
}