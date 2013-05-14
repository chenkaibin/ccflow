using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 此接口为程序员二次开发使用,在阅读代码前请注意如下事项.
    /// 1, CCFlow的对外的接口都是以静态方法来实现的.
    /// 2, 以 DB_ 开头的是需要返回结果集合的接口.
    /// 3, 以 Flow_ 是流程接口.
    /// 4, 以 Node_ 是节点接口。
    /// 5, 以 Port_ 是组织架构接口.
    /// 6, 以 DTS_ 是调度． 
    /// 7, 以 UI_ 是流程的功能窗口． 
    /// </summary>
    public class Dev2Interface
    {
        #region 自动执行
        /// <summary>
        /// 处理延期的任务.根据节点属性的设置
        /// </summary>
        /// <returns>返回处理的消息</returns>
        public static string DTS_DealDeferredWork()
        {
            string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <='" + DataType.CurrentData + "' ORDER BY FK_Emp";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            string msg = "";
            string dealWorkIDs = "";
            foreach (DataRow dr in dt.Rows)
            {
                string FK_Emp = dr["FK_Emp"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();
                int fk_node = int.Parse(dr["FK_Node"].ToString());
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                
                // 方式两个人同时处理一件工作, 一个人处理后，另外一个人还可以处理的情况.
                if (dealWorkIDs.Contains("," + workid + ","))
                    continue;
                dealWorkIDs += "," + workid + ",";

                if (WebUser.No != FK_Emp)
                {
                    Emp emp = new Emp(WebUser.No);
                    BP.Web.WebUser.SignInOfGener(emp);
                }

                BP.WF.Ext.NodeSheet nd = new BP.WF.Ext.NodeSheet();
                nd.NodeID = fk_node;
                nd.Retrieve();

                // 首先判断是否有启动的表达式, 它是是否自动执行的总阀门。
                if (string.IsNullOrEmpty(nd.DoOutTimeCond)==false)
                {
                    Node nodeN = new Node(nd.NodeID);
                    Work wk = nodeN.HisWork;
                    wk.OID = workid;
                    wk.Retrieve();
                    string exp=nd.DoOutTimeCond.Clone() as string;
                    if (Glo.ExeExp(exp, wk) == false)
                        continue; // 不能通过条件的设置.
                }

                switch (nd.HisOutTimeDeal)
                {
                    case OutTimeDeal.None:
                        break;
                    case OutTimeDeal.AutoTurntoNextStep: //自动转到下一步骤.
                        if (string.IsNullOrEmpty(nd.OutTimeDeal))
                        {
                            /*如果是空的,没有特定的点允许，就让其它向下执行。*/
                            msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        else
                        {
                            int nextNode = Dev2Interface.Node_GetNextStepNode(fk_flow, workid);
                            if (nd.OutTimeDeal.Contains(nextNode.ToString())) /*如果包含了当前点的ID,就让它执行下去.*/
                                msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        break;
                    case OutTimeDeal.AutoJumpToSpecNode: //自动的跳转下一个节点.
                        if (string.IsNullOrEmpty(nd.OutTimeDeal))
                            throw new Exception("@设置错误,没有设置要跳转的下一步节点.");
                        int nextNodeID=int.Parse(nd.OutTimeDeal);
                        msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, nextNodeID,null).ToMsgOfText();
                        break;
                    case OutTimeDeal.AutoShiftToSpecUser: //移交给指定的人员.
                        msg += BP.WF.Dev2Interface.Node_Shift(workid, nd.OutTimeDeal, "来自ccflow的自动消息:(" + BP.Web.WebUser.Name + ")工作未按时处理(" + nd.Name + "),现在移交给您。");
                        break;
                    case OutTimeDeal.SendMsgToSpecUser: //向指定的人员发消息.
                        BP.WF.Dev2Interface.Port_SendMail(nd.OutTimeDeal,
                            "来自ccflow的自动消息:(" + BP.Web.WebUser.Name + ")工作未按时处理(" + nd.Name + ")", "感谢您选择ccflow.","SpecEmp"+workid);
                        break;
                    case OutTimeDeal.DeleteFlow: //删除流程.
                        msg += BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                        break;
                    case OutTimeDeal.RunSQL:
                        msg += BP.DA.DBAccess.RunSQL( nd.OutTimeDeal);
                        break;
                    default:
                        throw new Exception("@错误没有判断的超时处理方式." + nd.HisOutTimeDeal);
                }
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);
            return msg;
        }
        /// <summary>
        /// 自动执行开始节点数据
        /// 说明:根据自动执行的流程设置，自动启动发起的流程。
        /// 比如：您根据ccflow的自动启动流程的设置，自动启动该流程，不使用ccflow的提供的服务程序，您需要按如下步骤去做。
        /// 1, 写一个自动调度的程序。
        /// 2，根据自己的时间需要调用这个接口。
        /// </summary>
        /// <param name="fl">流程实体,您可以 new Flow(flowNo); 来传入.</param>
        public static void DTS_AutoStarterFlow(Flow fl)
        {
            #region 读取数据.
            BP.Sys.MapExt me = new Sys.MapExt();
            int i = me.Retrieve(MapExtAttr.FK_MapData, "ND" + int.Parse(fl.No) + "01",
                MapExtAttr.ExtType, "PageLoadFull");
            if (i == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = BP.DA.DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }
            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);
                return;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.

            string nodeTable = "ND" + int.Parse(fl.No) + "01";
            MapData md = new MapData(nodeTable);

            foreach (DataRow dr in dtMain.Rows)
            {
                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + md.PTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                    continue; /*说明已经调度过了*/

                string starter = dr["Starter"].ToString();
                if (Web.WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        BP.DA.Log.DefaultLogWriteLineInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + emp.No + "不存在。");
                        continue;
                    }

                    BP.Web.WebUser.SignInOfGener(emp);
                }

                #region  给值.
                Work wk = fl.NewWork();
                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    string refPK = dr["MainPK"].ToString();
                    MapDtls dtls = wk.HisNode.MapData.MapDtls; // new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != refPK)
                                    continue;

                                dtlEn = dtl.HisGEDtl;

                                foreach (DataColumn dc in dt.Columns)
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());

                                dtlEn.RefPK = wk.OID.ToString();
                                dtlEn.Insert();
                            }
                        }
                    }
                }
                #endregion  给值.

                // 处理发送信息.
                Node nd = fl.HisStartNode;
                try
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    string msg = wn.NodeSend().ToMsgOfHtml();
                    //BP.DA.Log.DefaultLogWriteLineInfo(msg);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLineWarning(ex.Message);
                }
            }
            #endregion 处理流程发起.

        }
        #endregion

        #region 数据集合接口(如果您想获取一个结果集合的接口，都是以DB_开头的.)
        /// <summary>
        /// 获取能发起流程的人员
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns></returns>
        public static string GetFlowStarters(string fk_flow)
        {
            BP.WF.Node nd = new Node(int.Parse(fk_flow + "01"));
            string sql="";
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByBindEmp: /*按人员*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + nd.NodeID + ")";
                    break;
                case DeliveryWay.ByDept: /*按部门*/
                    sql = "SELECT * FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID + ")";
                    break;
                case DeliveryWay.ByStation: /*按岗位*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpStation WHERE FK_Station IN ( SELECT FK_Station from WF_nodeStation where FK_Node="+nd.NodeID+")) ";
                    break;
                default:
                    throw new Exception("@开始节点的人员访问规则错误,不允许在开始节点设置此访问类型:" + nd.HisDeliveryWay);
                    break;
            }
            return sql;
        }
        public static string GetFlowStarters(string fk_flow,string fk_dept)
        {
            BP.WF.Node nd = new Node(int.Parse(fk_flow + "01"));
            string sql = "";
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByBindEmp: /*按人员*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + nd.NodeID + ") and fk_dept='" + fk_dept + "'";
                    break;
                case DeliveryWay.ByDept: /*按部门*/
                    sql = "SELECT * FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID + ") and fk_dept='" + fk_dept + "' ";
                    break;
                case DeliveryWay.ByStation: /*按岗位*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpStation WHERE FK_Station IN ( SELECT FK_Station from WF_nodeStation where FK_Node=" + nd.NodeID + ")) and fk_dept='" + fk_dept + "' ";
                    break;
                default:
                    throw new Exception("@开始节点的人员访问规则错误,不允许在开始节点设置此访问类型:" + nd.HisDeliveryWay);
                    break;
            }
            return sql;
        }

        #region 与子流程相关.
        /// <summary>
        /// 获取流程事例的运行轨迹数据.
        /// 说明：使用这些数据可以生成流程的操作日志.
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <returns>GenerWorkFlows</returns>
        public static GenerWorkFlows DB_SubFlows(Int64 workid)
        {
            GenerWorkFlows gwf = new GenerWorkFlows();
            gwf.Retrieve(GenerWorkFlowAttr.PWorkID, workid);
            return gwf;
        }
        #endregion 获取流程事例的轨迹图

        #region 获取流程事例的轨迹图
        /// <summary>
        /// 获取流程事例的运行轨迹数据.
        /// 说明：使用这些数据可以生成流程的操作日志.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <returns>从临时表与轨迹表获取流程轨迹数据.</returns>
        public static DataTable DB_GenerTrack(string fk_flow, Int64 workid, Int64 fid)
        {
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Track WHERE FID=" + dbstr + "FID AND WorkID=" + dbstr + "WorkID ORDER BY RDT";
            ps.Add("WorkID", workid);
            ps.Add("FID", fid);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取一个流程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="userNo">操作员编号</param>
        /// <returns></returns>
        public static DataTable DB_GenerNDxxxRpt(string fk_flow,string userNo)
        {
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Rpt WHERE FlowStarter=" + dbstr + "FlowStarter  ORDER BY RDT";
            ps.Add(GERptAttr.FlowStarter, userNo);
            return DBAccess.RunSQLReturnTable(ps);

        }
        #endregion 获取流程事例的轨迹图

        #region 获取操送列表
        /// <summary>
        /// 获取指定人员的抄送列表
        /// 说明:可以根据这个列表生成指定用户的抄送数据.
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的所有抄送列表,结构同表WF_CCList.</returns>
        public static DataTable DB_CCList(string FK_Emp)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.Add("FK_Emp", FK_Emp);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(未读)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的未读的抄送列表</returns>
        public static DataTable DB_CCList_UnRead(string FK_Emp)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=0";
            ps.Add("FK_Emp", FK_Emp);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已读)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已读的抄送列表</returns>
        public static DataTable DB_CCList_Read(string FK_Emp)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=1";
            ps.Add("FK_Emp", FK_Emp);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已删除)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已删除的抄送列表</returns>
        public static DataTable DB_CCList_Delete(string FK_Emp)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=2";
            ps.Add("FK_Emp", FK_Emp);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion

        #region 获取当前操作员可以发起的流程集合
        /// <summary>
        /// 获取指定人员能够发起流程的集合.
        /// 说明:利用此接口可以生成用户的发起的流程列表.
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>BP.WF.Flows 可发起的流程对象集合,如何使用该方法形成发起工作列表,请参考:\WF\UC\Start.ascx</returns>
        public static Flows DB_GenerCanStartFlowsOfEntities(string userNo)
        {
            // 按岗位计算.
            string sql = "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeStation WHERE FK_Station IN (SELECT FK_Station FROM Port_EmpStation WHERE FK_Emp='" + WebUser.No + "')) ";
            sql += " UNION  "; //按指定的人员计算.
            sql += "  SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeEmp WHERE FK_Emp='" + userNo + "' ) ";
            sql += " UNION  "; // 按岗位计算.
            sql += " SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeDept WHERE FK_Dept IN(SELECT FK_Dept FROM Port_Emp WHERE No='" + userNo + "' UNION SELECT FK_DEPT FROM Port_EmpDept WHERE FK_Emp='" + userNo + "') ) ";

            Flows fls = new Flows();
            BP.En.QueryObject qo = new BP.En.QueryObject(fls);
            qo.AddWhereInSQL("No", sql);
            qo.addAnd();
            qo.AddWhere(FlowAttr.IsOK, true);
            qo.addAnd();
            qo.AddWhere(FlowAttr.IsCanStart, true);
            if (WebUser.IsAuthorize)
            {
                /*如果是授权状态*/
                qo.addAnd();
                WF.Port.WFEmp wfEmp = new Port.WFEmp(WebUser.No);
                qo.AddWhereIn("No", wfEmp.AuthorFlows);
            }

            qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
            qo.DoQuery();
            return fls;
        }
        /// <summary>
        /// 获取指定人员能够发起流程的集合
        /// 说明:利用此接口可以生成用户的发起的流程列表.
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>Datatable类型的数据集合,数据结构与表WF_Flow大致相同. 如何使用该方法形成发起工作列表,请参考:\WF\UC\Start.ascx</returns>
        public static DataTable DB_GenerCanStartFlowsOfDataTable(string userNo)
        {
            // 按岗位计算.
            string sql = "";
            sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeStation WHERE FK_Station IN (SELECT FK_Station FROM Port_EmpStation WHERE FK_Emp='" + WebUser.No + "')) ";
            sql += " UNION  "; //按指定的人员计算.
            sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeEmp WHERE FK_Emp='" + userNo + "' ) ";
            sql += " UNION  "; // 按岗位计算.
            sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeDept WHERE FK_Dept IN(SELECT FK_Dept FROM Port_Emp WHERE No='" + userNo + "' UNION SELECT FK_DEPT FROM Port_EmpDept WHERE FK_Emp='" + userNo + "') ) ";

            Flows fls = new Flows();
            BP.En.QueryObject qo = new BP.En.QueryObject(fls);
            qo.AddWhereInSQL("No", sql);
            qo.addAnd();
            qo.AddWhere(FlowAttr.IsOK, true);
            qo.addAnd();
            qo.AddWhere(FlowAttr.IsCanStart, true);
            if (WebUser.IsAuthorize)
            {
                /*如果是授权状态*/
                qo.addAnd();
                WF.Port.WFEmp wfEmp = new Port.WFEmp(WebUser.No);
                qo.AddWhereIn("No", wfEmp.AuthorFlows);
            }
            qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
            return qo.DoQueryToTable();
        }
        /// <summary>
        /// 获取(同表单)合流点上的子线程
        /// 说明:如果您要想在合流点看到所有的子线程运行的状态.
        /// </summary>
        /// <param name="nodeIDOfHL">合流点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>与表WF_GenerWorkerList结构类同的datatable.</returns>
        public static DataTable DB_GenerHLSubFlowDtl_TB(int nodeIDOfHL, Int64 workid)
        {
            Node nd = new Node(nodeIDOfHL);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            GenerWorkerLists wls = new GenerWorkerLists();
            QueryObject qo = new QueryObject(wls);
            qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Node,
                nd.FromNodes[0].GetValByKey(NodeAttr.NodeID));

            DataTable dt = qo.DoQueryToTable();
            if (dt.Rows.Count == 1)
            {
                qo.clear();
                qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
                return qo.DoQueryToTable();
            }
            return dt;
        }
        /// <summary>
        /// 获取(异表单)合流点上的子线程
        /// </summary>
        /// <param name="nodeIDOfHL">合流点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>与表WF_GenerWorkerList结构类同的datatable.</returns>
        public static DataTable DB_GenerHLSubFlowDtl_YB(int nodeIDOfHL, Int64 workid)
        {
            Node nd = new Node(nodeIDOfHL);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            GenerWorkerLists wls = new GenerWorkerLists();
            QueryObject qo = new QueryObject(wls);
            qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsPass, 0);
            return qo.DoQueryToTable();
        }
        #endregion 获取当前操作员可以发起的流程集合

        #region 流程草稿
        /// <summary>
        /// 获取当前操作员的指定流程的流程草稿数据
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>返回草稿数据集合,列信息. OID=工作ID,Title=标题,RDT=记录日期,FK_Flow=流程编号,FID=流程ID, FK_Node=节点ID</returns>
        public static DataTable DB_GenerDraftDataTable(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            /*获取数据.*/
            Flow fl = new Flow(fk_flow);
            string dbStr = BP.SystemConfig.AppCenterDBVarStr;


            int val = (int)WFState.Draft;
            BP.DA.Paras ps = new BP.DA.Paras();
            ps.SQL = "SELECT OID,Title,RDT,'" + fk_flow + "' as FK_Flow,FID, " + int.Parse(fk_flow) + "01 as FK_Node FROM " + fl.PTable + " WHERE WFState=" + val + " AND FlowStarter=" + dbStr + "FlowStarter";
            ps.Add(GERptAttr.FlowStarter, BP.Web.WebUser.No);

            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        #endregion 流程草稿

        #region 获取当前操作员的待办工作
        /// <summary>
        /// 获取当前人员待处理的工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>待办工作列表</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            Paras ps = new Paras();
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            string sql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                if (string.IsNullOrEmpty(fk_flow))
                {
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                }
                else
                {
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND FK_Flow=" + dbstr + "FK_Flow ORDER BY  ADT DESC ";
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.SpecFlows:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        sql = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);

        }
        /// <summary>
        /// 根据状态获取当前操作员的待办工作
        /// </summary>
        /// <param name="wfState">流程状态</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>表结构与视图WF_EmpWorks一致</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(WFState wfState, string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            Paras ps = new Paras();
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            string sql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                if (string.IsNullOrEmpty(fk_flow))
                {
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";
                    ps.Add("WFState", (int)wfState);
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                }
                else
                {
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow=" + dbstr + "FK_Flow ORDER BY  ADT DESC ";
                    ps.Add("WFState", (int)wfState);
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.SpecFlows:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取当前操作人员的待办信息
        /// 数据内容请参考图:WF_EmpWorks
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable()
        {
            Paras ps = new Paras();
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            string wfSql = " WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Forward + " OR WFState=" + (int)WFState.ReturnSta;
            string sql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/

                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.SpecFlows:
                    sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
             
        }
        /// <summary>
        /// 获得所有的流程挂起工作列表
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerHungUpList()
        {
            return DB_GenerHungUpList(null);
        }
        /// <summary>
        /// 获得指定流程挂起工作列表
        /// </summary>
        /// <param name="fk_flow">流程编号,如果编号为空则返回所有的流程挂起工作列表.</param>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerHungUpList(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.HungUp;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.FK_Flow='" + fk_flow + "' AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND  B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1   ";
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 ";
            }
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
        }
        /// <summary>
        /// 获得逻辑删除的流程
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerDeleteWorkList()
        {
            return DB_GenerDeleteWorkList(WebUser.No, null);
        }
        /// <summary>
        /// 获得逻辑删除的流程:根据流程编号
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <param name="fk_flow">流程编号(可以为空)</param>
        /// <returns>WF_GenerWorkFlow数据结构的集合</returns>
        public static DataTable DB_GenerDeleteWorkList(string userNo, string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.Delete;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND  B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1   ";
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 ";
            }
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
        }
        #endregion 获取当前操作员的待办工作

        #region 获取流程数据
        /// <summary>
        /// 根据流程状态获取指定流程数据
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="sta">流程状态</param>
        /// <returns>数据表OID,Title,RDT,FID</returns>
        public static DataTable DB_NDxxRpt(string fk_flow, WFState sta)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            Flow fl = new Flow(fk_flow);
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            string sql = "SELECT OID,Title,RDT,FID FROM "+fl.PTable+" WHERE WFState=" + (int)sta + " AND Rec=" + dbstr + "Rec";
            BP.DA.Paras ps = new BP.DA.Paras();
            ps.SQL = sql;
            ps.Add("Rec", BP.Web.WebUser.No);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion

        #region 获取当前可以退回的节点。
        /// <summary>
        /// 获取当前节点可以退回的节点
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <returns>No节点编号,Name节点名称,Rec记录人,RecName记录人名称</returns>
        public static DataTable DB_GenerWillReturnNodes(int fk_node, Int64 workid, Int64 fid)
        {
            DataTable dt = new DataTable("obt");
            dt.Columns.Add("No"); // 节点ID
            dt.Columns.Add("Name"); // 节点名称.
            dt.Columns.Add("Rec"); // 被退回节点上的操作员编号.
            dt.Columns.Add("RecName"); // 被退回节点上的操作员名称.

            Node nd = new Node(fk_node);
            if (nd.HisRunModel == RunModel.SubThread)
            {
                /*如果是子线程，它只能退回它的上一个节点，现在写死了，其它的设置不起作用了。*/
                Nodes nds = nd.FromNodes;
                foreach (Node ndFrom in nds)
                {
                    Work wk;
                    switch (ndFrom.HisRunModel)
                    {
                        case RunModel.FL:
                        case RunModel.FHL:
                            wk = ndFrom.HisWork;
                            wk.OID = fid;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.SubThread:
                            wk = ndFrom.HisWork;
                            wk.OID = workid;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.Ordinary:
                        default:
                            throw new Exception("流程设计异常，子线程的上一个节点不能是普通节点。");
                            break;
                    }
                    DataRow dr = dt.NewRow();
                    dr["No"] = ndFrom.NodeID;
                    dr["Name"] = wk.RecText + "=>" + ndFrom.Name;
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (nd.IsHL || nd.IsFLHL)
            {
                /*如果当前点是分流，或者是分合流，就不按退回规则计算了。*/
                string sql = "SELECT FK_Node AS No,FK_NodeText as Name, FK_Emp as Rec, FK_EmpText as RecName FROM WF_GenerWorkerlist WHERE FID=" + fid + " AND WorkID=" + workid + " ORDER BY RDT  ";
                return DBAccess.RunSQLReturnTable(sql);
            }


            WorkNode wn = new WorkNode(workid, fk_node);
            WorkNodes wns = new WorkNodes();
            switch (nd.HisReturnRole)
            {
                case ReturnRole.CanNotReturn:
                    return dt;
                case ReturnRole.ReturnAnyNodes:
                    if (wns.Count == 0)
                        wns.GenerByWorkID(wn.HisNode.HisFlow, workid);
                    foreach (WorkNode mywn in wns)
                    {
                        if (mywn.HisNode.NodeID == fk_node)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["No"] = mywn.HisNode.NodeID.ToString();
                        dr["Name"] = mywn.HisNode.Name;

                        dr["Rec"] = mywn.HisWork.Rec;
                        dr["RecName"] = mywn.HisWork.RecText;
                        dt.Rows.Add(dr);
                    }
                    break;
                case ReturnRole.ReturnPreviousNode:
                    WorkNode mywnP = wn.GetPreviousWorkNode();
                    //  turnTo = mywnP.HisWork.Rec + mywnP.HisWork.RecText;
                    DataRow dr1 = dt.NewRow();
                    dr1["No"] = mywnP.HisNode.NodeID.ToString();
                    dr1["Name"] = mywnP.HisNode.Name;


                    dr1["Rec"] = mywnP.HisWork.Rec;
                    dr1["RecName"] = mywnP.HisWork.RecText;
                    dt.Rows.Add(dr1);
                    break;
                case ReturnRole.ReturnSpecifiedNodes: //退回指定的节点。
                    if (wns.Count == 0)
                        wns.GenerByWorkID(wn.HisNode.HisFlow, workid);

                    NodeReturns rnds = new NodeReturns();
                    rnds.Retrieve(NodeReturnAttr.FK_Node, fk_node);
                    if (rnds.Count == 0)
                        throw new Exception("@流程设计错误，您设置该节点可以退回指定的节点，但是指定的节点集合为空，请在节点属性设置它的制订节点。");
                    foreach (WorkNode mywn in wns)
                    {
                        if (mywn.HisNode.NodeID == fk_node)
                            continue;

                        if (rnds.Contains(NodeReturnAttr.ReturnTo,
                            mywn.HisNode.NodeID) == false)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["No"] = mywn.HisNode.NodeID.ToString();
                        dr["Name"] = mywn.HisNode.Name;
                        dr["Rec"] = mywn.HisWork.Rec;
                        dr["RecName"] = mywn.HisWork.RecText;
                        dt.Rows.Add(dr);
                    }
                    break;
                case ReturnRole.ByReturnLine: //按照流程图画的退回线执行退回.
                    Directions dirs = new Directions();
                    dirs.Retrieve(DirectionAttr.Node, fk_node, DirectionAttr.DirType, 1);
                    if (dirs.Count == 0)
                        throw new Exception("@流程设计错误:当前节点没有画向后退回的退回线,更多的信息请参考退回规则.");
                    foreach (Direction dir in dirs)
                    {
                        Node toNode = new Node(dir.ToNode);
                        string sql = "SELECT FK_Emp,FK_EmpText FROM WF_GenerWorkerlist WHERE FK_Node="+toNode.NodeID+" AND IsEnable=1 AND IsPass=1";
                        DataTable dt1 = DBAccess.RunSQLReturnTable(sql);
                        if (dt1.Rows.Count == 0)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["No"] = toNode.NodeID.ToString();
                        dr["Name"] = toNode.Name;
                        dr["Rec"] = dt1.Rows[0][0];
                        dr["RecName"] = dt1.Rows[0][1];
                        dt.Rows.Add(dr);
                    }
                    break;
                default:
                    throw new Exception("@没有判断的退回类型。");
            }

            if (dt.Rows.Count == 0)
                throw new Exception("@没有计算出来要退回的节点，请管理员确认节点退回规则是否合理？");

            return dt;
        }
        #endregion 获取当前可以退回的节点

        #region 获取当前操作员的在途工作
        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.Runing;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 ";
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 ";
            }
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
        }
        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning()
        {
            return DB_GenerRuning(null);
        }
        #endregion 获取当前操作员的待办工作

        #endregion

        #region 登陆接口
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="SID">安全ID,请参考流程设计器操作手册</param>
        public static void Port_Login(string userNo, string sid)
        {
            string sql = "select sid from port_emp where no='" + userNo + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("用户不存在或者SID错误。");

            if (dt.Rows[0]["SID"].ToString() != sid)
                throw new Exception("用户不存在或者SID错误。");

            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, true);
            WebUser.IsWap = false;
            return;
        }
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        public static void Port_Login(string userNo)
        {
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, true);
            WebUser.IsWap = false;
            return;
        }
        /// <summary>
        /// 注销当前登录
        /// </summary>
        public static void Port_SigOut()
        {
            WebUser.Exit();
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailAddress">邮件地址</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <param name="msgFlag">标记(唯一的标记码，如果重复了就不发送了，可以为空.)</param>
        public static void Port_SendMailByAddress(string mailAddress, string msgTitle, string msgDoc, string msgFlag)
        {
            BP.Sys.SMS.SendEmail(mailAddress, msgTitle, msgDoc, msgFlag);
        }
        /// <summary>
        /// 发送邮件与消息
        /// </summary>
        /// <param name="userNo">信息接收人</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        public static void Port_SendMail(string userNo, string msgTitle, string msgDoc,string msgFlag)
        {
            Port_SendMail(userNo, msgTitle, msgDoc, msgFlag,null,0,0,0);
        }
        /// <summary>
        /// 发送邮件与消息(如果传入4大流程参数将会增加一个工作链接)
        /// </summary>
        /// <param name="userNo">信息接收人</param>
        /// <param name="title">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <param name="msgFlag">消息标志</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        public static void Port_SendMail(string userNo, string title, string msgDoc, string msgFlag, string flowNo, Int64 nodeID, Int64 workID, Int64 fid)
        {
            if (workID != 0)
                msgDoc += " <hr>打开工作: http://sss/ss.aspx?WorkID=" + workID + "&FK_Node=" + nodeID + "&FK_Flow=" + flowNo + "&FID=" + fid;

            BP.WF.Port.WFEmp emp = new Port.WFEmp(userNo);
            BP.Sys.SMS.SendEmail(emp.Email, title, msgDoc, msgFlag);
        }
        /// <summary>
        /// 发送SMS
        /// </summary>
        /// <param name="userNo">信息接收人</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        public static void Port_SendSMS(string tel, string telDoc)
        {
            BP.Sys.SMS.SendSMS(tel,telDoc);
        }
        
        /// <summary>
        /// 转化流程Code到流程编号
        /// </summary>
        /// <param name="flowCode">流程编号</param>
        /// <returns>返回编码</returns>
        public static string TurnFlowCodeToFlowNo(string flowCode)
        {
            if (string.IsNullOrEmpty(flowCode))
                return null;

            // 如果是编号，就不用转化.
            if (DataType.IsNumStr(flowCode))
                return flowCode;

            string s= DBAccess.RunSQLReturnStringIsNull("SELECT No FROM WF_Flow WHERE FlowCode='" + flowCode + "'", null);
            if (s == null)
                throw new Exception("@FlowCode错误:"+flowCode+",没有找到它的流程编号.");
            return s;
        }
        #endregion 登陆接口

        #region 与流程有关的接口
        public static GERpt Flow_GenerGERpt(string flowNo, Int64 workID )
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt", workID);
            return rpt;
        }
        /// <summary>
        /// 产生一个新的工作ID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>返回当前操作员创建的工作ID</returns>
        public static Int64 Flow_GenerWorkID(string flowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            Flow fl = new Flow(flowNo);
            return fl.NewWork().OID;
        }
        /// <summary>
        /// 产生一个新的工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>返回当前操作员创建的工作</returns>
        public static Work Flow_GenerWork(string flowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            Flow fl = new Flow(flowNo);
            Work wk = fl.NewWork();
            wk.ResetDefaultVal();
            return wk;
        }
       　
        /// <summary>
        /// 把流程从非正常运行状态恢复到正常运行状态
        /// 比如现在的流程的状态是，删除，挂起，现在恢复成正常运行。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">原因</param>
        /// <returns>执行信息</returns>
        public static void Flow_DoComeBackWrokFlow(string flowNo, Int64 workID, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            wf.DoComeBackWrokFlow(msg);
        }
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// 恢复失败抛出异常
        /// </summary>
        /// <param name="flowNo">要恢复的流程编号</param>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note">恢复的原因，此原因会记录到日志</param>
        public static string Flow_DoRebackWorkFlow(string flowNo, Int64 workid, int backToNodeID, string note)
        {
            BP.WF.Ext.FlowSheet fs = new Ext.FlowSheet(flowNo);
            return fs.DoRebackFlowData(workid, backToNodeID, note);
        }
        /// <summary>
        /// 执行删除流程:彻底的删除流程.
        /// 清除的内容如下:
        /// 1, 流程引擎中的数据.
        /// 2, 节点数据,NDxxRpt数据.
        /// 3, 轨迹表数据.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除它的子流程</param>
        /// <returns>执行信息</returns>
        public static string Flow_DoDeleteFlowByReal(string flowNo, Int64 workID, bool isDelSubFlow)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);
            WorkFlow wf = new WorkFlow(flowNo, workID);
            wf.DoDeleteWorkFlowByReal(isDelSubFlow);
            return "删除成功";
        }
        /// <summary>
        /// 删除已经完成的流程
        /// 注意:它不触发事件.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否删除子流程</param>
        /// <param name="note">删除原因</param>
        /// <returns>删除过程信息</returns>
        public static string Flow_DoDeleteWorkFlowAlreadyComplete(string flowNo, Int64 workID, bool isDelSubFlow, string note)
        {
            return WorkFlow.DoDeleteWorkFlowAlreadyComplete(flowNo, workID, isDelSubFlow, note);
        }
        /// <summary>
        /// 删除流程并写入日志
        /// 清除的内容如下:
        /// 1, 流程引擎中的数据.
        /// 2, 节点数据,NDxxRpt数据.
        /// 并作如下处理:
        /// 1, 保留track数据.
        /// 2, 写入流程删除记录表.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="deleteNote">删除原因</param>
        /// <param name="isDelSubFlow">是否要删除它的子流程</param>
        /// <returns>执行信息</returns>
        public static string Flow_DoDeleteFlowByWriteLog(string flowNo, Int64 workID, string deleteNote, bool isDelSubFlow)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);
            WorkFlow wf = new WorkFlow(flowNo, workID);
            return wf.DoDeleteWorkFlowByWriteLog(deleteNote, isDelSubFlow);
        }
        /// <summary>
        /// 执行逻辑删除流程:此流程并非真正的删除仅做了流程删除标记
        /// 比如:逻辑删除工单.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">逻辑删除的原因</param>
        /// <param name="isDelSubFlow">逻辑删除的原因</param>
        /// <returns>执行信息,执行不成功抛出异常.</returns>
        public static string Flow_DoDeleteFlowByFlag(string flowNo, Int64 workID, string msg, bool isDelSubFlow)
        {

            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            wf.DoDeleteWorkFlowByFlag(msg);
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                {
                    Flow_DoDeleteFlowByFlag(item.FK_Flow, item.WorkID, "删除子流程:" + msg, false);
                }
            }
            return "删除成功";
        }
        /// <summary>
        /// 撤销删除流程
        /// 说明:如果一个流程处于逻辑删除状态,要回复正常运行状态,就执行此接口.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作流程ID</param>
        /// <param name="msg">撤销删除的原因</param>
        /// <returns>执行消息,如果撤销不成功则抛出异常.</returns>
        public static string Flow_DoUnDeleteFlowByFlag(string flowNo, Int64 workID, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            wf.DoUnDeleteWorkFlowByFlag(msg);
            return "撤销删除成功.";
        }
      
        /// <summary>
        /// 执行-撤销发送
        /// 说明:如果流程转入了下一个节点,就会执行失败,就会抛出异常.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回成功执行信息</returns>
        public static string Flow_DoUnSend(string flowNo, Int64 workID)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            return wf.DoUnSend();
        }
        /// <summary>
        /// 执行流程结束
        /// 说明:正常的流程结束.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">流程结束原因</param>
        /// <returns>返回成功执行信息</returns>
        public static string Flow_DoFlowOver(string flowNo, Int64 workID, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            return wf.DoFlowOver(ActionType.FlowOver, msg);
        }
        /// <summary>
        /// 执行流程结束:强制的流程结束.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">强制流程结束的原因</param>
        /// <returns>执行强制结束流程</returns>
        public static string Flow_DoFlowOverByCoercion(string flowNo, Int64 workID, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            return wf.DoFlowOver(ActionType.FlowOverByCoercion, msg);
        }
        /// <summary>
        /// 获得执行下一步骤的节点ID，这个功能是在流程未发送前可以预先知道
        /// 它就要到达那一个节点上去,以方便在当前节点发送前处理业务逻辑.
        /// 1,首先保证当前人员是可以执行当前节点的工作.
        /// 2,其次保证获取下一个节点只有一个.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>下一步骤的所要到达的节点, 如果获取不到就会抛出异常.</returns>
        public static int Node_GetNextStepNode(string fk_flow, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            ////检查当前人员是否可以执行当前工作.
            //if (BP.WF.Dev2Interface.Flow_CheckIsCanDoCurrentWork( workid, WebUser.No) == false)
            //    throw new Exception("@当前人员不能执行此节点上的工作.");

            //获取当前nodeID.
            int currNodeID=BP.WF.Dev2Interface.Node_GetCurrentNodeID(fk_flow, workid);

            //获取
            Node nd =new Node(currNodeID) ;
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            WorkNode wn = new WorkNode(wk, nd);
            return wn.NodeSend_GenerNextStepNode().NodeID;
        }
        /// <summary>
        /// 获取指定的workid 在运行到的节点编号
        /// </summary>
        /// <param name="workID">需要找到的workid</param>
        /// <returns>返回节点编号. 如果没有找到，就会抛出异常.</returns>
        public static int Flow_GetCurrentNode(Int64 workID)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return BP.DA.DBAccess.RunSQLReturnValInt(ps);
        }
        /// <summary>
        /// 获取指定节点的Work
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>当前工作</returns>
        public static Work Flow_GetCurrentWork(int nodeID, Int64 workID)
        {
            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            return wk;
        }
        /// <summary>
        /// 获取当前工作节点的Work
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <returns>当前工作节点的Work</returns>
        public static Work Flow_GetCurrentWork(Int64 workID)
        {
            Node nd = new Node(Flow_GetCurrentNode(workID));
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            wk.ResetDefaultVal();
            return wk;
        }
        /// <summary>
        /// 指定 workid 当前节点由哪些人可以执行.
        /// </summary>
        /// <param name="workID">需要找到的workid</param>
        /// <returns>返回当前处理人员列表,数据结构与WF_GenerWorkerList一致.</returns>
        public static DataTable Flow_GetWorkerList(Int64 workID)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_GenerWorkerList WHERE IsEnable=1 AND IsPass=0 AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 检查是否可以发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>是否可以发起当前流程</returns>
        public static bool Flow_IsCanStartThisFlow(string flowNo, string userNo)
        {
            Node nd = new Node(int.Parse(flowNo + "01"));
            Paras ps = new Paras();
            string dbstr = SystemConfig.AppCenterDBVarStr;
            int num = 0;
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByStation:
                    ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeStation WHERE FK_Station IN (SELECT FK_Station FROM Port_EmpStation WHERE FK_Emp=" + dbstr + "FK_Emp) AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add("FK_Emp", userNo);
                    ps.Add("FK_Node", nd.NodeID);
                    num= DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByDept:
                    ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeDept WHERE FK_Dept IN (SELECT FK_Dept FROM Port_EmpDept WHERE FK_Emp=" + dbstr + "FK_Emp) AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add("FK_Emp", userNo);
                    ps.Add("FK_Node", nd.NodeID);
                    num= DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByBindEmp:
                       ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeEmp WHERE FK_Dept IN (SELECT FK_Dept FROM Port_EmpDept WHERE FK_Emp=" + dbstr + "FK_Emp) AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add("FK_Emp", userNo);
                    ps.Add("FK_Node", nd.NodeID);
                    num= DBAccess.RunSQLReturnValInt(ps);
                    break;
                default:
                    throw new Exception("@开始节点不允许设置此访问规则："+nd.HisDeliveryWay);
            }
            if (num == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 检查当前人员是否有权限处理当前的工作.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">要判断的操作人员</param>
        /// <returns>返回指定的人员是否有操作当前工作的权限</returns>
        public static bool Flow_IsCanDoCurrentWork(int nodeID, Int64 workID, string userNo)
        {
            if (workID == 0)
                return true;
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT c.RunModel FROM WF_GenerWorkFlow a , WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node=" + dbstr + "FK_Node AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND b.IsEnable=1 AND a.workid=" + dbstr + "WorkID";
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", userNo);
            ps.Add("WorkID", workID);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
                return false;

            int i = int.Parse(dt.Rows[0][0].ToString());
            RunModel rm = (RunModel)i;
            switch (rm)
            {
                case RunModel.Ordinary:
                    return true;
                case RunModel.FL:
                    return true;
                case RunModel.HL:
                    return true;
                case RunModel.FHL:
                    return true;
                case RunModel.SubThread:
                    return true;
                default:
                    break;
            }

            if (DBAccess.RunSQLReturnValInt(ps) == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 检查当前人员是否有查看指定流程的权限
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>返回是否可以查看</returns>
        public static bool Flow_IsCanViewCurrentWork(string fk_flow, Int64 workID, string userNo)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(*) FROM ND"+int.Parse(fk_flow)+"Track WHERE WorkID=" + dbstr + "WorkID AND (EmpFrom=" + dbstr + "user1 OR EmpTo=" + dbstr + "user2)";
            ps.Add("WorkID", workID);
            ps.Add("user1", userNo);
            ps.Add("user2", userNo);
            if (DBAccess.RunSQLReturnValInt(ps) == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 创建一个工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>要创建的工作，此工作已经具备了WorkID。</returns>
        public static StartWork Flow_NewStartWork(string flowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            Flow fl = new Flow(flowNo);
            return fl.NewWork();
        }
        /// <summary>
        /// 执行工作催办
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">催办消息</param>
        /// <param name="isPressSubFlow">是否催办子流程？</param>
        /// <returns>返回执行结果</returns>
        public static string Flow_DoPress(Int64 workID, string msg, bool isPressSubFlow)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            /*找到当前待办的工作人员*/
            GenerWorkerLists wls = new GenerWorkerLists(workID, gwf.FK_Node);
            string toEmp = "", toEmpName = "";
            string mailTitle = "催办:" + gwf.Title + ", 发送人:" + WebUser.Name;
            if (wls.Count == 1)
            {
                GenerWorkerList gwl = (GenerWorkerList)wls[0];
                toEmp = gwl.FK_Emp;
                toEmpName = gwl.FK_EmpText;

                BP.WF.Dev2Interface.Port_SendMail(toEmp, mailTitle, msg, "Press", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

                gwl.PressTimes = gwl.PressTimes + 1;
                gwl.Update();
                //gwl.PRI = 1;
            }
            else
            {
                foreach (GenerWorkerList wl in wls)
                {
                    if (wl.IsEnable == false)
                        continue;

                    toEmp += wl.FK_Emp + ",";
                    toEmpName += wl.FK_EmpText + ",";


                    BP.WF.Dev2Interface.Port_SendMail(wl.FK_Emp, mailTitle, msg, "Press", gwf.FK_Flow,gwf.FK_Node,gwf.WorkID,gwf.FID);

                    wl.Update(GenerWorkerListAttr.PressTimes, wl.PressTimes + 1);
                }
            }

            //写入日志.
            WorkNode wn = new WorkNode(workID, gwf.FK_Node);
            wn.AddToTrack(ActionType.Press, toEmp, toEmpName, gwf.FK_Node, gwf.NodeName, msg);

            if (isPressSubFlow)
            {
                string subMsg = "";
                GenerWorkFlows gwfs = gwf.HisSubFlowGenerWorkFlows;
                foreach (GenerWorkFlow item in gwfs)
                {
                    subMsg += "@已经启动对子线程:" + item.Title + "的催办,消息如下:";
                    subMsg += Flow_DoPress(item.WorkID, msg, false);
                }
                return "系统已经把您的信息通知给:" + toEmpName + "" + subMsg;
            }
            else
            {
                return "系统已经把您的信息通知给:" + toEmpName;
            }
        }
        /// <summary>
        /// 重新设置流程标题
        /// 可以在节点的任何位置调用它,产生新的标题。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>是否设置成功</returns>
        public static bool Flow_ReSetFlowTitle(string flowNo, int nodeID, Int64 workid)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            Node nd=new Node(nodeID);
            Work wk= nd.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            Flow fl=nd.HisFlow;
            string title = BP.WF.WorkNode.GenerTitle(fl, wk);
            return Flow_SetFlowTitle(flowNo, workid, title);
        }
      
        /// <summary>
        /// 设置流程标题
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="title">标题</param>
        /// <returns>是否设置成功</returns>
        public static bool Flow_SetFlowTitle(string flowNo, Int64 workid, string title)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + dbstr + "Title WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET Title=" + dbstr + "Title WHERE OID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            int num = DBAccess.RunSQL(ps);

            if (fl.HisDataStoreModel == DataStoreModel.ByCCFlow)
            {
                ps = new Paras();
                ps.SQL = "UPDATE ND" + int.Parse(flowNo + "01") + " SET Title=" + dbstr + "Title WHERE OID=" + dbstr + "WorkID";
                ps.Add(GenerWorkFlowAttr.Title, title);
                ps.Add(GenerWorkFlowAttr.WorkID, workid);
                DBAccess.RunSQL(ps);
            }

            if (num == 0)
                return false;
            else
                return true;
        }
        #endregion 与流程有关的接口

        #region get 属性节口
        /// <summary>
        /// 获得流程运行过程中的参数
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>如果没有就返回null,有就返回@参数名0=参数值0@参数名1=参数值1</returns>
        public static string GetFlowParas(int nodeID, Int64 workid)
        {
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT Paras FROM WF_GenerWorkerlist WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            return DBAccess.RunSQLReturnStringIsNull(ps, null);
        }
        #endregion get 属性节口

        #region 工作有关接口
        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">节点表单:主表数据以Key Value 方式传递(可以为空)</param>
        /// <param name="workDtls">节点表单:从表数据，从表名称与从表单的从表编号要对应(可以为空)</param>
        /// <param name="nextNodeID">发起后要跳转到的节点(可以为空)</param>
        /// <param name="nextWorker">发起后要跳转到的节点并指定的工作人员(可以为空)</param>
        /// <returns>发送到第二个节点的执行信息</returns>
        public static SendReturnObjs Node_StartWork(string flowNo, Hashtable ht, DataSet workDtls,
           int nextNodeID, string nextWorker)
        {
            return Node_StartWork(flowNo, ht, workDtls, nextNodeID, nextWorker, 0, null);
        }
       /// <summary>
       /// 发起流程
       /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="htWork">节点表单:主表数据以Key Value 方式传递(可以为空)</param>
        /// <param name="workDtls">节点表单:从表数据，从表名称与从表单的从表编号要对应(可以为空)</param>
        /// <param name="nextNodeID">发起后要跳转到的节点(可以为空)</param>
        /// <param name="nextWorker">发起后要跳转到的节点并指定的工作人员(可以为空)</param>
       /// <param name="parentWorkID">父流程的workid，如果没有可以为0</param>
       /// <param name="parentFlowNo">父流程的编号，如果没有可以为空</param>
        /// <returns>发送到第二个节点的执行信息</returns>
        public static SendReturnObjs Node_StartWork(string flowNo, Hashtable htWork, DataSet workDtls,
            int nextNodeID, string nextWorker, Int64 parentWorkID,string parentFlowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            //
            parentFlowNo = TurnFlowCodeToFlowNo(parentFlowNo);
            Flow fl = new Flow(flowNo);
            Work wk = fl.NewWork();
            Int64 workID = wk.OID;
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                    wk.SetValByKey(str, htWork[str]);
            }

            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }

            WorkNode wn = new WorkNode(wk, fl.HisStartNode);

            Node nextNoode = null;
            if (nextNodeID != 0)
                nextNoode = new Node(nextNodeID);

            SendReturnObjs objs = wn.NodeSend(nextNoode, nextWorker);
            if (parentWorkID != 0)
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET PWorkID=" + parentWorkID + ",PFlowNo='"+parentFlowNo+"' WHERE WorkID=" + objs.VarWorkID);

            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                    paras += "@" + key + "=" +  htWork[key].ToString();

                if (string.IsNullOrEmpty(paras) == false)
                {
                    string dbstr = SystemConfig.AppCenterDBVarStr;
                    Paras ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist set Paras=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, int.Parse(flowNo+"01"));
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.

            return objs;
        }
        /// <summary>
        /// 发起工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="nextWorker">操作员，如果为null就是当前人员。</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht, DataSet workDtls,
            string starter, string title)
        {
            return Node_CreateBlankWork(flowNo, ht, workDtls, starter, title, 0, null);
        }
        /// <summary>
        /// 创建开始节点工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="starter">流程的发起人</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <param name="parentWorkID">父流程的WorkID,如果没有父流程就传入为0.</param>
        /// <param name="parentFlowNo">父流程的流程编号,如果没有父流程就传入为null.</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht, DataSet workDtls,
            string starter, string title, Int64 parentWorkID, string parentFlowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            //转化成编号
            parentFlowNo = TurnFlowCodeToFlowNo(parentFlowNo);

            string dbstr = SystemConfig.AppCenterDBVarStr;

            if (string.IsNullOrEmpty(starter))
                starter = WebUser.No;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp empStarter = new Emp(starter);
            Work wk = fl.NewWork(starter);
            Int64 workID = wk.OID;

            #region 给各个属性-赋值
            if (ht != null)
            {
                foreach (string str in ht.Keys)
                    wk.SetValByKey(str, ht[str]);
            }
            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            Paras ps = new Paras();
            // 执行对报表的数据表WFState状态的更新,让它为runing的状态.
            if (string.IsNullOrEmpty(title) == false)
            {
                if (fl.TitleRole != "@OutPara")
                {
                    fl.TitleRole = "@OutPara";
                    fl.Update();
                }

                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.Title, title);
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }
            else
            {
                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,FK_Dept=" + dbstr + "FK_Dept,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.FK_Dept, empStarter.FK_Dept);
                ps.Add(GERptAttr.Title, WorkNode.GenerTitle(fl,wk));
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }

            // 删除有可能产生的垃圾数据,比如上一次没有发送成功，导致数据没有清除.
            ps = new Paras();
            ps.SQL = "DELETE WF_GenerWorkFlow  WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2";
            ps.Add("WorkID1", wk.OID);
            ps.Add("WorkID2", wk.OID);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "DELETE WF_GenerWorkerList  WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2";
            ps.Add("WorkID1", wk.OID);
            ps.Add("WorkID2", wk.OID);
            DBAccess.RunSQL(ps);

            ////写入日志.
            //WorkNode wn = new WorkNode(wk, nd);
            //wn.AddToTrack(ActionType.CallSubFlow, starter, emp.Name, nd.NodeID, nd.Name, "来自" + WebUser.No + "," + WebUser.Name
            //    + "工作发起.");

            return wk.OID;
        }
        /// <summary>
        /// 创建开始节点工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="flowStarter">流程的发起人，如果为null就是当前人员。</param>
        /// <param name="beloneDeptNo">该流程隶属于的部门编号</param>
        /// <param name="starters">开始工作节点的人员集合</param>
        /// <param name="title">流程标题</param>
        /// <param name="parentFlowNo">父流程编号，可以为空</param>
        /// <param name="parentWorkID">父流程ID,可以为0.</param>
        /// <returns>为子流程开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateStartNodeWork(string flowNo, Hashtable ht, DataSet workDtls, string flowStarter,
            string beloneDeptNo, List<string> starters, string title, string parentFlowNo, Int64 parentWorkID)
        {
            string strs = "";
            foreach (string item in starters)
                strs += item + ",";
            return Node_CreateStartNodeWork(flowNo, ht, workDtls,
           flowStarter, beloneDeptNo, strs, title, parentFlowNo, parentWorkID);
        }
        /// <summary>
        /// 创建开始节点工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="htWork">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="flowStarter">流程的发起人，如果为null就是当前人员。</param>
        /// <param name="beloneDeptNo">该流程隶属于的部门编号</param>
        /// <param name="starters">开始工作节点的人员集合</param>
        /// <param name="title">流程标题</param>
        /// <param name="parentFlowNo">父流程编号，可以为空</param>
        /// <param name="parentWorkID">父流程ID,可以为0.</param>
        /// <returns>为子流程开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateStartNodeWork(string flowNo, Hashtable htWork, DataSet workDtls,
          string flowStarter, string beloneDeptNo, string flowStarters, string title, string parentFlowNo, Int64 parentWorkID)
        {
            //转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);
            //转化成编号
            parentFlowNo = TurnFlowCodeToFlowNo(parentFlowNo);

            if (string.IsNullOrEmpty(flowStarter))
                flowStarter = WebUser.No;

            Flow fl = new Flow(flowNo);

            #region 处理流程标题.
            if (string.IsNullOrEmpty(title) == false && fl.TitleRole != "@OutPara")
            {
                /*如果标题不为空*/
                fl.TitleRole = "@OutPara"; //特殊标记不为空.
                fl.Update();
            }
            if (string.IsNullOrEmpty(title) == true && fl.TitleRole == "@OutPara")
            {
                /*如果标题为空 */
                fl.TitleRole = ""; //特殊标记不为空.
                fl.Update();
            }
            #endregion 处理流程标题.

            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp emp = new Emp(flowStarter);

            // 如果隶属部门为空.
            if (string.IsNullOrEmpty(beloneDeptNo))
                beloneDeptNo = emp.FK_Dept;

            // new 一个work.
            Work wk = fl.NewWork(flowStarter);
            Int64 workID = wk.OID;

            //隶属部门.
            Dept beloneDept = new Dept(beloneDeptNo);

            #region 为开始工作创建待办
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            int i = gwf.RetrieveFromDBSources();
            if (i == 0)
            {
                gwf.FlowName = fl.Name;
                gwf.FK_Flow = flowNo;
                gwf.FK_FlowSort = fl.FK_FlowSort;

                gwf.FK_Dept = beloneDeptNo;
                gwf.DeptName = beloneDept.Name;

                gwf.FK_Node = fl.StartNodeID;

                gwf.NodeName = nd.Name;
                gwf.WFState = WFState.Runing;

                if (string.IsNullOrEmpty(title))
                    gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
                else
                    gwf.Title = title;

                gwf.Starter = emp.No;
                gwf.StarterName = emp.Name;

                gwf.RDT = DataType.CurrentDataTime;

                if (htWork != null && htWork.ContainsKey("PRI") == true)
                    gwf.PRI = int.Parse(htWork["PRI"].ToString());

                if (htWork != null && htWork.ContainsKey("SDTOfNode") == true)
                    /*节点应完成时间*/
                    gwf.SDTOfNode = htWork["SDTOfNode"].ToString();

                if (htWork != null && htWork.ContainsKey("SDTOfFlow") == true)
                    /*流程应完成时间*/
                    gwf.SDTOfNode = htWork["SDTOfFlow"].ToString();


                gwf.PWorkID = parentWorkID;
                gwf.PFlowNo = parentFlowNo;
                gwf.Insert();

                // 处理接受人集合.
                string[] emps = flowStarters.Split(',');
                foreach (string s in emps)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    Emp myemp = new Emp(s);

                    // 产生工作列表.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = wk.OID;
                    gwl.FK_Emp = myemp.No;
                    gwl.FK_EmpText = myemp.Name;

                    gwl.FK_Node = nd.NodeID;
                    gwl.FK_NodeText = nd.Name;
                    gwl.FID = 0;

                    gwl.FK_Flow = fl.No;
                    gwl.FK_Dept1 = myemp.FK_Dept;

                    gwl.SDT = DataType.CurrentDataTime;
                    gwl.DTOfWarning = DataType.CurrentDataTime;
                    gwl.RDT = DataType.CurrentDataTime;
                    gwl.IsEnable = true;

                    gwl.IsPass = false;
                    gwl.Sender = flowStarter;
                    gwl.PRI = gwf.PRI;
                    gwl.Insert();
                }
            }
            #endregion 为开始工作创建待办

            #region 给各个属性-赋值
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                    wk.SetValByKey(str, htWork[str]);
            }

            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            // 执行对报表的数据表 WFState 状态的更新,让它为runing的状态.

            Paras ps = new Paras();
            string dbstr = SystemConfig.AppCenterDBVarStr;
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,Title=" + dbstr + "Title,FK_Dept="+dbstr+"FK_Dept,PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID WHERE OID=" + dbstr + "OID";
            ps.Add("WFState", (int)WFState.Runing);
            ps.Add("Title", gwf.Title);
            ps.Add("FK_Dept", gwf.FK_Dept);
            ps.Add("PFlowNo", gwf.PFlowNo);
            ps.Add("PWorkID", gwf.PWorkID);
            ps.Add("OID", wk.OID);
            DBAccess.RunSQL(ps);


            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                    paras += "@" + key + "=" + htWork[key].ToString();

                if (string.IsNullOrEmpty(paras) == false)
                {
                      ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist set Paras=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.

            ////写入日志.
            //WorkNode wn = new WorkNode(wk, nd);
            //wn.AddToTrack(ActionType.CallSubFlow, flowStarter, emp.Name, nd.NodeID, nd.Name, WebUser.No + "," + WebUser.Name
            //    + "工作发起.");
            return wk.OID;
        }
        /// <summary>
        /// 发起工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">表单明细表参数，可以为null。</param>
        /// <param name="flowStarter">流程发起人，如果为null就是当前人员。</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateStartNodeWork(string flowNo, Hashtable ht, DataSet workDtls,
            string flowStarter, string title)
        {
            return Node_CreateStartNodeWork(flowNo, ht, workDtls, flowStarter, title, 0, null);
        }
        /// <summary>
        /// 创建开始节点工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="htWork">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="flowStarter">流程的发起人，如果为null就是当前人员。</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <param name="parentWorkID">父流程的WorkID,如果没有父流程就传入为0.</param>
        /// <param name="parentFlowNo">父流程的流程编号,如果没有父流程就传入为null.</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateStartNodeWork(string flowNo, Hashtable htWork, DataSet workDtls,
            string flowStarter, string title, Int64 parentWorkID, string parentFlowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowCodeToFlowNo(flowNo);

            //转化成编号
            parentFlowNo = TurnFlowCodeToFlowNo(parentFlowNo);

            if (string.IsNullOrEmpty(flowStarter))
                flowStarter = WebUser.No;

            Flow fl = new Flow(flowNo);

            #region 处理流程标题.
            if (string.IsNullOrEmpty(title) == false && fl.TitleRole != "@OutPara")
            {
                /*如果标题不为空*/
                fl.TitleRole = "@OutPara"; //特殊标记不为空.
                fl.Update();
            }
            if (string.IsNullOrEmpty(title) == true && fl.TitleRole == "@OutPara")
            {
                /*如果标题为空 */
                fl.TitleRole = ""; //特殊标记不为空.
                fl.Update();
            }
            #endregion 处理流程标题.

            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp emp = new Emp(flowStarter);
            Work wk = fl.NewWork(flowStarter);
            Int64 workID = wk.OID;


            #region 为开始工作创建待办
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            int i = gwf.RetrieveFromDBSources();
            if (i == 0)
            {
                gwf.FlowName = fl.Name;
                gwf.FK_Flow = flowNo;
                gwf.FK_FlowSort = fl.FK_FlowSort;

                gwf.FK_Dept = emp.FK_Dept;
                gwf.DeptName = emp.FK_DeptText;
                gwf.FK_Node = fl.StartNodeID;

                gwf.NodeName = nd.Name;
                gwf.WFState = WFState.Runing;

                if (string.IsNullOrEmpty(title))
                    gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
                else
                    gwf.Title = title;

                gwf.Starter = emp.No;
                gwf.StarterName = emp.Name;
                gwf.RDT = DataType.CurrentDataTime;

                if (htWork != null && htWork.ContainsKey("PRI") == true)
                    gwf.PRI = int.Parse(htWork["PRI"].ToString());

                if (htWork != null && htWork.ContainsKey("SDTOfNode") == true)
                    /*节点应完成时间*/
                    gwf.SDTOfNode = htWork["SDTOfNode"].ToString();

                if (htWork != null && htWork.ContainsKey("SDTOfFlow") == true)
                    /*流程应完成时间*/
                    gwf.SDTOfNode = htWork["SDTOfFlow"].ToString();

                gwf.PWorkID = parentWorkID;
                gwf.PFlowNo = parentFlowNo;

                gwf.Insert();

                // 产生工作列表.
                GenerWorkerList gwl = new GenerWorkerList();
                gwl.WorkID = wk.OID;
                gwl.FK_Emp = emp.No;
                gwl.FK_EmpText = emp.Name;

                gwl.FK_Node = nd.NodeID;
                gwl.FK_NodeText = nd.Name;
                gwl.FID = 0;

                gwl.FK_Flow = fl.No;
                gwl.FK_Dept1 = emp.FK_Dept;

                gwl.SDT = DataType.CurrentDataTime;
                gwl.DTOfWarning = DataType.CurrentDataTime;
                gwl.RDT = DataType.CurrentDataTime;
                gwl.IsEnable = true;

                gwl.IsPass = false;
                gwl.Sender = WebUser.No;
                gwl.PRI = gwf.PRI;
                gwl.Insert();
            }
            #endregion 为开始工作创建待办

            #region 给各个属性-赋值
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                    wk.SetValByKey(str, htWork[str]);
            }
            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            // 执行对报表的数据表WFState状态的更新,让它为runing的状态. 
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,Title=" + dbstr + "Title,FK_Dept=" + dbstr + "FK_Dept,PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID WHERE OID=" + dbstr + "OID";
            ps.Add("WFState", (int)WFState.Runing);
            ps.Add("Title", gwf.Title);
            ps.Add("FK_Dept", gwf.FK_Dept);

            ps.Add("PFlowNo", gwf.PFlowNo);
            ps.Add("PWorkID", gwf.PWorkID);

            ps.Add("OID", wk.OID);
            DBAccess.RunSQL(ps);

            ////写入日志.
            //WorkNode wn = new WorkNode(wk, nd);
            //wn.AddToTrack(ActionType.CallSubFlow, flowStarter, emp.Name, nd.NodeID, nd.Name, "来自" + WebUser.No + "," + WebUser.Name
            //    + "工作发起.");

            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                    paras += "@" + key + "=" + htWork[key].ToString();

                if (string.IsNullOrEmpty(paras) == false)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist set Paras=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.


            return wk.OID;
        }
        /// <summary>
        /// 执行工作发送
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回发送结果</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, null, null, 0, null);
        }
        /// <summary>
        /// 执行工作发送
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="nextWorkers">下一步工作接受人ID，多个人用逗号分开。</param>
        /// <returns>返回发送结果</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID,string nextWorkers)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, null, null, 0, nextWorkers);
        }
        /// <summary>
        /// 执行工作发送
        /// </summary>
        /// <param name="fk_flow">工作编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="ht">节点表单数据</param>
        /// <returns>返回发送结果</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable ht)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, ht, null, 0, null);
        }
        /// <summary>
        /// 执行工作发送
        /// </summary>
        /// <param name="fk_flow">工作编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="ht">节点表单数据</param>
        /// <param name="dsDtl">节点表单从表数据</param>
        /// <returns>返回发送结果</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable ht, DataSet dsDtl)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, ht, dsDtl, 0, null);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="nodeID">节点编号</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, int toNodeID, string toEmps)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, null, null, toNodeID, toEmps);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">节点表单数据(Hashtable中的key与节点表单的字段名相同,value 就是字段值)</param>
        /// <returns>执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable htWork, int toNodeID, string nextWorkers)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, htWork, null, toNodeID, nextWorkers);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">节点表单数据(Hashtable中的key与节点表单的字段名相同,value 就是字段值)</param>
        /// <param name="workDtls">节点表单明从表数据(dataset可以包含多个table，每个table的名称与从表名称相同，列名与从表的字段相同, OID,RefPK列需要为空或者null )</param>
        /// <returns>执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable htWork, DataSet workDtls, int toNodeID, string nextWorkers)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            Node nd = new Node(Dev2Interface.Node_GetCurrentNodeID(fk_flow, workID));
            Work sw = nd.HisWork;
            if (workID != 0)
            {
                sw.OID = workID;
                sw.RetrieveFromDBSources();
            }
            sw.ResetDefaultVal();

            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                {
                    if (sw.Row.ContainsKey(str))
                        sw.SetValByKey(str, htWork[str]);
                    else
                        sw.Row.Add(str, htWork[str]);
                }
            }

            sw.Rec = WebUser.No;
            sw.RecText = WebUser.Name;
            sw.SetValByKey(StartWorkAttr.FK_Dept, WebUser.FK_Dept);

            sw.BeforeSave();
            sw.Save();

            // 增加上参数.
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                {
                    if (sw.Row.ContainsKey(str) == false)
                        sw.Row.Add(str, htWork[str]);
                }
            }

            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in sw.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, workID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = workID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = workID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }

            SendReturnObjs objs;
            //执行流程发送.
            WorkNode wn = new WorkNode(sw, nd);
            if (toNodeID == 0 || toNodeID == null)
                objs = wn.NodeSend(null, nextWorkers);
            else
                objs = wn.NodeSend(new Node(toNodeID), nextWorkers);

            #region 更新发送参数.
            if (htWork != null )
            {
                string dbstr = SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();

                string paras = "";
                foreach (string key in htWork.Keys)
                {
                    paras += "@" + key + "=" + htWork[key].ToString();
                    switch (key)
                    {
                        case WorkSysFieldAttr.SysSDTOfFlow:
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfFlow=" + dbstr + "SDTOfFlow WHERE WorkID=" + dbstr + "WorkID";
                            ps.Add(GenerWorkFlowAttr.SDTOfFlow, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            DBAccess.RunSQL(ps);

                            break;
                        case WorkSysFieldAttr.SysSDTOfNode:
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfNode=" + dbstr + "SDTOfNode WHERE WorkID=" + dbstr + "WorkID";
                            ps.Add(GenerWorkFlowAttr.SDTOfNode, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            DBAccess.RunSQL(ps);

                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkerlist SET SDT=" + dbstr + "SDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                            ps.Add(GenerWorkerListAttr.SDT, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            ps.Add(GenerWorkerListAttr.FK_Node, objs.VarToNodeID);
                            DBAccess.RunSQL(ps);
                            break;
                        default:
                            break;
                    }
                }

                if (string.IsNullOrEmpty(paras) == false)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET Paras=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.

            return objs;
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">抄送人员编号</param>
        /// <param name="empName">抄送人员人员名称</param>
        /// <returns>执行信息</returns>
        public static void Node_CC(string flowNo,Int64 workid,string empNo, string empName)
        {
            string title = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + workid, "无标题");
            Node_CC(flowNo,workid, empNo, empName, title, "信息抄送...");
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmpNo">抄送人员编号</param>
        /// <param name="toEmpName">抄送人员人员名称</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <returns>执行信息</returns>
        public static string Node_CC(string flowNo, Int64 workID, string toEmpNo, string toEmpName, string msgTitle, string msgDoc)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                Flow fl = new Flow(flowNo);
                string sql = "SELECT Title FROM " + fl.PTable + " WHERE OID=" + workID;
                string title = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);
                if (title == null)
                    throw new Exception("系统出现异常，请联系管理员，没有找到该workid"+workID+"的流程数据。");

                gwf.Title = title;
                gwf.FK_Flow = flowNo;
                gwf.FlowName = fl.Name;
                gwf.FK_Node = fl.StartNodeID;
                Node nd = new Node(fl.StartNodeID);
                gwf.NodeName = nd.Name;
            }

            CCList list = new CCList();
            list.MyPK = DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
            list.FK_Flow = gwf.FK_Flow;
            list.FlowName = gwf.FlowName;
            list.FK_Node = gwf.FK_Node;
            list.NodeName = gwf.NodeName;
            list.Title = msgTitle;
            list.Doc = msgDoc;
            list.CCTo = toEmpNo;
            list.RDT = DataType.CurrentDataTime;
            list.Rec = WebUser.No;

            list.WorkID = gwf.WorkID;
            list.FID = gwf.FID;

            list.PFlowNo = gwf.PFlowNo;
            list.PWorkID = gwf.PWorkID;

            try
            {
                list.Insert();
            }
            catch
            {
                list.CheckPhysicsTable();
                list.Update();
            }

            //记录日志.
            Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, workID, gwf.FID, gwf.FK_Node, gwf.NodeName,
                WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, toEmpNo, toEmpName, msgTitle);

            //发送邮件.
          BP.WF.Dev2Interface.Port_SendMail(toEmpNo, WebUser.Name + "把工作:" + gwf.Title, "抄送信息:" + msgTitle,"CC"+gwf.FK_Node+"_"+workID,
                gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

            return "已经成功的把工作抄送给:"+toEmpNo+","+toEmpName;
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmpNo">抄送给人员编号</param>
        /// <param name="toEmpName">抄送给人员名称</param>
        /// <param name="msgTitle">消息标题</param>
        /// <param name="msgDoc">消息内容</param>
        /// <param name="pFlowNo">父流程编号(可以为null)</param>
        /// <param name="pWorkID">父流程WorkID(可以为0)</param>
        /// <returns></returns>
        public static string Node_CC(string fk_flow,int fk_node,Int64 workID, string toEmpNo, string toEmpName, string msgTitle, string msgDoc,string pFlowNo,Int64 pWorkID)
        {
            Flow fl = new Flow(fk_flow);
            Node nd = new Node(fk_node);

            CCList list = new CCList();
            list.MyPK = DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
            list.FK_Flow = fk_flow;
            list.FlowName = fl.Name;
            list.FK_Node = fk_node ;
            list.NodeName = nd.Name;
            list.Title = msgTitle;
            list.Doc = msgDoc;
            list.CCTo = toEmpNo;
            list.RDT = DataType.CurrentDataTime;
            list.Rec = WebUser.No;
            list.WorkID = workID;
            list.FID = 0;

            list.PFlowNo = pFlowNo;
            list.PWorkID = pWorkID;

            try
            {
                list.Insert();
            }
            catch
            {
                list.CheckPhysicsTable();
                list.Update();
            }

            //记录日志.
            Glo.AddToTrack(ActionType.Shift, fk_flow, workID, list.FID, list.FK_Node, list.NodeName,
                WebUser.No, WebUser.Name, list.FK_Node, list.NodeName, toEmpNo, toEmpName, msgTitle);

            //发送邮件.
            Port_SendMail(toEmpNo, WebUser.Name + "把工作:" + list.Title, "移交信息:" + msgTitle,"SF"+list.FK_Node+"_"+list.WorkID,
                list.FK_Flow, list.FK_Node, list.WorkID, list.FID);

            return "已经成功的把工作抄送给:" + toEmpNo + "," + toEmpName;

        }
        /// <summary>
        /// 设置当前工作状态为草稿,如果启用了草稿,请在开始节点的表单保存按钮下增加上它.
        /// 必须是在开始节点时调用.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        public static void Node_SetDraft(string fk_flow, Int64 workID)
        {
            //转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 1)
            {
                if (gwf.FK_Node != int.Parse(fk_flow + "01"))
                    throw new Exception("@设置草稿错误，只有在开始节点时才能设置草稿，现在的节点是:" + gwf.Title);

                //设置成草稿.
                gwf.Update(GenerWorkFlowAttr.WFState, (int)WFState.Draft);
            }

            //设置流程数据表.
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Flow fl = new Flow(fk_flow);
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID ";
            ps.Add(GERptAttr.WFState, (int)WFState.Draft);
            ps.Add(GERptAttr.OID, workID);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存的信息</returns>
        public static string Node_SaveWork(string fk_flow, int fk_node, Int64 workID)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SaveWork(fk_flow,fk_node, workID, new Hashtable(), null);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">workid</param>
        /// <param name="wk">节点表单参数</param>
        /// <returns></returns>
        public static string Node_SaveWork(string fk_flow,int fk_node, Int64 workID, Hashtable wk)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return Node_SaveWork(fk_flow, fk_node,workID, wk, null);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">工作数据</param>
        /// <returns>返回执行信息</returns>
        public static string Node_SaveWork(string fk_flow, int fk_node, Int64 workID, Hashtable htWork, DataSet dsDtls)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            try
            {
                Node nd = new Node(fk_node);
                //if (nd.IsStartNode == false)
                //    return "@非开始节点不允许保存";

                Work sw = nd.HisWork;
                sw.OID = workID;
                sw.Retrieve();
                sw.ResetDefaultVal();
                if (htWork != null)
                {
                    foreach (string str in htWork.Keys)
                        sw.SetValByKey(str, htWork[str]);
                }

                //增加其它的字段.
                sw.SetValByKey(StartWorkAttr.FK_Dept, WebUser.FK_Dept);
                sw.BeforeSave();
                sw.Save();

                #region 更新发送参数.
                if (htWork != null)
                {
                    string paras = "";
                    foreach (string key in htWork.Keys)
                        paras += "@" + key + "=" + htWork[key].ToString();

                    if (string.IsNullOrEmpty(paras) == false)
                    {
                        string dbstr = SystemConfig.AppCenterDBVarStr;
                        Paras ps = new Paras();
                        ps.SQL = "UPDATE WF_GenerWorkerlist set Paras=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                        ps.Add(GenerWorkerListAttr.Paras, paras);
                        ps.Add(GenerWorkerListAttr.WorkID, workID);
                        ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                        DBAccess.RunSQL(ps);
                    }
                }
                #endregion 更新发送参数.

                if (dsDtls != null)
                {
                    //保存从表
                    foreach (DataTable dt in dsDtls.Tables)
                    {
                        foreach (MapDtl dtl in sw.HisMapDtls)
                        {
                            if (dt.TableName != dtl.No)
                                continue;
                            //获取dtls
                            GEDtls daDtls = new GEDtls(dtl.No);
                            daDtls.Delete(GEDtlAttr.RefPK, workID); // 清除现有的数据.

                            GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                            daDtl.RefPK = workID.ToString();

                            // 为从表复制数据.
                            foreach (DataRow dr in dt.Rows)
                            {
                                daDtl.ResetDefaultVal();
                                daDtl.RefPK = workID.ToString();

                                //明细列.
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    //设置属性.
                                    daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                                }
                                daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                            }
                        }
                    }
                }

                if (nd.SaveModel == SaveModel.NDAndRpt)
                {
                    /* 如果保存模式是节点表与Node与Rpt表. */
                    WorkNode wn = new WorkNode(sw, nd);
                    GERpt rptGe = nd.HisFlow.HisFlowData;
                    rptGe.SetValByKey("OID", workID);
                    wn.rptGe = rptGe;
                    if (rptGe.RetrieveFromDBSources() == 0)
                    {
                        rptGe.SetValByKey("OID", workID);
                        wn.DoCopyRptWork(sw);

                        rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "," + WebUser.Name);
                        rptGe.SetValByKey(GERptAttr.FlowStarter, WebUser.No);
                        rptGe.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDataTime);
                        rptGe.SetValByKey(GERptAttr.WFState, 0);
                        rptGe.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                        rptGe.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                        rptGe.Insert();
                    }
                    else
                    {
                        wn.DoCopyRptWork(sw);
                        rptGe.Update();
                    }
                }
                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "保存失败:" + ex.Message;
            }
        }
        /// <summary>
        /// 保存流程表单
        /// </summary>
        /// <param name="fk_mapdata">流程表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htData">流程表单数据Key Value 格式存放.</param>
        /// <returns>返回执行信息</returns>
        public static void Node_SaveFlowSheet(string fk_mapdata, Int64 workID, Hashtable htData)
        {
            Node_SaveFlowSheet(fk_mapdata, workID, htData, null);
        }
        /// <summary>
        /// 保存流程表单
        /// </summary>
        /// <param name="fk_mapdata">流程表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htData">流程表单数据Key Value 格式存放.</param>
        /// <param name="workDtls">从表数据</param>
        /// <returns>返回执行信息</returns>
        public static void Node_SaveFlowSheet(string fk_mapdata, Int64 workID, Hashtable htData, DataSet workDtls)
        {
            MapData md = new MapData(fk_mapdata);
            GEEntity en = md.HisGEEn;
            en.SetValByKey("OID", workID);
            int i = en.RetrieveFromDBSources();

            foreach (string key in htData.Keys)
                en.SetValByKey(key, htData[key].ToString());

            en.SetValByKey("OID", workID);

            FrmEvents fes = md.FrmEvents;
            fes.DoEventNode(FrmEventList.SaveBefore, en);
            if (i == 0)
                en.Insert();
            else
                en.Update();

            if (workDtls != null)
            {
                MapDtls dtls = new MapDtls(fk_mapdata);
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, workID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = workID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = workID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }

            fes.DoEventNode(FrmEventList.SaveAfter, en);
        }

        /// <summary>
        /// 增加下一步骤的接受人(用于当前步骤向下一步骤发送时增加接受人)
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="formNodeID">节点ID</param>
        /// <param name="emps">如果多个就用逗号分开</param>
        public static void Node_AddNextStepAccepters(Int64 workID, int formNodeID, string emps)
        {
            SelectAccper sa = new SelectAccper();
            sa.Delete(SelectAccperAttr.FK_Node, formNodeID, SelectAccperAttr.WorkID, workID);
            emps = emps.Replace(" ", "");
            emps = emps.Replace(";", ",");
            emps = emps.Replace("@", ",");
            string[] strs = emps.Split(',');
            foreach (string emp in strs)
            {
                if (string.IsNullOrEmpty(emp))
                    continue;
                sa.MyPK = formNodeID + "_" + workID + "_" + emp;
                sa.FK_Emp = emp;
                sa.FK_Node = formNodeID;
                sa.WorkID = workID;
                sa.Insert();
            }
        }
        /// <summary>
        /// 节点工作挂起
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="way">挂起方式</param>
        /// <param name="reldata">解除挂起日期(可以为空)</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns>返回执行信息</returns>
        public static string Node_HungUpWork(string fk_flow, Int64 workid, int wayInt, string reldata, string hungNote)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            HungUpWay way = (HungUpWay)wayInt;
            BP.WF.WorkFlow wf = new WorkFlow(fk_flow, workid);
            return wf.DoHungUp(way, reldata, hungNote);
        }
        /// <summary>
        /// 节点工作取消挂起
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="msg">取消挂起原因</param>
        /// <returns>执行信息</returns>
        public static void Node_UnHungUpWork(string fk_flow, Int64 workid, string msg)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            BP.WF.WorkFlow wf = new WorkFlow(fk_flow, workid);
            wf.DoUnHungUp();
        }
        /// <summary>
        /// 获取该节点上的挂起时间
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>返回时间串，如果没有挂起的动作就抛出异常.</returns>
        public static TimeSpan Node_GetHungUpTimeSpan(string flowNo, int nodeID, Int64 workid)
        {
            string dbstr=BP.SystemConfig.AppCenterDBVarStr;

            string instr = (int)ActionType.HungUp + "," + (int)ActionType.UnHungUp;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + dbstr + "WorkID AND " + TrackAttr.ActionType + " in (" + instr + ")  and  NDFrom=" + dbstr + "NDFrom ";
            ps.Add(TrackAttr.WorkID, workid);
            ps.Add(TrackAttr.NDFrom, nodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            foreach (DataRow item in dt.Rows)
            {
                ActionType at = (ActionType)item[TrackAttr.ActionType];

                //挂起时间.
                if (at == ActionType.HungUp)
                    dtStart = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());

                //解除挂起时间.
                if (at == ActionType.UnHungUp)
                    dtEnd = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());
            }

            TimeSpan ts = dtEnd - dtStart;
            return ts;
        }
        /// <summary>
        /// 工作移交
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="toEmp">移交到人员(只给移交给一个人)</param>
        /// <param name="msg">移交消息</param>
        public static string Node_Shift(Int64 workID,  string toEmp, string msg)
        {
            // 删除当前非配的工作。
            // 已经非配或者自动分配的任务。
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            int nodeId = gwf.FK_Node;

            //设置所有的工作人员为不可处理.
            string dbStr=SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=0  WHERE WorkID=" + dbStr + "WorkID AND FK_Node=" + dbStr + "FK_Node";
            ps.Add(GenerWorkerListAttr.WorkID, workID);
            ps.Add(GenerWorkerListAttr.FK_Node, nodeId);
            DBAccess.RunSQL(ps);


            //设置被移交人的FK_Emp 为当前处理人，（有可能被移交人不在工作列表里，就返回0.）
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=1  WHERE WorkID=" + dbStr + "WorkID AND FK_Node=" + dbStr + "FK_Node AND FK_Emp=" + dbStr + "FK_Emp";
            ps.Add(GenerWorkerListAttr.WorkID, workID);
            ps.Add(GenerWorkerListAttr.FK_Node, nodeId);
            ps.Add(GenerWorkerListAttr.FK_Emp, toEmp);
              int i = DBAccess.RunSQL(ps);
          
            Emp emp = new Emp(toEmp);
            GenerWorkerLists wls = null;
            GenerWorkerList wl = null;
            if (i == 0)
            {
                /*说明: 用其它的岗位上的人来处理的，就给他增加待办工作。*/
                wls = new GenerWorkerLists(workID, nodeId);
                wl = wls[0] as GenerWorkerList;
                wl.FK_Emp = toEmp.ToString();
                wl.FK_EmpText = emp.Name;
                wl.IsEnable = true;
                wl.Insert();

                // 清除工作者，为转发消息所用.
                wls.Clear();
                wls.AddEntity(wl);
            }

            BP.WF.Node nd = new BP.WF.Node(nodeId);
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            
            if (wk.Emps.Contains(","+toEmp)==false)
               wk.Emps = "," + toEmp + ".";

            wk.Rec = toEmp;
            wk.Update();

            ShiftWork sf = new ShiftWork();
            sf.CheckPhysicsTable();

            sf.WorkID = workID;
            sf.FK_Node = nodeId;
            sf.ToEmp = toEmp;
            sf.ToEmpName = emp.Name;
            sf.Note = msg;
            sf.FK_Emp = WebUser.No;
            sf.FK_EmpName = WebUser.Name;
            sf.Insert();

            //记录日志.
            Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg);

            //发送邮件.
            Port_SendMail(emp.No, WebUser.Name + "向您移交了工作" + gwf.Title, "移交信息:"+msg, "SF"+workID+"_"+sf.FK_Node,gwf.FK_Flow,gwf.FK_Node,gwf.WorkID,gwf.FID);

            string info = "@工作移交成功。@您已经成功的把工作移交给：" + emp.No + " , " + emp.Name;
            info += "@<a href='MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=UnShift&FK_Flow=" + nd.FK_Flow + "&WorkID=" + workID + "' ><img src='/WF/Img/UnDo.gif' border=0 />撤消工作移交</a>.";
            return info;
        }
        /// <summary>
        /// 执行工作退回(退回指定的点)
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currentNodeID">当前节点ID</param>
        /// <param name="returnToNodeID">退回到的工作ID</param>
        /// <param name="msg">退回原因</param>
        /// <param name="isBackToThisNode">退回后是否要原路返回？</param>
        /// <returns>执行结果，此结果要提示给用户。</returns>
        public static string Node_ReturnWork(string fk_flow, Int64 workID, Int64 fid, int currentNodeID, int returnToNodeID, string msg, bool isBackToThisNode)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            WorkReturn wr = new WorkReturn(fk_flow, workID, fid, currentNodeID, returnToNodeID, isBackToThisNode, msg);
            return wr.DoIt();
        }
        /// <summary>
        /// 获取当前工作的NodeID
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>指定工作的NodeID.</returns>
        public static int Node_GetCurrentNodeID(string fk_flow, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            int nodeID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + workid + " AND FK_Flow='" + fk_flow + "'", 0);
            if (nodeID == 0)
                return int.Parse(fk_flow + "01");
            return nodeID;
        }
        
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fid">流程ID</param>
        /// <param name="workid">工作ID</param>
        public static void Node_FHL_KillSubFlow(string fk_flow, Int64 fid, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            WorkFlow wkf = new WorkFlow(fk_flow, workid);
            wkf.DoDeleteWorkFlowByReal(true);
        }
        /// <summary>
        /// 合流点驳回子线程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fid">流程ID</param>
        /// <param name="workid">子线程ID</param>
        /// <param name="msg">驳回消息</param>
        public static string Node_FHL_DoReject(string fk_flow, int NodeSheetfReject, Int64 fid, Int64 workid, string msg)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            WorkFlow wkf = new WorkFlow(fk_flow, workid);
            return wkf.DoReject(fid, NodeSheetfReject, msg);
        }
       
        /// <summary>
        /// 跳转审核取回
        /// </summary>
        /// <param name="fromNodeID">从节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="tackToNodeID">取回到的节点ID</param>
        /// <returns></returns>
        public static string Node_Tackback(int fromNodeID, Int64 workid, int tackToNodeID)
        {

            /*
             * 1,首先检查是否有此权限.
             * 2, 执行工作跳转.
             * 3, 执行写入日志.
             */
            Node nd = new Node(tackToNodeID);
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByPreviousNodeFormEmpsField:
                    break;
            }

            WorkNode wn = new WorkNode(workid, fromNodeID);
            string msg = wn.NodeSend(new Node(tackToNodeID), BP.Web.WebUser.No).ToMsgOfHtml();
            wn.AddToTrack(ActionType.Tackback, WebUser.No, WebUser.Name, tackToNodeID, nd.Name, "执行跳转审核的取回.");
            return msg;
        }
        /// <summary>
        /// 设置是此工作为读取状态
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">WorkID</param>
        public static void Node_SetWorkRead(int nodeID, Int64 workid)
        {
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsRead=1 WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", WebUser.No);
            if (DBAccess.RunSQL(ps) == 0)
                throw new Exception("@设置的工作不存在，或者当前的登陆人员已经改变。");

            // 判断当前节点的已读回执.
            Node nd = new Node(nodeID);
            if (nd.ReadReceipts == ReadReceipts.None)
                return;

            bool isSend = false;
            if (nd.ReadReceipts == ReadReceipts.Auto)
                isSend=true;

            if (nd.ReadReceipts == ReadReceipts.BySysField)
            {
                /*获取上一个节点ID*/
                Nodes fromNodes = nd.FromNodes;
                int fromNodeID = 0;
                foreach (Node item in fromNodes)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Node FROM WF_GenerWorkerlist  WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                    ps.Add("WorkID", workid);
                    ps.Add("FK_Node", item.NodeID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        continue;
                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                    throw new Exception("@没有找到它的上一步工作。");

                try
                {
                    ps = new Paras();
                    ps.SQL = "SELECT " + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + " FROM ND" + fromNodeID + "    WHERE OID=" + dbstr + "OID";
                    ps.Add("OID", workid);
                    DataTable dt1 = DBAccess.RunSQLReturnTable(ps);
                    if (dt1.Rows[0][0].ToString() == "1")
                        isSend = true;
                }
                catch(Exception ex)
                {
                    throw new Exception("@流程设计错误:"+ex.Message +" 在当前节点上个您设置了安上一步的表单字段决定是否回执，但是上一个节点表单中没有约定的字段。");
                }
            }

            if (nd.ReadReceipts == ReadReceipts.BySDKPara)
            {
                /*如果是按开发者参数*/

                /*获取上一个节点ID*/
                Nodes fromNodes = nd.FromNodes;
                int fromNodeID = 0;
                foreach (Node item in fromNodes)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Node FROM WF_GenerWorkerlist  WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                    ps.Add("WorkID", workid);
                    ps.Add("FK_Node", item.NodeID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        continue;

                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                    throw new Exception("@没有找到它的上一步工作。");

                string paras = BP.WF.Dev2Interface.GetFlowParas(fromNodeID, workid);
                if (string.IsNullOrEmpty(paras) || paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=") == false)
                    throw new Exception("@流程设计错误:在当前节点上个您设置了按开发者参数决定是否回执，但是没有找到该参数。");

                // 开发者参数.
                if (paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=1") == true)
                    isSend = true;
            }


            if (isSend==true)
            {
                /*如果是自动的已读回执，就让它发送给当前节点的上一个发送人。*/

                // 获取流程标题.
                ps = new Paras();
                ps.SQL = "SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID ";
                ps.Add("WorkID", workid);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                string title = dt.Rows[0][0].ToString();

                // 获取流程的发送人.
                ps = new Paras();
                ps.SQL = "SELECT " + GenerWorkerListAttr.Sender + " FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                ps.Add("WorkID", workid);
                ps.Add("FK_Node", nodeID);
                dt = DBAccess.RunSQLReturnTable(ps);
                string sender = dt.Rows[0][0].ToString();

                //发送已读回执。
                BP.WF.Dev2Interface.Port_SendMail(sender, "已读回执:" + title,
                    "您发送的工作已经被" + WebUser.Name + "在" + DataType.CurrentDataTimeCNOfShort + " 打开.", "RP"+workid+"_"+nodeID,nd.FK_Flow,nd.NodeID,workid,0);
            }
        }
        /// <summary>
        /// 设置工作未读取
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        public static void Node_SetWorkUnRead(int nodeID, Int64 workid)
        {
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsRead=0 WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", WebUser.No);
            if (DBAccess.RunSQL(ps) == 0)
                throw new Exception("@设置的工作不存在，或者当前的登陆人员已经改变。");
        }
        #endregion 工作有关接口

        #region 流程属性与节点属性变更接口.
        /// <summary>
        /// 更改流程属性.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="isEnableFlow">启用与禁用 true启用  false禁用</param>
        /// <returns>执行结果</returns>
        public static string ChangeAttr_Flow(string fk_flow, bool isEnableFlow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            return ChangeAttr_Flow(fk_flow, FlowAttr.IsOK, isEnableFlow, null, null);
        }
        /// <summary>
        /// 更改流程属性
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="attr1">字段1</param>
        /// <param name="v1">值1</param>
        /// <param name="attr2">字段2(可为null)</param>
        /// <param name="v2">值2(可为null)</param>
        /// <returns>执行结果</returns>
        public static string ChangeAttr_Flow(string fk_flow, string attr1, object v1, string attr2, object v2)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            Flow fl = new Flow(fk_flow);
            if (attr1 != null)
                fl.SetValByKey(attr1, v1);
            if (attr2 != null)
                fl.SetValByKey(attr2, v2);
            fl.Update();
            return "修改成功";
        }
        /// <summary>
        /// 获取数据接口 
        /// 标记不同获得的数据也不同.
        /// Flow=流程数据
        /// </summary>
        /// <param name="flag">标记</param>
        /// <returns></returns>
        public static DataSet DS_GetDataByFlag(string flag)
        {
            DataSet ds = new DataSet();
            switch (flag)
            {
                case "Flow":
                    Flows fls = new Flows();
                    fls.RetrieveAll();
                    DataTable dt = fls.ToDataTableField();
                    dt.TableName = "WF_Flow";
                    dt.Columns.Add(new DataColumn("IsEnableText", typeof(string)));
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[FlowAttr.IsOK].ToString() == "1")
                            dr["IsEnableText"] = "启用";
                        else
                            dr["IsEnableText"] = "禁用";
                    }
                    ds.Tables.Add(dt);
                    return ds;
                default:
                    break;
            }
            throw new Exception("标记错误:" + flag);
        }
        #endregion 流程属性与节点属性变更接口.

        #region UI 接口
        /// <summary>
        /// 获取按钮状态
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">流程ID</param>
        /// <returns>返回按钮状态</returns>
        public static ButtonState UI_GetButtonState(string fk_flow, int fk_node, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            ButtonState bs = new ButtonState(fk_flow, fk_node, workid);
            return bs;
        }
        /// <summary>
        /// 打开退回窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Return(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/ReturnWorkSmall.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开转发窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Forward(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/ForwardSmall.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开抄送窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_CC(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/ForwardSmall.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开挂起窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_HungUp(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/WorkOpt/HungUp.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开催办窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Hurry(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/Hurry.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开跳转窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_JumpWay(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/JumpWaySmallSingle.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开流程轨迹窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="nodeID">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_FlowChartTruck(string fk_flow, int nodeID,Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            PubClass.WinOpen("/WF/Chart.aspx?FK_Flow=" + fk_flow + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 下一步工作的接受人
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Accepter(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);

            PubClass.WinOpen("/WF/Accepter.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开流程图窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_FlowChart(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            PubClass.WinOpen("/WF/Chart.aspx?FK_Flow=" + fk_flow,
                500, 400);
        }
        /// <summary>
        /// 打开OneWork
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_OneWork(string fk_flow, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowCodeToFlowNo(fk_flow);
            PubClass.WinOpen("/WF/WorkOpt/OneWork/Track.aspx?FK_Flow=" + fk_flow + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        #endregion UI 接口
    }
}
