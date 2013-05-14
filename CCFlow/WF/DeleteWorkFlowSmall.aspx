<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WinOpen.master" AutoEventWireup="true" CodeBehind="DeleteWorkFlowSmall.aspx.cs" Inherits="CCFlow.WF.DeleteWorkFlowSmall" %>
<%@ Register src="UC/DeleteWorkFlowUC.ascx" tagname="DeleteWorkFlowUC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:DeleteWorkFlowUC ID="DeleteWorkFlowUC1" runat="server" />
</asp:Content>
