<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="well">
    <div runat="server" id="step1">
        <fieldset>
            <legend>Step 1: Upload files</legend>
        </fieldset>
        <form class="form-inline" action="Default.aspx" enctype="multipart/form-data" method="post">
            <input type="file" id="uploadedfile" runat="server" clientidmode="Static" multiple="true"/>
            <span class="input-append">
                <input id="imagename" class="input-large" type="text">
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a>
            </span>
            <asp:Button type="submit" id="submit" text="Upload" runat="server" class="btn btn-success" OnClick="upload_onclick"/>
        </form>
        <hr />
        </div>
        <div runat="server" id="step2">
        <!-- Step 2 for Image Watermark. YJ Portion -->
        <fieldset>
        <legend id="step2image" runat="server">Step 2: Upload / Extract Watermark</legend>
        </fieldset>
        <asp:Repeater ID="UploadedImages" runat="server" OnItemDataBound="UploadedImages_ItemDataBound">
            <ItemTemplate>
            <!-- previous file -->
                <%# Eval("name") %>
                <br />
            <!-- watermark file -->
                <asp:FileUpload runat="server" id="fu" />
                <span class="input-append">
                    <input id="wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>" class="input-large" type="text">
                    <script type="text/javascript">
                        $('input[id=MainContent_UploadedImages_fu_<%# DataBinder.Eval(Container, "ItemIndex", "") %>]').change(function () {
                                $('#wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>').val($(this).val().replace(/C:\\fakepath\\/i, ''));
                            });
                    </script>
                    <a class="btn" onclick="$('input[id=MainContent_UploadedImages_fu_<%# DataBinder.Eval(Container, "ItemIndex", "")%>]').click();">Browse</a>
                </span>
                <asp:Button type="submit" id="submit_wm" runat="server" OnClick="uploadwm_onclick" text="Upload Watermark" class="btn btn-success"/>
            </ItemTemplate>
        </asp:Repeater>
        <!-- End Step 2 for YJ -->

        <!-- Start Step 2 for Gwendoline -->
        <!-- End Step 2 for Gwendoline -->
        
        <!-- Start Step 2 for Cyrus -->
        <!-- End Step 2 for Cyrus -->
        <hr />
        <asp:LinkButton runat="server" ID="back" OnClick="backtostep1_onclick" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to first step
        </asp:LinkButton>
        </div>
    </div>
</asp:Content>
