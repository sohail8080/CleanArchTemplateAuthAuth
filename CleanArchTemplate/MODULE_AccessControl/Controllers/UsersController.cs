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
    [Authorize(Roles ="Admin")]
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

        // GET: Users       
        public ActionResult Index()
		{
            var users = _context.Users.Include(u => u.Roles).ToList();
            Set_Flag_For_Admin();
			return View("Index", users);
		}

        public ActionResult Details(string id)
        {            
            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            return View("Details", user);
        }


        public ActionResult Delete(string id)
        {
            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            _context.Users.Remove(user);
            _context.SaveChanges();

            var users = _context.Users.Include(u => u.Roles).ToList();
            Set_Flag_For_Admin();
            return View("Index", users);
        }



        public ActionResult Create()
        {
            return View("Create");
        }



        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            // Get Data as View Model from View, Validate ViewModel
            if (ModelState.IsValid)
            {
                // Creat the Domain Object                
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DrivingLicense = model.DrivingLicense,// New Properties
                    Phone = model.Phone, // New Properties
                };

                // pass the Domain Object to the Service UserManager
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {

                    // Code to Create User with Role, Role, UserRole
                    //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
                    //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
                    //await roleManager.CreateAsync(new IdentityRole("Admin")); // Create Role in DB
                    //await UserManager.AddToRoleAsync(user.Id, "Admin"); // Add UserRole in DB

                    // Ever newly singned in User is assigned the Role of Customer
                    //await UserManager.AddToRoleAsync(user.Id, "Customer"); // Add UserRole in DB

                    var users = _context.Users.Include(u => u.Roles).ToList();
                    Set_Flag_For_Admin();
                    return View("Index", users);
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Register", model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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




    }
}