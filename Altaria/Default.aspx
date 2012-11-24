<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<h1 class="logo center">Altaria</h1>
<form action="Default.aspx" enctype="multipart/form-data" method="post">
    <input type="file" id="uploadedfile" runat="server"/>
    <asp:button type="submit" text="upload" runat="server" OnClick="upload_onclick"/>
    <asp:RequiredFieldValidator class="text-error" id="RequiredFile" runat="server" ControlToValidate="uploadedfile" ErrorMessage="No file is chosen!" />
</form>
</asp:Content>
