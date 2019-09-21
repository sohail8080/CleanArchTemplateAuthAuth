using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate
{
    public class ExtendedRazorViewEngine : RazorViewEngine
    {
        public ExtendedRazorViewEngine()
        {
            var viewLocations = new[] {
            
            "~/BC/Home/Presentation/Views/{1}/{0}.cshtml",
            "~/BC/AccessControl/Account/Presentation/Views/{1}/{0}.cshtml",
            "~/BC/AccessControl/Manage/Presentation/Views/{1}/{0}.cshtml",
             "~/Common/Views/{0}.cshtml"

            //"~/Views/{1}/{0}.ascx",
            //"~/Views/Shared/{0}.aspx",
            //"~/Views/Shared/{0}.ascx",
            //"~/AnotherPath/Views/{0}.ascx"

            //"~/MyThemes/{1}/{0}.cshtml",
            //"~/MyThemes/{1}/{0}.vbhtml",
            //// Add a shared location too, as the lines above are controller specific
            //"~/MyThemes/{0}.cshtml",
            //"~/MyThemes/{0}.vbhtml"
        };

            this.PartialViewLocationFormats = viewLocations;
            this.ViewLocationFormats = viewLocations;

        }

    }
}