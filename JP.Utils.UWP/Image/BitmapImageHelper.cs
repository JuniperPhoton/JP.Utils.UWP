using JP.Utils.Debug;
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace JP.Utils.Image
{
    public class BitmapHandleHelper
    {
        /// <summary>
        /// 改变图片大小
        /// </summary>
        /// <param name="sourceStream">包含图片数据的数据流</param>
        /// <param name="longSize">如果图片长大于宽，那么此为改编后的长度，反之是改变后的高度</param>
        /// <returns></returns>
        public static async Task<StorageFile> ResizeImage(StorageFile srcFile, uint longSize)
        {
            try
            {
                using (var fs = await srcFile.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fs);
                    uint height = decoder.PixelHeight;
                    uint weight = decoder.PixelWidth;

                    double rate;
                    uint destHeight = height;
                    uint destWeight = weight;

                    if (weight > height)
                    {
                        rate = longSize / (double)weight;
                        destHeight = weight > longSize ? (uint)(rate * height) : height;
                        destWeight = longSize;
                    }
                    else
                    {
                        rate = longSize / (double)height;
                        destWeight = height > longSize ? (uint)(rate * weight) : weight;
                        destHeight = longSize;
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
            }
            catch (Exception e)
            {
                var task = Logger.LogAsync(e);
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
        public static async Task<StorageFile> ResizeImageHard(StorageFile file, uint expWidth, uint expHeight)
        {
            using (var fs = await file.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fs);
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
}