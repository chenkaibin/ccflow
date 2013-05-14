<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="HungUpListSmall.aspx.cs" Inherits="CCFlow.WF.WF_MyFlowInfoSmall" %>
<%@ Register src="UC/HungUp.ascx" tagname="HungUp" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:HungUp ID="HungUp1" runat="server" />
</asp:Content>
