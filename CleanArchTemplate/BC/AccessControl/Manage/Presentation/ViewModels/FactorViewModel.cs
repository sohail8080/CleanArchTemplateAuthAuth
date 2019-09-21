using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CleanArchTemplate.BC.AccessControl.Manage.Presentation.ViewModels
{
    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }
}