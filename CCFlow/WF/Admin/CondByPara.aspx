<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CondByPara" Codebehind="CondByPara.aspx.cs" %>
<%@ Register src="UC/CondByPara.ascx" tagname="CondByPara" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:CondByPara ID="CondByPara1" runat="server" />
</asp:Content>

