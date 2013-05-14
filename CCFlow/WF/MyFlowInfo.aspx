<%@ Page Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" 
Inherits="CCFlow.WF.WF_MyFlowInfo" Title="ccflow" Codebehind="MyFlowInfo.aspx.cs" %>
<%@ Register src="UC/MyFlowInfo.ascx" tagname="MyFlowInfo" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <div style="width:100%;text-align:center" >
    <uc1:MyFlowInfo ID="MyFlowInfo1" runat="server" />
    </div>
</asp:Content>

