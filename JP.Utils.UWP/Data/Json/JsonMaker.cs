using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace JP.Utils.Data.Json
{
    public class JsonMaker
    {
        /// <summary>
        /// 把属性和值，格式化为JSON格式，比如 "name":"JuniperPhoton"
        /// </summary>
        /// <param name="propertyName">属性名字</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="isString">本身是否作为 Json 的一个字符串值，默认为 false。设置true 后，所有\都会被转义</param>
        /// <returns></returns>
        public static string MakeJsonObj(string propertyName,string propertyValue,bool isString=false)
        {
            string str;
            if(isString)
            {
                str = string.Format("\\\"{0}\\\":\\\"{1}\\\"", propertyName, propertyValue);
            }
            else str = string.Format("\"{0}\":\"{1}\"", propertyName, propertyValue);

            return str;
        }

        /// <summary>
        /// 把属性和值，格式化为JSON格式，比如 "gender":1
        /// </summary>
        /// <param name="propertyName">属性名字</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public static string MakeJsonObj(string propertyName, int propertyValue,bool isString= false)
        {
            string str;
            if(isString)
            {
                str = string.Format("\\\"{0}\\\":{1}", propertyName, propertyValue);
            }
            else str = string.Format("\"{0}\":{1}", propertyName, propertyValue);

            return str;
        }

        public static string MakeJsonObj(string propertyName, double propertyValue, bool isString = false)
        {
            string str;
            if (isString)
            {
                str = string.Format("\\\"{0}\\\":{1}", propertyName, propertyValue);
            }
            else str = string.Format("\"{0}\":{1}", propertyName, propertyValue);

            return str;
        }

        /// <summary>
        /// 把属性和值，格式化为JSON格式，比如 "IsCheckd":true
        /// </summary>
        /// <param name="propertyName">属性名字</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public static string MakeJsonObj(string propertyName, bool propertyValue, bool isString = false)
        {
            string str;
            if (isString)
            {
                str = string.Format("\\\"{0}\\\":{1}", propertyName, propertyValue?"true":"false");
            }
            else str = string.Format("\"{0}\":{1}", propertyName, propertyValue? "true" : "false");

            return str;
        }

        public static string MakeJsonObj(string propertyName, JsonArray propertyValue, bool isString = false)
        {
            string str;
            if (isString)
            {
                str = string.Format("\\\"{0}\\\":{1}", propertyName, propertyValue.ToString());
            }
            else str = string.Format("\"{0}\":{1}", propertyName, propertyValue.ToString());

            return str;
        }

        /// <summary>
        /// 格式化各属性JSON 格式为 JSON 字符串，比如 {"name":"chao","gender":"1"}
        /// </summary>
        /// <param name="objArray"></param>
        /// <returns></returns>
        public static string MakeJsonString(List<string> objArray)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for(int i=0;i<objArray.Count;i++)
            {
                sb.Append(objArray[i]);
                if(i!=objArray.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
