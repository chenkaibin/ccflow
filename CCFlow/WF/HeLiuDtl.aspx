<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_WorkOpt_HeLiuDtl" Codebehind="HeLiuDtl.aspx.cs" %>
<%@ Register src="./UC/Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="javascript">
    function DoDelSubFlow(fk_flow, workid) {
        if (window.confirm('您确定要终止进程吗？') == false)
            return;
        var url = 'Do.aspx?DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
        WinShowModalDialog(url, '');
        window.location.href = window.location.href; //aspxPage + '.aspx?WorkID=';
    }
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

