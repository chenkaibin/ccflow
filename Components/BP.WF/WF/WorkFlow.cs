using System;
using BP.En;
using BP.Web;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// WF 的摘要说明。
    /// 工作流
    /// 这里包含了两个方面
    /// 工作的信息．
    /// 流程的信息．
    /// </summary>
    public class WorkFlow
    {
        //public string ToE(string no, string chName)
        //{
        //    return BP.Sys.Language.GetValByUserLang(no, chName);
        //}
        //public string ToEP1(string no, string chName, string v)
        //{
        //    return string.Format(BP.Sys.Language.GetValByUserLang(no, chName), v);
        //}
        //public string ToEP2(string no, string chName, string v, string v2)
        //{
        //    return string.Format(BP.Sys.Language.GetValByUserLang(no, chName), v, v2);
        //}

        #region 当前工作统计信息
        /// <summary>
        /// 正常范围的运行的个数。
        /// </summary>
        public static int NumOfRuning(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=0";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 进入警告期限的个数
        /// </summary>
        public static int NumOfAlert(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=1";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 逾期
        /// </summary>
        public static int NumOfTimeout(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=2";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        #endregion

        #region  权限管理
        /// <summary>
        /// 是不是能够作当前的工作。
        /// </summary>
        /// <param name="empId">工作人员ID</param>
        /// <returns>是不是能够作当前的工作</returns>
        public bool IsCanDoCurrentWork(string empId)
        {
            //return true;
            // 找到当前的工作节点
            WorkNode wn = this.GetCurrentWorkNode();

            // 判断是不是开始工作节点..
            if (wn.HisNode.IsStartNode)
            {
                // 从物理上判断是不是有这个权限。
                return WorkFlow.IsCanDoWorkCheckByEmpStation(wn.HisNode.NodeID, empId);
            }

            // 判断他的工作生成的工作者.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID, wn.HisNode.NodeID);
            if (gwls.Count == 0)
            {
                //return true;
                //throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:工作ID="+this.WorkID+",节点ID="+wn.HisNode.NodeID );
                throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:WorkID=" + this.WorkID + ",NodeID=" + wn.HisNode.NodeID);
            }

            foreach (GenerWorkerList en in gwls)
            {
                if (en.FK_Emp == empId)
                    return true;
            }
            return false;
        }
        #endregion

        #region 流程公共方法
        /// <summary>
        /// 执行驳回
        /// 应用场景:子流程向分合点驳回时
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="fk_node">被驳回的节点</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string DoReject(Int64 fid, int fk_node, string msg)
        {
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, fk_node);
            //if (i == 0)
            //    throw new Exception("系统错误，没有找到应该找到的数据。");

            i = wl.Delete();
            //if (i == 0)
            //    throw new Exception("系统错误，没有删除应该删除的数据。");

            wl = new GenerWorkerList();
            i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.IsPass, 3);

            //if (i == 0)
            //    throw new Exception("系统错误，想找到退回的原始起点没有找到。");

            Node nd = new Node(fk_node);
            // 更新当前流程管理表的设置当前的节点。
            DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET FK_Node=" + fk_node + ", NodeName='" + nd.Name + "' WHERE WorkID=" + this.WorkID);

            wl.RDT = DataType.CurrentDataTime;
            wl.IsPass = false;
            wl.Update();

            return "工作已经驳回到(" + wl.FK_Emp + " , " + wl.FK_EmpText + ")";
            // wl.HisNode
        }
        /// <summary>
        /// 逻辑删除流程
        /// </summary>
        /// <param name="msg">逻辑删除流程原因，可以为空。</param>
        public void DoDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                //设置产生的工作流程为.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = BP.WF.WFState.Delete;
                gwf.Update();

                //记录日志 感谢 itdos and 888 , 提出了这个bug.
                WorkNode wn = new WorkNode(WorkID, gwf.FK_Node);
                wn.AddToTrack(ActionType.DeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);

                string sql = "UPDATE  ND" + int.Parse(this.HisFlow.No) + "Rpt SET WFState=" + (int)WFState.Delete + " WHERE OID=" + this.WorkID;
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 恢复逻辑删除流程
        /// </summary>
        /// <param name="msg">回复原因,可以为空.</param>
        public void DoUnDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + " WHERE  WorkID=" + this.WorkID);

                //设置产生的工作流程为.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = BP.WF.WFState.Runing;
                gwf.Update();
              
                WorkNode wn = new WorkNode(WorkID, gwf.FK_Node);
                wn.AddToTrack(ActionType.UnDeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 删除已经完成的流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <param name="note">删除原因</param>
        /// <returns>删除信息</returns>
        public static string DoDeleteWorkFlowAlreadyComplete(string flowNo, Int64 workID, bool isDelSubFlow, string note)
        {
            Log.DebugWriteInfo("开始删除流程:流程编号:"+flowNo+"-WorkID:"+workID+"-"+". 是否要删除子流程:"+isDelSubFlow+";删除原因:"+note);

            Flow fl = new Flow(flowNo);

            #region 记录流程删除日志
            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt");
            rpt.SetValByKey(GERptAttr.OID, workID);
            rpt.Retrieve();
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = workID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDataTime;
                log.OperDept = WebUser.FK_Dept;
                log.OperDeptName = WebUser.FK_DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = note;
                log.OID = workID;
                log.FK_Flow = flowNo;
                log.FK_FlowSort = fl.FK_FlowSort;
                log.InsertAsOID(log.OID);
            }
            catch (Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                return ex.StackTrace;
            }
            #endregion 记录流程删除日志

            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workID);
            DBAccess.RunSQL("DELETE FROM " + fl.PTable + " WHERE OID=" + workID);
            DBAccess.RunSQL("DELETE WF_CHEval WHERE  WorkID=" + workID); // 删除质量考核数据。

            string info = "";

            #region 正常的删除信息.
            string msg = "";
            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workID);

                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + workID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + workID);
                // 删除移交.
                DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + workID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE  FID=" + workID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");

                //删除所有节点上的数据.
                Nodes nds = fl.HisNodes;
                foreach (Node nd in nds)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + workID + " OR FID=" + workID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }
                }
                if (msg != "")
                {
                    Log.DebugWriteInfo(msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程 Err " + ex.TargetSite;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.


            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.

            BP.DA.Log.DefaultLogWriteLineInfo("@[" + fl.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + workID + "]。");
            return "已经完成的流程被您删除成功.";
        }
        /// <summary>
        /// 彻底的删除流程
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns>删除的消息</returns>
        public string DoDeleteWorkFlowByReal(bool isDelSubFlow)
        {
            string info = "";
            WorkNode wn = this.GetCurrentWorkNode();

            // 处理删除前事件。
            wn.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.BeforeFlowDel, wn.HisWork);

            DBAccess.RunSQL("DELETE FROM ND"+int.Parse(this.HisFlow.No)+"Track WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM "+this.HisFlow.PTable+" WHERE OID=" + this.WorkID);
            DBAccess.RunSQL("DELETE WF_CHEval WHERE  WorkID=" + this.WorkID); // 删除质量考核数据。


            #region 正常的删除信息.
            BP.DA.Log.DefaultLogWriteLineInfo("@[" + this.HisFlow.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + this.WorkID + "]。");
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + this.WorkID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + this.WorkID);
                // 删除移交.
                DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE  FID=" + this.WorkID + " AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");

                //删除所有节点上的数据.
                Nodes nds = this.HisFlow.HisNodes;
                foreach (Node nd in nds)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + this.WorkID + " OR FID=" + this.WorkID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }
                }
                if (msg != "")
                {
                    Log.DebugWriteInfo(msg);
                    //throw new Exception("@已经从工作者列表里面清除了.删除节点信息其间出现错误:" + msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisStartWork.OID + "," + this.HisStartWork.Title + "] Err " + ex.Message;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (this.FID != 0)
            {
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */
                sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + wn.HisWork.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束");
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /*说明全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0  WHERE IsPass=3  AND WorkID=" + wn.HisWork.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.FK_Node);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            /* 解决删除最后一个子流程时要把干流程也要删除。*/
                            sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + wn.HisNode.NodeID + " AND FID=" + wn.HisWork.FID;
                            int num = DBAccess.RunSQLReturnValInt(sql);
                            if (num == 0)
                            {
                                /*说明没有子进程，就要把这个流程执行完成。*/
                                WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                                info += "@所有的子线程已经结束。";
                                info += "@结束主流程信息。";
                                info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束");
                            }
                            break;
                    }
                }
            }
            #endregion

            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.

            return info;
        }
        /// <summary>
        /// 删除工作流程记录日志，并保留运动轨迹.
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns></returns>
        public string DoDeleteWorkFlowByWriteLog(string info, bool isDelSubFlow)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(this.HisFlow.No) + "Rpt", this.WorkID);
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = this.WorkID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDataTime;
                log.OperDept = WebUser.FK_Dept;
                log.OperDeptName = WebUser.FK_DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = info;
                log.OID = this.WorkID;
                log.FK_Flow = this.HisFlow.No;
                log.InsertAsOID( log.OID);
                return DoDeleteWorkFlowByReal(isDelSubFlow);
            }
            catch(Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                return ex.StackTrace;
            }
        }

        #region 流程的强制终止\删除 或者恢复使用流程,
        /// <summary>
        /// 恢复流程.
        /// </summary>
        /// <param name="msg">回复流程的原因</param>
        public void DoComeBackWrokFlow(string msg)
        {
            try
            {
                //设置产生的工作流程为
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = WFState.Runing;
                gwf.DirectUpdate();

                // 增加消息 
                WorkNode wn = this.GetCurrentWorkNode();
                GenerWorkerLists wls = new GenerWorkerLists(wn.HisWork.OID, wn.HisNode.NodeID);
                if (wls.Count == 0)
                    throw new Exception("@恢复流程出现错误,产生的工作者列表");
                BP.WF.MsgsManager.AddMsgs(wls, "恢复的流程", wn.HisNode.Name, "回复的流程");
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@恢复流程出现错误." + ex.Message);
                throw new Exception("@恢复流程出现错误." + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 得到当前的进行中的工作。
        /// </summary>
        /// <returns></returns>		 
        public WorkNode GetCurrentWorkNode()
        {
            //if (this.IsComplete)
            //    throw new Exception("@工作流程[" + this.HisStartWork.Title + "],已经完成。");

            int currNodeID = 0;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                this.DoFlowOver(ActionType.FlowOver, "非正常结束，没有找到当前的流程记录。");
                throw new Exception("@" + string.Format("工作流程{0}已经完成。", this.HisStartWork.Title));
            }

            Node nd = new Node(gwf.FK_Node);
            Work work = nd.HisWork;
            work.OID = this.WorkID;
            work.NodeID = nd.NodeID;
            work.SetValByKey("FK_Dept", Web.WebUser.FK_Dept);
            if (work.RetrieveFromDBSources() == 0)
            {
                Log.DefaultLogWriteLineError("@WorkID=" + this.WorkID + ",FK_Node=" + gwf.FK_Node + ".不应该出现查询不出来工作."); // 没有找到当前的工作节点的数据，流程出现未知的异常。
                work.Rec = Web.WebUser.No;
                try
                {
                    work.Insert();
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("@没有找到当前的工作节点的数据，流程出现未知的异常" + ex.Message + ",不应该出现"); // 没有找到当前的工作节点的数据
                }
            }
            work.FID = gwf.FID;

            WorkNode wn = new WorkNode(work, nd);
            return wn;
        }
        /// <summary>
        /// 结束分流的节点
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string DoFlowOverFeiLiu(GenerWorkFlow gwf)
        {
            // 查询出来有少没有完成的流程。
            int i = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE FID=" + gwf.FID + " AND WFState!=1");
            switch (i)
            {
                case 0:
                    throw new Exception("@不应该的错误。");
                case 1:
                    BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow  WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);
                    BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);
                    BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FID=" + gwf.FID);

                    StartWork wk = this.HisFlow.HisStartNode.HisWork as StartWork;
                    wk.OID = gwf.FID;
                    wk.Update();

                    return "@当前的工作已经完成，该流程上所有的工作都已经完成。";
                default:
                    BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=1 WHERE WorkID=" + this.WorkID);
                    BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + this.WorkID);
                    return "@当前的工作已经完成。";
            }
        }
        /// <summary>
        /// 处理子流程完成.
        /// </summary>
        /// <returns></returns>
        public string DoFlowSubOver()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Node nd = new Node(gwf.FK_Node);

            DBAccess.RunSQL("DELETE WF_GenerWorkFlow   WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE WF_GenerWorkerlist WHERE WorkID=" + this.WorkID);

            string sql = "SELECT count(*) FROM WF_GenerWorkFlow WHERE  FID=" + this.FID;
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
            {
                /*说明这是最后一个*/
                WorkFlow wf = new WorkFlow(gwf.FK_Flow, this.FID);
                wf.DoFlowOver(ActionType.FlowOver, "子流程结束");
                return "@当前子流程已完成，主流程已完成。";
            }
            else
            {
                return "@当前子流程已完成，主流程还有(" + num + ")个子流程未完成。";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="at"></param>
        /// <param name="stopMsg"></param>
        /// <returns></returns>
        public string DoFlowOver(ActionType at, string stopMsg)
        {
            if (string.IsNullOrEmpty(stopMsg))
                stopMsg = "流程结束";

            string msg = "";
            if (this.IsMainFlow == false)
            {
                /* 处理子流程完成*/
                return this.DoFlowSubOver();
            }

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Node nd = new Node(gwf.FK_Node);
            

            //处理明细数据的copy问题。 首先检查：当前节点（最后节点）是否有明细表。
            MapDtls dtls = nd.MapData.MapDtls; // new MapDtls("ND" + nd.NodeID);
            int i = 0;
            foreach (MapDtl dtl in dtls)
            {
                i++;
                // 查询出该明细表中的数据。
                GEDtls dtlDatas = new GEDtls(dtl.No);
                dtlDatas.Retrieve(GEDtlAttr.RefPK, this.WorkID);

                GEDtl geDtl = null;
                try
                {
                    // 创建一个Rpt对象。
                    geDtl = new GEDtl("ND" + int.Parse(this.HisFlow.No) + "RptDtl" + i.ToString());
                    geDtl.ResetDefaultVal();
                }
                catch
                {
#warning 此处需要修复。
                    continue;
                }

                // 复制到指定的报表中。
                foreach (GEDtl dtlData in dtlDatas)
                {
                    //geDtl.ResetDefaultVal();
                    //try
                    //{
                    //    //geDtl.Copy(geRpt); // 复制主表的数据。
                    //    //geDtl.Copy(dtlData);
                    //    //geDtl.SetValByKey("FlowStarterDept", geRpt.GetValStrByKey("FK_Dept")); // 发起人部门.
                    //    //geDtl.SetValByKey("FlowStartRDT", geRpt.GetValStrByKey("RDT")); //发起时间。
                    //    //geDtl.Insert();
                    //}
                    //catch
                    //{
                    //    geDtl.Update();
                    //}
                }
            }
            this._IsComplete = 1;

            string dbstr=BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerFH WHERE FID=" + dbstr + "FID";
            ps.Add(GenerFHAttr.FID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 清除流程注册信息.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            // 清除工作者.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            //加入轨迹.
            WorkNode wn = new WorkNode(WorkID, gwf.FK_Node);
            wn.AddToTrack(at, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                    stopMsg);

            return msg;
        }
        /// <summary>
        /// 在分流上结束流程。
        /// </summary>
        /// <returns></returns>
        public string DoFlowOverBranch123_del(Node nd)
        {
            string sql = "";
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=1 WHERE WorkID=" + this.WorkID);

            string msg = "";
            // 判断流程中是否还没有没有完成的支流。
            sql = "SELECT COUNT(WORKID) FROM WF_GenerWorkFlow WHERE WFState!=1 AND FID=" + this.FID;

            DataTable dt = DBAccess.RunSQLReturnTable("SELECT Rec FROM ND" + nd.NodeID + " WHERE FID=" + this.FID);
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
            {


                /*整个流程都结束了*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.FID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.FID);

                /* 输出整个流程完成的信息，给当前的用户。*/
                msg += "@整体流程完全结束，有{" + dt.Rows.Count + "}个人员参与了分支流程，您是最后一个完成此工作的人员。@分支流程参与者名单如下：";
                foreach (DataRow dr in dt.Rows)
                {
                    msg += dr[0].ToString() + "、";
                }
                return msg;
                //   return "@整个流程完全结束。" + this.GenerFHStartWorkInfo();
            }
            else
            {
                /* 还有其它人员没有完成此工作。*/

                msg += "@您的工作已经完。@整体流程目前还没有完全结束，有{" + dt.Rows.Count + "}个人员参与了分支流程，名单如下：";
                foreach (DataRow dr in dt.Rows)
                {
                    msg += dr[0].ToString() + "、";
                }
                return msg;
            }
        }
        /// <summary>
        /// 在干流上结束流程
        /// </summary>
        /// <param name="nd">结束的节点</param>
        /// <returns>返回的信息</returns>
        public string DoFlowOverRiver(Node nd)
        {
            try
            {
                string msg = "";

                /* 更新开始节点的状态。*/
                //   DBAccess.RunSQL("UPDATE ND" + this.StartNodeID + " SET WFState=1 WHERE OID=" + this.WorkID);

                /*整个流程都结束了*/
                DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.WorkID + " OR WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.WorkID + " OR WorkID=" + this.WorkID);
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("@结束流程时间出现异常：" + ex.Message);
            }
        }
        /// <summary>
        /// 在干流上结束流程
        /// </summary>
        /// <param name="nd">结束的节点</param>
        /// <returns>返回的信息</returns>
        public string DoFlowOverRiver_bak(Node nd)
        {
            try
            {
                string msg = "";

                /* 更新开始节点的状态。*/
                //DBAccess.RunSQL("UPDATE ND" + this.StartNodeID + " SET WFState=1 WHERE OID=" + this.WorkID);

                /*整个流程都结束了*/
                DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.FID);
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("@结束流程时间出现异常：" + ex.Message);
            }

            //try
            //{
            //    string msg = "";
            //    /* 更新开始节点的状态。*/
            //    DBAccess.RunSQL("UPDATE ND" + this.StartNodeID + " SET WFState=1 WHERE OID=" + this.WorkID);
            //    /*整个流程都结束了*/
            //    DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FID=" + this.WorkID);
            //    DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.WorkID);
            //    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + this.FID);
            //    return msg;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("@结束流程时间出现异常：" + ex.Message);
            //}
        }
        public string GenerFHStartWorkInfo()
        {
            string msg = "";
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT Title,RDT,Rec,OID FROM ND" + this.StartNodeID + " WHERE FID=" + this.FID);
            switch (dt.Rows.Count)
            {
                case 0:
                    Node nd = new Node(this.StartNodeID);
                    throw new Exception("@没有找到他们开始节点的数据，流程异常。FID=" + this.FID + "，节点：" + nd.Name + "节点ID：" + nd.NodeID);
                case 1:
                    msg = string.Format("@发起人： {0}  日期：{1} 发起的流程 标题：{2} ，已经成功完成。",
                        dt.Rows[0]["Rec"].ToString(), dt.Rows[0]["RDT"].ToString(), dt.Rows[0]["Title"].ToString());
                    break;
                default:
                    msg = "@下列(" + dt.Rows.Count + ")位人员发起的流程已经完成。";
                    foreach (DataRow dr in dt.Rows)
                    {
                        msg += "<br>发起人：" + dr["Rec"] + " 发起日期：" + dr["RDT"] + " 标题：" + dr["Title"] + "<a href='./../../WF/WFRpt.aspx?WorkID=" + dr["OID"] + "&FK_Flow=" + this.HisFlow.No + "' target=_blank>详细...</a>";
                    }
                    break;
            }
            return msg;
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.HisFlow.No + "01");
            }
        }
        /// <summary>
        /// 正常的流程结束
        /// </summary>		 
        public string DoFlowOverPlane(Node nd)
        {

            // 设置开始节点的状态。
            StartWork sw = this.HisStartWorkNode.HisWork as StartWork;
            sw.OID = this.WorkID;
            //sw.Update("WFState", (int)sw.WFState);
            sw.Update("WFState", (int)WFState.Complete);

            //查询出来报表的数据（主表的数据），以供明细表复制。
            BP.Sys.GEEntity geRpt = new GEEntity("ND" + int.Parse(this.HisFlow.No) + "Rpt");
            geRpt.SetValByKey("OID", this.WorkID);
            geRpt.Retrieve();
            geRpt.SetValByKey("FK_NY", DataType.CurrentYearMonth);


            string emps = ",";
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue;
                emps += wl.FK_Emp + ",";
            }
            geRpt.SetValByKey("Emps", emps);
            geRpt.Update();

            //geRpt.Update("Emps", emps);
            //处理明细数据的copy问题。 首先检查：当前节点（最后节点）是否有明细表。

            MapDtls dtls = new MapDtls("ND" + nd.NodeID);
            int i = 0;
            foreach (MapDtl dtl in dtls)
            {
                i++;
                // 查询出该明细表中的数据。
                GEDtls dtlDatas = new GEDtls(dtl.No);
                dtlDatas.Retrieve(GEDtlAttr.RefPK, this.WorkID);

                // 创建一个Rpt对象。
                GEEntity geDtl = new GEEntity("ND" + int.Parse(this.HisFlow.No) + "RptDtl" + i.ToString());
                // 复制到指定的报表中。
                foreach (GEDtl dtlData in dtlDatas)
                {
                    geDtl.ResetDefaultVal();
                    try
                    {
                        geDtl.Copy(geRpt); // 复制主表的数据。
                        geDtl.Copy(dtlData);
                        geDtl.SetValByKey("FlowStarterDept", geRpt.GetValStrByKey("FK_Dept")); // 发起人部门.
                        geDtl.SetValByKey("FlowStartRDT", geRpt.GetValStrByKey("RDT")); //发起时间。
                        geDtl.Insert();
                    }
                    catch
                    {
                        geDtl.Update();
                    }
                }
            }
            this._IsComplete = 1;


            // 清除流程。
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + ")  AND FK_Flow='" + this.HisFlow.No + "'");

            // 清除其他的工作者。
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + ")  AND FK_Node IN (SELECT NodeId FROM WF_Node WHERE FK_Flow='" + this.HisFlow.No + "') ");
            return "";


            //// 修改流程汇总中的流程状态。
            //CHOfFlow chf = new CHOfFlow();
            //chf.WorkID = this.WorkID;
            //chf.Update("WFState", (int)sw.WFState);
            // +"@" + this.ToEP2("WF5", "工作流程{0},{1}任务完成。", this.HisFlow.Name, this.HisStartWork.Title);  // 工作流程[" + HisFlow.Name + "] [" + HisStartWork.Title + "]任务完成。;
        }
        /// <summary>
        ///  抄送到
        /// </summary>
        /// <param name="dt"></param>
        public string CCTo(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return "";

            string emps = "";
            string empsExt = "";

            string ip = "127.0.0.1";
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            if (addressList.Length > 1)
                ip = addressList[1].ToString();
            else
                ip = addressList[0].ToString();


            foreach (DataRow dr in dt.Rows)
            {
                string no = dr[0].ToString();
                emps += no + ",";

                if (Glo.IsShowUserNoOnly)
                    empsExt += no + "、";
                else
                    empsExt += no + "(" + dr[1] + ")、";
            }

            Paras pss = new Paras();
            pss.Add("Sender", Web.WebUser.No);
            pss.Add("Receivers", emps);
            pss.Add("Title", "工作流抄送：工作名称:" + this.HisFlow.Name + "，最后处理人：" + Web.WebUser.Name);
            pss.Add("Context", "工作报告 http://" + ip + "/WF/WFRpt.aspx?WorkID=" + this.WorkID + "&FID=0");

            try
            {
                DBAccess.RunSP("CCstaff", pss);
                return "@" + empsExt;
            }
            catch (Exception ex)
            {
                return "@抄送出现错误，没有把该流程的信息抄送到(" + empsExt + ")请联系管理员检查系统异常" + ex.Message;
            }
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 他的节点
        /// </summary>
        private Nodes _HisNodes = null;
        /// <summary>
        /// 节点s
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                if (this._HisNodes == null)
                    this._HisNodes = this.HisFlow.HisNodes;
                return this._HisNodes;
            }
        }
        /// <summary>
        /// 工作节点s(普通的工作节点)
        /// </summary>
        private WorkNodes _HisWorkNodesOfWorkID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfWorkID
        {
            get
            {
                if (this._HisWorkNodesOfWorkID == null)
                {
                    this._HisWorkNodesOfWorkID = new WorkNodes();
                    this._HisWorkNodesOfWorkID.GenerByWorkID(this.HisFlow, this.WorkID);
                }
                return this._HisWorkNodesOfWorkID;
            }
        }
        /// <summary>
        /// 工作节点s
        /// </summary>
        private WorkNodes _HisWorkNodesOfFID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfFID
        {
            get
            {
                if (this._HisWorkNodesOfFID == null)
                {
                    this._HisWorkNodesOfFID = new WorkNodes();
                    this._HisWorkNodesOfFID.GenerByFID(this.HisFlow, this.FID);
                }
                return this._HisWorkNodesOfFID;
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        private Flow _HisFlow = null;
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                return this._HisFlow;
            }
        }
        private GenerWorkFlow  _HisGenerWorkFlow=null;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow==null)
                 _HisGenerWorkFlow =new GenerWorkFlow(this.WorkID);
                return _HisGenerWorkFlow;
            }
            set
            {
                _HisGenerWorkFlow = value;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _WorkID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this._WorkID;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _FID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this._FID;
            }
        }
        /// <summary>
        /// 是否是干流
        /// </summary>
        public bool IsMainFlow
        {
            get
            {
                if (this.FID != 0 && this.FID != this.WorkID)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        #region 构造方法
        public WorkFlow(string fk_flow, Int64 wkid)
        {
            this.HisGenerWorkFlow = new GenerWorkFlow(wkid);

            this._FID = this.HisGenerWorkFlow.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            Flow flow = new Flow(fk_flow);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }

        public WorkFlow(Flow flow, Int64 wkid)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wkid;
            gwf.RetrieveFromDBSources();

            this._FID = gwf.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        /// <summary>
        /// 建立一个工作流事例
        /// </summary>
        /// <param name="flow">流程No</param>
        /// <param name="wkid">工作ID</param>
        public WorkFlow(Flow flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        public WorkFlow(string FK_flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;

            Flow flow = new Flow(FK_flow);
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        #endregion

        #region 公共属性

        /// <summary>
        /// 开始工作
        /// </summary>
        private StartWork _HisStartWork = null;
        /// <summary>
        /// 他开始的工作.
        /// </summary>
        public StartWork HisStartWork
        {
            get
            {
                if (_HisStartWork == null)
                {
                    StartWork en = (StartWork)this.HisFlow.HisStartNode.HisWork;
                    en.OID = this.WorkID;
                    en.FID = this.FID;
                    if (en.RetrieveFromDBSources() == 0)
                        en.RetrieveFID();
                    _HisStartWork = en;
                }
                return _HisStartWork;
            }
        }
        /// <summary>
        /// 开始工作节点
        /// </summary>
        private WorkNode _HisStartWorkNode = null;
        /// <summary>
        /// 他开始的工作.
        /// </summary>
        public WorkNode HisStartWorkNode
        {
            get
            {
                if (_HisStartWorkNode == null)
                {
                    Node nd = this.HisFlow.HisStartNode;
                    StartWork en = (StartWork)nd.HisWork;
                    en.OID = this.WorkID;
                    en.Retrieve();

                    WorkNode wn = new WorkNode(en, nd);
                    _HisStartWorkNode = wn;

                }
                return _HisStartWorkNode;
            }
        }
        #endregion

        #region 运算属性
        public int _IsComplete = -1;
        /// <summary>
        /// 是不是完成
        /// </summary>
        public bool IsComplete
        {
            get
            {
                if (_IsComplete == -1)
                {
                    bool s = !DBAccess.IsExits("select workid from WF_GenerWorkFlow WHERE WorkID=" + this.WorkID + " AND FK_Flow='" + this.HisFlow.No + "'");
                    if (s)
                        _IsComplete = 1;
                    else
                        _IsComplete = 0;
                }

                if (_IsComplete == 0)
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 是不是完成
        /// </summary>
        public string IsCompleteStr
        {
            get
            {
                if (this.IsComplete)
                    return "已";
                else
                    return "未";
            }
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="empId">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpStation(int nodeId, string empId)
        {
            bool isCan = false;
            // 判断岗位对应关系是不是能够执行.
            string sql = "SELECT a.FK_Node FROM WF_NodeStation a,  Port_EmpStation b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " AND b.FK_Emp='" + empId + "' )";
            isCan = DBAccess.IsExits(sql);
            if (isCan)
                return true;
            // 判断他的主要工作岗位能不能执行它.
            sql = "select FK_Node from WF_NodeStation WHERE FK_Node=" + nodeId + " AND ( FK_Station in (select FK_Station from Port_Empstation WHERE FK_Emp='" + empId + "') ) ";
            return DBAccess.IsExits(sql);
        }
        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="dutyNo">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpDuty(int nodeId, string dutyNo)
        {
            string sql = "SELECT a.FK_Node FROM WF_NodeDuty  a,  Port_EmpDuty b WHERE (a.FK_Duty=b.FK_Duty) AND (a.FK_Node=" + nodeId + " AND b.FK_Duty=" + dutyNo + ")";
            if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="DeptNo">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpDept(int nodeId, string DeptNo)
        {
            string sql = "SELECT a.FK_Node FROM WF_NodeDept  a,  Port_EmpDept b WHERE (a.FK_Dept=b.FK_Dept) AND (a.FK_Node=" + nodeId + " AND b.FK_Dept=" + DeptNo + ")";
            if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 在物理上能构作这项工作的人员。
        /// </summary>
        /// <param name="nodeId">节点ID</param>		 
        /// <returns></returns>
        public static DataTable CanDoWorkEmps(int nodeId)
        {
            string sql = "select a.FK_Node, b.EmpID from WF_NodeStation  a,  Port_EmpStation b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " )";
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// GetEmpsBy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Emps GetEmpsBy(DataTable dt)
        {
            // 形成能够处理这件事情的用户几何。
            Emps emps = new Emps();
            foreach (DataRow dr in dt.Rows)
            {
                emps.AddEntity(new Emp(dr["EmpID"].ToString()));
            }
            return emps;
        }

        #endregion

        #region 流程方法
        public string DoUnSendSubFlow(GenerWorkFlow gwf)
        {
            WorkNode wn = this.GetCurrentWorkNode();
            WorkNode wnPri = wn.GetPreviousWorkNode();

            GenerWorkerList wl = new GenerWorkerList();
            int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, Web.WebUser.No,
                GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID);
            if (num == 0)
                return "@您不能执行撤消发送，因为当前工作不是您发送的。";

            // 处理事件。
            string msg = wn.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneBefore, wn.HisWork);

            // 删除工作者。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node.ToString());

            if (this.HisFlow.HisDataStoreModel == DataStoreModel.ByCCFlow)
            wn.HisWork.Delete();

            gwf.FK_Node = wnPri.HisNode.NodeID;
            gwf.NodeName = wnPri.HisNode.Name;
            gwf.Update();

            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);
            ShiftWorks fws = new ShiftWorks();
            fws.Delete(ShiftWorkAttr.FK_Node, wn.HisNode.NodeID.ToString(), ShiftWorkAttr.WorkID, this.WorkID.ToString());

            #region 判断撤消的百分比条件的临界点条件
            if (wn.HisNode.PassRate != 0)
            {
                decimal all = (decimal)BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) NUM FROM dbo.WF_GenerWorkerList WHERE FID=" + this.FID + " AND FK_Node=" + wnPri.HisNode.NodeID);
                decimal ok = (decimal)BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) NUM FROM dbo.WF_GenerWorkerList WHERE FID=" + this.FID + " AND IsPass=1 AND FK_Node=" + wnPri.HisNode.NodeID);
                decimal rate = ok / all * 100;
                if (wn.HisNode.PassRate <= rate)
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.FID);
                else
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=3 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.FID);
            }
            #endregion

            // 处理事件。
            msg += wn.HisNode.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneAfter, wn.HisWork);

            // 记录日志..
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "无");

            if (wnPri.HisNode.IsStartNode)
            {
                if (Web.WebUser.IsWap)
                {
                    return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='/WF/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                }
                else
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='/WF/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                    else
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='/WF/Do.aspx?ActionType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                }
            }
            else
            {
                // 更新是否显示。
                DBAccess.RunSQL("UPDATE WF_ForwardWork SET IsRead=1 WHERE WORKID=" + this.WorkID + " AND FK_Node=" + wnPri.HisNode.NodeID);

                if (Web.WebUser.IsWap == false)
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return  "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                    else
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
                else
                {
                    return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
            }
        }
        private string _AppType = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string AppType
        {
            get
            {
                if (_AppType == null)
                {
                    if (BP.SystemConfig.IsBSsystem == false)
                    {
                        _AppType = "WF";
                    }
                    else
                    {
                        if (BP.Web.WebUser.IsWap)
                            _AppType = "WF/WAP";
                        else
                        {
                            bool b = System.Web.HttpContext.Current.Request.RawUrl.ToLower().Contains("oneflow");
                            if (b)
                                _AppType = "WF/OneFlow";
                            else
                                _AppType = "WF";
                        }
                    }
                }
                return _AppType;
            }
        }
        private string _VirPath = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string VirPath
        {
            get
            {
                if (_VirPath == null)
                {
                    if (BP.SystemConfig.IsBSsystem)
                        _VirPath = System.Web.HttpContext.Current.Request.ApplicationPath;
                    else
                        _VirPath = "";
                }
                return _VirPath;
            }
        }
        /// <summary>
        /// 执行挂起
        /// </summary>
        /// <param name="way">挂起方式</param>
        /// <param name="relData">释放日期</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns></returns>
        public string DoHungUp(HungUpWay way, string relData, string hungNote)
        {
            if (this.HisGenerWorkFlow.WFState == WFState.HungUp)
                throw new Exception("@当前已经是挂起的状态您不能执行在挂起.");

            if (string.IsNullOrEmpty(hungNote))
                hungNote = "无";

            if (way == HungUpWay.SpecDataRel)
                if (relData.Length < 10)
                    throw new Exception("@解除挂起的日期不正确(" + relData + ")");
            if (relData == null)
                relData = "";

            HungUp hu = new HungUp();
            hu.FK_Node = this.HisGenerWorkFlow.FK_Node;
            hu.WorkID = this.WorkID;
            hu.MyPK =  hu.FK_Node + "_" + hu.WorkID;
            hu.HungUpWay = way; //挂起方式.
            hu.DTOfHungUp = DataType.CurrentDataTime; // 挂起时间
            hu.Rec = BP.Web.WebUser.No;  //挂起人
            hu.DTOfUnHungUp = relData; // 解除挂起时间。
            hu.Note = hungNote;
            hu.Insert();

            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/Track.aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "挂起" + hungNote;
            string emps = "";
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue; //不发送给禁用的人。

                //BP.WF.Port.WFEmp emp = new Port.WFEmp(wl.FK_Emp);
                emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

                //写入消息。
              BP.WF.Dev2Interface.Port_SendMail(wl.FK_Emp, title, mailDoc,"HungUp"+wl.WorkID,wl.FK_Flow,wl.FK_Node,wl.WorkID,wl.FID);
            }

            /* 执行 WF_GenerWorkFlow 挂起. */
            int hungSta = (int)WFState.HungUp;
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, hungSta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, hungSta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET DTOfHungUp=" + dbstr + "DTOfHungUp,DTOfUnHungUp=" + dbstr + "DTOfUnHungUp, HungUpTimes=HungUpTimes+1 WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfHungUp, DataType.CurrentDataTime);
            ps.Add(GenerWorkerListAttr.DTOfUnHungUp, relData);

            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.FK_Node);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);
            wn.AddToTrack(ActionType.HungUp, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, hungNote);
            return "已经成功执行挂起,并且已经通知给:" + emps;
        }
        /// <summary>
        /// 取消挂起
        /// </summary>
        /// <returns></returns>
        public string DoUnHungUp()
        {
            if (this.HisGenerWorkFlow.WFState != WFState.HungUp)
                throw new Exception("@非挂起状态,您不能解除挂起.");

            /* 执行解除挂起. */
            int sta = (int)WFState.Runing;
            string dbstr = BP.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET  DTOfUnHungUp=" + dbstr + "DTOfUnHungUp WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfUnHungUp, DataType.CurrentDataTime);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.FK_Node);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            //更新 HungUp
            HungUp hu = new HungUp();
            hu.FK_Node = this.HisGenerWorkFlow.FK_Node;
            hu.WorkID = this.HisGenerWorkFlow.WorkID;
            hu.MyPK = hu.FK_Node + "_" + hu.WorkID;
            if (hu.RetrieveFromDBSources() == 0)
                throw new Exception("@系统错误，没有找到挂起点");

            hu.DTOfUnHungUp = DataType.CurrentDataTime; // 挂起时间
            hu.Update();

            //更新他的主键。
            ps = new Paras();
            ps.SQL = "UPDATE WF_HungUp SET MyPK=" + SystemConfig.AppCenterDBVarStr + "MyPK WHERE MyPK=" + dbstr + "MyPK1";
            ps.Add("MyPK",BP.DA.DBAccess.GenerGUID());
            ps.Add("MyPK1",hu.MyPK);
            DBAccess.RunSQL(ps);


            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/Track.aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "解除挂起.";
            string emps = "";
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue; //不发送给禁用的人。

                emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

                //写入消息。
                BP.WF.Dev2Interface.Port_SendMail(wl.FK_Emp, title, mailDoc,
                    "HungUp" + wl.FK_Node + this.WorkID, HisGenerWorkFlow.FK_Flow, HisGenerWorkFlow.FK_Node, this.WorkID, this.FID);

                //写入消息。
                //Glo.SendMsg(wl.FK_Emp, title, mailDoc);
            }


            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);
            wn.AddToTrack(ActionType.UnHungUp, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "解除挂起,已经通知给:" + emps);
            return null;
        }
        /// <summary>
        /// 撤消移交
        /// </summary>
        /// <returns></returns>
        public string DoUnShift()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            if (wls.Count == 0)
                return "移交失败没有当前的工作。";

            Node nd = new Node(gwf.FK_Node);
            Work wk1 = nd.HisWork;
            wk1.OID = this.WorkID;
            wk1.Retrieve();

            // 记录日志.
            WorkNode wn = new WorkNode(wk1, nd);
            wn.AddToTrack(ActionType.UnShift, WebUser.No, WebUser.Name, nd.NodeID, nd.Name, "撤消移交");

            if (wls.Count == 1)
            {
                GenerWorkerList wl = (GenerWorkerList)wls[0];
                wl.FK_Emp = WebUser.No;
                wl.FK_EmpText = WebUser.Name;
                wl.IsEnable = true;
                wl.IsPass = false;
                wl.Update();
                return "@撤消移交成功，<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>";
            }

            bool isHaveMe = false;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FK_Emp == WebUser.No)
                {
                    wl.FK_Emp = WebUser.No;
                    wl.FK_EmpText = WebUser.Name;
                    wl.IsEnable = true;
                    wl.IsPass = false;
                    wl.Update();
                    return "@撤消移交成功，<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>";
                }
            }

            GenerWorkerList wk = (GenerWorkerList)wls[0];
            GenerWorkerList wkNew = new GenerWorkerList();
            wkNew.Copy(wk);
            wkNew.FK_Emp = WebUser.No;
            wkNew.FK_EmpText = WebUser.Name;
            wkNew.IsEnable = true;
            wkNew.IsPass = false;
            wkNew.Insert();

            return "@撤消移交成功，<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>";
        }
        /// <summary>
        /// 执行撤消
        /// </summary>
        public string DoUnSend()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            // 如果停留的节点是分合流。
            Node nd = new Node(gwf.FK_Node);
            switch (nd.HisNodeWorkType)
            {
                case NodeWorkType.WorkFHL:
                    throw new Exception("分合流点不允许撤消。");
                case NodeWorkType.WorkFL:
                    /*到达了分流点, 有两种情况1，未处理过。 2，已经处理过了.
                     *  这两种情况处理的方式不同的。
                     *  未处理的直接通过正常的模式退回。
                     *  已经处理过的要杀掉所有的已经发起的进程。
                     */
                    DataTable mydt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_GenerWorkerList WHERE FK_Node=" + nd.NodeID + " AND WorkID=" + this.WorkID + "  AND IsPass=1");
                    if (mydt.Rows.Count >= 1)
                        return this.DoUnSendFeiLiu(gwf);
                    break;
                case NodeWorkType.StartWorkFL:
                    return this.DoUnSendFeiLiu(gwf);
                case NodeWorkType.WorkHL:
                    if (this.IsMainFlow)
                    {
                        /* 首先找到与他最近的一个分流点，并且判断当前的操作员是不是分流点上的工作人员。*/
                        return this.DoUnSendHeiLiu_Main(gwf);
                    }
                    else
                    {
                        return this.DoUnSendSubFlow(gwf); //是子流程时.
                        //return this.DoUnSendSubFlow(gwf); //是子流程时.
                    }
                    break;
                case NodeWorkType.SubThreadWork:
                    break;
                default:
                    break;
            }

            if (nd.IsStartNode)
                return "您不能撤消发送，因为它是开始节点。";

            WorkNode wn = this.GetCurrentWorkNode();
            WorkNode wnPri = wn.GetPreviousWorkNode();
            GenerWorkerList wl = new GenerWorkerList();
            int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, Web.WebUser.No,
                GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID);

            if (num == 0)
                return "@您不能执行撤消发送，因为当前工作不是您发送的。";

            // 调用撤消发送前事件。
            string msg = nd.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneBefore, wn.HisWork);

            #region 删除当前节点数据。
            // 删除产生的工作列表。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node.ToString());

            // 删除工作信息,如果是按照ccflow格式存储的。
            if (this.HisFlow.HisDataStoreModel== DataStoreModel.ByCCFlow)
            wn.HisWork.Delete();

            // 删除附件信息。
            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + gwf.FK_Node + "' AND RefPKVal='" + this.WorkID + "'");
            #endregion 删除当前节点数据。

            // 更新.
            gwf.FK_Node = wnPri.HisNode.NodeID;
            gwf.NodeName = wnPri.HisNode.Name;
            gwf.Update();
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);

            // 记录日志..
            wnPri.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, wnPri.HisNode.NodeID, wnPri.HisNode.Name, "无");

            // 删除数据.
            if (wn.HisNode.IsStartNode)
            {
                DBAccess.RunSQL("DELETE WF_GenerFH WHERE FID=" + this.WorkID);
                DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + nd.NodeID);
            }

            if (wn.HisNode.IsEval)
            {
                /*如果是质量考核节点，并且撤销了。*/
                DBAccess.RunSQL("DELETE WF_CHEval WHERE FK_Node=" + wn.HisNode.NodeID+" AND WorkID="+this.WorkID);
            }

            #region 恢复工作轨迹，解决工作抢办。
            if (wnPri.HisNode.IsStartNode == false)
            {
                WorkNode ppPri = wnPri.GetPreviousWorkNode();
                wl = new GenerWorkerList();
                wl.Retrieve(GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);
                // BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + backtoNodeID + " AND WorkID=" + this.WorkID);
                RememberMe rm = new RememberMe();
                rm.Retrieve(RememberMeAttr.FK_Node, wnPri.HisNode.NodeID, RememberMeAttr.FK_Emp, ppPri.HisWork.Rec);

                string[] empStrs = rm.Objs.Split('@');
                foreach (string s in empStrs)
                {
                    if (s == "" || s == null)
                        continue;

                    if (s == wl.FK_Emp)
                        continue;
                    GenerWorkerList wlN = new GenerWorkerList();
                    wlN.Copy(wl);
                    wlN.FK_Emp = s;

                    WF.Port.WFEmp myEmp = new Port.WFEmp(s);
                    wlN.FK_EmpText = myEmp.Name;

                    wlN.Insert();
                }
            }
            #endregion 恢复工作轨迹，解决工作抢办。


            #region 如果是开始节点, 检查此流程是否有子线程，如果有则删除它们。
            if (nd.IsStartNode)
            {
                /*要检查一个是否有 子流程，如果有，则删除它们。*/
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

                if (gwfs.Count > 0)
                {
                    foreach (GenerWorkFlow item in gwfs)
                    {
                        /*删除每个子线程.*/
                        BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
                    }
                }
            }
            #endregion


            //调用撤消发送后事件。
            msg += nd.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneAfter, wn.HisWork);

            if (wnPri.HisNode.IsStartNode)
            {
                if (Web.WebUser.IsWap)
                {
                    if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                        return "@撤消发送执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='/WF/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                    else
                        return "@撤销成功." + msg;
                }
                else
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                    {
                        if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                            return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='/WF/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                        else
                            return  "撤销成功.";
                    }
                    else
                    {
                        if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                            return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='" + this.VirPath + this.AppType + "/Do.aspx?ActionType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                        else
                            return "撤销成功" + msg;
                    }
                }
            }
            else
            {
                // 更新是否显示。
                DBAccess.RunSQL("UPDATE WF_ForwardWork SET IsRead=1 WHERE WORKID=" + this.WorkID + " AND FK_Node=" + wnPri.HisNode.NodeID);
                if (Web.WebUser.IsWap == false)
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return   "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                    else
                        return   "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
                else
                {
                    return   "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
            }
        }
        /// <summary>
        /// 撤消分流点
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private string DoUnSendFeiLiu(GenerWorkFlow gwf)
        {
            string sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID + " AND FK_Emp='" + Web.WebUser.No + "' AND FK_Node='" + gwf.FK_Node + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return "@您不能执行撤消发送，因为当前工作不是您发送的。";

            //处理事件.
            Node nd = new Node(gwf.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = gwf.WorkID;
            wk.RetrieveFromDBSources();
            string msg = nd.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneBefore, wk);

            // 记录日志..
            WorkNode wn = new WorkNode(wk, nd);
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, "");


            // 删除分合流记录。
            if (nd.IsStartNode)
            {
                DBAccess.RunSQL("DELETE WF_GenerFH WHERE FID=" + this.WorkID);
                DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + nd.NodeID);
            }


            //删除上一个节点的数据。
            foreach (Node ndNext in nd.HisToNodes)
            {
                int i = DBAccess.RunSQL("DELETE WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + ndNext.NodeID);
                if (i == 0)
                    continue;

                // 删除工作记录。
                Works wks = ndNext.HisWorks;
                if (this.HisFlow.HisDataStoreModel == DataStoreModel.ByCCFlow)
                    wks.Delete(GenerWorkerListAttr.FID, this.WorkID);

                // 删除已经发起的流程。
                DBAccess.RunSQL("DELETE WF_GenerWorkFlow WHERE FID=" + this.WorkID + " AND FK_Node=" + ndNext.NodeID);
            }

            //设置当前节点。
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=1");
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerFH SET FK_Node=" + gwf.FK_Node + " WHERE FID=" + this.WorkID);


            // 设置当前节点的状态.
            Node cNode = new Node(gwf.FK_Node);
            Work cWork = cNode.HisWork;
            cWork.OID = this.WorkID;

            msg += nd.MapData.FrmEvents.DoEventNode(EventListOfNode.UndoneAfter, wk);

            if (cNode.IsStartNode)
            {
                if (Web.WebUser.IsWap)
                {
                    return   "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + cWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                }
                else
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return  "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "' ><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + cWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                    else
                        return  "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "' ><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='Do.aspx?ActionType=DeleteFlow&WorkID=" + cWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。" + msg;
                }
            }
            else
            {
                // 更新是否显示。
                DBAccess.RunSQL("UPDATE WF_ForwardWork SET IsRead=1 WHERE WORKID=" + this.WorkID + " AND FK_Node=" + cNode.NodeID);
                if (Web.WebUser.IsWap == false)
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "' ><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                    else
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "' ><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
                else
                {
                    return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=0&FK_Node=" + gwf.FK_Node + "' ><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。" + msg;
                }
            }
        }
        /// <summary>
        /// 执行撤销发送
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        public string DoUnSendHeiLiu_Main(GenerWorkFlow gwf)
        {
            Node currNode = new Node(gwf.FK_Node);
            Node priFLNode = currNode.HisPriFLNode;
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FK_Node, priFLNode.NodeID, GenerWorkerListAttr.FK_Emp, Web.WebUser.No);
            if (i == 0)
                return "@不是您把工作发送到当前节点上，所以您不能撤消。";

            WorkNode wn = this.GetCurrentWorkNode();
            WorkNode wnPri = new WorkNode(this.WorkID, priFLNode.NodeID);

            // 记录日志..
            wnPri.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, wnPri.HisNode.NodeID, wnPri.HisNode.Name, "无");

            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node.ToString());

            if (this.HisFlow.HisDataStoreModel == DataStoreModel.ByCCFlow)
            wn.HisWork.Delete();

            gwf.FK_Node = wnPri.HisNode.NodeID;
            gwf.NodeName = wnPri.HisNode.Name;
            gwf.Update();

            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerFH SET FK_Node=" + gwf.FK_Node + " WHERE FID=" + this.WorkID);

            ShiftWorks fws = new ShiftWorks();
            fws.Delete(ShiftWorkAttr.FK_Node, wn.HisNode.NodeID.ToString(),
                ShiftWorkAttr.WorkID, this.WorkID.ToString());

            //ReturnWorks rws = new ReturnWorks();
            //rws.Delete(ReturnWorkAttr.FK_Node, wn.HisNode.NodeID.ToString(),
            //    ReturnWorkAttr.WorkID, this.WorkID.ToString());

            #region 恢复工作轨迹，解决工作抢办。
            if (wnPri.HisNode.IsStartNode == false)
            {
                WorkNode ppPri = wnPri.GetPreviousWorkNode();
                wl = new GenerWorkerList();
                wl.Retrieve(GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);
                // BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + backtoNodeID + " AND WorkID=" + this.WorkID);
                RememberMe rm = new RememberMe();
                rm.Retrieve(RememberMeAttr.FK_Node, wnPri.HisNode.NodeID, RememberMeAttr.FK_Emp, ppPri.HisWork.Rec);

                string[] empStrs = rm.Objs.Split('@');
                foreach (string s in empStrs)
                {
                    if (s == "" || s == null)
                        continue;

                    if (s == wl.FK_Emp)
                        continue;
                    GenerWorkerList wlN = new GenerWorkerList();
                    wlN.Copy(wl);
                    wlN.FK_Emp = s;

                    WF.Port.WFEmp myEmp = new Port.WFEmp(s);
                    wlN.FK_EmpText = myEmp.Name;

                    wlN.Insert();
                }
            }
            #endregion 恢复工作轨迹，解决工作抢办。

            // 删除以前的节点数据.
            wnPri.DeleteToNodesData(priFLNode.HisToNodes);

            if (wnPri.HisNode.IsStartNode)
            {
                if (Web.WebUser.IsWap)
                {
                    if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='" + this.VirPath + this.AppType + "/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。";
                    else
                        return "撤销成功.";
                }
                else
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                    {
                        if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                            return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='" + this.VirPath + this.AppType + "/MyFlowInfo" + Glo.FromPageType + ".aspx?DoType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。";
                        else
                            return "撤销成功.";
                    }
                    else
                    {
                        if (wnPri.HisNode.HisFormType != FormType.SDKForm)
                            return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A> , <a href='" + this.VirPath + this.AppType + "/Do.aspx?ActionType=DeleteFlow&WorkID=" + wn.HisWork.OID + "&FK_Flow=" + this.HisFlow.No + "' /><img src='/WF/Img/Btn/Delete.gif' border=0/>此流程已经完成(删除它)</a>。";
                        else
                            return "撤销成功.";
                    }
                }
            }
            else
            {
                // 更新是否显示。
                DBAccess.RunSQL("UPDATE WF_ForwardWork SET IsRead=1 WHERE WORKID=" + this.WorkID + " AND FK_Node=" + wnPri.HisNode.NodeID);
                if (Web.WebUser.IsWap == false)
                {
                    if (this.HisFlow.FK_FlowSort != "00")
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。";
                    else
                        return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。";
                }
                else
                {
                    return "@撤消执行成功，您可以点这里<a href='" + this.VirPath + this.AppType + "/MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FK_Node=" + gwf.FK_Node + "'><img src='/WF/Img/Btn/Do.gif' border=0/>执行工作</A>。";
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 工作流程集合.
    /// </summary>
    public class WorkFlows : CollectionBase
    {
        #region 构造
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="flow">流程编号</param>
        public WorkFlows(Flow flow)
        {
            StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            ens.RetrieveAll(10000);
            foreach (StartWork sw in ens)
            {
                this.Add(new WorkFlow(flow, sw.OID, sw.FID));
            }
        }
        /// <summary>
        /// 工作流程集合
        /// </summary>
        public WorkFlows()
        {
        }
        /// <summary>
        /// 工作流程集合
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="flowState">工作ID</param> 
        public WorkFlows(Flow flow, int flowState)
        {
            //StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            //QueryObject qo = new QueryObject(ens);
            //qo.AddWhere(StartWorkAttr.WFState, flowState);
            //qo.DoQuery();
            //foreach (StartWork sw in ens)
            //{
            //    this.Add(new WorkFlow(flow, sw.OID, sw.FID));
            //}
        }

        #endregion

        #region 查询方法
        /// <summary>
        /// GetNotCompleteNode
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>StartWorks</returns>
        public static StartWorks GetNotCompleteWork(string flowNo)
        {
            return null;

            //Flow flow = new Flow(flowNo);
            //StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            //QueryObject qo = new QueryObject(ens);
            //qo.AddWhere(StartWorkAttr.WFState, "!=", 1);
            //qo.DoQuery();
            //return ens;

            /*
            foreach(StartWork sw in ens)
            {
                ens.AddEntity( new WorkFlow( flow, sw.OID) ) ; 
            }
            */
        }
        #endregion

        #region 方法
        /// <summary>
        /// 增加一个工作流程
        /// </summary>
        /// <param name="wn">工作流程</param>
        public void Add(WorkFlow wn)
        {
            this.InnerList.Add(wn);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public WorkFlow this[int index]
        {
            get
            {
                return (WorkFlow)this.InnerList[index];
            }
        }
        #endregion

        #region 关于调度的自动方法
        /// <summary>
        /// 清除死节点。
        /// 死节点的产生，就是用户非法的操作，或者系统出现存储故障，造成的流程中的当前工作节点没有工作人员，从而不能正常的运行下去。
        /// 清除死节点，就是把他们放到死节点工作集合里面。
        /// </summary>
        /// <returns></returns>
        public static string ClearBadWorkNode()
        {
            string infoMsg = "清除死节点的信息：";
            string errMsg = "清除死节点的错误信息：";
            return infoMsg + errMsg;
        }
        #endregion
    }
}
