<%@ Page Title="" Language="C#" MasterPageFile="~/WF/WorkOpt/OneWork/OneWork.master" AutoEventWireup="true" Inherits="CCFlow.WF.OneWork.WF_WorkOpt_OneWork_OP" Codebehind="OP.aspx.cs" %>
<%@ Register src="../../Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
    function DoFunc(doType, workid, fk_flow, fk_node) {

        if (doType == 'Del' || doType == 'Reset') {
            if (confirm('您确定要执行吗？') == false)
                return;
        }

        var url = '';
        if (doType == 'HungUp' || doType == 'UnHungUp') {
            url = './../HungUpOp.aspx?WorkID=' + workid + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
            var str = window.showModalDialog(url, '', 'dialogHeight: 350px; dialogWidth:500px;center: no; help: no');
            if (str == undefined)
                return;
            if (str == null)
                return;
            //this.close();
            window.location.href = window.location.href;
            return;
        }
        url = 'OP.aspx?DoType=' + doType + '&WorkID=' + workid + '&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node;
        window.location.href = url;
    }
    function Takeback(workid, fk_flow, fk_node, toNode) {
        if (confirm('您确定要执行吗？') == false)
            return;
        var url = '../../GetTaskSmall.aspx?DoType=Tackback&FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&ToNode=' + toNode + '&WorkID=' + workid;
        window.location.href = url;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub2" runat="server" />
</asp:Content>

