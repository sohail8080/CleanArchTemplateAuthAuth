using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CleanArchTemplate.AccessControl.Domain
{
    // Represents a Role entity in AccessControl Domain Model

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        //public ApplicationRole(string name, string description) : base(name)
        //{
        //    this.Description = description;
        //}

        // public virtual string Description { get; set; }
    }


    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole() : base() { }

    //    public ApplicationRole(string name, string description)
    //        : base(name)
    //    {
    //        this.Description = description;
    //    }

    //    public virtual string Description { get; set; }
    //}


}