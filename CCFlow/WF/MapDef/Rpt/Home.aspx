<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" Inherits="WF_MapDef_Rpt_Home" Codebehind="Home.aspx.cs" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript">
    function SelectColumns(fk_flow, fk_MapData) {
        var url = 'SelectColumns.aspx?FK_MapData=' + fk_MapData + '&FK_Flow=' + fk_flow;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function Card(fk_flow, fk_MapData) {
        var url = 'Card.aspx?FK_MapData=' + fk_MapData + '&FK_Flow=' + fk_flow;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 900px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function DoReSet(fk_flow, fk_MapData, idx) {
        if (window.confirm('您确定要重新设置吗？') == false)
            return;
        window.location.href = 'Home.aspx?DoType=ColumnsOrder&ActionType=Reset&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_MapData + '&Idx=' + idx;
    }
    function View(fk_flow, fk_MapData) {
        var url = '../../Rpt/Search.aspx?FK_MapData=' + fk_MapData + '&FK_Flow=' + fk_flow;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 900px;center: yes; help: no');
        window.location.href = window.location.href;
    }
    function DoLeft(fk_flow, fk_MapData, idx) {
        window.location.href = 'Home.aspx?DoType=ColumnsOrder&ActionType=Left&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_MapData + '&Idx=' + idx;
    }
    function DoRight(fk_flow, fk_MapData, idx) {
        window.location.href = 'Home.aspx?DoType=ColumnsOrder&ActionType=Right&FK_Flow=' + fk_flow + '&FK_MapData=' + fk_MapData + '&Idx=' + idx;
    }
    function DoRightSet(fk_flow, fk_MapData) {
        var url = 'DeptPower.aspx?FK_MapData=' + fk_MapData + '&FK_Flow=' + fk_flow;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 900px;center: yes; help: no');
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table width='100%' >
<tr>
<td align=left valign=top width='25%' class=Left>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>

    <td align=left valign=top width='70%'>
    <uc1:Pub ID="Pub2" runat="server" />
    </td>
    </tr>
    </table>
</asp:Content>

