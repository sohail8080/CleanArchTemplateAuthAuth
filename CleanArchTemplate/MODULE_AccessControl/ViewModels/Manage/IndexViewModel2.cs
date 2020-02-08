using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class IndexViewModel2
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        //[TempData]
        public string StatusMessage { get; set; }

    }
}