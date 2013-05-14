<%@ Page Title="" Language="C#" MasterPageFile="/WF/Comm/RefFunc/MasterPage.master" AutoEventWireup="true" 
Inherits="CCFlow.WF.Comm.RefFunc.Refmethod" Codebehind="Refmethod.aspx.cs" %>

<%@ Register src="Left.ascx" tagname="Left" tagprefix="uc1" %>
<%@ Register src="Refmethod.ascx" tagname="Refmethod" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" for="document" event="onkeydown">
<!--
 if (window.event.srcElement.tagName="TEXTAREA") 
     return false;
  if(event.keyCode==13)
     event.keyCode=9;
-->
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Left ID="Left1" runat="server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <uc2:Refmethod ID="Refmethod1" runat="server" />
</asp:Content>

