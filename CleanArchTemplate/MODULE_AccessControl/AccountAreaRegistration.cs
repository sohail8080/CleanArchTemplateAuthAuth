//using System.Web.Mvc;

//namespace CleanArchTemplate.AccessControl.Account
//{
//    public class AccountAreaRegistration : AreaRegistration
//    {
//        public override string AreaName
//        {
//            get
//            {
//                return "Account";
//            }
//        }

//        public override void RegisterArea(AreaRegistrationContext context)
//        {
//            context.MapRoute(
//                //"Account_default",
//                //"Account/{controller}/{action}/{id}",
//                //new { action = "Index", id = UrlParameter.Optional },
//                //new[] { "CleanArchTemplate.AccessControl.Account.Presentation.Controllers" }
//                name: "Account_default",
//                url: "Account/{controller}/{action}/{id}",
//                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
//                namespaces: new[] { "CleanArchTemplate.AccessControl.Account.Presentation.Controllers" }
//            );

//        }
//    }
//}