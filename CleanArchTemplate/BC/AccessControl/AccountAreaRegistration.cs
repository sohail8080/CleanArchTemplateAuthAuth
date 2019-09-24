//using System.Web.Mvc;

//namespace CleanArchTemplate.BC.AccessControl.Account
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
//                //new[] { "CleanArchTemplate.BC.AccessControl.Account.Presentation.Controllers" }
//                name: "Account_default",
//                url: "Account/{controller}/{action}/{id}",
//                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
//                namespaces: new[] { "CleanArchTemplate.BC.AccessControl.Account.Presentation.Controllers" }
//            );

//        }
//    }
//}