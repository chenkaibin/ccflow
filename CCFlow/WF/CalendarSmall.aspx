<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_CalendarSmall" Codebehind="CalendarSmall.aspx.cs" %>
<%@ Register src="UC/CalendarUC.ascx" tagname="CalendarUC" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:CalendarUC ID="CalendarUC1" runat="server" />
</asp:Content>

