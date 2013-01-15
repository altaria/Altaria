<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <fieldset>
            <legend>Choose an avenue</legend>
        </fieldset>
        <ul class="thumbnails">
            <li class="span4">
                <div class="thumbnail">
                    <img src="" alt="">
                    <p>
                        A series of image watermarking implementations based on several research papers.</p>
                    <p>
                        The test will also include attempted recovery of watermarks from attacked images.</p>
                    <a href="ImageWM.aspx" class="btn btn-success" style="width: 92%; margin-top: 10px">Image Watermarking</a>
                </div>
            </li>
            <li class="span4">
                <div class="thumbnail">
                    <img src="" alt="">
                    <p>
                        Identify audio tracks based on the uploaded content.</p>
                    <p>
                        Acts as a checkpoint for individuals before they upload media content online.</p>
                    <p>
                        Attempts to lower online copyright and legal cases.</p>
                    <a href="IdentifyAudio.aspx" class="btn btn-warning" style="width: 92%">Audio / Video Fingerprinting</a>
                </div>
            </li>
            <li class="span4">
                <div class="thumbnail">
                    <img src="" alt="">
                    <p>
                        Identify image based on uploaded content</p>
                    <p>
                        Acts as a checkpoint for individuals before they upload media content online/</p>
                    <p>
                        Lower online copyright and legal cases.</p>
                    <a href="IdentifyImage.aspx" class="btn btn-info" style="width: 92%">Image Fingerprinting</a>
                </div>
            </li>
        </ul>
        <ul class="thumbnails">
            <li class="span6">
                <div class="thumbnail">
                    <img src="" alt="">
                    <p>
                        Embeds DRM in audio files</p>
                    <p>
                        A key is generated unique to user which would be embedded in the audio file</p>
                    <p>
                        Watermarks a unique key into an audio file</p>
                    <a href="AudioWM.aspx" class="btn btn-primary">Embed Audio DRM</a>
                </div>
            </li>
            <li class="span6">
                <div class="thumbnail">
                    <img src="" alt="">
                    <p>
                        Verifies Embedded DRM in audio files</p>
                    <p>
                        Cross checks Embedded key with existing key in the database</p>
                    <p>
                        Assists in verfication of ownership of audio files</p>
                    <a href="AudioVerify.aspx" class="btn btn-danger">Verify Audio DRM</a>
                </div>
            </li>

        </ul>
    </div>
</asp:Content>
