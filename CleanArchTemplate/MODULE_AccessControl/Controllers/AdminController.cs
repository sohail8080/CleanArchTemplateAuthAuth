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
    [Authorize(Roles = RoleName.Admin)]
    public class AdminController : BaseController
    {

        public AdminController()
        {

        }

        public AdminController(ApplicationDbContext context,
                        ApplicationUserManager userManager,
                        ApplicationSignInManager signInManager,
                        ApplicationRoleManager roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        {           
            return View("Index");
        }

    }
}