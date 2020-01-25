using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.BaseClasses;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Linq;
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

        // Two App. Service used for Account Management
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext _context;

        public UsersController()
        {
            _context = new ApplicationDbContext();
        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public UsersController(ApplicationUserManager userManager,
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
            var users = _context.Users.Include(u => u.Roles).ToList();
            Set_Flag_For_Admin();
            return View("List", users);
        }

        public ActionResult Details(string id)
        {
            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            Set_Flag_For_Admin();
            return View("Details", user);
        }

        public ActionResult Create()
        {
            var viewModel = new UserFormViewModel();
            Set_Flag_For_Admin();
            return View("UserForm", viewModel);
        }

        public ActionResult Edit(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return HttpNotFound();

            var viewModel = new UserFormViewModel(user);
            Set_Flag_For_Admin();
            return View("UserForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UserFormViewModel viewModel)
        {
            IdentityResult result = null;
            if (ModelState.IsValid && string.IsNullOrEmpty(viewModel.Id))
            {
                // Creat the Domain Object                
                var user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                    DrivingLicense = viewModel.DrivingLicense,// New Properties
                    Phone = viewModel.Phone, // New Properties
                };

                // pass the Domain Object to the Service UserManager
                result = await UserManager.CreateAsync(user, viewModel.Password);
                HandleAddResult(result);

                if (result.Succeeded)
                {

                    // Code to Create User with Role, Role, UserRole
                    //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
                    //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
                    //await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)); // Create Role in DB
                    //await UserManager.AddToRoleAsync(user.Id, RoleName.Admin); // Add UserRole in DB

                    // Ever newly singned in User is assigned the Role of Customer
                    //await UserManager.AddToRoleAsync(user.Id, "Customer"); // Add UserRole in DB

                    return List();
                    //return RedirectToAction("List", "Users", new { area = "AccessControl" });


                    //var users = _context.Users.Include(u => u.Roles).ToList();
                    //Set_Flag_For_Admin();
                    //return View("List", users);
                }
                else
                {
                    AddErrors(result);
                    Set_Flag_For_Admin();
                    return View("UserForm", viewModel);
                }
            }
            else if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Id))
            {

                var userInDB = _context.Users.FirstOrDefault(u => u.Id == viewModel.Id);

                userInDB.UserName = viewModel.Email;
                userInDB.Email = viewModel.Email;
                userInDB.DrivingLicense = viewModel.DrivingLicense;
                userInDB.Phone = viewModel.Phone;
                HandleUpdateResult(_context.SaveChanges());

                return List();
                //return RedirectToAction("List", "Users", new { area = "AccessControl" });

            }
            //AddErrors(result);
            Set_Flag_For_Admin();
            return View("UserForm", viewModel);

        }

        public ActionResult Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            _context.Users.Remove(user);
            HandleDeleteResult(_context.SaveChanges());

            return List();
            //return RedirectToAction("List", "Users", new { area = "AccessControl" });
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
                return _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            }
            set
            {
                _roleManager = value;
            }
        }

        public void GetApplicationUser()
        {
            var manager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = manager.FindById(User.Identity.GetUserId());
            //var user2 = UserManager.FindById(User.Identity.GetUserId());

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());

            string ID = currentUser.Id;
            string Email = currentUser.Email;
            string Username = currentUser.UserName;

        }

    }
}