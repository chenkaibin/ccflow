<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.WAP.UC.MyFlowWap" Codebehind="MyFlowWap.ascx.cs" %>
<%@ Register src="/WF/Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc1" %>
<%@ Register src="../../UC/UCEn.ascx" tagname="UCEn" tagprefix="uc2" %>
<%@ Register src="../../UC/Pub.ascx" tagname="Pub" tagprefix="uc3" %>
<script language=javascript>
        function WinOpen(url) {
            WinOpen(url, 'z');
        }
        function WinOpen(url, winName) {
            var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
            newWindow.focus();
            return;
        }

    //执行分支流程退回到分合流节点。
    function DoSubFlowReturn(fid, workid, fk_node) {
        var url = 'ReturnWorkSubFlowToFHL.aspx?FID=' + fid + '&WorkID=' + workid + '&FK_Node=' + fk_node;
        var v = WinShowModalDialog(url, 'df');
        window.location.href = window.history.url;
    }
    function To(url) {
        window.location.href = url;
    }
</script>

<script language="javascript">
    function GroupBarClick(rowIdx) {
        var alt = document.getElementById('Img' + rowIdx).alert;
        var sta = 'block';
        if (alt == 'Max') {
            sta = 'block';
            alt = 'Min';
        } else {
            sta = 'none';
            alt = 'Max';
        }
        document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
        document.getElementById('Img' + rowIdx).alert = alt;
        var i = 0
        for (i = 0; i <= 40; i++) {
            if (document.getElementById(rowIdx + '_' + i) == null)
                continue;
            document.getElementById(rowIdx + '_' + i).style.display = sta;
        }
    }

    function DoDelSubFlow(fk_flow, workid) {
        if (window.confirm('您确定要终止进程吗？') == false)
            return;
        var url = 'Do.aspx?DoType=DelSubFlow&FK_Flow=' + fk_flow + '&WorkID=' + workid;
        WinShowModalDialog(url, '');
        window.location.href = window.location.href; //aspxPage + '.aspx?WorkID=';
    }
    function Do(warning, url) {
        if (window.confirm(warning) == false)
            return;
        window.location.href = url;
        // WinOpen(url);
    }
</script>

<style type="text/css">
.Bar
{
    width:500px;
    text-align:center;
}

#tabForm, D
{
    width:960px;
    text-align:left;
    margin:0 auto;
    margin-bottom:5px;
}

#divCCForm {
 position:relative;
 left:25PX;
 width:960px;
}
</style>

<div id=tabForm >
    <uc1:ToolBar ID="ToolBar1" runat="server" />
</div>

<div id="D" >
    <uc3:Pub ID="FlowMsg" runat="server" />
    <uc3:Pub ID="Pub1" runat="server" />
    <uc2:UCEn ID="UCEn1" runat="server" />
    <uc3:Pub ID="Pub2" runat="server" />
</div>