using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.AccessControl.Persistence;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.BaseClasses;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        public ActionResult Index()
        {
            throw new Exception("Exception happens on server.");
        }


        [HttpGet]
        public ActionResult List()
        {
            //var users =  await UserManager.Users.ToListAsync()
            var users = UserManager.Users.Include(u => u.Roles).ToList();

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

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View("Details", user);
        }

        // Show the Create Form
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var viewModel = new CreateUserFormViewModel();

            viewModel.AllRolesList = new SelectList(
                            items: await RoleManager.Roles.ToListAsync(),
                            dataValueField: "Name",
                            dataTextField: "Name");

            viewModel.AllClaimsList = new SelectList(
                            items: ClaimsStore.AllClaims,
                            dataValueField: "value",
                            dataTextField: "type");

            return View("CreateUserForm", viewModel);
        }


        // Show the Create Form
        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted
        [HttpGet]
        public ActionResult Create2()
        {
            var viewModel = new CreateUserFormViewModel2();

            viewModel.AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
            {
                IsSelected = false,
                RoleId = x.Id,
                RoleName = x.Name
            }).ToList();


            viewModel.AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
            {
                IsSelected = false,
                ClaimType = x.Type,
                ClaimValue = x.Value,
            }).ToList();

            return View("CreateUserForm2", viewModel);
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
        public async Task<ActionResult> Create(CreateUserFormViewModel viewModel, string[] selectedRoles, string[] selectedClaims)
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


            if (!ModelState.IsValid)
            {
                // Invalid Model, all Model Errors will be auto shown, no need to add yourself
                // Model has those Error Messages in it & Shown Automatically.                              
                ModelState.AddModelError("", "Something failed.");

                viewModel.AllRolesList = new SelectList(
                                items: await RoleManager.Roles.ToListAsync(),
                                dataValueField: "Name",
                                dataTextField: "Name");

                viewModel.AllClaimsList = new SelectList(
                                items: ClaimsStore.AllClaims,
                                dataValueField: "value",
                                dataTextField: "type");

                return View("CreateUserForm", viewModel);
            }

            // New User
            user = new ApplicationUser
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                DrivingLicense = viewModel.DrivingLicense,
                Phone = viewModel.Phone,
            };


            result = await UserManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                // Error occures while Adding New User                                            
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = new SelectList(
                                items: await RoleManager.Roles.ToListAsync(),
                                dataValueField: "Name",
                                dataTextField: "Name");

                viewModel.AllClaimsList = new SelectList(
                                items: ClaimsStore.AllClaims,
                                dataValueField: "value",
                                dataTextField: "type");

                return View("CreateUserForm", viewModel);

            }


            // New User Added Successfully now add it roles
            if (selectedRoles == null)
            {
                ViewBag.Message = "Record(s) addded successfully.";
                return List();
            }

            // If some roles are selected for New User, Add those roles
            result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

            // Errors occurs while adding Roles to New user                           
            if (!result.Succeeded)
            {
                // Error occurs while adding roles
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = new SelectList(
                                items: await RoleManager.Roles.ToListAsync(),
                                dataValueField: "Name",
                                dataTextField: "Name");

                viewModel.AllClaimsList = new SelectList(
                                items: ClaimsStore.AllClaims,
                                dataValueField: "value",
                                dataTextField: "type");

                return View("CreateUserForm", viewModel);

            }

            List<Claim> selectedClaimsOnForm = ClaimsStore.AllClaims.Where(c => selectedClaims.Contains(c.Value)).ToList();

            // Adding Claim Array
            foreach (var claim in selectedClaimsOnForm)
            { result = await UserManager.AddClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                // Error occurs while adding roles
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = new SelectList(
                                items: await RoleManager.Roles.ToListAsync(),
                                dataValueField: "Name",
                                dataTextField: "Name");

                viewModel.AllClaimsList = new SelectList(
                                items: ClaimsStore.AllClaims,
                                dataValueField: "value",
                                dataTextField: "type");

                return View("CreateUserForm", viewModel);

            }


            ViewBag.Message = "Record(s) addded successfully.";
            return List();

        }


        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create2(CreateUserFormViewModel2 viewModel)
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


            if (!ModelState.IsValid)
            {
                // Invalid Model, all Model Errors will be auto shown, no need to add yourself
                // Model has those Error Messages in it & Shown Automatically.                              
                ModelState.AddModelError("", "Something failed.");

                viewModel.AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
                {
                    IsSelected = false,
                    RoleId = x.Id,
                    RoleName = x.Name
                }).ToList();


                viewModel.AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
                {
                    IsSelected = false,
                    ClaimType = x.Type,
                }).ToList();

                return View("CreateUserForm2", viewModel);
            }

            // New User
            user = new ApplicationUser
            {
                UserName = viewModel.Email,
                Email = viewModel.Email,
                DrivingLicense = viewModel.DrivingLicense,
                Phone = viewModel.Phone,
            };


            result = await UserManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                // Error occures while Adding New User                                            
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
                {
                    IsSelected = false,
                    RoleId = x.Id,
                    RoleName = x.Name
                }).ToList();


                viewModel.AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
                {
                    IsSelected = false,
                    ClaimType = x.Type,
                    ClaimValue = x.Value,
                }).ToList();

                return View("CreateUserForm2", viewModel);

            }


            // New User Added Successfully now add it roles
            if (viewModel.IsAnyRoleSelected() == false)
            {
                ViewBag.Message = "Record(s) addded successfully.";
                return List();
            }

            // If some roles are selected for New User, Add those roles
            result = await UserManager.AddToRolesAsync(user.Id, viewModel.GetSelectedRoles());

            // Errors occurs while adding Roles to New user                           
            if (!result.Succeeded)
            {
                // Error occurs while adding roles
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
                {
                    IsSelected = false,
                    RoleId = x.Id,
                    RoleName = x.Name
                }).ToList();


                viewModel.AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
                {
                    IsSelected = false,
                    ClaimType = x.Type,
                    ClaimValue = x.Value,
                }).ToList();

                return View("CreateUserForm2", viewModel);

            }

            List<Claim> selectedClaimsOnForm = viewModel.GetSelectedClaims();

            // Adding Claim Array
            foreach (var claim in selectedClaimsOnForm)
            { result = await UserManager.AddClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                // Error occurs while adding roles
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                viewModel.AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
                {
                    IsSelected = false,
                    RoleId = x.Id,
                    RoleName = x.Name,
                }).ToList();


                viewModel.AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
                {
                    IsSelected = false,
                    ClaimType = x.Type,
                    ClaimValue = x.Value,
                }).ToList();

                return View("CreateUserForm2", viewModel);

            }

            ViewBag.Message = "Record(s) addded successfully.";
            return List();

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
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            var userClaims = await UserManager.GetClaimsAsync(user.Id);

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
                }).ToList(),
                SelectedClaimsList = ClaimsStore.AllClaims.Select(x => new SelectListItem()
                {
                    Selected = userClaims.Any(uc => uc.Value == x.Value),
                    Text = x.Type,
                    Value = x.Value
                }).ToList()
            };


            return View("EditUserForm", viewModel);
        }


        // Show the Edit Form
        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted
        [HttpGet]
        public async Task<ActionResult> Edit2(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var user = UserManager.Users.FirstOrDefault(u => u.Id == id);
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);
            // gets all the current claims of the user
            var userClaims = await UserManager.GetClaimsAsync(user.Id);

            // All Roles List, with True False for Selected/Unselected Role list
            var viewModel = new EditUserFormViewModel2()
            {
                Id = user.Id,
                Email = user.Email,
                DrivingLicense = user.DrivingLicense,
                Phone = user.Phone,
                AllRolesList = RoleManager.Roles.ToList().Select(x => new UserRole()
                {
                    IsSelected = userRoles.Contains(x.Name),
                    RoleId = x.Id,
                    RoleName = x.Name
                }).ToList(),
                AllClaimsList = ClaimsStore.AllClaims.Select(x => new UserClaim()
                {
                    IsSelected = userClaims.Any(uc => uc.Value == x.Value),
                    ClaimType = x.Type,
                    ClaimValue = x.Value,
                }).ToList()
            };


            return View("EditUserForm2", viewModel);
        }


        private bool CompareClaimValues2(IList<Claim> userClaims, string value)
        {
            //foreach (var claim in userClaims)
            //{
            //    if (claim.Value.Equals(value))
            //        return true;
            //}

            //return false;

            return userClaims.Any(uc => uc.Value == value);

        }


        private List<Claim> CompareClaimValues(List<UserClaim> userClaims)
        {
            List<Claim> selecteduserClaims = new List<Claim>();

            foreach (var claim in ClaimsStore.AllClaims)
            {
                foreach (var claim2 in userClaims)
                {
                    if (claim.Type == claim2.ClaimType && claim2.IsSelected == true)
                    {
                        selecteduserClaims.Add(claim);
                    }
                }

            }

            return selecteduserClaims;

            //return userClaims.Any(uc => uc.Value == value);
        }


        //var users = _context.Users.Include(u => u.Roles).ToList();
        //Set_Flag_For_Admin();
        //return View("List", users);

        // This shows how to Bind List of Objects on the UI.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserFormViewModel viewModel, string[] selectedRoles, string[] selectedClaims)
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
            IList<Claim> userClaims = await UserManager.GetClaimsAsync(viewModel.Id);

            // If SelectedRoles is null, then add Empty String
            selectedRoles = selectedRoles ?? new string[] { };

            selectedClaims = selectedClaims ?? new string[] { };

            IEnumerable<SelectListItem> SelectedRolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            {
                Selected = userRoles.Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });

            IEnumerable<SelectListItem> SelectedClaimsList = ClaimsStore.AllClaims.Select(x => new SelectListItem()
            {
                Selected = userClaims.Any(uc => uc.Value == x.Value),
                Text = x.Type,
                Value = x.Value
            });

            // User is to be Edited


            if (!ModelState.IsValid)
            {
                // Add Error
                ModelState.AddModelError("", "Something failed.");

                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }


            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {viewModel.Email} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            user.UserName = viewModel.Email;
            user.Email = viewModel.Email;
            user.DrivingLicense = viewModel.DrivingLicense;
            user.Phone = viewModel.Phone;

            result = await UserManager.UpdateAsync(user);

            // Error Occurs While Updating User, no need to add Roles & Claims
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                // show view
                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }

            // Only add newly added roles, do not add already added roles.
            result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.Except(userRoles).ToArray<string>());


            // Error occurs while adding roles array, but user edited
            if (!result.Succeeded)
            {
                // Add error
                ViewBag.Message = "Error occurred while adding Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                // Show view
                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }

            // Remove all roles other than selected roles.
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRoles).ToArray<string>());

            // Error occurs while removing roles, but user edited, role added, not removed
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }

            // Removing Claim Array
            foreach (var claim in userClaims)
            { result = await UserManager.RemoveClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }

            //IList<Claim> selectedClaimsOnForm = new List<Claim>();

            List<Claim> selectedClaimsOnForm = ClaimsStore.AllClaims.Where(c => selectedClaims.Contains(c.Value)).ToList();

            // Adding Claim Array
            foreach (var claim in selectedClaimsOnForm)
            { result = await UserManager.AddClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm", new EditUserFormViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    DrivingLicense = user.DrivingLicense,
                    Phone = user.Phone,
                    SelectedRolesList = SelectedRolesList,
                    SelectedClaimsList = SelectedClaimsList
                });
            }

            // User Added, Role Added, Role Removed Successfully. Show List Role
            ViewBag.Message = "Record(s) updated successfully.";
            return List();


        }


        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit2(EditUserFormViewModel2 viewModel)
        {
            // selectedRole is the name of the checkbox list on the html form

            // HERE WE ARE USING SAME FORM & VIEWMODEL FOR ADD & EDIT
            // BUT BOTH SCENARIOS ARE DIFFERENT,
            // ADD NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL & THEY ARE MANDATORY
            // WITH THEM MODEL WILL BE NOT VALIDATED
            // EDIT DO NOT NEED PASSWORD & CONFIRM PASSWORD IN VIEW & VIEWMODEL
            // MODEL VALIDATION WILL STOP US FROM EDITING USER AND WILL ASK FOR PASSWORKD & CONFIRM PASSWORD
            // SPLIT VIEWS & VIEWMODELS FOR ADD & EDIT

            //var user = UserManager.Users.FirstOrDefault(u => u.Id == viewModel.Id);

            // If SelectedRoles is null, then add Empty String
            //selectedRoles = selectedRoles ?? new string[] { };

            // not needed as by default initized in constructor
            //selectedClaims = selectedClaims ?? new string[] { };

            //IEnumerable<SelectListItem> SelectedRolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
            //{
            //    Selected = userRoles.Contains(x.Name),
            //    Text = x.Name,
            //    Value = x.Name
            //});

            //IEnumerable<SelectListItem> SelectedClaimsList = ClaimsStore.AllClaims.Select(x => new SelectListItem()
            //{
            //    Selected = userClaims.Any(uc => uc.Value == x.Value),
            //    Text = x.Type,
            //    Value = x.Value
            //});

            IdentityResult result = null;
            IList<string> userRoles = await UserManager.GetRolesAsync(viewModel.Id);
            IList<Claim> userClaims = await UserManager.GetClaimsAsync(viewModel.Id);

            if (!ModelState.IsValid)
            {
                // Add Error
                ModelState.AddModelError("", "Something failed.");
                return View("EditUserForm2", viewModel);
            }

            ApplicationUser user = await UserManager.FindByIdAsync(viewModel.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {viewModel.Email} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            user.UserName = viewModel.Email;
            user.Email = viewModel.Email;
            user.DrivingLicense = viewModel.DrivingLicense;
            user.Phone = viewModel.Phone;

            result = await UserManager.UpdateAsync(user);

            // Error Occurs While Updating User, no need to add Roles & Claims
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm2", viewModel);
            }

            // Only add newly added roles, do not add already added roles.
            result = await UserManager.AddToRolesAsync(user.Id, viewModel.GetSelectedRoles().Except(userRoles).ToArray<string>());


            // Error occurs while adding roles array, but user edited
            if (!result.Succeeded)
            {
                // Add error
                ViewBag.Message = "Error occurred while adding Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm2", viewModel);
            }

            // Remove all roles other than selected roles.
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(viewModel.GetSelectedRoles()).ToArray<string>());

            // Error occurs while removing roles, but user edited, role added, not removed
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm2", viewModel);
            }

            // Removing Claim Array
            foreach (var claim in userClaims)
            { result = await UserManager.RemoveClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm2", viewModel);
            }

            //IList<Claim> selectedClaimsOnForm = new List<Claim>();

            List<Claim> selectedClaimsOnForm = CompareClaimValues(viewModel.AllClaimsList);

            // Adding Claim Array
            foreach (var claim in selectedClaimsOnForm)
            { result = await UserManager.AddClaimAsync(user.Id, claim); }


            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }

                return View("EditUserForm2", viewModel);
            }

            // User Added, Role Added, Role Removed Successfully. Show List Role
            ViewBag.Message = "Record(s) updated successfully.";
            return List();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var userInDB = UserManager.Users.FirstOrDefault(u => u.Id == id);
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            IdentityResult result;
            result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }
                return View("Delete", user);
            }

            ViewBag.Message = "Record(s) deleted successfully.";
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
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
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
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
                //return HttpNotFound();
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                { ModelState.AddModelError("", error); }
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