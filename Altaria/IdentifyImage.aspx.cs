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
        public struct DigestRaw
        {
            [MarshalAsAttribute(UnmanagedType.LPStr)]
            public string id;
            [MarshalAsAttribute(UnmanagedType.SysUInt)]
            public IntPtr coeffs;
            [MarshalAsAttribute(UnmanagedType.U4)]
            public int size;
        }

        public struct Digest
        {
            public String id;
            public byte[] coeffs;
            public int size;

            public static Digest fromRaw(DigestRaw raw)
            {
                Digest d = new Digest();
                d.id = raw.id;
                d.coeffs = new byte[raw.size];
                Marshal.Copy(raw.coeffs, d.coeffs, 0, raw.size);

                return d;
            }
        }

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_dct_imagehash(string filename, ref ulong hash);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_compare_images(string filename1, string filename2, ref double pcc, double sigma, double gamma, int N, double threshold);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_hamming_distance(ref ulong hasha, ref ulong hashb);

        [DllImport(@"C:\temp\requiredFiles\pHash-0.9.4\Release\pHash.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ph_image_digest(string file, double sigma, double gamma, ref DigestRaw digest, int N);

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
                    // Retrieve all fingerprint for comparison
                    ImageFingerprint image = new ImageFingerprint();
                    ArrayList fingerprint = image.retrieveAllHashes();

                    int pcc = 0;
                    ImageFingerprint matchingPictures = null;
                    int highestPcc = 35;

                    // Retrieve digest of input image for comparison
                    DigestRaw raw = new DigestRaw();
                    ph_image_digest(savePath, 1, 1, ref raw, 180);
                    Digest d = Digest.fromRaw(raw);

                    // Compare fingerprint
                    for (int i = 0; i < fingerprint.Count; i++)
                    {
                        // Generate a struct for comparison from database
                        Digest d2 = new Digest();
                        d2.id = ((ImageFingerprint)fingerprint[i]).getDigestId();
                        d2.coeffs = ((ImageFingerprint)fingerprint[i]).getDigestCoeffs();
                        d2.size = 40;

                        // Compare the results
                        pcc = computeSimilarities(d.coeffs, d2.coeffs);

                        if (pcc >= highestPcc)
                        {
                            matchingPictures = (ImageFingerprint)fingerprint[i];
                            highestPcc = pcc;
                        }
                    }

                    // Compute hash values
                    ulong hasha = 0;
                    ph_dct_imagehash(savePath, ref hasha);

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

                        if (minHammingDistance <= 15)
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
                                    "Similarities with copyrighted image: " + (highestPcc / 40.0) * 100 + " %<br/>";
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
            DigestRaw raw = new DigestRaw();
            ph_image_digest(pathName.Text, 1, 1, ref raw, 180);
            Digest d = Digest.fromRaw(raw);

            ulong hash = 0;
            ph_dct_imagehash(pathName.Text, ref hash);

            string title = imageTitle.Text;
            string author = imageAuthor.Text;

            if (!String.IsNullOrEmpty(title) && !String.IsNullOrEmpty(author) && !title.Equals("Enter Title...") && !author.Equals("Enter Author..."))
            {
                ImageFingerprint image;
                if (!String.IsNullOrEmpty(d.id))
                    image = new ImageFingerprint(title, author, d.id.ToString(), d.coeffs, hash);
                else
                    image = new ImageFingerprint(title, author, null, d.coeffs, hash);

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

        private int computeSimilarities(byte[] coeffs1, byte[] coeffs2)
        {
            int similar = 0;

            for (int i = 0; i < coeffs1.Count(); i++)
            {
                int highest = coeffs2[i] + 4;
                int lowest = coeffs2[i] - 4;

                if (coeffs1[i] <= highest && coeffs1[i] >= lowest)
                {
                    similar += 1;
                }
            }

            return similar;
        }
    }
}