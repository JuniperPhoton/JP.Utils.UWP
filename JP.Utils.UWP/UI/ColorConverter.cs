using System;
using System.Text.RegularExpressions;
using Windows.Security.Cryptography;
using Windows.UI;

namespace JP.Utils.UI
{
    public static class ColorConverter
    {
        private static double GetLumaFromColor(Color color)
        {
            return 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
        }

        public static bool IsLight(Color color)
        {
            var luma = GetLumaFromColor(color);
            if (luma >= 120)
            {
                return true;
            }
            else return false;
        }

        public static bool IsLightColor(this Color color)
        {
            return IsLight(color);
        }

        public static string RGBToHex(int r, int g, int b)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// 将16进制的颜色标识转换为ARGB 表示的Color
        /// 格式为：#FFF75B44
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        [Obsolete("Please use ToColor method")]
        public static Color? HexToColor(string hexColor)
        {
            string r, g, b;
            string a = "FF";
            hexColor = hexColor.ToUpper();

            if (hexColor != string.Empty)
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

        [Obsolete("Please use ToColor method")]
        public static Color? GetColor(this string colorString)
        {
            return HexToColor(colorString);
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

        /// <summary>
        /// 将 HexString 转换为 Byte 数组数据
        /// </summary>
        /// <param name="hexString">需要转换的 HexString </param>
        /// <returns>转换完成的 Byte 数组</returns>
        public static byte[] DecodeFromHexString(String hexString)
        {
            var buff = CryptographicBuffer.DecodeFromHexString(hexString);
            byte[] data = new byte[buff.Length];
            CryptographicBuffer.CopyToByteArray(buff, out data);
            return data;
        }

        public static Color ToColor(this string value)
        {
            value = value.ToUpper();
            string colorString = string.Empty;
            var match = Regex.Match(value, @"#([0-9A-F]+)");
            if (match.Success)
            {
                var code = match.Groups[1].Value;
                switch (code.Length)
                {
                    case 3:
                        colorString = string.Format("FF{0}{0}{1}{1}{2}{2}", code[0], code[1], code[2]);
                        break;

                    case 4:
                        colorString = string.Format("{0}{0}{1}{1}{2}{2}{3}{3}", code[0], code[1], code[2], code[3]);
                        break;

                    case 6:
                        colorString = string.Concat("FF", code);
                        break;

                    case 8:
                        colorString = code;
                        break;
                }
            }

            if (colorString != string.Empty)
            {
                var colorData = DecodeFromHexString(colorString);
                return Color.FromArgb(colorData[0], colorData[1], colorData[2], colorData[3]);
            }
            else
            {
                throw new FormatException("ColorCode不符合规范,应该以#开头,加上3,4,6或8位的16进制颜色值(ARGB顺序).\r\n#123(#FF112233),#1234(11223344),#123456(#FF123456),#12345678(#12345678),都是合法的颜色代码格式.\r\n");
            }
        }

        public static Color ToColor(this int value)
        {
            return Color.FromArgb(
                (byte)((value & 0xFF000000) / 0x1000000),
                (byte)((value & 0x00FF0000) / 0x10000),
                (byte)((value & 0x0000FF00) / 0x10),
                (byte)((value & 0x000000FF)));
        }
    }
}