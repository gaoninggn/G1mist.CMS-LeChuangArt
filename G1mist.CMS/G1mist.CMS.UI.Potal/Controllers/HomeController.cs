using System.Web.Mvc;
using G1mist.CMS.Modal;

namespace G1mist.CMS.UI.Potal.Controllers
{
    public class HomeController : Controller
    {
        public IRepository.IRepository<T_Users> UserService { get; set; }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            //// throw new HttpException(500,"");
            var templatePath = Server.MapPath("/templates/");
            var velocityHelper = new Common.VelocityHelper(templatePath);

            var list = UserService.GetList(a => a.id > 0);
            velocityHelper.Put("userList", list);

            velocityHelper.Display("index.htm");
            return Content("1");
        }
    }
}
