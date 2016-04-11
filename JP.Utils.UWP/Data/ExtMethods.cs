using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Utils.Data
{
    public static class ExtMethods
    {
        public static bool IsNotNullOrEmpty(this string src)
        {
            return !string.IsNullOrEmpty(src);
        }
    }
}
