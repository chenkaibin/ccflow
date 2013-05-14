<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_ReturnWorkSmall" Codebehind="ReturnWorkSmall.aspx.cs" %>
<%@ Register src="UC/ReturnWork.ascx" tagname="ReturnWork" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"></script>
    <script type="text/javascript">
        function NoSubmit(ev) {
            if (window.event.srcElement.tagName == "TEXTAREA")
                return true;
            if (ev.keyCode == 13) {
                window.event.keyCode = 9;
                ev.keyCode = 9;
                return true;
            }
            return true;
        }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:ReturnWork ID="ReturnWork1" runat="server" />
</asp:Content>

