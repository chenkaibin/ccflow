<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_BillSet" Codebehind="Bill.aspx.cs" %>
<%@ Register Src="/WF/Comm/UC/ucsys.ascx" TagName="ucsys" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
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
    function AddBillType()
    {
        var url='/WF/Comm/PanelEns.aspx?EnsName=BP.WF.BillTypes';
        var b=window.showModalDialog( url , 'ass' ,'dialogHeight: 400px; dialogWidth: 600px;center: yes; help: no'); 
        window.location.href = window.location.href;
    }
    </script>
</head>
<body class="Body<%=BP.Web.WebUser.Style%>"   leftMargin="0"  topMargin="0" >
    <form id="form1" runat="server">
                   <uc2:ucsys ID="Ucsys1" runat="server" />
                   <uc2:ucsys ID="Ucsys2" runat="server" />
    </form>
</body>
</html>
