using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Altaria;

// EIWM = Embed Image WaterMark
// This is server transferred when no watermark is detected. 
namespace Altaria
{
    public partial class EIWM : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _Default prev = Context.Handler as _Default;
            AltariaImage ai = prev.getImage(); //ai is the non-watermarked image.
        }
    }
}