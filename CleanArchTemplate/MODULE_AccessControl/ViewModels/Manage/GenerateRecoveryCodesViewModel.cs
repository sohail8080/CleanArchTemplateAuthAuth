using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class GenerateRecoveryCodesViewModel
    {

        //[TempData]
        public string[] RecoveryCodes { get; set; }

        //[TempData]
        public string StatusMessage { get; set; }

    }
}