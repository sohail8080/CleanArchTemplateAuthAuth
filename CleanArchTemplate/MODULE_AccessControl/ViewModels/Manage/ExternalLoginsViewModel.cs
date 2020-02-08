using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class ExternalLoginsViewModel
    {

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        //public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        //[TempData]
        public string StatusMessage { get; set; }

    }
}