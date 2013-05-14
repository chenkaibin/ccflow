<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_MyFlowSmall" Title="工作处理" Codebehind="MyFlowSmall.aspx.cs" %>
<%@ Register src="UC/MyFlowUC.ascx" tagname="MyFlowUC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
    <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
    <style type="text/css">
    .D
    {
         text-align:center;
         width:500px;
         float:left;
          margin-left:20px;
    }
    </style>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class=D >
    <uc1:MyFlowUC ID="MyFlowUC1" runat="server" />
    </div>
    </asp:Content>