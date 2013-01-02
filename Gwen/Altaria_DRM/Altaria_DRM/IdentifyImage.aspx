<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="IdentifyImage.aspx.cs" Inherits="Altaria_DRM._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>Image Fingerprinting Library</p>

    <ul>
        <li>Identify image based on uploaded content</li>
        <li>Act as a checkpoint for individuals before they upload media content online</li>
        <li>Lower online copyright and legal cases</li>
    </ul>

    <b>Stage 1:</b> <asp:FileUpload id="fileUpload" runat="server" Width="520px" />
    <br /><br />
    
    <b>Stage 2: </b><asp:Button ID="scanFile" runat="server" Text="Scan Image File" 
        onclick="scanFile_Click" ></asp:Button>
        <asp:Label runat="server" ID="waiting" Text="Please wait a moment as we analyze your files" Visible="false"></asp:Label>
        <br /><br />
        <asp:Label runat="server" ID="result" Text="results" Visible="false"></asp:Label>
        <p><asp:TextBox ID="imageTitle" runat="server" Text="Enter Title..." Visible="false"></asp:TextBox>&nbsp;&nbsp;
        <asp:TextBox ID="imageAuthor" runat="server" Text="Enter Author..." Visible="false"></asp:TextBox>
        <asp:Button runat="server" ID="addImage" Text="Add Image" onclick="addImage_Click" Visible="false" />
        <asp:Label runat="server" ID="addImageResult" Text="add image result" Visible="false"></asp:Label>
        </p>
        <asp:Label runat="server" ID="pathName" Visible="false"></asp:Label>
</asp:Content>
