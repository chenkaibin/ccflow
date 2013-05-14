<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/2DiShi/Site.Master" AutoEventWireup="true" CodeBehind="S1_PaiDan.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleCom._2DiShi.S1_PaiDan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Send_Click" 
           style="width: 40px"  />
        <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
        <asp:Button ID="Btn_Track" runat="server" Text="轨迹图" onclick="Btn_Track_Click" />
        <hr />
        <%
            BP.Demo.tab_wf_commonkpiopti pti = new BP.Demo.tab_wf_commonkpiopti();
            pti.Retrieve(BP.Demo.tab_wf_commonkpioptiAttr.WorkID, this.WorkID);
             %>
             <fieldset>
             <legend>子流程主表数据</legend>
        <table border=1 >
        <caption></caption>
        <tr>
        <td>单据编号:<%=pti.wf_no %></td>
        <td>城市:<%=pti.region_id %></td>
        <td>负责人:<%=pti.wf_send_user%></td>
        </tr>
        </table>
            </fieldset>
        <%
            Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
            BP.Demo.tab_wf_commonkpioptivalues shebeis = new BP.Demo.tab_wf_commonkpioptivalues();
            shebeis.Retrieve(BP.Demo.tab_wf_commonkpioptivalueAttr.ParentWorkID, workid);
             %>

             <fieldset>
             <legend>设备信息：负责人就是子线程的接受人,几条明细记录就有几个子线程,这些设备信息是父流程采集过来的数据.</legend>
             <table border=1 width="70%">
             <tr>
             <td>负责人</td>
             <td>fk_flow</td>
             <td>设备地址</td>
             </tr>
             <%
               foreach (BP.Demo.tab_wf_commonkpioptivalue shebei in shebeis)
               {
                   %>
                   <tr>
                   <td><%=shebei.fuzeren %></td>
                   <td><%=shebei.fk_flow %></td>
                   <td><%=shebei.addr %></td>
                   </tr>
                  
               <% } %>
             </table>

            </fieldset>


</asp:Content>
