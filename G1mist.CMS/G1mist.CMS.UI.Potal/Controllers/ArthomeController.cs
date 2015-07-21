using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.IRepository;
using G1mist.CMS.Modal;
using HtmlAgilityPack;
using SharpConfig;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class ArthomeController : Controller
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
        public ArthomeController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            var path = System.Web.HttpContext.Current.Server.MapPath("~/config/static.config");
            _templatePath = tempDir;
            Config = Configuration.LoadFromFile(path);
        }
        #endregion

        //
        // GET: /Arthome/

        [HttpGet]
        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT传播教育id=12
            var teachArticles = ArticleService.GetList(a => a.cateid == 12).OrderByDescending(a => a.createtime).Take(2).ToList();

            //3.PUT展示鉴赏id=11
            //获取最新展示鉴赏文章(拿到ID),找到第一张图片的路径
            var listArticles = ArticleService.GetList(a => a.cateid == 11).OrderByDescending(a => a.createtime).Take(3).ToList();
            //获得三张轮播图id=31
            var pics = ArticleService.GetList(a => a.cateid == 31).Take(1).ToList();

            var paths = GetStudentPic(pics);

            velocityHelper.Put("paths", paths);

            var listpath = GetStudentPic(listArticles);
            var teachpath = GetVedioPath(teachArticles);

            //PUT展示鉴赏文章ID和图片路径
            velocityHelper.Put("list", listpath);
            //PUT传播教育文章ID和图片路径
            velocityHelper.Put("teach", teachpath);

            velocityHelper.Display("Arthome.htm");
        }

        [NonAction]
        private List<dynamic> GetStudentPic(IEnumerable<T_Articles> stuNews)
        {
            var list = new List<dynamic>();

            foreach (var stu in stuNews)
            {
                var content = Server.HtmlDecode(stu.body);
                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                foreach (var node in doc.DocumentNode.SelectNodes("//img"))
                {
                    list.Add(new { stu.id, src = node.Attributes["src"].Value, stu.title });
                }
            }

            return list;
        }

        [NonAction]
        private List<dynamic> GetVedioPath(IEnumerable<T_Articles> stuNews)
        {
            var list = new List<dynamic>();

            foreach (var stu in stuNews)
            {
                var content = Server.HtmlDecode(stu.body);
                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                foreach (var node in doc.DocumentNode.SelectNodes("//embed"))
                {
                    list.Add(new { stu.id, src = node.Attributes["src"].Value, stu.title });
                }
            }

            return list;
        }

        [NonAction]
        private string GetVedioPath(string body)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var node = doc.DocumentNode.SelectNodes("//embed")[0];

            return node.Attributes["src"].Value;
        }

        [HttpGet]
        public void List(int id)
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            PutStatic(velocityHelper);
            //参数ID传递出错
            if (id <= 0)
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }
            //分类不存在
            if (!CategoryService.Exits(a => a.id.Equals(id)))
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            var articles = ArticleService.GetList(a => a.cateid.Equals(id)).ToList();
            var cateName = CategoryService.GetModal(a => a.id.Equals(id)).name;

            velocityHelper.Put("active", id);
            velocityHelper.Put("cateName", cateName);
            velocityHelper.Put("articles", articles);
            velocityHelper.Display("arthomelist.htm");
        }

        [HttpGet]
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

            //按照节的名称读取节
            var section = Config["path"];
            var path = GetVedioPath(article.body);
            var site = section["site"].Value;
            var newpath = site + "scripts/ckplayer/ckplayer.swf?f=" + site + path.Substring(1);

            article.body = article.body.Replace(path, newpath);
            article.body = article.body.Replace("loop", "allowfullscreen");

            var cateName = CategoryService.GetModal(a => a.id.Equals(article.cateid)).name;

            velocityHelper.Put("active", article.cateid);
            velocityHelper.Put("cateName", cateName);
            velocityHelper.Put("article", article);
            velocityHelper.Display("arthomedetail.htm");
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
