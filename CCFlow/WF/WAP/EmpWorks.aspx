<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_EmpWorks" Title="Untitled Page" Codebehind="EmpWorks.aspx.cs" %>
<%@ Register src="../UC/EmpWorksWap.ascx" tagname="EmpWorksWap" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:EmpWorksWap ID="EmpWorksWap1" runat="server" />
</asp:Content>

