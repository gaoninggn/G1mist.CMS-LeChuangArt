using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace G1mist.CMS.UI.Potal.Filters
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var path = HttpContext.Current.Server.MapPath("/log/log.log");
            using (var fs = new FileStream(path, FileMode.Append))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(filterContext.Exception.Message);
                    if (filterContext.Exception.InnerException != null)
                    {
                        sw.WriteLine(filterContext.Exception.InnerException);
                    }
                }
            }
            filterContext.HttpContext.Response.Redirect("/static/error.html");
            filterContext.HttpContext.Response.End();
        }
    }
}