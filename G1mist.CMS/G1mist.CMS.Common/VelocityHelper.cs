using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using Commons.Collections;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace G1mist.CMS.Common
{
    /// <summary>
    /// NVelocity模板工具类 VelocityHelper
    /// </summary>
    public class VelocityHelper
    {
        private VelocityEngine _velocity;
        private VelocityContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="templatDir">模板文件夹路径</param>
        public VelocityHelper(string templatDir)
        {
            Init(templatDir);
        }

        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public VelocityHelper() { }

        /// <summary>
        /// 初始话NVelocity模块
        /// </summary>
        /// <param name="templatDir">模板文件夹路径</param>
        public void Init(string templatDir)
        {
            //创建VelocityEngine实例对象
            _velocity = new VelocityEngine();

            //使用设置初始化VelocityEngine
            var props = new ExtendedProperties();
            props.AddProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            props.AddProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, templatDir);
            props.AddProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
            props.AddProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");
            _velocity.Init(props);

            //为模板变量赋值
            _context = new VelocityContext();
        }

        /// <summary>
        /// 给模板变量赋值
        /// </summary>
        /// <param name="key">模板变量</param>
        /// <param name="value">模板变量值</param>
        public void Put(string key, object value)
        {
            if (_context == null)
                _context = new VelocityContext();
            _context.Put(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        public void PutHash(Hashtable hash)
        {
            foreach (DictionaryEntry para in hash)
            {
                Put(para.Key.ToString(), para.Value);
            }
        }

        /// <summary>
        /// 显示模板
        /// </summary>
        /// <param name="templatFileName">模板文件名</param>
        public void Display(string templatFileName)
        {
            //从文件中读取模板
            var template = _velocity.GetTemplate(templatFileName);
            //合并模板
            var writer = new StringWriter();
            template.Merge(_context, writer);
            //简体繁体的转换
            // int v = Cookie.GetValue("trade") == "" ? 0 : Convert.ToInt32(Cookie.GetValue("trade"));
            //string text = Zwbk.Common.ComMethod.getTrad3(writer.ToString(),v);
            //输出
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(writer.ToString());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 根据模板生成静态页面
        /// </summary>
        /// <param name="templatFileName"></param>
        /// <param name="htmlpath"></param>
        public void CreateHtml(string templatFileName, string htmlpath)
        {
            //从文件中读取模板
            var template = _velocity.GetTemplate(templatFileName);
            //合并模板
            var writer = new StringWriter();
            template.Merge(_context, writer);
            using (var write2 = new StreamWriter(htmlpath, false, Encoding.UTF8, 200))
            {
                write2.Write(writer);
                write2.Flush();
                write2.Close();
            }
        }
    }
}