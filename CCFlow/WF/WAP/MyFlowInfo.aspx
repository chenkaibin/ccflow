<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" 
Inherits="CCFlow.WF.WAP.WAP_MyFlowInfo" Title="驰骋工作流程引擎-信息提示" Codebehind="MyFlowInfo.aspx.cs" %>
<%@ Register src="../UC/MyFlowInfo.ascx" tagname="MyFlowInfo" tagprefix="uc1" %>
<%@ Register src="../UC/MyFlowInfoWap.ascx" tagname="MyFlowInfoWap" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc2:MyFlowInfoWap ID="MyFlowInfoWap1" runat="server" />
</asp:Content>