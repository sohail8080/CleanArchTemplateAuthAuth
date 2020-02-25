using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }

        //public bool TwoFactor { get; set; }// Old Property
        public bool Is2faEnabled { get; set; } // New Property
       

        //public bool BrowserRemembered { get; set; }// Old Property
        public bool IsMachineRemembered { get; set; } // New Property

        // Following are new properties
        public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string StatusMessage { get; set; }
        public bool HasAuthenticator { get; set; }
        public int RecoveryCodesLeft { get; set; }
       
       


    }
}