using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
namespace BP.WF
{
    /// <summary>
    /// 处理工作退回
    /// </summary>
    public class WorkReturn
    {
        #region 变量
        /// <summary>
        /// 从节点
        /// </summary>
        private Node HisNode = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Node ReurnToNode = null;
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 WorkID = 0;
        /// <summary>
        /// 流程ID
        /// </summary>
        private Int64 FID = 0;
        /// <summary>
        /// 是否按原路返回?
        /// </summary>
        private bool IsBackTrack = false;
        /// <summary>
        /// 退回原因
        /// </summary>
        private string Msg = "退回原因未填写.";
        /// <summary>
        /// 当前节点
        /// </summary>
        private Work HisWork = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Work ReurnToWork = null;
        private string dbStr = BP.SystemConfig.AppCenterDBVarStr;
        private Paras ps;
        #endregion

        /// <summary>
        /// 工作退回
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">WorkID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currNodeID">从节点</param>
        /// <param name="reurnToNodeID">退回到节点</param>
        /// <param name="isBackTrack">是否需要原路返回？</param>
        /// <param name="returnInfo">退回原因</param>
        public WorkReturn(string fk_flow, Int64 workID, Int64 fid, int currNodeID, int reurnToNodeID, bool isBackTrack, string returnInfo)
        {
            this.HisNode = new Node(currNodeID);
            this.ReurnToNode = new Node(reurnToNodeID);
            this.WorkID = workID;
            this.FID = fid;
            this.IsBackTrack = isBackTrack;
            this.Msg = returnInfo;

            //当前工作.
            this.HisWork = this.HisNode.HisWork;
            if (fid == 0)
            {
                this.HisWork.OID = workID;
                this.HisWork.Retrieve();
            }
            else
            {
                this.HisWork.OID = fid;
                this.HisWork.Retrieve();
            }

            //退回工作
            this.ReurnToWork = this.ReurnToNode.HisWork;
            this.ReurnToWork.OID = workID;
            if (this.ReurnToWork.RetrieveFromDBSources() == 0)
            {
                this.ReurnToWork.OID = fid;
                this.ReurnToWork.RetrieveFromDBSources();
            }
        }
        /// <summary>
        /// 执行退回.
        /// </summary>
        /// <returns>返回退回信息</returns>
        public string  DoIt()
        {
            switch (this.HisNode.HisRunModel)
            {
                case RunModel.Ordinary: /* 1： 普通节点向下发送的*/
                    switch (ReurnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:   /*1-1 普通节to普通节点 */
                           return ExeReturn1_1(); //
                            break;
                        case RunModel.FL:  /* 1-2 普通节to分流点   */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.HL:  /*1-3 普通节to合流点   */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.FHL: /*1-4 普通节点to分合流点 */
                            return ExeReturn1_1();
                            break;
                        case RunModel.SubThread: /*1-5 普通节to子线程点 */
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                            break;
                    }
                    break;
                case RunModel.FL: /* 2: 分流节点向下发送的*/
                    switch (this.ReurnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:    /*2.1 分流点to普通节点 */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.FL:  /*2.2 分流点to分流点  */
                        case RunModel.HL:  /*2.3 分流点to合流点,分合流点   */
                        case RunModel.FHL:
                           return  ExeReturn1_1(); //
                            break;
                        case RunModel.SubThread: /* 2.4 分流点to子线程点   */
                            throw new Exception("@退回错误:非法的设计模式或退回模式.分流点to子线程点,请反馈给管理员.");
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReurnToNode.Name + ")");
                            break;
                    }
                    break;
                case RunModel.HL:  /* 3: 合流节点向下发送 */
                    switch (this.ReurnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*3.1 普通工作节点 */
                          return   ExeReturn1_1(); //
                            break;
                        case RunModel.FL: /*3.2 合流点向分流点退回 */
                          return   ExeReturn3_2(); //
                            break;
                        case RunModel.HL: /*3.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                            break;
                        case RunModel.SubThread:/*3.4 合流点向子线程退回 */
                            return ExeReturn3_4();
                            break;
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                    }
                    break;
                case RunModel.FHL:  /* 4: 分流节点向下发送的 */
                    switch (this.ReurnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*4.1 普通工作节点 */
                            return ExeReturn1_1();
                            break;
                        case RunModel.FL: /*4.2 分流点 */
                        case RunModel.HL: /*4.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                        case RunModel.SubThread:/*4.5 子线程*/
                            return ExeReturn3_4();
                            break;
                        default:
                            throw new Exception("@没有判断的节点类型(" + this.ReurnToNode.Name + ")");
                    }
                    break;
                case RunModel.SubThread:  /* 5: 子线程节点向下发送的 */
                    switch (this.ReurnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*5.1 普通工作节点 */
                            throw new Exception("@非法的退回模式,,请反馈给管理员.");
                            break;
                        case RunModel.FL: /*5.2 分流点 */
                            throw new Exception("@目前不支持此场景下的退回,,请反馈给管理员.");
                        case RunModel.HL: /*5.3 合流点 */
                            throw new Exception("@非法的退回模式,,请反馈给管理员.");
                            break;
                        case RunModel.FHL: /*5.4 分合流点 */
                            throw new Exception("@目前不支持此场景下的退回,请反馈给管理员.");
                            break;
                        case RunModel.SubThread: /*5.5 子线程*/
                            ExeReturn1_1();
                            break;
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReurnToNode.Name + ")");
                    }
                    break;
                default:
                    throw new Exception("@没有判断的类型:" + this.HisNode.HisRunModel);
            }

            throw new Exception("@系统出现未判断的异常.");
        }
        /// <summary>
        /// 合流点向子线程退回
        /// </summary>
        private string ExeReturn3_4()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.FK_Node = this.ReurnToNode.NodeID;
            gwf.WFState = WFState.Runing;
            gwf.Update();

            string info = "@工作已经成功的退回到（" + ReurnToNode.Name + "）退回给：";
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReurnToNode.NodeID);

            string toEmp = "";
            string toEmpName = "";
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPass = false;
                item.IsRead = false;
                item.Update();
                info += item.FK_Emp + "," + item.FK_EmpText;
                toEmp = item.FK_Emp;
                toEmpName = item.FK_EmpText;
            }

            //删除已经发向合流点的汇总数据.
            MapDtls dtls = new MapDtls("ND" + this.HisNode.NodeID);
            foreach (MapDtl dtl in dtls)
            {
                /*如果是合流数据*/
                if (dtl.IsHLDtl)
                    BP.DA.DBAccess.RunSQL("DELETE " + dtl.PTable + " WHERE OID=" + this.WorkID);
            }

            

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReurnToNode.NodeID;
            rw.ReturnNodeName = this.ReurnToNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = toEmp; //退回给。

            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.Note = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();

            // 加入track.
            this.AddToTrack(ActionType.Return, toEmp, toEmpName,
                this.ReurnToNode.NodeID, this.ReurnToNode.Name, Msg);

            // 返回退回信息.
            return info;
        }
        /// <summary>
        /// 合流点向分流点退回
        /// </summary>
        private string ExeReturn3_2()
        {
            //删除分流点与合流点之间的子线程数据。
            if (this.ReurnToNode.IsStartNode == false)
                throw new Exception("@没有处理的模式。");

            //删除子线程节点数据。
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);

            foreach (GenerWorkerList item in gwls)
            {
                /* 删除 子线程数据 */
                DBAccess.RunSQL("DELETE ND" + item.FK_Node + " WHERE OID=" + item.WorkID);
            }

            //删除流程控制数据。
            DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE FID=" + this.WorkID);
            DBAccess.RunSQL("DELETE WF_GenerWorkerList WHERE FID=" + this.WorkID);
            DBAccess.RunSQL("DELETE WF_GenerFH WHERE FID=" + this.WorkID);

            return ExeReturn1_1();
        }
        /// <summary>
        /// 普通节点到普通节点的退回
        /// </summary>
        /// <returns></returns>
        private string ExeReturn1_1()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReurnToNode.NodeID;
            this.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.ReturnBefore, this.HisWork, atPara);
            if (this.HisNode.FocusField != "")
            {
                // 把数据更新它。
                this.HisWork.Update(this.HisNode.FocusField, "");
            }

            // 改变当前待办工作节点。
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow  SET WFState=" + dbStr + "WFState,FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName WHERE  WorkID=" + dbStr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, (int)WFState.ReturnSta);
            ps.Add(GenerWorkFlowAttr.FK_Node, this.ReurnToNode.NodeID);
            ps.Add(GenerWorkFlowAttr.NodeName, this.ReurnToNode.Name);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
            ps.Add("FK_Node", this.ReurnToNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            DBAccess.RunSQL(ps);


            //更新流程报表数据.
            ps = new Paras();
            ps.SQL = "UPDATE "+this.HisNode.HisFlow.PTable+" SET  WFState="+dbStr+"WFState, FlowEnder="+dbStr+"FlowEnder, FlowEndNode="+dbStr+"FlowEndNode WHERE OID=" + dbStr + "OID";
            ps.Add("WFState", (int)WFState.ReturnSta);
            ps.Add("FlowEnder", WebUser.No);
            ps.Add("FlowEndNode", ReurnToNode.NodeID);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);


            //从工作人员列表里找到被退回人的接受人.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Retrieve(GenerWorkerListAttr.FK_Node, this.ReurnToNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID =this.WorkID;
            rw.ReturnToNode = this.ReurnToNode.NodeID;
            rw.ReturnNodeName = this.ReurnToNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = gwl.FK_Emp; //退回给。

            rw.Note = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.Insert();

            // 加入track.
            this.AddToTrack(ActionType.Return, gwl.FK_Emp, gwl.FK_EmpText,
                this.ReurnToNode.NodeID, this.ReurnToNode.Name, Msg);

            try
            {
                // 记录退回日志.
                ReorderLog( this.ReurnToNode,this.HisNode, rw);
            }
            catch (Exception ex)
            {
                Log.DebugWriteWarning(ex.Message);
            }

            // 以退回到的节点向前数据用递归删除它。
            if (IsBackTrack == false)
            {
                /*如果退回不需要原路返回，就删除中间点的数据。*/
#warning 没有考虑两种流程数据存储模式。
                //DeleteToNodesData(this.ReurnToNode.HisToNodes);
            }

            // 向他发送消息。
            if (Glo.IsEnableSysMessage == true)
            {
                //   WF.Port.WFEmp wfemp = new Port.WFEmp(wnOfBackTo.HisWork.Rec);
                string title = string.Format("工作退回：流程:{0}.工作:{1},退回人:{2},需您处理",
                    this.HisNode.FlowName, this.ReurnToNode.Name, WebUser.Name);

                BP.WF.Dev2Interface.Port_SendMail(gwl.FK_Emp, title, Msg, "RE" + this.HisNode.NodeID + this.WorkID, ReurnToNode.FK_Flow, ReurnToNode.NodeID, this.WorkID,this.FID);
            }

            //退回后事件
            this.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.ReturnAfter, this.HisWork, atPara);

            // 返回退回信息.
            return "工作已经被您退回到("+this.ReurnToNode.Name+"),退回给("+gwl.FK_Emp+","+gwl.FK_EmpText+").";
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg)
        {
            Track t = new Track();
            t.WorkID = this.WorkID;
            t.FK_Flow = this.HisNode.FK_Flow;
            t.FID = this.FID;
            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = this.HisNode.NodeID;
            t.NDFromT = this.HisNode.Name;

            t.EmpFrom = WebUser.No;
            t.EmpFromT = WebUser.Name;
            t.FK_Flow = this.HisNode.FK_Flow;

            t.NDTo = toNDid;
            t.NDToT = toNDName;

            t.EmpTo = toEmp;
            t.EmpToT = toEmpName;
            t.Msg = msg;
            t.Insert();
        }
        private string infoLog = "";
        private void ReorderLog(Node fromND, Node toND, ReturnWork rw)
        {
            string filePath = BP.SystemConfig.PathOfDataUser + "\\ReturnLog\\" + this.HisNode.FK_Flow + "\\";
            if (System.IO.Directory.Exists(filePath) == false)
                System.IO.Directory.CreateDirectory(filePath);

            string file = filePath + "\\" + rw.MyPK;
            infoLog = "\r\n退回人:" + WebUser.No + "," + WebUser.Name + " \r\n退回节点:" + fromND.Name + " \r\n退回到:" + toND.Name;
            infoLog += "\r\n退回时间:" + DataType.CurrentDataTime;
            infoLog += "\r\n原因:" + rw.Note;

            ReorderLog(fromND, toND);
            DataType.WriteFile(file + ".txt", infoLog);
            DataType.WriteFile(file + ".htm", infoLog.Replace("\r\n", "<br>"));

           // this.HisWork.Delete();
        }
        private void ReorderLog(Node fromND, Node toND)
        {
            /*开始遍历到达的节点集合*/
            foreach (Node nd in fromND.HisToNodes)
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                if (wk.RetrieveFromDBSources() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.Retrieve(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                if (nd.IsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndsStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndsStrs)
                    {
                        if (s == "" || s == null)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.FK_Node);
                        Work subWK = subNd.GetWork(wl.WorkID);

                        infoLog += "\r\n*****************************************************************************************";
                        infoLog += "\r\n节点ID:" + subNd.NodeID + "  工作名称:" + subWK.EnDesc;
                        infoLog += "\r\n处理人:" + subWK.Rec + " , " + wk.RecOfEmp.Name;
                        infoLog += "\r\n接收时间:" + subWK.RDT + " 处理时间:" + subWK.CDT;
                        infoLog += "\r\n ------------------------------------------------- ";

                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (attr.UIVisible == false)
                                continue;
                            infoLog += "\r\n " + attr.Desc + ":" + subWK.GetValStrByKey(attr.Key);
                        }

                        //递归调用。
                        ReorderLog(subNd, toND);
                    }
                }
                else
                {
                    infoLog += "\r\n*****************************************************************************************";
                    infoLog += "\r\n节点ID:" + wk.NodeID + "  工作名称:" + wk.EnDesc;
                    infoLog += "\r\n处理人:" + wk.Rec + " , " + wk.RecOfEmp.Name;
                    infoLog += "\r\n接收时间:" + wk.RDT + " 处理时间:" + wk.CDT;
                    infoLog += "\r\n ------------------------------------------------- ";

                    foreach (Attr attr in wk.EnMap.Attrs)
                    {
                        if (attr.UIVisible == false)
                            continue;
                        infoLog += "\r\n" + attr.Desc + " : " + wk.GetValStrByKey(attr.Key);
                    }
                }

                /* 如果到了当前的节点 */
                if (nd.NodeID == toND.NodeID)
                    break;

                //递归调用。
                ReorderLog(nd, toND);
            }
        }
        /// <summary>
        /// 递归删除两个节点之间的数据
        /// </summary>
        /// <param name="nds">到达的节点集合</param>
        public void DeleteToNodesData(Nodes nds)
        {
            /*开始遍历到达的节点集合*/
            foreach (Node nd in nds)
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                if (wk.Delete() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.Delete(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                #region 删除当前节点数据，删除附件信息。
                // 删除明细表信息。
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    ps = new Paras();
                    ps.SQL = "DELETE " + dtl.PTable + " WHERE RefPK=" + dbStr + "WorkID";
                    ps.Add("WorkID", this.WorkID.ToString());
                    BP.DA.DBAccess.RunSQL(ps);
                }

                // 删除表单附件信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                // 删除签名信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmEleDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                #endregion 删除当前节点数据。


                /*说明:已经删除该节点数据。*/
                DBAccess.RunSQL("DELETE WF_GenerWorkerList WHERE (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2 ) AND FK_Node=" + dbStr + "FK_Node",
                    "WorkID1", this.WorkID, "WorkID2", this.WorkID, "FK_Node", nd.NodeID);
                if (nd.IsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndsStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndsStrs)
                    {
                        if (s == "" || s == null)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.FK_Node);
                        Work subWK = subNd.GetWork(wl.WorkID);
                        subWK.Delete();

                        //删除分流下步骤的节点信息.
                        DeleteToNodesData(subNd.HisToNodes);
                    }

                    DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                    DBAccess.RunSQL("DELETE WF_GenerWorkerList WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                    DBAccess.RunSQL("DELETE WF_GenerFH WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                }
                DeleteToNodesData(nd.HisToNodes);
            }
        }
        private WorkNode DoReturnSubFlow(int backtoNodeID, string msg, bool isHiden)
        {
            Node nd = new Node(backtoNodeID);
            ps = new Paras();
            ps.SQL = "DELETE  FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID  AND FID=" + dbStr + "FID";
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FID", this.HisWork.FID);
            BP.DA.DBAccess.RunSQL(ps);

            // 找出分合流点处理的人员.
            ps = new Paras();
            ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "FID";
            ps.Add("FID", this.HisWork.FID);
            ps.Add("FK_Node", backtoNodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count != 1)
                throw new Exception("@ system error , this values must be =1");

            string FK_Emp = dt.Rows[0][0].ToString();
            // 获取当前工作的信息.
            GenerWorkerList wl = new GenerWorkerList(this.HisWork.FID, this.HisNode.NodeID, FK_Emp);
            Emp emp = new Emp(FK_Emp);

            // 改变部分属性让它适应新的数据,并显示一条新的待办工作让用户看到。
            wl.IsPass = false;
            wl.WorkID = this.HisWork.OID;
            wl.FID = this.HisWork.FID;
            wl.RDT = DataType.CurrentDataTime;
            wl.FK_Emp = FK_Emp;
            wl.FK_EmpText = emp.Name;

            wl.FK_Node = backtoNodeID;
            wl.FK_NodeText = nd.Name;
            wl.WarningDays = nd.WarningDays;
            wl.FK_Dept1 = emp.FK_Dept;

            DateTime dtNew = DateTime.Now;
            dtNew = dtNew.AddDays(nd.WarningDays);
            wl.SDT = dtNew.ToString(DataType.SysDataFormat); // DataType.CurrentDataTime;
            wl.FK_Flow = this.HisNode.FK_Flow;
            wl.Insert();

            GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.OID);
            gwf.FK_Node = backtoNodeID;
            gwf.NodeName = nd.Name;
            gwf.DirectUpdate();

            ps = new Paras();
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=3 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
            BP.DA.DBAccess.RunSQL(ps);

            /* 如果是隐性退回。*/
            BP.WF.ReturnWork rw = new ReturnWork();
            rw.WorkID = wl.WorkID;
            rw.ReturnToNode = wl.FK_Node;
            rw.ReturnNode = this.HisNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;
            rw.ReturnToEmp = FK_Emp;
            rw.Note = msg;
            try
            {
                rw.MyPK = rw.ReturnToNode + "_" + rw.WorkID + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                rw.Insert();
            }
            catch
            {
                rw.MyPK = rw.ReturnToNode + "_" + rw.WorkID + "_" + BP.DA.DBAccess.GenerOID();
                rw.Insert();
            }


            // 加入track.
            this.AddToTrack(ActionType.Return, FK_Emp, emp.Name, backtoNodeID, nd.Name, msg);

            WorkNode wn = new WorkNode(this.HisWork.FID, backtoNodeID);
            if (Glo.IsEnableSysMessage)
            {
                //  WF.Port.WFEmp wfemp = new Port.WFEmp(wn.HisWork.Rec);
                string title = string.Format("工作退回：流程:{0}.工作:{1},退回人:{2},需您处理",
                      wn.HisNode.FlowName, wn.HisNode.Name, WebUser.Name);

                BP.WF.Dev2Interface.Port_SendMail(wn.HisWork.Rec, title, msg,
                    "RESub" + backtoNodeID + "_" + this.WorkID, nd.FK_Flow, nd.NodeID, this.WorkID, this.FID);
            }
            return wn;
        }
    }
}
