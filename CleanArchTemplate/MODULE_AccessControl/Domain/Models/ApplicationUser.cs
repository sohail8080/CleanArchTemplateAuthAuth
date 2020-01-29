using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CleanArchTemplate.AccessControl.Domain
{
    // You can add profile data for the user by adding more properties 
    // to your ApplicationUser class, 
    // please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    // Update Model, RegisterViewModel, Register.cshtml, 
    // AccountController==>Register()

    // IdentityUser: Default EntityFramework IUser implementation
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(255)]
        public string DrivingLicense { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in 
            // CookieAuthenticationOptions.AuthenticationType
            // Creates a ClaimsIdentity representing the user

            // The code below generates a ClaimsIdentity.ASP.NET Identity 
            // and OWIN Cookie Authentication are claims-based, therefore the 
            // framework requires the app to generate a ClaimsIdentity for the user. 
            // ClaimsIdentity has information about all the claims for the user, 
            // such as the user's name, age and what roles the user belongs to. 
            // You can also add more claims for the user at this stage.

            //The OWIN AuthenticationManager.SignIn method passes in the 
            //ClaimsIdentity and signs in the user:

            //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
            //{
            //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //    AuthenticationManager.SignIn(new AuthenticationProperties()
            //    {
            //        IsPersistent = isPersistent
            //    },
            //       await user.GenerateUserIdentityAsync(UserManager));
            //}

            var userIdentity = await manager.CreateIdentityAsync(this, 
                DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            // This is extension point, we can add more name value pairs here
            // related top Claims based Identity.
            // Claim: Represents a claims-based identity.

            return userIdentity;
        }
    }
}