using CleanArchTemplate.AccessControl.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class UserFormViewModel
    {

        public string Id { get; set; }


        [Required]
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
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
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
                return Id != "" ? "Edit User" : "New User";
            }
        }

        public UserFormViewModel()
        {
            Id = "";
        }

        public UserFormViewModel(ApplicationUser user)
        {
            Id = user.Id;
            Email = user.Email;
            DrivingLicense = user.DrivingLicense;// New Properties
            Phone = user.Phone; // New Properties

        }



    }
}