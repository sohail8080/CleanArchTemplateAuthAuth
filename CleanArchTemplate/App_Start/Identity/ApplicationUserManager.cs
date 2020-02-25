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
using System.Threading;
using OtpSharp;
using Base32;

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

        internal ApplicationDbContext Context = new ApplicationDbContext();

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

            // Our standard application at this point has 2 token providers registered,
            // namely the PhoneNumberTokenProvider and EmailTokenProvider.
            // These are used to send 2 factor authentication tokens to a 
            // user’s cellphone number and email address respectively.
            // The samples package automatically created the code which 
            // enable these 2 providers and it can be found in the 
            // App_Start\IdentityConfig.cs class:

            // Register two factor authentication providers. 
            // This application uses Phone and Emails as a step of receiving 
            // a code for verifying the user
            // You can write your own provider and plug it in here.

            // WHILE USER LOGIN, 2FA CODES CAN BE SEND BY THE EMAIL OR PHONE            

            // Register a two factor authentication provider with the TwoFactorProviders 
            // mapping
            // TokenProvider that generates tokens from the user's security stamp 
            // and notifies a user via their phone number

            // 2 token providers registered, PhoneNumberTokenProvider and EmailTokenProvider.
            // These are used to send 2 factor authentication tokens to 
            // a user’s cellphone number and email address respectively.
            // Following code enable these 2 providers
            // Both of these providers ultimately implement the IUserTokenProvider class
            // These Provider do the following jobs
            // 1- Generate a token for a user with a specific purpose: GenerateAsync()
            // 2- Validate a token for a user with a specific purpose: ValidateAsync()
            // 3- Notifies the user that a token has been generated, for example an email 
            // or sms could be sent, or this can be a no-op: NotifyAsync()
            // 4-Returns true if provider can be used for this user, 
            // i.e. could require a user to have an email IsValidProviderForUserAsync()

            manager.RegisterTwoFactorProvider("Phone Code",
                        new PhoneNumberTokenProvider<ApplicationUser>
                        {
                            MessageFormat = "Your security code is {0}"
                        });

            //     TokenProvider that generates tokens from the user's 
            // security stamp and notifies
            //     a user via their email
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


        public bool HasPassword(string userId)
        {
            var user = this.FindById(userId);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }


        public bool HasPhoneNumber(string userId)
        {
            var user = this.FindById(userId);
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }


        /// <summary>
        /// Find a user token if it exists.
        /// </summary>
        /// <param name="user">The token owner.</param>
        /// <param name="loginProvider">The login provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user token if it exists.</returns>
        protected Task<IdentityUserToken> FindTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Context.UserTokens.FindAsync(new object[] { user.Id, loginProvider, name }, cancellationToken);
        }

        /// <summary>
        /// Add a new user token.
        /// </summary>
        /// <param name="token">The token to be added.</param>
        /// <returns></returns>
        protected Task AddUserTokenAsync(IdentityUserToken token)
        {
            Context.UserTokens.Add(token);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Remove a new user token.
        /// </summary>
        /// <param name="token">The token to be removed.</param>
        /// <returns></returns>
        protected Task RemoveUserTokenAsync(IdentityUserToken token)
        {
            Context.UserTokens.Remove(token);
            return Task.CompletedTask;
        }


        public string GetAuthenticatorKeyAsync(ApplicationUser user)
        {
            var usertoken = Context.UserTokens.SingleOrDefault(ut => ut.UserId == user.Id);

            if (usertoken == null)
            {
                return null;
            }
            else
            {
                // AuthenticatorKey
                return usertoken.Value;
            }
        }


        public IdentityResult ResetAuthenticatorKeyAsync(ApplicationUser user)
        {

            try
            {

                var usertoken = Context.UserTokens.SingleOrDefault(ut => ut.UserId == user.Id);

                if (usertoken != null)
                {
                    Context.UserTokens.Remove(Context.UserTokens.SingleOrDefault(ut => ut.UserId == user.Id));
                }

                byte[] authenticatorKey = KeyGeneration.GenerateRandomKey(20);
                var identityUserToken = new IdentityUserToken();
                identityUserToken.UserId = user.Id;
                identityUserToken.LoginProvider = "AspNetUserStore";
                identityUserToken.Name = "AuthenticatorKey";               
                identityUserToken.Value = Base32Encoder.Encode(authenticatorKey);


                Context.UserTokens.Add(identityUserToken);
                Context.SaveChanges();

                return IdentityResult.Success;
            }
            catch (Exception exception)
            {

                return new IdentityResult(exception.ToString());
            }

        }

    }




}
