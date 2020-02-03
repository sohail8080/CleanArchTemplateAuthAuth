﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }

        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
}