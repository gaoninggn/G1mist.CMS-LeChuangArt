using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.Modal;
using Newtonsoft.Json;
using Microsoft.Security.Application;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class ArticlesController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public IRepository.IRepository<T_Articles> ArticleService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly string _templatePath;

        public ArticlesController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            _templatePath = tempDir;
        }

        public void List(int id)
        {
            var list = ArticleService.GetList(a => a.cateid.Equals(id)).ToList();

            var velocityHelper = new VelocityHelper(_templatePath);

            velocityHelper.Put("list", list);
            velocityHelper.Display("list.htm");
        }

        public void Show(int id)
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            var model = ArticleService.GetModal(a => a.id.Equals(id));

            if (model != null)
            {
                model.body = HttpContext.Server.HtmlDecode(model.body);
                velocityHelper.Put("model", model);
                velocityHelper.Display("show.htm");
            }
            else
            {
                HttpContext.Response.Redirect(@"/static/error.html");
                HttpContext.Response.End();
            }
        }
    }
}
