<%@ Page Title="流程数据管理" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true"
 Inherits="CCFlow.WF.Admin.WF_Admin_FlowDB" Codebehind="FlowDB.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function DelIt(fk_flow, workid) {
        if (window.confirm('Are you sure?') == false)
            return;
        var url = 'FlowDB.aspx?DoType=DelIt&FK_Flow=' + fk_flow + '&WorkID=' + workid;
        WinShowModalDialog(url, '');
        window.location.href = window.location.href; //aspxPage + '.aspx?WorkID=';
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>