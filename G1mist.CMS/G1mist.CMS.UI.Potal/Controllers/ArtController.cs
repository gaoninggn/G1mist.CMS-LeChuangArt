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
        [HttpGet]
        public void Index()
        {
            int stuId;
            var velocityHelper = new VelocityHelper(_templatePath);

            PutStatic(velocityHelper);

            //2.PUT学校概况id=7
            var lechuangNews = ArticleService.GetList(a => a.cateid == 7).Take(5).ToList();
            //3.PUT学科课程id=8
            var artNews = ArticleService.GetList(a => a.cateid == 8).Take(14).ToList();
            //4.PUT学生风采id=9
            //获取最新学生风采文章(拿到ID),找到第一张图片的路径
            var stuNews = ArticleService.GetList(a => a.cateid == 9).OrderByDescending(a => a.createtime).ToList();
            var path = GetStudentPic(stuNews).FirstOrDefault();

            // 获取三张轮播图
            // 获取3张轮播图 id =29
            var pics = ArticleService.GetList(a => a.cateid == 30).Take(1).ToList();
            var paths = GetStudentPic(pics);

            velocityHelper.Put("lechuangNews", lechuangNews);
            velocityHelper.Put("artNews", artNews);
            //PUT学生风采文章ID和第一张图片路径
            velocityHelper.Put("path", path);
            //轮播图
            velocityHelper.Put("paths", paths);
            velocityHelper.Display("Art.htm");
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
            velocityHelper.Display("artlist.htm");
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
            var cateName = CategoryService.GetModal(a => a.id.Equals(article.cateid)).name;

            velocityHelper.Put("active", article.cateid);
            velocityHelper.Put("cateName", cateName);
            velocityHelper.Put("article", article);
            velocityHelper.Display("artdetail.htm");
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
