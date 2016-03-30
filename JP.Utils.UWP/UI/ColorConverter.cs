using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace JP.Utils.UI
{
    public class ColorConverter
    {
        public static double GetLumaFromColor(Color color)
        {
            return  0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
        }

        public static bool IsLight(Color color)
        {
            var luma = GetLumaFromColor(color);
            if (luma >=120)
            {
                return true;
            }
            else return false;
        }

        public static string RGB2Hex(int r, int g, int b)
        {
            return String.Format("#{0:x2}{1:x2}{2:x2}", (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// 将16进制的颜色标识转换为ARGB 表示的Color
        /// 格式为：#FFF75B44
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Color? Hex2Color(string hexColor)
        {
            string r, g, b;
            string a = "FF";
            hexColor = hexColor.ToUpper();

            if (hexColor != String.Empty)
            {
                hexColor = hexColor.Trim();
                if (hexColor[0] == '#') hexColor = hexColor.Substring(1, hexColor.Length - 1);
                if (hexColor.Length == 8)
                {
                    a = hexColor.Substring(0, 2);
                    hexColor = hexColor.Substring(2, hexColor.Length - 2);
                }

                hexColor = hexColor.ToUpper();

                r = hexColor.Substring(0, 2);
                g = hexColor.Substring(2, 2);
                b = hexColor.Substring(4, 2);

                a = Convert.ToString(16 * GetIntFromHex(a.Substring(0, 1)) + GetIntFromHex(a.Substring(1, 1)));
                r = Convert.ToString(16 * GetIntFromHex(r.Substring(0, 1)) + GetIntFromHex(r.Substring(1, 1)));
                g = Convert.ToString(16 * GetIntFromHex(g.Substring(0, 1)) + GetIntFromHex(g.Substring(1, 1)));
                b = Convert.ToString(16 * GetIntFromHex(b.Substring(0, 1)) + GetIntFromHex(b.Substring(1, 1)));
                
                return Color.FromArgb((byte)Convert.ToInt32(a), (byte)Convert.ToInt32(r), (byte)Convert.ToInt32(g), (byte)Convert.ToInt32(b));
            }

            return null;
        }
        private static int GetIntFromHex(string strHex)
        {
            switch (strHex)
            {
                case ("A"):
                    {
                        return 10;
                    }
                case ("B"):
                    {
                        return 11;
                    }
                case ("C"):
                    {
                        return 12;
                    }
                case ("D"):
                    {
                        return 13;
                    }
                case ("E"):
                    {
                        return 14;
                    }
                case ("F"):
                    {
                        return 15;
                    }
                default:
                    {
                        return int.Parse(strHex);
                    }
            }
        }
    }
}
