using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.AccessControl.Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using CleanArchTemplate.Common.UOW;
using CleanArchTemplate.Common.BaseClasses;

namespace CleanArchTemplate.AccessControl.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        public AccountController()
        {

        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public AccountController(ApplicationDbContext context,
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


        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Login");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            // =============== EMAIL CONFIRMATION NEEDED=====================
            // Commnet this block if Email Confirmation no needed
            // Require the user to have a confirmed email before they can log on.

            //var user = await UserManager.FindByNameAsync(model.Email);
            var user = UserManager.Find(model.Email, model.Password);

            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");
                    
                    // Uncomment to debug locally  
                    ViewBag.Link = callbackUrl;

                    return View("ConfirmEmailInfoAfterLogin");
                    //ViewBag.errorMessage = "You must have a confirmed email to log on.";
                    //return View("Error");
                }
            }
            // =============== EMAIL CONFIRMATION NEEDED=====================

            // This doesn't count login failures towards account lockout only two factor authentication
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(
                               userName: model.Email,
                               password: model.Password,
                               isPersistent: model.RememberMe,
                               shouldLockout: false);

            switch (result)
            {
                // defaults routing rule define in the RouteConfig.cs
                // is the Controller Action executed after the successfull
                // login if there is not return url. it should be explicit here
                // Check URL is local Url.IsLocalUrl(returnUrl)

                // Lockout functionality is already there, we need to invoke it by seding 
                // shouldLockout: true in above statement. we can configure the 
                // FailtureAttempts before lockout LockoutTime in the 
                // ApplicationUserManager in Statrup ==> Identity class

                // Requires Verification is used if 2FA is enabled.
                // After UserName, password we get Confirmation Code by SMS 
                // We put that Confirmation Code in App and then Login. 2 Step Login
                //  Sign in requires addition verification (i.e. two factor)

                case SignInStatus.Success:
                    if (string.IsNullOrEmpty(returnUrl))
                    { return RedirectToAction("Index", "Home", new { area = "Home" }); }
                    else
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        { return RedirectToLocal(returnUrl); }
                        else // if RetURL is non-local, do not throw exception, but send to Home Page
                        { return RedirectToAction("Index", "Home", new { area = "Home" }); }

                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View("Login", model);
            }
        }

        // Show screen to provide SMS Code to Verify
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or 
            //external login
            // Has the user been verified (ie either via password or external login)
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }

            // Copied from Indenity Sample, do not know what following code does
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            // Following IF block is for Debugging
            if (user != null)
            {
                // Get a token for a specific two factor provider
                var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
                // Remove for Debug
                ViewBag.Code = code;
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }


            return View("VerifyCode", new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyCode", model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View("VerifyCode", model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View("Register");
        }

        //[HttpGet]
        //[HttpPost]
        //[AcceptVerbs("Get", "Post")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> IsEmailInUse(string email)
        {
            //Console.WriteLine("called");
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //return Json($"Email {email} is already in use.", JsonRequestBehavior.AllowGet);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            // Get Data as View Model from View, Validate ViewModel

            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return View("Register", model);
            }

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

            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("Register", model);
            }

            // Comment this Mosh code this only to Create User with Role, Role, UserRole
            //var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            //var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
            //await roleManager.CreateAsync(new IdentityRole(RoleName.Admin)); // Create Role in DB
            //await UserManager.AddToRoleAsync(user.Id, RoleName.Admin); // Add UserRole in DB

            // Ever newly singned in User is assigned the Role of Customer
            //await UserManager.AddToRoleAsync(user.Id, "Customer"); // Add UserRole in DB




            //===================== NO EMAIL CONFIRMATION===================
            // After Registration we are automatically Signed In. If you do not want
            // Immediate Sign In, but need Email Confirmation First, comment below line
            // and uncomment following lines.
            // Following AS Signin the User after makeing enttry to DB
            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //return RedirectToAction("Index", "Home", new { area = "Home" });
            // ==================== NO EMAIL CONFIRMATION =====================



            // =============== EMAIL CONFIRMATION NEEDED=====================
            // Commnet this block if Email Confirmation no needed
            // For more information on how to enable account confirmation and 
            //password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link

            // While registering, if you need Email Confirmation, uncomment
            // following 4 lines and comment the above 2 lines.

            // Helper Method is defined for following 3 lines, commented
            //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");

            // Uncomment to debug locally 
            //Folloiwng line can be used to 
            //debug the app and test registration without sending email. 
            TempData["ViewBagLink"] = callbackUrl;
            ViewBag.Link = callbackUrl;
            return View("ConfirmEmailInfoSRegister");
            // =============== EMAIL CONFIRMATION NEEDED=====================
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);

            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);

            await UserManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }


        //Sohail: Following Method is used for Email Confirmation
        // Method is taken form Identity Sample
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register2(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    ViewBag.Link = callbackUrl;
                    return View("DisplayEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return View("ForgotPassword", model);
            }

            var user = await UserManager.FindByNameAsync(model.Email);

            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            // For more information on how to enable account confirmation and 
            //password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

            // following 2 line for deubgging, 
            TempData["ViewBagLink"] = callbackUrl;
            ViewBag.Link = callbackUrl;

            return RedirectToAction("ForgotPasswordConfirmation", "Account", new { area = "AccessControl" });

        }

        // Taken from Identity Sample, use for Email Confirmation
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword2(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            return View("ForgotPasswordConfirmation");
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View("ResetPassword");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { area = "AccessControl" });
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account", new { area = "AccessControl" });
            }
            AddErrors(result);
            return View("ResetPassword");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View("ResetPasswordConfirmation");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View("SendCode", new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SendCode");
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage", new { area = "AccessControl" });
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    DrivingLicense = model.DrivingLicense,// New Properties
                    Phone = model.Phone, // New Properties
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View("ExternalLoginFailure");
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

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home", new { area = "Home" });
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}