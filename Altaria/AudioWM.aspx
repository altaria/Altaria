<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AudioWM.aspx.cs" Inherits="Altaria.AudioWM" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
            <fieldset>
                <legend>Embed DRM</legend>
            </fieldset>
            <ol>
                <li>Step 1 : Upload an audio file</li>
                <li>Step 2 : Select User</li>
                <li>Step 3 : A key unique to the user and the file will be generated to be embedded into the audio file</li> 
            </ol>
            <hr />
            <div runat="server" id="mediainfo">
            <h5>Media Info</h5>
            <asp:Label ID="FileInfoLabel" runat="server" Text=""></asp:Label>
            <hr />
            </div>
            
            <div runat="server" id="userinfo">
            <h5>User Info</h5>
            <asp:Label ID="UserInfoLabel" runat="server" Text=""></asp:Label>
            <hr />
            </div>
        <div runat="server" id="step1">
            <asp:Label ID="ErrorMessage" runat="server" 
                Text="" ForeColor="Red"></asp:Label>
                <br />
            <asp:FileUpload ID="uploadedfile" runat="server" ClientIDMode="Static" Width="520px" />
            <span class="input-append">
                <input id="imagename" class="input-large" type="text" />
                <a class="btn" onclick="$('input[id=uploadedfile]').click();">Browse</a> </span>
            <asp:Button ID="uploadAudio" Text="Upload" class="btn btn-success" OnClick="uploadAudio_click"
                runat="server" />
             <hr />
            <asp:LinkButton runat="server" ID="backmenu" PostBackUrl="~/Default.aspx" 
                class="btn btn-warning">
        <i class="icon-backward"></i>
        Back to menu
            </asp:LinkButton>
        </div>
        
        <div runat="server" id="step2">
            <h6>Select user to register to Media File</h6>
            <asp:DropDownList ID="UserDropdown" runat="server" DataSourceID="SqlDataSource1" DataTextField="userdataname" DataValueField="userdataname">
            </asp:DropDownList>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:altaria_databaseConnectionString %>" ProviderName="<%$ ConnectionStrings:altaria_databaseConnectionString.ProviderName %>" SelectCommand="SELECT userdataname from userdata;"></asp:SqlDataSource>
            <asp:Button ID="SelectUserButton" runat="server" Text="Accept" 
                onclick="SelectUserButton_Click" />
            
            <hr />
            <asp:LinkButton runat="server" ID="back1" OnClick="backtostep1_onclick" class="btn btn-warning">

        <i class="icon-backward"></i>
        Back to first step
            </asp:LinkButton>
        </div>
        <div runat="server" id="step3">
           
            <asp:Label ID="PreConfirmLabel" runat="server" Text=""></asp:Label>
            <asp:Button ID="Confirm" runat="server" Text="Confirm" onclick="Confirm_Click" />
            <br />
            <asp:LinkButton runat="server" ID="back2" OnClick="backtostep2_onclick" class="btn btn-warning">

        <i class="icon-backward"></i>
        Back to second step
            </asp:LinkButton>
        </div>
        </div>
</asp:Content>
