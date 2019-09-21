using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate
{

    public class CustomViewEngine : WebFormViewEngine
    {
        public CustomViewEngine()
        {
            var viewLocations = new[] {

            "~/BC/AccessControl/Home/Presentation/Views/Home/{0}.cshtml",
            "~/BC/AccessControl/Account/Presentation/Views/Account/{0}.cshtml",
            "~/BC/AccessControl/Manage/Presentation/Views/Manage/{0}.cshtml",            
            "~/Views/{1}/{0}.ascx",
            "~/Views/Shared/{0}.aspx",
            "~/Views/Shared/{0}.ascx",
            "~/AnotherPath/Views/{0}.ascx"
            // etc
        };

            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;
        }
    }

}