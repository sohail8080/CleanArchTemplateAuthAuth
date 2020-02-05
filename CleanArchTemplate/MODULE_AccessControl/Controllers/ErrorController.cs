using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using System.Threading.Tasks;
using CleanArchTemplate.AccessControl.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using CleanArchTemplate.Common.BaseClasses;

namespace CleanArchTemplate.AccessControl.Controllers
{
    [AllowAnonymous]
    public class ErrorController : BaseController
    {

        public ErrorController()
        {

        }

        public ErrorController(ApplicationDbContext context,
                        ApplicationUserManager userManager,
                        ApplicationSignInManager signInManager,
                        ApplicationRoleManager roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public ActionResult NotFound()
        {
            var statusCode = (int)System.Net.HttpStatusCode.NotFound;
            Response.StatusCode = statusCode;
            Response.TrySkipIisCustomErrors = true;
            HttpContext.Response.StatusCode = statusCode;
            HttpContext.Response.TrySkipIisCustomErrors = true;
            return View();
        }

        public ActionResult Index()
        {
            return View("Error");
        }


        public ActionResult Error()
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            return View("Error");
        }



    }
}