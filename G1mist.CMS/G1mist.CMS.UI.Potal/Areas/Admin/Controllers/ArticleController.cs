using System;
using System.Linq;
using System.Web.Mvc;
using G1mist.CMS.Modal;
using G1mist.CMS.UI.Potal.Models;
using Microsoft.Security.Application;
using Newtonsoft.Json;

namespace G1mist.CMS.UI.Potal.Areas.Admin.Controllers
{
    public class ArticleController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public IRepository.IRepository<T_Articles> ArticleService { get; set; }

        public IRepository.IRepository<T_Categories> CategoryService { get; set; }

        public IRepository.IRepository<T_Users> UserService { get; set; }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(T_Articles article)
        {
            var msg = new Message();
            var currentUser = UserService.GetModal(a => a.username.Equals(User.Identity.Name));

            if (article == null || string.IsNullOrEmpty(article.title) || article.cateid <= 0 || string.IsNullOrEmpty(article.body))
            {
                msg.code = 0;
                msg.body = "请确认信息的完整性";
            }
            else if (currentUser.type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                article.title = Encoder.HtmlEncode(article.title);
                article.body = Encoder.HtmlEncode(article.body);
                article.from = Encoder.HtmlEncode(article.from);
                article.author = Encoder.HtmlEncode(article.author);
                article.createtime = DateTime.Now;
                article.uid = currentUser.id;

                var result = ArticleService.Insert(article);

                if (result)
                {

                    msg.code = 1;
                    msg.body = "添加成功";
                }
                else
                {

                    msg.code = 0;
                    msg.body = "添加失败,请联系管理员";
                }
            }

            return Json(msg);
        }

        [HttpGet]
        public ActionResult Get(string limit, string offset, string order, string search)
        {
            int total;

            var list = string.IsNullOrEmpty(search) ? ArticleService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.id > 0, a => a.createtime, a => a, out total, false).ToList() : ArticleService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.title.Contains(search), a => a.createtime, a => a, out total, false).ToList();

            var resultList = list.Select(a => a.cateid != null ? new
            {
                a.id,
                a.title,
                a.createtime,
                a.@from,
                a.author,
                cate = GetCateNameById((int)a.cateid)
            } : null);

            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var result = new
            {
                total,
                rows = resultList
            };

            var json = JsonConvert.SerializeObject(result, settings);

            return Content(json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(T_Articles article)
        {
            var msg = new Message();
            var currentUser = UserService.GetModal(a => a.username.Equals(User.Identity.Name));

            if (article == null || string.IsNullOrEmpty(article.title) || article.cateid <= 0 || string.IsNullOrEmpty(article.body))
            {
                msg.code = 0;
                msg.body = "请确认信息的完整性";
            }
            else if (currentUser.type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                article.title = Encoder.HtmlEncode(article.title);
                article.body = Encoder.HtmlEncode(article.body);
                article.from = Encoder.HtmlEncode(article.from);
                article.author = Encoder.HtmlEncode(article.author);
                article.createtime = DateTime.Now;
                article.uid = currentUser.id;

                var result = ArticleService.Update(article);

                if (result)
                {
                    msg.code = 1;
                    msg.body = "编辑成功";
                }
                else
                {

                    msg.code = 0;
                    msg.body = "编辑失败,请联系管理员";
                }
            }

            return Json(msg);
        }

        [HttpGet]
        public ActionResult GetArticleById(int id)
        {
            var article = ArticleService.GetModal(a => a.id.Equals(id));

            if (article == null)
            {
                return Content("null");
            }

            var jsonObj = new
            {
                article.id,
                title = Server.HtmlDecode(article.title),
                from = Server.HtmlDecode(article.@from),
                author = Server.HtmlDecode(article.author),
                body = Server.HtmlDecode(article.body)
            };

            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetBodyById(int id)
        {
            var article = ArticleService.GetModal(a => a.id.Equals(id));

            if (article == null)
            {
                return Content("null");
            }

            var jsonObj = new
            {
                body = Server.HtmlDecode(article.body)
            };

            return Json(jsonObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var msg = new Message();

            if (string.IsNullOrEmpty(id))
            {
                msg.code = 0;
                msg.body = "ID为空";
            }
            else if (UserService.GetModal(a => a.username.Equals(User.Identity.Name)).type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                var bll = ArticleService;
                var model = bll.GetModal(a => a.id.Equals(int.Parse(id)));

                if (model == null)
                {
                    msg.code = 0;
                    msg.body = "文章不存在";
                }
                else
                {
                    var result = bll.Delete(model);
                    if (result)
                    {
                        msg.code = 1;
                        msg.body = "删除成功";
                    }
                    else
                    {
                        msg.code = 0;
                        msg.body = "删除失败,请联系管理员";
                    }
                }
            }
            return Json(msg);
        }

        [NonAction]
        private string GetCateNameById(int id)
        {
            var model = CategoryService.GetModal(a => a.id.Equals(id));

            return model == null ? "" : model.name;
        }
    }
}
