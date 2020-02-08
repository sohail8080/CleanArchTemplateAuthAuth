using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class TwoFactorAuthenticationViewModel
    {

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }


        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

 
        public string StatusMessage { get; set; }


    }
}