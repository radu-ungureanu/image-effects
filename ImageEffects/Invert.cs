using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace ImageEffects
{
    public class Invert
    {
        public WriteableBitmap InvertEffect(WriteableBitmap image)
        {
            WriteableBitmap result = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            int[] ARGBPx = image.Pixels;

            for (int pixelIndex = 0; pixelIndex < ARGBPx.Length; pixelIndex++)
            {
                int color = ARGBPx[pixelIndex];
                // Retrieve A, R, G, B colors
                var a = 0x000000FF & (color >> 24);
                var r = 0x000000FF & (color >> 16);
                var g = 0x000000FF & (color >> 8);
                var b = 0x000000FF & (color);

                // Invert
                r = 0x000000FF & (0xFF - r);
                g = 0x000000FF & (0xFF - g);
                b = 0x000000FF & (0xFF - b);

                // Set result color
                var invert = (a << 24) | (r << 16) | (g << 8) | b;
                ARGBPx[pixelIndex] = invert;
            }

            // Return resulted image
            ARGBPx.CopyTo(result.Pixels, 0);
            return result;
        }
    }
}
