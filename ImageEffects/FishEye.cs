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
    public class FishEye
    {
        private float xscale,
                  yscale,
                  xshift,
                  yshift,
                  thresh = 1;
        private int[] s;

        public WriteableBitmap FishEyeTransform(int[,] input, int PixelWidth, int PixelHeight, float k)
        {
            WriteableBitmap result = new WriteableBitmap(PixelWidth, PixelHeight);
            float centerX = PixelWidth / 2; //center of distortion
            float centerY = PixelHeight / 2;

            int width = PixelWidth; //image bounds
            int height = PixelHeight;

            int[,] dst = new int[height, width]; //output pic

            xshift = calc_shift(0, centerX - 1, centerX, k);
            float newcenterX = width - centerX;
            float xshift_2 = calc_shift(0, newcenterX - 1, newcenterX, k);

            yshift = calc_shift(0, centerY - 1, centerY, k);
            float newcenterY = height - centerY;
            float yshift_2 = calc_shift(0, newcenterY - 1, newcenterY, k);

            xscale = (width - xshift - xshift_2) / width;
            yscale = (height - yshift - yshift_2) / height;

            for (int j = 0; j < PixelWidth; j++)
                for (int i = 0; i < PixelHeight; i++)
                {
                    // Save the original pixel
                    int origPixel = input[i, j];

                    // Determine the place of the modified pixel
                    if (Math.Sqrt(Math.Pow(i - centerX, 2) + (Math.Pow(j - centerY, 2))) <= 300)
                    {
                        float x = getRadialX((float)i, (float)j, centerX, centerY, k);
                        float y = getRadialY((float)i, (float)j, centerX, centerY, k);
                        sampleImage(input, height, width, x, y);
                        int color = (s[0] << 24) | (s[1] << 16) | (s[2] << 8) | s[3];
                        dst[i, j] = color;
                    }
                    else
                    {
                        dst[i, j] = origPixel;
                    }
                }

            int[] ARGB = new int[width * height];
            int idx = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    ARGB[idx++] = dst[i, j];
                }

            // Return resulted image
            ARGB.CopyTo(result.Pixels, 0);
            return result;
        }

        private void sampleImage(int[,] arr, int PixelWidth, int PixelHeight, float idx0, float idx1)
        {
            s = new int[4];
            if (idx0 < 0 || idx1 < 0 || idx0 > (PixelHeight - 1) || idx1 > (PixelWidth - 1))
            {
                s[0] = 0;
                s[1] = 0;
                s[2] = 0;
                s[3] = 0;
                return;
            }

            float idx0_fl = (float)Math.Floor(idx0);
            float idx0_cl = (float)Math.Ceiling(idx0);
            float idx1_fl = (float)Math.Floor(idx1);
            float idx1_cl = (float)Math.Ceiling(idx1);

            int[] s1 = getARGB(arr, (int)idx0_fl, (int)idx1_fl);
            int[] s2 = getARGB(arr, (int)idx0_fl, (int)idx1_cl);
            int[] s3 = getARGB(arr, (int)idx0_cl, (int)idx1_cl);
            int[] s4 = getARGB(arr, (int)idx0_cl, (int)idx1_fl);

            float x = idx0 - idx0_fl;
            float y = idx1 - idx1_fl;

            s[0] = (int)(s1[0] * (1 - x) * (1 - y) + s2[0] * (1 - x) * y + s3[0] * x * y + s4[0] * x * (1 - y));
            s[1] = (int)(s1[1] * (1 - x) * (1 - y) + s2[1] * (1 - x) * y + s3[1] * x * y + s4[1] * x * (1 - y));
            s[2] = (int)(s1[2] * (1 - x) * (1 - y) + s2[2] * (1 - x) * y + s3[2] * x * y + s4[2] * x * (1 - y));
            s[3] = (int)(s1[3] * (1 - x) * (1 - y) + s2[3] * (1 - x) * y + s3[3] * x * y + s4[3] * x * (1 - y));
        }

        private int[] getARGB(int[,] buf, int x, int y)
        {
            int rgb = buf[x, y];
            int[] scalar = new int[4];
            scalar[0] = (rgb >> 24) & 0xFF;
            scalar[1] = (rgb >> 16) & 0xFF;
            scalar[2] = (rgb >> 8) & 0xFF;
            scalar[3] = (rgb >> 0) & 0xFF;
            return scalar;
        }

        private float getRadialX(float x, float y, float cx, float cy, float k)
        {
            x = (x * xscale + xshift);
            y = (y * yscale + yshift);
            float res = x + ((x - cx) * k * ((x - cx) * (x - cx) + (y - cy) * (y - cy)));
            return res;
        }

        private float getRadialY(float x, float y, float cx, float cy, float k)
        {
            x = (x * xscale + xshift);
            y = (y * yscale + yshift);
            float res = y + ((y - cy) * k * ((x - cx) * (x - cx) + (y - cy) * (y - cy)));
            return res;
        }

        private float calc_shift(float x1, float x2, float cx, float k)
        {
            float x3 = (float)(x1 + (x2 - x1) * 0.5);
            float res1 = x1 + ((x1 - cx) * k * ((x1 - cx) * (x1 - cx)));
            float res3 = x3 + ((x3 - cx) * k * ((x3 - cx) * (x3 - cx)));

            if (res1 > -thresh && res1 < thresh)
                return x1;
            if (res3 < 0)
            {
                return calc_shift(x3, x2, cx, k);
            }
            else
            {
                return calc_shift(x1, x3, cx, k);
            }
        }
    }
}
