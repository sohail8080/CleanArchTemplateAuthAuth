using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using System.Threading.Tasks;
using CleanArchTemplate.AccessControl.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using CleanArchTemplate.Common.BaseClasses;
using System.Data.Entity;

namespace CleanArchTemplate.AccessControl.Controllers
{

    [Authorize(Roles = RoleName.Admin)]
    public class UsersRolesController : BaseController
    {

        // Two App. Service used for Account Management
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext _context;

        public UsersRolesController()
        {
            _context = new ApplicationDbContext();
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public UsersRolesController(ApplicationUserManager userManager,
                                ApplicationSignInManager signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public ActionResult List(string userid)
        {

            var appUser = _context.Users.Include(u => u.Roles)
                .FirstOrDefault(u => u.Id == userid);

            var appRoles = _context.Roles.ToList();

            var list = UsersRolesViewModel.CreateUsersRolesList(appUser, appRoles);

            ViewBag.UserName = appUser.UserName;

            //RestoreViewBagMessage();

            Set_Flag_For_Admin();
            return View(@"~\MODULE_AccessControl\Views\Admin\Users\UsersRoles.cshtml", list);

        }


        public ActionResult AddUserRole(string userid, string rolename)
        {
            IdentityResult result;

            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(rolename))
                result = UserManager.AddToRole(userid, rolename);
            else
                return HttpNotFound("User or Role not found.");

            HandleAddResult(result);

            return List(userid);

            // Rediection cause loss of ViewBag Data having feed back
            //return RedirectToAction("List", "UsersRoles", routeValues: new { area = "AccessControl", userid = userid, rolename = rolename });

        }


        public ActionResult RemoveUserRole(string userid, string rolename)
        {
            IdentityResult result;

            if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(rolename))
                result = UserManager.RemoveFromRole(userid, rolename);
            else
                return HttpNotFound("User or Role not found.");

            HandleDeleteResult(result);

            return List(userid);

            // Rediection cause loss of ViewBag Data having feed back
            //return RedirectToAction("List", "UsersRoles", routeValues: new { area = "AccessControl", userid = userid, rolename = rolename });

        }












        // Controller is not getting these properties by DI/IOC
        private ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }

        // Controller is not getting these properties by DI/IOC
        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        // Controller is not getting these properties by DI/IOC
        private RoleManager<IdentityRole> RoleManager
        {
            get
            {
                return _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            }
            set
            {
                _roleManager = value;
            }
        }







    }
}