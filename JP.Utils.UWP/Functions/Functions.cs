using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace JP.Utils.Functions
{
    public static class Functions
    {
        public static bool HasCHZN(string inputString)
        {
            var RegCHZN = new Regex("[\\u4e00-\\u9fa5]");
            var m = RegCHZN.Match(inputString);
            return m.Success;
        }

        public static TimeSpan CalculateTimeDiff(this DateTime time1, DateTime time2)
        {
            var timeDiff = time1.Subtract(time2);
            return timeDiff;
        }

        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(strIn, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }

        public static string UrlEncode(this string str)
        {
            return WebUtility.UrlEncode(str);
        }

        public static void TryAdd(this Dictionary<string, string> dic, string key, string value)
        {
            try
            {
                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                }
                else dic.Add(key, value);
            }
            catch (Exception)
            {

            }
        }

        public static bool IsValidMobileNum(string strln)
        {
            return Regex.IsMatch(strln, @"^[1]+[8,3,5,7,4]+\d{9}");
        }

        public static string MakeStringFromList(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                var follow = list.ElementAt(i);
                sb.Append(follow);
                if (i != list.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        private static FrameworkElement FindVisualChild(DependencyObject element, string nameOfChildToFind)
        {
            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(element); x++)
            {
                var child = VisualTreeHelper.GetChild(element, x);

                if (child is FrameworkElement)
                {
                    string name = (string)child.GetValue(FrameworkElement.NameProperty);

                    if (name == nameOfChildToFind)
                    {
                        return (FrameworkElement)child;
                    }
                    else if (VisualTreeHelper.GetChildrenCount(child) > 0)
                    {
                        return FindVisualChild(child, nameOfChildToFind);
                    }
                }
            }

            return null;
        }
    }
}
