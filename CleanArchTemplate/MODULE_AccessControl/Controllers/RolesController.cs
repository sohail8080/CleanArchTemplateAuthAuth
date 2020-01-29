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
using System.Net;

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
        public RolesController(ApplicationDbContext context,
                                ApplicationUserManager userManager,
                                ApplicationSignInManager signInManager,
                                ApplicationRoleManager roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        ////////////////Below Controller Methods//////////////////////
        

        [HttpGet]
        public ActionResult List()
        {
            //Following we get All Roles by Role Manager
            // Both Requires ApplicationDbContext
            //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

            //var roles = await RoleManager.Roles.ToListAsync();
            var roles = RoleManager.Roles.Include(u => u.Users).ToList();
            Set_Flag_For_Admin();
            return View("List", roles);

            // Following we get All Roles by EF Repo
            // Both Requires ApplicationDbContext
            //var roles = context.Roles.ToList();
        }

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service

            var role = await RoleManager.FindByIdAsync(id);

            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();

            return View("Details", role);
        }

        // Show the Create Form
        // GET: /Roles/Create
        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new RoleFormViewModel();
            Set_Flag_For_Admin();
            return View("RoleForm", viewModel);
        }


        // Show the Role Edit Form
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var role = RoleManager.Roles.FirstOrDefault(r => r.Id == id);
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            { return HttpNotFound(); }

            var viewModel = new RoleFormViewModel(role);
            Set_Flag_For_Admin();
            return View("RoleForm", viewModel);
        }


        // Add or Update Role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RoleFormViewModel viewModel)
        {
            //IdentityResult result;

            if (!ModelState.IsValid)
            {
                return View("RoleForm", viewModel);
            }

            // Create New Role Case
            if (string.IsNullOrEmpty(viewModel.Id))
            {

                if (!RoleManager.RoleExists(viewModel.Name))
                {
                    IdentityResult result = await RoleManager.CreateAsync(new IdentityRole(viewModel.Name));
                    HandleAddResult(result); // Create Role in DB
                    
                    if(result.Succeeded)
                    { return List();}
                    else
                    { return View("RoleForm", viewModel);}

                }
                else
                {
                    ModelState.AddModelError("", "Role already exist.");
                    return View("RoleForm", viewModel);
                }

                //return List();
                //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            }
            else
            {
                // Edit Role Case

                if (!RoleManager.RoleExists(viewModel.Name))
                {
                    //var roleInDB = _context.Roles.FirstOrDefault(r => r.Id == viewModel.Id);
                    var roleInDB = await RoleManager.FindByIdAsync(viewModel.Id);

                    if (roleInDB == null)
                    {
                        return HttpNotFound();
                    }

                    roleInDB.Name = viewModel.Name;
                    IdentityResult result = await RoleManager.UpdateAsync(roleInDB);

                    //HandleUpdateResult(_context.SaveChanges());
                    HandleUpdateResult(result);

                    if (result.Succeeded)
                    { return List(); }
                    else
                    { return View("RoleForm", viewModel); }

                }
                else
                {
                    ModelState.AddModelError("", "Role already exist.");
                    return View("RoleForm", viewModel);
                }


                //return List();
                //return RedirectToAction("List", "Roles", new { area = "AccessControl" });
            }
        }


        // Save Role Name
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create2(IdentityRole role)
        //{
        //    //_context.Roles.Add(Role);
        //    RoleManager.CreateAsync(role);
        //    //_context.SaveChanges();
        //    return RedirectToAction("List", "Roles", routeValues: new { area = "AccessControl" });
        //}



        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            { return HttpNotFound(); }

            IdentityResult result = await RoleManager.DeleteAsync(role);

            HandleDeleteResult(result);

            //if (result.Succeeded)
            //{ return List(); }
            //else
            //{ return View("RoleForm", viewModel); }

            return List();



            //var role = RoleManager.FindById(id);
            //var result = RoleManager.Delete(role);

            //var role = _context.Roles.Where(r => r.Name == "my role name").FirstOrDefault();
            //var rold = _context.Roles.Select(r => r.Id == id);

            //var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            //_context.Roles.Remove(role);
            //HandleDeleteResult(_context.SaveChanges());

            //return RedirectToAction("List", "Roles", new { area = "AccessControl" });

        }




        // Following Method is to delete Role in 2 Steps
        // Following Method show the Role Deletion for Confirmation.
        // GET: /Roles/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete2(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                return HttpNotFound();
            }

            return View("Delete", role);
        }



        // Following method acutally delete the Role in 2nd Step
        // POST: /Roles/Delete/5
        [HttpPost]
        [ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
          
                if (string.IsNullOrEmpty(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var role = await RoleManager.FindByIdAsync(id);

                if (role == null)
                {
                    return HttpNotFound();
                }

                IdentityResult result;
                result = await RoleManager.DeleteAsync(role);


                //if (deleteUser != null)
                //{ result = await RoleManager.DeleteAsync(role);}
                //else
                //{ result = await RoleManager.DeleteAsync(role);}

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View("Delete", role);
                }

            //return RedirectToAction("List");
            ViewBag.Message = "Record(s) deleted successfully.";
            return List();


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