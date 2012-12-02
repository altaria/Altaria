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
        /// <summary>
        /// The dimensions of the originalbmp. transformedbmp should also have the same dimensions.
        /// </summary>
        public Int32[] dimensions    { get; private set; }
        /// <summary>
        /// Gets whether the image has been watermarked already.
        /// </summary>
        public bool isWatermarked    { get; private set; }

        //four subdomains of the transformed image by haar.
        public int[] hh   { get; private set; }
        public int max_hh { get; private set; }
        
        public int[] lh   { get; private set; }
        public int max_lh { get; private set; }
        
        public int[] hl   { get; private set; }
        public int max_hl { get; private set; }

        public int[] ll   { get; private set; }
        public int max_ll { get; private set; }

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

        public void HaarTransform()
        {
            int scale = 1;
            //scale is 1 by default
            int x, y, w = originalbmp.Width/scale, h = originalbmp.Height/scale;
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
            //Converting the bmp into 1D, via the 4 subdomains.
            this.hh = this.hl = this.lh = this.ll = new int[(w / 2) * (h / 2)];
            this.max_hh = this.max_hl = this.max_lh = this.max_ll = 0;
            //Since the image is grayscale, the range will be 0-255. (will the different color channels vary across the same grayscale?)
            
            //top left quarter of bmp is hh.
            int count = 0;
            for (int i = 0; i < w / 2; i++)
            {
                for (int j = 0; j < h / 2; j++)
                {
                    int value = this.transformedbmp.GetPixel(i, j).R;
                    hh[count] = value;
                    if (max_hh < value)
                        max_hh = value;
                    count++;
                }
            }
            
            //top right quarter of bmp is hl.
            count = 0;
            for (int i = w / 2; i < w; i++)
            {
                for (int j = 0; j < h / 2; j++)
                {
                    int value = this.transformedbmp.GetPixel(i, j).R;
                    hl[count] = value;
                    if (max_hl < value)
                        max_hl = value;
                    count++;
                }
            }
            //bottom left quarter of bmp is lh.
            count = 0;
            for (int i = 0; i < w / 2; i++)
            {
                for (int j = h / 2; j < h; j++)
                {
                    int value = this.transformedbmp.GetPixel(i, j).R;
                    lh[count] = value;
                    if (max_lh < value)
                        max_lh = value;
                    count++;
                }
            }
            //bottom right quarter of bmp is ll.
            count = 0;
            for (int i = w / 2; i < w; i++)
            {
                for (int j = h / 2; j < h; j++)
                {
                    int value = this.transformedbmp.GetPixel(i, j).R;
                    ll[count] = value;
                    if (max_ll < value)
                        max_ll = value;
                    count++;
                }
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