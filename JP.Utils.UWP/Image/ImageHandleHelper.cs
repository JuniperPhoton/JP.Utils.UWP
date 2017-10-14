using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace JP.Utils.Image
{
    public class ImageHandleHelper
    {
        /// <summary>
        /// 计算压缩后图像的尺寸
        /// </summary>
        /// <param name="scaledLong">压缩后的相对长度</param>
        /// <param name="width">原来的宽</param>
        /// <param name="height">原来的高</param>
        /// <returns>返回包含处理后的图像尺寸信息</returns>
        private static Size GetCompressedSize(uint scaledLong, uint width, uint height)
        {
            var outputHeight = height;
            var outputWidth = width;

            var minus = Math.Abs((int)width - (int)height);
            if (minus < 10)
            {
                outputHeight = scaledLong;
                outputWidth = scaledLong;
                return new Size(outputWidth, outputHeight);
            }

            if (width > height && width > scaledLong)
            {
                var factor = (double)scaledLong / width;
                width = (uint)(width * factor);
                height = (uint)(height * factor);
                outputWidth = width;
                outputHeight = height;
            }
            else if (height > width && height > scaledLong)
            {
                var factor = (double)scaledLong / height;
                width = (uint)(width * factor);
                height = (uint)(height * factor);
                outputWidth = width;
                outputHeight = height;
            }
            return new Size(outputWidth, outputHeight);
        }

        private static async Task<StorageFile> GetTempFile(string fileName)
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("temp", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            return file;
        }

        public static async Task DeleteTempFile()
        {
            var folder = await ApplicationData.Current.TemporaryFolder.GetFolderAsync("temp");
            if (folder != null)
            {
                await folder.DeleteAsync();
            }
        }

        public static async Task<StorageFile> CompressImageAsync(StorageFile fileToHandle, uint scaledLong)
        {
            using (var fileStream = await fileToHandle.OpenAsync(FileAccessMode.Read))
            {
                Guid guid;
                if (fileToHandle.FileType.Contains("jpg"))
                {
                    guid = BitmapEncoder.JpegEncoderId;
                }
                else if (fileToHandle.FileType.Contains("png"))
                {
                    guid = BitmapEncoder.PngEncoderId;
                }
                else
                {
                    throw new ArgumentException();
                }
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var data = await decoder.GetPixelDataAsync();
                var newSize = GetCompressedSize(scaledLong, decoder.PixelWidth, decoder.PixelHeight);

                var fileToSave = await GetTempFile(fileToHandle.Name);
                using (var saveStream = await fileToSave.OpenAsync(FileAccessMode.ReadWrite))
                {
                    try
                    {
                        var encoder = await BitmapEncoder.CreateAsync(guid, saveStream);
                        encoder.BitmapTransform.ScaledWidth = (uint)newSize.Width;
                        encoder.BitmapTransform.ScaledHeight = (uint)newSize.Height;
                        encoder.SetPixelData(
                            decoder.BitmapPixelFormat,
                            decoder.BitmapAlphaMode,
                            decoder.PixelWidth,
                            decoder.PixelHeight,
                            decoder.DpiX,
                            decoder.DpiY,
                            data.DetachPixelData());
                        await encoder.FlushAsync();
                        return fileToSave;
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
            }
        }
    }
}