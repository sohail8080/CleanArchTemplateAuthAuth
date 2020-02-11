using CleanArchTemplate.AccessControl.Domain;
using CleanArchTemplate.Common.UOW;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CleanArchTemplate.Home.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            ViewBag.Message = "Your application home page.";
            return View("Index");
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View("About");
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View("Contact");
        }

    }
}