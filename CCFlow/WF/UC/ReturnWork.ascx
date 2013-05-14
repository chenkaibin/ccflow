<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.UCReturnWork" Codebehind="ReturnWork.ascx.cs" %>
<%@ Register src="/WF/Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register src="/WF/Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc3" %>
<div align="center" >
            <div  align="center" style='height:30px;'  >
               <uc3:ToolBar ID="ToolBar1" runat="server" />
            </div>
            <div style='height:4px;' >
            </div>
            <div >
                <uc1:Pub ID="Pub1" runat="server" />
            </div>
</div>
<script type="text/javascript" >
    function OnChange(ctrl) {
        var text = ctrl.options[ctrl.selectedIndex].text;
        var user = text.substring(0, text.indexOf('='));
        var nodeName = text.substring(text.indexOf('>')+1, 1000);
        var objVal = '您好' + user+':';
        objVal += "  \t\n ";
        objVal += "  \t\n ";
        objVal += "   您处理的 “"+nodeName+"” 工作有错误，需要您重新办理． ";
        objVal += "\t\n   \t\n 礼! ";
        objVal += "  \t\n ";
        document.getElementById('ContentPlaceHolder1_ReturnWork1_Pub1_TB_Doc').value = objVal;
    }
</script>
