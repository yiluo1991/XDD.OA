using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using XDD.Core.DataAccess;
using System.Data.Entity;

namespace XDD.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer(new XDDDataInit());
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {

            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null || authCookie.Value == "")
            {
                return;
            }
            FormsAuthenticationTicket authTicket = null;
            try
            {

                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                string[] roles = authTicket.UserData.Split(',');
      
                FormsIdentity id = new FormsIdentity(authTicket);
                Context.User = new System.Security.Principal.GenericPrincipal(id, roles);
                
            }
            catch
            {
                return;
            }


        }
    }
}