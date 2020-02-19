using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CleanArchTemplate
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // 3 line by Metthew Rinzee
            //routes.LowercaseUrls = true;
            //routes.MapMvcAttributeRoutes();
            //routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Account_default",
            //    url: "Account/{controller}/{action}/{id}",
            //    defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "CleanArchTemplate.AccessControl.Account.Presentation.Controllers" }
            //);


            //routes.MapRoute(
            //    name: "Manage_default",
            //    url: "Manage/{controller}/{action}/{id}",
            //    defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "CleanArchTemplate.AccessControl.Manage.Presentation.Controllers" }
            //);


            //routes.MapRoute(
            //    name: "Home_default",
            //    url: "Home/{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "CleanArchTemplate.Home.Presentation.Controllers" }
            //);

            // Following Routing Rules Plus the Routing Rules defined inside the
            // Arae Files both combine to result in Total Routing Rules.
            // I think following Rules come last in the stack.
            // defaults is the Controller Action executed after the successfull
            // login and there is not return url
            // defaults is now overrided inside the AccController in the Login()

            // Starup Page of the application is defined in the following routing rule.

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CleanArchTemplate.Home.Presentation.Controllers", "CleanArchTemplate.AccessControl.Account.Presentation.Controllers", "CleanArchTemplate.AccessControl.Manage.Presentation.Controllers" }
            );
        }
    }
}
