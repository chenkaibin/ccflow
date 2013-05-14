<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Rpt/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.Rpt.WF_Rpt_D3" Codebehind="D3.aspx.cs" %>
<%@ Register src="/WF/Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc1" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="JavaScript" src="/WF/Comm/JScript.js"></script>
 <script type="text/javascript">
     //  事件.
     function DDL_mvals_OnChange(ctrl, ensName, attrKey) {

         var idx_Old = ctrl.selectedIndex;
         if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
             return;
         if (attrKey == null)
             return;
         var timestamp = Date.parse(new Date());
         var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey + '&D=' + timestamp;
         var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');
         if (val == '' || val == null) {
             ctrl.selectedIndex = 0;
         }
     }
     function Cell(d1, d2, paras) {

     }
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <table width="100%" >
<tr>
<td colspan=2 class="GroupTitle" >
    <uc1:ToolBar ID="ToolBar1" runat="server" />
    </td>
</tr>

<tr>
<td class=Left width='200px' valign=top >
    <uc2:Pub ID="Left" runat="server" />
    </td>
<td valign=top>
    <uc2:Pub ID="Right" runat="server" />
    </td>
</tr>
</table>
</asp:Content>

