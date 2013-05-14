<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Accepter" Title="接受人选择器"
 Codebehind="Accepter.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register assembly="BP.Web.Controls" namespace="BP.Web.Controls" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js"　type="text/javascript"></script>
        <script type="text/javascript">
            //调用发送按钮
            function send() {
                window.opener.document.getElementById('ContentPlaceHolder1_MyFlowUC1_MyFlow1_ToolBar1_Btn_Send').click();
                window.close();
            }
            function SetSelected(cb, ids) {
                //alert(ids);
                var arrmp = ids.split(',');
                var arrObj = document.all;
                var isCheck = false;
                if (cb.checked)
                    isCheck = true;
                else
                    isCheck = false;
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                        for (var idx = 0; idx <= arrmp.length; idx++) {
                            if (arrmp[idx] == '')
                                continue;
                            var cid = arrObj[i].name + ',';
                            var ctmp = arrmp[idx] + ',';
                            if (cid.indexOf(ctmp) > 1) {
                                arrObj[i].checked = isCheck;
                                //                    alert(arrObj[i].name + ' is checked ');
                                //                    alert(cid + ctmp);
                            }
                        }
                    }
                }
            }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<table style="border:0px;width:100%" >
<tr>
<td valign=top class=BigDoc>
    <uc1:Pub ID="Left" runat="server" />
    </td>
</tr>
<tr>
<td class=BigDoc>
    <uc1:Pub ID="Pub1" runat="server" />
    </td>
</tr>
</table>
</asp:Content>
