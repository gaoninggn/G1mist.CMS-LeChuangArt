using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.IRepository;
using G1mist.CMS.Modal;
using SharpConfig;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class ContactusController : Controller
    {
        #region 数据库服务及模版文件路径初始化
        /// <summary>
        /// 
        /// </summary>
        public IRepository<T_Articles> ArticleService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IRepository<T_Categories> CategoryService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly string _templatePath;

        /// <summary>
        /// 
        /// </summary>
        private Configuration Config { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ContactusController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            var path = System.Web.HttpContext.Current.Server.MapPath("~/config/static.config");
            _templatePath = tempDir;
            Config = Configuration.LoadFromFile(path);
        }
        #endregion

        //
        // GET: /Contactus/
        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            var article = ArticleService.GetModal(a => a.cateid == 36);
            if (article != null)
            {
                article.body = Server.HtmlDecode(article.body);
            }

            PutStatic(velocityHelper);

            velocityHelper.Put("article", article);
            velocityHelper.Display("lechuangxuexiao.htm");
        }
        public void Meishuguan()
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            var article = ArticleService.GetModal(a => a.cateid == 37);
            if (article != null)
            {
                article.body = Server.HtmlDecode(article.body);
            }

            PutStatic(velocityHelper);

            velocityHelper.Put("article", article);

            velocityHelper.Display("lechuangmeishuguan.htm");
        }
        public void Gongyi()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            var article = ArticleService.GetModal(a => a.cateid == 38);
            if (article != null)
            {
                article.body = Server.HtmlDecode(article.body);
            }

            PutStatic(velocityHelper);

            velocityHelper.Put("article", article);

            velocityHelper.Display("lechuanggongyi.htm");
        }
        public void Wenhua()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            var article = ArticleService.GetModal(a => a.cateid == 39);
            if (article != null)
            {
                article.body = Server.HtmlDecode(article.body);
            }

            PutStatic(velocityHelper);

            velocityHelper.Put("article", article);

            velocityHelper.Display("lechuangwenhua.htm");
        }

        [NonAction]
        private void PutStatic(VelocityHelper velocityHelper)
        {
            //按照节的名称读取节
            var section = Config["path"];

            velocityHelper.Put("css", section["css"].Value);
            velocityHelper.Put("js", section["js"].Value);
            velocityHelper.Put("images", section["images"].Value);
            velocityHelper.Put("site", section["site"].Value);
        }
    }
}
