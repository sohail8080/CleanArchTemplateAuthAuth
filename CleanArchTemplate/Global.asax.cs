using CleanArchTemplate.AccessControl.Controllers;
using CleanArchTemplate.AccessControl.Controllers.ErrorControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CleanArchTemplate
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private static readonly Logger _log = LogManager.GetCurrentClassLogger();

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


            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            //AuthConfig.RegisterAuth();
            //// Calling Global action filter
            //GlobalFilters.Filters.Add(new MyExceptionHandler());

            // To handle exceptions
            //GlobalFilters.Filters.Add(new CustomExceptionFilter());///////////

            //_log.Info("Application started");

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


        protected void Application_Error45333(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                StringBuilder err = new StringBuilder();
                err.Append("Error caught in Application_Error event\n");
                err.Append("Error in: " + (Context.Session == null ? string.Empty : Request.Url.ToString()));
                err.Append("\nError Message:" + ex.Message);
                if (null != ex.InnerException)
                    err.Append("\nInner Error Message:" + ex.InnerException.Message);
                err.Append("\n\nStack Trace:" + ex.StackTrace);
                Server.ClearError();

                if (null != Context.Session)
                {
                    err.Append($"Session: Identity name:[{Thread.CurrentPrincipal.Identity.Name}] IsAuthenticated:{Thread.CurrentPrincipal.Identity.IsAuthenticated}");
                }
                //_log.Error(err.ToString());

                if (null != Context.Session)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "ErrorPage");
                    routeData.Values.Add("action", "Error");
                    routeData.Values.Add("exception", ex);

                    if (ex.GetType() == typeof(HttpException))
                    {
                        routeData.Values.Add("statusCode", ((HttpException)ex).GetHttpCode());
                    }
                    else
                    {
                        routeData.Values.Add("statusCode", 500);
                    }
                    Response.TrySkipIisCustomErrors = true;
                    IController controller = new ErrorPageController();
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();
                }
            }

        }

        protected void Application_Error8887(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                StringBuilder err = new StringBuilder();
                err.Append("Error caught in Application_Error event\n");
                err.Append("Error in: " + (Context.Session == null ? string.Empty : Request.Url.ToString()));
                err.Append("\nError Message:" + ex.Message);
                if (null != ex.InnerException)
                    err.Append("\nInner Error Message:" + ex.InnerException.Message);
                err.Append("\n\nStack Trace:" + ex.StackTrace);
                Server.ClearError();

                if (null != Context.Session)
                {
                    err.Append($"Session: Identity name:[{Thread.CurrentPrincipal.Identity.Name}] IsAuthenticated:{Thread.CurrentPrincipal.Identity.IsAuthenticated}");
                }
                //_log.Error(err.ToString());

                if (null != Context.Session)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "ErrorPage");
                    routeData.Values.Add("action", "Error");
                    routeData.Values.Add("exception", ex);

                    if (ex.GetType() == typeof(HttpException))
                    {
                        routeData.Values.Add("statusCode", ((HttpException)ex).GetHttpCode());
                    }
                    else
                    {
                        routeData.Values.Add("statusCode", 500);
                    }
                    Response.TrySkipIisCustomErrors = true;
                    IController controller = new ErrorPageController();
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();
                }
            }
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


        protected void Application_Error88()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            // Do logging here
        }


        protected void Application_Error66()
        {

            if (Context.IsCustomErrorEnabled)
                ShowCustomErrorPage(Server.GetLastError());

        }
        private void ShowCustomErrorPage(Exception exception)
        {
            var httpException = exception as HttpException ?? new HttpException(500, "Internal Server Error", exception);

            Response.Clear();
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            switch (httpException.GetHttpCode())
            {
                case 403:
                    routeData.Values.Add("action", "HttpError403");
                    break;

                case 404:
                    routeData.Values.Add("action", "HttpError404");
                    break;

                case 500:
                    routeData.Values.Add("action", "HttpError500");
                    break;

                default:
                    routeData.Values.Add("action", "GeneralError");
                    routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
                    break;
            }

            Server.ClearError();

            IController controller = new ErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        public void Application_Error555(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "ErrorPage");
            routeData.Values.Add("action", "Error");
            routeData.Values.Add("exception", exception);

            if (exception.GetType() == typeof(HttpException))
            {
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else
            {
                routeData.Values.Add("statusCode", 500);
            }

            Response.TrySkipIisCustomErrors = true;
            IController controller = new ErrorPageController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.End();
        }

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

