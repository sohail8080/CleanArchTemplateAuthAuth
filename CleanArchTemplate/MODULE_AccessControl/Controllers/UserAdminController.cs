using CleanArchTemplate;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.BaseClasses;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : BaseController
    {
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }


        // DONE
        // GET: /Users/
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }


        // DONE
        // GET: /Users/Details/5
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View("Details", user);
        }


        // DONE
        // GET: /Users/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }


        //
        // POST: /Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                // Model is valid
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add Seleted Roles to the New User 
                if (adminresult.Succeeded)
                {
                    // New User Added Successfully now add it roles
                    if (selectedRoles != null)
                    {
                        // If some roles are selected for New User, Add those roles
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            // Error occurs while adding roles
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            // Show again the Create View, Ref Data Filled
                            return View();
                        }
                    }
                }
                else
                {
                    // Error occurs while adding User
                    ModelState.AddModelError("", adminresult.Errors.First());
                    // Put All Roles List in the ViewBag
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    // Show again the Create View, Ref Data Filled
                    return View();
                }

                // If User Added Successfully & No Role was selected for New user
                //If Both User & it Roles are Added Successfully
                // Show the Users List
                return RedirectToAction("Index");
            }

            // Model is not Valid.
            // Select All Role list & show Create view.
            // All Model Errors will be automatically shown.
            // No need to add Error Message when Model is invalid.
            // Model has those Error Messages in it & Shown Automatically.
            // Show the Create Form with Ref. Data
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            // Show again the Create View, Ref Data Filled
            return View();
        }


        // DONE
        // GET: /Users/Edit/1
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

 
        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email, Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            ApplicationUser user = null;
            IList<string> userRoles = null;

            if (ModelState.IsValid)
            {
                // Edit User Data is Valid

                user = await UserManager.FindByIdAsync(editUser.Id);

                if (user == null)
                { return HttpNotFound(); }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                userRoles = await UserManager.GetRolesAsync(user.Id);

                // If SelectedRoles is null, then add Empty String
                selectedRole = selectedRole ?? new string[] { };

                // Only add newly added roles, do not add already added roles.
                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());

                    user = await UserManager.FindByIdAsync(editUser.Id);
                    userRoles = await UserManager.GetRolesAsync(user.Id);
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    });
                }

                // Remove all Roles other than selected roles.
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());

                    user = await UserManager.FindByIdAsync(editUser.Id);
                    userRoles = await UserManager.GetRolesAsync(user.Id);
                    return View(new EditUserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                        {
                            Selected = userRoles.Contains(x.Name),
                            Text = x.Name,
                            Value = x.Name
                        })
                    });
                }

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Something failed.");

            //return RedirectToAction("Edit", routeValues: new { id = editUser.Id } );

            user = await UserManager.FindByIdAsync(editUser.Id);
            userRoles = await UserManager.GetRolesAsync(user.Id);
            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });

            //return View(ViewModel);
            //return View(GetUserFormViewModel(editUser.Id).Result);
        }


        //
        // GET: /Users/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        //
        // POST: /Users/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        private async Task<EditUserViewModel> GetEditUserViewModel(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
                return null;

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var ViewModel = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };

            return ViewModel;
        }


        private async Task<EditUserViewModel> GetEditUserViewModel(ApplicationUser user)
        {
            //var user = await UserManager.FindByIdAsync(id);

            //if (user == null)
            //return null;

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var ViewModel = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };

            return ViewModel;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }

                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

            }

            base.Dispose(disposing);
        }

    }
}
