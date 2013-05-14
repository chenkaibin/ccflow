<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WAP_Msg" Title="Untitled Page" Codebehind="Msg.aspx.cs" %>

<%@ Register src="../UC/Msg.ascx" tagname="Msg" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Msg ID="Msg1" runat="server" />
</asp:Content>

