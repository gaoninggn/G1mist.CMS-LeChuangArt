using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace G1mist.CMS.UI.Potal
{
    public class HttpModules : Controller, IHttpModule
    {
        #region Init and dispose
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.AuthorizeRequest += context_AuthorizeRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Dispose()
        {

        }
        #endregion

        #region 权限检查
        /// <summary>
        /// 在这个事件中验证用户是否得到授权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_AuthorizeRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            //获取访问URL
            var path = context.Request.Path;
            //处理对后台管理页面的访问
            HandleAdminBackStage(path, context);

            //1.获取当前Request Path的ControllerName和ActionName
            //2.获取当前登录的User
            //3.查询数据库,确定当前User所在Role组能否访问该Controller和Action
            //4.如果可以,则通过;如果不可以,返回401码
        }

        /// <summary>
        /// 检查用户是否授权
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckAuth(HttpContext context)
        {
            if (context.User == null)
            {
                return false;
            }

            return context.User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 处理对后台管理页面的访问
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private void HandleAdminBackStage(string path, HttpContext context)
        {
            path = path.ToLower();
            if (path.Contains("admin") && !path.Contains("admin/login") && !path.Contains("admin/user/login"))
            {
                //如果访问的是后台管理系统,则判断是否登录
                var result = CheckAuth(context);

                //如果用户未经授权,则跳转至登录页面
                if (!result)
                {
                    context.Response.Redirect("/areas/admin/login.html");
                    context.Response.End();
                }
            }
        }
        #endregion

        #region 处理请求
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void context_BeginRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            //获取访问URL
            var path = context.Request.Path;
            //获取访问文件的拓展名
            var extention = Path.GetExtension(path);

            if (path.Equals("/index.shtml"))
            {
                HandleIndex(context);
            }
            else
            {
                //根据访问的拓展名来选择处理程序
                switch (extention)
                {
                    //.shtml为前台UI
                    case ".shtml": HandleUiPages(path, context); break;
                    //.htm为前台的模板页
                    case ".htm": HandleAccessTemplates(context); break;
                }
            }
        }

        private void HandleIndex(HttpContext context)
        {
            var urlHelper = new UrlHelper(context.Request.RequestContext);
            var url = UrlHelper.GenerateUrl("", "index", "index", null, urlHelper.RouteCollection, context.Request.RequestContext, false) ?? "";

            ReWriteToPage(context, url);
        }

        /// <summary>
        /// 访问模板页面时的处理
        /// </summary>
        /// <param name="context">HttpContext</param>
        private static void HandleAccessTemplates(HttpContext context)
        {
            //禁止直接访问模板页
            //如果访问的是模板页,则跳转到错误页面
            RedirectToErrorPage(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="context"></param>
        private static void HandleUiPages(string path, HttpContext context)
        {
            //1.当访问的为*.shtml时进入该处理函数.如/news/list/7.shtml
            //2.过滤掉第一个'/',如news/list/7.shtml
            var parameters = path.Substring(1).Split('/');

            //3.使用split('/')将URL分开,如news/list/7.shtml得到[news],[list],[7.shtml]
            if (parameters.Length == 3)
            {
                var url = GenerateUrl(parameters, context);

                //如果生成的URL为空,则为错误的URL,将跳转至错误页
                if (string.IsNullOrEmpty(url))
                {
                    RedirectToErrorPage(context);
                }
                //如生成的URL不为空,则为正确的URL,将跳转至生成的URL
                else
                {
                    ReWriteToPage(context, url);
                }
            }
            //错误的url,将跳转到错误页面
            else
            {
                RedirectToErrorPage(context);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GenerateUrl(IList<string> parameters, HttpContext context)
        {
            var controller = parameters[0].Trim();
            var action = parameters[1].Trim();
            var id = parameters[2].Trim();

            if (string.IsNullOrEmpty(id))
            {
                RedirectToErrorPage(context);
            }
            else
            {
                id = id.Substring(0, id.IndexOf('.'));
            }

            var urlHelper = new UrlHelper(context.Request.RequestContext);

            //url中包含list,为列表页
            if (action.Equals("list"))
            {
                var url = UrlHelper.GenerateUrl("UrlMatch", "list", controller, new RouteValueDictionary { { "id", id } }, urlHelper.RouteCollection, context.Request.RequestContext, false) ?? "";

                return string.IsNullOrEmpty(url) ? "" : url;
            }
            //url中包含show,为详情页
            if (action.Equals("show"))
            {
                var url = UrlHelper.GenerateUrl("UrlMatch", "show", controller, new RouteValueDictionary { { "id", id } }, urlHelper.RouteCollection,
                    context.Request.RequestContext, false) ?? "";

                return string.IsNullOrEmpty(url) ? "" : url;
            }

            //错误的url
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        private static void RedirectToErrorPage(HttpContext context)
        {
            context.Response.Redirect(@"/static/error.html");
            context.Response.End();
        }

        /// <summary>
        /// URL重写
        /// </summary>
        /// <param name="context"></param>
        /// <param name="url"></param>
        private static void ReWriteToPage(HttpContext context, string url)
        {
            context.RewritePath(url);
        }

        #endregion
    }
}