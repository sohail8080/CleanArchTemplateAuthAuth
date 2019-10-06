using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CleanArchTemplate.AccessControl.Domain;
using System.Collections;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class UsersRolesViewModel : IEnumerable<UsersRolesViewModel>
    {

        public string RoleId = string.Empty;
        public string RoleName = string.Empty;
        public string UserId = string.Empty;
        public string UserName = string.Empty;
        public bool IsRoleAdded = false;


        public static IList<UsersRolesViewModel> CreateUsersRolesList(ApplicationUser appUser,
                                   List<IdentityRole> appRoles)
        {
            List<UsersRolesViewModel> listUsersRolesViewModel = new List<UsersRolesViewModel>();

            if (appUser == null)
                throw new ArgumentNullException("Application User not found.");

            if (appRoles == null)
                throw new ArgumentNullException("Application Roles not found.");

            UsersRolesViewModel viewModel;
            foreach (var role in appRoles)
            {
                viewModel = new UsersRolesViewModel();
                viewModel.UserId = appUser.Id;
                viewModel.UserName = appUser.UserName;
                viewModel.RoleId = role.Id;
                viewModel.RoleName = role.Name;
                if (appUser.Roles.Any(r => r.RoleId == role.Id))                
                    viewModel.IsRoleAdded = true;                
                else
                    viewModel.IsRoleAdded = false;

                listUsersRolesViewModel.Add(viewModel);

            }

            return listUsersRolesViewModel;

        }



        #region Implementation of IEnumerable
        public IEnumerator<UsersRolesViewModel> GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion


    }
}