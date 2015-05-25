using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace G1mist.CMS.Common
{
    public class IpHelper
    {
        public static IPResult GetAreasByIp(string ip)
        {
            var IsLocalIP = CheckIP(ip);

            if (IsLocalIP)
            {
                return new IPResult
                {
                    errMsg = "成功",
                    errNum = "1",
                    retData = new retData
                        {
                            city = "本地",
                            province = "本地",
                            country = "本地",
                            carrier = "本地",
                            ip = ip,
                            district = "本地"
                        }
                };
            }
            else
            {
                var client = new HttpClient();

                var response = client.GetAsync("http://apistore.baidu.com/microservice/iplookup?ip=" + ip).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // 解析响应体。阻塞！
                    var result = response.Content.ReadAsStringAsync().Result;
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };
                    var ipResult = JsonConvert.DeserializeObject<IPResult>(result, settings);

                    return ipResult;

                }
            }
            return null;
        }

        private static bool CheckIP(string ip)
        {
            if (ip.Equals("127.0.0.1") || ip.Equals("::1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class IPResult
    {
        public string errNum { get; set; }
        public string errMsg { get; set; }
        public retData retData { get; set; }
    }

    public class retData
    {
        public string ip { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string carrier { get; set; }
    }
}
