using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using G1mist.CMS.Common;
using G1mist.CMS.IRepository;
using G1mist.CMS.Modal;
using HtmlAgilityPack;
using SharpConfig;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class IndexController : Controller
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
        public IndexController()
        {
            var tempDir = System.Web.HttpContext.Current.Server.MapPath("~/templates/");
            var path = System.Web.HttpContext.Current.Server.MapPath("~/config/static.config");
            _templatePath = tempDir;
            Config = Configuration.LoadFromFile(path);
        }
        #endregion

        #region Actions
        [HttpGet]
        public void Index()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            // 获取首页三张轮播图
            // 获取3张轮播图 id =35
            var pics = ArticleService.GetList(a => a.cateid == 35).Take(1).ToList();
            var paths = GetPic(pics);

            velocityHelper.Put("paths", paths);
            velocityHelper.Put("this", this);
            velocityHelper.Display("index.htm");
        }
        [NonAction]
        private List<dynamic> GetPic(IEnumerable<T_Articles> stuNews)
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

        public string GetCategoryString(int index)
        {
            var list = new List<string>
            {
                "news","art","arthome","gongyi","wenhua"
            };

            return list[index];
        }

        [HttpGet]
        public void News()
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            //1.PUT前台相关路径
            PutStatic(velocityHelper);
            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(5).ToList();
            //3.PUT艺术动态id=3
            var artNews = ArticleService.GetList(a => a.cateid == 3).Take(14).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);

            velocityHelper.Display("news.htm");
        }
        [HttpGet]
        public void Art()
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
        [HttpGet]
        public void Arthome()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(5).ToList();
            //3.PUT艺术动态id=3
            var artNews = ArticleService.GetList(a => a.cateid == 3).Take(14).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);

            velocityHelper.Display("Arthome.htm");
        }
        [HttpGet]
        public void Gongyi()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(14).ToList();
            //3.PUT仁德慈善id=16
            var artNews = ArticleService.GetList(a => a.cateid == 16).Take(5).ToList();
            //3.PUT公益资讯id=17
            var gongyiNews = ArticleService.GetList(a => a.cateid == 17).Take(5).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);
            velocityHelper.Put("gongyiNews", gongyiNews);

            velocityHelper.Display("Gongyi.htm");
        }
        [HttpGet]
        public void Wenhua()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(14).ToList();
            //3.PUT公司简介id=19
            var company = ArticleService.GetList(a => a.cateid == 19).Take(5).ToList();
            //3.PUT招商合作id=22
            var cooperation = ArticleService.GetList(a => a.cateid == 22).Take(5).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("company", company);
            velocityHelper.Put("cooperation", cooperation);

            velocityHelper.Display("Wenhua.htm");
        }
        [HttpGet]
        public void Zanzhu()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(14).ToList();
            //3.PUT公司简介id=19
            var artNews = ArticleService.GetList(a => a.cateid == 19).Take(14).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);

            velocityHelper.Display("Zanzhu.htm");
        }
        [HttpGet]
        public void Contactus()
        {
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT乐闯资讯id=4
            var lechuangNews = ArticleService.GetList(a => a.cateid == 4).Take(5).ToList();
            //3.PUT艺术动态id=3
            var artNews = ArticleService.GetList(a => a.cateid == 3).Take(14).ToList();

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);

            velocityHelper.Display("Contactus.htm");
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

            velocityHelper.Put("article", article);
            velocityHelper.Display("newsdetail.htm");
        }
        [HttpGet]
        public void List(int cateId)
        {
            var velocityHelper = new VelocityHelper(_templatePath);
            PutStatic(velocityHelper);

            if (cateId <= 0)
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            if (!CategoryService.Exits(a => a.id.Equals(cateId)))
            {
                Response.Redirect(@"/static/error.html");
                Response.End();
            }

            var list = CategoryService.GetList(a => a.id.Equals(cateId));

            velocityHelper.Put("list", list);
            velocityHelper.Display("newslist.htm");
        }

        #endregion

        #region NonAction-辅助方法
        [NonAction]
        public List<T_Articles> GetArticlesByCate(int cateid)
        {
            var list = ArticleService.GetList(a => a.cateid.Equals(cateid)).ToList();

            list.ForEach(a => { a.body = HttpContext.Server.HtmlDecode(a.body); });

            return list;
        }

        /// <summary>
        /// 向前台Put JS CSS IMAGES SITE等路径
        /// </summary>
        /// <param name="velocityHelper"></param>
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
        #endregion
    }
}
