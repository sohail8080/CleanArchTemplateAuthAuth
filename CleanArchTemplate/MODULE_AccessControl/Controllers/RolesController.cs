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

namespace CleanArchTemplate.AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {

        // Two App. Service used for Account Management
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        //ApplicationDbContext context;

        public RolesController()
        {
            //context = new ApplicationDbContext();
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public RolesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

      
        public ActionResult Index()
        {
            //Following we get All Roles by Role Manager
            // Both Requires ApplicationDbContext
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
            var roles = roleManager.Roles.ToList();

            // Following we get All Roles by EF Repo
            // Both Requires ApplicationDbContext
            //var roles = context.Roles.ToList();
            return View("Index", roles);

        }

       
        public ActionResult Create()
        {            
            return View("Create");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<ActionResult> Create(CreateRoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                View("Create", viewModel);
            }

            // Create Role Method 1 by Calling RoleManager Service Asyn Method
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

            if (!roleManager.RoleExists(viewModel.Name))
            {
                await roleManager.CreateAsync(new IdentityRole(viewModel.Name)); // Create Role in DB
            }
            else
            {
                ModelState.AddModelError("", "Role already exist.");
                return View("Create", viewModel);
            }

            return RedirectToAction("Index", "Roles", new { area = "AccessControl" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(IdentityRole Role)
        {
            //context.Roles.Add(Role);
            //context.SaveChanges();
            return RedirectToAction("Index", "Roles", new { area = "AccessControl" });
        }

    }
}