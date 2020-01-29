using CleanArchTemplate.AccessControl.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class EditUserFormViewModel
    {

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

        public EditUserFormViewModel()
        {
            //Id = "";
        }

        public EditUserFormViewModel(ApplicationUser user)
        {
            Id = user.Id;
            Email = user.Email;
            DrivingLicense = user.DrivingLicense;// New Properties
            Phone = user.Phone; // New Properties

        }

        public IEnumerable<SelectListItem> SelectedRolesList { get; set; }

        //public SelectList AllRolesList { get; set; }

    }
}