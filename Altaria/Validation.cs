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
            //insert your validation to check whether it is audio or not
            return false;
        }
        public static bool isVideo(string type)
        {
            //insert your validation to check whether it is video or not
            return false;
        }
        public static bool isImage(string type)
        {
            //check file byte headers to check in the future.
            // http://www.mikekunz.com/image_file_header.html

            if (type.ToLower().StartsWith("image"))
                return true;
            return false;
        }

    }
}