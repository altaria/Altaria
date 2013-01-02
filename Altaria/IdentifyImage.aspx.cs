using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Altaria.Model;
using System.Collections;

namespace Altaria
{
    public partial class IdentifyImage1 : System.Web.UI.Page
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Digest
        {
            public string id;
            public IntPtr coeffs;
            public int size;
        }

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_dct_imagehash(string filename, ref ulong hash);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_compare_images(string filename1, string filename2, ref double pcc, double sigma, double gamma, int N, double threshold);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_hamming_distance(ref ulong hasha, ref ulong hashb);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_image_digest(string file, double sigma, double gamma, ref Digest digest, int N);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_crosscorr(ref Digest digesta, ref Digest digestb, ref double pcc, double threshold);

        String savePath = @"C:\temp\uploads\";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void scanFile_Click(object sender, EventArgs e)
        {
            waiting.Visible = false;
            addImage.Visible = false;
            imageTitle.Visible = false;
            imageAuthor.Visible = false;
            addImageResult.Visible = false;

            if (uploadedfile.HasFile)
            {
                String fileName = uploadedfile.FileName;
                savePath += fileName;
                uploadedfile.SaveAs(savePath);

                string fileExtension = Path.GetExtension(savePath);

                if (fileExtension.Equals(".jpg") || fileExtension.Equals(".png"))
                {
                    // Retrieve digest of input image for comparison
                    Digest d = new Digest();
                    ph_image_digest(savePath, 1, 1, ref d, 180);

                    // Compute hash values
                    ulong hasha = 0;
                    ph_dct_imagehash(savePath, ref hasha);

                    // Retrieve all fingerprint for comparison
                    ImageFingerprint image = new ImageFingerprint();
                    ArrayList fingerprint = image.retrieveAllHashes();

                    double pcc = 0.0;
                    ImageFingerprint matchingPictures = null;
                    double highestPcc = 0;

                    // Compare fingerprint
                    for (int i = 0; i < fingerprint.Count; i++)
                    {
                        // Generate a struct for comparison from database
                        Digest d2 = new Digest();
                        d2.id = ((ImageFingerprint)fingerprint[i]).getDigestId();
                        d2.coeffs = ((ImageFingerprint)fingerprint[i]).getDigestCoeffs();
                        d2.size = ((ImageFingerprint)fingerprint[i]).getDigestSize();

                        // Compare the results
                        int matching = ph_crosscorr(ref d, ref d2, ref pcc, 0.9);

                        if (matching == 1)
                        {
                            if (pcc > highestPcc)
                            {
                                matchingPictures = (ImageFingerprint)fingerprint[i];
                                highestPcc = pcc;
                            }
                        }
                    }

                    if (matchingPictures == null)
                    {
                        int minHammingDistance = 20;
                        for (int i = 0; i < fingerprint.Count; i++)
                        {
                            // Compute Hamming Distance
                            int hammingDistance = computeHammingDistance(hasha, ((ImageFingerprint)fingerprint[i]).getHash());

                            if (hammingDistance < minHammingDistance)
                            {
                                matchingPictures = (ImageFingerprint)fingerprint[i];
                                minHammingDistance = hammingDistance;
                            }
                        }

                        if (minHammingDistance <= 17)
                        {
                            result.Text = "An image has been identified with the following information:<br/><br/>" +
                                    "Title: <b>" + matchingPictures.getImageTitle() + "</b><br/>" +
                                    "Author: " + matchingPictures.getImageAuthor() + "<br/>" +
                                    "Hamming Distance: " + minHammingDistance + "<br/>";
                        }
                        else
                        {
                            result.Text = "<b>No pictures has been identified in our database.</b><br/><br/>You can contribute by adding this image into our database.";
                            pathName.Text = savePath;
                            addImage.Visible = true;
                            imageAuthor.Visible = true;
                            imageTitle.Visible = true;

                            imageTitle.Text = "Enter Title...";
                            imageAuthor.Text = "Enter Author...";

                            imageTitle.Attributes.Add("onfocus", "if(this.value == 'Enter Title...') this.value='';");
                            imageTitle.Attributes.Add("onblur", "if(this.value == '' || this.value == ' ') this.value='Enter Title...'");

                            imageAuthor.Attributes.Add("onfocus", "if(this.value == 'Enter Author...') this.value='';");
                            imageAuthor.Attributes.Add("onblur", "if(this.value == '' || this.value == ' ') this.value='Enter Author...'");
                        }
                    }
                    else
                    {
                        result.Text = "An image has been identified with the following information:<br/><br/>" +
                                    "Title: <b>" + matchingPictures.getImageTitle() + "</b><br/>" +
                                    "Author: " + matchingPictures.getImageAuthor() + "<br/>" +
                                    "Similarities (Scale from 0 to 1): " + Math.Round(highestPcc, 2) + "<br/>";
                    }

                    result.Visible = true;
                }
                else
                {
                    waiting.Text = "Incorrect file format has been used. Only jpg and png files will be accepted.";
                    waiting.Visible = true;
                }
            }
            else
            {
                waiting.Text = "No file has been selected for analysis";
                waiting.Visible = true;
            }
        }

        protected void addImage_Click(object sender, EventArgs e)
        {
            Digest d = new Digest();
            ph_image_digest(pathName.Text, 1, 1, ref d, 180);

            ulong hash = 0;
            ph_dct_imagehash(pathName.Text, ref hash);

            string title = imageTitle.Text;
            string author = imageAuthor.Text;

            if (!String.IsNullOrEmpty(title) && !String.IsNullOrEmpty(author) && !title.Equals("Enter Title...") && !author.Equals("Enter Author..."))
            {
                ImageFingerprint image = new ImageFingerprint(title, author, d.id, d.coeffs, d.size, hash);
                if (image.insertNewImage())
                    addImageResult.Text = "Image successfully added. Thank you for your contribution";
                else
                    addImageResult.Text = "Error in inserting image. Please try again later";
            }
            else
            {
                addImageResult.Text = "Please fill in the appropriate fields to add an image into our database.";
            }

            addImageResult.Visible = true;
        }

        private int computeHammingDistance(ulong hasha, ulong hashb)
        {
            int dist = 0;

            string hash1 = Convert.ToString(hasha);
            string hash2 = Convert.ToString(hashb);

            while (hash2.Count() < 20)
                hash2 += "0";

            while (hash1.Count() < 20)
                hash1 += "0";

            for (int i = 0; i < hash1.Count(); i++)
            {
                if (!hash1[i].Equals(hash2[i]))
                    dist += 1;
            }

            return dist;
        }
    }
}