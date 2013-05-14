<%@ Page Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Link" Title="常用链接" Codebehind="Link.aspx.cs" %>

<%@ Register src="UC/Link.ascx" tagname="Link" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table>
<tr>
<td>
    &nbsp;</td>
<td>
    <uc1:Link ID="Link2" runat="server" />
</td>
    </tr>
    </table>
</asp:Content>

