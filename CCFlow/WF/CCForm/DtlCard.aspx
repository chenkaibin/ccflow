<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_DtlFrm" Codebehind="DtlCard.aspx.cs" %>
<%@ Register src="/WF/UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:UCEn ID="UCEn1" runat="server" />
</asp:Content>
