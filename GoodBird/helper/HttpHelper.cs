using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace GoodBird
{
    /// <summary>
    /// 动态类，每个实例使用单独session
    /// </summary>
    public class HttpHelper
    {
        public string cookieStr = "";
        public readonly string domain = "";
        private CookieContainer cookie = new CookieContainer();
        private string QueriesData = "";
        /// <summary>
        /// 拼接请求参数
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        public string SetQueries(Dictionary<string, string> queries)
        {
            int queriesLength = queries.Count;

            QueriesData = "";
            foreach (var item in queries)
            {
                if (queriesLength == 1)
                    QueriesData += $"{item.Key}={item.Value}";
                else
                    QueriesData += $"{item.Key}={item.Value}&";
                queriesLength--;
            }
            return QueriesData;
        }

        public HttpHelper(string domain)
        {
            this.domain = domain;
        }

        /// <summary>
        /// post请求返回html
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpPost()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(domain);
            request.AllowAutoRedirect = false; //禁止自动重定向
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(QueriesData);
            request.CookieContainer = cookie; //cookie信息由CookieContainer自行维护
            byte[] queriesBytes = Encoding.UTF8.GetBytes(QueriesData);
            request.ContentLength = queriesBytes.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(queriesBytes, 0, queriesBytes.Length);
            myRequestStream.Close();

            HttpWebResponse response = null;
            try
            {
                this.SetCertificatePolicy();
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("网络请求错误信息: " + e.Message);
            }
            //获取重定向地址
            //string url1 = response.Headers["Location"];
            if (response != null)
            {
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            else
            {
                return "error"; //post请求返回为空
            }

        }

        /// <summary>
        /// get请求获取返回的html
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpGet()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(domain + (QueriesData == "" ? "" : "?") + QueriesData);
            request.Method = "GET";
            request.Accept = "*/*";

            request.CookieContainer = new CookieContainer();

            if(cookieStr != "")
                request.CookieContainer.Add(AnalyseCookies());
            this.SetCertificatePolicy();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            // response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        /// <summary>
        /// 获得响应中的图像流
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Stream GetResponseImage(string url)
        {
            Stream reset = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.KeepAlive = true;
                req.Method = "GET";
                req.AllowAutoRedirect = true;
                req.CookieContainer = cookie;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                req.Timeout = 50000;

                Encoding myEncoding = Encoding.GetEncoding("UTF-8");
                this.SetCertificatePolicy();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                reset = res.GetResponseStream();

                return reset;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 正则获取匹配的第一个值
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string getStringByRegex(string html, string pattern)
        {

            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matchs = re.Matches(html);
            if (matchs.Count > 0)
            {
                return matchs[0].Groups[1].Value;
            }
            else
                return "";
        }

        /// <summary>
        /// 正则验证返回的response是否正确
        /// </summary>
        /// <param name="html"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool verifyResponseHtml(string html, string pattern)
        {
            Regex re = new Regex(pattern);
            return re.IsMatch(html);
        }

        //注册证书验证回调事件，在请求之前注册
        private void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
        }
        /// <summary>  
        /// 远程证书验证，固定返回true 
        /// </summary>  
        private bool RemoteCertificateValidate(object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
        private CookieCollection AnalyseCookies()
        {
            string[] items = cookieStr.Split(';');

            if (items.Length <= 1)
                return null;

            CookieCollection cookies = new CookieCollection();
            foreach (string item in items)
            {
                string key = item.Trim().Split('=')[0];
                string value = item.Trim().Split('=')[1];
                Cookie cookie = new Cookie(key, value, "/s", domain);
                cookies.Add(cookie);
            }
            return cookies;
        }
    }
}