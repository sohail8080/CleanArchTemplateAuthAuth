using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CleanArchTemplate
{
    //Exposes role related api which will automatically save changes 
    // to the RoleStore

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(RoleStore<IdentityRole> store)
                                    : base(store)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }

        internal ApplicationDbContext Context = new ApplicationDbContext();


        /// <summary>
        /// Get the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public async virtual Task<IList<Claim>> GetClaimsAsync(IdentityRole role, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            //return await Context.RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToListAsync(cancellationToken);
            return Context.RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }


        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task AddClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            Context.RoleClaims.Add(CreateRoleClaim(role, claim));
            return Task.FromResult(false);
        }


        /// <summary>
        /// Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async virtual Task RemoveClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            //var claims = await Context.RoleClaims.Where(rc => rc.RoleId.Equals(role.Id) && rc.ClaimValue == claim.Value && rc.ClaimType == claim.Type).ToList(cancellationToken);
            var claims = Context.RoleClaims.Where(rc => rc.RoleId.Equals(role.Id) && rc.ClaimValue == claim.Value && rc.ClaimType == claim.Type).ToList();
            foreach (var c in claims)
            {
                Context.RoleClaims.Remove(c);
            }
        }


        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual IdentityRoleClaim CreateRoleClaim(IdentityRole role, Claim claim)
            => new IdentityRoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };


    }
}