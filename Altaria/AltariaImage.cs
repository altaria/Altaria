using System;
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
        /// Gets whether the image has been watermarked already.
        /// </summary>
        public bool isWatermarked    { get; private set; }

        //four subdomains of the transformed image by haar.
        public string name { get; private set; }
        //constructor
        public AltariaImage(Bitmap bmp, string name)
        {
            this.originalbmp = bmp; //original image
            this.dimensions = new Int32[2]{ bmp.Height, bmp.Width };
            this.name = name;
            this.isWatermarked = false;
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
                this.transformedbmp.Save("C:\\temp\\asdf.bmp");
            }
        }

        //Reverse of HaarTransform. Returns a bitmap to prevent overriding of original bmp.
        public Bitmap HaarRestore()
        {
            Bitmap bmp = this.transformedbmp;
            int scale = 1;
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
    }

}
