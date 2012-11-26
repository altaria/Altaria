<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="well">
        <form action="Default.aspx" enctype="multipart/form-data" method="post">
            <input type="file" id="uploadedfile" runat="server" clientidmode="Static"/>
            <span class="input-append">
                <input id="imagename" class="input-large" type="text">
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a>
            </span>
            <asp:button type="submit" id="submit" text="upload" runat="server" class="btn btn-success" OnClick="upload_onclick"/>
            <asp:RequiredFieldValidator class="text-error" id="RequiredFile" runat="server" ControlToValidate="uploadedfile" ErrorMessage="No file is chosen!" />
        </form>
    </div>
</asp:Content>
