using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Altaria
{
    public partial class AudioVerify : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            step2.Visible = false;
        }

        protected void uploadAudio_click(object sender, EventArgs e)
        {
            step1.Visible = false;
            step2.Visible = true;
            if (uploadedfile.HasFile)
            {
                uploadedaudioname.Text = uploadedfile.FileName;
                String extension = Path.GetExtension(uploadedfile.FileName);
                Label1.Text = extension;
            }


        }
        protected void backtostep1_onclick(object sender, EventArgs e)
        {
            step1.Visible = true;
            step2.Visible = false;
        }
    }
}