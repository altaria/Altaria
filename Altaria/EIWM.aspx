<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EIWM.aspx.cs" Inherits="Altaria.EIWM" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <form action="EIWM.aspx" enctype="multipart/form-data" method="post">
            <input type="file" id="uploadedfile" runat="server" clientidmode="Static"/>
            <span class="input-append">
                <input id="imagename" class="input-large" type="text">
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a>
            </span>
            <asp:button type="submit" id="submit" text="upload watermark" runat="server" class="btn btn-success"/>
            <asp:RequiredFieldValidator class="text-error" id="RequiredFile" runat="server" ControlToValidate="uploadedfile" ErrorMessage="No file is chosen!" />
        </form>
    </div>
</asp:Content>
