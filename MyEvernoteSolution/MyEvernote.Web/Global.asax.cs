using System.Data.Entity;
using MyEvernote.Common;
using MyEvernote.Web.Initialize;
using System.Web.Mvc;
using System.Web.Routing;
using MyEvernote.DataAccessLayer.EntityFramework;

namespace MyEvernote.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            App.Common = new WebCommon();
        }
    }
}
