<%@ Page Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CondDept" Title="未命名頁面" Codebehind="CondDept.aspx.cs" %>
<%@ Register src="UC/CondDepts.ascx" tagname="CondDept" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:CondDept ID="CondDept1" runat="server" />
</asp:Content>

