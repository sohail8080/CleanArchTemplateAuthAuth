using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.BaseClasses;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    // When following attribute applied at global level, there is not 
    // need to do UI based Athentication. But done for demo purpose
    [Authorize(Roles = RoleName.Admin)]
    public class UsersController : BaseController
    {

        public UsersController()
        {
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public UsersController(ApplicationDbContext context,
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
            //var users =  await UserManager.Users.ToListAsync()
            var users = UserManager.Users.Include(u => u.Roles).ToList();
            //sohail Set_Flag_For_Admin();
            return View("List", users);
        }


        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var user = UserManager.Users.Where(u => u.Id == id).FirstOrDefault();
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            //sohail Set_Flag_For_Admin();

            return View("Details", user);
        }

        // Show the Create Form
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var viewModel = new CreateUserFormViewModel();
            viewModel.AllRolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            //sohail Set_Flag_For_Admin();
            return View("CreateUserForm", viewModel);
        }


        // Code to Create User with Role, Role, UserRole
        //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
        //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
        //await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)); // Create Role in DB
        //await UserManager.AddToRoleAsync(user.Id, RoleName.Admin); // Add UserRole in DB

        // Ever newly singned in User is assigned the Role of Customer
        //await UserManager.AddToRoleAsync(user.Id, "Customer"); // Add UserRole in DB

        //return List();
        //return RedirectToAction("List", "Users", new { area = "AccessControl" });


        //var users = _context.Users.Include(u => u.Roles).ToList();
        //Set_Flag_For_Admin();
        //return View("List", users);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserFormViewModel viewModel, params string[] selectedRoles)
        {
            // selectedRole is the name of the checkbox list on the html form

            // HERE WE ARE USING SAME FORM & VIEWMODEL FOR ADD & EDIT
            // BUT BOTH SCENARIOS ARE DIFFERENT,
            // ADD NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL & THEY ARE MANDATORY
            // WITH THEM MODEL WILL BE NOT VALIDATED
            // EDIT DO NOT NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL
            // MODEL VALIDATION WILL STOP US FROM EDITING USER AND WILL ASK FOR PASSWORKD & CONFIRM PASSWORD
            // SPLIT VIEWS & VIEWMODELS FOR ADD & EDIT

            IdentityResult result = null;
            ApplicationUser user = null;

            // New User

            // New User Model is Valid
            if (ModelState.IsValid)
            {
                user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                    DrivingLicense = viewModel.DrivingLicense,
                    Phone = viewModel.Phone,
                };

                result = await UserManager.CreateAsync(user, viewModel.Password);

                HandleAddResultOneError(result);

                //Add Seleted Roles to the New User 
                if (result.Succeeded)
                {
                    // New User Added Successfully now add it roles
                    if (selectedRoles != null)
                    {
                        // If some roles are selected for New User, Add those roles
                        result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

                        // Errors occurs while adding Roles to New user                           
                        if (!result.Succeeded)
                        {
                            // Error occurs while adding roles
                            ModelState.AddModelError("", result.Errors.First());
                            viewModel.AllRolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            // Show again the Create View, Ref Data Filled
                            return View("CreateUserForm", viewModel);
                        }
                        else
                        {
                            // No Errors occurs while adding Roles of New User
                            var users2 = UserManager.Users.Include(u => u.Roles).ToList();
                            //sohail Set_Flag_For_Admin();
                            ViewBag.Message = "Record(s) updated successfully.";
                            return View("List", users2);

                        }
                    }

                }
                else
                {
                    // Error occures while Adding New User
                    //AddErrors(result);
                    // Put All Roles List in the ViewBag
                    //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    viewModel.AllRolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                    //sohail Set_Flag_For_Admin();
                    return View("CreateUserForm", viewModel);
                }

                // If User Added Successfully & No Role was selected for New user OR
                // If Both User & it Roles are Added Successfully
                // Show the Users List
                var users = UserManager.Users.Include(u => u.Roles).ToList();
                //sohail Set_Flag_For_Admin();
                ViewBag.Message = "Record(s) addded successfully.";
                return View("List", users);
            }
            else
            {
                // Model is not Valid.
                // Select All Role list & show Create view.
                // All Model Errors will be automatically shown.
                // No need to add Error Message when Model is invalid.
                // Model has those Error Messages in it & Shown Automatically.
                // Show the Create Form with Ref. Data
                //ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                ModelState.AddModelError("", "Something failed.");
                viewModel.AllRolesList = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                //sohail Set_Flag_For_Admin();
                // Show again the Create View, Ref Data Filled
                return View("CreateUserForm", viewModel);
            }

        }


        // Show the Edit Form
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var user = UserManager.Users.FirstOrDefault(u => u.Id == id);
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            { return HttpNotFound(); }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            // All Roles List, with True False for Selected/Unselected Role list
            var viewModel = new EditUserFormViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                DrivingLicense = user.DrivingLicense,
                Phone = user.Phone,
                SelectedRolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            };

            //sohail Set_Flag_For_Admin();
            return View("EditUserForm", viewModel);
        }


        //var users = _context.Users.Include(u => u.Roles).ToList();
        //Set_Flag_For_Admin();
        //return View("List", users);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserFormViewModel viewModel, params string[] selectedRoles)
        {
            // selectedRole is the name of the checkbox list on the html form

            // HERE WE ARE USING SAME FORM & VIEWMODEL FOR ADD & EDIT
            // BUT BOTH SCENARIOS ARE DIFFERENT,
            // ADD NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL & THEY ARE MANDATORY
            // WITH THEM MODEL WILL BE NOT VALIDATED
            // EDIT DO NOT NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL
            // MODEL VALIDATION WILL STOP US FROM EDITING USER AND WILL ASK FOR PASSWORKD & CONFIRM PASSWORD
            // SPLIT VIEWS & VIEWMODELS FOR ADD & EDIT

            IdentityResult result = null;

            //var user = UserManager.Users.FirstOrDefault(u => u.Id == viewModel.Id);
            ApplicationUser user = await UserManager.FindByIdAsync(viewModel.Id);
            IList<string> userRoles = await UserManager.GetRolesAsync(viewModel.Id);
            // If SelectedRoles is null, then add Empty String
            selectedRoles = selectedRoles ?? new string[] { };

            IEnumerable<SelectListItem> SelectedRolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = userRoles.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });

            // User is to be Edited

            // Edit User Data is Valid
            if (ModelState.IsValid)
            {

                if (user == null)
                { return HttpNotFound(); }

                user.UserName = viewModel.Email;
                user.Email = viewModel.Email;
                user.DrivingLicense = viewModel.DrivingLicense;
                user.Phone = viewModel.Phone;

                result = await UserManager.UpdateAsync(user);

                // Error Occurs While Adding Roles
                if (!result.Succeeded)
                {
                    // Add Error
                    HandleUpdateResultOneError(result);

                    // show view
                    return View("EditUserForm", new EditUserFormViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        SelectedRolesList = SelectedRolesList
                    });
                }

                // Only add newly added roles, do not add already added roles.
                result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.Except(userRoles).ToArray<string>());

                // Error Occurs While Adding Roles
                if (!result.Succeeded)
                {
                    // Add Error
                    HandleUpdateResultOneError(result);

                    // show view
                    return View("EditUserForm", new EditUserFormViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        SelectedRolesList = SelectedRolesList
                    });
                }


                // Remove all Roles other than selected roles.
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRoles).ToArray<string>());


                // Error Occurs While Removing Roles
                if (!result.Succeeded)
                {
                    // Edit Error
                    HandleUpdateResultOneError(result);

                    return View("EditUserForm", new EditUserFormViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        SelectedRolesList = SelectedRolesList
                    });
                }

                // User Added, Role Added, Role Removed Successfully. Show List Role

                var users = UserManager.Users.Include(u => u.Roles).ToList();
                //sohail Set_Flag_For_Admin();
                ViewBag.Message = "Record(s) updated successfully.";
                return View("List", users);
            }

            // Add Error
            ModelState.AddModelError("", "Something failed.");

            return View("EditUserForm", new EditUserFormViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                SelectedRolesList = SelectedRolesList
            });

        }








        /*
                        // If User Edited successfully.
                        if (result.Succeeded)
                        {
                            // If some Roles selected for Edited User
                            if (selectedRoles != null)
                            {
                                // Remove All Roles of Edited User
                                result = await UserManager.RemoveFromRolesAsync(userInDB.Id, selectedRoles);

                                // Errors occurs while removing Roles of Edited User
                                if (!result.Succeeded)
                                {
                                    ModelState.AddModelError("", result.Errors.First());
                                    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                                    return View("EditUserForm", viewModel);
                                }
                                else
                                {
                                    // No Errors occurs while removing Roles of Edited User
                                    //????????????
                                }

                                // Add Selected Roles to the Edited User
                                result = await UserManager.AddToRolesAsync(userInDB.Id, selectedRoles);

                                // Errors occurs while updating Roles of Edited User
                                if (!result.Succeeded)
                                {
                                    ModelState.AddModelError("", result.Errors.First());
                                    ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                                    return View("EditUserForm", viewModel);
                                }
                                else
                                {
                                    // No Errors occurs while updating Roles of Edited User
                                    //????????????
                                }
                            }
                        }
                        else
                        {
                            // Error occures while Editing New User
                            AddErrors(result);
                            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                            Set_Flag_For_Admin();
                            return View("EditUserForm", viewModel);
                        }

                        //return List();
                        //return RedirectToAction("List", "Users", new { area = "AccessControl" });
                    }
                    else
                    {
                        //Edit User Data is Invalid

                    }

                    //AddErrors(result);
                    Set_Flag_For_Admin();
                    return View("EditUserForm", viewModel);

                }
        */




        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userInDB = UserManager.Users.FirstOrDefault(u => u.Id == id);
            //_context.Users.Remove(user);
            IdentityResult result = await UserManager.UpdateAsync(userInDB);
            HandleDeleteResult(result);
            //HandleDeleteResult(_context.SaveChanges());

            return List();
            //return RedirectToAction("List", "Users", new { area = "AccessControl" });
        }


        [HttpGet]
        public async Task<ActionResult> Delete2(string id)
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

            return View("Delete", user);
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

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            IdentityResult result;
            result = await UserManager.DeleteAsync(user);

            //if (deleteUser != null)
            //{ result = await RoleManager.DeleteAsync(role);}
            //else
            //{ result = await RoleManager.DeleteAsync(role);}

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
                return View("Delete", user);
            }

            ViewBag.Message = "Record(s) deleted successfully.";
            return List();

        }



        public void GetApplicationUser()
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = manager.FindById(User.Identity.GetUserId());
            //var user2 = UserManager.FindById(User.Identity.GetUserId());

            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Username = currentUser.UserName;

        }

    }
}