using System.Web.Mvc;
using G1mist.CMS.Common;

namespace G1mist.CMS.UI.Potal.Filters
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            LogHelper.Error(filterContext.Exception.Message);

            if (filterContext.Exception.InnerException != null)
            {
                LogHelper.Error(filterContext.Exception.InnerException.Message);
            }

            filterContext.HttpContext.Response.Redirect("/static/error.html");
            filterContext.HttpContext.Response.End();
        }
    }
}