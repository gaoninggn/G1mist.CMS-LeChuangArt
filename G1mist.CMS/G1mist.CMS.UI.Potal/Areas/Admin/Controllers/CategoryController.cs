using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using G1mist.CMS.Modal;
using G1mist.CMS.UI.Potal.Models;

namespace G1mist.CMS.UI.Potal.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        #region 注入数据库服务


        /// <summary>
        /// 分类表服务
        /// </summary>
        public IRepository.IRepository<T_Categories> CategoryService { get; set; }

        /// <summary>
        /// 用户表服务
        /// </summary>
        public IRepository.IRepository<T_Users> UserService { get; set; }

        /// <summary>
        /// 文章表服务
        /// </summary>
        public IRepository.IRepository<T_Articles> ArticleService { get; set; }
        #endregion

        #region Actions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="order"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get(string limit, string offset, string order, string search)
        {
            int total;

            var list = string.IsNullOrEmpty(search) ? CategoryService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.id > 0, a => a.createtime, a => a, out total, false).ToList() : CategoryService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.name.Contains(search), a => a.createtime, a => a, out total, false).ToList();

            var resultList = list.Select(a => new
            {
                a.id,
                a.name,
                a.createtime,
                parent = GetParentName(a.parentid)
            });

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
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCate()
        {
            var list = CategoryService.GetList(a => a.parentid == -1).Select(a => new
            {
                a.id,
                a.name
            }).ToList();
            list.Insert(0, new { id = -1, name = "请选择一级分类,不选择则为一级分类" });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAllCate()
        {
            var list = CategoryService.GetList(a => a.id > 0).Select(a => new
            {
                a.id,
                a.name,
                a.parentid
            }).ToList();
            list.Insert(0, new { id = -1, name = "-----请选择分类-----", parentid = -1 });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCateNameById(int id)
        {
            var model = CategoryService.GetModal(a => a.id == id);

            if (model == null)
            {
                return Content("");
            }

            return Content(model.name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(T_Categories cate)
        {
            var msg = new Message();
            var currentUser = UserService.GetModal(a => a.username.Equals(User.Identity.Name));

            //用户为空
            if (cate == null)
            {
                msg.code = 0;
                msg.body = "请确认信息的完整性";
            }
            else if (cate.name.Length > 120)
            {
                msg.code = 0;
                msg.body = "分类名的长度不能大于120";
            }
            else if (CheckCateIsExits(cate.name))
            {
                msg.code = 0;
                msg.body = "该分类已存在";
            }
            else if (cate.id.Equals(cate.parentid))
            {
                msg.code = 0;
                msg.body = "上级分类不能是自己";
            }
            else if (currentUser.type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                var bll = CategoryService;

                cate.createtime = DateTime.Now;
                cate.uid = currentUser.id;

                var result = bll.Insert(cate);

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var msg = new Message();

            if (id == 0)
            {
                msg.code = 0;
                msg.body = "非法的ID";
            }
            else if (UserService.GetModal(a => a.username.Equals(User.Identity.Name)).type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                var bll = CategoryService;
                var model = bll.GetModal(a => a.id.Equals(id));
                if (model == null)
                {
                    msg.code = 0;
                    msg.body = "分类不存在ID";
                }
                else
                {
                    var isExitsSubCate = bll.GetModal(a => a.parentid.Equals(id)) != null;
                    var isExitsArticle = ArticleService.GetModal(a => a.cateid.Equals(id)) != null;

                    if (isExitsSubCate)
                    {
                        msg.code = 0;
                        msg.body = "该分类存在子分类,不能删除";
                    }
                    else if (isExitsArticle)
                    {
                        msg.code = 0;
                        msg.body = "该分类下还存在文章,不能删除";
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
                            msg.body = "删除失败";
                        }
                    }
                }
            }

            return Json(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, string name, int parentid)
        {
            var msg = new Message();

            if (id <= 0 || string.IsNullOrEmpty(name))
            {
                msg.code = 0;
                msg.body = "请检查信息的完整性";
            }
            else if (parentid <= 0 && parentid != -1)
            {
                msg.code = 0;
                msg.body = "上级分类选择出错";
            }
            else if (id.Equals(parentid))
            {
                msg.code = 0;
                msg.body = "上级分类不能是自己";
            }
            else if (UserService.GetModal(a => a.username.Equals(User.Identity.Name)).type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                var model = CategoryService.GetModal(a => a.id.Equals(id));

                if (model == null)
                {
                    msg.code = 0;
                    msg.body = "分类不存在";
                }
                else
                {
                    model.name = name;
                    model.parentid = parentid;

                    var result = CategoryService.Update(model);

                    if (result)
                    {
                        msg.code = 1;
                        msg.body = "修改成功";

                    }
                    else
                    {
                        msg.code = 0;
                        msg.body = "修改失败,请联系管理员";
                    }
                }
            }
            return Json(msg);
        }
        #endregion

        #region NonAction-辅助方法


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [NonAction]
        private string GetParentName(int pid)
        {
            var model = CategoryService.GetModal(a => a.id.Equals(pid));

            return model == null ? "" : model.name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        [NonAction]
        private bool CheckCateIsExits(string cate)
        {
            return CategoryService.GetModal(a => a.name.Equals(cate)) != null;
        }
        #endregion
    }
}
