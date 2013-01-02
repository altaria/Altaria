<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <fieldset>
            <legend>Choose an avenue</legend>
        </fieldset>
        <a href="ImageWM.aspx" class="btn btn-success">Image Watermarking</a> <a href="IdentifyAudio.aspx"
            class="btn btn-warning">Audio Fingerprinting</a> <a href="IdentifyImage.aspx" class="btn btn-info">
                Image Fingerprinting</a>
    </div>
</asp:Content>
