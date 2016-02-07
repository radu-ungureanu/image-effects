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
    public class BlackWhite
    {
        public WriteableBitmap BlackWhiteEffect(WriteableBitmap image)
        {
            WriteableBitmap binary = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            int[] histogramData = new int[256];
            int maxCount = 0;

            // First determine the histogram image
            for (int pixelIndex = 0; pixelIndex < image.Pixels.Length; pixelIndex++)
            {
                byte intensity = (byte)image.Pixels[pixelIndex];
                histogramData[intensity]++;

                if (histogramData[intensity] > maxCount)
                {
                    maxCount = histogramData[intensity];
                }
            }

            // Figure out the average intensity
            long average = 0;
            for (int intensity = 0; intensity < 256; intensity++)
            {
                average += intensity * histogramData[intensity];
            }

            average /= image.Pixels.Length;

            for (int pixelIndex = 0; pixelIndex < image.Pixels.Length; pixelIndex++)
            {
                byte intensity = (byte)image.Pixels[pixelIndex];

                // Set the pixels greater than or equal to the average
                // to white and everything else to black
                if (intensity >= average)
                {
                    intensity = 255;
                    unchecked { binary.Pixels[pixelIndex] = (int)0xFFFFFFFF; }
                }
                else
                {
                    intensity = 0;
                    unchecked { binary.Pixels[pixelIndex] = (int)0xFF000000; }
                }
            }
            return binary;
        }
    }
}
