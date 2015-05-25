using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using G1mist.CMS.Common;
using G1mist.CMS.Modal;
using Newtonsoft.Json;
using G1mist.CMS.UI.Potal.Models;

namespace G1mist.CMS.UI.Potal.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public IRepository.IRepository<T_Users> UserService { get; set; }

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
            List<T_Users> list;

            if (string.IsNullOrEmpty(search))
            {
                list = UserService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.id > 0, a => a.createtime,
               a => new T_Users
               {
                   id = a.id,
                   type = a.type,
                   lastlogintime = a.lastlogintime,
                   lastloginarea = a.lastloginarea,
                   lastloginip = a.lastloginip,
                   createtime = a.createtime,
                   username = a.username
               }, out total, false).ToList();
            }
            else
            {
                list = UserService.GetListByPage(int.Parse(offset), int.Parse(limit), a => a.username.Contains(search), a => a.createtime,
              a => new T_Users
              {
                  id = a.id,
                  type = a.type,
                  lastlogintime = a.lastlogintime,
                  lastloginarea = a.lastloginarea,
                  lastloginip = a.lastloginip,
                  createtime = a.createtime,
                  username = a.username
              }, out total, false).ToList();
            }

            //根据数据生成json
            var json = GetJson(total, list);

            return Content(json);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(T_Users user)
        {
            var msg = new Message();

            //用户为空
            if (user == null)
            {
                msg.code = 0;
                msg.body = "该用户不存在";
            }
            else if (UserService.GetModal(a => a.username.Equals(User.Identity.Name)).type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                //检查用户名格式: result>0则正确,-1为空,-2为长度过长
                var userNameFormatCheck = CheckUserName(user.username);

                //检查用户名有效性,即是否重复
                var userNameOkCheck = CheckUserNameIsExits(user.username);

                //检查用户类型
                var typeCheck = CheckUserType(user.type);

                //用户名为空
                if (userNameFormatCheck == -1)
                {
                    msg.code = 0;
                    msg.body = "用户名为空";
                }
                //用户名长度不能大于120
                else if (userNameFormatCheck == -2)
                {
                    msg.code = 0;
                    msg.body = "用户名长度不能大于120";
                }
                //用户名是否存在
                else if (userNameOkCheck)
                {
                    msg.code = 0;
                    msg.body = "用户已存在";
                }
                //请选择正确的用户类型
                else if (!typeCheck)
                {
                    msg.code = 0;
                    msg.body = "请选择正确的用户类型";
                }
                //用户名输入正确且已经选择正确的用户类型
                else
                {
                    //执行添加用户操作
                    var result = DoAdd(user);

                    //添加成功
                    if (result)
                    {
                        msg.code = 1;
                        msg.body = "添加成功";
                    }
                    //添加失败
                    else
                    {
                        msg.code = 0;
                        msg.body = "添加失败";
                    }
                }

            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var msg = new Message();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                msg.code = 0;
                msg.body = "用户名或密码为空";
            }
            else
            {
                var bll = UserService;
                var user = bll.GetModal(a => a.username.Equals(username));

                if (user == null)
                {
                    msg.code = 0;
                    msg.body = "用户不存在";
                }
                else
                {
                    var salt = user.salt;
                    var pwd = user.password;
                    var loginPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, "MD5");

                    //比较密码
                    if (pwd.Equals(loginPwd))
                    {
                        //登录成功后设置Cookie
                        FormsAuthentication.SetAuthCookie(user.username, false);

                        user.lastlogintime = DateTime.Now;
                        user.lastloginip = Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress;
                        user.lastloginarea = GetArea(Request.UserHostAddress);

                        bll.Update(user);

                        msg.code = 1;
                        msg.body = "登录成功";
                    }
                    else
                    {
                        msg.code = 0;
                        msg.body = "密码不正确";
                    }
                }
            }
            return Json(msg);
        }

        [NonAction]
        private string GetArea(string userHostAddress)
        {
            var ip = IpHelper.GetAreasByIp(userHostAddress);
            if (ip.retData.country == "本地")
            {
                return "本地";
            }

            return ip.retData.country + "," + ip.retData.province + "," + ip.retData.city + "," + ip.retData.district + "," + ip.retData.carrier;
        }

        [HttpPost]
        public ActionResult Logout()
        {
            var msg = new Message { body = "OK", code = 1 };

            if (User != null)
            {
                FormsAuthentication.SignOut();
            }

            return Json(msg);
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
                var bll = UserService;
                var model = bll.GetModal(a => a.id.Equals(int.Parse(id)));

                if (model == null)
                {
                    msg.code = 0;
                    msg.body = "ID为空";

                }
                else if (model.T_Articles.Count > 0)
                {
                    msg.code = 0;
                    msg.body = "此用户下还有文章,不能删除";
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

        [HttpGet]
        public ActionResult GetUser()
        {
            var username = User.Identity.Name;
            var user = UserService.GetModal(a => a.username.Equals(username));
            var msg = new Message
            {
                code = 1,
                body = new { username, time = user.lastlogintime.ToString(), ip = user.lastloginip, area = user.lastloginarea }
            };
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldpwd"></param>
        /// <param name="newpwd"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePwd(string oldpwd, string newpwd)
        {
            var msg = new Message();

            if (string.IsNullOrEmpty(oldpwd) || string.IsNullOrEmpty(newpwd))
            {
                msg.code = 0;
                msg.body = "请确认信息的完整性";
            }
            else
            {
                //根据用户ID获取用户实体
                var username = User.Identity.Name;
                var user = UserService.GetModal(a => a.username.Equals(username));
                if (user != null)
                {
                    //获取用户加密盐
                    var salt = user.salt;
                    //计算用户输入的旧密码是否正确
                    var isOldPwdCurrect = FormsAuthentication.HashPasswordForStoringInConfigFile(oldpwd + salt, "MD5").Equals(user.password);

                    if (isOldPwdCurrect)
                    {
                        //获取新的加密盐
                        var newsalt = Security.GetPwdSalt();
                        //设置新的加密盐
                        user.salt = newsalt;
                        //计算新的密码
                        user.password = FormsAuthentication.HashPasswordForStoringInConfigFile(newpwd + salt, "MD5");

                        var res = UserService.Update(user);

                        if (res)
                        {
                            msg.code = 1;
                            msg.body = "修改成功,请牢记您的密码";
                        }
                        else
                        {
                            msg.code = 0;
                            msg.body = "修改失败,请联系管理员";
                        }
                    }
                    else
                    {
                        msg.code = 0;
                        msg.body = "旧密码不正确";
                    }
                }
                else
                {
                    msg.code = 0;
                    msg.body = "用户不存在";
                }
            }

            return Json(msg);
        }

        #region NonAction-辅助方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        [NonAction]
        private bool DoAdd(T_Users user)
        {
            //生成6位加密盐
            var salt = Security.GetPwdSalt();

            //初始密码为666666
            //将密码与加密盐混淆,做MD5运算
            var pass = FormsAuthentication.HashPasswordForStoringInConfigFile("666666" + salt, "MD5");

            //创建时间
            user.createtime = DateTime.Now;
            //密码
            user.password = pass;
            //加密盐
            user.salt = salt;
            //默认上次登录时间为2000-1-1
            user.lastlogintime = DateTime.Parse("2000-1-1");

            //执行插入操作
            return UserService.Insert(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [NonAction]
        private bool CheckUserType(int type)
        {
            return type == 0 || type == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [NonAction]
        private int CheckUserName(string username)
        {
            if (string.IsNullOrEmpty(username.Trim()))
            {
                return -1;
            }

            if (username.Length > 120)
            {
                return -2;
            }

            return username.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [NonAction]
        private bool CheckUserNameIsExits(string username)
        {
            return UserService.GetModal(a => a.username.Equals(username)) != null;
        }

        /// <summary>
        /// 根据数据生成JSON字串
        /// </summary>
        /// <param name="total"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [NonAction]
        private static string GetJson(int total, List<T_Users> list)
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var result = new
            {
                total,
                rows = list
            };

            var json = JsonConvert.SerializeObject(result, settings);
            return json;
        }

        #endregion
    }
}
