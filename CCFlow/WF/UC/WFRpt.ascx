<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.WFRpt" Codebehind="WFRpt.ascx.cs" %>
<%@ Register src="UCEn.ascx" tagname="UCEn" tagprefix="uc1" %>
<uc1:UCEn ID="UCEn1" runat="server" />
<script  type="text/javascript">
function ReinitIframe(frmID, tdID) {
    try {

        var iframe = document.getElementById(frmID);
        var tdF = document.getElementById(tdID);
        iframe.height = iframe.contentWindow.document.body.scrollHeight;
        iframe.width = iframe.contentWindow.document.body.scrollWidth;
        if (tdF.width < iframe.width) {
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
.ActionType
{
    width:16px;
    height:16px;
}
</style>