using System;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.Storage.Streams;

namespace JP.Utils.Data
{
    public static class FileIOHelper
    {
        public static async Task<byte[]> ReadFileToByteArrayAsync(this StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                return stream.ReadStreamToByteArray();
            }
        }

        public static async Task<IRandomAccessStream> ReadFileToIRandomStreamAsync(this StorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                return stream.AsRandomAccessStream();
            }
        }

        public static byte[] ReadStreamToByteArray(this IRandomAccessStream oriStream)
        {
            var stream = oriStream.AsStreamForRead();
            stream.Seek(0, SeekOrigin.Begin);
            var data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
