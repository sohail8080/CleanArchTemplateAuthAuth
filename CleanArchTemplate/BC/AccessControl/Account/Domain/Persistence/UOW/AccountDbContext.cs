using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CleanArchTemplate.BC.AccessControl.Account.Domain.Models;

namespace CleanArchTemplate.BC.AccessControl.Account.Domain.Persistence.UOW
{
    //public class AccountDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public AccountDbContext()
    //        : base("AccountDbContext", throwIfV1Schema: false)
    //    {
    //    }

    //    public static AccountDbContext Create()
    //    {
    //        return new AccountDbContext();
    //    }
    //}
}