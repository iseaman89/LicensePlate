using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using Tesseract;
using static System.Drawing.Imaging.ImageFormat;
using Rectangle = System.Drawing.Rectangle;

namespace LicensePlateServer.Statics;

public static class ImageConverter
{
    public static Bitmap ImageSharpToBitmap(this SixLabors.ImageSharp.Image img)
    {
        if (img == null) return new Bitmap(0, 0);
        var stream = new MemoryStream();
        img.Save(stream, BmpFormat.Instance);
        stream.Position = 0;
        return new Bitmap(stream);
    }

    public static Image<TPixel> ToImageSharpImage<TPixel>(this System.Drawing.Bitmap bitmap) where TPixel : unmanaged, IPixel<TPixel>
    {
        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream, Png);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return SixLabors.ImageSharp.Image.Load<TPixel>(memoryStream);
        }
    }
    
    public static SixLabors.ImageSharp.Rectangle ConvertRectangleFToRectangleImageSharp(SixLabors.ImageSharp.RectangleF rectF)
    {
        // Truncate the floating-point values to integers (you could also use Math.Round, Math.Ceiling, etc.)
        int x = (int)rectF.X;
        int y = (int)rectF.Y;
        int width = (int)rectF.Width;
        int height = (int)rectF.Height;

        // Create and return the System.Drawing.Rectangle
        return new SixLabors.ImageSharp.Rectangle(x, y, width, height);
    }
    
    public static System.Drawing.Rectangle ConvertRectangleFToRectangle(SixLabors.ImageSharp.RectangleF rectF)
    {
        // Truncate the floating-point values to integers (you could also use Math.Round, Math.Ceiling, etc.)
        int x = (int)rectF.X;
        int y = (int)rectF.Y;
        int width = (int)rectF.Width;
        int height = (int)rectF.Height;

        // Create and return the System.Drawing.Rectangle
        return new System.Drawing.Rectangle(x, y, width, height);
    }
    
    public static Pix ConvertBitmapToPix(Bitmap bitmap)
    {
        Pix pix = null;
   
        using (var stream = new System.IO.MemoryStream())
        {
            bitmap.Save(stream, Png);
            stream.Position = 0;
            pix = Pix.LoadFromMemory(stream.ToArray());
        }
        return pix;
    }
}