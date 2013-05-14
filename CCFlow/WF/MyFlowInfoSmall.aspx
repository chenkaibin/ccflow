<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_MyFlowInfoSmall" Codebehind="MyFlowInfoSmall.aspx.cs" %>
<%@ Register src="UC/MyFlowInfo.ascx" tagname="MyFlowInfo" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js" type="text/javascript" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script type="text/javascript" language="javascript" >
    if (window.opener && !window.opener.closed) {
        if (window.opener.name == "main") {
            window.opener.location.href = window.opener.location.href;
            window.opener.top.leftFrame.location.href = window.opener.top.leftFrame.location.href;
        }
    }
</script>
<br>
<table width='80%' align=center >
<tr>
<td width='80%'  align=center>
    <uc2:MyFlowInfo ID="MyFlowInfo1" runat="server" />
    <br />
    <br />
    <br />
</td>
</tr>
</table>
</asp:Content>
