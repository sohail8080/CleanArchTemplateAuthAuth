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
    // When following attribute applied at global level, there is not 
    // need to do UI based Athentication. But done for demo purpose
    [Authorize(Roles = RoleName.Admin)]
    public class RolesController : BaseController
    {

        public RolesController()
        {
           
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public RolesController(ApplicationUserManager userManager, 
                                ApplicationSignInManager signInManager,
                                ApplicationRoleManager roleManager)
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


        public async Task<ActionResult> Delete(string id)
        {
            //var role = RoleManager.FindById(id);
            //var result = RoleManager.Delete(role);

            //var role = _context.Roles.Where(r => r.Name == "my role name").FirstOrDefault();
            //var rold = _context.Roles.Select(r => r.Id == id);

            //var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            //_context.Roles.Remove(role);
            //HandleDeleteResult(_context.SaveChanges());

            var role = await RoleManager.FindByIdAsync(id);
            await RoleManager.DeleteAsync(role);
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
            var role = RoleManager.Roles.FirstOrDefault(r => r.Id == id);

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
            //IdentityResult result;

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
                //var roleInDB = _context.Roles.FirstOrDefault(r => r.Id == viewModel.Id);
                var roleInDB = await RoleManager.FindByIdAsync(viewModel.Id);

                roleInDB.Name = viewModel.Name;
                IdentityResult result = await RoleManager.UpdateAsync(roleInDB);

                //HandleUpdateResult(_context.SaveChanges());
                HandleUpdateResult(result);
                return List();
                //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(IdentityRole role)
        {
            //_context.Roles.Add(Role);
            RoleManager.CreateAsync(role);
            //_context.SaveChanges();
            return RedirectToAction("List", "Roles", routeValues:new { area = "AccessControl" });
        }


        public IEnumerable<ApplicationUser> GetApplicationUsersInRole(string roleName)
        {
            return from role in RoleManager.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in UserManager.Users
                   on userRoles.UserId equals user.Id
                   where user.EmailConfirmed == true
                   select user;
        }


        public IEnumerable<ApplicationUser> GetApplicationUsersInRole2(string roleName)
        {
            var users = UserManager.Users.Include(u => u.Roles).ToList();
            var role = RoleManager.Roles.FirstOrDefault(r => r.Name == roleName);

            var list = from user in users
                   where user.Roles.Any(r => r.RoleId == role.Id)
                   select user;

            return list;
        }







    }
}