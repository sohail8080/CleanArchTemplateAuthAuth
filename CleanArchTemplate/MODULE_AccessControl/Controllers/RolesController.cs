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
    public class RolesController : BaseController
    {

        // Two App. Service used for Account Management
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext _context;

        public RolesController()
        {
            _context = new ApplicationDbContext();
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public RolesController(ApplicationUserManager userManager, 
                                ApplicationSignInManager signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }





        ////////////////Below Controller Methods//////////////////////
      
        public ActionResult List()
        {
            //Following we get All Roles by Role Manager
            // Both Requires ApplicationDbContext
            //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

            var roles = RoleManager.Roles.ToList();
            Set_Flag_For_Admin();
            return View("List", roles);

            // Following we get All Roles by EF Repo
            // Both Requires ApplicationDbContext
            //var roles = context.Roles.ToList();
        }

        public ActionResult Details(string id)
        {
            //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

            var role = RoleManager.FindById(id);
            return View("Details", role);
        }


        public ActionResult Delete(string id)
        {
            //var role = RoleManager.FindById(id);
            //var result = RoleManager.Delete(role);

            //var role = _context.Roles.Where(r => r.Name == "my role name").FirstOrDefault();
            //var rold = _context.Roles.Select(r => r.Id == id);
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            _context.Roles.Remove(role);
            HandleDeleteResult(_context.SaveChanges());

            return List();
            //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            
        }

        public ActionResult Create()
        {
            var viewModel = new RoleFormViewModel();
            Set_Flag_For_Admin();
            return View("RoleForm", viewModel);
        }

        public ActionResult Edit(string id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);

            if (role == null)
                return HttpNotFound();

            var viewModel = new RoleFormViewModel(role);
            Set_Flag_For_Admin();
            return View("RoleForm", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<ActionResult> Save(RoleFormViewModel viewModel)
        {
            IdentityResult result;

            if (!ModelState.IsValid)
            {
                return View("RoleForm", viewModel);
            }

            if(string.IsNullOrEmpty(viewModel.Id))
            {
                // Create Role Method 1 by Calling RoleManager Service Asyn Method
                //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
                //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

                if (!RoleManager.RoleExists(viewModel.Name))
                {
                    HandleAddResult(await RoleManager.CreateAsync(new IdentityRole(viewModel.Name))); // Create Role in DB
                }
                else
                {
                    ModelState.AddModelError("", "Role already exist.");
                    return View("RoleForm", viewModel);
                }

                return List();
                //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            }
            else
            {
                var roleInDB = _context.Roles.FirstOrDefault(r => r.Id == viewModel.Id);

                roleInDB.Name = viewModel.Name;
                HandleUpdateResult(_context.SaveChanges());

                return List();
                //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(IdentityRole Role)
        {
            _context.Roles.Add(Role);
            _context.SaveChanges();
            return RedirectToAction("List", "Roles", routeValues:new { area = "AccessControl" });
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


        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(string roleName)
        {
            return from role in _context.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in _context.Users
                   on userRoles.UserId equals user.Id
                   where user.EmailConfirmed == true
                   select user;
        }


        public IEnumerable<ApplicationUser> GetApplicationUsersInRole2(string roleName)
        {
            var users = _context.Users.Include(u => u.Roles).ToList();
            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);

            var list = from user in users
                   where user.Roles.Any(r => r.RoleId == role.Id)
                   select user;

            return list;
        }







    }
}