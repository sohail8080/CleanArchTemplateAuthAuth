using System.Web.Mvc;

namespace CleanArchTemplate.Areas.Home
{
    public class HomeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Home";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(

                name: "Home_default",
                url: "Home/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "CleanArchTemplate.BC.Home.Controllers" }

                //"Home_default",
                //"Home/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional },
                //new[] { "CleanArchTemplate.BC.Home.Controllers" }
            );
        }
    }
}