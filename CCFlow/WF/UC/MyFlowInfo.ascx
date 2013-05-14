<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.MyFlowInfo" Codebehind="MyFlowInfo.ascx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<%@ Register src="/WF/Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc4" %>
<%@ Register src="MyFlowInfoWap.ascx" tagname="MyFlowInfoWap" tagprefix="uc3" %>

<br>
<div align="center">
<span >
    <uc4:ToolBar ID="ToolBar1" runat="server"  />
      <uc3:MyFlowInfoWap ID="MyFlowInfoWap1" runat="server" />
      </span>
</div>
