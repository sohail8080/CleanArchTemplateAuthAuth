using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CleanArchTemplate.AccessControl.ViewModels
{
    public class UserRole
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}