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
        }
        protected List<AltariaImage> ai = new List<AltariaImage>();
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
                        AltariaImage temp_ai = new AltariaImage(new Bitmap(file.InputStream), file.FileName);
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
                    //start embed of watermark
                    //step 1: Two images are taken as input
                    AltariaImage wm = new AltariaImage(new Bitmap(fu.PostedFile.InputStream), fu.PostedFile.FileName);
                    AltariaImage ci = (AltariaImage)Session[((Label)(ri.FindControl("ci"))).Text]; 
                    
                    //step 2: The sizes of the images are extracted
                    // this is already done in AltariaImage on creation.
                    int wm_height = wm.dimensions[0];
                    int wm_width  = wm.dimensions[1];
                    int ci_height = ci.dimensions[0];
                    int ci_width  = ci.dimensions[1];
                    
                    //step 3: Normalize and reshape the watermark
                    int[] reshaped_wm = wm.Reshape();
                    
                    //step 4: Transforming the cover image into wavelet domain using DWT
                    ci.HaarTransform(null, 2); //two levels
                }
            }
        }

        protected void UploadedImages_ItemDataBound(object sender, RepeaterItemEventArgs riea)
        {
            step1.Visible = false;
            if (riea.Item.ItemType == ListItemType.Item || riea.Item.ItemType == ListItemType.AlternatingItem)
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

        protected void backtostep1_onclick(object sender, EventArgs e)
        {
            step1.Visible = true;
            step2.Visible = false;
        }
    }
}
