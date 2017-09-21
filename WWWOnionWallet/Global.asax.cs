using OnionWallet.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WWWOnionWallet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");           //Remove Server Header   
            Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            Server.ClearError();

            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            try
            {
                OnionWalletEntities entities = new OnionWalletEntities();
                Log log = new Log();
                log.CreateDate = DateTime.Now;
                log.Level = 3;
                log.Type = 1;
                log.Message = (exception != null) ? exception.Message : "Hmmm!?";
                log.Message = log.Message + Environment.NewLine + Environment.NewLine +
                    Context.Request.RequestContext.RouteData.Values["controller"].ToString() + "/" +
                    Context.Request.RequestContext.RouteData.Values["action"].ToString();

                entities.Logs.Add(log);
            }
            catch (Exception ex)
            {
                //so what
            }
            finally
            {
                if (exception != null)
                {
                    if (((HttpException)exception).GetHttpCode() == 404)
                    {
                        Response.Redirect(url.Action("Error404", "Error"));
                    }
                    else if (((HttpException)exception).GetHttpCode() == 500)
                    {
                        Response.Redirect(url.Action("Error500", "Error"));
                    }
                    else
                    {
                        Response.Redirect(url.Action("Index", "Error"));
                    }
                }
                else
                {
                    Response.Redirect(url.Action("Index", "Error"));
                }
            }

            return;
        }
    }
}
