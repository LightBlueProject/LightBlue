using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace LightBlue.MultiHost.ViewModel
{
    static class ImageUtil
    {
        public static ImageSource ColourImage(string resource, SolidColorBrush brush)
        {
            // Copy pixel colour values from existing image.
            // (This loads them from an embedded resource. BitmapDecoder can work with any Stream, though.)
            //StreamResourceInfo x = Application.GetResourceStream(new Uri(BaseUriHelper.GetBaseUri(this), "Image.png"));
            var uri = new Uri("pack://application:,,,/LightBlue.MultiHost;component/" + resource);
            StreamResourceInfo x = Application.GetResourceStream(uri);
            BitmapDecoder dec = BitmapDecoder.Create(x.Stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapFrame image = dec.Frames[0];

            byte[] pixels = new byte[image.PixelWidth * image.PixelHeight * 4];
            image.CopyPixels(pixels, image.PixelWidth * 4, 0);

            // Modify the alpha pixels
            for (int i = 0; i < pixels.Length / 4; ++i)
            {
                byte a = pixels[i * 4 + 3];

                if (a != 0)
                {
                    pixels[i * 4] = brush.Color.B;
                    pixels[i * 4 + 1] = brush.Color.G;
                    pixels[i * 4 + 2] = brush.Color.R;
                }
            }

            // Write the modified pixels into a new bitmap and use that as the source of an Image
            var bmp = new WriteableBitmap(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, PixelFormats.Pbgra32, null);
            bmp.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), pixels, image.PixelWidth * 4, 0);
            return bmp;
        }
    }
}