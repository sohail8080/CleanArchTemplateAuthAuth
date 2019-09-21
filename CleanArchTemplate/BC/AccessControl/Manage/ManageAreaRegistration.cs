using System.Web.Mvc;

namespace leanArchTemplate.BC.AccessControl.Manage
{
    public class ManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(

                name: "Manage_default",
                url: "Manage/{controller}/{action}/{id}",
                defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CleanArchTemplate.BC.AccessControl.Manage.Presentation.Controllers" }
                //"Manage_default",
                //"Manage/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional },
                //new[] { "CleanArchTemplate.BC.AccessControl.Manage.Presentation.Controllers" }

            );
        }
    }
}