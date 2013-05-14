<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Admin.WF_Admin_SetFlowSort" Codebehind="SetFlowSort.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="流程类型名称:"></asp:Label>
        <asp:TextBox ID="Tbx_FlowSortName" runat="server"></asp:TextBox>
        <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
        <asp:Label ID="LbMessage" runat="server"></asp:Label>
    &nbsp;<br />
        <asp:Button ID="Btn_Save0" runat="server" Text="Close" 
            onclick="Btn_Save_Click" />
    </div>
    </form>
</body>
</html>
