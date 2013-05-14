<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_EmpWorksSmallSingle" Codebehind="EmpWorksSmallSingle.aspx.cs" %>
<%@ Register src="UC/EmpWorks.ascx" tagname="EmpWorks" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:EmpWorks ID="EmpWorks1" runat="server" />
</asp:Content>

