<%@ Page Title="Image Fingerprinting" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="IdentifyImage.aspx.cs" Inherits="Altaria.IdentifyImage1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="well">
        <fieldset>
            <legend>Image Fingerprinting Library</legend>
        </fieldset>
        <ul>
            <li>Identify image based on uploaded content</li>
            <li>Act as a checkpoint for individuals before they upload media content online</li>
            <li>Lower online copyright and legal cases</li>
        </ul>
        <b>Stage 1:</b>
        <asp:FileUpload ID="uploadedfile" runat="server" ClientIDMode="Static" Width="520px" />
        <span class="input-append">
            <input id="imagename" class="input-large" type="text" />
            <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> </span>
        <br />
        <br />
        <b>Stage 2: </b>
        <asp:Button class="btn btn-success" ID="scanFile" runat="server" Text="Scan Image File"
            OnClick="scanFile_Click"></asp:Button>
        <asp:Label runat="server" ID="waiting" Text="Please wait a moment as we analyze your files"
            Visible="false"></asp:Label>
        <br />
        <br />
        <asp:Label runat="server" ID="result" Text="results" Visible="false"></asp:Label>
        <p>
            <asp:TextBox ID="imageTitle" runat="server" Text="Enter Title..." Visible="false"></asp:TextBox>&nbsp;&nbsp;
            <asp:TextBox ID="imageAuthor" runat="server" Text="Enter Author..." Visible="false"></asp:TextBox>
            <asp:Button runat="server" ID="addImage" Text="Add Image" OnClick="addImage_Click"
                Visible="false" />
            <asp:Label runat="server" ID="addImageResult" Text="add image result" Visible="false"></asp:Label>
        </p>
        <asp:Label runat="server" ID="pathName" Visible="false"></asp:Label>
        <hr />
        <asp:LinkButton runat="server" ID="LinkButton1" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
    </div>
</asp:Content>
