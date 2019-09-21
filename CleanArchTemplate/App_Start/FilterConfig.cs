using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Show the Error.cshtml when unhandled exception occurs
            filters.Add(new HandleErrorAttribute());
        }
    }
}
