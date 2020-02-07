using CleanArchTemplate.AccessControl.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CleanArchTemplate
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Start()
        {
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new CustomViewEngine());

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ExtendedRazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // To handle exceptions
            //GlobalFilters.Filters.Add(new CustomExceptionFilter());///////////

        }

        // Any unhandeled exception within ASP.NET will bubble up to this event. 
        //There is also no concept of routes anymore(because it is outside the MVC scope). 
        //If you want to redirect to a specific error page you have to know
        //the 
        //1- exact URL or 
        //2- configure it to co-exist with "customErrors" or "httpErrors" in the web.config
        //But be careful, if you have set >>filterContext.ExceptionHandled = true in one of
        //the >>previous methods then the exception will not bubble up to Application_Error.


        // If HandleErrorAttribute is not applied then 
        // Called if throw new Exception("Exception happens on server.");
        // Called if throw new HttpException(500, "Internal Server Error");
        // Not called if return HttpNotFound();
        // Not called if return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        // Not called if return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        // Not called if Response.StatusCode = 500;// Internal Server Error return Content("Hello world");
        protected void Application_Error()
        {


        }


        // If HandleErrorAttribute is not applied then 
        // Called if throw new Exception("Exception happens on server.");
        // Called if throw new HttpException(500, "Internal Server Error");
        // Not called if return HttpNotFound();
        // Not called if return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        // Not called if return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        // Not called if Response.StatusCode = 500;// Internal Server Error return Content("Hello world");

        protected void Application_Error(Object sender, EventArgs e)
        {

        }

        protected void Application_Error1(Object sender, EventArgs e)
        {
            var raisedException = Server.GetLastError();
            var ex = Server.GetLastError();

            // Process exception
            //log the error!
            //_Logger.Error(ex);
        }


        protected void Application_Error2()
        {

            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Errors";
            routeData.Values["action"] = "Internal";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;
            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "Forbidden";
                        break;
                    case 404:
                        routeData.Values["action"] = "NotFound";
                        break;
                }
            }

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;
            IController errorsController = new ErrorController();
            HttpContextWrapper wrapper = new HttpContextWrapper(Context);
            var rc = new RequestContext(wrapper, routeData);
            errorsController.Execute(rc);
        }


        protected void Application_Error3(Object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            if (exception is HttpUnhandledException)
            {
                Server.Transfer("~/Error.aspx");
            }
            if (exception != null)
            {
                Server.Transfer("~/Error.aspx");
            }
            try
            {
                // This is to stop a problem where we were seeing "gibberish" in the
                // chrome and firefox browsers
                HttpApplication app = sender as HttpApplication;
                app.Response.Filter = null;
            }
            catch
            {
            }
        }


        protected void Application_Error6(Object sender, EventArgs e)
        {
            // See http://stackoverflow.com/questions/13905164/how-to-make-custom-error-pages-work-in-asp-net-mvc-4
            // for additional context on use of this technique

            var exception = Server.GetLastError();
            if (exception != null)
            {
                // This would be a good place to log any relevant details about the exception.
                // Since we are going to pass exception information to our error page via querystring,
                // it will only be practical to issue a short message. 
                //Further detail would have to be logged somewhere.

                // This will invoke our error page, passing the exception message via 
                //querystring parameter
                // Note that we chose to use Server.TransferRequest, which is only 
                //supported in IIS 7 and above.
                // As an alternative, Response.Redirect could be used instead.
                // Server.Transfer does not work 
                //(see https://support.microsoft.com/en-us/kb/320439 )
                Server.TransferRequest("~/Error?Message=" + exception.Message);
            }

        }


        //   //////////////////////////////////////////////////

        //log4net.ILog log = log4net.LogManager.GetLogger(typeof(WebApiApplication));

        //public override void Init()
        //{
        // base.Init();
        // this.Error += Application_Error2;
        //}

        //void Application_Error2(object sender, EventArgs e)
        //{
        //var ex = Server.GetLastError();
        //log.Error(ex);
        //}
        //   //////////////////////////////////////////////////

        // Calls at the end of every request
        protected void Application_EndRequest()
        {

        }

        protected void Application_EndRequest1()
        {
            if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "NotFound";

                //IController c = new ErrorsController3();
                //c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
        }


        protected void Application_EndRequest2()
        {
            if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = "AreaName"; // In case controller is in another area
                rd.Values["controller"] = "Errors";
                rd.Values["action"] = "NotFound";

                var rc = new RequestContext(new HttpContextWrapper(Context), rd);
                var c = ControllerBuilder.Current.GetControllerFactory().CreateController(rc, "Errors");
            }
        }


        protected void Application_EndRequest3()
        {
            if (Context.Response.StatusCode == 404)
            {
                var exception = Server.GetLastError();
                var httpException = exception as HttpException;
                Response.Clear();
                Server.ClearError();
                var routeData = new RouteData();
                routeData.Values["controller"] = "ErrorManager";
                routeData.Values["action"] = "Fire404Error";
                routeData.Values["exception"] = exception;
                Response.StatusCode = 500;

                if (httpException != null)
                {
                    Response.StatusCode = httpException.GetHttpCode();
                    switch (Response.StatusCode)
                    {
                        case 404:
                            routeData.Values["action"] = "Fire404Error";
                            break;
                    }
                }
                // Avoid IIS7 getting in the middle
                Response.TrySkipIisCustomErrors = true;
                //IController errormanagerController = new ErrorsController3();
                HttpContextWrapper wrapper = new HttpContextWrapper(Context);
                var rc = new RequestContext(wrapper, routeData);
                //errormanagerController.Execute(rc);
            }
        }




    }
}
