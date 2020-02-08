using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class EnableAuthenticatorViewModel
    {

        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }


        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        //[TempData]
        public string[] RecoveryCodes { get; set; }

        //[TempData]
        public string StatusMessage { get; set; }

    }
}