<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ReturnWorkSmallSingle" Codebehind="ReturnWorkSmallSingle.aspx.cs" %>
<%@ Register src="UC/ReturnWork.ascx" tagname="ReturnWork" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
    <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ReturnWork ID="ReturnWork1" runat="server" />
</asp:Content>

