using ButterUWP.Exceptions;
using ButterUWP.Model;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using JP.Utils.Debug;
using Qiniu.IO.Resumable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace ButterUWP.API
{
    public static class CloudServices
    {
        /// <summary>
        /// 更新用户名字
        /// </summary>
        /// <param name="newName">新名字</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> UpdateUserNameAsync(string newName)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("screen_name", newName));

            return await APIHelper.SendPostRequestAsync(UrlHelper.UpdateNameUrl, dic);
        }

        #region Comment
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="commentContent">评论内容</param>
        /// <param name="imgid">评论的照片ID</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> MakeCommentAsync(string commentContent,string imgid)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("content", commentContent));
            dic.Add(new KeyValuePair<string, string>("imgid", imgid));

            return await APIHelper.SendPostRequestAsync(UrlHelper.MakeCommentUrl, dic);
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="imgid"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetCommentsAsync(string page,string limit, string imgid)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("page", page));
            dic.Add(new KeyValuePair<string, string>("limit", limit));
            dic.Add(new KeyValuePair<string, string>("imgid", imgid));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetCommentsUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> DeleteCommentsAsync(string commentid)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("id", commentid));

            return await APIHelper.SendPostRequestAsync(UrlHelper.DeleteCommentUrl, dic);
        }

        /// <summary>
        /// 举报评论
        /// </summary>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> ReportCommentsAsync(string commentid,string type,string reason)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("commentid", commentid));
            dic.Add(new KeyValuePair<string, string>("reporttype", type));
            dic.Add(new KeyValuePair<string, string>("reportreason", reason));

            return await APIHelper.SendPostRequestAsync(UrlHelper.ReportCommentUrl, dic);
        }
        #endregion

        #region Search
        public async static Task<CommonRespMsg> GetSearchHotWordsAsnyc()
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetSearchHotWordsUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="type">0 是照片，1是用户</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> SearchAsync(int type,string page,string limit,string keyword)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("page", page));
            dic.Add(new KeyValuePair<string, string>("per_page", limit));
            dic.Add(new KeyValuePair<string, string>("q", keyword));

            var url = UrlHelper.MakeFullUrlForGetReq(type==0?UrlHelper.GetSearchImageResultUrl:UrlHelper.GetSearchUserResultUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }
        #endregion

        #region UserSquare
        public async static Task<CommonRespMsg> GetUserSquareAsync(string page, string limit)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("page", page));
            dic.Add(new KeyValuePair<string, string>("limit", limit));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetUserSquareUrl,dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        public async static Task<CommonRespMsg> GetOfficialAsync()
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetOfficialUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }
        #endregion

        #region Message
        /// <summary>
        /// 获取未读消息数目
        /// </summary>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetMessageCountAsync()
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetUnReadMessageCountUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        /// <summary>
        /// 获取消息
        /// </summary>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetMessagesAsync()
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetMessagesUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }
        #endregion

        #region Banner
        /// <summary>
        /// 获取 Banner 
        /// </summary>
        /// <param name="oriUrl"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetBannerAsync(string oriUrl)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            string url = UrlHelper.MakeFullUrlForGetReq(oriUrl, dic);
            var resultToReturn = await APIHelper.SendGetRequestAsync(url);
            return resultToReturn;
        }
        #endregion

        #region Activity
        /// <summary>
        /// 获取队形
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetActivityAsync(string activityID)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("limit", "1"));
            dic.Add(new KeyValuePair<string, string>("page", "0"));
            dic.Add(new KeyValuePair<string, string>("activity_id", activityID.ToString()));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetActivityUrl, dic);
            var result = await APIHelper.SendGetRequestAsync(url);
            return result;
        }

        /// <summary>
        /// 获取队形里的图片
        /// </summary>
        /// <param name="activityID">活动ID</param>
        /// <param name="type">类型，0 是最新的，1是精选的</param>
        /// <param name="lastID">上一个分页最后的ID</param>
        /// <param name="limit">返回的数目</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetActivityImage(string activityID, int type, string lastID, int limit)
        {
            CommonRespMsg resultToReturn = new CommonRespMsg();
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("lastid", lastID == null ? "" : lastID));
                dic.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("type", type.ToString()));
                dic.Add(new KeyValuePair<string, string>("activity_id", activityID.ToString()));

                string url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetActivityImagesUrl, dic);

                resultToReturn = await APIHelper.SendGetRequestAsync(url);

                return resultToReturn;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetActivityImage));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }

        #endregion

        #region Login

        /// <summary>
        /// 注册的时候，检查手机号是否被注册，并发送验证短信。
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> CheckPhoneNum(string phoneNum,string zone="86")
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("mobile", phoneNum));
            dic.Add(new KeyValuePair<string, string>("zone", zone));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.CheckPhoneNumUrl, dic);
            try
            {
                return await APIHelper.SendGetRequestAsync(url);
            }
            catch(APIException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecord(ex, nameof(CloudServices), nameof(CheckPhoneNum));
                throw ex;
            }
        }

        /// <summary>
        /// 注册账户
        /// </summary>
        /// <param name="phoneNum">电话号码</param>
        /// <param name="zone">地区，默认86</param>
        /// <param name="code">手机验证码</param>
        /// <param name="nickName">昵称</param>
        /// <param name="passwordInRaw">原始密码</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> Register(string phoneNum,string zone,string nickName,string passwordInRaw, string code="86")
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("mobile", phoneNum));
            dic.Add(new KeyValuePair<string, string>("zone", zone));
            dic.Add(new KeyValuePair<string, string>("password", passwordInRaw));
            dic.Add(new KeyValuePair<string, string>("code", code));

            return await APIHelper.SendPostRequestAsync(UrlHelper.RegisterUrl,dic);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="type">0:Email 1:Weibo 2:QQ 3:Wechat 4:Mobile</param>
        /// <param name="emailOrMobile"></param>
        /// <param name="password"></param>
        /// <param name="zoneCode">default:86</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> LoginViaEmailOrMobileAsync(int type, string emailOrMobile, string passwordMD5, string zoneCode = "86")
        {
            if(type!=0 && type!=4)
            {
                throw new ArgumentException();
            }

            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("password", passwordMD5));
            dic.Add(new KeyValuePair<string, string>("type", type.ToString()));
            dic.Add(new KeyValuePair<string, string>(type == 0 ? "email" : "mobile", emailOrMobile));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            if (type == 4)
            {
                dic.Add(new KeyValuePair<string, string>("zone", zoneCode));
            }

            try
            {
                return await APIHelper.SendPostRequestAsync(UrlHelper.LoginUrl, dic);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(LoginViaEmailOrMobileAsync));
                throw e;
            }
        }

        public async static Task<CommonRespMsg> LoginViaWeiboAsync(string weiboAT)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("type","1"));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("weibo_token", weiboAT));

            try
            {
                return await APIHelper.SendPostRequestAsync(UrlHelper.LoginUrl, dic);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(LoginViaEmailOrMobileAsync));
                throw e;
            }
        }


        #endregion

        #region User
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="targetID">用户ID</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> GetUserInfo(string targetID)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("userid", targetID));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetUserInfoUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        //获取关注/粉丝
        public async static Task<CommonRespMsg> GetUserInvolvedPeople(string requestUrl, string page, string limit, string targetID)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("page", page));
            dic.Add(new KeyValuePair<string, string>("limit", limit));
            dic.Add(new KeyValuePair<string, string>("userid", targetID));

            var url = UrlHelper.MakeFullUrlForGetReq(requestUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        #endregion

        #region Common

        /// <summary>
        /// 获取图片流里的图片
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="baseUrl"></param>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public async static Task<IEnumerable<ButterPhoto>> GetImageListAsync(string uid, int page, int limit, string baseUrl, string targetID = "")
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>(nameof(page), page.ToString()));
                dic.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", uid));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                if (!string.IsNullOrEmpty(targetID)) dic.Add(new KeyValuePair<string, string>("userid", targetID));

                string url = UrlHelper.MakeFullUrlForGetReq(baseUrl, dic);
                IEnumerable<ButterPhoto> dataToReturn = new List<ButterPhoto>();

                var resultToReturn = await APIHelper.SendGetRequestAsync(url);
                var json = resultToReturn.JsonSrc;

                JsonObject jsonobj = null;
                JsonArray jsonArray = null;
                var isObject = JsonObject.TryParse(json, out jsonobj);

                if (isObject)
                {
                    jsonArray = JsonParser.GetJsonArrayFromJsonObj(jsonobj, "datas");
                    if (jsonArray == null) jsonArray = JsonParser.GetJsonArrayFromJsonObj(jsonobj, "list");
                }
                else JsonArray.TryParse(json, out jsonArray);
                dataToReturn = await ButterPhoto.GenerateListAsync(jsonArray.ToString());

                return dataToReturn;
            }
            catch (Exception e2)
            {
                var task = ExceptionHelper.WriteRecord(e2, nameof(CloudServices), nameof(GetImageListAsync));
                throw;
            }
        }


        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="fileData">包含图片信息的对象</param>
        /// <param name="imgKind">照片类型</param>
        /// <returns></returns>
        public async static Task<CommonRespMsg> UploadImageAsnyc(ButterFileToUpload fileData, UploadImageKind imgKind,CancellationToken ctoken)
        {
            var kindstr = "1:1";
            if (fileData.ScaleKind == ScaleKind.Scale_3x4) kindstr = "3:4";
            else if (fileData.ScaleKind == ScaleKind.Scale_4x3) kindstr = "4:3";

            CommonRespMsg result = new CommonRespMsg();
            try
            {
                using (var client = new HttpClient())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("uid", LocalSettingHelper.GetValue("uid"));
                    dic.Add("access_token", LocalSettingHelper.GetValue("access_token"));
                    dic.Add("appkey", UrlHelper.APP_KEY);
                    dic.Add("img_info", "{\"scale\":\"" + kindstr + "\"}");
                    dic.Add("ps", fileData.PsData);
                    dic.Add("is_private", fileData.IsPrivate ? "1" : "0");
                    if (fileData.ActivityID != null) dic.Add("activity_id", fileData.ActivityID);
                    if (fileData.OriginalImageID != -1) dic.Add("ding_imgid", fileData.OriginalImageID.ToString());

                    var resp = await client.PostAsync(new Uri(imgKind==UploadImageKind.Photo?UrlHelper.UploadImageUrl:UrlHelper.UploadAvatarUrl), new HttpFormUrlEncodedContent(dic));
                    var json = JsonObject.Parse(await resp.Content.ReadAsStringAsync());

                    var token = JsonParser.GetStringFromJsonObj(json, "token");
                    var key = JsonParser.GetStringFromJsonObj(json, "key");

                    Settings setting = new Settings();
                    ResumablePutExtra extra = new ResumablePutExtra();
                    ResumablePut rclient = new ResumablePut(setting, extra);
                    extra.Notify += ((sendern, en) =>
                      {

                      });

                    ctoken.ThrowIfCancellationRequested();
                   
                    var ioresult = await Task.Run(async () =>
                      {
                          ctoken.ThrowIfCancellationRequested();

                          return await rclient.PutFile(token, fileData.File.Path, key,ctoken);
                      });

                    if (!ioresult.OK) throw new APIException(int.Parse(ioresult.StatusCode.ToString()), ioresult.Exception.Message);
                    else
                    {
                        return result;
                    }
                }
            }
            catch (APIException)
            {
                result.IsSuccessful = false;
                return result;
            }
            catch(OperationCanceledException)
            {
                throw;
            }
            catch (Exception ee)
            {
                var task = ExceptionHelper.WriteRecord(ee, nameof(CloudServices), nameof(UploadImageAsnyc));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = ee.Message
                };
                return resultMsg;
            }
        }

        public async static Task<CommonRespMsg> GetOriginalPsAsync()
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetOriginalPsUrl, dic);
            var result = await APIHelper.SendGetRequestAsync(url);
            return result;
        }
        #endregion

        #region Ding
        public async static Task<CommonRespMsg> DingFont(List<int> missedIDs, string imgID)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("imgid", imgID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                foreach (var item in missedIDs)
                {
                    dic.Add(new KeyValuePair<string, string>("missing_fonts[]", item.ToString()));
                }
                return await APIHelper.SendPostRequestAsync(UrlHelper.DingFontUrl, dic);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(DingFont));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }

        public async static Task<CommonRespMsg> DingShape(List<string> missedShapeName)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("icon_name", string.Join(",", missedShapeName.ToArray())));

                return await APIHelper.SendPostRequestAsync(UrlHelper.DingShapeUrl, dic);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(DingShape));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }

        #endregion

        #region Image
        public async static Task<CommonRespMsg> GetImgeInfoAsync(string imgID,bool isLogined)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("imgid", imgID));

                var url = UrlHelper.MakeFullUrlForGetReq(isLogined?UrlHelper.GetImageDetailUrl:UrlHelper.GetPublicImageDetailUrl, dic);
                return await APIHelper.SendGetRequestAsync(url);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetImgeInfoAsync));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }

        public async static Task<bool> LikeImageAsync(bool like, string imgID)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("imgid", imgID));
                dic.Add(new KeyValuePair<string, string>("unlike", like ? "0" : "1"));

                var result = await APIHelper.SendPostRequestAsync(UrlHelper.LikeImageUrl, dic);
                if (!result.IsSuccessful) return false;

                var jsondata = JsonObject.Parse(result.JsonSrc);
                var status = JsonParser.GetStringFromJsonObj(jsondata, "status");
                if (status != "200") return false;

                return true;

            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(LikeImageAsync));
                return false;
            }
        }

        public async static Task<bool> DeleteImage(string imageID)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("imgid", imageID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

                var result = await APIHelper.SendPostRequestAsync(UrlHelper.DeleteImageUrl, dic);
                var jsonobj = JsonObject.Parse(result.JsonSrc);
                var code = jsonobj["status"].GetNumber();
                if (code == 200) return true;
                else return false;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(DeleteImage));
                return false;
            }
        }

        public async static Task<bool> StoreAsync(bool isStore, bool isImg, string imgID, string userid)
        {
            if (isImg && imgID == null) throw new ArgumentException("If it's img, imgid should not be null.");
            if (!isImg && userid == null) throw new ArgumentException("If it's user, userid should not be null.");

            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("key", isStore ? "1" : "0"));
            dic.Add(new KeyValuePair<string, string>("user_or_img", isImg ? "img" : "user"));
            if (isImg) dic.Add(new KeyValuePair<string, string>("imgid", imgID));
            else dic.Add(new KeyValuePair<string, string>("userid", userid));

            var result = await APIHelper.SendPostRequestAsync(UrlHelper.StoreUrl, dic);
            if (!result.IsSuccessful) return false;
            else
            {
                var json = result.JsonSrc;
                JsonObject obj = JsonObject.Parse(json);
                var store = JsonParser.GetStringFromJsonObj(obj, "store");
                if (store == null) return false;
                else return true;
            }
        }
        #endregion

        #region Shop involved
        //TODO: Test this method
        /// <summary>
        /// 获取商店页面的信息
        /// </summary>
        /// <param name="type">0 代表字体，1代表图形包</param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> GetProductList(int type, int page, int limit)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("page_s", (page * 10).ToString()));
                dic.Add(new KeyValuePair<string, string>("limit", limit.ToString()));

                var url = UrlHelper.MakeFullUrlForGetReq(type == 0 ? UrlHelper.GetFontProductsUrl : UrlHelper.GetPackageProductsUrl, dic);
                return await APIHelper.SendGetRequestAsync(url);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetProductList));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }
        /// <summary>
        /// 获取字体/图形包的详情页
        /// </summary>
        /// <param name="type">0  是字体 1 是图形包</param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> GetProductDetailList(int type, string productID)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>(type == 0 ? "productid" : "packetid", productID));

                var url = UrlHelper.MakeFullUrlForGetReq(type == 0 ? UrlHelper.GetFontProductsDetailUrl : UrlHelper.GetShapeProductsDetailUrl, dic);
                return await APIHelper.SendGetRequestAsync(url);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetProductDetailList));
                var resultMsg = new CommonRespMsg();
                resultMsg.IsSuccessful = false;
                resultMsg.ErrorMsg = e.Message;
                return resultMsg;
            }
        }

        public static async Task<CommonRespMsg> GetShapeProductDownloadUrl(string packetID)
        {
            try
            {
                var dic = new List<KeyValuePair<string, string>>();
                dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
                dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
                dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
                dic.Add(new KeyValuePair<string, string>("packetid", packetID));
                return await APIHelper.SendPostRequestAsync(UrlHelper.GetShapeProductDownloadUrl, dic);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetShapeProductDownloadUrl));
                var resultMsg = new CommonRespMsg()
                {
                    IsSuccessful = false,
                    ErrorMsg = e.Message
                };
                return resultMsg;
            }
        }

        #endregion

        #region Butter Pay
        public static async Task<CommonRespMsg> GetPaymentCharge(string chargeJson)
        {
            try
            {
                return await APIHelper.SendPostRequestAsync(UrlHelper.GetPaymentChargeUrl, chargeJson);
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e, nameof(CloudServices), nameof(GetPaymentCharge));
                return new CommonRespMsg() { IsSuccessful = false, ExtraErrorMsg = e.Message };
            }
        }

        /// <summary>
        /// 通知服务器我开始下载了
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public static async Task<CommonRespMsg> StartDownloadProduct(string productid)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));
            dic.Add(new KeyValuePair<string, string>("productid", productid));

            return await APIHelper.SendPostRequestAsync(UrlHelper.StartDownloadProductUrl, dic);
        }
        #endregion

        #region Timeline 
        public static async Task<CommonRespMsg> GetTimelineList(string lastid,string limit)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("lastid", lastid));
            dic.Add(new KeyValuePair<string, string>("limit", limit));
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetTimelineUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }

        public static async Task<CommonRespMsg> GetFeedlist(string lastid, string limit)
        {
            var dic = new List<KeyValuePair<string, string>>();
            dic.Add(new KeyValuePair<string, string>("lastdynamicid", lastid));
            dic.Add(new KeyValuePair<string, string>("limit", limit));
            dic.Add(new KeyValuePair<string, string>("access_token", UrlHelper.ACCESS_TOKEN));
            dic.Add(new KeyValuePair<string, string>("uid", UrlHelper.UID));
            dic.Add(new KeyValuePair<string, string>("appkey", UrlHelper.APP_KEY));

            var url = UrlHelper.MakeFullUrlForGetReq(UrlHelper.GetFeedUrl, dic);
            return await APIHelper.SendGetRequestAsync(url);
        }
        #endregion
    }
}