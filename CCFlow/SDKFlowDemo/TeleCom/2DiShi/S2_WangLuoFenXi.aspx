<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/TeleCom/2DiShi/Site.Master" AutoEventWireup="true" CodeBehind="S2_WangLuoFenXi.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TeleCom._2DiShi.S2_WangLuoFenXi" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    // 功能执行.
    function CallSubFlow(shebeiID, fk_flow, fk_node, parentWorkid, fid) {
        var url = "CallSubFlow.aspx?SheBeiID=" + shebeiID + '&ParentWorkID=' + parentWorkid + '&FID=' + fid;
        var newWindow = window.open(url, 'z', 'scrollbars=yes,resizable=yes,toolbar=false,location=false');
        newWindow.focus();
        return;
    }

    // 处理待办工作.
    function DealSubFlow(SheBeiID,workid) {
        var url = 'DealSubFlow.aspx?SheBeiID=' + SheBeiID + '&WorkID=' + workid + '&FK_Flow=027&FK_Node=2701&FID=0';
        var newWindow = window.open(url, 'z', 'scrollbars=yes,resizable=yes,toolbar=false,location=false');
        newWindow.focus();
        return;
    }

    // 打开流程轨迹图.
    function Chart(fk_flow, fk_node, workid, fid) {
        var url = "/WF/Chart.aspx?FK_Flow=" + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid + '&FID=' + fid;
       // alert('将要启用子线程:' + url);
        var newWindow = window.open(url, 'z', 'scrollbars=yes,resizable=yes,toolbar=false,location=false');
        newWindow.focus();
        return;
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h3>市局网络分析节点:(每一台设备都要发起一个子流程,只有每条设备的子流程完全处理完成此节点的工作才能向下运动)</h3>
<br />
<asp:Button ID="Btn_Send" runat="server" Text="发送" onclick="Btn_Send_Click" 
           style="width: 40px"  />
        <asp:Button ID="Btn_Save" runat="server" Text="保存" onclick="Btn_Save_Click" />
        <asp:Button ID="Btn_Track" runat="server" Text="轨迹图" onclick="Btn_Track_Click" />
        <hr />
        <%
            Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
            BP.Demo.tab_wf_commonkpiopti pti = new BP.Demo.tab_wf_commonkpiopti();
            pti.Retrieve(BP.Demo.tab_wf_commonkpioptiAttr.WorkID, workid);
         %>
             <fieldset>
             <legend>地市流程主表数据</legend>
        <table border=1 >
        <tr>
        <td>单据编号:<%=pti.wf_no %></td>
        <td>城市:<%=pti.region_id %></td>
        <td>负责人:<%=pti.wf_send_user%></td>
        </tr>
        </table>
            </fieldset>
        <%
            string fk_flow = this.Request.QueryString["FK_Flow"];
            int fk_node = int.Parse(this.Request.QueryString["FK_Node"]);
            Int64 fid = Int64.Parse(this.Request.QueryString["FID"]);

            string sql = "select * from tab_wf_commonkpioptivalue  where  ParentWorkID=" + workid;
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            
             %>

             <fieldset>
             <legend>本地市设备信息：(设备的状态不同，所执行的操作不一样)</legend>
             <table border="1" width="70%">
             <tr>
             <th>负责人</th>
             <th>设备地址</th>
             <th>ParentWorkID</th>
             <th>WorkID</th>
             <th>流程状态</th>
             <th>子线程停留节点</th>
             <th>子线程停留节点ID</th>
             <th>操作</th>
             </tr>
             <%
                 
                 //遍历设备信息表。
                 foreach (System.Data.DataRow dr in dt.Rows)
                 {
                     string shebeiID = dr[BP.Demo.tab_wf_commonkpioptivalueAttr.OID].ToString();
                     string fuzeren = dr[BP.Demo.tab_wf_commonkpioptivalueAttr.fuzeren].ToString();
                     string addr = dr[BP.Demo.tab_wf_commonkpioptivalueAttr.addr].ToString();
                     
                     int sheBeiWorkID = int.Parse(dr[BP.Demo.tab_wf_commonkpioptivalueAttr.WorkID].ToString());
                     int parentWorkID = int.Parse(dr[BP.Demo.tab_wf_commonkpioptivalueAttr.ParentWorkID].ToString());
                     
                     BP.WF.GERpt gerpt=null;
                     BP.WF.WFState wfState = BP.WF.WFState.Draft;
                     
                     string nodeStop = "无";
                     int flowEndNodeID = 0;
                     if (sheBeiWorkID != 0)
                     {
                         gerpt = BP.WF.Dev2Interface.Flow_GenerGERpt("027", sheBeiWorkID);
                         wfState = gerpt.WFState;
                         nodeStop = gerpt.FlowEndNodeText;
                         flowEndNodeID = gerpt.FlowEndNode;
                     }
                   %>
                   <tr>
                   <td><%= fuzeren%></td>
                   <td><%=addr%></td>
                   <td><%=parentWorkID%></td>
                   <td><%=sheBeiWorkID%></td>
                   <td><%=wfState.ToString()%></td>
                   <td><%=nodeStop%> </td>
                   <td><%=flowEndNodeID%> </td>
                   <td>
                  <% if (sheBeiWorkID == 0)
                     {  /* 此设备没有发起子流程 */ %>
                   <a  href="javascript:CallSubFlow('<%= shebeiID %>', '027', '<%=fk_node %>', '<%=workid %>', '<%=fid %>');" >启动设备维护流程</a>
                   <% }  %>


                    <% if (sheBeiWorkID != 0)
                       {  /*此设备没有发起子流程*/ %>

                      <a  href="javascript:Chart( '027', '<%=fk_node %>', '<%=subFlowWorkID %>', '<%=fid %>');" >轨迹图</a>

                   <% }  %>


                     <% if (sheBeiWorkID != 0 && gerpt.FlowEndNode == 2701)
                        {  /*如果停留节点是开始节点，就让它处理工作..*/ %>
                      <a  href="javascript:DealSubFlow('<%=shebeiID %>','<%=sheBeiWorkID %>');" ><b>工作处理</b></a>
                     <% }  %>
                   </td>
                   </tr>
               <% } %>
             </table>
            </fieldset>
</asp:Content>