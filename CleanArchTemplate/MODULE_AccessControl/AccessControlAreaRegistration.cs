using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl
{
    public class AccessControlAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AccessControl";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                name: "AccessControl_admin",
                url: "AccessControl/Admin/{controller}/{action}/{id}",
                defaults: new { controller = "Users", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CleanArchTemplate.AccessControl.Controllers" }
            );


            context.MapRoute(
                //"AccessControl_default",
                //"AccessControl/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional },
                //new[] { "CleanArchTemplate.AccessControl.Controllers" }
                name: "AccessControl_default",
                url: "AccessControl/{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CleanArchTemplate.AccessControl.Controllers" }
            );

        }
    }



}