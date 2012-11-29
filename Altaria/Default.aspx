<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Altaria._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="well">
    <asp:UpdatePanel runat="server">
    <ContentTemplate>
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
            <asp:RequiredFieldValidator class="text-error" id="RequiredFile" runat="server" ControlToValidate="uploadedfile" ErrorMessage="No file is chosen!" />
        </form>
        <hr />
        <!-- Step 2 for Image Watermark. YJ Portion -->
        <fieldset>
        <legend id="step2image" runat="server">Step 2: Upload / Extract Watermark</legend>
        </fieldset>
        <asp:Repeater ID="UploadedImages" runat="server" OnItemDataBound="UploadedImages_ItemDataBound">
            <ItemTemplate>
                <%# Eval("name") %><br />
                <form id="wm_form" action="Default.aspx" enctype="multipart/form-data" method="post" class="form-inline">
                    <input type="file" id="uploadedwm<%# DataBinder.Eval(Container, "ItemIndex", "") %>"/>
                    <span class="input-append">
                        <input id="wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>" class="input-large" type="text">
                        <script type="text/javascript">
                                    $('input[id=uploadedwm<%# DataBinder.Eval(Container, "ItemIndex", "") %>]').change(function () {
                                        $('#wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>').val($(this).val().replace(/C:\\fakepath\\/i, ''));
                                    });
                        </script>
                        <a class="btn" onclick="$('input[id=uploadedwm<%# DataBinder.Eval(Container, "ItemIndex", "")%>]').click();">Browse</a>
                    </span>
                    <asp:button type="submit" text="Upload Watermark" runat="server" class="btn btn-success" OnClick="uploadwm_onclick"/>
                </form>
            </ItemTemplate>
        </asp:Repeater>
        <!-- End Step 2 for YJ -->

        <!-- Start Step 2 for Gwendoline -->
        <!-- End Step 2 for Gwendoline -->
        
        <!-- Start Step 2 for Cyrus -->
        <!-- End Step 2 for Cyrus -->
    
    </ContentTemplate>
    <Triggers>
        <asp:PostBackTrigger ControlID = "submit" />
    </Triggers>
    </asp:UpdatePanel>
    </div>
</asp:Content>
