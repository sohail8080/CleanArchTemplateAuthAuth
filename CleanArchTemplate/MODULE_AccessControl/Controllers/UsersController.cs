using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CleanArchTemplate.Common.UOW;
using CleanArchTemplate.AccessControl.Domain;
using Microsoft.AspNet.Identity.Owin;
using CleanArchTemplate.Common.BaseClasses;
using System.Data.Entity;

namespace CleanArchTemplate.AccessControl.Controllers
{
	[Authorize(Roles ="Admin")]
	public class UsersController : BaseController
    {

        // Two App. Service used for Account Management
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public UsersController()
        {
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        // Controller is not getting these properties by DI/IOC
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        // Controller is not getting these properties by DI/IOC
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        ////////////////Below Controller Methods//////////////////////

        // GET: Users       
        public ActionResult Index()
		{
            var users = new ApplicationDbContext().Users.Include(u => u.Roles).ToList();
            Set_Flag_For_Admin();
			return View("Index", users);
		}

        public ActionResult Details(string id)
        {            
            var user = new ApplicationDbContext().Users.Where(u => u.Id == id).FirstOrDefault();
            return View("Details", user);
        }


	}
}