using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class RoleFormViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public string Title
        {
            get
            {
                return Id != "" ? "Edit Role" : "New Role";
            }
        }

        public RoleFormViewModel()
        {
            Id = "";
        }

        public RoleFormViewModel(IdentityRole role)
        {
            Id = role.Id;
            Name = role.Name;
        }


    }
}