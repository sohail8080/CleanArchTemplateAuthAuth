using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.BaseClasses;
using CleanArchTemplate.Common.UOW;

namespace CleanArchTemplate.AccessControl.Controllers
{
    // All these Use Cases are allowed to the Singed In Users
    // There for Authorize Attribute at the level of Controller Class
    [Authorize]
    public class ManageController : BaseController
    {
        public ManageController()
        {

        }

        // Based on the Configuration, both Services will be provided by DI/IOC
        // Currently they are coded in the controller.
        public ManageController(ApplicationDbContext context,
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
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View("Index", model);
        }

        // Coped from Indenity, I do not know where used
        // GET: /Account/RemoveLogin
        [HttpGet]
        public ActionResult RemoveLogin()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }


        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            //return RedirectToAction("ManageLogins", new { Message = message });
            return RedirectToAction("ManageLogins", "Manage", new { area = "AccessControl", Message = message });
        }

        // Just show the Screen to input the Mobile Number
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View("AddPhoneNumber");
        }

        // This Screen is used to get User Phone, Send SMS to that Phone, Ask for Validation
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddPhoneNumber", model);
            }

            // To use your phone number for 2 Factor Authentication you will need 
            // to add it to your user account first.


            //After you have confirmed the new phone number you can use it to 
            // receive your 2 Factor Authentication security code when logging in. 
            // To test this make sure you log out of the application and log in to
            // the application again.  After logging in using your email address 
            // and password you will be presented with a page where you can specify
            // where you want to send the 2 Factor Authentication security code to.
            // We have added both an email address and phone number we can send it 
            // to either, but for the purposes of this demo we will select PhoneCode 
            // in order to send an SMS to our phone number.

            // Generate the token and send it
            // Generate a code that the user can use to change their phone number to a specific
            // number
            // This Code will be send to the User Mobile Number
            // For Mobile Number Verification, User will Re-Type this Code
            // to verify that Phone Number is his phone number.

          // The 2FA codes are generated using Time - based One - time Password 
          // Algorithm and codes are valid for six minutes. If you take more than 
          // six minutes to enter the code, you'll get an Invalid code error message.

          // You can add more 2FA providers such as QR code generators or 
          // you can write you own(See Using Google Authenticator 
          // with ASP.NET Identity).

          // Code/Token to Verify Phone != Code to Signin under 2FA
          var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);

            // If the SMS service is configured, then Send Code to the SMS
            // This is done only to ensure the User on the Computer is providing the 
            // Phone number he has in his hand. It is not some ones else.
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };

                // With No implementaiton nothing is done and success response is returned.
                await UserManager.SmsService.SendAsync(message);
            }

            // Redirect to VerifyPhoneNumber Screen.
            //return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
            return RedirectToAction("VerifyPhoneNumber", "Manage", new { area = "AccessControl", PhoneNumber = model.Number });
        }


        // Coped from Indentity sample i do not know used where
        // POST: /Manage/RememberBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RememberBrowser()
        {
            var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);

            //return RedirectToAction("Index", "Manage");
            return RedirectToAction("Index", "Manage", new { area = "AccessControl" });
        }

        // Coped from Indentity sample i do not know used where
        // POST: /Manage/ForgetBrowser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetBrowser()
        {

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            //return RedirectToAction("Index", "Manage");
            return RedirectToAction("Index", "Manage", new { area = "AccessControl" });
        }


        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            //Set two factor authentication enabled for a user.
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                //     Creates a user identity and then signs the identity using the 
                //AuthenticationManager
                // I think this refresh the Login Cookie after enabling the 2FA

                //Note the SignInAsync must be called because enable 2FA is a 
                //change to  the security profile. When 2FA is enabled, 
                // the user will need to use 2FA to log in, 
                // using the 2FA approaches they have registered
                // (SMS and email in the sample).
                // Adding 2FA approaches is a seperate step, it could be Email/SMS
                // Enabbling/Disabling the 2FA is seperate step, 
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            // If User not found, then go to Index Page
            return RedirectToAction("Index", "Manage", new { area = "AccessControl" });
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            // Set 2fa disabled
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                // Refresh Login Cookie
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage", new { area = "AccessControl" });
        }

        // We get to this View when we ADD/CHANGE the phone number
        // on the Index.cshtml of the Manage module
        // Invoke this View by clicking the User Name Link on Page
        // This View is not invoked by the User directly.
        // GET: /Manage/VerifyPhoneNumber
        // This View Does nothing with the phoneNumber but just pass it to the 
        // Post.VerifyPhoneNumber() so that it could be verified and put in the 
        // Database. This View Just take the Verificatin Code & Phone as pass it to the
        // Post.VerifyPhoneNumber()
        // Following Debugging Code is uses the Phone number to generated the Token
        // and pass to the Next View that is used to verify the Code.
        // Code itself is sent in the Post.AddPhoneNumber();
        // Post.AddPhoneNumber() + Get.VerifyPhoneNumber() are called in sequence.

        [HttpGet]
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            //     Generate a code that the user can use to change their phone number 
            //     to a specific number
            // Following two line are for Debugging purpose
            // In
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            ViewBag.Code = code;
            // Send an SMS through the SMS provider to verify the phone number

            if (phoneNumber == null)
            {
                return View("Error");
            }
            else
            {
                var viewModel = new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber };
                return View("VerifyPhoneNumber", viewModel);
            }

            // Above is my Code, following is template code
            //return phoneNumber == null ? View("Error") : View("VerifyPhoneNumber", new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }


        //
        // GET: /Account/VerifyPhoneNumber
        [HttpGet]
        public async Task<ActionResult> VerifyPhoneNumber2(string phoneNumber)
        {
            // This code allows you exercise the flow without actually sending codes
            // For production use please register a SMS provider in IdentityConfig and 
            // generate a code here.
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            ViewBag.Status = "For DEMO purposes only, the current code is " + code;
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }


        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyPhoneNumber", model);
            }

            // UserId, PhoneNo, Token/Code is used to Change the phone number
            // Token verification is done inside the Business class of UserManager.ChangePhoneNumberAsync()

            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);

            if (!result.Succeeded)
            {
                // If we got this far, something failed, redisplay form
                ModelState.AddModelError("", "Failed to verify phone");
                return View("VerifyPhoneNumber", model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                // Refresh the Login Cookie
                //     Creates a user identity and then signs the identity using the 
                // AuthenticationManager
                // When you change your security profile, a new security stamp is 
                // generated and stored in the SecurityStamp field of the 
                // AspNetUsers table.Note, the SecurityStamp field is different 
                // from the security cookie.The security cookie is not stored in the 
                // AspNetUsers table(or anywhere else in the Identity DB).
                // The security cookie token is self - signed using DPAPI and is 
                // created with the UserId, SecurityStamp and expiration time 
                // information.

                // The cookie middleware checks the cookie on each request.
                // The SecurityStampValidator method in the Startup class hits the DB 
                // and checks security stamp periodically, as specified with the 
                // validateInterval.This only happens every 30 minutes (in our sample) 
                // unless you change your security profile.The 30 minute interval was 
                // chosen to minimize trips to the database.

                // The SignInAsync method needs to be called when any change is made 
                // to the security profile.When the security profile changes, 
                // the database is updates the SecurityStamp field, and without 
                // calling the SignInAsync method you would stay logged in only 
                // until the next time the OWIN pipeline hits the database
                // (the validateInterval).You can test this by changing the 
                // SignInAsync method to return immediately, and setting the cookie 
                // validateInterval property from 30 minutes to 5 seconds:

                // Clear the temporary cookies used for external and two factor sign ins

                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            // Index Page of Manage Module
            //return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.AddPhoneSuccess, area = "AccessControl" });

        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);

            if (!result.Succeeded)
            {
                //return RedirectToAction("Index", new { Message = ManageMessageId.Error });
                return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.Error, area = "AccessControl" });
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            //return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.RemovePhoneSuccess, area = "AccessControl" });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("ChangePassword", model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.ChangePasswordSuccess, area = "AccessControl" });


        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View("SetPassword");
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return View("SetPassword", model);
            }

            var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("SetPassword", model);
            }

            // Success
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }

            //return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
            return RedirectToAction("Index", "Manage", new { Message = ManageMessageId.SetPasswordSuccess, area = "AccessControl" });

        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View("ManageLogins", new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                //return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
                return RedirectToAction("ManageLogins", "Manage", new { Message = ManageMessageId.Error, area = "AccessControl" });

            }

            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion


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