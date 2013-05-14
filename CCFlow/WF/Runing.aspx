<%@ Page Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Runing" Title="无标题页" Codebehind="Runing.aspx.cs" %>
<%@ Register src="UC/Runing.ascx" tagname="Runing" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Runing ID="Runing1" runat="server" />
</asp:Content>

