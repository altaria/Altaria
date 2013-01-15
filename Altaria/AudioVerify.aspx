<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AudioVerify.aspx.cs" Inherits="Altaria.AudioVerify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
    <div id="step1">
            <fieldset>
                <legend>Verify DRM</legend>
            </fieldset>
            <ol>
                <li>Step 1 : Upload an audio file</li>
                <li>Step 2 : Select 'Verify' and audio file will be verified against the database determine embedded file information.</li>
            </ol>
            <asp:FileUpload ID="uploadedfile" runat="server" ClientIDMode="Static" Width="520px" />
            <span class="input-append">
                <input id="imagename" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> </span>
            <asp:Button ID="scanAudio" Text="Upload" class="btn btn-success"
                runat="server" />
        </div>
        <div id="step3">
            <hr />
            <asp:LinkButton runat="server" ID="LinkButton1" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
        </div>
        </div>
</asp:Content>
