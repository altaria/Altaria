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
            step2image.Visible = false;
        }
        protected List<AltariaImage> ai = new List<AltariaImage>();
        public List<AltariaImage> getImages()
        {
            return ai;
        }
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
                        ai.Add(new AltariaImage(new Bitmap(file.InputStream), file.FileName));
                    }
                    else
                    {
                        // random file type.
                    }
                }
                UploadedImages.DataSource = ai;
                step2image.Visible = true;
                UploadedImages.DataBind();
            }
        }
        
        //upload watermark
        protected void uploadwm_onclick(object sender, EventArgs e)
        {
            HttpFileCollection hfc = Request.Files;
        }

        protected void UploadedImages_ItemDataBound(object sender, RepeaterItemEventArgs riea)
        {
            AltariaImage ai = riea.Item.DataItem as AltariaImage;
            if (ai.isWatermarked)
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
}
