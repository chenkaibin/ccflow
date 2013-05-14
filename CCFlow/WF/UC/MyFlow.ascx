<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.MyFlow" Codebehind="MyFlow.ascx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register src="UCEn.ascx" tagname="UCEn" tagprefix="uc2" %>
<%@ Register src="../Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc3" %>
<script language="javascript" type="text/javascript" >
    function Change() {
        var btn = document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Save');
        if (btn != null) {
            if (btn.value.valueOf('*') == -1)
                btn.value = btn.value + '*';
        }
    }
    var longCtlID = 'ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_';
    function KindEditerSync() {
        try {
            if (editor1 != null) {
                editor1.sync();
            }
        }
        catch (err) {
        }
    }
    // ccform 为开发者提供的内置函数. 
    // 获取DDL值 
    function ReqDDL(ddlID) {
        var v = document.getElementById(longCtlID + 'DDL_' + ddlID).value;
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }
    // 获取TB值
    function ReqTB(tbID) {
        var v = document.getElementById(longCtlID + 'TB_' + tbID).value;
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }
    // 获取CheckBox值
    function ReqCB(cbID) {
        var v = document.getElementById(longCtlID+'CB_' + cbID).value;
        if (v == null) {
            alert('没有找到ID=' + cbID + '的单选控件.');
        }
        return v;
    }
    // 获取附件文件名称,如果附件没有上传就返回null.
    function ReqAthFileName(athID) {
        var v = document.getElementById(athID);
        if (v == null) {
            return null;
        }
        var fileName = v.alt;
        return fileName;
    }

    /// 获取DDL Obj
    function ReqDDLObj(ddlID) {
        var v = document.getElementById(longCtlID+'DDL_' + ddlID);
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }
    // 获取TB Obj
    function ReqTBObj(tbID) {
        var v = document.getElementById(longCtlID+'TB_' + tbID);
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }
    // 获取CheckBox Obj值
    function ReqCBObj(cbID) {
        var v = document.getElementById(longCtlID+'CB_' + cbID);
        if (v == null) {
            alert('没有找到ID=' + cbID + '的单选控件.');
        }
        return v;
    }
    // 设置值.
    function SetCtrlVal(ctrlID, val) {
        document.getElementById(longCtlID + 'TB_' + ctrlID).value = val;
        document.getElementById(longCtlID + 'DDL_' + ctrlID).value = val;
        document.getElementById(longCtlID+'CB_' + ctrlID).value = val;
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
    function WinOpen(url, winName) {
        var newWindow = window.open(url, winName, 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
        newWindow.focus();
        return;
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
<div id="tabForm" >
    <uc3:ToolBar ID="ToolBar1" runat="server" />
<hr>
</div>
<div id="D" >
    <uc1:Pub ID="Pub1" runat="server" />
    <uc1:Pub ID="FlowMsg" runat="server" />
    <uc2:UCEn ID="UCEn1" runat="server" />
    <uc1:Pub ID="Pub2" runat="server" />
</div>