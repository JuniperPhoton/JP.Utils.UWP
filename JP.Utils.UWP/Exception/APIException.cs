using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Exceptions
{
    public class APIException : Exception
    {
        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }

        public APIException(int code, string msg)
        {
            ErrorCode = code;
            ErrorMsg = msg;
        }
    }
}
