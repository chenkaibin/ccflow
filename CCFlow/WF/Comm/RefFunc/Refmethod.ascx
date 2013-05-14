<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.Comm.RefFunc.RefmethodUC" Codebehind="Refmethod.ascx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="BP.Web.Controls" Assembly="BP.Web.Controls" %>
<%@ Register TagPrefix="uc1" TagName="UCEn" Src="/WF/Comm/UC/UCEn.ascx" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls"
 Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<script language="javascript" for="document" event="onkeydown">
<!--
 if (window.event.srcElement.tagName="TEXTAREA") 
     return false;
  if(event.keyCode==13)
     event.keyCode=9;
-->
</script>

<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="100%" border="1" class=Table  border=0>
					<TR>
						<TD>
                        <uc1:UCEn id="UCEn2" runat="server"></uc1:UCEn>
							 </TD>
					</TR>
					<TR>
						<TD class=TD>
							<cc1:BPToolBar id="BPToolBar1" runat="server" CssClass=toolbar ></cc1:BPToolBar></TD>
					</TR>
					<TR>
						<TD  class=TD>
                        <br>
                        <br>
							<uc1:UCEn id="UCEn1" runat="server"></uc1:UCEn>
                        <br>
                        <br>
                            
                            </TD>
					</TR>
				</TABLE>