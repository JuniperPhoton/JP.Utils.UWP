namespace JP.API
{
    public class CommonRespMsg
    {
        /// <summary>
        /// 是否请求成功而且得到正确的数据，如果能请求成功但是返回来有Error字段，也应该是false,
        /// 请求失败的话应该抛出异常
        /// </summary>
        public bool IsRequestSuccessful { get; set; } = true;

        public string RequestErrorMsg { get; set; } = string.Empty;

        public string JsonSrc { get; set; } = string.Empty;

        public CommonRespMsg()
        {

        }
    }
}
