using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using G1mist.CMS.Repository;
using System.Web.Security;

namespace G1mist.CMS.UnitTest
{
    /// <summary>
    /// TestCRUD 的摘要说明
    /// </summary>
    [TestClass]
    public class TestCRUD
    {
        [TestMethod]
        public void TestMethod1()
        {
            var userRepo = new UserRepo();
            var salt = Common.Security.GetPwdSalt();
            var pwd = FormsAuthentication.HashPasswordForStoringInConfigFile("gaoning" + salt, "MD5");
            var result = userRepo.Insert(new G1mist.CMS.Modal.T_Users
            {
                username = "gaoning",
                password = pwd,
                salt = salt,
                createtime = DateTime.Now
            });

            Assert.AreEqual(true, result);
        }
    }
}
