using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using AForge.Imaging.Filters;

namespace Altaria
{
    /// <summary>
    /// Improvement over the old AltariaImage. Uses a colored RGB plane instead of grayscale.
    /// According to this paper at this website
    /// http://arxiv.org/ftp/arxiv/papers/1208/1208.0803.pdf
    /// </summary>
    public class AltariaImage
    {
        public Bitmap originalbmp { get; private set; }
        private const double alpha = 0.9;
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

        public AltariaImage(Bitmap bmp, string name)
        {
            watermarked = false;
            originalbmp = bmp;
            Name = name;
            Height = bmp.Height;
            Width = bmp.Width;

            er_plane = new Bitmap(Width, Height);
            eg_plane = new Bitmap(Width, Height);
            eb_plane = new Bitmap(Width, Height);
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
            //r_plane.Save("C:\\temp\\transformed_r_" + Name + ".bmp");
            //g_plane.Save("C:\\temp\\transformed_g_" + Name + ".bmp");
            //b_plane.Save("C:\\temp\\transformed_b_" + Name + ".bmp");
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
                //rr_plane.Save("C:\\temp\\restored_r_" + Name + ".bmp");
                rg_plane.Save("C:\\temp\\restored_g_" + Name + ".bmp");
                //rb_plane.Save("C:\\temp\\restored_b_" + Name + ".bmp");
            }
            else
            {
                //...
            }
        }

        /// <summary>
        /// Embeds a watermark to the planes with alpha blending.
        /// </summary>
        /// <param name="wm">The watermark to embed</param>
        /// <param name="alpha">alpha. defaults to 0.9.</param>
        public void AlphaBlend(AltariaImage wm, double alpha = 0.9)
        {
            if (wm.IsTransformed())
            {
                // SII=alpha*(CI) + (1.0-alpha)*(SI)
                //alpha ranges from 0.0 to 1.0
                Bitmap rbmp = new Bitmap(wm.r_plane);
                Bitmap gbmp = new Bitmap(wm.g_plane);
                Bitmap bbmp = new Bitmap(wm.b_plane);

                double finalpixel = 0;
                //Final pixel = alpha * (First image's source pixel) + (1.0-alpha) * (Second image's source pixel)

                //embed r_plane into all sub bands
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * r_plane.GetPixel(i, j).R + (1.0 - alpha) * rbmp.GetPixel(i, j).R;
                        er_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                //embed g_plane into all sub bands
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * g_plane.GetPixel(i, j).G + (1.0 - alpha) * gbmp.GetPixel(i, j).G;
                        eg_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                //embed b_plane into all sub bands
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        finalpixel = alpha * b_plane.GetPixel(i, j).B + (1.0 - alpha) * bbmp.GetPixel(i, j).B;
                        eb_plane.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarked = true;
            }
        }
        /// <summary>
        /// Concatenate the color planes to restore the colored image.
        /// </summary>
        public void ConcatPlanes()
        {
            concatbmp = new Bitmap(Width, Height);
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    concatbmp.SetPixel(i, j, Color.FromArgb(rr_plane.GetPixel(i, j).R, rg_plane.GetPixel(i, j).G, rb_plane.GetPixel(i, j).B));
            //if (watermarked)
            //    concatbmp.Save("C:\\temp\\concated_wm_" + Name + ".bmp");
            //else
            //    concatbmp.Save("C:\\temp\\concated_" + Name + ".bmp");
        }

        /// <summary>
        /// Alpha Blending for all planes, for lh and hl subbands, but embedding with full watermark (non transformed).
        /// The watermark pixels will be spread out with a formula and is grayscale
        /// <param name="alpha">specifies the alpha. defaults to alpha of AltariaImage.</param>
        /// <param name="random">specifies whether pixel placement follows an algorithm.</param>
        /// <param name="allplanes">specifies whether the watermark will be added into all the planes. defaults to false.</param>
        /// </summary>
        public void AdvancedAlphaBlend(double alpha = alpha, bool random = false, bool allplanes = false)
        {
            //obtain the watermark from the file system
            double finalpixel = 0;
            Bitmap wm = Bitmap.FromFile("C:\\temp\\requiredFiles\\wm.bmp") as Bitmap;
            //get the four subbands of the planes as individual bitmaps

            Bitmap ll_r, hl_r, lh_r, hh_r;
            Bitmap ll_g, hl_g, lh_g, hh_g;
            Bitmap ll_b, hl_b, lh_b, hh_b;
            Rectangle ll_crop = new Rectangle(0, 0, Width / 2, Height / 2);
            Rectangle hl_crop = new Rectangle(Width / 2, 0, Width / 2, Height / 2);
            Rectangle lh_crop = new Rectangle(0, Height / 2, Width / 2, Height / 2);
            Rectangle hh_crop = new Rectangle(Width / 2, Height / 2, Width / 2, Height / 2);

            // r_plane
            ll_r = r_plane.Clone(ll_crop, r_plane.PixelFormat);
            hl_r = r_plane.Clone(hl_crop, r_plane.PixelFormat);
            lh_r = r_plane.Clone(lh_crop, r_plane.PixelFormat);
            hh_r = r_plane.Clone(hh_crop, r_plane.PixelFormat);

            // g_plane
            ll_g = g_plane.Clone(ll_crop, g_plane.PixelFormat);
            hl_g = g_plane.Clone(hl_crop, g_plane.PixelFormat);
            lh_g = g_plane.Clone(lh_crop, g_plane.PixelFormat);
            hh_g = g_plane.Clone(hh_crop, g_plane.PixelFormat);

            // r_plane
            ll_b = b_plane.Clone(ll_crop, b_plane.PixelFormat);
            hl_b = b_plane.Clone(hl_crop, b_plane.PixelFormat);
            lh_b = b_plane.Clone(lh_crop, b_plane.PixelFormat);
            hh_b = b_plane.Clone(hh_crop, b_plane.PixelFormat);

            if (!random)
            {
                //embed
                for (int i = 0; i < wm.Width; i++)
                    for (int j = 0; j < wm.Height; j++)
                    {
                        //hl
                        finalpixel = alpha * hl_r.GetPixel(i, j).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                        hl_r.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                        finalpixel = alpha * hl_g.GetPixel(i, j).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                        hl_g.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                        finalpixel = alpha * hl_b.GetPixel(i, j).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                        hl_b.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));

                        //lh
                        finalpixel = alpha * lh_r.GetPixel(i, j).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                        lh_r.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                        finalpixel = alpha * lh_g.GetPixel(i, j).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                        lh_g.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                        finalpixel = alpha * lh_b.GetPixel(i, j).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                        lh_b.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));

                        if (allplanes)
                        {
                            //ll
                            finalpixel = alpha * ll_r.GetPixel(i, j).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                            ll_r.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                            finalpixel = alpha * ll_g.GetPixel(i, j).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                            ll_g.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                            finalpixel = alpha * ll_b.GetPixel(i, j).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                            ll_b.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                            //hh
                            finalpixel = alpha * hh_r.GetPixel(i, j).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                            hh_r.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                            finalpixel = alpha * hh_g.GetPixel(i, j).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                            hh_g.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                            finalpixel = alpha * hh_b.GetPixel(i, j).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                            hh_b.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                        }
                    }
            }
            else
            {
                //determine the position of placement based on maximum coefficient of original image
                int max_hl_r = 0, max_hl_g = 0, max_hl_b = 0;
                int max_lh_r = 0, max_lh_g = 0, max_lh_b = 0;
                for (int i = 0; i < hl_r.Width; i++)
                    for (int j = 0; j < hl_r.Height; j++)
                    {
                        int temp = hl_r.GetPixel(i, j).R;
                        //oh yeah ternary operations because I am lazy
                        //hl
                        max_hl_r = temp > max_hl_r ? temp : max_hl_r;
                        temp = hl_g.GetPixel(i, j).G;
                        max_hl_g = temp > max_hl_g ? temp : max_hl_g;
                        temp = hl_b.GetPixel(i, j).B;
                        max_hl_b = temp > max_hl_b ? temp : max_hl_b;
                        //lh
                        temp = lh_r.GetPixel(i, j).R;
                        max_lh_r = temp > max_lh_r ? temp : max_lh_r;
                        temp = lh_g.GetPixel(i, j).G;
                        max_lh_g = temp > max_lh_g ? temp : max_lh_g;
                        temp = lh_b.GetPixel(i, j).B;
                        max_lh_b = temp > max_lh_b ? temp : max_lh_b;
                    }
                //setting the watermark
                for (int i = 0, ii = hl_r.Width - wm.Width; i < wm.Width; i++, ii++)
                {
                    //hl_r
                    for (int j = 0, start = max_hl_r; j < wm.Height; j++, start++)
                    {
                        int s = start % hl_r.Width;
                        finalpixel = alpha * hl_r.GetPixel(i, s).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                        hl_r.SetPixel(i, s, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    //hl_g
                    for (int j = 0, start = max_hl_g; j < wm.Height; j++, start++)
                    {
                        int s = start % hl_g.Width;
                        finalpixel = alpha * hl_g.GetPixel(ii, s).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                        hl_g.SetPixel(ii, s, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    //hl_b
                    for (int j = 0, jj = hl_b.Height - wm.Height, start = max_hl_b; j < wm.Height; j++, start++, jj++)
                    {
                        finalpixel = alpha * hl_b.GetPixel(i, jj).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                        hl_b.SetPixel(i, jj, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }

                    //lh_r
                    for (int j = 0, start = max_lh_r; j < wm.Height; j++, start++)
                    {
                        int s = start % lh_r.Width;
                        finalpixel = alpha * lh_r.GetPixel(i, s).R + (1.0 - alpha) * wm.GetPixel(i, j).R;
                        lh_r.SetPixel(i, s, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    //lh_g
                    for (int j = 0, start = max_lh_g; j < wm.Height; j++, start++)
                    {
                        int s = start % lh_g.Width;
                        finalpixel = alpha * lh_g.GetPixel(ii, s).G + (1.0 - alpha) * wm.GetPixel(i, j).G;
                        lh_g.SetPixel(ii, s, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    //lh_b
                    for (int j = 0, jj = lh_b.Height - wm.Height, start = max_lh_b; j < wm.Height; j++, start++, jj++)
                    {
                        finalpixel = alpha * lh_b.GetPixel(i, jj).B + (1.0 - alpha) * wm.GetPixel(i, j).B;
                        lh_b.SetPixel(i, jj, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                }
            }

            //merge the subbands back
            using (Graphics g = Graphics.FromImage(er_plane))
            {
                g.DrawImage(ll_r, 0, 0);
                g.DrawImage(hl_r, Width / 2, 0);
                g.DrawImage(lh_r, 0, Height / 2);
                g.DrawImage(hh_r, Width / 2, Height / 2);
            }

            using (Graphics g = Graphics.FromImage(eg_plane))
            {
                g.DrawImage(ll_g, 0, 0);
                g.DrawImage(hl_g, Width / 2, 0);
                g.DrawImage(lh_g, 0, Height / 2);
                g.DrawImage(hh_g, Width / 2, Height / 2);
            }

            using (Graphics g = Graphics.FromImage(eb_plane))
            {
                g.DrawImage(ll_b, 0, 0);
                g.DrawImage(hl_b, Width / 2, 0);
                g.DrawImage(lh_b, 0, Height / 2);
                g.DrawImage(hh_b, Width / 2, Height / 2);
            }
            er_plane.Save("C:\\temp\\transformed_aab_r_" + Name + ".bmp");
            eg_plane.Save("C:\\temp\\transformed_aab_g_" + Name + ".bmp");
            eb_plane.Save("C:\\temp\\transformed_aab_b_" + Name + ".bmp");
            watermarked = true;
        }

        /// <summary>
        /// Extract the watermark
        /// </summary>
        /// <param name="origin">The original image to compare with the watermarked image</param>
        /// <param name="wm_height">optional. specifies the watermark height. defaults to 32.</param>
        /// <param name="wm_width">optional. specifies the watermark width. defaults to 32.</param>
        /// <param name="random">optional. specifies whether the watermark has been embedded randomly. defaults to false.</param>
        /// <param name="alpha">optional. specifies the alpha. defaults to AltariaImage's constant alpha.</param>
        public Bitmap ExtractWatermark(AltariaImage origin, bool random = false, int wm_height = 32, int wm_width = 32, double alpha = alpha)
        {
            //get hl and lh of the planes as individual bitmaps
            Bitmap hl_r, lh_r;
            Bitmap hl_g, lh_g;
            Bitmap hl_b, lh_b;
            Bitmap origin_hl_r, origin_lh_r;
            Bitmap origin_hl_g, origin_lh_g;
            Bitmap origin_hl_b, origin_lh_b;

            double finalpixel = 0;

            Bitmap result = new Bitmap(wm_width, wm_height);
            Rectangle hl_crop = new Rectangle(Width / 2, 0, Width / 2, Height / 2);
            Rectangle lh_crop = new Rectangle(0, Height / 2, Width / 2, Height / 2);

            // r_plane
            hl_r = r_plane.Clone(hl_crop, r_plane.PixelFormat);
            lh_r = r_plane.Clone(lh_crop, r_plane.PixelFormat);
            origin_hl_r = origin.r_plane.Clone(hl_crop, origin.r_plane.PixelFormat);
            origin_lh_r = origin.r_plane.Clone(lh_crop, origin.r_plane.PixelFormat);

            // g_plane
            hl_g = g_plane.Clone(hl_crop, g_plane.PixelFormat);
            lh_g = g_plane.Clone(lh_crop, g_plane.PixelFormat);
            origin_hl_g = origin.g_plane.Clone(hl_crop, origin.g_plane.PixelFormat);
            origin_lh_g = origin.g_plane.Clone(lh_crop, origin.g_plane.PixelFormat);

            // r_plane
            hl_b = b_plane.Clone(hl_crop, b_plane.PixelFormat);
            lh_b = b_plane.Clone(lh_crop, b_plane.PixelFormat);
            origin_hl_b = origin.b_plane.Clone(hl_crop, origin.b_plane.PixelFormat);
            origin_lh_b = origin.b_plane.Clone(lh_crop, origin.b_plane.PixelFormat);

            List<Bitmap> watermarks = new List<Bitmap>();
            if (random)
            {
                //extract the embedded watermarks from the randomized embedding
                //determine the position of placement based on maximum coefficient of original image
                int max_hl_r = 0, max_hl_g = 0, max_hl_b = 0;
                int max_lh_r = 0, max_lh_g = 0, max_lh_b = 0;
                for (int i = 0; i < hl_r.Width; i++)
                    for (int j = 0; j < hl_r.Height; j++)
                    {
                        int temp = hl_r.GetPixel(i, j).R;
                        //oh yeah ternary operations because I am lazy
                        //hl
                        max_hl_r = temp > max_hl_r ? temp : max_hl_r;
                        temp = hl_g.GetPixel(i, j).G;
                        max_hl_g = temp > max_hl_g ? temp : max_hl_g;
                        temp = hl_b.GetPixel(i, j).B;
                        max_hl_b = temp > max_hl_b ? temp : max_hl_b;
                        //lh
                        temp = lh_r.GetPixel(i, j).R;
                        max_lh_r = temp > max_lh_r ? temp : max_lh_r;
                        temp = lh_g.GetPixel(i, j).G;
                        max_lh_g = temp > max_lh_g ? temp : max_lh_g;
                        temp = lh_b.GetPixel(i, j).B;
                        max_lh_b = temp > max_lh_b ? temp : max_lh_b;
                    }
                //extracting the watermark
                for (int i = 0, ii = hl_r.Width - wm_width; i < wm_width; i++, ii++)
                {
                    //hl_r
                    for (int j = 0, start = max_hl_r; j < wm_height; j++, start++)
                    {
                        int s = start % hl_r.Width;
                        finalpixel = (hl_r.GetPixel(i, s).R - alpha * origin_hl_r.GetPixel(i, s).R) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                    //hl_g
                    for (int j = 0, start = max_hl_g; j < wm_height; j++, start++)
                    {
                        int s = start % hl_g.Width;
                        finalpixel = (hl_g.GetPixel(ii, s).G - alpha * origin_hl_g.GetPixel(ii, s).G) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                    //hl_b
                    for (int j = 0, jj = hl_b.Height - wm_height, start = max_hl_b; j < wm_height; j++, start++, jj++)
                    {
                        finalpixel = (hl_b.GetPixel(i, jj).B - alpha * origin_hl_b.GetPixel(i, jj).B) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                    //lh_r
                    for (int j = 0, start = max_lh_r; j < wm_height; j++, start++)
                    {
                        int s = start % lh_r.Width;
                        finalpixel = (lh_r.GetPixel(i, s).R - alpha * origin_lh_r.GetPixel(i, s).R) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                    //lh_g
                    for (int j = 0, start = max_lh_g; j < wm_height; j++, start++)
                    {
                        int s = start % lh_g.Width;
                        finalpixel = (lh_g.GetPixel(ii, s).G - alpha * origin_lh_g.GetPixel(ii, s).G) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                    //lh_b
                    for (int j = 0, jj = lh_b.Height - wm_height, start = max_lh_b; j < wm_height; j++, start++, jj++)
                    {
                        finalpixel = (lh_b.GetPixel(i, jj).B - alpha * origin_lh_b.GetPixel(i, jj).B) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                    watermarks.Add(result);
                }
            }
            else
            {
                //extract the embedded watermarks
                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //lh_r
                        finalpixel = (lh_r.GetPixel(i, j).R - alpha * origin_lh_r.GetPixel(i, j).R) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);
                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //hl_r
                        finalpixel = (hl_r.GetPixel(i, j).R - alpha * origin_hl_r.GetPixel(i, j).R) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);

                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //lh_g
                        finalpixel = (lh_g.GetPixel(i, j).G - alpha * origin_lh_g.GetPixel(i, j).G) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);
                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //hl_g
                        finalpixel = (hl_g.GetPixel(i, j).G - alpha * origin_hl_g.GetPixel(i, j).G) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);

                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //lh_b
                        finalpixel = (lh_b.GetPixel(i, j).B - alpha * origin_lh_b.GetPixel(i, j).B) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);
                for (int i = 0; i < wm_width; i++)
                    for (int j = 0; j < wm_height; j++)
                    {
                        //hl_b
                        finalpixel = (hl_b.GetPixel(i, j).B - alpha * origin_hl_b.GetPixel(i, j).B) / (1.0 - alpha);
                        if (finalpixel < 0 || finalpixel > 255) finalpixel = 0;
                        result.SetPixel(i, j, Color.FromArgb((int)finalpixel, (int)finalpixel, (int)finalpixel));
                    }
                watermarks.Add(result);
            }
            Bitmap finalresult = new Bitmap(wm_width * 6, wm_height);

            using (Graphics g = Graphics.FromImage(finalresult))
            {
                for (int i = 0; i < 6; i++)
                {
                    g.DrawImage(watermarks[i], i * wm_width, 0);
                }
            }
            //test
            finalresult.Save("C:\\temp\\result.bmp");
            return finalresult;
        }

        /// <summary>
        /// Performs a salt and pepper attack on the bmp.
        /// </summary>
        /// <param name="p">the intensity to perform the attack at</param>
        /// <returns>the resulting image</returns>
        internal AltariaImage SaltAndPepper(double p)
        {
            AltariaImage nai = new AltariaImage(new Bitmap(this.originalbmp), this.Name);
            SaltAndPepperNoise filter = new SaltAndPepperNoise(p);
            filter.ApplyInPlace(nai.originalbmp);
            return nai;
        }

        /// <summary>
        /// Function to obtain the Peak Signal-to-Noise Ratio. A higher value would normally indicate that the reconstruction
        /// is of a higher quality. It is an approximation to human perception of reconstruction quality.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="wm"></param>
        /// <returns></returns>
        public static double PSNR(Bitmap normal, Bitmap wm)
        {
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
                    MSE += Math.Pow((normal.GetPixel(j, i).R - wm.GetPixel(j, i).R), 2);
                    if (normal.GetPixel(j, i).R > MAX)
                        MAX = normal.GetPixel(j, i).R;
                    if (wm.GetPixel(j, i).R > MAX)
                        MAX = wm.GetPixel(j, i).R;
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
                        covariance += ((normal.GetPixel(i, j).R - mean_normal) * (wm.GetPixel(i, j).R - mean_wm)) / (n * n);
                    }
                }
                variance_normal = diff_normal_squared / (n * n);
                variance_wm = diff_wm_squared / (n * n);
                return (((2 * mean_normal * mean_wm + c1) * (2 * covariance + c2)) / ((mean_normal * mean_normal + mean_wm * mean_wm + c1) * (variance_normal + variance_wm + c2)));
            }
            else
            {
                throw new InvalidOperationException("The images have to be of the same dimensions of n*n.");
            }

        }
    }
}
