<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CondPRI" Codebehind="CondPRI.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register src="./UC/CondBar.ascx" tagname="CondBar" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc2:CondBar ID="CondBar1" runat="server" />
    <table border=0 width='70%' height="400px" >
    <tr>
    <td valign=top align=right><uc1:Pub ID="Left" runat="server" /></td>
    <td valign=top  align=left><uc1:Pub ID="Pub1" runat="server" /></td>
    </tr>
    </table>
</asp:Content>


