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
        Bitmap bmp;
        Int32[] dimensions = new Int32[2];
        List<double> hh, hl, lh, ll; //four subdomains of the transformed image by haar.
        public string name;
        //constructor
        public AltariaImage(Bitmap bmp, string name)
        {
            this.bmp = bmp; //original image
            this.dimensions = new Int32[2]{ bmp.Height, bmp.Width };
            this.name = name;
        }

        //functions
        public bool isWatermarked()
        {
            return false;
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

        public Bitmap HaarTransform()
        {
            // scale is set to 1 as only one level of wavelet decomposition is done in this example.
            int x, y, w = bmp.Width, h = bmp.Height;
            int r1, g1, b1, r2, g2, b2;
            //samples from image
            Color s1, s2;

            Bitmap newbmp = new Bitmap(bmp);
            //Apply the 2D haar wavelet transform; perform one horizontal pass
            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x += 2)
                {
                    s1 = bmp.GetPixel(x, y); s2 = bmp.GetPixel(x + 1, y);
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
            bmp = newbmp;
            newbmp = new Bitmap(bmp);

            //perform one vertical pass
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = bmp.GetPixel(x, y); s2 = bmp.GetPixel(x, y + 1);
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

            bmp = newbmp;
            return bmp;
        }
    }
}