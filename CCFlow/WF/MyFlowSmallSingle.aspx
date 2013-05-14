<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_MyFlowSmallSingle" Codebehind="MyFlowSmallSingle.aspx.cs" %>
<%@ Register src="UC/MyFlowUC.ascx" tagname="MyFlowUC" tagprefix="uc1" %>
<%@ Register src="UC/MyFlow.ascx" tagname="MyFlow" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
    <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    
    <uc2:MyFlow ID="MyFlow1" runat="server" />

    
</asp:Content>