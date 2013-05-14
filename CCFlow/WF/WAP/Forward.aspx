<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WAP/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.WF_WAP_Forward" Codebehind="Forward.aspx.cs" %>

<%@ Register src="../UC/ForwardUC.ascx" tagname="ForwardUC" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ForwardUC ID="ForwardUC1" runat="server" />
</asp:Content>

