<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/2DiShi/Site.Master" AutoEventWireup="true" CodeBehind="CallSubFlow.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleCom._2DiShi.CallSubFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h3>子子流程(设备维护流程-开始节点)</h3>
<asp:Button ID="Btn_Send" runat="server" Text="发送子子流程" 
        onclick="Btn_Send_Click" />

  <asp:CheckBox ID="CB_IsShiShi" Text="是否需要制定方案(此值决定子子流程的方向)" runat="server" />
               <br>下一节点的工作人员:
              <asp:TextBox ID="TB_FZR" Text="fuhui" runat="server"></asp:TextBox>(此值决定子子流程的处理人)
此步功能要单独发送一个子子流程.
<hr />
        <fieldset>
        <legend>子子流程基础信息</legend>
        <table border=1>
          <tr>
           <td>字段</td>
           <td>字段描述</td>
           <td>值</td>
         </tr>
        <%
            int shebeiID = int.Parse(this.Request.QueryString["SheBeiID"]);
            
            BP.Demo.tab_wf_commonkpioptivalue en = new BP.Demo.tab_wf_commonkpioptivalue();
            en.OID = shebeiID;
            en.Retrieve(); //根据设备ID 查询出来该设备的信息.
            
            foreach (BP.En.Attr  item in en.EnMap.Attrs)
            {
            %>
           <tr>
           <td> <% =item.Key  %> </td>
           <td> <% = item.Desc %> </td>
           <td><% =en.GetValStrByKey(item.Key) %></td>
            </tr>
            <% }%>
            </table>
        </fieldset>
        <hr />
             

</asp:Content>
