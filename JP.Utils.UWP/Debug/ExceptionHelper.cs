using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Runtime.InteropServices;
using JP.Utils.Data;
using System.IO;
using Windows.ApplicationModel.Email;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace JP.Utils.Debug
{
    public class ExceptionHelper
    {

        /// <summary>
        /// 写入独立储存，文件名为error.log
        /// </summary>
        /// <param name="e">EX</param>
        /// <param name="methodName">附带的信息</param>
        /// <returns>成功返回TRUE，失败返回FALSE</returns>
        public async static Task<bool> WriteRecordAsync(Exception e, string className = "", string methodName = "", string extraStr = "")
        {
            try
            {
                var localfolder = ApplicationData.Current.LocalFolder;
                var file = await localfolder.CreateFileAsync("error.log", CreationCollisionOption.OpenIfExists);
                var contents = Environment.NewLine +
                    "EXCEPTION:" +
                    e.ToString() +
                    Environment.NewLine +
                    "CLASS:" +
                    className +
                    Environment.NewLine +
                    "METHOD:" +
                    methodName +
                    Environment.NewLine +
                    "EXTRA:" +
                    extraStr +
                    Environment.NewLine +
                    "---------------";
                await FileIO.AppendTextAsync(file, contents);
                await UmengSDK.UmengAnalytics.TrackError(contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读出独立储存的文件
        /// </summary>
        /// <returns>返回读出的字符串，如果出错则返回NULL</returns>
        public async static Task<string> ReadRecordAsync()
        {
            try
            {
                var localfolder = ApplicationData.Current.LocalFolder;
                var file = await localfolder.GetFileAsync("error.log");
                if (file == null) return "";
                string text = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                if (text == null) text = "";
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async static Task<EmailAttachment> GetLogFileAttachement()
        {
            try
            {
                var localfolder = ApplicationData.Current.LocalFolder;
                var file = await localfolder.GetFileAsync("error.log");

                if (file == null) throw new ArgumentNullException();

                var copiedFile = await file.CopyAsync(localfolder, "error.log", NameCollisionOption.GenerateUniqueName);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                if (copiedFile == null) throw new ArgumentNullException();

                var attachment = new EmailAttachment();
                attachment.FileName = "错误日志.log";
                attachment.Data = RandomAccessStreamReference.CreateFromFile(copiedFile);

                return attachment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static Task EraseRecord()
        {
            var localfolder = ApplicationData.Current.LocalFolder;
            var file = await localfolder.TryGetFileAsync("error.log");
            if (file != null) await FileIO.WriteTextAsync(file, "");
        }
    }
}
