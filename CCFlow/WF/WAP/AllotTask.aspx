<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_AllotTask" Title="Untitled Page" Codebehind="AllotTask.aspx.cs" %>
<%@ Register src="../UC/AllotTask.ascx" tagname="AllotTask" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:AllotTask ID="AllotTask1" runat="server" />
</asp:Content>

