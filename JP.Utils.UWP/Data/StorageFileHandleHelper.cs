using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JP.Utils.Data
{
    public static class StorageFileHandleHelper
    {
        /// <summary>
        /// 尝试从文件夹中获得文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="fileName">文件名</param>
        /// <returns>如果文件不存在，则返回 NULL</returns>
        public async static Task<StorageFile> TryGetFileAsync(this StorageFolder folder,string fileName)
        {
            try
            {
                var file = await folder.GetFileAsync(fileName);
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
