using JP.Utils.Debug;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace JP.Utils.Image
{
    public class BitmapHandleHelper
    {
        /// <summary>
        /// 改变图片大小
        /// </summary>
        /// <param name="sourceStream">包含图片数据的数据流</param>
        /// <param name="scaleLong">如果图片长大于宽，那么此为改编后的长度，反之是改变后的高度</param>
        /// <returns></returns>
        public static async Task<StorageFile> ResizeImage(IRandomAccessStream sourceStream, uint scaleLong)
        {
            try
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
                uint height = decoder.PixelHeight;
                uint weight = decoder.PixelWidth;

                double rate;
                uint destHeight = height;
                uint destWeight = weight;

                if (weight > height)
                {
                    rate = scaleLong / (double)weight;
                    destHeight = weight > scaleLong ? (uint)(rate * height) : height;
                    destWeight = scaleLong;
                }
                else
                {
                    rate = scaleLong / (double)height;
                    destWeight = height > scaleLong ? (uint)(rate * weight) : weight;
                    destHeight = scaleLong;
                }

                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledWidth = destWeight,
                    ScaledHeight = destHeight
                };

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                var folder = ApplicationData.Current.TemporaryFolder;
                var tempfile = await folder.CreateFileAsync("temp.jpg", CreationCollisionOption.GenerateUniqueName);
                using (var destStream = await tempfile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, transform.ScaledWidth, transform.ScaledHeight, 100, 100, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                    return tempfile;
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e, nameof(BitmapHandleHelper), nameof(ResizeImage));
                return null;
            }
        }

        /// <summary>
        /// 改变图片的大小
        /// </summary>
        /// <param name="sourceStream">包含图片数据的流</param>
        /// <param name="expWidth">期望的宽度</param>
        /// <param name="expHeight">期望的高度</param>
        /// <returns></returns>
        public static async Task<StorageFile> ResizeImageHard(IRandomAccessStream sourceStream, uint expWidth, uint expHeight)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);
            uint height = decoder.PixelHeight;
            uint weight = decoder.PixelWidth;

            uint destHeight = height > expHeight ? expHeight : height;
            uint destWeight = weight > expWidth ? expWidth : weight;

            BitmapTransform transform = new BitmapTransform()
            {
                ScaledWidth = destWeight,
                ScaledHeight = destHeight
            };

            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Rgba8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage);

            var tempfile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
            using (var destStream = await tempfile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destStream);
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, transform.ScaledWidth, transform.ScaledHeight, 100, 100, pixelData.DetachPixelData());
                await encoder.FlushAsync();
                return tempfile;
            }
        }
    }
}
