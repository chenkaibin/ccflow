<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_Frm" Codebehind="Frm.aspx.cs" %>
<%@ Register src="/WF/UC/UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="/WF/Comm/JScript.js" type="text/javascript" ></script>
    <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" type="text/javascript" ></script>
	<script language="JavaScript" src="MapExt.js" type="text/javascript" ></script>
    <script language='JavaScript' src='/WF/Scripts/jquery-1.4.1.min.js' type="text/javascript"></script>
<script type="text/javascript" language="javascript" >
    // 获取DDL值
    function ReqDDL(ddlID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_DDL_' + ddlID).value;
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }

    // 获取TB值
    function ReqTB(tbID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_TB_' + tbID).value;
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }

    // 获取CheckBox值
    function ReqCB(cbID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_CB_' + cbID).value;
        if (v == null) {
            alert('没有找到ID=' + cbID + '的文本框控件.');
        }
        return v;
    }

    /// 获取DDL Obj
    function ReqDDLObj(ddlID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_DDL_' + ddlID);
        if (v == null) {
            alert('没有找到ID=' + ddlID + '的下拉框控件.');
        }
        return v;
    }
    // 获取TB Obj
    function ReqTBObj(tbID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_TB_' + tbID);
        if (v == null) {
            alert('没有找到ID=' + tbID + '的文本框控件.');
        }
        return v;
    }
    // 获取CheckBox Obj值
    function ReqCBObj(cbID) {
        var v = document.getElementById('ContentPlaceHolder1_UCEn1_CB_' + cbID);
        if (v == null) {
            alert('没有找到ID=' + cbID + '的单选控件.');
        }
        return v;
    }
    // 设置值.
    function SetCtrlVal(ctrlID, val) {
        document.getElementById('ContentPlaceHolder1_UCEn1_TB_' + ctrlID).value = val;
        document.getElementById('ContentPlaceHolder1_UCEn1_DDL_' + ctrlID).value = val;
        document.getElementById('ContentPlaceHolder1_UCEn1_CB_' + ctrlID).value = val;
    }

    var isFrmChange = false;
    var isChange = false;
    function SaveDtlData() {
        if (isChange == false)
            return;
        var btn = document.getElementById("<%=Btn_Save.ClientID %>");
        if (btn) {
            btn.click();
            isChange = false;
        }
    }

    function Change(id) {
        isChange = true;
        var tagElement = window.parent.document.getElementById("HL" + id);
        if (tagElement) {
            var tabText = tagElement.innerText;
            var lastChar = tabText.substring(tabText.length - 1, tabText.length);
            if (lastChar != "*") {
                tagElement.innerHTML = tagElement.innerText + '*';
            }
        }
    }
    function TROver(ctrl) {
        ctrl.style.backgroundColor = 'LightSteelBlue';
    }

    function TROut(ctrl) {
        ctrl.style.backgroundColor = 'white';
    }
    function Del(id, ens, refPk, pageIdx) {
        if (window.confirm('您确定要执行删除吗？') == false)
            return;

        var url = 'Do.aspx?DoType=DelDtl&OID=' + id + '&EnsName=' + ens;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
        window.location.href = 'Dtl.aspx?EnsName=' + ens + '&RefPKVal=' + refPk + '&PageIdx=' + pageIdx;
    }
    function DtlOpt(workId, fk_mapdtl) {
        var url = 'DtlOpt.aspx?WorkID=' + workId + '&FK_MapDtl=' + fk_mapdtl;
        var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
        window.location.href = 'Dtl.aspx?EnsName=' + fk_mapdtl + '&RefPKVal=' + workId;
    }
    function OnKeyPress() {
    }

    function ReinitIframe(frmID, tdID) {
        try {

            var iframe = document.getElementById(frmID);
            var tdF = document.getElementById(tdID);

            iframe.height = iframe.contentWindow.document.body.scrollHeight;
            iframe.width = iframe.contentWindow.document.body.scrollWidth;

            if (tdF.width < iframe.width) {
                //alert(tdF.width +'  ' + iframe.width);
                tdF.width = iframe.width;
            } else {
                iframe.width = tdF.width;
            }

            tdF.height = iframe.height;
            return;

        } catch (ex) {

            return;
        }
        return;
    }
     
    </script>
    <style type="text/css">
        .HBtn
        {
        	/* display:none; */
        	visibility:visible;
        	background-color:White;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" >
<div style="float:left" >
<asp:Button ID="Btn_Save" runat="server" Text=""  Width="0" Height="0"   CssClass="HBtn"   Visible="false"  
        onclick="Btn_Save_Click"  />
        </div>

<asp:Button ID="Btn_Print" runat="server" Text="打印"  CssClass="Btn" Visible="true" />
    <uc1:UCEn ID="UCEn1" runat="server" />
</asp:Content>
