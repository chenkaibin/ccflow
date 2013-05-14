<%@ Register TagPrefix="uc1" TagName="UCSys" Src="/WF/Comm/UC/UCSys.ascx" %>
<%@ Page language="c#" Inherits=" CCFlow.WF.Rpt.UIContrastDtl" Codebehind="GroupDtl.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../UC/Pub.ascx" tagname="Pub" tagprefix="uc2" %>
<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD >
		<title>数据挖掘->详细信息</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
        <script type="text/javascript">
            function WinOpen(url) {
                var newWindow = window.open(url, 'df', 'width=700,height=400,top=100,left=300,scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');
                newWindow.focus();
                return;
            }
    </script>
    <base target=_self />
	</HEAD>
	<body  onkeypress=Esc() leftMargin=0    topMargin=0 >
		<form id="Form1" method="post" runat="server">
<uc2:Pub ID="Pub1" runat="server" />
		</form>
	</body>
</HTML>
