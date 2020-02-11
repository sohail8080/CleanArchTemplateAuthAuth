using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class RegisterViewModel
    {
        //Required
        //Range
        //StringLength
        //Compare
        //Regular Expression


        //[ValidEmailDomain(allowedDomain: "pragimtech.com",
        //ErrorMessage = "Email domain must be pragimtech.com")]
        [Remote(action: "IsEmailInUse", controller: "Account", areaName: "AccessControl", ErrorMessage = "Email already in use.")]
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [Editable(true)]
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


    }
}