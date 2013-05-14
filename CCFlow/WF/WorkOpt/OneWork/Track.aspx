<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WorkOpt/OneWork/OneWork.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneWork.WF_WorkOpt_Track" Codebehind="Track.aspx.cs" %>
<%@ Register src="TruakUC.ascx" tagname="TruakUC" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'height=800,width=1030,top=' + (window.screen.availHeight - 800) / 2 + ',left=' + (window.screen.availWidth - 1030) / 2 + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:TruakUC ID="TruakUC1" runat="server" />
</asp:Content>

