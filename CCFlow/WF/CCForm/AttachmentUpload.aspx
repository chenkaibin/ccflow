<%@ Page Title="上传文件" Language="C#" MasterPageFile="WinOpen.master" 
AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_CCForm_AttachmentUpload" Codebehind="AttachmentUpload.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript" >
    function Del(fk_ath, pkVal, delPKVal) {
        if (window.confirm('您确定要删除吗？ ') == false)
            return ;
        window.location.href = 'AttachmentUpload.aspx?DoType=Del&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal;
    }

    
</script>

<style  type="text/css">
   .TBNote
   {
       border-bottom-color:Black;
       background-color:Silver;
   }
   
   Button
   {
       font-size:12px;
   }
   
   .Btn
   {
       font-size:12px;
   }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>