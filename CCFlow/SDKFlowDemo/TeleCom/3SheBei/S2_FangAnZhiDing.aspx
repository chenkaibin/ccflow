﻿<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/3SheBei/Site.Master" AutoEventWireup="true" CodeBehind="S2_FangAnZhiDing.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleCom._3SheBei.S2_FangAnZhiDing" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Click" />
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Click"/>
    <asp:Button ID="Btn_Return" runat="server" Text="退回" onclick="Btn_Click" />
    <asp:Button ID="Btn_Track" runat="server" Text="轨迹" onclick="Btn_Click" />
    <asp:Button ID="Btn_Forward" runat="server" Text="转发" onclick="Btn_Click" />
        <fieldset>
        <legend>设备流程的基础信息</legend>
        <table border=1>
          <tr>
           <td>字段</td>
           <td>字段描述</td>
           <td>值</td>
         </tr>
        <%
            int workID = int.Parse(this.Request.QueryString["WorkID"]);
            
            BP.Demo.tab_wf_commonkpioptivalue en = new BP.Demo.tab_wf_commonkpioptivalue();
            en.WorkID = workID;
            en.Retrieve(BP.Demo.tab_wf_commonkpioptivalueAttr.WorkID,workID); //根据设备ID 查询出来该设备的信息.
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

       <asp:CheckBox ID="CB_IsCheck" Text="是否需要审核(此值决定流程的方向)" runat="server" />
               <br>下一步骤的工作人员:
              <asp:TextBox ID="TB_NextWorker" Text="fuhui" runat="server"></asp:TextBox>
    <hr />
</asp:Content>
