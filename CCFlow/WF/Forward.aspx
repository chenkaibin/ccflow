<%@ Page Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Forward_UI" Title="无标题页" Codebehind="Forward.aspx.cs" %>
<%@ Register src="UC/ForwardUC.ascx" tagname="ForwardUC" tagprefix="uc266" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc266:ForwardUC ID="ForwardUC1" runat="server" />
</asp:Content>