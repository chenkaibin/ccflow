<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/Telecom/1ShengJu/Site1.Master" AutoEventWireup="true" CodeBehind="S1_Start.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TelecomDemo.Parent.S1_Start" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Button ID="Btn_Send" runat="server" Text="发起工单" onclick="Btn_Send_Click" />
    <asp:Button ID="Btn_Save" runat="server" Text="保存工单" onclick="Btn_Save_Click" />
    <asp:Button ID="Btn_FlowChat" runat="server" Text="流程图" onclick="Btn_Chat_Click" />

    <hr />

    <fieldset>
    <legend>工单主表信息</legend>
    发起人:<asp:TextBox ID="TB_FaQiRen" Text="zhangshan"  runat="server"></asp:TextBox>
    要求完成时间:<asp:TextBox ID="TB_SDT" Text="2013-01-04" runat="server"></asp:TextBox>
    <br />
    指标名称:<asp:TextBox ID="TB_ZBName" Text="掉话率" runat="server"></asp:TextBox>
    指标值:<asp:TextBox ID="TB_ZBVal" Text="10%" runat="server"></asp:TextBox>

    <br />
    单据编号:<asp:TextBox ID="TB_wf_no" Text="10%" runat="server"></asp:TextBox>
    发起时间:<asp:TextBox ID="TB_wf_send_time" Text="10%" runat="server"></asp:TextBox>
    </fieldset>

    <fieldset>
    <legend>工单从表信息(每条信息就是一个子流程,地市负责人就是子流程的第一个节点的处理人)</legend>
         <table border=1 width='100%' >
          <tr>
         <th>地市</th>
         <th>地市负责人</th>
         <th>单据编号</th>
         <th>状态</th>
         <th>编辑设备信息</th>
         </tr>
 
         <tr>
         <td><input value='济南市' type=text  /></td>
         <td><input value='zhoutianjiao' type=text  /></td>
         <td><input value='111-222-333' type=text  /></td>
         <td>初始化</td>
         <td>设备</td>
         </tr>
         
         <tr>
         <td colspan=5 >
         <table border="1" width='60%' >
         <caption >设备信息(从表的从表,每条信息就是一个子子流程)</caption>
         <tr>
         <th>设备编号</th>
         <th>设备负责人</th>
         <th>设备位置</th>
         </tr>

         <tr>
         <td>abc-abc</td>
         <td>guobaogeng</td>
         <td>济南高新区xx路xx号</td>
         </tr>

          <tr>
         <td>abc-123</td>
         <td>fuhui</td>
         <td>济南历城区xx路xx号</td>
         </tr>
         </table>

         </td>
         </tr>

     </table>
    </fieldset>

    <h3>省局发起工单-发送后并启动市局的任务,让市局工作处于待办状态.</h3>
</asp:Content>
