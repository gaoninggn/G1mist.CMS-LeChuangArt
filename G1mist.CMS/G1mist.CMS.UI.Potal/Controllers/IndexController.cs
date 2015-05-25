using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.Modal;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class IndexController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public IRepository.IRepository<T_Articles> ArticleService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly string _templatePath;

        public IndexController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            _templatePath = tempDir;
        }

        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            velocityHelper.Put("articles", this);
            velocityHelper.Display("index.htm");
        }

        [NonAction]
        public List<T_Articles> GetArticlesByCate(int cateid)
        {
            var list = ArticleService.GetList(a => a.cateid.Equals(cateid)).ToList();

            list.ForEach((a) => { a.body = HttpContext.Server.HtmlDecode(a.body); });

            return list;
        }
    }
}
