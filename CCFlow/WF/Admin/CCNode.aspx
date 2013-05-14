<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_CCNode" Codebehind="CCNode.aspx.cs" %>
<%@ Register Src="/WF/Comm/UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link href="/WF/Comm/Style/Table0.css" rel="stylesheet" type="text/css" />
    <script  language=javascript>
    function Del(mypk, fk_flow, refoid)
    {
        if (window.confirm('您确定要删除吗？') ==false)
            return ;
    
        var url='Do.aspx?DoType=Del&MyPK='+mypk+'&RefOID='+refoid;
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    </script>
    <base target=_self />
</head>
<body class="Body<%=BP.Web.WebUser.Style%>"   leftMargin=0  topMargin=0>
    <form id="form1" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;
            height: 100%">
            <tr>
                <td valign=top>
                   <uc2:ucsys ID="Ucsys1" runat="server" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
