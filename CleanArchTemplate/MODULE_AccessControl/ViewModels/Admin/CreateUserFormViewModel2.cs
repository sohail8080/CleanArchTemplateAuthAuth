using CleanArchTemplate.AccessControl.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class CreateUserFormViewModel2
    {
        // This shows how to Bind List of Objects on the UI.
        // This show how to get List of Objects as Automatic Model Binding 
        // when form is posted

        //public string Id { get; set; }


        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

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
                return "New User";
            }
        }

        public CreateUserFormViewModel2()
        {
            //Id = "";
            AllClaimsList = new List<UserClaim>();
        }

        public CreateUserFormViewModel2(ApplicationUser user)
        {
            //Id = user.Id;
            Email = user.Email;
            DrivingLicense = user.DrivingLicense;// New Properties
            Phone = user.Phone; // New Properties

        }

        //public IEnumerable<SelectListItem> SelectedRolesList { get; set; }

        public SelectList AllRolesList { get; set; }

        public List<UserClaim> AllClaimsList { get; set; }

        //public SelectList AllClaimsList { get; set; }

       

    }
}