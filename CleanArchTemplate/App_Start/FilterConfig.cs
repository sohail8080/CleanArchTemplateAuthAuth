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
            // it is related to how we set customErrors in web.com
            // see web.config for details  <customErrors mode="On" />
            // This filter is applied to all Controllers & All Cont. Actions
            // Filler is exectued after the Cont. Action
            // Custom Error Pages for Internal Server Errors 500
            // Additionally the HandleErrorAttribute only handles 500 internal server errors.
            //It's error handling scope is limited to action methods within the MVC framework. 
            //    This means it won't be able to catch and process exceptions raised 
            //    from outside the ASP.NET MVC handler

            // You can use the attribute to decorate a controller class or a particular action method.It supports custom error pages per exception type out of the box:

            //[HandleError(ExceptionType = typeof(SqlException), View = "DatabaseError")]]
            //In order to get the HandleErrorAttribute working you also need to turn customErrors mode on in your web.config:

            //<system.web>
            //    <customErrors mode = "On" />
            //</ system.web >

            //You can use the attribute to decorate a controller class or a 
            //particular action method.
            //It supports custom error pages per exception type out of the box:
            //[HandleError(ExceptionType = typeof(SqlException), View = "DatabaseError")]]
        
            //The HandleErrorAttribute is the most limited in scope.
            //Many application errors will bypass this filter and therefore 
            //it is not ideal for >>global application error handling.

            //It is a great tool for >>action specific error handling like 
            //>>>>additional fault tolerance for a critical action method though.

            // if following filter is included, all 500 error will be
            // handled by the this. nothing in web.config will work
            // if following is commented only then Custom Errors will 
            // work in the web.config
           //filters.Add(new HandleErrorAttribute());


            // To handle exceptions
            //filters.Add(new CustomExceptionFilter());///////////

            // If Most of the Use Cases of Application are password protected
            // and some UCs are for Anonymous Users, then apply filter on Global 
            // Level, where we need to give access, we will give this explicity.
            // Check out the Account and Manage Contorllers for this settings.
            filters.Add(new AuthorizeAttribute());


            //     Represents an attribute that forces an unsecured HTTP request to be re-sent over
            //     HTTPS.
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
