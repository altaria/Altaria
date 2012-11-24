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

        }

        //Upload file
        protected void upload_onclick(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                HttpPostedFile file = uploadedfile.PostedFile;
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
                    // else if image, convert to AltariaImage first.
                    AltariaImage ai = new AltariaImage(new Bitmap(file.InputStream));
                    // check watermark
                    if (ai.isWatermarked())
                    {
                        //todo: extract watermark, perform attacks.
                    }
                    else
                    {
                        //redirect to step 2
                    }
                }
            }
        }
    }
}
