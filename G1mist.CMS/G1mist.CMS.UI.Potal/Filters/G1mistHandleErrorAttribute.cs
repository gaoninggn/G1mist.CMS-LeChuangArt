using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;

namespace G1mist.CMS.UI.Potal.Filters
{
    public class G1MistHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //使用log4net或其他记录错误消息
            var error = filterContext.Exception;
            var message = error.Message;//错误信息
            var url = HttpContext.Current.Request.RawUrl;//错误发生地址

            LogHelper.Error(url + ":" + message);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("/static/error500.html");

            base.OnException(filterContext);
        }
    }
}