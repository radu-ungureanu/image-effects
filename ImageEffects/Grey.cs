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
    public class Grey
    {
        // Make a grey image
        public WriteableBitmap GreyEffect(WriteableBitmap image)
        {
            WriteableBitmap result = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            int[] ARGBPx = image.Pixels;

            for (int pixelIndex = 0; pixelIndex < ARGBPx.Length; pixelIndex++)
            {
                int color = ARGBPx[pixelIndex];
                int gray = 0;
                // Retrieve A, R, G, B colors
                int a = color >> 24;
                int r = (color & 0x00ff0000) >> 16;
                int g = (color & 0x0000ff00) >> 8;
                int b = (color & 0x000000ff);

                if ((r == g) && (g == b))
                {
                    gray = color;
                }
                else
                {
                    // Calculate for the illumination.
                    int i = (7 * r + 38 * g + 19 * b + 32) >> 6;
                    gray = ((a & 0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
                }

                // Set result color
                ARGBPx[pixelIndex] = gray;
            }
            // Return resulted image
            ARGBPx.CopyTo(result.Pixels, 0);
            return result;
        }
    }
}
