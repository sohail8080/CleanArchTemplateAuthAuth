using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}