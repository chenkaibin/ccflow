<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_Start" Title="Untitled Page" Codebehind="Start.aspx.cs" %>

<%@ Register src="../UC/Start.ascx" tagname="Start" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Start ID="Start1" runat="server" />
</asp:Content>

