<%@ Page Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_Cond" Title="未命名頁面" Codebehind="Cond.aspx.cs" %>
<%@ Register src="UC/CondExt.ascx" tagname="CondExt" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:CondExt ID="CondExt1" runat="server" />
</asp:Content>

