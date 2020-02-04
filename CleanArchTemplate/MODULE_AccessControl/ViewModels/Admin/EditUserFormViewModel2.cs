using CleanArchTemplate.AccessControl.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web.Mvc;
using System.Linq;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class EditUserFormViewModel2
    {

        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted

        [Required]
        public string Id { get; set; }


        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        // New Properties Added to the Application user

        [Required]
        [Display(Name = "Driving License")]
        public string DrivingLicense { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        // New Properties Added to the Application user


        public string Title
        {
            get
            {
                return "Edit User";
            }
        }

        public EditUserFormViewModel2()
        {
            //Id = "";
            AllRolesList = new List<UserRole>();
            AllClaimsList = new List<UserClaim>();
        }

        public EditUserFormViewModel2(ApplicationUser user)
        {
            Id = user.Id;
            Email = user.Email;
            DrivingLicense = user.DrivingLicense;// New Properties
            Phone = user.Phone; // New Properties

        }


        public List<UserRole> AllRolesList { get; set; }

        public List<UserClaim> AllClaimsList { get; set; }


        public string[] GetSelectedRoles()
        {
            return AllRolesList.Where(r => r.IsSelected == true).Select(s => s.RoleName).ToList().ToArray();
        }

        public List<Claim> GetSelectedClaims()
        {
            return AllClaimsList.Where(c => c.IsSelected == true).Select(s => new Claim(s.ClaimType, s.ClaimType)).ToList();
        }


        public bool IsAnyRoleSelected()
        {
            return AllRolesList.Any(r => r.IsSelected == true);
        }

        public bool IsAnyClaimSelected()
        {
            return AllClaimsList.Any(c => c.IsSelected == true);
        }




        //public IEnumerable<SelectListItem> SelectedRolesList { get; set; }
        //public IEnumerable<SelectListItem> SelectedClaimsList { get; set; }

        //public List<UserRole> SelectedRolesList { get; set; }
        //public List<UserClaim> SelectedClaimsList { get; set; }

        //public IEnumerable<Claim> selectedClaims { get; set; }

        //public SelectList AllRolesList { get; set; }

    }
}