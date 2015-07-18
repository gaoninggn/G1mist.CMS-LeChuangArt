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
    public class ArtController : Controller
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
        public ArtController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            var path = System.Web.HttpContext.Current.Server.MapPath("~/config/static.config");
            _templatePath = tempDir;
            Config = Configuration.LoadFromFile(path);
        }
        #endregion
        //
        // GET: /Art/

        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT学校概况id=7
            var lechuangNews = ArticleService.GetList(a => a.cateid == 7).Take(5).ToList();
            //3.PUT学科课程id=8
            var artNews = ArticleService.GetList(a => a.cateid == 8).Take(14).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);

            velocityHelper.Display("Art.htm");
        }

        public void Detail(int id)
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            PutStatic(velocityHelper);

            if (id <= 0)
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            if (!ArticleService.Exits(a => a.id.Equals(id)))
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            var article = ArticleService.GetModal(a => a.id.Equals(id));
            article.body = Server.HtmlDecode(article.body);

            velocityHelper.Put("article", article);
            velocityHelper.Display("newsdetail.htm");
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
