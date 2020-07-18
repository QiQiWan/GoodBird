using System.IO;
using System.Text.RegularExpressions;

namespace GoodBird
{
    /// <summary>
    /// 内容提取帮助类
    /// </summary>
    public class RegexHelper
    {
        /// <summary>
        /// 返回json中的结果数组
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static public string GetElems(string content, string name)
        {
            string pattern = "\"" + name + "\": \\[[\\s\\S]*\\]";
            content = Regex.Match(content, pattern).Value;
            content = content.Replace("\"" + name + "\": [", "");
            content = content.Replace("]", "");
            return content;
        }
        static public string GetElem(string content, string name)
        {
            string headP = "\"" + name + "\":\"";
            string pattern = "\"" + name + "\":\\s*\".*?(?=\")"; //后断言匹配引号
            string result = Regex.Match(content, pattern).Value;
            result = result.Replace(headP, "");
            return result;
        }

        static public string GetNumElem(string content, string name)
        {
            string headP = "\"" + name + "\":";
            string pattern = "\"" + name + "\":\\s*[0-9]+"; //后断言匹配引号
            string result = Regex.Match(content, pattern).Value;
            result = result.Replace(headP, "");
            return result;
        }
        static public string TextToBase64(string text)
        {
            byte[] buff = System.Text.Encoding.UTF8.GetBytes(text);
            string strBaser64 = System.Convert.ToBase64String(buff);
            return strBaser64;
        }

        public static T[] MergeArr<T>(T[] headArr, T[] footArr)
        {
            T[] buffer = new T[headArr.Length + footArr.Length];
            headArr.CopyTo(buffer, 0);
            footArr.CopyTo(buffer, headArr.Length);
            return buffer;
        }
    }
}