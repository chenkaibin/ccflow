<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Batch" Codebehind="Batch.aspx.cs" %>

<%@ Register src="UC/Batch.ascx" tagname="Batch" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script language="javascript">
　　    function selectAll() {
        var arrObj = document.all;
        if (document.aspnetForm.checkedAll.checked) {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    if (arrObj[i].name.indexOf('IDX_') > 0)
                        arrObj[i].checked = true;
                }
            }
        } else {
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    if (arrObj[i].name.indexOf('IDX_') > 0)
                        arrObj[i].checked = false;
                }
            }
        }
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Batch ID="Batch1" runat="server" />
</asp:Content>

