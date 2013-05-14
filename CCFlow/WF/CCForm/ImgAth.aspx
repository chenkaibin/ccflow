<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_ImgAth" Codebehind="ImgAth.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript" src="./ImgAth/jquery-1.3.1.min.js"></script>
    <script type="text/javascript" src="./ImgAth/jquery.bitmapcutter.js"></script>
    <link rel="Stylesheet" type="text/css" href="./ImgAth/jquery.bitmapcutter.css" />
    <script type="text/javascript" src="./ImgAth/ajaxfileupload.js"></script>
    <!--剪切图片-->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<style type="text/css">
        #container
        {
        	margin:0px auto;
        	background-color:#fff;
        	border:solid 1px #7d9edb;
        	padding:5px;
        }
</style>
<script type="text/javascript">
    function ImageCut(srcUrl,w,h) {
        $.fn.bitmapCutter({
            src: srcUrl,
            renderTo: '#container',
            holderSize: { width: 420, height: 400 },
            cutterSize: { width: w, height: h },
            onGenerated: function (src) {
                document.getElementById('ContentPlaceHolder1_txtPhotoUrl').value = src;
            },
            rotateAngle: 90,
            lang: { clockwise: '顺时针旋转{0}度.' }
        });
    }
</script>

<script type="text/javascript">
 var ImgAth= '<%=ImgAth%>';
 var MyPK= '<%=MyPK%>';
    function ajaxFileUpload() {
        jQuery.ajaxFileUpload
	    (
	          {
	              url: 'FileUpload.aspx?ImgAth=' + ImgAth + '&MyPK=' + MyPK,
	              secureuri: false,
	              fileElementId: 'fileToUpload',
	              dataType: 'json',
	              success: function (data, status) {
	                  if (typeof (data.error) != 'undefined') {
	                      if (data.error != '') {
	                          alert(data.error);
	                      }
	                      else {
	                          document.getElementById('container').innerHTML = '';
	                          ImageCut(data.msg, 100, 120);
	                          document.getElementById('ContentPlaceHolder1_txtPhotoUrl').value = data.msg;
	                      }
	                  }
	              },
	              error: function (data, status, e) {
	                  alert(e);
	              }
	          }
	    );
        return false;
    }
</script>
    <table width="100%" border="0" cellspacing="0" cellpadding="5">
	    <tr>
	    <td style="font-size:12px; width: 142px; " >&nbsp&nbsp&nbsp上传头像图片:</td>
	    <td align="left"><input id="fileToUpload" name="fileToUpload" type="file" size="38"  onchange="ajaxFileUpload();" /></td>
	    </tr>
    <tr>
        <td colspan="2">
       <div id="container">
       </div>  
    </td>
    </tr>
    <tr><td colspan="2">
     <asp:TextBox ID="txtPhotoUrl" runat="server"></asp:TextBox>
     <asp:Button ID="btnSubmit" class=Btn runat="server" Text="保 存" 
            onclick="btnSubmit_Click"  />
    &nbsp;&nbsp;
    <asp:Button ID="btnCancle" runat="server" class=Btn Text="取 消"  
            onclick="btnCancle_Click" />
               </td></tr>
    </table>
</asp:Content>

