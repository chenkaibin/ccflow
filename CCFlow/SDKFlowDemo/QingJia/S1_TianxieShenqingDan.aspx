<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/QingJia/MasterPage.master" AutoEventWireup="true" Inherits="Demo_QingJiaTiao_S1_TianxieShenqingDan" Codebehind="S1_TianxieShenqingDan.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="../ccflow.js" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <!-- 按钮控制区域  -->
    <tr>
        <td  align=left>
            <asp:Button ID="Btn_NewFlow" runat="server" Text="新建流程" />
            <asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Send_Click" 
                style="margin-top: 0px" />
            <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
            <asp:Button ID="Btn_Return" runat="server" Text="退回" 
                onclick="Btn_Return_Click" />
            <asp:Button ID="Btn_Forward" runat="server" Text="转发" />
            <asp:Button ID="Btn_UnSend" runat="server" Text="撤销发送" onclick="Btn_UnSend_Click" />
            <asp:Button ID="Btn_DelFlow" runat="server" Text="删除流程" />
            <asp:Button ID="Btn_Track" runat="server" Text="工作轨迹" />
        </td>
    </tr>
    <!-- 按钮控制区域  end -->
       <tr>
        <td  align=left>
     <fieldset >
     <legend> 请假的基本信息</legend>
            请假人:<asp:TextBox ID="TB_Title" runat="server" Height="19px"></asp:TextBox>
            <br />
            请假天数:<asp:TextBox ID="TB_qingjiatian" runat="server" Text="1" ></asp:TextBox> 天
             <br />
            请假原因:<asp:TextBox ID="TB_QQYY" runat="server" Text="1" ></asp:TextBox>
     </fieldset>
       </td>
    </tr>
</asp:Content>

