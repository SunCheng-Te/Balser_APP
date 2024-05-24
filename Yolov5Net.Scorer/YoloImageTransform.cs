using Emgu.CV;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Yolov5Net.Scorer
{
    public class YoloImageTransform
    {
        public Bitmap BitmapImage;
        public Mat frame;
        public Image<Rgba32> Rgba32Image;
        //public YoloImageTransform(Mat frame)
        //{
        //    Bitmap image = new Bitmap(frame.Width, frame.Height, frame.Step,
        //    PixelFormat.Format24bppRgb, frame.DataPointer);
        //    MemoryStream imageStream = new MemoryStream();
        //    image.Save(imageStream, ImageFormat.Png);
        //    imageStream.Seek(0, SeekOrigin.Begin);            
        //    Rgba32Image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageStream);
        //    BitmapImage = image;
        //}
        public YoloImageTransform(Bitmap bitmap)
        {
            // 將Bitmap轉換為Image<Rgba32>
            Rgba32Image = ConvertBitmapToImage(bitmap);
            BitmapImage = bitmap;
        }

        private Image<Rgba32> ConvertBitmapToImage(Bitmap bitmap)
        {
            //int width = bitmap.Width;
            //int height = bitmap.Height;
            // 使用SixLabors.ImageSharp建立一個新的Image<Rgba32>
            Image<Rgba32> image = new Image<Rgba32>(bitmap.Width, bitmap.Height);
            // 取得Bitmap的像素數組
            BitmapData bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int bytesPerPixel = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[byteCount];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixels, 0, byteCount);
            bitmap.UnlockBits(bitmapData);
            // 將像素數組複製到Image<Rgba32>
            Parallel.For(0, image.Height, y =>
            {
                int rowStart = y * bitmapData.Stride;

                for (int x = 0; x < image.Width; x++)
                {
                    int index = rowStart + x * bytesPerPixel;
                    Rgba32 pixelColor = new Rgba32(pixels[index + 2], pixels[index + 1], pixels[index], pixels[index + 3]);
                    image[x, y] = pixelColor;
                }
            });
            return image;
        }

    }
}
