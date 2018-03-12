using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Renderer
{
    // From https://stackoverflow.com/a/8058012/158285
    class ImageCrop
    {
        public static byte[][] GetRGB(Bitmap bmp)
        {
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmp_data.Scan0;
            int num_pixels = bmp.Width * bmp.Height, num_bytes = bmp_data.Stride * bmp.Height, padding = bmp_data.Stride - bmp.Width * 3, i = 0, ct = 1;
            byte[] r = new byte[num_pixels], g = new byte[num_pixels], b = new byte[num_pixels], rgb = new byte[num_bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgb, 0, num_bytes);

            for (int x = 0; x < num_bytes - 3; x += 3)
            {
                if (x == (bmp_data.Stride * ct - padding)) { x += padding; ct++; };
                r[i] = rgb[x]; g[i] = rgb[x + 1]; b[i] = rgb[x + 2]; i++;
            }
            bmp.UnlockBits(bmp_data);
            return new byte[3][] { r, g, b };
        }
        public static Image AutoCrop(Bitmap bmp)
        {
            //Get an array containing the R,G,B components of each pixel
            var pixels = GetRGB(bmp);

            int h = bmp.Height - 1, w = bmp.Width, top = 0, bottom = h, left = bmp.Width, right = 0, white = 0;
            int tolerance = 95; // 95%

            bool prev_color = false;
            for (int i = 0; i < pixels[0].Length; i++)
            {
                int x = (i % (w)), y = (int)(Math.Floor((decimal)(i / w))), tol = 255 * tolerance / 100;
                if (pixels[0][i] >= tol && pixels[1][i] >= tol && pixels[2][i] >= tol) { white++; right = (x > right && white == 1) ? x : right; }
                else { left = (x < left && white >= 1) ? x : left; right = (x == w - 1 && white == 0) ? w - 1 : right; white = 0; }
                if (white == w) { top = (y - top < 3) ? y : top; bottom = (prev_color && x == w - 1 && y > top + 1) ? y : bottom; }
                left = (x == 0 && white == 0) ? 0 : left; bottom = (y == h && x == w - 1 && white != w && prev_color) ? h + 1 : bottom;
                if (x == w - 1) { prev_color = (white < w) ? true : false; white = 0; }
            }
            right = (right == 0) ? w : right; left = (left == w) ? 0 : left;

            //Crop the image
            if (bottom - top > 0)
            {
                Bitmap bmpCrop = bmp.Clone(new Rectangle(left, top, right - left + 1, bottom - top), bmp.PixelFormat);

                return (Bitmap)(bmpCrop);
            }
            else
            {
                return bmp;
            }
        }


    }
}
