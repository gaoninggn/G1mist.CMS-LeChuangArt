using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace G1mist.CMS.Common
{
    /// <summary>
    /// XML文档助手类
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// XElement对象
        /// </summary>
        private readonly XElement _element;

        /// <summary>
        /// XML文件路径
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">XML文件的路径</param>
        public XmlHelper(string path)
        {
            _path = path;
            _element = XElement.Load(path);
        }

        /// <summary>
        /// 通过节点名称获取值
        /// </summary>
        /// <param name="elementName">节点名称</param>
        /// <returns></returns>
        public string GetValue(string elementName)
        {
            var firstOrDefault = (from r in _element.Elements(elementName)
                                  select r).FirstOrDefault();

            if (firstOrDefault != null)
            {
                var value = firstOrDefault.Value;
                return value;
            }
            return null;
        }

        /// <summary>
        /// 通过节点名设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetValue(string key, string value)
        {
            var xElement = _element.Element(key);
            if (xElement != null) xElement.ReplaceWith(new XElement(key, value));
            _element.Save(_path);
        }
    }
}
