using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Altaria.Model;

namespace Altaria
{
    public partial class AudioWM : System.Web.UI.Page
    {
        AudioReader ar;
        protected void Page_Load(object sender, EventArgs e)
        {
            step2.Visible = false;
            ErrorMessage.Visible = false;
            step3.Visible = false;
            mediainfo.Visible = false;
            userinfo.Visible = false;
            frameinfo.Visible = false;

        }

        protected void uploadAudio_click(object sender, EventArgs e)
        {
            if (Page.IsValid && uploadedfile.HasFile)
            {
                String extension = Path.GetExtension(uploadedfile.FileName);
                if (AudioReader.isAudioFile(uploadedfile.FileName))
                {
                    string filename = uploadedfile.FileName;
                    uploadedfile.SaveAs(Server.MapPath("~\\Audio\\") + filename);
                    string filepath = Server.MapPath("~\\Audio\\");
                    ar = new AudioReader(filename, filepath);
                    FileInfoLabel.Text = ar.audioInfo();
                    //ar.testloop();
                    step1.Visible = false;
                    step2.Visible = true;
                    namelabel.Text = filename;
                    pathlabel.Text = filepath;
                    mediainfo.Visible = true;
                    ErrorMessage.Visible = false;
                }
                else
                {
                    ErrorMessage.Text = "Please upload an Audio File (.wav, .midi, .mp3, etc...)";
                    ErrorMessage.Visible = true;
                }
            }
            else
            {
                ErrorMessage.Text = "No file found!";
                ErrorMessage.Visible = true;
            }
        }
        
        protected void SelectUserButton_Click(object sender, EventArgs e)
        {
            UserInfoLabel.Text = UserDropdown.SelectedItem.ToString();
            useridlabel.Text = UserDropdown.SelectedItem.Value;
            step1.Visible = false;
            step2.Visible = false;
            step3.Visible = true;
            userinfo.Visible = true;
            mediainfo.Visible = true;
            frameinfo.Visible = false;
        }
        protected void Confirm_Click(object sender, EventArgs e)
        {

            ar = new AudioReader(namelabel.Text, pathlabel.Text);
            //FrameInfoLabel.Text = FrameDropDown.SelectedValue;
            //int headerlength = Int32.Parse(FrameDropDown.SelectedValue);
            FrameInfoLabel.Text = "40000";
            int headerlength = 40000;
            int userid = Int32.Parse(useridlabel.Text);
            ar.readAudio(userid, headerlength,1);
            userinfo.Visible = true;
            mediainfo.Visible = true;
            frameinfo.Visible = true;
        }

        protected void backtostep1_onclick(object sender, EventArgs e)
        {
            step1.Visible = true;
            step2.Visible = false;
            step3.Visible = false;
            mediainfo.Visible = false;
            userinfo.Visible = false;
            frameinfo.Visible = false;
        }

        protected void backtostep2_onclick(object sender, EventArgs e)
        {
            step1.Visible = false;
            step2.Visible = true;
            step3.Visible = false;
            mediainfo.Visible = true;
            userinfo.Visible = false;
            frameinfo.Visible = false;
        }

        
    }
}