using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate
{
    public class FilterConfig
    {
        // All Action Filter applied here will be applied to 
        // all Actions of All Contollers in the application.
        // Here we configure the Global Level CCCs related to Controllers.
        // By default, Asp.net mvc, attach/associate CCCs to the Controllers
        // So there is an automatic execution of CCCs related to Controllers
        // In Asp.net mvc, CCCs are related to Action Filters and Controllers
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // For all Controller Actions, Show the Error.cshtml when 
            // unhandled exception occurs
            filters.Add(new HandleErrorAttribute());

            // If Most of the Use Cases of Application are password protected
            // and some UCs are for Anonymous Users, then apply filter on Global 
            // Level, where we need to give access, we will give this explicity.
            // Check out the Account and Manage Contorllers for this settings.
            filters.Add(new AuthorizeAttribute());
        }
    }
}
