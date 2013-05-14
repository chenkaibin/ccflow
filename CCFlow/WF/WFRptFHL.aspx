<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Face_WFRptFHL" Title="无标题页" Codebehind="WFRptFHL.aspx.cs" %>

<%@ Register src="../WF/Pub.ascx" tagname="Pub" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div align=center>
    <uc1:Pub ID="Pub1" runat="server" />
    </div>
</asp:Content>

