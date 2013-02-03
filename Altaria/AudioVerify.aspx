<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AudioVerify.aspx.cs" Inherits="Altaria.AudioVerify" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style>
.h6-special {
		margin: 10px 0 0 0;
	}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <fieldset>
                <legend>Verify DRM</legend>
            </fieldset>
            <ol>
                <li>Step 1 : Upload an audio file</li>
                <li>Step 2 : Select 'Verify' and audio file will be verified against the database determine embedded file information.</li>
            </ol>
            <hr />
    
    <div runat="server" id="step1">
    <h6>Select user to verify to Media File</h6>
    <asp:Label ID="ErrorMessage3" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
    <br />
            <asp:DropDownList ID="UserDropdown" runat="server" DataSourceID="SqlDataSource1" DataTextField="userdataname" DataValueField="iduserdata">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                ConnectionString="<%$ ConnectionStrings:altaria_databaseConnectionString %>" 
                ProviderName="<%$ ConnectionStrings:altaria_databaseConnectionString.ProviderName %>" 
                SelectCommand="SELECT  * from userdata;"></asp:SqlDataSource>

    <h6 class="h6-special">Original Audio File</h6>
    <asp:Label ID="ErrorMessage1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
    <br />
            <asp:FileUpload ID="uploadedfile" runat="server" ClientIDMode="Static" Width="520px" />
            <span class="input-append">
                <input id="imagename" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> </span>
                <br />
                <br />
    <h6 class="h6-special">Watermarked Audio File</h6>
        <asp:Label ID="ErrorMessage2" runat="server" Text="" ForeColor="Red" Visible="false" ></asp:Label>
        <br />
            <asp:FileUpload ID="uploadedfile2" runat="server" ClientIDMode="Static" Width="520px" />
            <span class="input-append">
                <input id="imagename2" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile2]').click();">Browse</a> </span>

                <br />

                <br />            
                <asp:Button ID="uploadAudio" Text="Upload" class="btn btn-success" OnClick="uploadAudio_click"
                runat="server" />
             <hr />
            <asp:LinkButton runat="server" ID="LinkButton1" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
        </div>
        
        <div runat="server" id="step2">
            <h5>Media Info - Original File</h5>
            <asp:Label runat="server" ID="originalinfolabel" Text="" Visible="true"></asp:Label>
            <br />
            <h5>Media Info - Watermarked File File</h5>
            <asp:Label runat="server" ID="watermarkinfolabel" Text="" Visible="true"></asp:Label>
            <br />
            <h5>Watermark Data</h5>
            <asp:Label runat="server" ID="watermarkdatalabel" Text="" Visible="true"></asp:Label>
            <hr />
            <asp:LinkButton runat="server" ID="LinkButton2" PostBackUrl="~/Default.aspx" class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
        </div>
        <div id="step3">
           
    </div>
        </div>
</asp:Content>
