<%@ Page Title="" Language="C#" MasterPageFile="~/WF/Rpt/MasterPage.master" AutoEventWireup="true" Inherits="CCFlow.WF.Rpt.WF_Rpt_G" Codebehind="Group.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="UCSys" Src="/WF/Comm/UC/UCSys.ascx" %>
<%@ Register src="/WF/Comm/UC/ToolBar.ascx" tagname="ToolBar" tagprefix="uc2" %>
<%@ Register src="/WF/Comm/UC/UCGraphics.ascx" tagname="ucgraphics" tagprefix="uc3" %>
<%@ Register src="../Pub.ascx" tagname="Pub" tagprefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
.MyTable
{
    text-align:left;
}
</style>
		<script language="JavaScript" src="/WF/Comm/JScript.js"></script>
		<base target=_self />
        <script type="text/javascript">
            //  事件.
            function DDL_mvals_OnChange(ctrl, ensName, attrKey) {

                var idx_Old = ctrl.selectedIndex;
                if (ctrl.options[ctrl.selectedIndex].value != 'mvals')
                    return;
                if (attrKey == null)
                    return;
                var timestamp = Date.parse(new Date());
                var url = 'SelectMVals.aspx?EnsName=' + ensName + '&AttrKey=' + attrKey + '&D=' + timestamp;
                var val = window.showModalDialog(url, 'dg', 'dialogHeight: 450px; dialogWidth: 450px; center: yes; help: no');
                if (val == '' || val == null) {
                    ctrl.selectedIndex = 0;
                }
            }
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" Runat="Server">
    <TABLE height="100%" style="background:none;"  cellPadding=0 width="100%" align=left border=0>
				<TR>
					<TD colSpan="2"  class="GroupTitle" >
                     <uc4:Pub ID="Pub1" runat="server" />
                        <uc2:ToolBar ID="ToolBar1" runat="server" />
                    </TD>
				</TR>
				<TR vAlign="top" height="100%">
					<TD vAlign="top" noWrap width="18%" class=Left  >
						<TABLE width="100%" cellspacing="1" style="border:1px #abcbe6 solid;">
							<TR>
								<TD border="0" class="Title" ><b>显示内容</b></TD>
							</TR>
							<tr>
								<TD  style="font-size:12px; border:0px">
                                <asp:checkboxlist id="CheckBoxList1"  BorderStyle=None Width="100%" runat="server" AutoPostBack=true >
                                </asp:checkboxlist>
                                </TD>
							</TR>
                            <TR>
								<TD border="0" class="Title" ><b>分析项目</b></TD>
							</TR>
							<tr>
								<TD nowarp=true align="left"  style="font-size:12px;" BorderStyle=None Width="100%"  >
                                <uc1:ucsys id="UCSys2" runat="server"></uc1:ucsys>
                                </TD>
							</TR>
							<TR>
								<TD class="Title" >
								<asp:CheckBox ID="CB_IsShowPict" runat="server" Text="显示图形" AutoPostBack=true Font-Bold="True" />
								</TD>
							</TR>
							<TR>
								<TD align="Left">
								<table width='100%' border="1px;" class='t' >
								<tr style="font-size:12px;">
								<TD nowarp=true>高度:</TD>
								  <TD class="TD">
                                      <asp:TextBox ID="TB_H" runat="server" Text="400" style="Width:90px;height:auto; text-align:right"></asp:TextBox>
                                      <textbox> </textbox> </TD>
								 </tr>
								<tr style="font-size:12px;">
								<TD  nowarp=true >宽度:</TD>
									<TD class="TD" >
									    <asp:TextBox ID="TB_W" runat="server" Text="800" style="Width:90px;height:auto; text-align:right"></asp:TextBox>
									</TD>
								</TR>
								</table>
                            </TD>
							</TR>
						</TABLE>
					</TD>
					<TD valign="top"   class=TD  class="align:left" >
                    <uc1:ucsys id="UCSys1" runat="server"></uc1:ucsys>
						<uc1:UCSys ID="UCSys3" runat="server" />
						
                        <%--<cc1:BPImage id="Img1" BorderWidth=0 runat="server"></cc1:BPImage>
						<cc1:BPImage  id="Img2"  BorderWidth=0 runat="server"></cc1:BPImage>
						<cc1:BPImage id="Img3"  BorderWidth=0 runat="server"></cc1:BPImage>--%>
				   </TD>
				</TR>
			</TABLE>
</asp:Content>

