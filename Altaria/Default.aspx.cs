using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Web.UI.HtmlControls;
using Altaria;

namespace Altaria
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            step2.Visible = false;
            step3.Visible = false;
        }
        //protected List<AltariaImage> ai = new List<AltariaImage>();
        protected List<NewAltariaImage> ai = new List<NewAltariaImage>();
        //Upload file
        protected void upload_onclick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                HttpFileCollection hfc = Request.Files;
                for (int i = 0; i < hfc.Count; i++)
                {
                    HttpPostedFile file = hfc[i];
                    int len = file.ContentLength;
                    string type = file.ContentType;
                    //todo: determine content type.
                    byte[] file_bytes = new byte[len];
                    file.InputStream.Read(file_bytes, 0, len);
                    // if audio/ video...
                    if (Validation.isAudio(type))
                    {
                        // do something..
                    }
                    else if (Validation.isVideo(type))
                    {
                        // do something..
                    }
                    else if (Validation.isImage(type))
                    {
                        //AltariaImage temp_ai = new AltariaImage(new Bitmap(file.InputStream), file.FileName);
                        //AltariaImage temp_ai = new AltariaImage(file.InputStream, file.FileName);
                        NewAltariaImage temp_ai = new NewAltariaImage(new Bitmap(file.InputStream), file.FileName);
                        ai.Add(temp_ai);
                        //add uploaded file to session
                        Session.Add(file.FileName, temp_ai);
                    }
                    else
                    {
                        // random file type.
                    }
                }

                UploadedImages.DataSource = ai;
                step2.Visible = true;
                UploadedImages.DataBind();
            }
        }

        protected void uploadwm_onclick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            RepeaterItem ri = btn.NamingContainer as RepeaterItem;
            FileUpload fu = ri.FindControl("fu") as FileUpload;
            if (fu.HasFile)
            {
                //validate whether is image
                if (Validation.isImage(fu.PostedFile.ContentType))
                {
                    Session.Add(fu.PostedFile.FileName, new NewAltariaImage(new Bitmap(fu.PostedFile.InputStream), fu.PostedFile.FileName));
                    step3.Visible = true;
                    extract.Visible = false;
                    rplane_img.ImageUrl = "ImageHandler.ashx?file=" + ((Label)(ri.FindControl("ci"))).Text + "&wm=" + fu.PostedFile.FileName + "&mode=rplane";
                    erplane_img.ImageUrl = "ImageHandler.ashx?file=" + ((Label)(ri.FindControl("ci"))).Text + "&wm=" + fu.PostedFile.FileName + "&mode=abfull_plane";
                    //Alpha Blending for all subbands
                    //mode = aball
                    alphablending_all_img.ImageUrl = "ImageHandler.ashx?file=" + ((Label)(ri.FindControl("ci"))).Text + "&wm="+ fu.PostedFile.FileName+"&mode=aball";
                    //Alpha Blending for lh and hl subbands, but embedding with full watermark (non transformed).
                    //The watermark pixels will be spread out with a formula and is grayscale
                    //mode = abfull
                    alphablending_full_img.ImageUrl = "ImageHandler.ashx?file=" + ((Label)(ri.FindControl("ci"))).Text + "&wm=" + fu.PostedFile.FileName + "&mode=abfull";
                }
            }
        }

        protected void uploadorigin_onclick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            RepeaterItem ri = btn.NamingContainer as RepeaterItem;
            FileUpload fu = ri.FindControl("fu") as FileUpload;
            if (fu.HasFile)
            {
                //validate whether is image
                if (Validation.isImage(fu.PostedFile.ContentType))
                {
                    Session.Add(fu.PostedFile.FileName, new NewAltariaImage(new Bitmap(fu.PostedFile.InputStream), fu.PostedFile.FileName));
                    step3.Visible = true;
                    embed.Visible = false;
                    //Extract watermark
                    extracted_img.ImageUrl = "ImageHandler.ashx?file=" + ((Label)(ri.FindControl("ci"))).Text + "&wm=" + fu.PostedFile.FileName + "&mode=abfull_ex";
                }
            }
        }

        protected void UploadedImages_ItemDataBound(object sender, RepeaterItemEventArgs riea)
        {
            step1.Visible = false;
            if (riea.Item.ItemType == ListItemType.Item || riea.Item.ItemType == ListItemType.AlternatingItem)
            {   
                //AltariaImage ai = riea.Item.DataItem as AltariaImage;
                NewAltariaImage ai = riea.Item.DataItem as NewAltariaImage;
                if (ai.watermarked)
                {
                    //watermarked
                    riea.Item.FindControl("wm_form").Visible = false;                    
                }
                else
                {
                    //not watermarked, embed watermark
                }
            }
        }

        protected void backtostep1_onclick(object sender, EventArgs e)
        {
            step1.Visible = true;
            step2.Visible = false;
        }
    }
}
