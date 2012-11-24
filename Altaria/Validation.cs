using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace Altaria
{
    public class Validation
    {
        public static bool isAudio(string type)
        {
            return false;
        }
        public static bool isVideo(string type)
        {
            return false;
        }
        public static bool isImage(string type)
        {
            return true;
        }
        public static bool isWatermarked(Bitmap file)
        {
            return false;
        }
    }
}