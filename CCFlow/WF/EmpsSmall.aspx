<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" CodeBehind="EmpsSmall.aspx.cs" Inherits="CCFlow.WF.EmpsSmall" %>
<%@ Register src="UC/Emps.ascx" tagname="Emps" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Emps ID="Emps1" runat="server" />
</asp:Content>

