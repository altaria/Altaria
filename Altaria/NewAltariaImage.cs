using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Altaria
{
    /// <summary>
    /// Improvement over the old AltariaImage. Uses a colored RGB plane instead of grayscale.
    /// According to this paper at this website
    /// http://arxiv.org/ftp/arxiv/papers/1208/1208.0803.pdf
    /// </summary>
    public class NewAltariaImage
    {
        public Bitmap originalbmp { get; private set; }
        
        //transformed color planes
        public Bitmap r_plane { get; private set; }
        public Bitmap g_plane { get; private set; }
        public Bitmap b_plane { get; private set; }

        //embedded color planes
        public Bitmap er_plane { get; private set; }
        public Bitmap eg_plane { get; private set; }
        public Bitmap eb_plane { get; private set; }

        //restored color planes
        public Bitmap rr_plane { get; private set; }
        public Bitmap rg_plane { get; private set; }
        public Bitmap rb_plane { get; private set; }

        //concatentated bitmap from the restored color planes.
        public Bitmap concatbmp { get; private set; }

        public int Height { get; private set; }
        public int Width { get; private set; }
        public string Name { get; private set; }
        private bool transformed = false;
        public bool watermarked { get; private set; }

        public NewAltariaImage(Bitmap bmp, string name)
        {
            watermarked = false;
            originalbmp = bmp;
            Name = name;
            Height = bmp.Height;
            Width = bmp.Width;
        }

        public bool IsTransformed()
        {
            return transformed;
        }
        /// <summary>
        /// Transforms the image with Haar DWT. Only transform one level.
        /// </summary>
        public void HaarTransform()
        {
            //scale should be 1 by default
            //1,2,4,8,16
            int x, y, w = originalbmp.Width, h = originalbmp.Height;
            int r1, g1, b1, r2, g2, b2;
            //samples from image
            Color s1, s2;

            Bitmap newbmp_r = new Bitmap(originalbmp);
            Bitmap newbmp_g = new Bitmap(originalbmp);
            Bitmap newbmp_b = new Bitmap(originalbmp);
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

                    newbmp_r.SetPixel(x / 2, y, Color.FromArgb(r1, r1, r1));
                    newbmp_r.SetPixel(x / 2 + w / 2, y, Color.FromArgb(r2, r2, r2));
                    newbmp_g.SetPixel(x / 2, y, Color.FromArgb(g1, g1, g1));
                    newbmp_g.SetPixel(x / 2 + w / 2, y, Color.FromArgb(g2, g2, g2));
                    newbmp_b.SetPixel(x / 2, y, Color.FromArgb(b1, b1, b1));
                    newbmp_b.SetPixel(x / 2 + w / 2, y, Color.FromArgb(b2, b2, b2));
                }
            }
            //for each derived planes, perform one vertical pass
            Bitmap placeholder_r = newbmp_r;
            newbmp_r = new Bitmap(placeholder_r);
            //red
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = placeholder_r.GetPixel(x, y); s2 = placeholder_r.GetPixel(x, y + 1);
                    r2 = (s2.R - s1.R);
                    if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                    r1 = (s1.R - 128 + r2 / 2);
                    if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;

                    r1 += 128;
                    r2 += 128;

                    newbmp_r.SetPixel(x, y / 2, Color.FromArgb(r1, r1, r1));
                    newbmp_r.SetPixel(x, y / 2 + h / 2, Color.FromArgb(r2, r2, r2));
                }
            }
            Bitmap placeholder_g = newbmp_g;
            newbmp_g = new Bitmap(placeholder_g);
            //green
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = placeholder_g.GetPixel(x, y); s2 = placeholder_g.GetPixel(x, y + 1);
                    g2 = (s2.G - s1.G);
                    if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                    g1 = (s1.G - 128 + g2 / 2);
                    if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                    g1 += 128; 
                    g2 += 128; 

                    newbmp_g.SetPixel(x, y / 2, Color.FromArgb(g1, g1, g1));
                    newbmp_g.SetPixel(x, y / 2 + h / 2, Color.FromArgb(g2, g2, g2));
                }
            }
            Bitmap placeholder_b = newbmp_b;
            newbmp_b = new Bitmap(placeholder_b);
            //blue
            for (y = 0; y < h; y += 2)
            {
                for (x = 0; x < w; x++)
                {
                    s1 = placeholder_b.GetPixel(x, y); s2 = placeholder_b.GetPixel(x, y + 1);
                    b2 = (s2.B - s1.B);
                    if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;
                    b1 = (s1.B - 128 + b2 / 2);
                    if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;

                    b1 += 128;
                    b2 += 128;

                    newbmp_b.SetPixel(x, y / 2, Color.FromArgb(b1, b1, b1));
                    newbmp_b.SetPixel(x, y / 2 + h / 2, Color.FromArgb(b2, b2, b2));
                }
            }
            r_plane = newbmp_r;
            g_plane = newbmp_g;
            b_plane = newbmp_b;
            //save to test
            r_plane.Save("C:\\temp\\transformed_r_" + Name + ".bmp");
            g_plane.Save("C:\\temp\\transformed_g_" + Name + ".bmp");
            b_plane.Save("C:\\temp\\transformed_b_" + Name + ".bmp");
            transformed = true;
        }
        /// <summary>
        /// Does a reverse of the Haar DWT on the color planes. Will only do first level composition.
        /// </summary>
        public void HaarRestore()
        {
            if (transformed)
            {
                Bitmap rbmp, gbmp, bbmp;
                if (watermarked)
                {
                    rbmp = er_plane;
                    gbmp = eg_plane;
                    bbmp = eb_plane;
                }
                else
                {
                    rbmp = r_plane;
                    gbmp = g_plane;
                    bbmp = b_plane;
                }
                //bitmap has been transformed. restoring is possible
                int x, y, w = rbmp.Width, h = rbmp.Height; //dimensions should be the same across the planes
                int r1, g1, b1, r2, g2, b2;

                //samples from image
                Color s1, s2;
                Bitmap newbmp_r = new Bitmap(rbmp);
                Bitmap newbmp_g = new Bitmap(gbmp);
                Bitmap newbmp_b = new Bitmap(bbmp);

                //perform one vertical pass for red
                for (y = 0; y < h; y += 2)
                {
                    for (x = 0; x < w; x++)
                    {
                        s1 = rbmp.GetPixel(x, y / 2);
                        s2 = rbmp.GetPixel(x, y / 2 + h / 2);
                        r1 = s1.R - 128;
                        r2 = s2.R - 128;
                        r1 = (r1 - r2 / 2);
                        if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                        r2 = (r2 + r1);
                        if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                        r1 += 128;
                        r2 += 128;
                        newbmp_r.SetPixel(x, y, Color.FromArgb(r1, r1, r1));
                        newbmp_r.SetPixel(x, y + 1, Color.FromArgb(r2, r2, r2));
                    }
                }
                rbmp = newbmp_r;
                newbmp_r = new Bitmap(rbmp);

                //perform one vertical pass for green
                for (y = 0; y < h; y += 2)
                {
                    for (x = 0; x < w; x++)
                    {
                        s1 = gbmp.GetPixel(x, y / 2);
                        s2 = gbmp.GetPixel(x, y / 2 + h / 2);
                        g1 = s1.G - 128;
                        g2 = s2.G - 128;
                        g1 = (g1 - g2 / 2);
                        if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                        g2 = (g2 + g1);
                        if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                        g1 += 128;
                        g2 += 128;
                        newbmp_g.SetPixel(x, y, Color.FromArgb(g1, g1, g1));
                        newbmp_g.SetPixel(x, y + 1, Color.FromArgb(g2, g2, g2));
                    }
                }
                gbmp = newbmp_g;
                newbmp_g = new Bitmap(gbmp);

                //perform one vertical pass for blue
                for (y = 0; y < h; y += 2)
                {
                    for (x = 0; x < w; x++)
                    {
                        s1 = bbmp.GetPixel(x, y / 2);
                        s2 = bbmp.GetPixel(x, y / 2 + h / 2);
                        b1 = s1.B - 128;
                        b2 = s2.B - 128;
                        b1 = (b1 - b2 / 2);
                        if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;
                        b2 = (b2 + b1);
                        if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;
                        b1 += 128;
                        b2 += 128;
                        newbmp_b.SetPixel(x, y, Color.FromArgb(b1, b1, b1));
                        newbmp_b.SetPixel(x, y + 1, Color.FromArgb(b2, b2, b2));
                    }
                }
                bbmp = newbmp_b;
                newbmp_b = new Bitmap(bbmp);

                //Perform one horizontal pass for red
                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x += 2)
                    {
                        s1 = rbmp.GetPixel(x / 2, y);
                        s2 = rbmp.GetPixel(x / 2 + w / 2, y);

                        r1 = s1.R - 128; 
                        r2 = s2.R - 128;
                        r1 = (r1 - r2 / 2);

                        if (r1 < -128) r1 += 256; if (r1 > 127) r1 -= 256;
                        r2 = (r2 + r1); 

                        if (r2 < -128) r2 += 256; if (r2 > 127) r2 -= 256;
                        r1 += 128;
                        r2 += 128;

                        newbmp_r.SetPixel(x, y, Color.FromArgb(r1, r1, r1));
                        newbmp_r.SetPixel(x + 1, y, Color.FromArgb(r2, r2, r2));
                    }
                }
                //Perform one horizontal pass for green
                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x += 2)
                    {
                        s1 = gbmp.GetPixel(x / 2, y);
                        s2 = gbmp.GetPixel(x / 2 + w / 2, y);

                        g1 = s1.G - 128;
                        g2 = s2.G - 128;
                        g1 = (g1 - g2 / 2);

                        if (g1 < -128) g1 += 256; if (g1 > 127) g1 -= 256;
                        g2 = (g2 + g1);

                        if (g2 < -128) g2 += 256; if (g2 > 127) g2 -= 256;
                        g1 += 128;
                        g2 += 128;

                        newbmp_g.SetPixel(x, y, Color.FromArgb(g1, g1, g1));
                        newbmp_g.SetPixel(x + 1, y, Color.FromArgb(g2, g2, g2));
                    }
                }
                //Perform one horizontal pass for blue
                for (y = 0; y < h; y++)
                {
                    for (x = 0; x < w; x += 2)
                    {
                        s1 = bbmp.GetPixel(x / 2, y);
                        s2 = bbmp.GetPixel(x / 2 + w / 2, y);

                        b1 = s1.B - 128;
                        b2 = s2.B - 128;
                        b1 = (b1 - b2 / 2);

                        if (b1 < -128) b1 += 256; if (b1 > 127) b1 -= 256;
                        b2 = (b2 + b1);

                        if (b2 < -128) b2 += 256; if (b2 > 127) b2 -= 256;
                        b1 += 128;
                        b2 += 128;

                        newbmp_b.SetPixel(x, y, Color.FromArgb(b1, b1, b1));
                        newbmp_b.SetPixel(x + 1, y, Color.FromArgb(b2, b2, b2));
                    }
                }
                    rr_plane = newbmp_r;
                    rg_plane = newbmp_g;
                    rb_plane = newbmp_b;

                    //write to files
                    rr_plane.Save("C:\\temp\\restored_r_" + Name + ".bmp");
                    rg_plane.Save("C:\\temp\\restored_g_" + Name + ".bmp");
                    rb_plane.Save("C:\\temp\\restored_b_" + Name + ".bmp");
            }
            else
            {
                //...
            }
        }
        
        /// <summary>
        /// Embeds a watermark to the planes
        /// </summary>
        /// <param name="wm">The watermark to embed</param>
        /// <param name="alpha">alpha. defaults to 0.9.</param>
        public void EmbedWatermark(NewAltariaImage wm, double alpha = 0.9)
        {
            if (wm.IsTransformed())
            {
                // SII=alpha*(CI) + (1.0-alpha)*(SI)
                //alpha ranges from 0.0 to 1.0
                Bitmap rbmp = new Bitmap(wm.r_plane);
                Bitmap gbmp = new Bitmap(wm.g_plane);
                Bitmap bbmp = new Bitmap(wm.b_plane);

                this.er_plane = new Bitmap(Width, Height);
                this.eg_plane = new Bitmap(Width, Height);
                this.eb_plane = new Bitmap(Width, Height);
                
                double finalpixel = 0;
                //Final pixel = alpha * (First image's source pixel) + (1.0-alpha) * (Second image's source pixel)
                
                //embed r_plane into all planes
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * r_plane.GetPixel(i, j).R + (1.0 - alpha) * rbmp.GetPixel(i, j).R;
                        er_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                //embed g_plane into all planes
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * g_plane.GetPixel(i, j).G + (1.0 - alpha) * gbmp.GetPixel(i, j).G;
                        eg_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                //embed b_plane into all planes
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * b_plane.GetPixel(i, j).B + (1.0 - alpha) * bbmp.GetPixel(i, j).B;
                        eb_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                
                //embed r_plane into lh and hl
                /*
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < Height / 2; j++) //hl
                    {
                        finalpixel = alpha * r_plane.GetPixel(i, j).R + (1.0 - alpha) * rbmp.GetPixel(i, j).R;
                        er_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = Height / 2; j < Height; j++) //hh
                    {
                        er_plane.SetPixel(i, j, r_plane.GetPixel(i, j));
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = Height / 2; j < Height; j++) //lh
                    {
                        finalpixel = alpha * r_plane.GetPixel(i, j).R + (1.0 - alpha) * rbmp.GetPixel(i, j).R;
                        er_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = 0; j < Height / 2; j++) //ll
                    {
                        er_plane.SetPixel(i, j, r_plane.GetPixel(i, j));
                    }
                }
                //embed g_plane into lh and hl
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < Height / 2; j++) //hl
                    {
                        finalpixel = alpha * g_plane.GetPixel(i, j).G + (1.0 - alpha) * gbmp.GetPixel(i, j).G;
                        eg_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = Height / 2; j < Height; j++) //hh
                    {
                        eg_plane.SetPixel(i, j, g_plane.GetPixel(i, j));
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = Height / 2; j < Height; j++) //lh
                    {
                        finalpixel = alpha * g_plane.GetPixel(i, j).G + (1.0 - alpha) * gbmp.GetPixel(i, j).G;
                        eg_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = 0; j < Height / 2; j++) //ll
                    {
                        eg_plane.SetPixel(i, j, g_plane.GetPixel(i, j));
                    }
                }
                //embed b_plane into lh and hl
                for (int i = Width / 2; i < Width; i++)
                {
                    for (int j = 0; j < Height / 2; j++) //hl
                    {
                        finalpixel = alpha * b_plane.GetPixel(i, j).B + (1.0 - alpha) * bbmp.GetPixel(i, j).B;
                        eb_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = Height / 2; j < Height; j++) //hh
                    {
                        eb_plane.SetPixel(i, j, b_plane.GetPixel(i, j));
                    }
                }
                for (int i = 0; i < Width / 2; i++)
                {
                    for (int j = Height / 2; j < Height; j++) //lh
                    {
                        finalpixel = alpha * b_plane.GetPixel(i, j).B + (1.0 - alpha) * rbmp.GetPixel(i, j).B;
                        eb_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    for (int j = 0; j < Height / 2; j++) //ll
                    {
                        eb_plane.SetPixel(i, j, b_plane.GetPixel(i, j));
                    }
                }*/
                watermarked = true;
            }
        }

        /// <summary>
        /// Concatenate the color planes to restore the colored image.
        /// </summary>
        public void ConcatPlanes()
        {
            concatbmp = new Bitmap(Width, Height);
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    concatbmp.SetPixel(j, i, Color.FromArgb(rr_plane.GetPixel(j, i).R, rg_plane.GetPixel(j, i).G, rb_plane.GetPixel(j, i).B));
            if (watermarked)
                concatbmp.Save("C:\\temp\\concated_wm_" + Name + ".bmp");
            else
                concatbmp.Save("C:\\temp\\concated_" + Name + ".bmp");
        }
    }
}