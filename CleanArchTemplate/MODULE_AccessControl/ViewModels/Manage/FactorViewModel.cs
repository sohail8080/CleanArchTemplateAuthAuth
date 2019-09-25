using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }
}