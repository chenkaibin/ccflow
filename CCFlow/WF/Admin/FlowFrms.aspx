<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Admin/WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_FlowFrms" Codebehind="FlowFrms.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript">
    function New() {
        window.location.href = window.location.href;
    }
    function AddIt(fk_mapdata, fk_node) {
        var url = 'FlowFrms.aspx?DoType=Add&FK_MapData=' + fk_mapdata + '&FK_Node=' + fk_node;
        window.location.href = url;
    }
    function DelIt(fk_mapdata, fk_node) {
        if (window.confirm('您确定要移除吗？') == false)
            return;
        var url = 'FlowFrms.aspx?DoType=Del&FK_MapData=' + fk_mapdata + '&FK_Node=' + fk_node;
        window.location.href = url;
    }
</script>
<base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" >
    <table width='900px' height='100%' align=center  border="0px" >
<tr>
<td align=left valign=top width='20%'  border="0px">
    <uc1:Pub ID="Left" runat="server" />
    </td>
    <td align="Left" valign=top width='80%'  border="0px">
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
    </tr>
    </table>
</asp:Content>

