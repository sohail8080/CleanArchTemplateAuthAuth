using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CleanArchTemplate.AccessControl.Domain;

namespace CleanArchTemplate.Common.UOW
{

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<IdentityRoleClaim> RoleClaims { get; set; }

        public DbSet<IdentityUserToken> UserTokens { get; set; }

        public ApplicationDbContext()
         : base("DefaultConnection", throwIfV1Schema: false)
         //: base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    foreach (var foreignKey in modelBuilder.Entity().SelectMany(e => e.GetForeignKeys()))
        //    {
        //        foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        //    }
        //}


    }
}