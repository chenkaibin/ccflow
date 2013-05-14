<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_MyFlow" Title="My Flow" Codebehind="MyFlow.aspx.cs" %>
<%@ Register src="UC/MyFlowWap.ascx" tagname="MyFlowWap" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc2:MyFlowWap ID="MyFlowWap1" runat="server" />
</asp:Content>

