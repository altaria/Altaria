<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="IdentifyAudio.aspx.cs" Inherits="Altaria_DRM.IdentifyAudio" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>Audio Fingerprinting Library</p>

    <ul>
        <li>Identify audio track based on uploaded content</li>
        <li>Act as a checkpoint for individuals before they upload media content online</li>
        <li>Lower online copyright and legal cases</li>
    </ul>

    <b>Stage 1:</b> <asp:FileUpload id="fileUpload" runat="server" Width="520px" />
    <br /><br />

    <b>Stage 2: </b><asp:Button id="scanAudio" Text="Scan Audio Files" OnClick="scanAudio_Click" runat="server" />
    <asp:Label runat="server" ID="waiting" Text="Please wait a moment as we analyze your files" Visible="false"></asp:Label>
    <p>
    <b>Stage 3: <asp:Button runat="server" ID="exportToPdf" Text="Results Analysis (PDF)" 
        onclick="exportToPdf_Click" Visible="True" enabled="false" /></b><br /><br />
    <asp:Label runat="server" ID="result" Text="Results Analysis" Visible="false"></asp:Label>
    </p>
    <p>

    <asp:Button runat="server" ID="ingestFingerprint" Text="Add Song"
            onclick="ingestFingerprint_Click" Visible="False" />
            <asp:Label runat="server" ID="addSongResult" Text="Result" Visible=false></asp:Label>
            <asp:Label Text="" runat="server" ID="pathName" Visible="false"></asp:Label>
        <asp:Label ID="exportedResults" runat="server" Visible="False"></asp:Label>
    </p>
        </asp:Content>
