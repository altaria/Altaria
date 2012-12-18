﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Altaria
{
    public class AltariaImage
    {
        //variables
        ///<summary>The original bmp.</summary>
        public Bitmap originalbmp    { get; private set; } 
        /// <summary>
        /// The transformed bmp after HaarTransform.
        /// </summary>
        public Bitmap transformedbmp { get; private set; }
        public bool transformed = false;
        /// <summary>
        /// The dimensions of the originalbmp. transformedbmp should also have the same dimensions.
        /// </summary>
        public Int32[] dimensions    { get; private set; }

        /// <summary>
        /// The watermarkedbmp after EmbedWatermark
        /// </summary>
        public Bitmap watermarkedbmp { get; private set; }
        public bool watermarked    { get; private set; }

        //four subdomains of the transformed image by haar.
        public string name { get; private set; }
        //constructor
        public AltariaImage(Bitmap bmp, string name)
        {
            this.originalbmp = bmp; //original image
            this.dimensions = new Int32[2]{ bmp.Height, bmp.Width };
            this.name = name;
            this.watermarked = false;
        }
        
/* The Haar Transform is performed in two stages. First, each row is transformed. Then each column is transformed. The coefficients
 * that store the "averages" are moved into the first portion of the image while the detail coefficients remain on the outer portion
 * of the image. Since the averaged version is on the top left, further transforms can be applied directly to that quarter of the
 * image. Hence the use of a scale variable to determine which portion of the image to transform. Note that the image stored in the 
 * Bitmap bmp should have dimensions that are a power of 2.
 * 
 * http://books.google.com.sg/books?id=IGtIWmM2GWIC&pg=PA164&lpg=PA164&dq=c%23+haar+transform&source=bl&ots=eCFYSo3h7R&sig=PX_k3bV4emlrHclPqhUzKN6M9qU&hl=en&sa=X&ei=C2GwUJ_NA82mrAf65oHYBg&ved=0CCwQ6AEwAA#v=onepage&q=c%23%20haar%20transform&f=false
 *
 * *
 */

        /// <summary>
        /// Transforms the image with Haar DWT. 
        /// </summary>
        /// <param name="bmp">The bitmap to transform. If none is provided, originalbmp will be used.</param>
        /// <param name="level">The number of times to perform Haar Transform.</param>
        /// <param name="scale">The starting scale. Defaults to 1.</param>
        public void HaarTransform(Bitmap bmp, int level, int scale = 1)
        {
            Bitmap originalbmp = bmp;
            if (bmp == null)
            {
                originalbmp = this.originalbmp;
            }
            //scale should be 1 by default
            int x, y, w = originalbmp.Width / scale, h = originalbmp.Height / scale;
            int r1, g1, b1, r2, g2, b2;
            //samples from image
            Color s1, s2;

            Bitmap newbmp = new Bitmap(originalbmp);
            //Apply the 2D haar wavelet transform; perform one horizontal pass
            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x += 2)
                {
                    s1 = originalbmp.GetPixel(x, y); s2 = originalbmp.GetPixel(x + 1, y);
                    r2 = (s2.R - s1.R); g2 = (s2.G - s1.G); b2 = (s2.B - s1.B);

                    if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                    if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                    if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;

                    r1 = (s1.R - 128 + r2 / 2);
                    g1 = (s1.G - 128 + g2 / 2);
                    b1 = (s1.B - 128 + b2 / 2);

                    if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                    if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                    if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;

                    r1 += 128; g1 += 128; b1 += 128;
                    r2 += 128; g2 += 128; b2 += 128;

                    newbmp.SetPixel(x / 2, y, Color.FromArgb(r1, b1, g1));
                    newbmp.SetPixel(x / 2 + w / 2, y, Color.FromArgb(r2, b2, g2));
                }
            }
            originalbmp = newbmp;
            newbmp = new Bitmap(originalbmp);

            //perform one vertical pass
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = originalbmp.GetPixel(x, y); s2 = originalbmp.GetPixel(x, y + 1);
                    r2 = (s2.R - s1.R); g2 = (s2.G - s1.G); b2 = (s2.B - s1.B);

                    if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                    if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                    if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;

                    r1 = (s1.R - 128 + r2 / 2);
                    g1 = (s1.G - 128 + g2 / 2);
                    b1 = (s1.B - 128 + b2 / 2);

                    if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                    if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                    if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;

                    r1 += 128; g1 += 128; b1 += 128;
                    r2 += 128; g2 += 128; b2 += 128;

                    newbmp.SetPixel(x, y / 2, Color.FromArgb(r1, b1, g1));
                    newbmp.SetPixel(x, y / 2 + h / 2, Color.FromArgb(r2, b2, g2));

                }
            }
            this.transformedbmp = newbmp;
            if (level > scale)
            {
                //if level is more than scale, call function one more time.
                HaarTransform(transformedbmp, level, scale+1);
            }
            else
            {
                //save to test
                this.transformedbmp.Save("C:\\temp\\inverse.bmp");
            }
        }

        /// <summary>
        /// Does a reverse of the Haar DWT.
        /// </summary>
        /// <param name="bmp">The bitmap to reverse.</param>
        /// <param name="level">The number of times to perform the inverse, depending on level of decomposition.</param>
        /// <returns>The restored bitmap from the watermarkedbmp.</returns>
        public Bitmap HaarRestore(Bitmap bbmp, int level)
        {
            int scale = level;
            Bitmap bmp = bbmp;
            if (bmp == null)
            {
                if (watermarked)
                    bmp = this.watermarkedbmp;
                else
                    bmp = this.transformedbmp;
            }

            int x, y, w = bmp.Width/scale, h = bmp.Height/scale;
            int r1, g1, b1, r2, g2, b2;

            //samples from image
            Color s1, s2;
            Bitmap newbmp = new Bitmap(bmp);
           
            //perform one vertical pass
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = bmp.GetPixel(x, y / 2);
                    s2 = bmp.GetPixel(x, y / 2 + h / 2);
                    
                    r1 = s1.R - 128; g1 = s1.G - 128; b1 = s1.B - 128;
                    r2 = s2.R - 128; g2 = s2.G - 128; b2 = s2.B - 128;
                    r1 = (r1 - r2 / 2); g1 = (g1 - g2 / 2); b1 = (b1 - b2 / 2);

                    if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                    if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                    if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;
                    r2 = (r2 + r1); g2 = (g2 + g1); b2 = (b2 + b1);

                    if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                    if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                    if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;
                    r1 += 128; g1 += 128; b1 += 128;
                    r2 += 128; g2 += 128; b2 += 128;

                    newbmp.SetPixel(x, y, Color.FromArgb(r1, g1, b1));
                    newbmp.SetPixel(x, y + 1, Color.FromArgb(r2, g2, b2));
                }
            }
            bmp = newbmp;
            newbmp = new Bitmap(bmp);
            //Perform one horizontal pass
            for (y = 0; y < h; y ++)
            {
                for (x = 0; x < w; x+=2)
                {
                    s1 = bmp.GetPixel(x/2, y);
                    s2 = bmp.GetPixel(x/2 + w/2, y);

                    r1 = s1.R - 128; g1 = s1.G - 128; b1 = s1.B - 128;
                    r2 = s2.R - 128; g2 = s2.G - 128; b2 = s2.B - 128;
                    r1 = (r1 - r2 / 2); g1 = (g1 - g2 / 2); b1 = (b1 - b2 / 2);

                    if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                    if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                    if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;
                    r2 = (r2 + r1); g2 = (g2 + g1); b2 = (b2 + b1);

                    if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                    if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                    if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;
                    r1 += 128; g1 += 128; b1 += 128;
                    r2 += 128; g2 += 128; b2 += 128;

                    newbmp.SetPixel(x, y, Color.FromArgb(r1, g1, b1));
                    newbmp.SetPixel(x+1, y, Color.FromArgb(r2, g2, b2));
                }
            }
            bmp = newbmp;
            if (level > 1)
            {
                //if level is more than scale, call function one more time.
                HaarRestore(bmp, level - 1);
            }
            else
            {
                //save to test
                bmp.Save("C:\\temp\\restore.bmp");
            }
            return bmp;
        }
        public int[] Reshape()
        {
            //Normalize the image to 0 and 1
            Color c;
            int[] reshaped_image = new int[originalbmp.Height * originalbmp.Width];
            int count = 0;
            for (int y = 0; y < originalbmp.Height; y++)
            {
                for (int x = 0; x < originalbmp.Width; x++)
                {
                    c = originalbmp.GetPixel(x, y);
                    int total = c.R + c.G + c.B;
                    //255*3=765
                    if (total >= 765 / 2)
                    {
                        //set int to 1. (white color)
                        reshaped_image[count] = 1;
                    }
                    else
                    {
                        //set int to 0. (black color)
                        reshaped_image[count] = 0;
                    }
                    count++;
                }
            }
            return reshaped_image;
        }

        public void EmbedWatermark(AltariaImage watermark)
        {
                Bitmap b = new Bitmap(this.transformedbmp);
                Bitmap wm = new Bitmap(watermark.originalbmp);
                //double alpha = 0.2;
                /*
                 * the below code is based on putmark.c
                */
                int n3 = b.Height / 8;
                int n2 = n3 * 2;
                int color = 0;
                for (int i = 0; i < n3; i++)
                    for (int j = n3; j < n2; j++){
                        //color = (int)(b.GetPixel(i, j).R * (1 + alpha * (double)(wm.GetPixel(i,j - n3).R)));
                        color = wm.GetPixel(i, j - n3).R;
                        b.SetPixel(i, j, Color.FromArgb(color, color, color));
                    }
                for (int i = n3; i < n2; i++)
                    for (int j = 0; j < n3; j++)
                    {
                        //color = (int)(b.GetPixel(i,j).R * (1 - alpha * (double)(wm.GetPixel(i - n3, j).R)));
                        color = wm.GetPixel(i - n3, j).R;
                        b.SetPixel(i, j, Color.FromArgb(color, color, color));
                    }

                this.watermarked = true;
                this.watermarkedbmp = b;
        }
        /// <summary>
        /// Function to obtain the Peak Signal-to-Noise Ratio. A higher value would normally indicate that the reconstruction
        /// is of a higher quality. It is an approximation to human perception of reconstruction quality.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="wm"></param>
        /// <returns></returns>
        public static double PSNR(Bitmap normal, Bitmap wm){
            int m = normal.Height;
            int n = normal.Width;
            //MSE = mean square error; it is for 2 m*n monochrome images I and K where one image is considered a noisy
            //approximation of the other
            double MSE = 0;
            int MAX = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    MSE += Math.Pow((normal.GetPixel(j, i).R - wm.GetPixel(j, i).R),2);
                    if (normal.GetPixel(j, i).R > MAX)
                        MAX = normal.GetPixel(j,i).R;
                    if (wm.GetPixel(j,i).R > MAX)
                        MAX = wm.GetPixel(j,i).R;
                }
            }
            MSE *= 1 / (m * n);
            double PSNR = 20 * Math.Log10(MAX / Math.Sqrt(MSE));
            return PSNR;
        }

        /// <summary>
        /// Structural Similarity Index Measure; measures the similarity between two images. A value of 1 means that the images
        /// are exactly the same.
        /// </summary>
        /// <param name="normal">The original image</param>
        /// <param name="wm">The watermarked image</param>
        /// <returns>A value between -1 and 1.</returns>
        public static double SSIM(Bitmap normal, Bitmap wm)
        {
            //normal and wm has to be the same image with the dimensions n*n.
            if (normal.Height == normal.Width && wm.Height == wm.Width && normal.Height == wm.Height)
            {
                double sum_normal = 0, sum_wm = 0, mean_normal, mean_wm, variance_normal, variance_wm, covariance = 0;
                double L = Math.Pow(2, 8) - 1;
                double k1 = 0.01;
                double k2 = 0.03;
                double c1 = Math.Pow(k1 * L, 2);
                double c2 = Math.Pow(k2 * L, 2);
                int n = normal.Height;

                //find sum
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        sum_normal += normal.GetPixel(i, j).R;
                        sum_wm += wm.GetPixel(i, j).R;
                    }
                }
                mean_normal = sum_normal / (n * n);
                mean_wm = sum_wm / (n * n);

                //find variance and covariance
                double diff_normal_squared = 0, diff_wm_squared = 0;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        diff_normal_squared += Math.Pow((normal.GetPixel(i, j).R - mean_normal), 2);
                        diff_wm_squared += Math.Pow((wm.GetPixel(i, j).R - mean_wm), 2);
                        covariance += ((normal.GetPixel(i, j).R - mean_normal) * (wm.GetPixel(i, j).R - mean_wm))/(n*n);
                    }
                }
                variance_normal = diff_normal_squared / (n * n);
                variance_wm = diff_wm_squared / (n * n);
                return (((2 * mean_normal * mean_wm + c1) * (2 * covariance + c2)) / ((mean_normal * mean_normal + mean_wm * mean_wm + c1)*(variance_normal + variance_wm + c2)));
            }
            else
            {
                throw new InvalidOperationException("The images have to be of the same dimensions of n*n.");
            }
            
        }
    }

}
