using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Web.Filters
{
    public class Exc:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            filterContext.Controller.TempData["LastError"] = filterContext.Exception;//hatayı tempdataya set ettik

            filterContext.ExceptionHandled = true;//hatayı biz yöneteceğiz o yüzden true
            filterContext.Result=new RedirectResult("/Home/HasError");
        }
    }
}