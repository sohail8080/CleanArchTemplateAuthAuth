using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;

namespace CleanArchTemplate
{
    // This is whole about Configuring the Asp.net Indentiy
    // in asp.net core this is done in Start up ConfigureService();
    // This is Configuration Wrappper Class for UserManage Class. 
    // This class extends UserManager Class.
    // We can Use Composition instead of Ineheritence for this job
    // This class adds Factory Method for Creating ApplicationUserManager
    // In application the methods of Super Class are used.
    // Factory Method is used to inject this class into OWIN Context
    // I think OWIN context is some Application level repo for obejcts
    // I thin OWIN Context is some Name Service use to store objects
    // application level.

    // Configure the application user manager used in this application. 
    //UserManager is defined in ASP.NET Identity and is used by the application.
    // This class is just a Configuraiton Wrapper on the top of UserManager

    // This is the Factory Class that provides Configure Application User Manager
    // ApplicationUserManager configuration tells how we want to Configure
    // the Auth and Auth related to a User. This class proivdes methods to 
    // Hydrate and Perisist UserInfo in the underlying Idenity Tables related
    // to Users. Thsi class is the Domain Service Class that provides 
    // Services methods Users. it inovolve complex Auth & Auth Biz Logic.
    // As Configuring UserManager is complex therefore put in seperate class
    // By Configuration we tell HOW we want to use this part of Auth & Auth
    // This class is used when we want to customize UI for different users 
    // based on Auth and Auth configuration done below.
    // This class is used when we want to have Admin Manager App for manage
    // the Auth & Auth part of our application

    // UserManager for users where the primary key for the User is of type string
    // Exposes user related api which will automatically save changes to the 
    // UserStore

    // This class extend UserManager class for Application User
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        // Followong method is used to create the ApplicationUserManager
        // to manage Users inside the Application
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            // This will return a ApplicationUserManager hooked to the UserStore
            // This will return a ApplicationUserManager hooked to the DB

            // UserStore 
            // EntityFramework based user store implementation that supports IUserStore, 
            // IUserLoginStore,
            // IUserClaimStore and IUserRoleStore
            // Default constuctor which uses a new instance of a 
            // default EntityyDbContext

            // Class creating the objet of itself
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            
            // Configure validation logic for usernames
            // What type of UserName are allow and not allowed
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            // What type of password policy application will have
            // Password Policy can be configured here.
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,

            };

            // Configure user lockout defaults
            // Configure User Lock out Policy
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. 
            // This application uses Phone and Emails as a step of receiving 
            // a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", 
                new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            manager.RegisterTwoFactorProvider("Email Code", 
                new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });


            //     Used to send email
            manager.EmailService = new EmailService();
            //     Used to send a sms message
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            // Return the Configured User Manager
            return manager;
        }
    }
}