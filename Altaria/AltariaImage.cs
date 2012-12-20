using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;

namespace Altaria
{
    public class AltariaImage
    {
        //private const double alpha = 0.2;
        //private const double c0 = 0.7071067;
        //private const double c1 = 0.7071067;
        public Bitmap originalbmp    { get; private set; } 
        public Bitmap transformedbmp { get; private set; }
        public Int32[] dimensions    { get; private set; }
        public Bitmap watermarkedbmp { get; private set; }
        
        public bool is_transformed      { get; private set; }
        public bool is_watermarked      { get; private set; }
        //public double[,] original       { get; private set; }
        //private double[,] placeholder;
        //public double[,] transformed    { get; private set; }
        public string name              { get; private set; }

        public AltariaImage(Bitmap bmp, string name)
        {
            this.originalbmp = bmp; //original image
            this.dimensions = new Int32[2]{ bmp.Height, bmp.Width };
            this.name = name;
            this.is_watermarked = false;
        }
        /*
        public AltariaImage(Stream filestream, string filename)
        {
            Bitmap bmp = new Bitmap(filestream);
            filestream.Position = 0;
            name = filename;
            original = new double[bmp.Height, bmp.Width];
            placeholder = new double[bmp.Height, bmp.Width];
            transformed = new double[bmp.Height, bmp.Width];
            dimensions = new Int32[2] { bmp.Height, bmp.Width };
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                    original[i,j] = filestream.ReadByte();
            is_watermarked = false;
        }
        */
        /// <summary>
        /// Based on putmark.c
        /// </summary>
        /// <param name="level">The number of times to decompose.</param>
        /*public void NewHaarTransform(int level)
        {
            transformed = original;
            int ix, jy;
            int L = original.GetLength(0);
            int Lh = L / 2;
            while (level > 0)
            {
                for (int j = 0; j < L; j++)
                {
                    for (int i = 0; i <= L - 1; i += 2)
                    {
                        ix = i >> 1;
                        placeholder[ix, j] = c0 * transformed[i, j] + c1 * transformed[i + 1, j];
                        placeholder[ix + Lh, j] = c0 * transformed[i, j] - c1 * transformed[i + 1, j];
                    }
                }
                for (int i = 0; i < L; i++)
                {
                    for (int j = 0; j <= L - 1; j += 2)
                    {
                        jy = j >> 1;
                        transformed[i, jy] = c0 * placeholder[i, j] + c1 * placeholder[i, j + 1];
                        transformed[i, jy + Lh] = c0 * placeholder[i, j] - c1 * placeholder[i, j + 1];
                    }
                }
                --level;
                L = Lh;
                Lh = L / 2;
            }
        }*/
        /// <summary>
        /// Based on putmark.c
        /// </summary>
        /// <param name="level">The number of times to decompose</param>
        /*
        public void NewHaarRestore(int level)
        {
            int Lh = (int)(original.GetLength(0) / Math.Pow(2, level));
            int L = Lh * 2;
            int jy, ix;
            while (level > 0)
            {
                for (int i = 0; i < L; i++)
                    for (int j = 0; j < L; j += 2)
                    {
                        jy = j >> 1;
                        placeholder[i, j] = transformed[i,jy];
                        placeholder[i,j + 1] = transformed[i,jy + Lh];
                    }
                for (int i = 0; i < L; i++)
                    for (int j = 0; j < L; j += 2)
                    {
                        transformed[i,j] = c0 * placeholder[i,j] + c0 * placeholder[i,j + 1];
                        transformed[i,j + 1] = c1 * placeholder[i,j] - c1 * placeholder[i,j + 1];
                    }
                for (int j = 0; j < L; j++)
                    for (int i = 0; i < L; i += 2)
                    {
                        ix = i >> 1;
                        placeholder[i,j] = transformed[ix,j];
                        placeholder[i + 1,j] = transformed[ix + Lh,j];
                    }
                for (int j = 0; j < L; j++)
                    for (int i = 0; i < L; i += 2)
                    {
                        transformed[i,j] = c0 * placeholder[i,j] + c0 * placeholder[i + 1,j];
                        transformed[i + 1,j] = c1 * placeholder[i,j] - c1 * placeholder[i + 1,j];
                    }
                --level;
                Lh = L;
                L = Lh * 2;
            }
            byte[] data = new byte[transformed.Length];
            for (int i = 0; i < transformed.GetLength(0); i++)
                for (int j = 0; j < transformed.GetLength(1); j++)
                    data[i+j] = (byte)transformed[i,j];
            //convert the 2d array to image.
            /*System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Bgr32;
            int rawStride = (transformed.GetLength(1) * pf.BitsPerPixel + 7) / 8;
            BitmapSource bms = BitmapSource.Create(dimensions[1], dimensions[0], 96, 96, pf, null, data, rawStride);
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bms));
                enc.Save(ms);
                new Bitmap(ms).Save("C:\\temp\\watermarked.bmp");
            }
        }*/
        /*
        public void NewEmbedWatermark(AltariaImage wm)
        {
            int n3 = original.GetLength(0) / 8;
            int n2 = n3 * 2;
            for (int i = 0; i < n3; i++)
                for (int j = n3; j < n2; j++)
                {
                    transformed[i, j] *= (1 + alpha * wm.original[i, j - n3]);
                }
            for (int i = n3; i < n2; i++)
                for (int j = 0; j < n3; j++)
                {
                    transformed[i, j] *= (1.0 - alpha * wm.original[i - n3, j]);
                }
            is_watermarked = true;
        }*/
/* 
 * http://books.google.com.sg/books?id=IGtIWmM2GWIC&pg=PA164&lpg=PA164&dq=c%23+haar+transform&source=bl&ots=eCFYSo3h7R&sig=PX_k3bV4emlrHclPqhUzKN6M9qU&hl=en&sa=X&ei=C2GwUJ_NA82mrAf65oHYBg&ved=0CCwQ6AEwAA#v=onepage&q=c%23%20haar%20transform&f=false
 */

        public void HaarTransform(int level)
        {
            HaarTransform(null, level);
        }
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
                this.transformedbmp.Save("C:\\temp\\transformed.bmp");
            }
        }

        public void HaarRestore(int level)
        {
            HaarRestore(null, level);
        }
        /// <summary>
        /// Does a reverse of the Haar DWT.
        /// </summary>
        /// <param name="bmp">The bitmap to reverse.</param>
        /// <param name="level">The number of times to perform the inverse, depending on level of decomposition.</param>
        /// <returns>The restored bitmap from the watermarkedbmp.</returns>
        public void HaarRestore(Bitmap bbmp, int level)
        {
            int scale = level;
            Bitmap bmp = bbmp;
            if (bmp == null)
            {
                if (is_watermarked)
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
                if (is_watermarked)
                    bmp.Save("C:\\temp\\watermarked.bmp");
                else
                    bmp.Save("C:\\temp\\restored.bmp");
            }
        }
        /// <summary>
        /// Embeds the watermark into the transformed bmp.
        /// </summary>
        /// <param name="watermark">The watermark to embed</param>
        /// <param name="level">The level of subband to embed it in. This assumes that the bmp has already been transformed to the similar level.</param>
        public void EmbedWatermark(AltariaImage watermark, int level)
        {
            Bitmap b = new Bitmap(this.transformedbmp);
            Bitmap wm = new Bitmap(watermark.originalbmp);
            //find the lowest LH and HL, depending on level of decomposition.
    
            this.is_watermarked = true;
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
