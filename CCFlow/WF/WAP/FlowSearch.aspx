<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_Search" Title="Untitled Page" Codebehind="FlowSearch.aspx.cs" %>
<%@ Register src="../UC/FlowSearch.ascx" tagname="FlowSearch" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:FlowSearch ID="FlowSearch1" runat="server" />
</asp:Content>

