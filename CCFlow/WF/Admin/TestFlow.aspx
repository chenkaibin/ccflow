<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_TestFlow"
    CodeBehind="TestFlow.aspx.cs" %>

<%@ Register Src="/WF/Comm/UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc2" %>
<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <script type="text/javascript" language="javascript">
        function Del(mypk, fk_flow, refoid) {
            if (window.confirm('Are you sure?') == false)
                return;

            var url = 'Do.aspx?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function WinOpen(url) {
            var b = window.open(url, 'ass', 'width=700,top=50,left=50,height=500,scrollbars=yes,resizable=yes,toolbar=false,location=false');
            b.focus();
        }
        function WinOpen(url, w, h, name) {
            var b = window.open(url, name, 'width=' + w + ',height=' + h + ',scrollbars=yes,resizable=yes,toolbar=false,location=false,center: yes');
        }
        function WinOpenWAP_Cross(url) {
            var b = window.open(url, 'ass', 'width=50,top=50,left=50,height=20,scrollbars=yes,resizable=yes,toolbar=false,location=false');
        }
    </script>
    <link href="/WF/Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
</head>
<body leftmargin="0" topmargin="0" bgcolor="white">
    <form id="form1" runat="server">
    <uc2:ucsys ID="Ucsys1" runat="server" />
    </form>
</body>
</html>
