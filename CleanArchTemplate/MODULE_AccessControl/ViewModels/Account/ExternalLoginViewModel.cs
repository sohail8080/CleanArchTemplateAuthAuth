using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        //[TempData]
        public string ErrorMessage { get; set; }

    }
}