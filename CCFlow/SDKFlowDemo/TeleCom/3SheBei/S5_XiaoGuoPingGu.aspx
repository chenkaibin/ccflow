<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/3SheBei/Site.Master" AutoEventWireup="true" CodeBehind="S5_XiaoGuoPingGu.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleCom._3SheBei.S5_XiaoGuoPingGu" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Click" />
    <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Click"/>
    <asp:Button ID="Btn_Return" runat="server" Text="退回" onclick="Btn_Click" />
    <asp:Button ID="Btn_Track" runat="server" Text="轨迹" onclick="Btn_Click" />
    <asp:Button ID="Btn_Forward" runat="server" Text="转发" onclick="Btn_Click" />
    <hr />
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
            int WorkID = int.Parse(this.Request.QueryString["WorkID"]);
            
            BP.Demo.tab_wf_commonkpioptivalue en = new BP.Demo.tab_wf_commonkpioptivalue();
            en.Retrieve(BP.Demo.tab_wf_commonkpioptivalueAttr.WorkID,WorkID); //根据设备ID 查询出来该设备的信息.
            
            foreach (BP.En.Attr  item in en.EnMap.Attrs)
            {
            %>
           <tr>
           <td> <% =item.Key  %> </td>
           <td> <% = item.Desc %> </td>
           <td><% =en.GetValStrByKey(item.Key) %>
               </td>
            </tr>
            <% }%>
            </table>
        </fieldset>
</asp:Content>
