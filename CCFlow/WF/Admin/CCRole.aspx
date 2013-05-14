<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CCRole" Codebehind="CCRole.aspx.cs" %>
<%@ Register Src="/WF/UC/Pub.ascx" TagName="Pub" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <uc2:Pub ID="Pub1" runat="server" />
</asp:Content>

