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
    // Configure the application sign-in manager which is used in this application.
    // Manages Sign In operations for users
    // Domain Service for Sign In Operations
    // used to log in users log out.
    // uses ApplicationUserManager for its operations
    // Factory Class for the inject ApplicationSignInManager in owin context
    // Does not requrire Configuration as in case of UserManager configuration

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        // Factory Method for creating the Object of itself for Startup.Auth.cs
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}