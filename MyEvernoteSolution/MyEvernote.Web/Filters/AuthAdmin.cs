using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyEvernote.Web.Models;

namespace MyEvernote.Web.Filters
{
    public class AuthAdmin:FilterAttribute,IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentSession.User!=null&&CurrentSession.User.IsAdmin==false)
            {//kişi admin değilse
                filterContext.Result=new RedirectResult("/Home/AccessDenied");
            }
        }
    }
}