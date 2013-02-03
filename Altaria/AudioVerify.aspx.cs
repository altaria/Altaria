using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Altaria.Model;
using System.Diagnostics;

namespace Altaria
{
    public partial class AudioVerify : System.Web.UI.Page
    {
        AudioReader ar1;
        AudioReader ar2;
        protected void Page_Load(object sender, EventArgs e)
        {
            step2.Visible = false;
            ErrorMessage1.Visible = false;
            ErrorMessage2.Visible = false;
        }

       

        protected void uploadAudio_click(object sender, EventArgs e)
        {
            
                


            if (Page.IsValid)
            {
                if(AudioReader.isAudioFile(uploadedfile.FileName) && AudioReader.isAudioFile(uploadedfile2.FileName) && uploadedfile.HasFile && uploadedfile2.HasFile)
                {
                    string filepath = Server.MapPath("~\\AudioWatermark\\");
                    string filename;

                    filename = uploadedfile.FileName;
                    uploadedfile.SaveAs(Server.MapPath("~\\AudioWatermark\\") + filename);
                    ar1 = new AudioReader(filename, filepath);
                    originalinfolabel.Text = ar1.audioInfo();
                    string file1path = filepath;
                    string file1name = filename;
                    ;

                    filename = uploadedfile2.FileName;
                    uploadedfile2.SaveAs(Server.MapPath("~\\AudioWatermark\\") + filename);
                    ar2 = new AudioReader(filename, filepath);
                    watermarkinfolabel.Text = ar2.audioInfo();
                    string file2path = filepath;
                    string file2name = filename;
                    watermarkdatalabel.Text = AudioReader.verifyWatermark(ar1.readAudio(0, 40000, 2),ar2.readAudio(0, 40000, 2),Int32.Parse(UserDropdown.SelectedValue), file1name, file2path+file2name);

                    step1.Visible = false;
                    step2.Visible = true;
                }
                
                
                if (!uploadedfile.HasFile)
                {
                    ErrorMessage1.Text = "No file found!";
                    ErrorMessage1.Visible = true;
                }
                else
                {
                    if (!AudioReader.isAudioFile(uploadedfile.FileName))
                    {
                        ErrorMessage1.Text = "Please upload an Audio File (.wav, .midi, .mp3, etc...)";
                        ErrorMessage1.Visible = true;

                    }
                }
                if (!uploadedfile2.HasFile)
                {
                    ErrorMessage2.Text = "No file found!";
                    ErrorMessage2.Visible = true;
                }
                else
                {
                    if (!AudioReader.isAudioFile(uploadedfile2.FileName))
                    {
                        ErrorMessage2.Text = "Please upload an Audio File (.wav, .midi, .mp3, etc...)";
                        ErrorMessage2.Visible = true;
                    }
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