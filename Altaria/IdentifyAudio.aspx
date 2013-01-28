<%@ Page Title="Audio Fingerprinting" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="IdentifyAudio.aspx.cs" Inherits="Altaria.IdentifyAudio" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <fieldset>
            <legend>Audio/Video Fingerprinting Library</legend>
        </fieldset>
        <div class="space">
            <b>Stage 1:</b>
            <asp:FileUpload ID="uploadedfile" runat="server" ClientIDMode="Static" Width="520px" />
            <span class="input-append">
                <input id="imagename" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> 
            </span> 
        </div>
        <div class="space">
            <b>Stage 2: </b>
            <asp:Button ID="scanAudio" Text="Scan Audio Files" OnClick="scanAudio_Click" class="btn btn-success"
                runat="server" />
            <asp:Label runat="server" ID="waiting" Text="Please wait a moment as we analyze your files"
                Visible="false"></asp:Label>
        </div>
        <div class="space">
            <b>Stage 3:
                <asp:Button runat="server" ID="exportToPdf" Text="Results Analysis (PDF)" OnClick="exportToPdf_Click"
                    Visible="True" Enabled="false" class="btn" /></b><br />
            <br />
            <asp:Label runat="server" ID="result" Text="Results Analysis" Visible="false"></asp:Label><br />
            <asp:TextBox ID="videoTitle" runat="server" Text="Enter Title..." Visible="false"></asp:TextBox>&nbsp;&nbsp;
            <asp:TextBox ID="videoAuthor" runat="server" Text="Enter Author..." Visible="false"></asp:TextBox>
            <asp:Button runat="server" ID="ingestFingerprint" Text="Add Song" OnClick="ingestFingerprint_Click"
                Visible="False" />
            <asp:Label runat="server" ID="addSongResult" Text="Result" Visible="false"></asp:Label>
            <asp:Label Text="" runat="server" ID="pathName" Visible="false"></asp:Label>
            <asp:Label ID="exportedResults" runat="server" Visible="False"></asp:Label>
        </div>
        <hr />
        <asp:LinkButton runat="server" ID="LinkButton1" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
    </div>
</asp:Content>
