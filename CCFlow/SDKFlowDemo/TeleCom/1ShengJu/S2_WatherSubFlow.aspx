<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/1ShengJu/Site1.Master" AutoEventWireup="true" CodeBehind="S2_WatherSubFlow.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleComDemo.ShengJu.S2_WatherSubFlow" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2>省局监控每个市局子流程进展情况(<font color=red >如果各个地市流程没有完成，就不能提交.</font>)</h2>
 <asp:Button ID="Btn_Send" runat="server" Text="提交审核" onclick="Btn_Send_Click" />
 <%
     /*从子流程业务主表里获取数据，来查看子流程的变化 . */
    
     string sql = "SELECT a.*, b.wf_no, b.wf_send_user,b.region_id FROM WF_GenerWorkFlow a,  tab_wf_commonkpiopti b WHERE a.WorkID=b.WorkID and a.PWorkID=" + this.Request.QueryString["WorkID"];
     System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
    
   %>

   <fieldset>
   <legend>数据源获取的sql</legend>
   <%=sql %>

  <br>
  <hr />
  说明：通过业务表与流程引擎表相关联，就可以形成业务流程数据控制状态。
   </fieldset>

     <table border=1 >
     <caption>子流程运转信息</caption>
     <tr>
     <th>WorkID</th>
     <th>流程状态</th>
     <th>BillNo</th>
     <th>标题</th>
     <th>region_id</th>
     <th>处理人</th>
     <th>操作</th>

     </tr>
     <%
     foreach (System.Data.DataRow dr in dt.Rows)
     {
  %>
     <tr>
     <td><% =dr["WorkID"].ToString() %></td>
     <td><% =dr["WFState"].ToString()%></td>
     <td><% =dr["wf_no"].ToString()%></td>
     <td><% =dr["Title"].ToString() %></td>
     <td><% =dr["region_id"].ToString()%></td>
     <td><% =dr["wf_send_user"].ToString()%></td>
     <td><a href='Do.aspx?DoType=DelSubFlow&WorkID=<% =dr["WorkID"].ToString()%>&FK_Flow=<%=dr["FK_Flow"] %>' >删除子流程</a></td>
     </tr>

  <% } %>
     </table>
</asp:Content>
