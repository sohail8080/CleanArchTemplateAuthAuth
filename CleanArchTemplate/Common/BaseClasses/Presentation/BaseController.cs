﻿using CleanArchTemplate.AccessControl.ViewModels;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.Common.BaseClasses
{
    public class BaseController : Controller
    {

        IList<string> userRoles = null;

        //protected void HandleResult(IdentityResult result, string successMessage)
        //{
        //    if (!result.Succeeded)
        //    { AddErrors(result); }
        //    else
        //    {
        //        ViewBag.Message = successMessage;
        //        // TempData is used in case of RedirectAction to persever data
        //        //TempData["Message"] = successMessage;
        //    }
        //}




        protected void HandleAddResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while adding Record(s)";
                AddErrors(result);
            }
            else
            {
                ViewBag.Message = "Record(s) added successfully.";
            }
        }

        protected void HandleUpdateResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
                AddErrors(result);
            }
            else
            {
                ViewBag.Message = "Record(s) updated successfully.";
            }
        }

        protected void HandleDeleteResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                ViewBag.Message = "Error occurred while deleting Record(s)";
                AddErrors(result);
            }
            else
            {
                ViewBag.Message = "Record(s) deleted successfully.";
            }
        }




        protected void HandleAddResult(int result)
        {
            if (result <= 0)
            {
                ViewBag.Message = "Error occurred while adding Record(s)";
            }
            else
            {
                ViewBag.Message = "Record(s) added successfully.";
            }
        }

        protected void HandleUpdateResult(int result)
        {
            if (result <= 0)
            {
                ViewBag.Message = "Error occurred while updating Record(s)";
            }
            else
            {
                ViewBag.Message = "Record(s) updated successfully.";
            }
        }

        protected void HandleDeleteResult(int result)
        {
            if (result <= 0)
            {
                ViewBag.Message = "Error occurred while deleting Record(s)";
            }
            else
            {
                ViewBag.Message = "Record(s) deleted successfully.";
            }
        }


        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected void RestoreViewBagMessage()
        {
            if (ViewBag.Message != null && ViewBag.Message != string.Empty)
                ViewBag.Message = ViewBag.Message;
        }


        protected void RestoreTempDataMessage()
        {
            if (TempData["Message"] != null && TempData["Message"].ToString() != string.Empty)
                ViewBag.Message = TempData["Message"].ToString();
        }

        protected void Set_Flag_For_Admin()
        {
            bool isAdminUser = false;
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                var userRoles = userManager.GetRoles(User.Identity.GetUserId());
                if (userRoles == null | userRoles.Count <= 0)
                { isAdminUser = false; }

                //if (userRoles[0].ToString() == RoleName.Admin)
                if (userRoles.Contains(RoleName.Admin))
                { isAdminUser = true; }
                else
                { isAdminUser = false; }
            }

            // this ViewBag value will be accessible in View & Child Views
            ViewBag.isAdminUser = isAdminUser;

        }


        protected void Set_Flag_For_Admin2()
        {
            // this ViewBag value will be accessible in View & Child Views
            ViewBag.isAdminUser = Is_User_In_Role2(RoleName.Admin);
        }


        protected IList<string> Get_User_Roles()
        {
            if (userRoles != null)
                return userRoles;

            if (User.Identity.GetUserId() == null)
                return null;

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            userRoles = userManager.GetRoles(User.Identity.GetUserId());

            return userRoles;

            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();


        }


        protected bool Is_User_In_Role(string role)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();            
            //var roleManager = Request.GetOwinContext().Get<RoleManager<IdentityRole>>();

            var user = User.Identity.GetUserId();
            if (user != null)
            {
                return userManager.IsInRole(User.Identity.GetUserId(), role);
            }

            return false;

        }


        protected bool Is_User_In_Role2(string role)
        {
            if (Get_User_Roles() == null || Get_User_Roles().Count <= 0)
                return false;

            return Get_User_Roles().Contains(role);

        }


        protected void Set_Flag_For_Roles()
        {
            var userRoles = Get_User_Roles();

            foreach (var role in userRoles)
            {
                ViewData.Add(role, true);
            }
        }


        protected bool Create_Role(string role)
        {
            // Following is the Startup code, this should be executed when the 
            // Application first starts and add Roles to the Application
            // This can be done in Configuration Method of Code First Migration
            // This can be done when Application DB is migrated and Ref data is added
            // Find place in MVC Framework, Code that exectues only once.

            var resultFlag = false;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());// Repo Role Class
            var roleManager = new RoleManager<IdentityRole>(roleStore); // Creat Role Service
            if (!roleManager.RoleExists("Customer"))
            {
                var result = roleManager.Create(new IdentityRole("Customer")); // Create Role in DB                        
                if (result.Succeeded)
                    resultFlag = true;
                else
                    resultFlag = false;

            }

            return resultFlag;

        }

    }
}