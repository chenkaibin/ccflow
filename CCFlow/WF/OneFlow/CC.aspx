<%@ Page Title="" Language="C#" MasterPageFile="~/WF/OneFlow/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneFlow.WF_OneFlow_CC" Codebehind="CC.aspx.cs" %>
<%@ Register src="../UC/CC.ascx" tagname="CC" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Right" Runat="Server">
    <uc2:CC ID="CC1" runat="server" />
</asp:Content>