using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace JP.Utils.UI
{
    public class UpdateTileHelper
    {
        /// <summary>
        /// 支持宽/中/小磁贴的更新
        /// </summary>
        /// <param name="wideTileGrid"></param>
        /// <param name="mediumTileGrid"></param>
        /// <param name="smallTileGrid"></param>
        /// <returns></returns>
        public async static Task<bool> UpdateTileForWindowsPhone(UIElement wideTileGrid, UIElement mediumTileGrid, UIElement smallTileGrid)
        {
            try
            {
                string widename = await SaveUIElementToFile(wideTileGrid, TileCategory.Wide);
                string mediumname = await SaveUIElementToFile(mediumTileGrid, TileCategory.Medium);
                string smallname = await SaveUIElementToFile(smallTileGrid, TileCategory.Small);

                if (String.IsNullOrEmpty(widename) || String.IsNullOrEmpty(mediumname) || String.IsNullOrEmpty(smallname))
                {
                    throw new NullReferenceException();
                }
                //small
                var smallTileContent = TileContentFactory.CreateTileSquare71x71Image();
                smallTileContent.Image.Src = "ms-appdata:///local/" + smallname;

                //medium
                var mediumTileContent = TileContentFactory.CreateTileSquare150x150Image();
                mediumTileContent.RequireSquare71x71Content = true;
                mediumTileContent.Square71x71Content = smallTileContent;
                mediumTileContent.Image.Src = "ms-appdata:///local/" + mediumname;
                mediumTileContent.Branding = TileBranding.None;

                //wide
                var wideTileContent = TileContentFactory.CreateTileWide310x150Image();
                wideTileContent.RequireSquare150x150Content = true;
                wideTileContent.Square150x150Content = mediumTileContent;
                wideTileContent.Image.Src = "ms-appdata:///local/" + widename;
                wideTileContent.Branding = TileBranding.None;

                var notification = wideTileContent.CreateNotification();
                //TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 支持大/宽/中/小磁贴的更新
        /// </summary>
        /// <param name="largeTileGrid"></param>
        /// <param name="wideTileGrid"></param>
        /// <param name="mediumTileGrid"></param>
        /// <param name="smallTileGrid"></param>
        /// <returns></returns>
        public async static Task<bool> UpdateTileForWindows(UIElement largeTileGrid, UIElement wideTileGrid, UIElement mediumTileGrid, UIElement smallTileGrid)
        {
            try
            {
                string largename = await SaveUIElementToFile(largeTileGrid, TileCategory.Large);
                string widename = await SaveUIElementToFile(wideTileGrid, TileCategory.Wide);
                string mediumname = await SaveUIElementToFile(mediumTileGrid, TileCategory.Medium);
                string smallname = await SaveUIElementToFile(smallTileGrid, TileCategory.Small);


                if (String.IsNullOrEmpty(widename) || String.IsNullOrEmpty(mediumname) || String.IsNullOrEmpty(smallname) || String.IsNullOrEmpty(largename))
                {
                    throw new NullReferenceException();
                }

                //small
                var smallTileContent = TileContentFactory.CreateTileSquare71x71Image();
                smallTileContent.Image.Src = "ms-appdata:///local/" + smallname;

                //medium
                var mediumTileContent = TileContentFactory.CreateTileSquare150x150Image();
                mediumTileContent.RequireSquare71x71Content = true;
                mediumTileContent.Square71x71Content = smallTileContent;
                mediumTileContent.Image.Src = "ms-appdata:///local/" + mediumname;
                mediumTileContent.Branding = TileBranding.None;

                //wide
                var wideTileContent = TileContentFactory.CreateTileWide310x150Image();
                wideTileContent.RequireSquare150x150Content = true;
                wideTileContent.Square150x150Content = mediumTileContent;
                wideTileContent.Image.Src = "ms-appdata:///local/" + widename;
                wideTileContent.Branding = TileBranding.None;

                var largeTileContent = TileContentFactory.CreateTileSquare310x310Image();
                largeTileContent.RequireWide310x150Content = true;
                largeTileContent.Wide310x150Content = wideTileContent;
                largeTileContent.Image.Src = "ms-appdata:///local/" + largename;
                largeTileContent.Branding = TileBranding.None;

                var notification = largeTileContent.CreateNotification();
                //TileUpdateManager.CreateTileUpdaterForApplication().Clear();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async static Task<string> SaveUIElementToFile(UIElement element, TileCategory cate)
        {

            try
            {
                string filename = "";
                switch (cate)
                {
                    case TileCategory.Small: filename = "smallTile.png"; break;
                    case TileCategory.Medium: filename = "mediumTile.png"; break;
                    case TileCategory.Wide: filename = "wideTile.png"; break;
                    case TileCategory.Large: filename = "largeTile.png"; break;
                    default: filename = "largeTile.png"; break;
                }

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                CachedFileManager.DeferUpdates(file);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(element);
                    var pixels = await bitmap.GetPixelsAsync();

                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixels.ToArray());

                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(file);

                return file.Name;
            }
            catch (Exception e)
            {

                return null;
            }

        }


        public static void ClearAll()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }
    }

    public enum TileCategory
    {
        Small,
        Medium,
        Wide,
        Large
    }
}
