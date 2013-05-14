<%@ Register TagPrefix="uc1" TagName="UCEn" Src="UC/UCEn.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Page language="c#" Inherits="BP.Web.Comm.UIRefMethod" Codebehind="Refmethod.aspx.cs" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<!DocType HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Esc ¼ü¹Ø±Õ´°¿Ú.</title>
		<meta content="Microsoft FrontPage 5.0" name="GENERATOR"/>
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="Style.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="JScript.js"></script>
		<script language="JavaScript" src="Menu.js"></script>
		<script language="JavaScript" src="ActiveX.js"></script>
		<base target="_self" />
		<LINK href="Style.css" type="text/css" rel="stylesheet">
		<LINK href="./Style/Table.css" type="text/css" rel="stylesheet">
		<script language="javascript" for="document" event="onkeydown">
<!--
 if (window.event.srcElement.tagName="TEXTAREA") 
     return false;
  if(event.keyCode==13)
     event.keyCode=9;
-->
</script>
	</HEAD>
	<body  onkeypress=Esc() leftMargin=0 
topMargin=0>
		<form id="Form1" method="post" runat="server">
				<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="1" class=Table  border=0>
					<TR>
						<TD>
							<asp:Label id="Label1" runat="server">Label</asp:Label></TD>
					</TR>
					<TR>
						<TD class=TD>
							<cc1:BPToolBar id="BPToolBar1" runat="server" CssClass=toolbar ></cc1:BPToolBar></TD>
					</TR>
					<TR>
						<TD  class=TD>
							<uc1:UCEn id="UCEn1" runat="server"></uc1:UCEn></TD>
					</TR>
				</TABLE>
		</form>
	</body>
</HTML>
