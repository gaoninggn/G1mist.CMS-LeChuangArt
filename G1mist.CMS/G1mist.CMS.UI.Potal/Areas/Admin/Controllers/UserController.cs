using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using G1mist.CMS.Common;
using G1mist.CMS.Modal;
using Newtonsoft.Json;
using G1mist.CMS.UI.Potal.Models;
using GeetestSDK;
using Microsoft.VisualBasic;
using G1mist.CMS.UI.Potal.Filters;

namespace G1mist.CMS.UI.Potal.Areas.Admin.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [ExceptionFilter]
    public class UserController : Controller
    {
        #region 注入数据库服务

        /// <summary>
        /// 用户表服务
        /// </summary>
        public IRepository.IRepository<T_Users> UserService { get; set; }
        #endregion

        #region Actions



        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="limit">取N条</param>
        /// <param name="offset">从第N条开始</param>
        /// <param name="order">排序条件</param>
        /// <param name="search">过滤条件</param>
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
        ///  新增用户
        /// </summary>
        /// <param name="user">用户实体</param>
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
            //判断用户的权限
            else if (UserService.GetModal(a => a.username.Equals(User.Identity.Name)).type == 0)
            {
                msg.code = 0;
                msg.body = "您的权限不足";
            }
            else
            {
                //检查用户名格式: result>0则正确,-1为空,-2为长度过长
                var userNameFormatCheck = CheckUserNameLength(user.username);

                //检查用户名有效性,即是否重复
                var userNameOkCheck = CheckUserNameIsExits(user.username);

                //检查用户类型
                var typeCheck = CheckUserType(user.type);

                //用户名为空
                switch (userNameFormatCheck)
                {
                    case -1:
                        msg.code = 0;
                        msg.body = "用户名为空";
                        break;
                    case -2:
                        msg.code = 0;
                        msg.body = "用户名长度不能大于120";
                        break;
                    default:
                        if (userNameOkCheck)
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
                        break;
                }

            }
            return Json(msg);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var msg = new Message();

            //获取前端滑动验证的三个数据
            var geetestChallenge = Request["validate[geetest_challenge]"] ?? "";
            var geetestValidate = Request["validate[geetest_validate]"] ?? "";
            var geetestSeccode = Request["validate[geetest_seccode]"] ?? "";

            //滑动验证的后端校验
            var result = BackValidate(geetestChallenge, geetestValidate, geetestSeccode);

            if (!result)
            {
                msg.code = 0;
                msg.body = "滑动验证失败,请重试";
            }
            else if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                msg.code = 0;
                msg.body = "用户名或密码为空";
            }
            else
            {
                var decryptedName = DecryptStr(username);
                var decryptedPwd = DecryptStr(password);

                var bll = UserService;
                var user = bll.GetModal(a => a.username.Equals(decryptedName));

                if (user == null)
                {
                    msg.code = 0;
                    msg.body = "用户不存在";
                }
                else
                {
                    var salt = user.salt;
                    var pwd = user.password;
                    var loginPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(decryptedPwd + salt, "MD5");

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
                        LogHelper.Info("用户" + user.username + "登录成功");
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

        /// <summary>
        /// 通过用户IP获取用户地址
        /// </summary>
        /// <param name="userHostAddress">IP地址</param>
        /// <returns></returns>
        [NonAction]
        private string GetArea(string userHostAddress)
        {
            var ip = IpHelper.GetAreasByIp(userHostAddress);
            if (ip.RetData.Country == "本地")
            {
                return "本地";
            }

            return ip.RetData.Country + "," + ip.RetData.Province + "," + ip.RetData.City + "," + ip.RetData.District + "," + ip.RetData.Carrier;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Logout()
        {
            var msg = new Message { body = "OK", code = 1 };

            if (User != null)
            {
                FormsAuthentication.SignOut();
                LogHelper.Info("用户" + User.Identity.Name + "登出");
            }

            return Json(msg);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
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
            else if (!Information.IsNumeric(id))
            {
                msg.code = 0;
                msg.body = "ID应为数字";
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
                else if (GetCurrentUserId().Equals(int.Parse(id)))
                {
                    msg.code = 0;
                    msg.body = "不能删除自己";
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                    var hashPasswordForStoringInConfigFile = FormsAuthentication.HashPasswordForStoringInConfigFile(oldpwd + salt, "MD5");
                    var isOldPwdCurrect = hashPasswordForStoringInConfigFile != null && hashPasswordForStoringInConfigFile.Equals(user.password);

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

        #endregion

        #region NonAction-辅助方法
        /// <summary>
        /// 通过当前登录用户的用户名获取当前登录用户的ID
        /// </summary>
        /// <returns></returns>
        private int GetCurrentUserId()
        {
            var username = User.Identity.Name;
            var user = UserService.GetModal(a => a.username.Equals(username));

            return user.id;
        }

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
        private int CheckUserNameLength(string username)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geetestChallenge"></param>
        /// <param name="geetestValidate"></param>
        /// <param name="geetestSeccode"></param>
        /// <returns></returns>
        [NonAction]
        private static bool BackValidate(string geetestChallenge, string geetestValidate, string geetestSeccode)
        {
            if (!string.IsNullOrEmpty(geetestChallenge) && !string.IsNullOrEmpty(geetestValidate) &&
                !string.IsNullOrEmpty(geetestSeccode))
            {
                const string privateKey = "91f6e4142cf97fa261c8decc1be5c2fd";
                var geetest = new GeetestLib(privateKey);

                var result = geetest.validate(geetestChallenge, geetestValidate, geetestSeccode);

                return result;
            }
            return false;
        }

        /// <summary>
        /// 对密文进行解密
        /// </summary>
        /// <param name="encryptedStr"></param>
        /// <returns></returns>
        private string DecryptStr(string encryptedStr)
        {
            var path = Server.MapPath("~/config/keys.config");
            var privateKey = new XmlHelper(path).GetValue("privateKey").Trim();

            var rsa = new RsaCryptoService(privateKey);
            var decryptStr = rsa.Decrypt(encryptedStr);
            return decryptStr;
        }

        #endregion
    }
}
