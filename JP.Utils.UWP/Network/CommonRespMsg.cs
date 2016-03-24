namespace JP.API
{
    public class CommonRespMsg
    {
        /// <summary>
        /// 是否请求成功而且得到正确的数据，如果能请求成功但是返回来有Error字段，也应该是false
        /// </summary>
        public bool IsSuccessful { get; set; } = true;

        /// <summary>
        /// 错误代码，0是成功
        /// </summary>
        public int ErrorCode { get; set; } = 0;

        /// <summary>
        /// 错误信息，默认是空
        /// </summary>
        public string ErrorMsg { get; set; } = string.Empty;

        public string ExtraErrorMsg { get; set; } = string.Empty;

        public string JsonSrc { get; set; } = string.Empty;

        public string ExtraData { get; set; }

        public CommonRespMsg()
        {

        }
    }
}
