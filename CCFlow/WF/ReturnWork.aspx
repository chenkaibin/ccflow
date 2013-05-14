<%@ Page Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ReturnWork" Title="Untitled Page" Codebehind="ReturnWork.aspx.cs" %>
<%@ Register src="UC/ReturnWork.ascx" tagname="ReturnWork" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ReturnWork ID="ReturnWork1" runat="server" />
</asp:Content>

