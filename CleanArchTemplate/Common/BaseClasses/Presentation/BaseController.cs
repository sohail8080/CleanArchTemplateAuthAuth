using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.Common.BaseClasses
{
    public class BaseController : Controller
    {

        IList<string> userRoles = null;


        public void Set_Flag_For_Admin()
        {
            bool isAdminUser = false;
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                var userRoles = userManager.GetRoles(User.Identity.GetUserId());
                if (userRoles == null | userRoles.Count <= 0)
                { isAdminUser = false; }

                //if (userRoles[0].ToString() == "Admin")
                if (userRoles.Contains("Admin"))
                { isAdminUser = true; }
                else
                { isAdminUser = false; }
            }

            // this ViewBag value will be accessible in View & Child Views
            ViewBag.isAdminUser = isAdminUser;

        }


        public void Set_Flag_For_Admin2()
        {
            // this ViewBag value will be accessible in View & Child Views
            ViewBag.isAdminUser = Is_User_In_Role2("Admin");
        }


        public IList<string> Get_User_Roles()
        {
            if (userRoles != null)
                return userRoles;

            if (User.Identity.GetUserId() == null)
                return null;

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userRoles = userManager.GetRoles(User.Identity.GetUserId());

            return userRoles;

            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();


        }


        public bool Is_User_In_Role(string role)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                return userManager.IsInRole(User.Identity.GetUserId(), role);
            }

            return false;

        }


        public bool Is_User_In_Role2(string role)
        {
            if (Get_User_Roles() == null || Get_User_Roles().Count <= 0)
                return false;

            return Get_User_Roles().Contains(role);

        }


        public void Set_Flag_For_Roles()
        {
            var userRoles = Get_User_Roles();

            foreach (var role in userRoles)
            {
                ViewData.Add(role, true);
            }
        }


        public bool Create_Role(string role)
        {
            // Following is the Startup code, this should be executed when the 
            // Application first starts and add Roles to the Application
            // This can be done in Configuration Method of Code First Migration
            // This can be done when Application DB is migrated and Ref data is added
            // Find place in MVC Framework, Code that exectues only once.

            var resultFlag = false;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
            if (!roleManager.RoleExists("Customer"))
            {
                var result = roleManager.Create(new IdentityRole("Customer")); // Create Role in DB                        
                if (result.Succeeded)
                    resultFlag = true;
                else
                    resultFlag = false;

            }

            return resultFlag;

        }

    }
}