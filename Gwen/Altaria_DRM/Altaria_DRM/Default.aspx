<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Altaria_DRM.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div style="text-align:center; margin-top:4%">
        <p>
        Altaria is a Digital Rights Management Platform that aims to protect you and your work against copyright and legal issues.
        </p>

        <p>Please upload a file and select the following options to perform:</p>
        
        <asp:TextBox ID="fileName" runat="server" Width="324px"></asp:TextBox>&nbsp;
        <asp:Button ID="browseForFile" runat="server" Text="Browse Files" />

        <p>
        <asp:Button ID="watermark" runat="server" Height="96px" 
            Text="Image Fingerprinting Library" Width="250px" 
            PostBackUrl="~/IdentifyImage.aspx" />

        <asp:Button ID="fingerprint" runat="server" Height="96px" 
            Text="Audio Fingerprinting Library" Width="236px" 
            PostBackUrl="~/IdentifyAudio.aspx" />
        </p>
        
        </div>
</asp:Content>
