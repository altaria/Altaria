<%@ Page Title="Altaria" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="ImageWM.aspx.cs" Inherits="Altaria.ImageWM" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="well">
        <div runat="server" id="step1">
            <fieldset>
                <legend>Stage 1: Upload files</legend>
            </fieldset>
            <form class="form-inline" action="Default.aspx" enctype="multipart/form-data" method="post">
            <input type="file" id="uploadedfile" runat="server" clientidmode="Static" multiple="true" />
            <span class="input-append">
                <input id="imagename" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> </span>
            <asp:Button type="submit" ID="submit" Text="Upload" runat="server" class="btn btn-success"
                OnClick="upload_onclick" />
            </form>
            <hr />
            <asp:LinkButton runat="server" ID="LinkButton1" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
        </div>
        <div runat="server" id="step2">
            <fieldset>
                <legend id="step2image" runat="server">Stage 2: Upload / Extract Watermark</legend>
            </fieldset>
            <asp:Repeater ID="UploadedImages" runat="server" OnItemDataBound="UploadedImages_ItemDataBound">
                <ItemTemplate>
                    <div class="row-fluid">
                        <div class="span6">
                            <asp:Label runat="server" ID="ci" Text='<%# Eval("name") %>'></asp:Label>
                            <br />
                            <asp:FileUpload runat="server" ID="fu" />
                            <span class="input-append">
                                <input id="wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>" class="input-large"
                                    type="text">
                                <script type="text/javascript">
                                    $('input[id=MainContent_UploadedImages_fu_<%# DataBinder.Eval(Container, "ItemIndex", "") %>]').change(function () {
                                        $('#wm_name<%# DataBinder.Eval(Container, "ItemIndex", "") %>').val($(this).val().replace(/C:\\fakepath\\/i, ''));
                                    });
                                </script>
                                <a class="btn" onclick="$('input[id=MainContent_UploadedImages_fu_<%# DataBinder.Eval(Container, "ItemIndex", "")%>]').click();">
                                    Browse</a> </span>
                            <asp:Button type="submit" ID="submit_wm" runat="server" OnClick="uploadwm_onclick"
                                Text="Upload Watermark" class="btn btn-success" Style="margin-left: 5px;" />
                            <asp:DropDownList ID="alpha_list" runat="server">
                                <asp:ListItem>select α for extraction</asp:ListItem>
                                <asp:ListItem>0.1</asp:ListItem>
                                <asp:ListItem>0.2</asp:ListItem>
                                <asp:ListItem>0.3</asp:ListItem>
                                <asp:ListItem>0.4</asp:ListItem>
                                <asp:ListItem>0.5</asp:ListItem>
                                <asp:ListItem>0.6</asp:ListItem>
                                <asp:ListItem>0.7</asp:ListItem>
                                <asp:ListItem>0.8</asp:ListItem>
                                <asp:ListItem>0.9</asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button type="submit" ID="submit_origin" runat="server" OnClick="uploadorigin_onclick"
                                Text="Upload original to extract watermark" class="btn btn-success" Style="float: right" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <hr />
            <asp:LinkButton runat="server" ID="back" OnClick="backtostep1_onclick" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to first stage
            </asp:LinkButton>
        </div>
        <div runat="server" id="step3">
            <div runat="server" id="embed">
                <div class="row-fluid">
                    <div class="span6">
                        <asp:Label Text="Transformed Red Plane of Original" runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="rplane_img" />
                        <hr />
                        <asp:Label ID="Label2" Text="Alpha Blending: All sub bands with varying alphas of 0.9 and 0.7"
                            runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="alphablending_all_img" />
                        <asp:Image runat="server" ID="alphablending_all_obv_img" />
                        <hr />
                    </div>
                    <div class="span6">
                        <asp:Label ID="Label1" Text="Embedded Watermark in Transformed Plane" runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="erplane_img" />
                        <hr />
                        <asp:Label Text="Alpha Blending for all sub bands, with non-transformed grayscale watermark."
                            runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="alphablending_full_img_all" />
                        <asp:Image runat="server" ID="alphablending_full_obv_img_all" />
                        <hr />
                        <asp:Label Text="Alpha Blending for lh and hl sub bands, with non-transformed grayscale watermark."
                            runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="alphablending_full_img" />
                        <asp:Image runat="server" ID="alphablending_full_obv_img" />
                        <hr />
                        <asp:Label Text="Alpha Blending for lh and hl sub bands, with randomized placement of non-transformed grayscale watermark."
                            runat="server"></asp:Label>
                        <br />
                        <asp:Image runat="server" ID="alphablending_full_random_img" />
                        <asp:Image runat="server" ID="alphablending_full_random_obv_img" />
                    </div>
                    <asp:Button OnClick="Calculate_Values" Text="SSIM, PSNR" runat="server" />
                </div>
            </div>
            <div runat="server" id="extract">
                <asp:Label Text="Results" runat="server" />
                <hr />
                <div class="container">
                    <div class="row-fluid">
                        <div class="span3">
                            <asp:Image runat="server" ID="original_img" />
                            <br />
                            <asp:Image runat="server" ID="extracted_img" />
                        </div>
                        <div class="span3">
                            <asp:Image runat="server" ID="snp1" />
                            <br />
                            <asp:Image runat="server" ID="extracted_snp1" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
