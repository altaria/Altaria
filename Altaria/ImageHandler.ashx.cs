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
                    ((AltariaImage)context.Session[original]).originalbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
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
                    AltariaImage wm = (AltariaImage)context.Session[wmname];
                    AltariaImage ci = (AltariaImage)context.Session[filename];
                    if (!ci.IsTransformed())
                        ci.HaarTransform();
                    if (!wm.IsTransformed())
                        wm.HaarTransform();
                    //save the transform to session to avoid transformation again
                    context.Session[filename] = ci;
                    context.Session[wmname] = wm;
                    if (mode == "aball")
                    {
                        try
                        {
                            string alpha = context.Request.QueryString["alpha"];
                            double a = Convert.ToDouble(alpha) / 10.0;
                            ci.AlphaBlend(wm, a);
                        }
                        catch
                        {
                            ci.AlphaBlend(wm);
                        }
                        ci.HaarRestore();
                        ci.ConcatPlanes();
                        ci.concatbmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                    else if (mode == "aball_plane")
                    {
                        ci.AlphaBlend(wm, 0.7);
                        ci.er_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
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
                    else if (mode == "abfull_plane")
                    {
                        ci.AdvancedAlphaBlend(0.5);
                        ci.er_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
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
                    else if (mode == "abfull_all_plane")
                    {
                        ci.AdvancedAlphaBlend(alpha: 0.7, allplanes: true);
                        ci.er_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
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
                    else if (mode == "abfull_rand_plane")
                    {
                        ci.AdvancedAlphaBlend(alpha: 0.7, random: true);
                        ci.eb_plane.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Bmp);
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
                    
                }
                else
                {
                    AltariaImage ci = (AltariaImage)context.Session[filename];
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
