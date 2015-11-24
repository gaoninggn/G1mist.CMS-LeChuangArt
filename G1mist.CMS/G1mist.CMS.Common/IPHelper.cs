using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace G1mist.CMS.Common
{
    public class IpHelper
    {
        public static IpResult GetAreasByIp(string ip)
        {
            var isLocalIp = CheckIp(ip);

            if (isLocalIp)
            {
                return new IpResult
                {
                    ErrMsg = "成功",
                    ErrNum = "1",
                    RetData = new RetData
                        {
                            City = "本地",
                            Province = "本地",
                            Country = "本地",
                            Carrier = "本地",
                            Ip = ip,
                            District = "本地"
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
                    var ipResult = JsonConvert.DeserializeObject<IpResult>(result, settings);

                    return ipResult;

                }
            }
            return null;
        }

        private static bool CheckIp(string ip)
        {
            return ip.Equals("127.0.0.1") || ip.Equals("::1");
        }
    }

    public class IpResult
    {
        public string ErrNum { get; set; }
        public string ErrMsg { get; set; }
        public RetData RetData { get; set; }
    }

    public class RetData
    {
        public string Ip { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Carrier { get; set; }
    }
}
