using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.SessionState;

namespace Altaria
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string filename = context.Request.QueryString["file"];
            string wmname = context.Request.QueryString["wm"];
            string mode = context.Request.QueryString["mode"];
            string original = context.Request.QueryString["original"];
            context.Response.ContentType = "image/bmp";
            if (original != null)
            {
                try
                {
                    ((NewAltariaImage)context.Session[original]).originalbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception)
                {
                    ((Bitmap)context.Session[original]).Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            else
                //start embed of watermark
                //--------------------------------------START------------------------------------------------------//
                //step 1: Two images are taken as input
                //AltariaImage wm = new AltariaImage(new Bitmap(fu.PostedFile.InputStream), fu.PostedFile.FileName);
                //AltariaImage wm = new AltariaImage(fu.PostedFile.InputStream, fu.PostedFile.FileName);
                //AltariaImage ci = (AltariaImage)Session[((Label)(ri.FindControl("ci"))).Text]; 
                if (wmname != null && mode != null)
                {
                    NewAltariaImage wm = (NewAltariaImage)context.Session[wmname];
                    NewAltariaImage ci = (NewAltariaImage)context.Session[filename];
                    if (!ci.IsTransformed())
                        ci.HaarTransform();
                    if (!wm.IsTransformed())
                        wm.HaarTransform();
                    //save the transform to session to avoid transformation again
                    context.Session[filename] = ci;
                    context.Session[wmname] = wm;
                    if (mode == "rplane")
                    {
                        ci.r_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "aball")
                    {
                        ci.AlphaBlendTest(wm).Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull")
                    {
                        try
                        {
                            string alpha = context.Request.QueryString["alpha"];
                            double a = Convert.ToDouble(alpha) / 10.0;
                            ci.AdvancedAlphaBlend(alpha: a);
                        }
                        catch (Exception)
                        {
                            ci.AdvancedAlphaBlend();
                        }
                        ci.HaarRestore();
                        ci.ConcatPlanes();
                        ci.concatbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull_all")
                    {
                        try
                        {
                            string alpha = context.Request.QueryString["alpha"];
                            double a = Convert.ToDouble(alpha) / 10.0;
                            ci.AdvancedAlphaBlend(alpha: a, allplanes: true);
                        }
                        catch (Exception)
                        {
                            ci.AdvancedAlphaBlend(allplanes: true);
                        }
                        ci.HaarRestore();
                        ci.ConcatPlanes();
                        ci.concatbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull_rand")
                    {
                        try
                        {
                            string alpha = context.Request.QueryString["alpha"];
                            double a = Convert.ToDouble(alpha) / 10.0;
                            ci.AdvancedAlphaBlend(alpha: a, random: true);
                        }
                        catch (Exception)
                        {
                            ci.AdvancedAlphaBlend(random: true);
                        }
                        ci.HaarRestore();
                        ci.ConcatPlanes();
                        ci.concatbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull_plane")
                    {
                        ci.AdvancedAlphaBlend();
                        ci.er_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull_ex")
                    {
                        Bitmap ex_wm = ci.ExtractWatermark(wm);
                        ex_wm.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "abfull_ex_rand")
                    {
                        Bitmap ex_wm;
                        try
                        {
                            string alpha = context.Request.QueryString["alpha"];
                            double a = Convert.ToDouble(alpha) / 10.0;
                            ex_wm = ci.ExtractWatermark(wm, true, alpha: a);
                        }
                        catch (Exception)
                        {
                            ex_wm = ci.ExtractWatermark(wm, true);
                        }
                        ex_wm.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    //step 2: The sizes of the images are extracted
                    // this is already done in AltariaImage on creation.
                    //int wm_height = wm.dimensions[0];
                    //int wm_width  = wm.dimensions[1];
                    //int ci_height = ci.dimensions[0];
                    //int ci_width  = ci.dimensions[1];
                    //the watermark height and width 
                    //step 3: Normalize and reshape the watermark
                    //int[] reshaped_wm = wm.Reshape();

                    // CRITERIA: 
                    // The images have to be square with dimensions of multiples of 2, and the watermark dimensions has
                    // to be 1/8 of the cover image dimensions.

                    /*if (ci_height % 2 == 0 && wm_height == wm_width && ci_height == ci_width && wm_height * 8 == ci_height)
                    {
                        //step 4: Transforming the cover image into wavelet domain using DWT
                        //perform 3 level decomposition
                        //ci.HaarTransform(3);
                        //ci.HaarRestore(3); //to get the restored bmp for demonstration
                        //wm.HaarTransform(3); //decompose watermark for use
                        //wm.HaarRestore(3); // to get the restored bmp for demonstration
                        //ci.NewHaarTransform(3);
                        //step 5: Embed the watermark
                        //ci.EmbedWatermark(wm, 3);
                        //ci.NewEmbedWatermark(wm);
                        //step 6: Restore the image
                        //ci.HaarRestore(3);
                        //ci.NewHaarRestore(3);
                        //step 7: Allow the user to download the watermarked image
                    }
                    else
                    {
                        List<string> errors = new List<string>();
                        //todo: display error message on return
                        if (ci_height % 2 != 0)
                            errors.Add("Cover image has dimensions that are not divisible by 2.");

                        if (wm_height != wm_width)
                            errors.Add("Watermark is not a square.");

                        if (ci_height != ci_width)
                            errors.Add("Cover image is not a square.");

                        if (wm_height * 8 != ci_height)
                            errors.Add("Watermark dimensions should be 1/8 of the cover image.");
                    }*/
                    //---------------------------------------END-------------------------------------------------------//
                }
                else
                {
                    NewAltariaImage ci = (NewAltariaImage)context.Session[filename];
                    ci.originalbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
