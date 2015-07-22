using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.IRepository;
using G1mist.CMS.Modal;
using SharpConfig;
using HtmlAgilityPack;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class ZanzhuController : Controller
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
        public ZanzhuController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            var path = System.Web.HttpContext.Current.Server.MapPath("~/config/static.config");
            _templatePath = tempDir;
            Config = Configuration.LoadFromFile(path);
        }
        #endregion

        //
        // GET: /Zanzhu/
        [HttpGet]
        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            //1.PUT前台相关路径
            PutStatic(velocityHelper);
            //2.PUT捐赠学校id=24
            var juanzengxuexiao = ArticleService.GetList(a => a.cateid == 24).Take(5).ToList();
            //3.PUT赞助美术馆id=25
            var zanzhuart = ArticleService.GetList(a => a.cateid == 25).Take(14).ToList();
            //3.PUT赞助教育id=26
            var zanzhuschool = ArticleService.GetList(a => a.cateid == 26).Take(14).ToList();

            //获取3张轮播图
            var pic = ArticleService.GetList(a => a.cateid == 34).Take(1).ToList();
            var paths = GetStudentPic(pic);

            velocityHelper.Put("juanzengxuexiao", juanzengxuexiao);
            velocityHelper.Put("zanzhuart", zanzhuart);
            velocityHelper.Put("zanzhuschool", zanzhuschool);
            velocityHelper.Put("paths", paths);

            velocityHelper.Display("zanzhu.htm");
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
            //文章不存在
            if (!CategoryService.Exits(a => a.id.Equals(id)))
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            var article = ArticleService.GetList(a => a.cateid.Equals(id));
            var cateName = CategoryService.GetModal(a => a.id.Equals(id)).name;

            velocityHelper.Put("active", id);
            velocityHelper.Put("cateName", cateName);
            velocityHelper.Put("article", article);
            velocityHelper.Display("zanzhulist.htm");
        }

        [HttpGet]
        public void Detail(int id)
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            PutStatic(velocityHelper);
            //参数ID传递出错
            if (id <= 0)
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }
            //文章不存在
            if (!ArticleService.Exits(a => a.id.Equals(id)))
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            var article = ArticleService.GetModal(a => a.id.Equals(id));
            article.body = Server.HtmlDecode(article.body);
            var cateName = CategoryService.GetModal(a => a.id.Equals(article.cateid)).name;

            //按照节的名称读取节
            var section = Config["path"];
            var path = GetVedioPath(article.body);
            var site = section["site"].Value;
            if (!string.IsNullOrEmpty(path))
            {
                var newpath = site + "scripts/ckplayer/ckplayer.swf?f=" + site + path.Substring(1);

                article.body = article.body.Replace(path, newpath);
                article.body = article.body.Replace("loop", "allowfullscreen");
            }

            velocityHelper.Put("active", article.cateid);
            velocityHelper.Put("cateName", cateName);
            velocityHelper.Put("article", article);
            velocityHelper.Display("zanzhudetail.htm");
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

        [NonAction]
        private List<dynamic> GetStudentPic(IEnumerable<T_Articles> stuNews)
        {
            var list = new List<dynamic>();

            foreach (var stu in stuNews)
            {
                var content = Server.HtmlDecode(stu.body);
                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                if (doc.DocumentNode.SelectNodes("//img").Count > 0)
                {
                    foreach (var node in doc.DocumentNode.SelectNodes("//img"))
                    {
                        list.Add(new { stu.id, src = node.Attributes["src"].Value, stu.title });
                    }
                }
            }

            return list;
        }

        [NonAction]
        private string GetVedioPath(string body)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            if (doc.DocumentNode.SelectNodes("//embed") != null && doc.DocumentNode.SelectNodes("//embed").Count > 0)
            {
                var node = doc.DocumentNode.SelectNodes("//embed")[0];
                return node.Attributes["src"].Value;
            }
            return "";
        }
    }
}
